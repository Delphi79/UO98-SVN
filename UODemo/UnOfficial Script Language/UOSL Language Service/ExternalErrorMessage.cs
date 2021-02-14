using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace JoinUO.UOSL.Service
{
    public class ExternalErrorMessage : ParserMessage
    {
        public string FilePath { get; private set; }

        public ExternalErrorMessage(string filepath, ParserErrorLevel level, SourceLocation location, string message, ParserState parserState)
            : base(level, location, message, parserState)
        {
            FilePath = filepath;
        }
    }
}