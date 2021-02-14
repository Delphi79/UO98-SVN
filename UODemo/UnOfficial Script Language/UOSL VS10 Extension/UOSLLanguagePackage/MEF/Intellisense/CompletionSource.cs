using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using JoinUO.UOSL.Service;
using JoinUO.UOSL.Service.ASTNodes;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;


namespace JoinUO.UOSL.Package.MEF.Intellisense
{
    [Export(typeof(IVsTextViewCreationListener))]
    [Name("completion handler")]
    [ContentType("UOSL")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal class CompletionHandlerProvider : IVsTextViewCreationListener
    {
        [Import] internal IVsEditorAdaptersFactoryService AdapterService = null;
        [Import] internal ICompletionBroker CompletionBroker { get; set; }
        [Import] internal SVsServiceProvider ServiceProvider { get; set; }
       
        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            ITextView textView = AdapterService.GetWpfTextView(textViewAdapter);
            if (textView == null)
                return;

            Func<CompletionCommandHandler> createCommandHandler = delegate() { return new CompletionCommandHandler(textViewAdapter, textView, this); };
            textView.Properties.GetOrCreateSingletonProperty(createCommandHandler);
        }
    }

    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("UOSL")]
    [Name("completion provider")]
    internal class CompletionSourceProvider : ICompletionSourceProvider
    {
        [Import] internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new CompletionSource(this, textBuffer);
        }
    }

    internal class CompletionSource : ICompletionSource
    {
        private CompletionSourceProvider m_sourceProvider;
        private ITextBuffer m_textBuffer;

        private List<Completion> m_core;
        private List<Completion> m_trigsFull;
        private List<Completion> m_trigsNames;
        private List<Completion> m_types;

        static INodeProviderBroker nbroker = new NodeProviderBroker();
        NodeProvider nodeprovider = null;

        public CompletionSource(CompletionSourceProvider sourceProvider, ITextBuffer textBuffer)
        {
            m_sourceProvider = sourceProvider;
            m_textBuffer = textBuffer;
            nodeprovider = nbroker.GetNodeProvider(textBuffer);

            IEnumerable<Irony.Parsing.KeyTerm> keys = UOSLGrammar.Keywords.Values;
            m_types = new List<Completion>();
            foreach(var key in keys)
            {
                if (key is Tokenizer && ((Tokenizer)key).Tokenize() is UOdemoSDK.UoToken.UoTypeName)
                    m_types.Add(new Completion(key.Name, " " + key.Name, key.Name, null, null));
            }
            if (m_types.Count == 0) m_types = null;

            if (nodeprovider.CoreFuncs != null)
            {
                Dictionary<string, Service.Method> Funcs = new Dictionary<string, Service.Method>();
                
                foreach (var func in nodeprovider.CoreFuncs)
                    if (!Funcs.ContainsKey(func.Name))
                        Funcs.Add(func.Name,func);

                m_core = new List<Completion>();
                foreach (string str in Funcs.Keys.OrderBy(comp => comp))
                    m_core.Add(new Completion(Funcs[str].ToString(false), str, Funcs[str].Description, null, null));
            }

            if (nodeprovider.Triggers != null)
            {
                Dictionary<string, JoinUO.UOdemoSDK.Events.EventDefinition> Trigs = new Dictionary<string, JoinUO.UOdemoSDK.Events.EventDefinition>();
                
                foreach (var func in nodeprovider.Triggers)
                    if (!Trigs.ContainsKey(func.Name))
                        Trigs.Add(func.Name, func);

                m_trigsFull = new List<Completion>();
                foreach (string str in Trigs.Keys.OrderBy(comp => comp))
                {
                    string trig;
                    if (Trigs[str].EventType == UOdemoSDK.Events.EventDefinition.EventParamType.Number)
                        trig = string.Format("trigger<NUMBER> {0}", Trigs[str].ToString());
                    else if (Trigs[str].EventType == UOdemoSDK.Events.EventDefinition.EventParamType.Text)
                        trig = string.Format("trigger<\"WORD\"> {0}", Trigs[str].ToString());
                    else if (Trigs[str].EventType == UOdemoSDK.Events.EventDefinition.EventParamType.Time)
                        trig = string.Format("trigger<\"TIMESTRING\"> {0}", Trigs[str].ToString());
                    else
                        trig = string.Format("trigger {0}", Trigs[str].ToString());
                    m_trigsFull.Add(new Completion(trig, trig, str, null, null));
                }

                m_trigsNames = new List<Completion>();
                foreach (string str in Trigs.Keys.OrderBy(comp => comp))
                {
                    string trig = string.Format("{0}", Trigs[str].ToString());
                    m_trigsNames.Add(new Completion(str, trig, str, null, null));
                }
            }
        }

        void ICompletionSource.AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            List<string> strList = new List<string>();

            SnapshotPoint point=session.GetTriggerPoint(m_textBuffer).GetPoint(m_textBuffer.CurrentSnapshot);

            if (nodeprovider.isComment(point)) return;

            Node node = nodeprovider.GetMostSpecificNode(point);

            List<Completion> m_compList = null;
            if (node != null && node.astnode is ScopedNode)
            {
                var terms = ((ScopedNode)node.astnode).ExpectedTerms.Where(term => term.FlagIsSet(Irony.Parsing.TermFlags.IsKeyword) || term.FlagIsSet(Irony.Parsing.TermFlags.IsPunctuation));
                if (terms.Count() > 0)
                {
                    foreach (var term in terms)
                        if (!strList.Contains(term.Name))
                            strList.Add(term.Name);
                }
                else
                {
                    foreach (var term in UOSL.Service.UOSLBase.Keywords)
                        if (!strList.Contains(term.Key))
                            strList.Add(term.Key);
                }

                if (strList.Count > 0)
                {
                    foreach (string str in strList)
                        (m_compList ?? (m_compList = new List<Completion>())).Add(new Completion(str, str, str, null, null));

                    strList.Clear();
                }
            }


            if (node == null || node.astnode.AsString == "Declarations" || node.astnode.AsString == "Script")
            {
                int pos = point.Position;
                // move to beginning of previous word
                while ((pos >= point.Snapshot.Length || char.IsWhiteSpace(point.Snapshot[pos])) && pos >= 0) pos--;
                int end = pos;
                while (!char.IsWhiteSpace(point.Snapshot[pos]) && pos >= 0) pos--;
                string keyword = pos > 0 && pos < end ? point.Snapshot.GetText(pos + 1, end - pos) : null;
                if (keyword == "trigger" && m_trigsNames != null)
                {
                    completionSets.Add(new CompletionSet(
                        "Triggers",    //the non-localized title of the tab
                        "Triggers",    //the display title of the tab
                        FindTokenSpanAtPosition(session.GetTriggerPoint(m_textBuffer),
                            session),
                        m_trigsNames,
                        null));
                }
                else if (m_types != null && keyword == "function" || keyword == "member" || keyword == "forward")
                {
                    completionSets.Add(new CompletionSet(
                        "Types",    //the non-localized title of the tab
                        "Types",    //the display title of the tab
                        FindTokenSpanAtPosition(session.GetTriggerPoint(m_textBuffer),
                            session),
                        m_types,
                        null));
                }
                else if (m_trigsFull != null)
                {
                    if(m_compList!=null)
                        completionSets.Add(new CompletionSet(
                            "Keywords",    //the non-localized title of the tab
                            "Keywords",    //the display title of the tab
                            FindTokenSpanAtPosition(session.GetTriggerPoint(m_textBuffer),
                                session),
                            m_compList,
                            null));

                    completionSets.Add(new CompletionSet(
                        "Triggers",    //the non-localized title of the tab
                        "Triggers",    //the display title of the tab
                        FindTokenSpanAtPosition(session.GetTriggerPoint(m_textBuffer),
                            session),
                        m_trigsFull,
                        null));
                }
                else if (m_compList != null)
                    completionSets.Add(new CompletionSet(
                        "Keywords",    //the non-localized title of the tab
                        "Keywords",    //the display title of the tab
                        FindTokenSpanAtPosition(session.GetTriggerPoint(m_textBuffer),
                            session),
                        m_compList,
                        null));
            }
            else
            {
                if (m_compList != null)
                    completionSets.Add(new CompletionSet(
                        "Keywords",    //the non-localized title of the tab
                        "Keywords",    //the display title of the tab
                        FindTokenSpanAtPosition(session.GetTriggerPoint(m_textBuffer),
                            session),
                        m_compList,
                        null));

                ScopedNode snode = node.astnode as ScopedNode;
                if (snode.ScopeVars != null)
                {
                    foreach (var var in snode.ScopeVars)
                        strList.Add(var.Name);

                    if (strList.Count > 0)
                    {
                        strList.Sort();

                        m_compList = new List<Completion>();
                        foreach (string str in strList)
                            m_compList.Add(new Completion(str, str, str, null, null));

                        completionSets.Add(new CompletionSet(
                            "Vars",    //the non-localized title of the tab
                            "Vars",    //the display title of the tab
                            FindTokenSpanAtPosition(session.GetTriggerPoint(m_textBuffer),
                                session),
                            m_compList,
                            null));

                        strList.Clear();
                    }
                }

                if (nodeprovider.Funcs != null)
                {
                    foreach (var func in nodeprovider.Funcs)
                        if (!strList.Contains(func.Name))
                            strList.Add(func.Name);

                    if (strList.Count > 0)
                    {
                        strList.Sort();

                        m_compList = new List<Completion>();
                        foreach (string str in strList)
                            m_compList.Add(new Completion(str, str, str, null, null));

                        completionSets.Add(new CompletionSet(
                            "Funcs",    //the non-localized title of the tab
                            "Funcs",    //the display title of the tab
                            FindTokenSpanAtPosition(session.GetTriggerPoint(m_textBuffer),
                                session),
                            m_compList,
                            null));
                    }

                    strList.Clear();

                }

                if (m_core != null)
                    completionSets.Add(new CompletionSet(
                        "Core",    //the non-localized title of the tab
                        "Core",    //the display title of the tab
                        FindTokenSpanAtPosition(session.GetTriggerPoint(m_textBuffer),
                            session),
                        m_core,
                        null));

            }
        }

        private ITrackingSpan FindTokenSpanAtPosition(ITrackingPoint point, ICompletionSession session)
        {
            SnapshotPoint currentPoint = (session.TextView.Caret.Position.BufferPosition) - 1;
            ITextStructureNavigator navigator = m_sourceProvider.NavigatorService.GetTextStructureNavigator(m_textBuffer);
            TextExtent extent = navigator.GetExtentOfWord(currentPoint);
            return currentPoint.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
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


    internal class CompletionCommandHandler : IOleCommandTarget
    {
        private IOleCommandTarget m_nextCommandHandler;
        private ITextView m_textView;
        private CompletionHandlerProvider m_provider;
        private ICompletionSession m_session;
        internal CompletionCommandHandler(IVsTextView textViewAdapter, ITextView textView, CompletionHandlerProvider provider)
        {
            this.m_textView = textView;
            this.m_provider = provider;

            //add the command to the command chain
            textViewAdapter.AddCommandFilter(this, out m_nextCommandHandler);
        }
        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return m_nextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (VsShellUtilities.IsInAutomationFunction(m_provider.ServiceProvider))
            {
                return m_nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            }
            //make a copy of this so we can look at it after forwarding some commands
            uint commandID = nCmdID;
            char typedChar = char.MinValue;
            //make sure the input is a char before getting it
            if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
            {
                typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
            }

            //check for a commit character
            if (nCmdID == (uint)VSConstants.VSStd2KCmdID.RETURN
                || nCmdID == (uint)VSConstants.VSStd2KCmdID.TAB
                || (/*char.IsWhiteSpace(typedChar) ||*/ char.IsPunctuation(typedChar))) // Elected not to use whitespace as commit, as cometimes completions pop up in new comments, and completion spaces aggrivate
            {
                //check for a a selection
                if (m_session != null && !m_session.IsDismissed)
                {
                    //if the selection is fully selected, commit the current session
                    if (m_session.SelectedCompletionSet.SelectionStatus.IsSelected)
                    {
                        m_session.Commit();
                        //also, don't add the character to the buffer
                        return VSConstants.S_OK;
                    }
                    else
                    {
                        //if there is no selection, dismiss the session
                        m_session.Dismiss();
                    }
                }
            }

            //pass along the command so the char is added to the buffer
            int retVal = m_nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            bool handled = false;
            if (!typedChar.Equals(char.MinValue) && char.IsLetterOrDigit(typedChar))
            {
                if (m_session == null || m_session.IsDismissed) // If there is no active session, bring up completion
                {
                    this.TriggerCompletion();
                    if (m_session != null)
                        m_session.Filter();
                }
                else    //the completion session is already active, so just filter
                {
                    m_session.Filter();
                }
                handled = true;
            }
            else if (commandID == (uint)VSConstants.VSStd2KCmdID.BACKSPACE   //redo the filter if there is a deletion
                || commandID == (uint)VSConstants.VSStd2KCmdID.DELETE)
            {
                if (m_session != null && !m_session.IsDismissed)
                    m_session.Filter();
                handled = true;
            }
            if (handled) return VSConstants.S_OK;
            return retVal;
        }

        private bool TriggerCompletion()
        {
            //the caret must be in a non-projection location 
            SnapshotPoint? caretPoint =
            m_textView.Caret.Position.Point.GetPoint(
            textBuffer => (!textBuffer.ContentType.IsOfType("projection")), PositionAffinity.Predecessor);
            if (!caretPoint.HasValue)
            {
                return false;
            }

            m_session = m_provider.CompletionBroker.CreateCompletionSession
         (m_textView,
                caretPoint.Value.Snapshot.CreateTrackingPoint(caretPoint.Value.Position, PointTrackingMode.Positive),
                true);

            //subscribe to the Dismissed event on the session 
            m_session.Dismissed += this.OnSessionDismissed;
            m_session.Start();

            return true;
        }

        private void OnSessionDismissed(object sender, EventArgs e)
        {
            m_session.Dismissed -= this.OnSessionDismissed;
            m_session = null;
        }
    }

}
