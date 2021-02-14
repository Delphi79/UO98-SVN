using System;
using System.Collections.Generic;
using JoinUO.UOdemoSDK;
using JoinUO.UOSL.Service.ASTNodes;
using Irony.Parsing;
using System.Text;

namespace JoinUO.UOSL.Service
{
    public interface IScopedObject
    {
        string Name { get; set; }
        string Description { get; set; }
        string Type { get; set; }
        string DefFilename { get; }
        IList<ScopedNode> References { get; }
    }

    [CLSCompliant(false)]
    public class Field : IScopedObject
    {
        public IList<ScopedNode> References { get; private set; }

        internal Field(string name, UoToken uotypetoken, ScopedNode locationnode, ParsingContext context, bool isconst = false)
        {
            Name = name;
            Description = string.Empty;

            Node = locationnode;
            if (Node != null)
                AddReference(Node);
            
            if (uotypetoken != null)
            {
                UoTypeToken = uotypetoken;
                Type = uotypetoken.Value;
            }
            else
            {
                Type = null;
                UoTypeToken = null;
            }
            Value = null;
            DefFilename = context.CurrentParseTree.FileName;
            isConst = isconst;
        }

        internal Field(DeclarationNode node, ParsingContext context, bool isconst = false)
            : this(node.Name.ValueString, node.UoTypeToken, node, context, isconst)
        {
        }

        internal ScopedNode Node { get; private set; }
        public string Name { get; set; }
        public string DefFilename { get; private set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public UoToken UoTypeToken;
        public object Value;
        public bool isConst { get; private set; }
        public ScopedNode Scope { get; internal set; }

        internal void AddReference(ScopedNode node)
        {
          //  if (node is DeclarationNode) node = ((DeclarationNode)Node).DeclNode;
            if (References == null || !References.Contains(node))
                (References ?? (References = new List<ScopedNode>())).Add(node);
        }
    }

    /// <summary>
    /// A field which is a list will use this object for it's values
    /// </summary>
    [CLSCompliant(false)]
    public struct ListElement
    {
        public UoToken UoTypeToken;
        public object Value;
    }

    [CLSCompliant(false)]
    public class Method : IScopedObject
    {
        public IList<ScopedNode> References { get; private set; }

        internal Method(FunctionProtoNode node, ParsingContext context)
        {
            ForwardNode = node;
            DefNode = null;
            Name = Description = Description = Type = null;
            UoTypeToken = null;
            Parameters = null;
            Load(node, context);
        }

        internal Method(FunctionDefNode node, ParsingContext context)
        {
            ForwardNode = null;
            DefNode = node;
            Name = Description = Description = Type = null;
            UoTypeToken = null;
            Parameters = null;
            Define(node, context);
        }

        internal void AddReference(ScopedNode node)
        {
            if (References == null || !References.Contains(node))
                (References ?? (References = new List<ScopedNode>())).Add(node);
        }

        internal void Define(FunctionDefNode node, ParsingContext context)
        {
            DefNode = node;
            DefFilename = context.CurrentParseTree.FileName;
            Load(node, context);
        }

        private void Load(FunctionProtoNode node, ParsingContext context)
        {
            if (node != null)
                AddReference(node.DeclNode);
            
            Name = string.Intern(node.Name.ValueString);
            Description = string.Empty;
            if (node.UoTypeToken != null)
            {
                if (UoTypeToken != null)
                {
                    if(UoTypeToken != node.UoTypeToken)
                        context.AddParserMessage(ParserErrorLevel.Error, new SourceSpan(context.CurrentToken.Location,context.CurrentToken.Length) , "Re-Definition of {0} does not match prior declaration return type.", Name);
                }
                else
                {
                    UoTypeToken = node.UoTypeToken;
                    Type = node.UoTypeToken.Value;
                }
            }
            else
            {
                Type = null;
                UoTypeToken = null;
            }
            if (node.ChildNodes[1].ChildNodes.Count > 0) // has params
            {
                IList<Parameter> OldParameters = Parameters;

                Parameters = new List<Parameter>(node.ChildNodes[1].ChildNodes.Count);
                foreach (Irony.Ast.AstNode cnode in node.ChildNodes[1].ChildNodes)
                    Parameters.Add(new Parameter((DeclarationNode)cnode));

                if (OldParameters != null)
                {
                    if (OldParameters.Count != Parameters.Count)
                        context.AddParserMessage(ParserErrorLevel.Error, (DefNode ?? ForwardNode).Span , "Re-Definition of {0} does not match prior declaration parameters.", Name);
                    else
                    {
                        for(int i=0;i<Parameters.Count;i++)
                            if(Parameters[i].UoTypeToken != OldParameters[i].UoTypeToken)
                                context.AddParserMessage(ParserErrorLevel.Error, Parameters[i].DecNode.Span, "Re-Definition of {0} does not match prior declaration parameter type for {1}.", Name, Parameters[i].Name);
                    }
                }
                else if(ForwardNode!=null && DefNode!=null && (Parameters!=null || Parameters.Count > 0))
                    context.AddParserMessage(ParserErrorLevel.Error, DefNode.Location, "Re-Definition of {0} does not match prior declaration parameters.", Name);

            }
            else if(Parameters!=null && Parameters.Count > 0)
                context.AddParserMessage(ParserErrorLevel.Error, DefNode.Location, "Re-Definition of {0} does not match prior declaration parameters.", Name);
            else
                Parameters = null;
        }

        internal FunctionProtoNode ForwardNode;
        internal FunctionDefNode DefNode;
        public string DefFilename { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public UoToken UoTypeToken;
        public IList<Parameter> Parameters;

        public string ToString(bool includeType)
        {
            StringBuilder sb = new StringBuilder();
            if (includeType)
            {
                sb.Append(UoTypeToken == null ? "any" : UoTypeToken.Value);
                sb.Append(' ');
            }
            sb.Append(Name);
            sb.Append(Punct.LPara.Value);
            if (Parameters != null)
            {
                foreach (var param in Parameters)
                {
                    if (param.UoTypeToken == null)
                        sb.Append("any ");
                    else
                    {
                        sb.Append(param.UoTypeToken.Value);
                        sb.Append(' ');
                    }
                    sb.Append(param.Name);
                    sb.Append(", ");
                }
                if (sb[sb.Length - 2] == ',')
                    sb.Remove(sb.Length - 2, 2);
            }
            sb.Append(Punct.RPara.Value);

            return string.Intern(sb.ToString());
        }

        public override string ToString()
        {
            return ToString(true);
        }

    }

    [CLSCompliant(false)]
    public class Parameter
    {
        internal Parameter(DeclarationNode node)
        {
            DecNode = node;
            Name = node.NameString;
            Display = node.AsString;
            Description = null;
            UoTypeToken = node.UoTypeToken;
            Type = UoTypeToken == null/*any*/ ? null : UoTypeToken.Value;
        }
        internal DeclarationNode DecNode;
        public string Name { get; set; }
        public string Display;
        public string Description { get; set; }
        public string Type { get; set; }
        public UoToken UoTypeToken;
    }
}