using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using System.IO;
using JoinUO.UOSL.Service.ASTNodes;
using JoinUO.UOdemoSDK;

namespace JoinUO.UOSL.Service
{
    class UoCoreFunctionNode : FunctionProtoNode
    {
        //protected override string Label { get { return "Extern: "; } }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            return string.Empty;
            //throw new NotImplementedException("Script generation for core function imports is not implemented.");
        }

        public override UoToken UoTypeToken
        {
            get
            {
                if (TypeToken != null/*any*/ && TypeToken.KeyTerm is Tokenizer)
                {
                    return ((Tokenizer)TypeToken.KeyTerm).Tokenize();
                }
                else
                    return null;
            }
        }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
        }
    }

    [CLSCompliant(false)]
    public class CommandFileParser : UOSLBase
    {
        public void CreateFuncsNode(ParsingContext context, ParseTreeNode parseNode)
        {
            ScopedNode node = new ScopedNode();
            node.Init(context, parseNode);
            parseNode.AstNode = node;

            foreach(ParseTreeNode cnode in parseNode.ChildNodes)
            {
                ((ScopedNode)cnode.AstNode).Parent = node;
                node.ChildNodes.Add((ScopedNode)cnode.AstNode);
            }
        }

        public CommandFileParser() : base()
        {
            NonTerminal Funcs = new NonTerminal("Funcs", CreateFuncsNode);
            
            Funcs.Rule = MakeStarRule(Funcs, null, ExternalDeclaration);
            Root = Funcs;

        }
    }

    [CLSCompliant(false)]
    public class CoreCommands
    {
        public static DateTime CoreFileDate = DateTime.MinValue;
        private static HashSet<Method> m_CoreCommands = null;
        public static List<ParserMessage> CoreErrors = new List<ParserMessage>();
        public static HashSet<Method> Funcs { get { return m_CoreCommands; } }

        private static CommandFileParser m_CommandParser=null;
        private static CommandFileParser CommandParser { get { return m_CommandParser ?? (m_CommandParser = new CommandFileParser()); } }

        // call this in declarations
        static private bool Loaded = false;
        internal static void LoadCoreCommands(ParsingContext context)
        {
            if (Loaded)
                return;
            string path = "Externals.txt";
            string fullpath = Utils.FindFile(path, Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            ParseTree tree = null;
            if (File.Exists(fullpath))
            {
                Loaded = true;

                FileInfo fi = new FileInfo(fullpath);
                if (fi.LastWriteTime > CoreFileDate)
                {
                    ParsingContext subcontext = new ParsingContext(new Parser(CommandParser));

                    using (StreamReader reader = new StreamReader(fullpath))
                    {
                        tree = subcontext.Parser.Parse(reader.ReadToEnd(), fullpath);
                    }

                    CoreErrors.Clear();

                    if (tree == null)
                    {
                        m_CoreCommands = null;
                        CoreErrors.Add(new ParserMessage(ParserErrorLevel.Warning, SourceLocation.Empty, string.Format("CoreCommands parse failed on: {0}", fullpath ?? path), null));
                    }
                    else
                    {
                        CoreFileDate = fi.LastWriteTime;

                        if (m_CoreCommands == null)
                            m_CoreCommands = new HashSet<Method>();
                        else
                            m_CoreCommands.Clear();

                        if (tree.HasErrors())
                            foreach (ParserMessage error in tree.ParserMessages)
                            {
                                if (error is ExternalErrorMessage)
                                    CoreErrors.Add(error);
                                else
                                    CoreErrors.Add(new ExternalErrorMessage(fullpath, error.Level, error.Location, error.Message, error.ParserState));
                            }
                        if (tree.Root != null)
                            foreach (ScopedNode cnode in ((ScopedNode)tree.Root.AstNode).ChildNodes)
                                m_CoreCommands.Add(new Method((FunctionProtoNode)cnode, context));
                    }
                }
            }
            else
            {
                CoreErrors.Add(new ParserMessage(ParserErrorLevel.Warning, SourceLocation.Empty, string.Format("CoreCommands declarations not found: {0}", fullpath ?? path), null));
                m_CoreCommands = new HashSet<Method>();
            }

            if (context != null)
                foreach (ParserMessage error in CoreErrors)
                {
                    if (error is ExternalErrorMessage)
                        context.CurrentParseTree.ParserMessages.Add(error);
                    else
                        context.CurrentParseTree.ParserMessages.Add(new ExternalErrorMessage(fullpath, error.Level, error.Location, error.Message, error.ParserState));
                }
        }
    }
}
