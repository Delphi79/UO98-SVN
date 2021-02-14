using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick.Tests
{
    abstract partial class BaseTest
    {
        static ItemRange arbitraryItemIDTestRange = new ItemRange(2416, 3922);

        static Location arbitraryTestRangeBegin = new Location(10, 10, 0);
        static Location arbitraryTestRangeEnd = new Location(100, 100, 100);

        private static Random _random;
        private static Random Random
        {
            get
            {
                if(_random==null)
                {
                    System.Threading.Thread.Sleep(1);
                    _random=new System.Random();
                }
                return _random;
            }
        }

        protected static int RandomMinMax(int min, int max)
        {
            return (int)(Random.NextDouble() * (max-min) + min) + 1;
        }

        protected static ItemAndLocation GetRandomItemAndLocation()
        {
            ItemAndLocation itemandlocation = new ItemAndLocation();
            itemandlocation.ItemID = GetRandomTestItem();
            itemandlocation.Location = GetRandomOceanTestLocation();
            return itemandlocation;
        }

        protected static ushort GetRandomTestItem()
        {
            return (ushort)RandomMinMax(arbitraryItemIDTestRange.Begin, arbitraryItemIDTestRange.End);
        }

        protected static Location GetRandomOceanTestLocation()
        {
            Location location;

            location.X = (short)RandomMinMax(arbitraryTestRangeBegin.X, arbitraryTestRangeEnd.X);
            location.Y = (short)RandomMinMax(arbitraryTestRangeBegin.Y, arbitraryTestRangeEnd.Y);
            location.Z = (short)RandomMinMax(arbitraryTestRangeBegin.Z, arbitraryTestRangeEnd.Z);

            return location;
        }

        protected static Location GetRandomMapLocation()
        {
            Location location;

            location.X = (short)RandomMinMax(Server.ServerConfiguration.MapStartX,Server.ServerConfiguration.MapStartX +Server.ServerConfiguration.MapWidth);
            location.Y = (short)RandomMinMax(Server.ServerConfiguration.MapStartY,Server.ServerConfiguration.MapStartY +Server.ServerConfiguration.MapHeight);
            location.Z = (short)RandomMinMax(arbitraryTestRangeBegin.Z, arbitraryTestRangeEnd.Z);

            return location;
        }

        protected static int CreateTestItemThenFind(ItemAndLocation itemandlocation)
        {
            int ItemID = itemandlocation.ItemID;
            Location location = itemandlocation.Location;

            int serial = Server.createGlobalObjectAt(ItemID, location);

            if(Assert(serial > 0, "Failed to create Object. {0}", itemandlocation))
            {
                Item item;
                if (Assert(Server.TryFindObject(serial, out item), "Created Item not found in world. serial:{0}", serial))
                {
                    Assert(item.ObjectType == ItemID, "Created Item not of correct ObjectType. expected:({0}) actual:({1})", ItemID, item.ObjectType);

                    Assert(item.Location == location, "Created Item not in correct location. expected:({0}) actual:({1})", location, item.Location);
                }
            }

            StateAddObject(serial);

            return serial;
        }

        protected static void DeleteTestItems(params int[] serials)
        {
            foreach (int serial in serials)
                DeleteTestItem(serial);
        }

        protected static void DeleteTestItem(Serial serial)
        {
            if (Assert(Server.deleteObject(serial), "deleteObject returned false. serial:{0}", serial))
                StateRemoveObject(serial);
        }


    }
}
