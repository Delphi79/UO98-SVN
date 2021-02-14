using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;

namespace JoinUO.UOSL.Service.ASTNodes
{
    class FunctionInvocationNode : ExpressionNode, IStatement
    {
        protected override bool CanReduce { get { return false; } }

        public bool needsSemi { get { return true; } }

        public Token Name;

        List<ExpressionNode> Arguments;

        protected override void BuildExpression(ParsingContext context, ParseTreeNode treeNode)
        {
            // explicitly do nothing
        }

        //public override UOdemoSDK.UoToken UoTypeToken
        //{
        //    get
        //    {
        //        if (base.UoTypeToken != null) return base.UoTypeToken;
        //        Method found = this.Funcs.Find(method => method.Name == Name.Text);
        //        return found.Name == this.Name.ValueString ? found.UoTypeToken : null;
        //    }
        //}

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Name = treeNode.FirstChild.Token;
            AsString = "Call: " + Name.ValueString;

            if (treeNode.ChildNodes.Count == 2)
            {
                Arguments = new List<ExpressionNode>();
                foreach (ParseTreeNode node in treeNode.LastChild.ChildNodes)
                {
                    ExpressionNode exp = ExpressionNode.Reduce(node);
                    Arguments.Add(exp);
                    ChildNodes.Add(exp);
                    (exp).Parent = this;
                }
            }

        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            return string.Format("{0}{1}{2}{3}{4}", Indenter(indentationlevel), Name.Text, Punct.LPara.Value, string.Join(",", Arguments.Select(arg => arg.GenerateScript(options))), Punct.RPara.Value);
        }

        internal static bool AreSameParams(IList<Parameter> a, IList<ExpressionNode> b, bool nullparamok=false)
        {
            if (a != null && a.Count == 1 && a[0].UoTypeToken == Types.Void)
                return (b == null || b.Count == 0);

            if ((a == null || a.Count == 0) != (b == null || b.Count == 0)) return false;
            if ((a == null || a.Count == 0)) return true;
            if (a.Count != b.Count) return false;

            for (int i = 0; i < a.Count; i++)
                if ((!nullparamok || a[i].UoTypeToken != null) // core commands allow null parameter type, indicating "any"
                    && (a[i].UoTypeToken != b[i].UoTypeToken && b[i].UoTypeToken != null)) // b[i] can be null if it's a list indexer, allow this.
                    return false;
            return true;
        }

        public override void CheckScope(ParsingContext context)
        {
            if (TreeFuncs != null)
            {
                Method found = TreeFuncs.FirstOrDefault(func => func.Name == Name.Text); // look for a script function first
                if (found == null)
                {
                    // if not found, find all matching extern not case sensitive, and check parameters
                    IEnumerable<Method> matchingExterns = null;
                    matchingExterns = CoreCommands.Funcs.Where(func => func.Name.ToLower() == Name.Text.ToLower());
                    if (matchingExterns == null || matchingExterns.Count() == 0)
                        context.AddParserMessage(ParserErrorLevel.Error, this.Span, "Undeclared function {0}.", Name.Text);
                    else if ((found = matchingExterns.FirstOrDefault(func => AreSameParams(func.Parameters, this.Arguments, true))) == null)
                        context.AddParserMessage(ParserErrorLevel.Error, this.Span, "Invalid params for core function {0}.", Name.Text);
                    else
                        found.AddReference(this);
                }
                else
                {
                    // check params
                    if ((Arguments == null || Arguments.Count == 0) != (found.Parameters == null || found.Parameters.Count == 0))
                        context.AddParserMessage(ParserErrorLevel.Error, this.Span, "Argument mismatch.");
                    else if (found.Parameters != null && found.Parameters.Count > 0)
                    {
                        if (Arguments == null || Arguments.Count == 0)
                            context.AddParserMessage(ParserErrorLevel.Error, this.Span, "This function call requires arguments.");
                        else
                            for (int i = 0; i < found.Parameters.Count; i++)
                            {
                                if (i >= Arguments.Count)
                                {
                                    context.AddParserMessage(ParserErrorLevel.Error, this.ChildNodes[this.ChildNodes.Count - 1].Span, "Insufficient arguments.");
                                    break;
                                }
                                if (Arguments[i].UoTypeToken != found.Parameters[i].UoTypeToken)
                                {
                                    if (Arguments[i].UoTypeToken == null)
                                        context.AddParserMessage(ParserErrorLevel.Info, this.ChildNodes[i].Span, "Use of \"any\" as a typed argument.");
                                    else
                                        context.AddParserMessage(ParserErrorLevel.Error, this.ChildNodes[i].Span, "Argument type mismatch.");
                                }
                            }
                        if (Arguments.Count > found.Parameters.Count)
                            context.AddParserMessage(ParserErrorLevel.Error, this.ChildNodes[found.Parameters.Count].Span, "Too many arguments.");
                    }
                    // check location
                    if (found.DefNode == null)
                        context.AddParserMessage(ParserErrorLevel.Error, this.Span, "Function {0} is declared, but not defined.", found.Name);
                    else if (found.DefFilename==context.CurrentParseTree.FileName && found.DefNode.Location.Position > this.Location.Position && (found.ForwardNode == null || found.ForwardNode.Location.Position > this.Location.Position))
                        context.AddParserMessage(ParserErrorLevel.Error, this.Span, "Function {0} must be declared before it is used.", found.Name);
                }
                if (found != null)
                {
                    this.SetUoTypeToken(found.UoTypeToken);
                    found.AddReference(this);
                }
            }
        }
    }
}
