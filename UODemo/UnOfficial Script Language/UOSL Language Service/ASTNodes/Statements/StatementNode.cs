using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace JoinUO.UOSL.Service.ASTNodes
{
    interface IStatement
    {
        // TODO: Execute Statement
        bool needsSemi{get;}
        string GenerateScript(LanguageOption options, int indentationlevel = 0);
    }

    class BlockNode : StatementNode
    {
        public override bool needsSemi { get { return false; } }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            ParseTreeNode cnode = treeNode.FirstChild;
            while (cnode != null && cnode.ChildNodes.Count > 0 && cnode.Term.Name != "Statements")
                cnode = cnode.FirstChild; // find statements.

            if (cnode != null && cnode.Term.Name == "Statements")
                AddStatementList(this, cnode, context);
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            return GenerateBlock(ChildNodes.Select(node => node as IStatement).ToArray(), options, indentationlevel);
        }

        internal static string GenerateBlock(IStatement[] statements, LanguageOption options, int indentationlevel)
        {
            StringBuilder sb = new StringBuilder(Indenter(indentationlevel, "{0}\n", Punct.LBrace.Value));
            if (statements != null)
                for (int i = 0; i < statements.Length; i++)
                {
                    string format = statements[i].needsSemi ? "{0}{1}\n" : "{0}\n";
                    sb.Append(string.Format(format, ((ScopedNode)statements[i]).GenerateScript(options, indentationlevel + 1), Punct.Semi.Value));
                }
            sb.Append(Indenter(indentationlevel, "{0}", Punct.RBrace.Value));
            return sb.ToString();
        }
    }

    abstract class StatementNode : ScopedNode, IStatement
    {
        public abstract bool needsSemi { get; }

        public static void AddStatementList(ScopedNode aststatementsroot, ParseTreeNode parsestatementsroot, ParsingContext context, bool adderrors=true)
        {
            foreach (ParseTreeNode node in parsestatementsroot.ChildNodes)
            {
                ParseTreeNode cnode = null;

                if (node.AstNode is DeclarationNode) // member
                    cnode = node;
                else if (node.ChildNodes.Count > 0)
                {
                    if (node.FirstChild.AstNode is DeclarationNode)
                        cnode = node.FirstChild;
                    else if (node.FirstChild.ChildNodes.Count > 0 && node.FirstChild.FirstChild.AstNode is DeclarationNode)
                        cnode = node.FirstChild.FirstChild;
                }

                if (cnode != null)
                {
                    if (cnode.AstNode is MemberDeclarationNode)
                        UOSLBase.AddMember(new Field((MemberDeclarationNode)cnode.AstNode, context), context);
                    //else if this is a regular variable declaration within a function, the function will add the var to scope when the function node is created
                }

                ScopedNode newnode = GetStatement(node, context, adderrors);
                if (newnode != null)
                {
                    aststatementsroot.ChildNodes.Add(newnode);
                    newnode.Parent = aststatementsroot;
                }
            }

        }

        public static ScopedNode GetStatement(ParseTreeNode node, ParsingContext context, bool adderrors = true)
        {
            ParseTreeNode ToAdd = node;

            // dig
            while ((!(ToAdd.AstNode is IStatement) || (ToAdd.AstNode is ScopedNode && ((ScopedNode)ToAdd.AstNode).AsString == "SemiStatement")) && ToAdd.ChildNodes.Count == 1)
                ToAdd = ToAdd.FirstChild;

            if (ToAdd.AstNode is ExpressionNode && !(ToAdd.AstNode is IStatement)) // error
            {
                context.AddParserMessage(ParserErrorLevel.Warning, node.Span, "Not a statement (Expression).");
                return null;
            }

            if (adderrors && !(ToAdd.AstNode is IStatement))
                context.AddParserMessage(ParserErrorLevel.Error, ((Irony.Ast.AstNode)ToAdd.AstNode).Span, "Not a statement.", ((Irony.Ast.AstNode)ToAdd.AstNode).AsString);

            return ToAdd.AstNode as ScopedNode;
        }

    }
}
