using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace UoClientSDK
{
    static class ReflectionHelpers
    {
        public static IEnumerable<Type> GetAttributedTypesFromAssembly<TAttribute>() where TAttribute : System.Attribute
        {
            return GetAttributedTypesFromAssembly<TAttribute>(Assembly.GetExecutingAssembly());
        }

        public static IEnumerable<Type> GetAttributedTypesFromAssembly<TAttribute>(Assembly assembly) where TAttribute : System.Attribute
        {
            return
                   from t in assembly.GetTypes()
                   where t.IsDefined(typeof(TAttribute), true)
                   select t;
        }

    }
}
