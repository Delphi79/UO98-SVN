#region License
/* **********************************************************************************
 * Copyright (c) Roman Ivantsov
 * This source code is subject to terms and conditions of the MIT License
 * for Irony. A copy of the license can be found in the License.txt file
 * at the root of this distribution. 
 * By using this source code in any fashion, you are agreeing to be bound by the terms of the 
 * MIT License.
 * You must not remove this notice from this software.
 * **********************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Irony.Parsing;
using Irony.Interpreter;
using Irony.Ast;
using Irony;
using JoinUO.UOdemoSDK;

namespace JoinUO.UOSL.Service.ASTNodes
{

    class MemberDeclarationNode : VariableDeclarationNode
    {
        protected override void InitChildren(ParseTreeNode treeNode)
        {
            Declaration = treeNode.LastChild.FirstChild.AstNode as DeclarationNode;
            Assign = treeNode.LastChild.LastChild.AstNode as AssignNode;
            Name = Declaration.Name;
            TypeToken = Declaration.Name;
            AsString = string.Format("MemberDeclaration: {0}",Name);
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            return Indenter(indentationlevel, "{0} {1}", Keyword.Member.Value, base.GenerateScript(options));
        }
    }

    class ConstantDeclarationNode : VariableDeclarationNode
    {
        public Field field { get; private set; }

        public override bool needsSemi { get { return false; } }

        protected override void InitChildren(ParseTreeNode treeNode)
        {
            Declaration = treeNode.LastChild.FirstChild.ChildNodes[0].AstNode as DeclarationNode;
            Assign = treeNode.LastChild.LastChild.LastChild.AstNode as AssignNode;
            Name = Declaration.Name;
            TypeToken = Declaration.Name;
            AsString = string.Format("ConstantDeclaration: {0}", Name);
        }

        public struct ConstantListElement
        {
            public ConstantListElement(UoToken uotoken, UoToken uotypetoken)
            {
                UoToken = uotoken;
                UoTypeToken = uotypetoken;
            }
            public UoToken UoToken;
            public UoToken UoTypeToken;
        }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            LanguageOption Options = context.GetOptions();

            field = new Field(this, context, true);
            if (!Options.HasOption(LanguageOption.Constants))
                context.AddParserMessage(ParserErrorLevel.Error, this.Span, "Constants are not valid for this source.");
            else if (Assign==null || Assign.Expression.UoToken == null || !Assign.Expression.UoToken.IsLiteral)
            {
                if (Assign!=null && Assign.Expression.AsString == "ExpressionList")
                {
                    foreach (ExpressionNode node in Assign.Expression.ChildNodes)
                        if (node.UoToken == null || !node.UoToken.IsLiteral)
                            context.AddParserMessage(ParserErrorLevel.Error, node.Span, "Constant list elements must be compile time constants.");

                    field.Value = Assign.Expression.ChildNodes.Select(node => new ConstantListElement( ((ExpressionNode)node).UoToken,((ExpressionNode)node).UoTypeToken));

                }
                else
                {
                    context.AddParserMessage(ParserErrorLevel.Error, Span, "Constants must be assigned a compile time constant value when declared.");
                    field.Value = "0";
                }
            }
            else
                field.Value = Assign.Expression.UoToken.Value;

            UOSLBase.AddMember(field, context);
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            if (options.HasOption(LanguageOption.Constants))
                return Indenter(indentationlevel, "const {0}{1}{2}\n", this.Declaration.GenerateScript(options), Assign == null ? string.Empty : Assign.GenerateScript(options), Punct.Semi.Value);
            else
                return string.Empty;
                //throw new NotImplementedException("The specified language options do not support constants.");
        }
    }

    class VariableDeclarationNode : DeclarationNode
    {
        public DeclarationNode Declaration;
        public AssignNode Assign;

        protected override void InitChildren(ParseTreeNode treeNode)
        {
            if (treeNode.FirstChild.FirstChild.AstNode is DeclarationNode)
            {
                Declaration = treeNode.FirstChild.FirstChild.AstNode as DeclarationNode;
                Assign = treeNode.FirstChild.LastChild.AstNode as AssignNode;
            }
            else
            {
                Declaration = treeNode.FirstChild.AstNode as DeclarationNode;
                Assign = treeNode.LastChild.AstNode as AssignNode;
            }

            Name = Declaration.Name;
            TypeToken = Declaration.Name;
            AsString = "VariableDeclaration";
        }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            ChildNodes.Add(Declaration);
            Declaration.Parent = this;
            this.SetUoTypeToken(Declaration.UoTypeToken);
            if (Assign != null)
            {
                ChildNodes.Add(Assign);
                Assign.Parent = this;
                if (Assign.UoTypeToken != null && this.UoTypeToken != null)
                    CheckScope(context);
            }
        }

        public override void CheckScope(ParsingContext context)
        {
            if (Assign != null)
                Assign.CheckAssignTo(UoTypeToken, context);
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            return Indenter(indentationlevel, "{0}{1}", Declaration.GenerateScript(options), (Assign == null) ? string.Empty : Assign.GenerateScript(options));
        }

    }

    class DeclarationNode : StatementNode
    {
        public virtual string NameString { get { return Name.ValueString; } }
        public Token Name;
        public Token TypeToken;

        public override bool needsSemi { get { return true; } }

        public override UoToken UoTypeToken
        {
            get
            {
                if (TypeToken.KeyTerm is Tokenizer)
                {
                    return ((Tokenizer)TypeToken.KeyTerm).Tokenize();
                }
                else
                    return base.UoTypeToken;
            }
        }

        public Symbol Symbol
        {
            get
            {
                return Name.Symbol;
            }
        }

        protected virtual void InitChildren(ParseTreeNode treeNode)
        {
            Name = treeNode.ChildNodes[1].Token;
            TypeToken = treeNode.FirstChild.Token;
            AsString = TypeToken.Text + " " + Name.Text; //Symbol.Text;
        }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            InitChildren(treeNode);
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            return Indenter(indentationlevel, "{0} {1}", UoTypeToken.Value, Name.Text);
        }

        public override void EvaluateNode(EvaluationContext context, AstMode mode)
        {
            switch (mode)
            {
                case AstMode.Read:
                    object value;
                    if (context.TryGetValue(Symbol, out value))
                        context.Data.Push(value);
                    else
                        context.ThrowError(Resources.ErrVarNotDefined, Symbol);
                    break;
                case AstMode.Write:
                    context.SetValue(Symbol, context.Data.Pop());
                    break;
            }
        }

    }//class
}//namespace
