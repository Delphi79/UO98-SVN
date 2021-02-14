using System;
using System.Collections.Generic;
using Irony.Parsing;
using JoinUO.UOSL.Service;
using JoinUO.UOSL.Service.ASTNodes;

namespace JoinUO.UOSL.Package
{
    public class Resolver : IASTResolver
    {
        #region IASTResolver Members

        ScopedNode _node;
        Source _source;

        public Resolver(Source source, object parseResult)
        {
            _node = parseResult as ScopedNode;
            _source = source;
        }

        public IList<Declaration> FindCompletions(object result, int line, int col)
        {
            // Used for intellisense.
            List<Declaration> declarations = new List<Declaration>();

            // Add keywords defined by grammar

            IEnumerable<KeyTerm> keys = (Configuration.Grammar is UOSLGrammar) ? UOSLGrammar.Keywords.Values : Configuration.Grammar.KeyTerms.Values;

            foreach (KeyTerm key in keys)
            {
                //if (key.FlagIsSet(TermFlags.IsKeyword))
                {
                    declarations.Add(new Declaration("", key.Text, 206, key.Name));
                }
            }

            //if (_node != null)
            //{
            //    ScopedNode found = FindNode(_node.TreeNode, line, col);

            //    if (found != null)
            //    {
            //        if (found.TreeFuncs != null)
            //            foreach (Method func in found.TreeFuncs)
            //                declarations.Add(new Declaration(func.Description, func.Name, 207, func.Name));
            //        if (found.ScopeVars != null)
            //            foreach (Field field in found.ScopeVars)
            //                declarations.Add(new Declaration(field.Description, field.Name, 208, field.Name));
            //    }
            //}

            declarations.Sort();
            return declarations;
        }

        private ScopedNode FindNode(ParseTreeNode rootnode, int line, int col)
        {
            ScopedNode found = null;

            foreach (ParseTreeNode childnode in rootnode.ChildNodes)
            {
                ScopedNode node = childnode.AstNode as ScopedNode;
                if (node != null)
                {
                    int endline, endcol;

                    try
                    {
                        _source.GetLineIndexOfPosition(node.Span.EndPosition, out endline, out endcol);

                        if ((node.Location.Line < line && endline > line) || (node.Location.Line == line && node.Location.Column >= col && (endline > line || endcol >= col)))
                        {
                            found = FindNode(node.TreeNode, line, col) ?? node as ScopedNode;
                        }
                    }
                    catch (ArgumentException) { }
                }
            }

            return found;
        }

        public IList<Declaration> FindMembers(object result, int line, int col)
        {
            List<Declaration> members = new List<Declaration>();

            if (_node != null)
            {
                ScopedNode found = FindNode(_node.TreeNode, line, col);

                if (found != null)
                {
                    if (found.TreeFuncs != null)
                        foreach (Method func in found.TreeFuncs)
                            members.Add(new Declaration(func.Description, func.Name, 207, func.Name));
                    if (found.ScopeVars != null)
                        foreach (Field field in found.ScopeVars)
                            members.Add(new Declaration(field.Description, field.Name, 208, field.Name));
                }
            }

            return members;
        }

        public string FindQuickInfo(object result, int line, int col)
        {
            return "unknown";
        }

        public IList<Method> FindMethods(object result, int line, int col, string name)
        {

            return new List<Method>();
        }

        #endregion
    }
}
