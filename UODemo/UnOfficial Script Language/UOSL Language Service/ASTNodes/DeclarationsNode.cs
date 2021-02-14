using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace JoinUO.UOSL.Service.ASTNodes
{
    [CLSCompliant(false)]
    public class DeclarationsNode : ScopedNode
    {
        UOSLBase m_grammar;

        public DeclarationsNode(UOSLBase grammar)
        {
            m_grammar = grammar;
        }

        public Dictionary<string, string> Depends = new Dictionary<string, string>();

        public override void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            base.Init(context, parseNode);

            List<Method> ScopeFuncs= UOSLBase.GetFuncs(context);

            CoreCommands.LoadCoreCommands(context);

            LanguageOption Options = context.GetOptions();

            bool canLoadInherit = true;

            foreach (ParseTreeNode cnode in parseNode.ChildNodes)
            {
                ScopedNode toAdd = null;

                if (cnode.AstNode is InheritsNode)
                {
                    if (canLoadInherit)
                    {
                        toAdd = (ScopedNode)cnode.AstNode;
                        InheritsNode iNode = (InheritsNode)cnode.AstNode;
                        if (Depends.ContainsKey(iNode.Filename))
                            context.AddParserMessage(ParserErrorLevel.Error, cnode.Span, "File {0} is already loaded in this inheritance chain, cannot reload.", iNode.Filename);
                        else
                            m_grammar.LoadFile(iNode.Filename, this, context, Depends);
                    }
                    else
                        context.AddParserMessage(ParserErrorLevel.Error, cnode.Span, "Inherits declaration(s) must appear first in the file, may not be made after other declarations.");
                }
                else
                {
                    canLoadInherit = false;

                    if (cnode.AstNode is EventDefNode)
                    {
                        toAdd = (EventDefNode)cnode.AstNode;
                    }
                    else if (cnode.AstNode is UoCoreFunctionNode)
                    {
                        CoreCommands.Funcs.Add(new Method((FunctionProtoNode)cnode.AstNode, context));
                    }
                    else if (cnode.AstNode is FunctionDefNode)
                    {
                        toAdd = (FunctionDefNode)cnode.AstNode;

                        Method found;
                        string name = ((FunctionDefNode)toAdd).NameString;

                        if (ScopeFuncs != null && (found = ScopeFuncs.FirstOrDefault(func => func.Name == name)) != null && found.DefNode == null)
                            found.Define((FunctionDefNode)toAdd, context);
                        else
                            UOSLBase.AddFunc(new Method((FunctionDefNode)toAdd, context), context);
                    }
                    else if (cnode.AstNode is FunctionProtoNode)
                    {
                        toAdd = (FunctionProtoNode)cnode.AstNode;
                        UOSLBase.AddFunc(new Method((FunctionProtoNode)toAdd, context), context);
                    }
                    else if (cnode.AstNode is ConstantDeclarationNode)
                    {
                        ConstantDeclarationNode constnode = (ConstantDeclarationNode)cnode.AstNode;
                        toAdd = constnode;
                    }
                    else if (cnode.AstNode is MemberDeclarationNode)
                    {
                        toAdd = (ScopedNode)cnode.AstNode;
                        if (((MemberDeclarationNode)cnode.AstNode).Assign != null)
                            context.AddParserMessage(ParserErrorLevel.Warning, ((MemberDeclarationNode)cnode.AstNode).Assign.Span, "Member assignments in Declarations are not respected.", ((MemberDeclarationNode)cnode.AstNode).Declaration.Term.Name);
                        UOSLBase.AddMember(new Field((MemberDeclarationNode)cnode.AstNode, context), context); // global scope?
                    }
                    else if (cnode.AstNode is DeclarationNode)
                    {
                        toAdd = (DeclarationNode)cnode.AstNode;
                        AddVar(new Field((DeclarationNode)toAdd, context), context);
                    }
                }

                if (toAdd != null)
                {
                    ChildNodes.Add(toAdd);
                    toAdd.Parent = this;
                }
            }
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            StringBuilder sb = new StringBuilder();
            foreach (ScopedNode node in ChildNodes)
            {
                if (node is FunctionDefNode || node is EventDefNode || node is ConstantDeclarationNode)
                    sb.Append(node.GenerateScript(options));
                else
                {
                    sb.Append(node.GenerateScript(options));
                    sb.Append(Punct.Semi.Value);
                    sb.Append('\n');
                }
            }
            return sb.ToString();
        }
    
    }
}
