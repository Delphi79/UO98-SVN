using System;
using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Package;
using JoinUO.UOSL.Service;

namespace JoinUO.UOSL.Package
{
    public class AuthoringScope : Microsoft.VisualStudio.Package.AuthoringScope
    {
        public AuthoringScope(Source source, object parseResult)
        {
            _source = source;
            this.parseResult = parseResult;

            if (resolver == null || parseResult != null)
                this.resolver = new Resolver(source, parseResult);
        }

        Source _source;
        object parseResult;
        public object LastParseResult
        {
            get { return parseResult; }
            set 
            {
                if (value != parseResult)
                {
                    parseResult = value;
                    resolver = new Resolver(_source, value);
                }
            }
        }


        IASTResolver resolver;

        // ParseReason.QuickInfo
        public override string GetDataTipText(int line, int col, out TextSpan span)
        {
            span = new TextSpan();
            return null;
        }

        // ParseReason.CompleteWord
        // ParseReason.DisplayMemberList
        // ParseReason.MemberSelect
        // ParseReason.MemberSelectAndHilightBraces
        public override Microsoft.VisualStudio.Package.Declarations GetDeclarations(IVsTextView view, int line, int col, TokenInfo info, ParseReason reason)
        {
            IList<Declaration> declarations;
            switch (reason)
            {
                case ParseReason.CompleteWord:
                    declarations = resolver.FindCompletions(parseResult, line, col);
                    break;
                case ParseReason.DisplayMemberList:
                case ParseReason.MemberSelect:
                case ParseReason.MemberSelectAndHighlightBraces:
                    declarations = resolver.FindMembers(parseResult, line, col);
                    break;
                default:
                    throw new ArgumentException("reason");
            }

            return new Declarations(declarations);
        }

        // ParseReason.GetMethods
        public override Microsoft.VisualStudio.Package.Methods GetMethods(int line, int col, string name)
        {
            return new Methods(resolver.FindMethods(parseResult, line, col, name));
        }

        // ParseReason.Goto
        public override string Goto(VSConstants.VSStd97CmdID cmd, IVsTextView textView, int line, int col, out TextSpan span)
        {
            span = new TextSpan();
            return null;
        }
    }
}