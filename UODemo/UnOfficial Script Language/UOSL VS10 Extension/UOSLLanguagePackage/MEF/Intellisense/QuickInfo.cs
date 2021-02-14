using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;



namespace JoinUO.UOSL.Package.MEF
{
    [Export(typeof(IIntellisenseControllerProvider))]
    [Name("ToolTip QuickInfo Controller")]
    [ContentType("UOSL")]
    internal class QuickInfoControllerProvider : IIntellisenseControllerProvider
    {
        [Import]
        internal IQuickInfoBroker QuickInfoBroker { get; set; }

        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            return new QuickInfoController(textView, subjectBuffers, this);
        }
    }

    [Export(typeof(IQuickInfoSourceProvider))]
    [Name("ToolTip QuickInfo Source")]
    [Order(Before = "Default Quick Info Presenter")]
    [ContentType("UOSL")]
    internal class QuickInfoSourceProvider : IQuickInfoSourceProvider
    {
        [Import]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        [Import]
        internal ITextBufferFactoryService TextBufferFactoryService { get; set; }

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return new QuickInfoSource(this, textBuffer);
        }
    }

    internal class QuickInfoSource : IQuickInfoSource
    {

        private QuickInfoSourceProvider m_provider;
        private ITextBuffer m_subjectBuffer;
        private Dictionary<string, string> m_dictionary;

        static INodeProviderBroker nbroker = new NodeProviderBroker();

        NodeProvider nodeprovider = null;

        public QuickInfoSource(QuickInfoSourceProvider provider, ITextBuffer subjectBuffer)
        {
            m_provider = provider;
            m_subjectBuffer = subjectBuffer;

            nodeprovider = nbroker.GetNodeProvider(subjectBuffer);

            //TODO: Make this a dictionary of List<string>, to support overloads
            m_dictionary = new Dictionary<string, string>();

            // Load script functions, core functions and triggers.

            if(nodeprovider.Funcs!=null)
                foreach (var func in nodeprovider.Funcs)
                    m_dictionary.Add(func.Name, func.ToString()); // TODO: Include filename info

            if (nodeprovider.Triggers != null)
                foreach (var func in nodeprovider.Triggers)
                    m_dictionary.Add(func.Name, string.Format("Trigger: {0}", func.ToString()));

            if (nodeprovider.CoreFuncs != null)
                foreach (var func in nodeprovider.CoreFuncs)
                {
                    if (m_dictionary.ContainsKey(func.Name))
                    {   // overloaded, add another line
                        m_dictionary[func.Name] += string.Format("\nCore: {0}", func.ToString());
                    }
                    else
                        m_dictionary.Add(func.Name, string.Format("Core: {0}", func.ToString()));
                }
           
        }
        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> qiContent, out ITrackingSpan applicableToSpan)
        {
            // Map the trigger point down to our buffer.
            SnapshotPoint? subjectTriggerPoint = session.GetTriggerPoint(m_subjectBuffer.CurrentSnapshot);
            if (!subjectTriggerPoint.HasValue)
            {
                applicableToSpan = null;
                return;
            }

            ITextSnapshot currentSnapshot = subjectTriggerPoint.Value.Snapshot;
            SnapshotSpan querySpan = new SnapshotSpan(subjectTriggerPoint.Value, 0);

            //look for occurrences of our QuickInfo words in the span
            ITextStructureNavigator navigator = m_provider.NavigatorService.GetTextStructureNavigator(m_subjectBuffer);
            TextExtent extent = navigator.GetExtentOfWord(subjectTriggerPoint.Value);
            string searchText = extent.Span.GetText();

            foreach (string key in m_dictionary.Keys)
            {
                if (StringComparer.InvariantCultureIgnoreCase.Equals(searchText, key))
                {
                    applicableToSpan = currentSnapshot.CreateTrackingSpan
                        (
                            extent.Span, SpanTrackingMode.EdgeInclusive
                        );

                    string value;
                    m_dictionary.TryGetValue(key, out value);
                    if (value != null)
                        qiContent.Add(value);
                    else
                        qiContent.Add("");

                    return;
                }
            }

            applicableToSpan = null;
        }


        private bool m_isDisposed;
        public void Dispose()
        {
            if (!m_isDisposed)
            {
                GC.SuppressFinalize(this);
                m_isDisposed = true;
            }
        }
    }

    internal class QuickInfoController : IIntellisenseController
    {
        private ITextView m_textView;
        private IList<ITextBuffer> m_subjectBuffers;
        private QuickInfoControllerProvider m_provider;
        private IQuickInfoSession m_session;

        internal QuickInfoController(ITextView textView, IList<ITextBuffer> subjectBuffers, QuickInfoControllerProvider provider)
        {
            m_textView = textView;
            m_subjectBuffers = subjectBuffers;
            m_provider = provider;

            m_textView.MouseHover += this.OnTextViewMouseHover;
        }

        private void OnTextViewMouseHover(object sender, MouseHoverEventArgs e)
        {
            //find the mouse position by mapping down to the subject buffer
            SnapshotPoint? point = m_textView.BufferGraph.MapDownToFirstMatch
                 (new SnapshotPoint(m_textView.TextSnapshot, e.Position),
                PointTrackingMode.Positive,
                snapshot => m_subjectBuffers.Contains(snapshot.TextBuffer),
                PositionAffinity.Predecessor);

            if (point != null)
            {
                ITrackingPoint triggerPoint = point.Value.Snapshot.CreateTrackingPoint(point.Value.Position,
                PointTrackingMode.Positive);

                if (!m_provider.QuickInfoBroker.IsQuickInfoActive(m_textView))
                {
                    m_session = m_provider.QuickInfoBroker.TriggerQuickInfo(m_textView, triggerPoint, true);
                }
            }
        }

        public void Detach(ITextView textView)
        {
            if (m_textView == textView)
            {
                m_textView.MouseHover -= this.OnTextViewMouseHover;
                m_textView = null;
            }
        }

        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }

        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }
    }
}
