using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using JoinUO.UOdemoSDK;
using Irony.Ast;
using System.Globalization;

namespace JoinUO.UOSL.Service.ASTNodes
{
    class IndexedNode : ExpressionNode
    {
        ExpressionNode Object;
        ExpressionNode Index;

        protected override void BuildExpression(ParsingContext context, ParseTreeNode treeNode)
        {
            AsString = "Indexed";

            Object = (ExpressionNode)treeNode.FirstChild.FirstChild.AstNode;
            Index = Reduce(treeNode.LastChild.FirstChild);

            ChildNodes.Add(Object);
            ChildNodes.Add(Index);
            Object.Parent = Index.Parent = this;

        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            if (UoToken != null) return UoToken.Value;
            if (Object is UoFieldExpressionNode && ((UoFieldExpressionNode)Object).isConst && !options.HasOption(LanguageOption.Constants))
            {
                int index;
                UoFieldExpressionNode node = (UoFieldExpressionNode)Object;
                if (node.UoTypeToken != Types.List)
                    throw new InvalidOperationException(string.Format("Cannot index, This constant expression ({0}) is not a list", node.Name));
                else if (Index.UoToken == null || !Index.UoToken.IsLiteral)
                    throw new InvalidOperationException("Constant list indexers must be compile time constants.");
                else if (!Index.UoToken.Value.TryParseHex(out index))
                    throw new InvalidOperationException("Constant list indexers must be compile time constant integers.");
                else if (index >= Object.ChildNodes.Count)
                    return "0";
                else
                    return ((UoFieldExpressionNode)Object.ChildNodes[index]).GenerateScript(options);
            }
            return string.Format("{0}{1}{2}{3}", Object.GenerateScript(options), Punct.LBrack.Value, Index.GenerateScript(options), Punct.RBrack.Value);
        }

        public override void CheckScope(ParsingContext context)
        {
            if (Object is UoFieldExpressionNode && ((UoFieldExpressionNode)Object).isConst)
            {
                int index;

                UoFieldExpressionNode node = (UoFieldExpressionNode)Object;
                if (node.UoTypeToken != Types.List)
                    context.AddParserMessage(ParserErrorLevel.Error, node.Span, "Cannot index, This constant expression ({0}) is not a list", node.Name);
                else if (Index.UoToken == null || !Index.UoToken.IsLiteral)
                    context.AddParserMessage(ParserErrorLevel.Error, node.Span, "Constant list indexers must be compile time constants.");
                else if (!Index.UoToken.Value.TryParseHex(out index))
                    context.AddParserMessage(ParserErrorLevel.Error, node.Span, "Constant list indexers must be compile time constant integers.");
                else
                {
                    IEnumerable<ConstantDeclarationNode.ConstantListElement> values = (IEnumerable<ConstantDeclarationNode.ConstantListElement>)((UoFieldExpressionNode)Object).Field.Value;
                    if (index < values.Count())
                    {
                        ConstantDeclarationNode.ConstantListElement value = values.ElementAt(index);
                        SetUoToken(value.UoToken);
                        SetUoTypeToken(value.UoTypeToken);
                   }
                }
            }
            else if (Object.UoTypeToken == Types.String)
                SetUoTypeToken(Types.Char);
            else
                SetUoTypeToken(null); // any
        }

    }

    class UnaryPrefixedNode : ExpressionNode
    {
        private ScopedNode m_Operation;
        public ScopedNode Operation { get { return m_Operation; } }
        public UoToken Operator { get { return Operation.UoToken; } }

        ExpressionNode Expression;

        protected override bool CanReduce { get { return false; } }

        UoToken ExpressionType;
        
        protected override void BuildExpression(ParsingContext context, ParseTreeNode treeNode)
        {
            AsString = "UnaryPrefixed";

            m_Operation = (ScopedNode)treeNode.ChildNodes[0].FirstChild.AstNode;
            m_Operation.SetUoToken(((Tokenizer)Operation.TreeNode.Term).Tokenize());

            ChildNodes.Add(Expression = ExpressionNode.Reduce(treeNode.LastChild));
            ChildNodes[0].Parent = this;

            AsString = m_Operation.Term.Name;
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            return string.Format(Expression is BinaryNode ? "{0}{2}{1}{3}" : "{0}{1}", Operator.Value, Expression.GenerateScript(options), Punct.LPara.Value, Punct.RPara.Value);
        }

        public override void CheckScope(ParsingContext context)
        {
            ExpressionType = ((ScopedNode)ChildNodes[0]).UoTypeToken;
            SetUoTypeToken(ExpressionType);
        }

    }


    class BinaryNode : ExpressionNode
    {
        private ScopedNode m_Operation;
        public ScopedNode Operation { get { return m_Operation; } }
        public UoToken Operator { get { return Operation.UoToken; } }

        public ExpressionNode Left, Right;

        UoToken leftType;
        UoToken rightType;

        protected override void BuildExpression(ParsingContext context, ParseTreeNode treeNode)
        {
            m_Operation = (ScopedNode)treeNode.ChildNodes[1].FirstChild.AstNode;
            m_Operation.SetUoToken(((Tokenizer)Operation.TreeNode.Term).Tokenize());

            AsString = m_Operation.Term.Name;

            ChildNodes.Add(Left = Reduce(treeNode.FirstChild));
            ChildNodes.Add(Right = Reduce(treeNode.LastChild));

            Left.Parent = Right.Parent = this;
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            string format;
            if (Right is BinaryNode || Right is UnaryPrefixedNode)
                format = options.HasOption(LanguageOption.ExtraPunct) ? "{1} {0} {3}{2}{4}" : "{1}{0}{3}{2}{4}";
            else if (Right is IndexedNode)
                format = options.HasOption(LanguageOption.ExtraPunct) ? "{1} {0} {3}{2}{4}" : "{1}{0}{3}{2}{4}";
            else
                format = options.HasOption(LanguageOption.ExtraPunct) ? "{1} {0} {2}" : "{1}{0}{2}";
            return string.Format(format, Operator.Value, Left.GenerateScript(options), Right.GenerateScript(options), Punct.LPara.Value, Punct.RPara.Value);
        }

        public override void CheckScope(ParsingContext context)
        {
            leftType = ((ScopedNode)ChildNodes[0]).UoTypeToken;
            rightType = ((ScopedNode)ChildNodes[1]).UoTypeToken;

            // This should be written to use then UoTokens, may need a new function for determining valid types for operations
            switch (Operator.Value)
            {
                case "==":
                case"!=":
                    if ((leftType != null && leftType != Types.Int && leftType != Types.String && leftType != Types.Loc && leftType != Types.Obj) ||
                        (rightType != null && rightType != Types.Int && rightType != Types.String && rightType != Types.Loc && rightType != Types.Obj))
                    {
                        context.AddParserMessage(ParserErrorLevel.Error, m_Operation.Span, "Binary expression: Equivalency can only be performed between int, string, location or object.");
                    }
                    SetUoTypeToken(Types.Int);
                    break;
                case "+":
                    if(leftType==null) leftType=rightType;
                    if ((leftType == Types.String || rightType == Types.Char) && (rightType == Types.String ||
                        rightType == Types.Char || rightType == Types.Int || rightType == Types.Loc) || rightType == null)
                    {
                        SetUoTypeToken(Types.String);

                        // Check length on constant string concatenation
                        if(Left.UoToken is UoToken.UoConstantString && Right.UoToken is UoToken.UoConstantString)
                            if(Left.UoToken.Value.Length + Right.UoToken.Value.Length > 2047)
                                context.AddParserMessage(ParserErrorLevel.Error, m_Operation.Span, "The maximum result length of this expression is 2047 characters.");

                        break;
                    }
                    if (leftType != null || rightType != null)
                        goto case "-";
                    break; // if both null we can't set;
                case "-":
                case "*":
                case "/":
                case "%":
                case "<":
                case ">":
                case "<=":
                case ">=":
                case "^":
                case "&&":
                case "||":
                    if (leftType != null && leftType != Types.Int)
                    {
                        context.AddParserMessage(ParserErrorLevel.Error, Left.Span, "Binary expression ({0}): Integer required.", Operator.Value);
                    }
                    if (rightType != null && rightType != Types.Int)
                    {
                        context.AddParserMessage(ParserErrorLevel.Error, Right.Span, "Binary expression ({0}): Integer required.", Operator.Value);
                    }
                    SetUoTypeToken(Types.Int);
                    break;
                default: // require matched types
                    context.AddParserMessage(ParserErrorLevel.Error, m_Operation.Span, "Binary expression: Unknown operator.");
                    break;
            }
        }
    }

    class UoFieldExpressionNode : ExpressionNode
    {
        internal Token Name;

        internal Field Field;

        public bool isConst { get { return Field != null && Field.isConst; } }

        protected override void BuildExpression(ParsingContext context, ParseTreeNode treeNode)
        {
            // explicitly do nothing
        }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Name = treeNode.Token;
            AsString = Name.Text;
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            if (Field == null || !Field.isConst || options.HasOption(LanguageOption.Constants))
                return Name.ValueString;
            else // const
            {
                if (Field.Value is IEnumerable<ConstantDeclarationNode.ConstantListElement>)
                    return string.Join(",", ((IEnumerable<ConstantDeclarationNode.ConstantListElement>)Field.Value).Select(element => element.UoToken.Value).ToArray());
                else
                    return (Field.Value ?? (object)0).ToString();
            }
        }

        public override void CheckScope(ParsingContext context)
        {
            if (this.ScopeVars != null)
            {
                Field found = this.ScopeVars.FirstOrDefault(field => field.Name == Name.Text);
                if (found == null)
                    context.AddParserMessage(ParserErrorLevel.Error, this.Span, "Undeclared variable. ({0})", Name.Text);
                else
                {
                    if (found.Node!=null && found.Node.Location.Position > this.Location.Position && found.DefFilename == context.CurrentParseTree.FileName)
                        context.AddParserMessage(ParserErrorLevel.Error, this.Span, "{1} {0} must be declared before use.", Name.Text, found.isConst ? "Constant" : "Variable");
                    this.SetUoTypeToken(found.UoTypeToken);
                    Field = found;
                    Field.AddReference(this);
                }
            }
        }

    }

    // single or list
    class ExpressionNode : ScopedNode
    {
        // Generally lists can be a mix of any valid type. Checking needs to be done separately for location assignments
        protected virtual bool TypesMustMatch { get { return false; } }

        Token token;

        protected virtual void BuildExpression(ParsingContext context, ParseTreeNode treeNode)
        {
            token = treeNode.ChildNodes[0].Token as Token;
            if (token!=null && token.Terminal is Tokenizer)         // terminals do not need to be rechecked
            {
                SetUoToken(((Tokenizer)token.Terminal).Tokenize(token.Value));
                SetUoTypeToken(((Tokenizer)token.Terminal).TokenType);
                AsString = treeNode.ChildNodes[0].Token.ValueString;
            }
            else
            {
                foreach (ParseTreeNode cnode in treeNode.ChildNodes)
                    if (cnode.AstNode is ExpressionNode)
                    {
                        AstNode node = Reduce(cnode); // (ExpressionNode)cnode.AstNode;
                        ChildNodes.Add(node);
                        node.Parent = this;
                    }

                // Nulls in a list are ok on the first pass, but not on the second, so this is only done here if there are no null types
                if (ChildNodes.Count == 1 || ChildNodes.TrueForAll(node => ((ExpressionNode)node).UoTypeToken != null))
                    CheckScope(context);
            }
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            if(token!=null) return token.Text;
            if(UoToken !=null) return UoToken.Value;
            return string.Join(",", ChildNodes.Select(
                expression => string.Format(((ExpressionNode)expression).ChildNodes.Count == 0 ? "{0}" : "{1}{0}{2}", ((ExpressionNode)expression).GenerateScript(options), Punct.LPara, Punct.RPara)
                ));
        }

        public static ExpressionNode Reduce(ParseTreeNode treeNode)
        {
            ParseTreeNode node = treeNode;

            while (node.ChildNodes.Count == 1 && node.FirstChild.AstNode is ExpressionNode && ((ExpressionNode)node.AstNode).CanReduce)
                node = node.FirstChild;

            return (ExpressionNode)node.AstNode;
        }

        /// <summary>
        /// Find expression first expression in chain
        /// </summary>
        public static ParseTreeNode Dig(ParseTreeNode treeNode)
        {
            ParseTreeNode node = treeNode;
            while (node.ChildNodes.Count == 1 && !(node.AstNode is ExpressionNode))
                node = node.FirstChild;
            return node.AstNode is ExpressionNode ? node : null;

        }

        public bool IsValidLocationList(ParsingContext context)
        {
            if (ChildNodes.Count != 3) return false;
            
            for (int i = 1; i < ChildNodes.Count; i++)
                if (((ExpressionNode)ChildNodes[i]).UoTypeToken != Types.Int)
                    return false;
            return true;
        }

        public override void CheckScope(ParsingContext context)
        {
            if (ChildNodes.Count > 1) // list
            {
                bool ok = true;
                if (TypesMustMatch)
                    for (int i = 1; i < ChildNodes.Count; i++)
                        if (((ExpressionNode)ChildNodes[i]).UoTypeToken != ((ExpressionNode)ChildNodes[0]).UoTypeToken)
                        {
                            context.AddParserMessage(ParserErrorLevel.Error, ChildNodes[i].Span, "List item type mismatch", ChildNodes[i].AsString);
                            ok = false;
                        }
                if (ok) SetUoTypeToken(UOSLBase.KeyToken<UoToken.UoTypeName_list>.Token);
            }
            else if (ChildNodes.Count == 1)
            {
                SetUoTypeToken(((ExpressionNode)ChildNodes[0]).UoTypeToken);
            }
        }

        protected virtual bool CanReduce { get { return true; } }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            BuildExpression(context, treeNode);

            // Reduce chained expressions
            if(CanReduce)
            for (int i = 0; i < ChildNodes.Count; i++)
            {
                ExpressionNode node = (ExpressionNode)ChildNodes[i];
                while (node.CanReduce && node.ChildNodes.Count == 1 && node.ChildNodes[0] is ExpressionNode)
                {
                    node = (ExpressionNode)node.ChildNodes[0];
                    ChildNodes[i] = node;
                }
            }
        }
    }
}
