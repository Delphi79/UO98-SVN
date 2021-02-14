using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick.Tests
{
    /// <summary>
    /// A class for doing testing against a live server. Should be initiated from the server when it is in test mode.
    /// </summary>
    public class OnlineTests
    {
        /// <summary>
        /// Execute all tests against the running server
        /// </summary>
        public static void BeginOnlineTesting()
        {
            (new AllTests()).ExecuteAll();
        }

        private class AllTests : BaseTest
        {
            public override bool ExecuteAll()
            {
                StateBegin("All Tests");

                Assert((new World()).ExecuteAll());
                Assert((new Scripts()).ExecuteAll());
                Assert((new ObjVars()).ExecuteAll());
                Assert((new ObjectPropertyTests()).ExecuteAll());

                if (AssertPeek())
                    TestMessage(true, "Passed.");
                else
                    TestMessage(false, "One or more tests Failed.");

                return StateResultFinal();
            }
        }


    }

}
