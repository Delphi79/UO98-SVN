using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick.Tests
{
    partial class BaseTest
    {
        private static Stack<TestState> States = new Stack<TestState>();

        private struct TestState
        {
            public bool Pass;
            public string TestName;
            public List<int> ObjectsCreated;

            public TestState(string testname)
            {
                Pass = true;
                TestName = testname;
                ObjectsCreated = new List<int>();
            }
        }

        protected static void StateBegin(string testName)
        {
            States.Push(new TestState(testName));
        }

        protected static void StateAddObject(int serial)
        {
            if (serial == 0)
                TestMessage(false, "Tried to add an object of serial Zero to state.");
            else
                States.Peek().ObjectsCreated.Add(serial);
        }

        protected static void StateRemoveObject(int serial)
        {
            List<int> objects = States.Peek().ObjectsCreated;
            if (objects.Contains(serial))
                objects.Remove(serial);
            else
                TestMessage(false, "Tried to remove an object not in state.");
        }

        protected static bool AssertSameString(string expected, string actual)
        {
            return Assert(expected == actual, "Expected \"{0}\" Actual \"{1}\"", expected, actual);
        }

        protected static bool AssertSame<TVal>(TVal expected, TVal actual) where TVal : struct
        {
            return Assert(expected.Equals(actual), "Expected \"{0}\" Actual \"{1}\"", expected, actual);
        }

        protected static bool Assert(bool testexpression, string failmessageformat, params object[] args)
        {
            if (!testexpression && !string.IsNullOrEmpty(failmessageformat))
                TestMessage(false, failmessageformat, args);
            return Assert(testexpression);
        }

        protected static bool Assert(bool testexpression)
        {
            TestState state = States.Pop();
            state.Pass = state.Pass & testexpression;
            States.Push(state);
            return testexpression;
        }

        protected bool AssertPeek()
        {
            return States.Peek().Pass;
        }

        protected static bool StateResultFinal()
        {
            VerifyCleanup();
            return States.Pop().Pass;
        }

        private static void VerifyCleanup()
        {
            List<int> objects = States.Peek().ObjectsCreated;
            if (objects.Count > 0)
                TestMessage(false, "Objects not cleaned up: {0}", objects.Count);
        }

        protected static void TestMessage(bool passed, string format, params object[] args)
        {
            ConsoleUtils.PushColor(passed ? ConsoleColor.Green : ConsoleColor.Red);
            Console.WriteLine("{0}: {1}", States.Peek().TestName, string.Format(format, args));
            ConsoleUtils.PopColor();
        }
    }
}
