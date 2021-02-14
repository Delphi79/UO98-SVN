using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick.Tests
{
    abstract partial class BaseTest
    {
        public abstract bool ExecuteAll();

        protected delegate bool TestMethod();

        protected bool RunTest(TestMethod test)
        {
            StateBegin(test.Method.Name);

            if (Assert(test()))
                TestMessage(true, "Passed");
            else
                TestMessage(false, "Failed");

            return StateResultFinal();
        }


    }
}
