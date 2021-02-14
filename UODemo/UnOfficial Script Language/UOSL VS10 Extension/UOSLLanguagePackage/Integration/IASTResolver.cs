using System;
using System.Collections.Generic;
using JoinUO.UOSL.Service;

namespace JoinUO.UOSL.Package
{
    interface IASTResolver
    {
        IList<Declaration> FindCompletions(object result, int line, int col);
        IList<Declaration> FindMembers(object result, int line, int col);
        string FindQuickInfo(object result, int line, int col);
        IList<Method> FindMethods(object result, int line, int col, string name);
    }
}