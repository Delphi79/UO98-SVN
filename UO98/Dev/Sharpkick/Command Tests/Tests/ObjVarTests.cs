using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick.Tests
{
    class ObjVars : BaseTest
    {
        public override bool ExecuteAll()
        {
            bool result = true;

            result &= RunTest(Test_ObjVar_Int);
            result &= RunTest(Test_ObjVar_String);
            result &= RunTest(Test_ObjVar_Location);
            result &= RunTest(Test_ObjVar_MiscBehavior);

            return result;
        }

        public static bool Test_ObjVar_Int()
        {
            int serial;

            string VarName = "testInt";
            string VarNameBadCase = "TestInt";

            StateBegin("Test_ObjVar_Int");

            serial = CreateTestItemThenFind(GetRandomItemAndLocation());

            int expected = RandomMinMax(0, 10000);

            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.Integer), "Should not have found an existing ObjVar named {0}", VarName);

            Server.setObjVar(serial, VarName, expected);

            int actual = Server.getObjVarInt(serial, VarName);
            AssertSame(expected, actual);

            Assert(Server.hasObjVarOfType(serial, VarName, VariableType.Integer), "Didn't find the assigned ObjVar named {0}", VarName);
            Assert(!Server.hasObjVarOfType(serial, VarNameBadCase, VariableType.Integer), "Should not have ObjVar named {0}", VarNameBadCase);

            actual = Server.getObjVarInt(serial, VarNameBadCase);
            expected = 0;
            AssertSame(expected, actual);

            Server.removeObjVar(serial, VarName);
            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.Integer), "Should not have found an ObjVar named {0} after removal", VarName);

            DeleteTestItem(serial);

            return StateResultFinal();

        }

        public static bool Test_ObjVar_String()
        {
            int serial;

            string VarName = "testString";

            StateBegin("Test_ObjVar_String");

            serial = CreateTestItemThenFind(GetRandomItemAndLocation());

            string expected = "this is a test String. It has s0me numb3r5 1n it t@@!";

            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.String), "Should not have found an existing ObjVar named {0}", VarName);

            Server.setObjVar(serial, VarName, expected);

            string actual = Server.getObjVarString(serial, VarName);
            AssertSameString(expected, actual);

            Assert(Server.hasObjVarOfType(serial, VarName, VariableType.String), "Didn't find the assigned ObjVar named {0}", VarName);

            Server.removeObjVar(serial, VarName);
            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.String), "Should not have found an ObjVar named {0} after removal", VarName);

            DeleteTestItem(serial);

            return StateResultFinal();

        }

        public static bool Test_ObjVar_Location()
        {
            int serial;

            string VarName = "testLoc";

            StateBegin("Test_ObjVar_Location");

            serial = CreateTestItemThenFind(GetRandomItemAndLocation());

            Location expected = GetRandomMapLocation();

            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.Location), "Should not have found an existing ObjVar named {0}", VarName);

            Server.setObjVar(serial, VarName, expected);

            Location actual;

            if (Assert(Server.getObjVarLocation(serial, VarName, out actual), "Couldn't retrieve Location ObjVar {0}", VarName))
                AssertSame(expected, actual);

            Assert(Server.hasObjVarOfType(serial, VarName, VariableType.Location), "Didn't find the assigned ObjVar named {0}", VarName);

            Server.removeObjVar(serial, VarName);
            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.Location), "Should not have found an ObjVar named {0} after removal", VarName);

            DeleteTestItem(serial);

            return StateResultFinal();

        }

        public static bool Test_ObjVar_MiscBehavior()
        {
            int serial;

            string VarName = "testVar";
            int expectedSuccessReturnValue=1;
            int expectedFailureReturnValue = 0;

            StateBegin("Test_ObjVar_MiscBehavior");
 
            serial = CreateTestItemThenFind(GetRandomItemAndLocation());

            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.List), "Should not have found an existing List ObjVar named {0}", VarName);
            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.Integer), "Should not have found an existing Integer ObjVar named {0}", VarName);
            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.Location), "Should not have found an existing Location ObjVar named {0}", VarName);
            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.Object), "Should not have found an existing Object ObjVar named {0}", VarName);
            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.String), "Should not have found an existing String ObjVar named {0}", VarName);
            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.Unknown), "Should not have found an existing Unknown ObjVar named {0}", VarName);
            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.UNKNOWN_2), "Should not have found an existing UNKNOWN_2 ObjVar named {0}", VarName);
            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.UNKNOWN_6), "Should not have found an existing UNKNOWN_6 ObjVar named {0}", VarName);

            Assert(Server.setObjVar(serial, VarName, "Hello") == expectedSuccessReturnValue, "setObjVar to \"Hello\" didn't return {0}.", expectedSuccessReturnValue);
            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.List), "Should not have found an existing List ObjVar named {0}", VarName);
            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.Integer), "Should not have found an existing Integer ObjVar named {0}", VarName);
            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.Location), "Should not have found an existing Location ObjVar named {0}", VarName);
            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.Object), "Should not have found an existing Object ObjVar named {0}", VarName);
            Assert(Server.hasObjVarOfType(serial, VarName, VariableType.String), "Failed to find an existing String ObjVar named {0}", VarName);
            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.UNKNOWN_2), "Should not have found an existing UNKNOWN_2 ObjVar named {0}", VarName);
            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.UNKNOWN_6), "Should not have found an existing UNKNOWN_6 ObjVar named {0}", VarName);

            Assert(Server.hasObjVarOfType(serial, VarName, VariableType.Unknown), "Failed to find an existing ObjVar named {0} via 'Unknown' type", VarName);

            Assert(Server.setObjVar(serial, VarName, 44) == expectedSuccessReturnValue, "setObjVar to 44 didn't return {0}.", expectedSuccessReturnValue);

            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.String), "Found an existing String ObjVar named {0} after overwrite.", VarName);
            Assert(Server.hasObjVarOfType(serial, VarName, VariableType.Integer), "Failed to find an Integer ObjVar named {0} after overwrite.", VarName);

            Server.removeObjVar(serial, VarName);
            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.String), "Found an existing String ObjVar named {0} after remove.", VarName);
            Assert(!Server.hasObjVarOfType(serial, VarName, VariableType.Integer), "Found an existing Integer ObjVar named {0} after remove.", VarName);

            DeleteTestItem(serial);

            Assert(Server.setObjVar(serial, VarName, 78) == expectedFailureReturnValue, "setObjVar to 78 didn't return {0}. Expected Fail on deleted item.", expectedFailureReturnValue);

            return StateResultFinal();

        }

    }
}
