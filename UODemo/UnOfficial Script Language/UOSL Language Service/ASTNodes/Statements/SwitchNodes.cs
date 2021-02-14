using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;
using JoinUO.UOdemoSDK;

namespace JoinUO.UOSL.Service.ASTNodes
{
    class SwitchNode : StatementNode
    {
        internal ExpressionNode Expression;
        private List<SwitchSectionNode> m_Sections;
        IEnumerable<SwitchSectionNode> Sections { get { return m_Sections; } }

        public override bool needsSemi { get { return false; } }

        public override void Init(Irony.Parsing.ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            Expression = ExpressionNode.Reduce(treeNode.ChildNodes[1]);
            ChildNodes.Add(Expression);
            Expression.Parent=this;
            m_Sections = new List<SwitchSectionNode>(treeNode.LastChild.ChildNodes.Count);
            foreach (ParseTreeNode node in treeNode.LastChild.ChildNodes)
            {
                SwitchSectionNode section = (SwitchSectionNode)node.AstNode;
                m_Sections.Add(section);
                ChildNodes.Add(section);
                section.Parent = this;
            }
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            StringBuilder sb = new StringBuilder(Indenter(indentationlevel,"{0}{1}{2}{3}\n",Keyword.Switch.Value,Punct.LPara.Value,Expression.GenerateScript(options),Punct.RPara.Value));
            
            sb.Append(Indenter(indentationlevel,"{0}\n", Punct.LBrace.Value));

            if(Sections!=null)
                foreach (SwitchSectionNode section in Sections)
                {
                    if (section.isDefault)
                        sb.Append(Indenter(indentationlevel + 1, "{0}{1}\n", Keyword.Default.Value, options.HasOption(LanguageOption.ColonAfterSwitchCase) ? Punct.Colon.Value : string.Empty));
                    foreach (ExpressionNode label in section.Labels)
                        sb.Append(Indenter(indentationlevel + 1, "{0} {1}{2}\n", Keyword.Case.Value, label.GenerateScript(options), options.HasOption(LanguageOption.ColonAfterSwitchCase) ? Punct.Colon.Value : string.Empty));
                    foreach (IStatement statement in section.Statements)
                    {
                        string format = statement.needsSemi ? "{0}{1}\n" : "{0}\n";
                        sb.Append(string.Format(format, statement.GenerateScript(options, indentationlevel + 2), Punct.Semi.Value));
                    }
                }

            sb.Append(Indenter(indentationlevel, "{0}\n", Punct.RBrace.Value));

            return sb.ToString();
        }

        public override void CheckScope(ParsingContext context)
        {
            bool unknown = (Expression.UoTypeToken == null);

            Dictionary<string, ExpressionNode> tokens = new Dictionary<string, ExpressionNode>();

            foreach (SwitchSectionNode section in Sections)
                foreach (ExpressionNode label in section.Labels)
                {
                    if (Expression.UoTypeToken == null) // If we don't know, make a guess that it's the first known type we encounter
                        Expression.SetUoTypeToken(label.UoTypeToken);
                    if (Expression.UoTypeToken != label.UoTypeToken)
                        context.AddParserMessage(ParserErrorLevel.Error, label.Span, "Case label type mismatch, must be {0}.", Expression.UoTypeToken.Value);
                    if(label.UoToken==null)
                        context.AddParserMessage(ParserErrorLevel.Error, label.Span, "Case label undefined: {0}", label.AsString);
                    else if (tokens.ContainsKey(label.UoToken.Value))
                        context.AddParserMessage(ParserErrorLevel.Warning, tokens[label.UoToken.Value].Span, "Duplicate Case label follows, this will not be executed.");
                    else
                        tokens.Add(label.UoToken.Value, label);
                }

            if (unknown)
            {
                if (Expression.UoTypeToken == null)
                    context.AddParserMessage(ParserErrorLevel.Warning, Expression.Span, "Expression type unknown, and could not guess from case labels.");
                else
                    context.AddParserMessage(ParserErrorLevel.Info, Expression.Span, "Expression type unknown, guessing {0} based on case labels.", Expression.UoTypeToken.Value);
            }
            else if (Expression.UoTypeToken != Types.Int)
                context.AddParserMessage(ParserErrorLevel.Warning, Expression.Span, "Expression type must be int.");
        }

    }

    class SwitchSectionNode : ScopedNode
    {
        internal List<ExpressionNode> Labels;
        internal List<IStatement> Statements;

        bool m_isDefault = false;
        public bool isDefault { get { return m_isDefault; } }

        public override void CheckScope(ParsingContext context)
        {
        }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            LanguageOption Options = context.GetOptions();
            bool RequiresColon = Options.HasOption(LanguageOption.ColonAfterSwitchCase);

            ScopedNode labels=(ScopedNode)treeNode.FirstChild.AstNode;
            labels.Parent = this;
            ChildNodes.Add(labels);
            Labels = new List<ExpressionNode>(treeNode.FirstChild.ChildNodes.Count);
            foreach (ParseTreeNode node in treeNode.FirstChild.ChildNodes)
            {
                if (RequiresColon && context.Source.Text[node.Span.EndPosition-1]!=':')
                    context.AddParserMessage(ParserErrorLevel.Error, node.FirstChild.Span, "Colon expected.");
                if (((Tokenizer)node.ChildNodes[0].Term).Tokenize() == Keyword.Default) // kinda hacky
                    m_isDefault = true;
                else
                {
                    ExpressionNode exp = ExpressionNode.Reduce(node.ChildNodes[1]);
                    Labels.Add(exp);
                    labels.ChildNodes.Add(exp);
                    exp.Parent = labels;
                    if(exp.UoTypeToken==null || !exp.UoTypeToken.IsLiteral)
                        context.AddParserMessage(ParserErrorLevel.Error, exp.Span, "A case label must be a literal value.");
                }
            }

            ScopedNode statements = (ScopedNode)treeNode.LastChild.AstNode;
            statements.Parent = this;
            ChildNodes.Add(statements);
            StatementNode.AddStatementList(statements, treeNode.LastChild, context);
            Statements = new List<IStatement>(statements.ChildNodes.Count);
            foreach (AstNode node in statements.ChildNodes)
            {
                if (node is IStatement)
                    Statements.Add((IStatement)node);
            }

            AsString += " " + (isDefault ? "default" : "case");

        }
    }
}
