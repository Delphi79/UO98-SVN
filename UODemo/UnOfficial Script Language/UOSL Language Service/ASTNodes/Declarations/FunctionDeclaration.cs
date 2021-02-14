#region License
// Based on Irony.Ast.FunctionDefNode:
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
using Irony.Interpreter;
using Irony.Parsing;
using Irony.Ast;
using JoinUO.UOdemoSDK;

namespace JoinUO.UOSL.Service.ASTNodes
{
    [CLSCompliant(false)]
    public class ParametersNode : ScopedNode
    {
        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            foreach (ParseTreeNode node in treeNode.ChildNodes)
            {
                ChildNodes.Add((AstNode)node.AstNode);
                ((AstNode)node.AstNode).Parent = this;
            }
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            if(ChildNodes.Count==0) return "()";
            List<String> paramaters=new List<string>();
            foreach (AstNode node in ChildNodes)
            {
                paramaters.Add(((ScopedNode)node).GenerateScript(options));
            }
            return string.Format("{1}{0}{2}", string.Join(Punct.Comma.Value, paramaters),Punct.LPara.Value,Punct.RPara.Value);
        }

        public static void CreateFromForward(ParsingContext context, ParseTreeNode parseNode)
        {
            ParametersNode node = new ParametersNode();
            node.Init(context, parseNode);
            parseNode.AstNode = node;
            node.ChildNodes.Clear();
            foreach (ParseTreeNode cnode in parseNode.ChildNodes)
            {
                DeclarationNode param = new ProtoParamDeclaration();
                param.Init(context, cnode);
                param.Parent = node;
                node.ChildNodes.Add(param);
            }
        }

        public static void CreateFromExtern(ParsingContext context, ParseTreeNode parseNode)
        {
            ParametersNode node = new ParametersNode();
            node.Init(context, parseNode);
            parseNode.AstNode = node;
            node.ChildNodes.Clear();
            foreach (ParseTreeNode cnode in parseNode.ChildNodes)
            {
                DeclarationNode param = new ProtoParamDeclaration();
                param.Init(context, cnode);
                param.Parent = node;
                node.ChildNodes.Add(param);
            }

        }
    }

    class ProtoParamDeclaration : DeclarationNode
    {
        static int i = 0;
        
        string name;
        public override string NameString { get { return name; } }

        protected override void InitChildren(ParseTreeNode treeNode)
        {

            if (treeNode.LastChild.LastChild.ChildNodes.Count == 1)
            {
                Name = treeNode.LastChild.LastChild.FirstChild.Token;
                name = Name.Text;
            }
            else
            {   // find unique name
                name = string.Format("arg{0}", i++);
            }
            TypeToken = treeNode.FirstChild.FirstChild.Token;
            AsString = TypeToken.Text + " " + name;
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            return TypeToken.ValueString;
        }

        public override void EvaluateNode(EvaluationContext context, AstMode mode)
        {
        }
    }

    class FunctionProtoNode : ScopedNode
    {
        public virtual string NameString { get { return Name.ValueString; } }
        
        public Token Name;
        public Token TypeToken;
        public override UoToken UoTypeToken
        {
            get
            {
                if (TypeToken.KeyTerm is Tokenizer)
                {
                    return ((Tokenizer)TypeToken.KeyTerm).Tokenize();
                }
                else
                    return null;
            }
        }

        public Symbol Symbol
        {
            get
            {
                return Name.Symbol;
            }
        }

        internal ScopedNode DeclNode;
        protected ParametersNode Parameters;

        protected virtual UoToken DeclKeyword { get { return Keyword.Forward; } }
        protected string Label { get { return string.Format("{0}: ", DeclKeyword.Value); } }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            ParseTreeNode declNode = treeNode.ChildNodes[1].FirstChild;
            if (declNode.ToString() == "TypedFunction")
                declNode = declNode.FirstChild;

            TypeToken = declNode.FirstChild.Token;

            Name = declNode.LastChild.Token;
            DeclNode = (ScopedNode)declNode.AstNode;
            Parameters = (ParametersNode)treeNode.ChildNodes[2].AstNode;

            if (TypeToken != null)
                AsString = Label + TypeToken.ValueString + " " + Name.ValueString;

            ChildNodes.Add(DeclNode);
            DeclNode.Parent = this;
            ChildNodes.Add(Parameters);
            Parameters.Parent = this;

            if (UoTypeToken == Types.List && !(this is UoCoreFunctionNode))
                context.AddParserMessage(ParserErrorLevel.Warning, new SourceSpan(TypeToken.Location, TypeToken.Length), "Function may not return a list.");
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            return string.Format("{0} {1} {2}{3}", DeclKeyword.Value, UoTypeToken.Value, Name.Text , Parameters.GenerateScript(options));
        }
    }

    //A node representing function definition
    class FunctionDefNode : FunctionProtoNode, ICallTarget
    {
        public static List<T> FindNodesOfType<T>(ParseTreeNode root, List<T> toReturn = null, bool firstOnly = false) where T : ScopedNode
        {
            if (toReturn == null)
                toReturn = new List<T>();

            foreach (ParseTreeNode node in root.ChildNodes)
            {
                if (node.AstNode is T)
                {
                    toReturn.Add((T)node.AstNode);
                    if (firstOnly) return toReturn;
                }
                else
                    FindNodesOfType(node, toReturn);
            }

            return toReturn;
        }

        ScopedNode Body;
        protected override UoToken DeclKeyword { get { return Keyword.Function; } }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            Body = (ScopedNode)treeNode.ChildNodes[3].AstNode;
            ChildNodes.Add(Body);
            DeclNode.Parent = Parameters.Parent = Body.Parent = this;

            // Add params to scope
            foreach (DeclarationNode node in Parameters.ChildNodes)
            {
                Field field = new Field(node, context);
                AddVar(field, context);
            }
            // Add vars in all child nodes to scope
            foreach (DeclarationNode node in FindNodesOfType<DeclarationNode>(treeNode.ChildNodes[3]))
            {
                Field field = new Field(node, context);
                AddVar(field, context);
            }

            List<ReturnNode> returns = FindNodesOfType<ReturnNode>(treeNode.ChildNodes[3],firstOnly: true);
            bool hasreturns = returns != null && returns.Count > 0;
            if(UoTypeToken!=Types.Void && !hasreturns)
                context.AddParserMessage(Irony.Parsing.ParserErrorLevel.Warning, this.Span, "A non-void function should return an expression.");
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            return string.Format("{0} {1} {2}{3}\n{4}\n", DeclKeyword.Value, UoTypeToken.Value, Name.Text, Parameters.GenerateScript(options), Body.GenerateScript(options));
        }

        //public override void EvaluateNode(EvaluationContext context, AstMode mode)
        //{
        //    //push the function into the stack
        //    context.Data.Push(this);
        //    NameNode.Evaluate(context, AstMode.Write);
        //}

        #region ICallTarget Members

        public void Call(EvaluationContext context)
        {
            //context.PushFrame(this.NameNode.ToString(), this, context.CurrentFrame);
            //Parameters.Evaluate(context, AstMode.None);
            //Body.Evaluate(context, AstMode.None);
            //context.PopFrame();
        }

        #endregion
    }//class

}//namespace
