using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;
using JoinUO.UOdemoSDK;

namespace JoinUO.UOSL.Service.ASTNodes
{
    [CLSCompliant(false)]
    public class ScopedNode : AstNode
    {
        internal List<Field> m_LocalVars;

        IEnumerable<Field> m_eVars = null;
        public IEnumerable<Field> ScopeVars
        {
            get
            {
                if (m_eVars != null) return m_eVars;

                ScopedNode node = this;
                while (node != null)
                {
                    if (node.m_LocalVars != null)
                    {
                        if (m_eVars == null)
                            m_eVars = node.m_LocalVars.AsEnumerable();
                        else
                            m_eVars = m_eVars.Concat(node.m_LocalVars.AsEnumerable());
                    }
                    node = node.Parent as ScopedNode;
                }
                return m_eVars;
            }
        }

        UoToken m_UoTypeToken = null;
        public virtual UoToken UoTypeToken { get { return m_UoTypeToken; } }
        
        UoToken m_UoToken = null;
        public virtual UoToken UoToken { get { return m_UoToken; } }

        public ParseTreeNode TreeNode;

        public ScopedNode() : base() { }

        public void CheckTree(ParsingContext context)
        {
            foreach (AstNode node in ChildNodes)
                if(node is ScopedNode)
                ((ScopedNode)node).CheckTree(context);
            CheckScope(context);
        }

        public virtual void CheckScope(ParsingContext context)
        {
            if (m_LocalVars != null)
            {
                List<Field> members = UOSLBase.GetMembers(context);
                foreach (Field field in m_LocalVars)
                {
                    Field found = null;
                    if (members != null && (found = members.FirstOrDefault(member => member.Name == field.Name)) != null && found.Node != field.Node && found.Node!=null && !(field.Node is MemberDeclarationNode) )
                    {
                        if(context.GetOptions()==LanguageOption.Extended)
                            context.AddParserMessage(ParserErrorLevel.Error, field.Node.Span, "Cannot declare a local variable {0}. Member field {0} is declared.", field.Name);
                        else if (found.UoTypeToken == field.UoTypeToken)
                            context.AddParserMessage(ParserErrorLevel.Info, field.Node.Span, "Member field {0} is also declared, this local variable is OK.", field.Name);
                        else
                            context.AddParserMessage(ParserErrorLevel.Info, field.Node.Span, "Member Field {0} is also declared as a different type, this local variable is OK.", field.Name);
                    }
                }
                if (TreeFuncs != null)
                {
                    Method found;

                    foreach (Field field in m_LocalVars)
                        if ((found = TreeFuncs.FirstOrDefault(func => func.Name == field.Name)) != null)
                            context.AddParserMessage(ParserErrorLevel.Error, field.Node.Span, "A function {0} is already declared with the same name.", field.Name);
                }
            }
        }

        public IEnumerable<Method> TreeFuncs { get; private set; }

        public IEnumerable<Terminal> ExpectedTerms { get; private set; }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            ExpectedTerms = new List<Terminal>(treeNode.State.ExpectedTerminals);

            TreeFuncs = UOSLBase.GetFuncs(context);

            TreeNode = treeNode;

            if (treeNode.Token != null && treeNode.Token.KeyTerm is Tokenizer)
            {
                m_UoTypeToken = ((Tokenizer)treeNode.Token.KeyTerm).TokenType;
            }
            else if (treeNode.ChildNodes.Count==1 && treeNode.FirstChild.AstNode is ScopedNode)
            {
                SetUoToken(((ScopedNode)treeNode.FirstChild.AstNode).UoToken);
                SetUoTypeToken(((ScopedNode)treeNode.FirstChild.AstNode).UoTypeToken);
            }
        }

        public void SetUoTypeToken(UoToken token)
        {
            m_UoTypeToken = token;
        }

        public void SetUoToken(UoToken token)
        {
            m_UoToken = token;
        }

        public virtual void AddVar(Field field, ParsingContext context)
        {
            Field found=null;

            if (UOSLGrammar.Keywords.ContainsKey(field.Name) && context.GetOptions()==LanguageOption.Extended)
                context.AddParserMessage(ParserErrorLevel.Info, field.Node.Span, "{0} is a keyword. It cannot be used as a Field name.", field.Name);

            if (ScopeVars != null && (found = ScopeVars.FirstOrDefault(var => var.Name == field.Name)) != null)
            {
                LanguageOption Options = context.GetOptions();

                if (Options == LanguageOption.Extended)
                    context.AddParserMessage(ParserErrorLevel.Error, field.Node.Span, "Field {0} is already declared as a different type.", field.Name);
                else if (found.UoTypeToken != field.UoTypeToken)
                    {
                        // This is OK in Native mode, but an warning in Enhanced mode
                        context.AddParserMessage(Options == LanguageOption.Native ? ParserErrorLevel.Warning : ParserErrorLevel.Error
                            , field.Node.Span, "Field {0} is already declared as a different type, declaration ignored.", field.Name);
                    }
                else
                    context.AddParserMessage(ParserErrorLevel.Info, field.Node.Span, "Redeclaration of Field {0} ignored.", field.Name);
            }
            if (found == null) // ignore redeclaraions
            {
                field.Scope = this;
                (m_LocalVars ?? (m_LocalVars = new List<Field>())).Add(field);
            }
            else
                found.AddReference(this);
        }

        private static StringBuilder _Indenter = new StringBuilder();
        protected static string Indenter(int indent)
        {
            if(indent==0) return string.Empty;
            if(indent==_Indenter.Length) return _Indenter.ToString();
            if (_Indenter.Length > indent)
                _Indenter.Remove(indent, _Indenter.Length - indent);
            else
                _Indenter.Append(' ', indent - _Indenter.Length);
            return _Indenter.ToString();
        }

        protected static string Indenter(int indent, string format, params string[] args)
        {
            return (string.Format("{0}{1}", Indenter(indent), string.Format(format, args)));
        }

        public virtual string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            if (ChildNodes.Count == 0 || ChildNodes[0]==null)
                return string.Empty;
            if (ChildNodes.Count == 1)
                return (((ScopedNode)ChildNodes[0]).GenerateScript(options));

            StringBuilder sb = new StringBuilder(ChildNodes.Count);
            int i=0;
            try
            {
                foreach (ScopedNode node in ChildNodes)
                {
                    i++;
                    sb.Append(node.GenerateScript(options, indentationlevel));
                }
            }
            catch (InvalidCastException ex)
            {
                throw new NotSupportedException(string.Format("Node {0} is not a scoped node", ChildNodes[i]), ex);
            }
            return sb.ToString();

        }

    }

}
