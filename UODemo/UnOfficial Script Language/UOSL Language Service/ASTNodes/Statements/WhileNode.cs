using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace JoinUO.UOSL.Service.ASTNodes
{
    class WhileNode : StatementNode
    {
        ExpressionNode Expression;
        ScopedNode Statement;

        public override bool needsSemi { get { return false; } }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            LanguageOption Options = context.GetOptions();

            Expression = ExpressionNode.Reduce(ExpressionNode.Dig(treeNode.ChildNodes[1]));
            ChildNodes.Add(Expression);
            Expression.Parent = this;

            if (treeNode.ChildNodes.Count > 2)
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
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            StringBuilder sb = new StringBuilder(Indenter(indentationlevel, "{0}{1}{2}{3}\n", Keyword.While.Value, Punct.LPara.Value, Expression.GenerateScript(options), Punct.RPara.Value));
            if (!options.HasOption(LanguageOption.UnBracedLoopsIfs) && !(Statement is BlockNode))
                sb.Append(BlockNode.GenerateBlock(Statement==null ? null : new IStatement[] { (IStatement)Statement }, options, indentationlevel));
            else
                sb.Append(((ScopedNode)Statement).GenerateScript(options, indentationlevel));

            return sb.ToString();
        }

    }
}
