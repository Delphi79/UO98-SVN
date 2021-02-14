using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UO98
{
    static class Insensitive
    {
        static bool Equals(string a, string b)
        {
            return string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
