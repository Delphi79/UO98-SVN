using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;
using JoinUO.UOdemoSDK;

namespace JoinUO.UOSL.Service.ASTNodes
{
    class EventParamNode<TParam> : ScopedNode where TParam : UoToken.UoTypeName
    {
        private UoToken m_UoToken;
        public override UoToken UoToken { get { return m_UoToken; } }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            int index = treeNode.ChildNodes.Count > 1 ? 1 : 0;
            m_UoToken = ((Tokenizer)treeNode.ChildNodes[index].Term).Tokenize(treeNode.ChildNodes[index].Token.Value);

            if(m_UoToken is UoToken.UoConstantString && ((UoToken.UoConstantString)m_UoToken).Value.Length >127)
                context.AddParserMessage(ParserErrorLevel.Error, treeNode.ChildNodes[index].Span, "The maximum length is 127 characters.");
            
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            if (typeof(TParam) == typeof(UoToken.UoTypeName_int) && UoToken.Value.StartsWith("0x"))
                return int.Parse(UoToken.Value.Substring(2), System.Globalization.NumberStyles.HexNumber).ToString();
            return UoToken.Value;
        }

    }

    abstract class EventDecNode : ScopedNode
    {
        public abstract Type ParamType { get; }
    }

    class EventDecNode<TParam> : EventDecNode where TParam : UoToken.UoTypeName
    {
        public override Type ParamType { get { return typeof(TParam); } }     
    }

    class EventDefNativeNode : EventDefNode
    {
        protected override void LoadNodes(ParsingContext context, ParseTreeNode treeNode)
        {
            ChanceNode = treeNode.ChildNodes[1].ChildNodes.Count > 0 ? (EventParamNode<UoToken.UoTypeName_int>)treeNode.ChildNodes[1].FirstChild.AstNode : null;

            ParseTreeNode declNode = treeNode.ChildNodes[2];
            Name = declNode.FirstChild.Token;
            EventParameter = declNode.ChildNodes.Count > 1 ? (ScopedNode)declNode.LastChild.AstNode : null;

            DeclNode = (EventDecNode)declNode.AstNode;
            //Parameters = null; // (ParametersNode)treeNode.ChildNodes[3].AstNode;
            Body = (ScopedNode)treeNode.ChildNodes[3].AstNode;

            // Get the parameters
            Events.EventDefinition def = Events.GetEventDefinition(Name.Text);
            if (def == null)
                context.AddParserMessage(ParserErrorLevel.Error, this.Span, "Event name {0} not found.", Name.Text);
            else
            {
                // add params
                string[] p = def.Parameters;
                if(p!=null)
                {
                    foreach (string param in p)
                    {
                        UoToken typetoken;

                        string[] split = param.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        switch (split[0])
                        {
                            case "integer":
                                typetoken = Types.Int; break;
                            case "list":
                                typetoken = Types.List; break;
                            case "location":
                                typetoken = Types.Loc; break;
                            case "object":
                                typetoken = Types.Obj; break;
                            case "string":
                                typetoken = Types.String; break;
                            default:
                                context.AddParserMessage(ParserErrorLevel.Error, this.Span, "Event parameters for {0} not resolvable in {1}.", param, Name.Text);
                                typetoken = null; break;

                        }

                        Field field = new Field(split[1], typetoken, this, context);
                        AddVar(field, context);
                    }
                }
            }
        }
    }

    class EventDefNode : ScopedNode
    {
        protected ScopedNode Body;
        protected EventParamNode<UoToken.UoTypeName_int> ChanceNode;
        protected EventDecNode DeclNode;
        protected ScopedNode EventParameter;
        protected ParametersNode Parameters;
        
        public Token Name;

        protected virtual void LoadNodes(ParsingContext context, ParseTreeNode treeNode)
        {
            ChanceNode = treeNode.ChildNodes[1].ChildNodes.Count > 0 ? (EventParamNode<UoToken.UoTypeName_int>)treeNode.ChildNodes[1].FirstChild.AstNode : null;

            ParseTreeNode declNode = treeNode.ChildNodes[2];
            Name = declNode.FirstChild.Token;
            EventParameter = declNode.ChildNodes.Count > 1 ? (ScopedNode)declNode.LastChild.AstNode : null;

            DeclNode = (EventDecNode)declNode.AstNode;
            Parameters = (ParametersNode)treeNode.ChildNodes[3].AstNode;
            Body = (ScopedNode)treeNode.ChildNodes[4].AstNode;
        }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            LoadNodes(context, treeNode);

            if (ChanceNode != null && ChanceNode.UoToken.Value == "0x00")
                context.AddParserMessage(ParserErrorLevel.Warning, this.Span, "Event chance is zero, this trigger will never occur.");

            AsString = "Event: " + Name.ValueString;

            ChildNodes.Add(Body);
            ChildNodes.Add(DeclNode);
            DeclNode.Parent = Body.Parent = this;
            
            if (Parameters != null)
            {
                ChildNodes.Add(Parameters);
                Parameters.Parent = this;
            }

            if (ChanceNode != null)
            {
                ChildNodes.Add(ChanceNode);
                ChanceNode.Parent = this;
            }

            if (EventParameter != null)
            {
                ChildNodes.Add(EventParameter);
                EventParameter.Parent = this;
            }

            // Add params to scope (unless null *native*)
            if(Parameters!=null)
                foreach (DeclarationNode node in Parameters.ChildNodes)
                {
                    Field field = new Field(node, context);
                    AddVar(field, context);
                }
            // Add vars in all child nodes to scope
            foreach (DeclarationNode node in FunctionDefNode.FindNodesOfType<DeclarationNode>(treeNode.LastChild))
            {
                Field field = new Field(node, context);
                AddVar(field, context);
            }
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            if (!options.HasOption(LanguageOption.UOSLTriggers))
            {  // Native
                return string.Format("{0}{1} {2}{3}\n{4}\n", Keyword.Trigger.Value,
                    ChanceNode == null ? string.Empty : string.Format(" {0}",((ScopedNode)ChanceNode).GenerateScript(options)),
                    Name.Text,
                    EventParameter == null ? string.Empty : string.Format("{0}{1}{2}", Punct.LPara.Value, ((ScopedNode)EventParameter).GenerateScript(options), Punct.RPara.Value),
                    Body.GenerateScript(options));
            }

            string functparams;
            if (Parameters == null)
            {
                string[] s = Events.GetEventParameters(Name.Text);
                functparams = string.Format("{1}{0}{2}", s == null ? string.Empty : string.Join(Punct.Comma.Value, s), Punct.LPara.Value, Punct.RPara.Value);
            }
            else
                functparams = Parameters.GenerateScript(options);

            return string.Format("{0}{1} {2}{3}{4}\n{5}\n", Keyword.Trigger.Value,
                ChanceNode == null ? string.Empty : string.Format("{0}{1}{2}", Punct.LChev.Value, ((ScopedNode)ChanceNode).GenerateScript(options), Punct.RChev.Value),
                Name.Text,
                EventParameter == null ? string.Empty : string.Format("{0}{1}{2}", Punct.LChev.Value, ((ScopedNode)EventParameter).GenerateScript(options), Punct.RChev.Value),
                functparams, Body.GenerateScript(options));
        }
    }
}
