using System;
using System.Collections.Generic;
using System.Diagnostics;
using Irony.Ast;
using Irony.Parsing;
using JoinUO.UOSL.Service;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace JoinUO.UOSL.Package
{
    public class IronyLanguageService : Microsoft.VisualStudio.Package.LanguageService
    {
        public static readonly object lock_parser = new object();

        struct LanguageSet
        {
            private static Dictionary<LanguageOption, LanguageSet> registry = new Dictionary<LanguageOption, LanguageSet>();

            public static LanguageSet Acquire(LanguageOption option)
            {
                if (registry.ContainsKey(option))
                    return registry[option];
                return registry[option] = new LanguageSet(option);
            }

            private LanguageSet(LanguageOption option)
            {
                grammar = new UOSLGrammar(option);
                parser = new Parser(grammar);
                context = new ParsingContext(parser);
            }

            private Grammar grammar;
            private Parser parser;
            private ParsingContext context;

            public Grammar Grammar { get { return grammar; } }
            public Parser Parser { get { return parser; } }
            public ParsingContext Context { get { return context; } }
        }

        //private Grammar grammar;
        //private Parser parser;
        //private ParsingContext context;

        public IronyLanguageService()
        {
            //grammar = new UOSLGrammar(LanguageOption.Extended);
            //parser = new Parser(grammar);
            //context = new ParsingContext(parser);
        }

        #region Custom Colors
        public override int GetColorableItem(int index, out IVsColorableItem item)
        {
            if (index <= Configuration.ColorableItems.Count)
            {
                item = Configuration.ColorableItems[index - 1];
                return Microsoft.VisualStudio.VSConstants.S_OK;
            }
            else
            {
                throw new ArgumentNullException("index");
            }
        }

        public override int GetItemCount(out int count)
        {
            count = Configuration.ColorableItems.Count;
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }
        #endregion

        #region MPF Accessor and Factory specialisation
        private LanguagePreferences preferences;
        public override LanguagePreferences GetLanguagePreferences()
        {
            if (this.preferences == null)
            {
                this.preferences = new LanguagePreferences(this.Site,
                                                        typeof(IronyLanguageService).GUID,
                                                        this.Name);
                this.preferences.Init();
            }

            return this.preferences;
        }

        public override Microsoft.VisualStudio.Package.Source CreateSource(IVsTextLines buffer)
        {
            return new Source(this, buffer, this.GetColorizer(buffer));
        }

        private IScanner scanner;
        public override IScanner GetScanner(IVsTextLines buffer)
        {
            if (scanner == null)
                this.scanner = new LineScanner(LanguageSet.Acquire(LanguageOption.Extended).Grammar);

            return this.scanner;
        }
        #endregion

        public override void OnIdle(bool periodic)
        {
            // from IronPythonLanguage sample
            // this appears to be necessary to get a parse request with ParseReason = Check?
            Source src = (Source)GetSource(this.LastActiveTextView);
            if (src != null && src.LastParseTime >= Int32.MaxValue >> 12)
            {
                src.LastParseTime = 0;
            }
            base.OnIdle(periodic);
        }

        private void SetLanguageOption(Source source)
        {
            LanguageOption option;
            if (!Utils.TryGetLanguageDeclaration(source.GetLine(0), out option))
                option = LanguageOption.Extended;
            source.option = option;
        }

        Dictionary<Source, AuthoringScope> ScopeDict = new Dictionary<Source, AuthoringScope>();

        public override Microsoft.VisualStudio.Package.AuthoringScope ParseSource(ParseRequest req)
        {
            Debug.Print("ParseSource at ({0}:{1}), reason {2}", req.Line, req.Col, req.Reason);
            Source source = (Source)this.GetSource(req.FileName);

            if (source.GetLineCount() > 0)
            {
                switch (req.Reason)
                {
                    case ParseReason.Check:
                        SetLanguageOption(source);
                        Parser parser = LanguageSet.Acquire(source.option).Parser;

                        // This is where you perform your syntax highlighting.
                        // Parse entire source as given in req.Text.
                        // Store results in the AuthoringScope object.
                        ParseTree tree;
                        ParseTreeNode root;
                        lock (lock_parser)
                        {
                            root = (tree = parser.Parse(req.Text, req.FileName)).Root;

                            AstNode node = null;
                            if (root != null)
                            {
                                if (req.FileName != "<Source>")
                                    JoinUO.UOSL.Service.UOSLBase.ReCache(req.FileName, tree);

                                node = (AstNode)root.AstNode;
                            }
                            source.ParseResult = node;
                        }

                        // Used for brace matching.
                        TokenStack braces = parser.Context.OpenBraces;
                        foreach (Token brace in braces)
                        {
                            TextSpan openBrace = new TextSpan();
                            openBrace.iStartLine = brace.Location.Line;
                            openBrace.iStartIndex = brace.Location.Column;
                            openBrace.iEndLine = brace.Location.Line;
                            openBrace.iEndIndex = openBrace.iStartIndex + brace.Length;

                            TextSpan closeBrace = new TextSpan();
                            if (brace.OtherBrace != null)   // Added check: this did not seem to be handled -=Derrick=-
                            {
                                closeBrace.iStartLine = brace.OtherBrace.Location.Line;
                                closeBrace.iStartIndex = brace.OtherBrace.Location.Column;
                                closeBrace.iEndLine = brace.OtherBrace.Location.Line;
                                closeBrace.iEndIndex = closeBrace.iStartIndex + brace.OtherBrace.Length;

                                if (source.Braces == null)
                                {
                                    source.Braces = new List<TextSpan[]>();
                                }
                                source.Braces.Add(new TextSpan[2] { openBrace, closeBrace });
                            }
                            else // Added error message -=Derrick=- 
                                req.Sink.AddError(req.FileName, string.Format("Unmatched opening brace '{0}'", brace.Text), openBrace, Severity.Error);
                        }

                        if (parser.Context.CurrentParseTree.ParserMessages.Count > 0)
                        {
                            foreach (ParserMessage error in parser.Context.CurrentParseTree.ParserMessages)
                            {
                                TextSpan span = new TextSpan();
                                int line, col;
                                if (!(error is ExternalErrorMessage) || ((ExternalErrorMessage)error).FilePath == parser.Context.CurrentParseTree.FileName)
                                {
                                    source.GetLineIndexOfPosition(error.Span.Location.Position, out line, out col);
                                    span.iStartLine = line;
                                    span.iStartIndex = col;

                                    source.GetLineIndexOfPosition(error.Span.EndPosition, out line, out col);
                                    span.iEndLine = Math.Max(span.iStartLine, line);
                                    span.iEndIndex = Math.Max(span.iStartIndex, col);
                                }
                                else
                                {
                                    span.iStartLine = error.Span.Location.Line;
                                    span.iStartIndex = error.Span.Location.Column;
                                    span.iEndLine = span.iStartLine;
                                    span.iEndIndex = span.iStartIndex;
                                }

                                Severity level = error.Level == ParserErrorLevel.Error ? Severity.Error : error.Level == ParserErrorLevel.Warning ? Severity.Warning : Severity.Hint;
                                if (error is ExternalErrorMessage)
                                    req.Sink.AddError(((ExternalErrorMessage)error).FilePath, error.Message, span, level);
                                else
                                    req.Sink.AddError(req.FileName, error.Message, span, level);
                            }
                        }
                        break;

                    case ParseReason.DisplayMemberList:
                        // Parse the line specified in req.Line for the two
                        // tokens just before req.Col to obtain the identifier
                        // and the member connector symbol.
                        // Examine existing parse tree for members of the identifer
                        // and return a list of members in your version of the
                        // Declarations class as stored in the AuthoringScope
                        // object.
                        break;

                    case ParseReason.MethodTip:
                        // Parse the line specified in req.Line for the token
                        // just before req.Col to obtain the name of the method
                        // being entered.
                        // Examine the existing parse tree for all method signatures
                        // with the same name and return a list of those signatures
                        // in your version of the Methods class as stored in the
                        // AuthoringScope object.
                        break;

                    case ParseReason.QuickInfo:
                        break;

                    case ParseReason.HighlightBraces:
                    case ParseReason.MemberSelectAndHighlightBraces:
                    case ParseReason.MatchBraces:
                        if (source.Braces != null)
                        {
                            foreach (TextSpan[] brace in source.Braces)
                            {
                                if (brace.Length == 2)
                                    req.Sink.MatchPair(brace[0], brace[1], 1);
                                else if (brace.Length >= 3)
                                    req.Sink.MatchTriple(brace[0], brace[1], brace[2], 1);
                            }
                        }
                        break;
                }
            }
            AuthoringScope _scope = null;
            ScopeDict.TryGetValue(source, out _scope);

            if (_scope == null)
                _scope = ScopeDict[source] = new AuthoringScope(source, source.ParseResult);
            else if (source.ParseResult != null)
                _scope.LastParseResult = source.ParseResult;

            return _scope;
        }

        /// <summary>
        /// Called to determine if the given location can have a breakpoint applied to it. 
        /// </summary>
        /// <param name="buffer">The IVsTextBuffer object containing the source file.</param>
        /// <param name="line">The line number where the breakpoint is to be set.</param>
        /// <param name="col">The offset into the line where the breakpoint is to be set.</param>
        /// <param name="pCodeSpan">
        /// Returns the TextSpan giving the extent of the code affected by the breakpoint if the 
        /// breakpoint can be set.
        /// </param>
        /// <returns>
        /// If successful, returns S_OK; otherwise returns S_FALSE if there is no code at the given 
        /// position or returns an error code (the validation is deferred until the debug engine is loaded). 
        /// </returns>
        /// <remarks>
        /// <para>
        /// CAUTION: Even if you do not intend to support the ValidateBreakpointLocation but your language 
        /// does support breakpoints, you must override the ValidateBreakpointLocation method and return a 
        /// span that contains the specified line and column; otherwise, breakpoints cannot be set anywhere 
        /// except line 1. You can return E_NOTIMPL to indicate that you do not otherwise support this 
        /// method but the span must always be set. The example shows how this can be done.
        /// </para>
        /// <para>
        /// Since the language service parses the code, it generally knows what is considered code and what 
        /// is not. Normally, the debug engine is loaded and the pending breakpoints are bound to the source. It is at this time the breakpoint location is validated. This method is a fast way to determine if a breakpoint can be set at a particular location without loading the debug engine.
        /// </para>
        /// <para>
        /// You can implement this method to call the ParseSource method with the parse reason of CodeSpan. 
        /// The parser examines the specified location and returns a span identifying the code at that 
        /// location. If there is code at the location, the span identifying that code should be passed to 
        /// your implementation of the CodeSpan method in your version of the AuthoringSink class. Then your 
        /// implementation of the ValidateBreakpointLocation method retrieves that span from your version of 
        /// the AuthoringSink class and returns that span in the pCodeSpan argument.
        /// </para>
        /// <para>
        /// The base method returns E_NOTIMPL.
        /// </para>
        /// </remarks>
        public override int ValidateBreakpointLocation(IVsTextBuffer buffer, int line, int col, TextSpan[] pCodeSpan)
        {
            // TODO: Add code to not allow breakpoints to be placed on non-code lines.
            // TODO: Refactor to allow breakpoint locations to span multiple lines.
            //if (pCodeSpan != null)
            //{
            //    pCodeSpan[0].iStartLine = line;
            //    pCodeSpan[0].iStartIndex = col;
            //    pCodeSpan[0].iEndLine = line;
            //    pCodeSpan[0].iEndIndex = col;
            //    if (buffer != null)
            //    {
            //        int length;
            //        buffer.GetLengthOfLine(line, out length);
            //        pCodeSpan[0].iStartIndex = 0;
            //        pCodeSpan[0].iEndIndex = length;
            //    }
            //    return VSConstants.S_OK;
            //}
            //else
            {
                return VSConstants.S_FALSE;
            }
        }

        public override ViewFilter CreateViewFilter(CodeWindowManager mgr, IVsTextView newView)
        {
            return new IronyViewFilter(mgr, newView);
        }

        public override string Name
        {
            get { return Configuration.Name; }
        }

        public override string GetFormatFilterList()
        {
            return Configuration.FormatList;
        }
    }
}