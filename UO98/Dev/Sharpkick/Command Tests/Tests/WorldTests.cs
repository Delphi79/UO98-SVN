using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick.Tests
{
    class World : BaseTest
    {

        public override bool ExecuteAll()
        {
            bool result=true;

            result &= RunTest(Test_createGlobalObjectAt);
            result &= RunTest(Test_getFirstObjectOftype);
            result &= RunTest(Test_getNextObjectOftype);
            result &= RunTest(Test_TryFindObjectZero);

            return result;
        }

        public static bool Test_createGlobalObjectAt()
        {
            int serial;

            StateBegin("createGlobalObjectAt");

            serial = CreateTestItemThenFind(GetRandomItemAndLocation());
            Assert(serial > 0);

            DeleteTestItem(serial);

            return StateResultFinal();
        }

        public static bool Test_getFirstObjectOftype()
        {
            ItemAndLocation itemandlocation = GetRandomItemAndLocation();

            StateBegin("getFirstObjectOftype");

            int serial_expected = CreateTestItemThenFind(itemandlocation);

            int serial_found = Server.getFirstObjectOftype(itemandlocation.Location, itemandlocation.ItemID);
            Assert(serial_found == serial_expected, "Item And Location: {0} Serials don't match. found:{1}, expected:{2}", itemandlocation, serial_found, serial_expected);

            DeleteTestItem(serial_expected);

            return StateResultFinal();

        }

        public static bool Test_getNextObjectOftype()
        {
            ItemAndLocation itemandlocation = GetRandomItemAndLocation();

            StateBegin("getFirstObjectOftype");

            int serial_1 = CreateTestItemThenFind(itemandlocation);
            int serial_2 = CreateTestItemThenFind(itemandlocation);

            Assert(serial_1 != serial_2, "Both created items have same serial.");

            int serial_found1 = Server.getFirstObjectOftype(itemandlocation.Location, itemandlocation.ItemID);
            int serial_found2 = Server.getNextObjectOfType(itemandlocation.Location, itemandlocation.ItemID, serial_found1);

            Assert(serial_found1 != serial_found2, "Both found items have same serial.");

            List<int> found = new List<int>();
            found.Add(serial_found1);
            found.Add(serial_found2);

            Assert(
                found.Contains(serial_1) && found.Contains(serial_2),
                "Item And Location: {0} Both Items not found. expected:{1},{2} found:{3},{4}", itemandlocation, serial_1, serial_2, serial_found1, serial_found2);

            DeleteTestItems(serial_1, serial_2);
            
            return StateResultFinal();
        }

        public static bool Test_TryFindObjectZero()
        {
            StateBegin("TryFindObjectZero");

            Item item;
            Assert(!Server.TryFindObject(0, out item), "Should not have found object with serial zero.");

            return StateResultFinal();
        }

    }
}
