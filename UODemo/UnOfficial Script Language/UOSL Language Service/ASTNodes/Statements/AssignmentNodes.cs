using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using JoinUO.UOdemoSDK;

namespace JoinUO.UOSL.Service.ASTNodes
{
    class AssignNode : ScopedNode
    {
        public ExpressionNode Expression;
        UoToken Operator;

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            if (treeNode.LastChild.AstNode is ExpressionNode) // If not, this is probably a unary assignment
            {
                Expression = (ExpressionNode)treeNode.LastChild.AstNode;
                if (Expression.ChildNodes.Count == 1 && Expression.AsString == "ExpressionList") // single expression ExpressionList
                    Expression = (ExpressionNode)Expression.ChildNodes[0];
                AsString = "=";

                Operator = ((Tokenizer)(treeNode.FirstChild.Term is Tokenizer ? treeNode.FirstChild.Term : treeNode.FirstChild.FirstChild.Term)).Tokenize() ;
                if (!(Operator is UoToken.UoOperator_Assignment))
                    context.AddParserMessage(ParserErrorLevel.Warning, this.Span, "While this symbol ({0}) will compile as an assignment, it is poor form.", Operator.Value);

                ChildNodes.Add(Expression);
                Expression.Parent = this;

                SetUoTypeToken(Expression.UoTypeToken);
            }
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            if (Expression.AsString != "ExpressionList") // single expression
                return Indenter(indentationlevel, "{0}{1}", Operator.Value, Expression.GenerateScript(options));
            else
                return Indenter(indentationlevel, "{0}{1}", Operator.Value, string.Join(",", Expression.ChildNodes.Select(exp => ((ScopedNode)exp).GenerateScript(options)).ToArray()));
        }

        bool warned = false; // This prevents multiple error messages

        public bool CheckAssignTo(UoToken type, ParsingContext context)
        {
            if (this.UoTypeToken == type || this.UoTypeToken == null/*any*/) return true;
            if (type == Types.List) return true;
            if (type == Types.String &&
                    (
                        UoTypeToken == Types.Int ||
                        UoTypeToken == Types.Obj
                    ))
                return true;
            if (type == Types.Int && UoTypeToken == Types.String)
            {
                if (!warned)
                    context.AddParserMessage(ParserErrorLevel.Info, Span, "Type mismatch, but valid: int=string");
                warned = true;
                return true;
            }
            if (type == UOSLBase.KeyToken<UoToken.UoTypeName_loc>.Token)
            {
                if (Expression.IsValidLocationList(context))
                {
                    Expression.SetUoTypeToken(UOSLBase.KeyToken<UoToken.UoTypeName_loc>.Token);
                    return true;
                }
                else
                {
                    if (!warned)
                        context.AddParserMessage(ParserErrorLevel.Error, Span, "Not a valid Location assignment.");
                    warned = true;
                    return false;
                }
            }

            if (!warned)
                context.AddParserMessage(ParserErrorLevel.Error, Span, "Assignment Type mismatch.");
            warned = true;

            return false;
        }

        public override void CheckScope(ParsingContext context)
        {
            SetUoTypeToken(Expression.UoTypeToken);
        }
    }

    class UnaryAssign : AssignNode
    {
        public UoFieldExpressionNode Field;
        public UoToken Operator;
        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            SetUoTypeToken(Types.Int);
            Field = (UoFieldExpressionNode)treeNode.FirstChild.AstNode;
            Operator = ((Tokenizer)treeNode.LastChild.LastChild.Term).Tokenize();
            AsString = Operator.Value;
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            return Indenter(indentationlevel, Operator.Value);
        }


        public override void CheckScope(ParsingContext context)
        {
            // Type does not change, do nothing.
        }
    }

    class AssignmentNode : StatementNode
    {
        AssignNode Assign;
        UoFieldExpressionNode Field;

        public override bool needsSemi { get { return true; } }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            if (treeNode.ChildNodes[0].AstNode is UoFieldExpressionNode)
            {
                Field = (UoFieldExpressionNode)treeNode.ChildNodes[0].AstNode;
                Assign = (AssignNode)treeNode.ChildNodes[1].AstNode;
            }
            else if (treeNode.ChildNodes[0].AstNode is UnaryAssign)
            {
                Field = ((UnaryAssign)treeNode.ChildNodes[0].AstNode).Field;
                Assign = (UnaryAssign)treeNode.ChildNodes[0].AstNode;
            }

            ChildNodes.Add(Field);
            ChildNodes.Add(Assign);
            Field.Parent = Assign.Parent = this;
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            return Indenter(indentationlevel, "{0}{1}", Field.GenerateScript(options), Assign.GenerateScript(options));
        }

        public override void CheckScope(ParsingContext context)
        {
            if (Field.isConst)
                context.AddParserMessage(ParserErrorLevel.Error, Field.Span, "{0} is a constant expression, it cannot be assigned to.", Field.Name);
            else
                Assign.CheckAssignTo(Field.UoTypeToken, context);
        }
    }
}
