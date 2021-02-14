using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace JoinUO.UOSL.Service.ASTNodes
{
    class StringNode : ScopedNode
    {
        protected virtual int MaxLen { get { return 2047; } }

        public override void Init(Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            if (treeNode.Token.ValueString.Length > MaxLen)
                context.AddParserMessage(ParserErrorLevel.Error, this.Span, "The maximum length is {0} character{1}.", MaxLen, MaxLen != 1 ? "s" : string.Empty);
        }
    }

    class CharNode : StringNode
    {
        protected override int MaxLen { get { return 1; } }
    }
}
