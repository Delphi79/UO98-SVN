using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using Irony.Parsing;
using JoinUO.UOdemoSDK;
using JoinUO.UOSL.Service;
using JoinUO.UOSL.Service.ASTNodes;
using Microsoft.VisualStudio.Text;
using System.Diagnostics;

namespace JoinUO.UOSL.Package.MEF
{
    internal interface INodeProviderBroker
    {
        NodeProvider GetNodeProvider(ITextBuffer buffer);
    }

    [Export(typeof(INodeProviderBroker))]
    internal class NodeProviderBroker : INodeProviderBroker
    {
        //the real parser
        Parser parser;

        public NodeProvider GetNodeProvider(ITextBuffer buffer)
        {
            NodeProvider provider;
            if (!buffer.Properties.TryGetProperty(typeof(NodeProvider), out provider))
            {
                LanguageOption options;
                if (!buffer.Properties.TryGetProperty(typeof(LanguageOption), out options))
                {
                    if (buffer.CurrentSnapshot.LineCount > 0)
                    { // Determine language
                        // try to determine from comments
                        if(!Utils.TryGetLanguageDeclaration(buffer.CurrentSnapshot.GetLineFromLineNumber(0).GetText(),out options))
                        {// try to determine from file extension
                            ITextDocument doc;
                            if (buffer.Properties.TryGetProperty(typeof(ITextDocument), out doc))
                            {
                                if (doc.FilePath.EndsWith(".uosl"))
                                    options = LanguageOption.Extended;
                                else
                                    options = LanguageOption.Enhanced;
                            }
                        }
                    }

                    buffer.Properties.AddProperty(typeof(LanguageOption), options);

                }
                parser = new Parser(UOSL.Service.UOSLBase.GetGrammar(options));
                buffer.Properties.AddProperty(typeof(NodeProvider), provider = new NodeProvider(parser, buffer));
            }
            return provider;
        }

    }

    class NodeProvider : IDisposable
    {
        public ParseTree LastValidTree=null;

        public IEnumerable<Events.EventDefinition> Triggers
        {
            get
            {
                return UOdemoSDK.Events.SupportedEvents;
            }
        }

        public IEnumerable<Method> Funcs
        {
            get
            {
                if (LastValidTree == null) return null;
                return ((ScopedNode)LastValidTree.Root.AstNode).TreeFuncs;
            }

        }

        public IEnumerable<Method> CoreFuncs
        {
            get
            {
                return CoreCommands.Funcs;

                // Below is preserved because CoreCommands really should be scoped, but currently they are not.
                //if (LastValidTree == null) return null;
                //return ((ScopedNode)LastValidTree.Root.AstNode).TreeFuncs; 
            }

        }

        const int PARSING_DELAY = 1500; //ms

        private List<Node> nodes = new List<Node>();
        private object node_lock = new object();
        private Parser parser;
        private ITextBuffer buffer;

        Timer parserTimer;

        private bool m_isDisposed;
        public void Dispose()
        {
            if (!m_isDisposed)
            {
                if (parserTimer != null)
                    parserTimer.Dispose();
                parserTimer = null;

                GC.SuppressFinalize(this);
                m_isDisposed = true;
            }
        }

        public NodeProvider(Parser parser, ITextBuffer buffer)
        {
            this.parser = parser;
            this.buffer = buffer;
            rebuildNodes(buffer.CurrentSnapshot);
            buffer.Changed+=new EventHandler<TextContentChangedEventArgs>(buffer_Changed);
            parserTimer = new Timer(rebuildNodes, buffer.CurrentSnapshot, 0, Timeout.Infinite);
        }

        public delegate void SnapshotEvent(SnapshotSpan snapshotSpan);

        void buffer_Changed(object sender, TextContentChangedEventArgs e)
        {
            // shut down the old one
            parserTimer.Dispose();

            // put the call to the rebuildNodes on timer
            parserTimer = new Timer(rebuildNodes, e.After, PARSING_DELAY, Timeout.Infinite);
        }

        private void rebuildNodes(object state)
        {
            ITextSnapshot snapshot = (ITextSnapshot)state;
            ThreadPool.QueueUserWorkItem(rebuildNodesAsynch, snapshot);
        }

        public event SnapshotEvent NodesChanged;

        private void rebuildNodesAsynch(object snapshotObject)
        {
            ITextSnapshot snapshot = (ITextSnapshot)snapshotObject;
            ParseTree tree;
            try
            {
                lock (IronyLanguageService.lock_parser)
                {
                    tree = parser.Parse(snapshot.GetText(), (string)snapshot.TextBuffer.Properties.GetProperty<ITextDocument>(typeof(ITextDocument)).FilePath);
                }
            }
            catch (Exception ex)
            {
                Debug.Print("An exception occurred parsing source: {0}", ex.Message);
                return;
            }
            if (tree.Root == null || tree.Root.AstNode == null) return;

            lock (node_lock)
            {
                this.LastValidTree = tree;
            }

            var nodes = ((Irony.Ast.AstNode)tree.Root.AstNode).ChildNodes;  // root is "Script", omitted.
            List<Node> new_nodes = nodes.Aggregate(new List<Node>(), (list, node) => { list.Add(new Node(this, null, snapshot, node)); return list; });

            var comments = tree.Tokens.Where(token => token.Terminal is CommentTerminal);
            List<Node> comment_nodes = comments.Aggregate(new List<Node>(), (list, token) => { list.Add(new Node(this, null, snapshot, token)); return list; });

            new_nodes.AddRange(comment_nodes);

            List<Node> oldNodes;
            lock (node_lock)
            {
                oldNodes = this.nodes;
                this.nodes = new_nodes;
            }
            oldNodes.ForEach(node => node.Dispose());
            RaiseNodesChanged(snapshot);
        }

        /// <summary>
        /// Raises the <see cref="NodesChanged"/> event
        /// </summary>
        /// <param name="snapshot"></param>
        internal void RaiseNodesChanged(ITextSnapshot snapshot)
        {
            if (NodesChanged != null)
                NodesChanged(new SnapshotSpan(snapshot, 0, snapshot.Length));
        }

        /// <summary>
        /// Returns a list of nodes in the specified snapshot span
        /// </summary>
        /// <param name="snapshotSpan"></param>
        /// <returns></returns>
        internal List<Node> GetNodes(ITextSnapshot snapshot)
        {
            List<Node> nodes;
            lock (node_lock)
            {
                nodes = this.nodes;

                // just in case if while the tokens list was being rebuilt
                // another modification was made
                if (nodes.Count > 0 && this.nodes[0].SnapshotSpan.Snapshot != snapshot)
                    this.nodes.ForEach(node => node.TranslateTo(snapshot));
            }

            return nodes;
        }

        /// <summary>
        /// Returns a list of syntax nodes in the specified snapshot span
        /// </summary>
        /// <param name="snapshotSpan"></param>
        /// <param name="predicate">the predicate controlling what nodes to include in the list</param>
        /// <returns></returns>
        internal List<Node> GetNodes(SnapshotSpan snapshotSpan, Predicate<Node> predicate)
        {
            return GetNodes(snapshotSpan, GetNodes(snapshotSpan.Snapshot))
                .FindAll(predicate);
        }

        /// <summary>
        /// Walks the syntax node tree building a flat list of nodes intersecting with the span
        /// </summary>
        /// <param name="snapshotSpan"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private List<Node> GetNodes(SnapshotSpan snapshotSpan, IEnumerable<Node> nodes)
        {
            List<Node> result = new List<Node>();
            foreach (Node node in nodes)
            {
                if (node.SnapshotSpan.IntersectsWith(snapshotSpan) || node.ExtensionSpan.IntersectsWith(snapshotSpan))
                    result.Add(node);
                result.AddRange(GetNodes(snapshotSpan, node.Children));
            }
            return result;
        }

        /// <summary>
        /// Returns a list of syntax nodes based on the point in the text buffer
        /// </summary>
        /// <param name="point">point identifying the desired node</param>
        /// <param name="predicate">the predicate controlling what nodes to include in the list</param>
        /// <returns></returns>
        internal List<Node> GetNodes(SnapshotPoint point, Predicate<Node> predicate)
        {
            return GetNodes(new SnapshotSpan(point.Snapshot, point.Position, 0), predicate);
        }

        public IEnumerable<Field> GetFields(SnapshotPoint point)
        {
            Node anode = GetMostSpecificNode(point);
            ScopedNode node;
            if (anode ==null || (node = anode.astnode as ScopedNode) == null) return null;
            return node.ScopeVars;
        }

        internal bool isComment(SnapshotPoint point)
        {
            return GetNodes(new SnapshotSpan(point.Snapshot, point.Position, 0), node => node.astnode == null).FirstOrDefault() != null;
        }

        internal Node GetMostSpecificNode(SnapshotPoint point)
        {
            return GetNodes(new SnapshotSpan(point.Snapshot, point.Position, 0), node => node.astnode is ScopedNode).OrderBy(node => node.Position).LastOrDefault();
        }
    }
}
