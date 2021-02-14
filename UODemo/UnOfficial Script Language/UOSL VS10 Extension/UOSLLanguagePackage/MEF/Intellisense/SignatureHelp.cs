using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;


namespace JoinUO.UOSL.Package.MEF
{
    [Export(typeof(ISignatureHelpSourceProvider))]
    [Name("Signature Help source")]
    [Order(Before = "default")]
    [ContentType("UOSL")]
    internal class SignatureHelpSourceProvider : ISignatureHelpSourceProvider
    {
        public ISignatureHelpSource TryCreateSignatureHelpSource(ITextBuffer textBuffer)
        {
            return new SignatureHelpSource(textBuffer);
        }
    }

    [Export(typeof(IVsTextViewCreationListener))]
    [Name("Signature Help controller")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    [ContentType("UOSL")]
    internal class SignatureHelpCommandProvider : IVsTextViewCreationListener
    {
        [Import] internal IVsEditorAdaptersFactoryService AdapterService;
        [Import] internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }
        [Import] internal ISignatureHelpBroker SignatureHelpBroker;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            ITextView textView = AdapterService.GetWpfTextView(textViewAdapter);
            if (textView == null)
                return;

            textView.Properties.GetOrCreateSingletonProperty(
                 () => new SignatureHelpCommandHandler(textViewAdapter, textView,
                    NavigatorService.GetTextStructureNavigator(textView.TextBuffer),
                    SignatureHelpBroker));
        }
    }

    internal class SignatureHelpSource : ISignatureHelpSource
    {
        static INodeProviderBroker nbroker = new NodeProviderBroker();
        NodeProvider nodeprovider = null;

        private ITextBuffer m_textBuffer;
        public SignatureHelpSource(ITextBuffer textBuffer)
        {
            nodeprovider = nbroker.GetNodeProvider(textBuffer);
            m_textBuffer = textBuffer;
        }

        public void AugmentSignatureHelpSession(ISignatureHelpSession session, IList<ISignature> signatures)
        {
            ITextSnapshot snapshot = m_textBuffer.CurrentSnapshot;
            int position = session.GetTriggerPoint(m_textBuffer).GetPosition(snapshot);

            Span span;

            if (session.Properties.ContainsProperty("span"))
                span = session.Properties.GetProperty<Span>("span");
            else
                span = new Span(position + 1, 1);

            ITrackingSpan applicableToSpan = m_textBuffer.CurrentSnapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeInclusive, TrackingFidelityMode.Forward);

            if (session.Properties.ContainsProperty("word"))
            {
                string word = session.Properties.GetProperty<string>("word");

                if (nodeprovider.Funcs != null)
                    foreach (var func in nodeprovider.Funcs)
                        if (StringComparer.InvariantCultureIgnoreCase.Equals(word, func.Name))
                            signatures.Add(CreateSignature(m_textBuffer, func.ToString(), func.Description, applicableToSpan));

                if (nodeprovider.Triggers != null)
                    foreach (var func in nodeprovider.Triggers)
                        if (StringComparer.InvariantCultureIgnoreCase.Equals(word, func.Name))
                            signatures.Add(CreateSignature(m_textBuffer, func.ToString(), string.Empty, applicableToSpan));

                if (nodeprovider.CoreFuncs != null)
                    foreach (var func in nodeprovider.CoreFuncs)
                        if (StringComparer.InvariantCultureIgnoreCase.Equals(word, func.Name))
                            signatures.Add(CreateSignature(m_textBuffer, func.ToString(), func.Description, applicableToSpan));
            }
        }

        private Signature CreateSignature(ITextBuffer textBuffer, string methodSig, string methodDoc, ITrackingSpan span)
        {
            Signature sig = new Signature(textBuffer, methodSig, methodDoc, null);
            textBuffer.Changed += new EventHandler<TextContentChangedEventArgs>(sig.OnSubjectBufferChanged);

            //find the parameters in the method signature (expect methodname(one, two)
            string[] pars = methodSig.Split(new char[] { '(', ',', ')' });
            List<IParameter> paramList = new List<IParameter>();

            int locusSearchStart = 0;
            for (int i = 1; i < pars.Length; i++)
            {
                string param = pars[i].Trim();

                if (string.IsNullOrEmpty(param))
                    continue;

                //find where this parameter is located in the method signature
                int locusStart = methodSig.IndexOf(param, locusSearchStart);
                if (locusStart >= 0)
                {
                    Span locus = new Span(locusStart, param.Length);
                    locusSearchStart = locusStart + param.Length;
                    paramList.Add(new Parameter("Parameter documentation not yet available.", locus, param, sig));
                }
            }

            sig.Parameters = new ReadOnlyCollection<IParameter>(paramList);
            sig.ApplicableToSpan = span;
            sig.ComputeCurrentParameter();
            return sig;
        }

        public ISignature GetBestMatch(ISignatureHelpSession session)
        {
            //foreach (var sig in session.Signatures)
            //{
            //    ITrackingSpan applicableToSpan = sig.ApplicableToSpan;
            //    string text = applicableToSpan.GetText(applicableToSpan.TextBuffer.CurrentSnapshot);

            //    //maybe return something
            //    //    return session.Signatures[0];
            //}
            return null;
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

    internal class Parameter : IParameter
    {
        public string Documentation { get; private set; }
        public Span Locus { get; private set; }
        public string Name { get; private set; }
        public ISignature Signature { get; private set; }
        public Span PrettyPrintedLocus { get; private set; }

        public Parameter(string documentation, Span locus, string name, ISignature signature)
        {
            Documentation = documentation;
            Locus = locus;
            Name = name;
            Signature = signature;
        }
    }

    internal class Signature : ISignature
    {
        private ITextBuffer m_subjectBuffer;
        private IParameter m_currentParameter;
        private string m_content;
        private string m_documentation;
        private ITrackingSpan m_applicableToSpan;
        private ReadOnlyCollection<IParameter> m_parameters;
        private string m_printContent;

        public event EventHandler<CurrentParameterChangedEventArgs> CurrentParameterChanged;

        internal Signature(ITextBuffer subjectBuffer, string content, string doc, ReadOnlyCollection<IParameter> parameters)
        {
            m_subjectBuffer = subjectBuffer;
            m_content = content;
            m_documentation = doc;
            m_parameters = parameters;
            m_subjectBuffer.Changed += new EventHandler<TextContentChangedEventArgs>(OnSubjectBufferChanged);
        }

        public IParameter CurrentParameter
        {
            get { return m_currentParameter; }
            internal set
            {
                if (m_currentParameter != value)
                {
                    IParameter prevCurrentParameter = m_currentParameter;
                    m_currentParameter = value;
                    this.RaiseCurrentParameterChanged(prevCurrentParameter, m_currentParameter);
                }
            }
        }

        private void RaiseCurrentParameterChanged(IParameter prevCurrentParameter, IParameter newCurrentParameter)
        {
            EventHandler<CurrentParameterChangedEventArgs> tempHandler = this.CurrentParameterChanged;
            if (tempHandler != null)
            {
                tempHandler(this, new CurrentParameterChangedEventArgs(prevCurrentParameter, newCurrentParameter));
            }
        }

        internal void ComputeCurrentParameter()
        {
            
            if (Parameters.Count == 0)
            {
                this.CurrentParameter = null;
                return;
            }

            //the number of commas in the string is the index of the current parameter
            string sigText = ApplicableToSpan.GetText(m_subjectBuffer.CurrentSnapshot);

            Debug.WriteLine("ComputeCurrentParameter {0} Text:{1}", m_content, sigText);

            int currentIndex = 0;
            int commaCount = 0;
            while (currentIndex < sigText.Length)
            {
                int commaIndex = sigText.IndexOf(',', currentIndex);
                if (commaIndex == -1)
                {
                    break;
                }
                commaCount++;
                currentIndex = commaIndex + 1;
            }

            if (commaCount < Parameters.Count)
            {
                this.CurrentParameter = Parameters[commaCount];
            }
            else
            {
                //too many commas, so use the last parameter as the current one.
                this.CurrentParameter = Parameters[Parameters.Count - 1];
            }
        }

        internal void OnSubjectBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            this.ComputeCurrentParameter();
        }


        public ITrackingSpan ApplicableToSpan
        {
            get { return (m_applicableToSpan); }
            internal set { m_applicableToSpan = value; }
        }

        public string Content
        {
            get { return (m_content); }
            internal set { m_content = value; }
        }

        public string Documentation
        {
            get { return (m_documentation); }
            internal set { m_documentation = value; }
        }

        public ReadOnlyCollection<IParameter> Parameters
        {
            get { return (m_parameters); }
            internal set { m_parameters = value; }
        }

        public string PrettyPrintedContent
        {
            get { return (m_printContent); }
            internal set { m_printContent = value; }
        }
    }

    internal sealed class SignatureHelpCommandHandler : IOleCommandTarget
    {
        IOleCommandTarget m_nextCommandHandler;
        ITextView m_textView;
        ISignatureHelpBroker m_broker;
        ISignatureHelpSession m_session;
        ITextStructureNavigator m_navigator;

        internal SignatureHelpCommandHandler(IVsTextView textViewAdapter, ITextView textView, ITextStructureNavigator nav, ISignatureHelpBroker broker)
        {
            this.m_textView = textView;
            this.m_broker = broker;
            this.m_navigator = nav;

            //add this to the filter chain
            textViewAdapter.AddCommandFilter(this, out m_nextCommandHandler);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            char typedChar = char.MinValue;

            if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
            {
                typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
                if (typedChar.Equals('(') && (m_session == null || m_session.IsDismissed))
                {
                    //move the point back so it's in the preceding word
                    SnapshotPoint point = m_textView.Caret.Position.BufferPosition - 1;
                    TextExtent extent = m_navigator.GetExtentOfWord(point);
                    string word = extent.Span.GetText();

                    m_session = m_broker.CreateSignatureHelpSession(m_textView, m_textView.TextSnapshot.CreateTrackingPoint(point.Position, PointTrackingMode.Positive), true);
                    m_session.Properties.AddProperty("word", word);
                    m_session.Start();
                }
                else if (typedChar.Equals(',') && (m_session == null || m_session.IsDismissed))
                {
                    int paramPos;
                    int pos = m_textView.Caret.Position.BufferPosition - 1;
                    while (pos > 0 && m_textView.TextSnapshot[pos] != '(') pos--;
                    if (pos > 0)
                    {
                        paramPos = pos;
                        pos--;
                        while (pos > 0 && char.IsWhiteSpace(m_textView.TextSnapshot[pos])) pos--;
                        if (pos > 0)
                        {
                            SnapshotPoint point = new SnapshotPoint(m_textView.TextSnapshot, pos);
                            TextExtent extent = m_navigator.GetExtentOfWord(point);
                            string word = extent.Span.GetText();


                            m_session = m_broker.CreateSignatureHelpSession(m_textView, m_textView.TextSnapshot.CreateTrackingPoint(m_textView.Caret.Position.BufferPosition - 1, PointTrackingMode.Positive), true);
                            m_session.Properties.AddProperty("word", word);
                            m_session.Properties.AddProperty("span", new Span(paramPos, m_textView.Caret.Position.BufferPosition - paramPos));
                            m_session.Start();

                        }
                    }
                }
                else if (typedChar.Equals(')') && m_session != null)
                {
                    m_session.Dismiss();
                    m_session = null;
                }
                else if (m_session!=null && m_session.IsDismissed)
                    m_session = null;
            }
            return m_nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }


        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return m_nextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

    }
}
