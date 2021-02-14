//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.ComponentModel.Composition;
//using Microsoft.VisualStudio.Text.Outlining;
//using Microsoft.VisualStudio.Text.Tagging;
//using Microsoft.VisualStudio.Utilities;
//using Microsoft.VisualStudio.Text;
//using Microsoft.VisualStudio.Text.Adornments;
//using Microsoft.VisualStudio.Text.Outlining;
//using Microsoft.VisualStudio.Text.Tagging;
//using Microsoft.VisualStudio.Utilities;
//using JoinUO.UOSL.Service;

//namespace JoinUO.UOSL.Package.MEF
//{
//    [Export(typeof(ITaggerProvider))]
//    [ContentType("UOSL")]
//    [TagType(typeof(ErrorTag))]
//    class ErrorTaggerProvider : ITaggerProvider
//    {
//        [Import] internal INodeProviderBroker nodeProviderBroker { get; set; }

//        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
//        {
//            //create a single tagger for each buffer.
//            Func<ITagger<T>> sc = delegate() { return new ErrorTagger(nodeProviderBroker, buffer) as ITagger<T>; };
//            return buffer.Properties.GetOrCreateSingletonProperty<ITagger<T>>(sc);
//        }
//    }

//    class ErrorTagger : ITagger<ErrorTag>
//    {
//        private NodeProvider nodeProvider;
 
//        public ErrorTagger(INodeProviderBroker nodeProviderBroker, ITextBuffer buffer)
//        {
//            nodeProvider = nodeProviderBroker.GetNodeProvider(buffer);
//            nodeProvider.NodesChanged += new NodeProvider.SnapshotEvent(provider_TagsChanged);
//        }
 
//        void provider_TagsChanged(SnapshotSpan snapshotSpan)
//        {
//            if (TagsChanged != null)
//                TagsChanged(this, new SnapshotSpanEventArgs(snapshotSpan));
//        }
 
//        public IEnumerable<ITagSpan<ErrorTag>> GetTags(Microsoft.VisualStudio.Text.NormalizedSnapshotSpanCollection spans)
//        {
//            foreach (SnapshotSpan span in spans)
//            {
//                foreach (NodeSnapshot node in nodeProvider.GetNodes(span))
//                {
//                    if (node.SnapshotSpan.OverlapsWith(span) && node.Node.ErrorMessage.Severity > 0)
//                        yield return new TagSpan<Constants.ErrorTag>(node.SnapshotSpan, new Constants.ErrorTag());
//                }
//            }
//        }
 
//        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
//    }
//}
