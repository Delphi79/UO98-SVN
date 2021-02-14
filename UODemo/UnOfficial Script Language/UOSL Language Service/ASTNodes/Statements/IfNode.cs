using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace JoinUO.UOSL.Service.ASTNodes
{
    class IfNode : StatementNode
    {
        ExpressionNode Expression;
        ScopedNode Statement;
        ScopedNode StatementElse;

        public override bool needsSemi { get { return false; } }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            LanguageOption Options = context.GetOptions();

            Expression = ExpressionNode.Reduce(treeNode.ChildNodes[1]);
            ChildNodes.Add(Expression);
            Expression.Parent = this;

            if (treeNode.ChildNodes.Count > 3)
            {
                Statement = StatementNode.GetStatement(treeNode.ChildNodes[2], context) as ScopedNode;
                if (Statement != null)
                {
                    ChildNodes.Add(Statement);
                    Statement.Parent = this;
                }
            }
            else Statement = null;

            if (!Options.HasOption(LanguageOption.UnBracedLoopsIfs) && !(Statement is BlockNode))
                context.AddParserMessage(ParserErrorLevel.Error, Statement == null ? treeNode.Span : Statement.Span, "Statement must be enclosed in a block.");

            if (treeNode.LastChild.ChildNodes.Count > 0)
            {
                if (treeNode.LastChild.FirstChild.ChildNodes.Count > 1)
                {
                    StatementElse = StatementNode.GetStatement(treeNode.LastChild.FirstChild.ChildNodes[1], context) as ScopedNode;
                    if (StatementElse != null)
                    {
                        ChildNodes.Add(StatementElse);
                        StatementElse.Parent = this;
                    }
                }
                else StatementElse = null;

                if (!Options.HasOption(LanguageOption.UnBracedLoopsIfs) && !(StatementElse is BlockNode))
                    context.AddParserMessage(ParserErrorLevel.Error, StatementElse == null ? treeNode.LastChild.FirstChild.Span : StatementElse.Span, "Statement must be enclosed in a block.");
            }
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            StringBuilder sb = new StringBuilder(string.Format("{0}{1}{2}{3}{4}\n", Indenter(indentationlevel), Keyword.If.Value, Punct.LPara.Value, Expression.GenerateScript(options), Punct.RPara.Value));
            if (!options.HasOption(LanguageOption.UnBracedLoopsIfs) && !(Statement is BlockNode))
                sb.Append(BlockNode.GenerateBlock(Statement == null ? null : new IStatement[] { (IStatement)Statement }, options, indentationlevel));
            else
                sb.Append(Statement.GenerateScript(options,indentationlevel));
            if (StatementElse != null)
            {
                sb.Append('\n');
                sb.Append(Indenter(indentationlevel));
                sb.Append(Keyword.Else.Value);
                sb.Append('\n');
                if (!options.HasOption(LanguageOption.UnBracedLoopsIfs) && !(StatementElse is BlockNode))
                    sb.Append(BlockNode.GenerateBlock(StatementElse == null ? null : new IStatement[] { (IStatement)StatementElse }, options, indentationlevel));
                else
                    sb.Append(StatementElse.GenerateScript(options, indentationlevel));
            }
            return sb.ToString();
        }
    }
}
