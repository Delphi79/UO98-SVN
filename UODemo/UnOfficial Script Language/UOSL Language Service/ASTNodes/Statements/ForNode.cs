using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace JoinUO.UOSL.Service.ASTNodes
{
    class ForNode : StatementNode
    {
        IStatement LoopInitializer;
        ExpressionNode LoopExpression;
        IStatement LoopStatement;
        IStatement Loop;

        public override bool needsSemi { get { return false; } }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            LanguageOption Options = context.GetOptions();

            ParseTreeNode forBlock = treeNode.ChildNodes[1].FirstChild;
            if (forBlock.ChildNodes[0].ChildNodes.Count > 0 && forBlock.ChildNodes[0].FirstChild.ChildNodes.Count > 0)
            {
                LoopInitializer = StatementNode.GetStatement(forBlock.ChildNodes[0].FirstChild.FirstChild, context) as IStatement;
                if (LoopInitializer != null)
                {
                    ((ScopedNode)LoopInitializer).Parent = this;
                    ChildNodes.Add(((ScopedNode)LoopInitializer));
                }
            }

            if (forBlock.ChildNodes[1].ChildNodes.Count > 0 && forBlock.ChildNodes[1].FirstChild.ChildNodes.Count > 0)
            {
                LoopExpression = ExpressionNode.Reduce(forBlock.ChildNodes[1].FirstChild.FirstChild);
                if (LoopExpression is ScopedNode)
                {
                    ((ScopedNode)LoopExpression).Parent = this;
                    ChildNodes.Add(((ScopedNode)LoopExpression));
                }
            }

            if (forBlock.ChildNodes[2].ChildNodes.Count > 0 && forBlock.ChildNodes[2].FirstChild.ChildNodes.Count > 0)
            {
                LoopStatement = StatementNode.GetStatement(forBlock.ChildNodes[2].FirstChild.FirstChild, context) as IStatement;
                if (LoopStatement != null)
                {
                    ((ScopedNode)LoopStatement).Parent = this;
                    ChildNodes.Add(((ScopedNode)LoopStatement));
                }
            }

            ScopedNode LoopNode = StatementNode.GetStatement(treeNode.LastChild, context, false);
            Loop = LoopNode as IStatement;
            if (Loop != null)
            {
                ((ScopedNode)Loop).Parent = this;
                ChildNodes.Add(((ScopedNode)Loop));
            }
            else
                context.AddParserMessage(ParserErrorLevel.Error, treeNode.LastChild.Span, "Loop statement is invalid.");

            if (!Options.HasOption(LanguageOption.UnBracedLoopsIfs) && !(Loop is BlockNode))
                context.AddParserMessage(ParserErrorLevel.Error, LoopNode.Span, "Statement must be enclosed in a block.");

            if (LoopExpression == null || !(LoopInitializer is AssignmentNode || LoopInitializer is VariableDeclarationNode))
                if (Options!=LanguageOption.Extended) // Extended will be normalized without error
                {
                    if (!(LoopInitializer is AssignmentNode || LoopInitializer is VariableDeclarationNode || LoopInitializer==null))
                        context.AddParserMessage(ParserErrorLevel.Error, forBlock.ChildNodes[0].Span, "Loop initializer must be empty, or an assignment or declaration.");
                    if (LoopExpression == null)
                        context.AddParserMessage(ParserErrorLevel.Error, forBlock.ChildNodes[1].Span, "Loop expression cannot be empty.");
                }
                else
                    context.AddParserMessage(ParserErrorLevel.Info, forBlock.ChildNodes[0].Span, "Invalid For loop will be converted to a While.");
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            StringBuilder sb;

            if (LoopExpression == null || !(LoopInitializer is AssignmentNode || LoopInitializer is VariableDeclarationNode))
            {   // Convert to a while loop for compatibility

                sb = new StringBuilder();
                if (LoopInitializer != null)
                    sb.Append(Indenter(indentationlevel, "{0}{1}\n", ((ScopedNode)LoopInitializer).GenerateScript(options), Punct.Semi.Value));
                sb.Append(Indenter(indentationlevel, "{0}{1}{2}{3}\n", Keyword.While.Value,
                    Punct.LPara.Value, LoopExpression == null ? "1" : LoopExpression.GenerateScript(options), Punct.RPara.Value));
                List<IStatement> statements = new List<IStatement>();
                if (Loop is BlockNode)
                    statements.AddRange(((BlockNode)Loop).ChildNodes.Select(node => node as IStatement));
                if (LoopStatement != null)
                    statements.Add(LoopStatement as IStatement);
                sb.Append(BlockNode.GenerateBlock(statements.ToArray(), options, indentationlevel));
            }
            else
            {
                sb = new StringBuilder(Indenter(indentationlevel, "{0}{1}{2}{5}{3}{5}{4}{6}\n", Keyword.For.Value, Punct.LPara.Value,
                    LoopInitializer == null ? string.Empty : ((ScopedNode)LoopInitializer).GenerateScript(options),
                    LoopExpression == null ? string.Empty : LoopExpression.GenerateScript(options),
                    LoopStatement == null ? string.Empty : ((ScopedNode)LoopStatement).GenerateScript(options),
                    Punct.Semi.Value, Punct.RPara.Value));
                if (Loop == null)
                    sb.Append("{}");
                else if (!options.HasOption(LanguageOption.UnBracedLoopsIfs) && !(Loop is BlockNode))
                    sb.Append(BlockNode.GenerateBlock(new IStatement[] { Loop }, options, indentationlevel));
                else
                    sb.Append(((ScopedNode)Loop).GenerateScript(options, indentationlevel));
            }
            return sb.ToString();
        }

        public override void CheckScope(ParsingContext context)
        {
            
        }

    }
}
