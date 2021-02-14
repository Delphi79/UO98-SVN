using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoinUO.UOSL
{
    static class StringExtensions
    {
        /// <summary>
        /// Case insensitive string comparer
        /// </summary>
        /// <returns>True if the strings are equal</returns>
        public static bool EqualsCI(this string a, string b)
        {
            return StringComparer.InvariantCultureIgnoreCase.Equals(a, b);
        }
    }
}
