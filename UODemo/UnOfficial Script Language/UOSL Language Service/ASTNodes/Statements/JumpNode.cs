using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoinUO.UOdemoSDK;

namespace JoinUO.UOSL.Service.ASTNodes
{
    class JumpNode : StatementNode
    {
        public override bool needsSemi { get { return true; } }

        public override void Init(Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            SetUoToken((treeNode.FirstChild.Term as Tokenizer).Tokenize());
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            return Indenter(indentationlevel, UoToken.Value);
        }
    }

    class ReturnNode : JumpNode
    {
        ExpressionNode optExpression;

        public override void Init(Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            if (treeNode.ChildNodes[1].ChildNodes.Count > 0)
            {
                Irony.Parsing.ParseTreeNode n = ExpressionNode.Dig(treeNode.ChildNodes[1].ChildNodes[0]);
                LanguageOption Options = context.GetOptions();
                if (n != null)
                {
                    optExpression = ExpressionNode.Reduce(n);
                    if (optExpression != null)
                    {
                        
                        optExpression.Parent = this;
                        ChildNodes.Add(optExpression);
                        if (Options!=LanguageOption.Extended && context.Source.Text[optExpression.Location.Position-1] != '(')
                            context.AddParserMessage(Irony.Parsing.ParserErrorLevel.Error, ChildNodes[0].Span, "Return statement value must be parenthesized.");

                    }
                }
                else
                    if (Options==LanguageOption.Extended && context.Source.Text[treeNode.ChildNodes[1].Span.Location.Position] == '(')
                        context.AddParserMessage(Irony.Parsing.ParserErrorLevel.Error, treeNode.ChildNodes[1].Span, "Empty parenthesis are not valid here (UOSL Extended).");

            }
            if (treeNode.ChildNodes[1].ChildNodes.Count > 1)
                context.AddParserMessage(Irony.Parsing.ParserErrorLevel.Error, ChildNodes[1].Span, "Unexpected argument.");
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            if (optExpression == null /*&& options!= LanguageOption.Native && !options.HasOption(LanguageOption.ExtraPunct)*/) return base.GenerateScript(options, indentationlevel);
            return string.Format("{0}{1}{2}{3}", base.GenerateScript(options, indentationlevel), Punct.LPara.Value, optExpression == null ? string.Empty : optExpression.GenerateScript(options), Punct.RPara.Value);
        }

        public override void CheckScope(Irony.Parsing.ParsingContext context)
        {
            // find function
            ScopedNode parent = this.Parent as ScopedNode;
            while (parent != null && !(parent is FunctionDefNode))
                parent = parent.Parent as ScopedNode;
            if (parent != null)
            {
                if (parent.UoTypeToken == Types.Void)
                {
                    if (optExpression != null)
                        context.AddParserMessage(Irony.Parsing.ParserErrorLevel.Error, this.Span, "A void function may not return an expression.");
                }
                else if (optExpression == null)
                    context.AddParserMessage(Irony.Parsing.ParserErrorLevel.Error, this.Span, "This function must return an expression of type {0}.", parent.UoTypeToken.Value);
                else
                {
                    if (parent.UoTypeToken != optExpression.UoTypeToken)
                    {
                        if (optExpression.UoTypeToken == null)
                            context.AddParserMessage(Irony.Parsing.ParserErrorLevel.Info, optExpression.Span, "Return type indeterminate must be of type {0} at runtime.", parent.UoTypeToken.Value);
                        else
                            context.AddParserMessage(Irony.Parsing.ParserErrorLevel.Error, optExpression.Span, "Return type must be of type {0}. This expression evaluates to {1}", parent.UoTypeToken.Value, optExpression.UoTypeToken.Value);
                    }
                }
            }
        }

    }
}
