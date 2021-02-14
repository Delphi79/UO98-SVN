using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick.Tests
{
    class ObjectPropertyTests : BaseTest
    {
        public override bool ExecuteAll()
        {
            bool result = true;

            result &= RunTest(Test_ObjProps_GetQuantity);
            result &= RunTest(Test_ObjProps_OverloadedWeight);
            result &= RunTest(Test_ObjProps_GetSetHue);
            result &= RunTest(Test_ObjProps_GetLocation);

            return result;
        }

        public static bool Test_ObjProps_GetQuantity()
        {
            int serial;

            StateBegin("Test_ObjProps_Weight");

            serial = CreateTestItemThenFind(GetRandomItemAndLocation());

            AssertSame(Server.getQuantity(serial), 1);

            DeleteTestItem(serial);

            return StateResultFinal();

        }

        public static bool Test_ObjProps_OverloadedWeight()
        {
            int serial;

            StateBegin("Test_ObjProps_Weight");

            serial = CreateTestItemThenFind(GetRandomItemAndLocation());

            Server.SetOverloadedWeight(serial, 200);
            AssertSame(Server.getWeight(serial), 200);

            DeleteTestItem(serial);

            return StateResultFinal();

        }

        public static bool Test_ObjProps_GetSetHue()
        {
            int serial;

            StateBegin("Test_ObjProps_GetSetHue");

            serial = CreateTestItemThenFind(GetRandomItemAndLocation());

            Item item;
            if (Assert(Server.TryFindObject(serial, out item), "Couldn't find item."))
            {
                AssertSame(item.Hue, 0);
                Server.setHue(serial, 51);
                if (Assert(Server.TryFindObject(serial, out item), "Couldn't find item after hue change."))
                    AssertSame(item.Hue, 51);
            }

            DeleteTestItem(serial);

            return StateResultFinal();

        }

        public static bool Test_ObjProps_GetLocation()
        {
            int serial;

            StateBegin("Test_ObjProps_GetLocation");

            ItemAndLocation itemandlocation = GetRandomItemAndLocation();

            serial = CreateTestItemThenFind(itemandlocation);

            Item item;
            if (Assert(Server.TryFindObject(serial, out item), "Couldn't find item."))
                AssertSame(itemandlocation.Location, Server.getLocation(serial));

            DeleteTestItem(serial);

            return StateResultFinal();

        }


    }
}
