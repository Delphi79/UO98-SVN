using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick.Tests
{
    class Scripts : BaseTest
    {
        const string ValidInertScriptName = "test";
        const string InvalidScriptName = "doesnotexist";

        static Serial serial;

        public override bool ExecuteAll()
        {
            bool result = true;

            result &= RunTest(Test_Scripts_AddWhenValid);
            result &= RunTest(Test_Scripts_AddWhenInvalidScript);
            result &= RunTest(Test_Scripts_AddWhenInvalidItem);
            result &= RunTest(Test_Scripts_DeleteWhenDoesntHave);
            result &= RunTest(Test_Scripts_DeleteWhenInvalidItem);
            result &= RunTest(Test_Scripts_DeleteWhenInvalidScript);

            return result;
        }

        public static bool Test_Scripts_AddWhenValid()
        {
            StateBegin("Test_Scripts_AddWhenValid");

            serial = CreateTestItemThenFind(GetRandomItemAndLocation());
            
            string result = Server.addScript(serial, ValidInertScriptName);
            Assert(string.IsNullOrEmpty(result), "Add Script Failed: {0}", result);
            ShouldHaveScript(serial, ValidInertScriptName);
            DeleteTestItem(serial);

            return StateResultFinal();
        }

        public static bool Test_Scripts_AddWhenInvalidScript()
        {
            StateBegin("Test_Scripts_AddWhenInvalidScript");

            serial = CreateTestItemThenFind(GetRandomItemAndLocation());

            string expectedFailure = "Failed to get script class";
            string result = Server.addScript(serial, InvalidScriptName);
            AssertSameString(expectedFailure, result);

            ShouldNotHaveScript(serial, InvalidScriptName);

            DeleteTestItem(serial);

            return StateResultFinal();
        }

        public static bool Test_Scripts_AddWhenInvalidItem()
        {
            StateBegin("Test_Scripts_AddWhenInvalidItem");

            serial = (Serial)0;

            string expectedFailure = "Item not found";
            string result = Server.addScript(serial, ValidInertScriptName);
            AssertSameString(expectedFailure, result);

            ShouldNotHaveScript(serial, ValidInertScriptName);

            return StateResultFinal();
        }

        public static bool Test_Scripts_DeleteWhenValid()
        {
            StateBegin("Test_Scripts_DeleteWhenValid");

            serial = CreateTestItemThenFind(GetRandomItemAndLocation());

            string result = Server.addScript(serial, ValidInertScriptName);
            Assert(string.IsNullOrEmpty(result), "Add Script Failed: {0}", result);
            ShouldHaveScript(serial, ValidInertScriptName);

            Assert(Server.detachScript(serial, ValidInertScriptName), "Detach script failed.");
            ShouldNotHaveScript(serial, ValidInertScriptName);

            DeleteTestItem(serial);

            return StateResultFinal();
        }

        public static bool Test_Scripts_DeleteWhenDoesntHave()
        {
            StateBegin("Test_Scripts_DeleteWhenDoesntHave");

            serial = CreateTestItemThenFind(GetRandomItemAndLocation());

            ShouldNotHaveScript(serial, ValidInertScriptName);

            Assert(Server.detachScript(serial, ValidInertScriptName), "Detach script returned false for detach unattached script, expect true.");
            ShouldNotHaveScript(serial, ValidInertScriptName);

            DeleteTestItem(serial);

            return StateResultFinal();
        }

        public static bool Test_Scripts_DeleteWhenInvalidItem()
        {
            StateBegin("Test_Scripts_DeleteWhenInvalidItem");

            serial = CreateTestItemThenFind(GetRandomItemAndLocation());

            ShouldNotHaveScript(serial, ValidInertScriptName);

            DeleteTestItem(serial);

            Assert(!Server.detachScript(serial, ValidInertScriptName), "Detach script returned true for invalid item serial, expected false.");

            return StateResultFinal();
        }

        public static bool Test_Scripts_DeleteWhenInvalidScript()
        {
            StateBegin("Test_Scripts_DeleteWhenInvalidScript");

            serial = CreateTestItemThenFind(GetRandomItemAndLocation());

            ShouldNotHaveScript(serial, InvalidScriptName);

            Assert(Server.detachScript(serial, InvalidScriptName), "Detach script returned false for detach invalid script, expect true.");
            ShouldNotHaveScript(serial, InvalidScriptName);

            DeleteTestItem(serial);

            return StateResultFinal();
        }
        static void ShouldHaveScript(Serial testSerial, string scriptname)
        {
            Assert(Server.hasScript(testSerial, scriptname), "Added script {0} not found.", scriptname);
        }

        static void ShouldNotHaveScript(Serial testSerial, string scriptname)
        {
            Assert(!Server.hasScript(testSerial, scriptname), "Should not have found script {0} on object {1}.", scriptname, testSerial);
        }

    }
}
