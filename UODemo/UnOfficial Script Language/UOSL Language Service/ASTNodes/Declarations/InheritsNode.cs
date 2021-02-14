using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;

namespace JoinUO.UOSL.Service.ASTNodes
{
    [CLSCompliant(false)]
    public class InheritsNode : ScopedNode
    {
        public string Filename { get; private set; }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            Filename = treeNode.LastChild.Token.Text;
        }

        public override string GenerateScript(LanguageOption options, int indentationlevel = 0)
        {
            return string.Format("{0} {1}", Keyword.Inherits.Value, Filename);
        }
    }
}
