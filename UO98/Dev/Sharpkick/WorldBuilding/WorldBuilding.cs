using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick.WorldBuilding
{
    static class Builder
    {
        const sbyte zRangeForItemLocationEqualityInCore = 8;

        static bool AddSkaraFerry = MyServerConfig.DecorationAddSkaraFerry;

        public static void Configure()
        {
            Server.Core.OnPulse += new OnPulseEventHandler(EventSink_OnPulse);
        }

        static void EventSink_OnPulse()
        {
            if(Server.TimeManager.PulseNum == 20 && MyServerConfig.DecorationEnabled)
            {
                Decoration.DecorateBase();
                Teleporters.GenerateBaseTeleporters();
                DungeonEntranceTeleporters.Generate();
                Shrines.Generate();

                if (AddSkaraFerry)
                {
                    Decoration.DecorateSkaraFerry();
                    Teleporters.GenerateSkaraFerryTeleporters();
                }

                Server.Core.OnPulse -= EventSink_OnPulse;
            }
        }

        public static int FindExistingItemSerial(ItemAndLocation itemAndLocation)
        {
            int serial = 0;

            if (isPortcullis(itemAndLocation.ItemID))
                serial = FindItemWithinZRangeAboveOrBelow(itemAndLocation, 40);
            else if (isDoor(itemAndLocation.ItemID))
                serial = FindAnyNearbyDoorWithExactHomeLocationOf(itemAndLocation.Location);
            else
                serial = SerialOfExistingItemAtLocation(itemAndLocation);

            return serial;
        }

        public static bool ItemExistsAtExactLocation(ItemAndLocation itemAndLocation)
        {
            Location loc=itemAndLocation.Location;
            int itemid = itemAndLocation.ItemID;
            return Server.getFirstObjectOftype(loc, itemid) != 0;
        }

        public static int SerialOfFirstExistingItemAtLocation(ItemAndLocation itemAndLocation)
        {
            return Server.getFirstObjectOftype(itemAndLocation.Location, itemAndLocation.ItemID);
        }

        public static int SerialOfExistingItemAtLocation(ItemAndLocation itemAndLocation)
        {
            int serial = Server.getFirstObjectOftype(itemAndLocation.Location, itemAndLocation.ItemID);
            while (serial != 0)
            {
                Location locationOfFoundItem = Server.getLocation(serial);
                if (locationOfFoundItem == itemAndLocation.Location)
                    break;
                else
                    serial = Server.getNextObjectOfType(itemAndLocation.Location, itemAndLocation.ItemID, serial);
            }
            return serial;
        }

        public static bool TryCreateItem(ItemAndLocation itemAndLocation)
        {
            int serial = TryCreateItemReturnSerial(itemAndLocation);
            return (serial != 0);
        }

        public static int TryCreateItemReturnSerial(ItemAndLocation itemAndLocation)
        {
            return Server.createGlobalObjectAt(itemAndLocation.ItemID,itemAndLocation.Location);
        }

        public static string AddScriptToFirstItemAtLocation(ItemAndLocation itemAndLocation, string script)
        {
            int serial = Server.getFirstObjectOftype(itemAndLocation.Location, itemAndLocation.ItemID);
            if (serial != 0)
                return Server.addScript(serial, script);
            else
                return "Item Not Found.";
        }

        public static void DeleteItem(int serial)
        {
            Server.addScript(serial, "commandDelete");
        }

        // TODO: This may be able to be eliminated once COMMAND_getObjectsInRangeWithFlags is importable.
        static ItemRange[] DoorRanges = new ItemRange[] 
        { 
            new ItemRange(1653, 1782),  // most regular doors
            new ItemRange(244,245),     // secret
            new ItemRange(800, 801),    // secret
            new ItemRange(816, 817),    // secret
            new ItemRange(832, 833),    // secret
            new ItemRange(848, 849),    // secret
            new ItemRange(864, 865),    // secret
            new ItemRange(2105, 2120),  // light wooden gate
            new ItemRange(2084, 2098),  // metal gate
            new ItemRange(2124, 2139),  // metal gate
            new ItemRange(2150, 2165),  // dark wooden gate
            new ItemRange(6414, 6415),  // bar door
            new ItemRange(8173, 8188),  // cell door
        };

        public static bool isPortcullis(int itemID)
        {
            return itemID == 1781 || itemID == 1782;
        }

        public static bool isDoor(int itemID)
        {
            return Global.TileData.HasFlag((ushort)itemID, TileFlag.Door);
        }

        public static int FindItemWithinZRangeAboveOrBelow(ItemAndLocation itemAndLocation, byte Zrange)
        {
            int serial = 0;
            
            sbyte zStart=(sbyte)Math.Max(itemAndLocation.Location.Z - Zrange,sbyte.MinValue);
            sbyte zEnd=(sbyte)Math.Min(itemAndLocation.Location.Z + Zrange,sbyte.MaxValue);
            
            itemAndLocation.Location.Z=zStart;
            while (serial == 0 && itemAndLocation.Location.Z <= zEnd)
            {
                serial = SerialOfFirstExistingItemAtLocation(itemAndLocation);
                itemAndLocation.Location.Z += zRangeForItemLocationEqualityInCore;
            }

            return serial;                
        }

        public static int FindAnyNearbyDoorWithExactHomeLocationOf(Location locHome)
        {
            int serial = 0;

            LocationDelta[] RelativeLocationsToCheck = new LocationDelta[]
            {
                new LocationDelta(0,0,0),
                new LocationDelta(1,1,0),
                new LocationDelta(-1,-1,0),
                new LocationDelta(-1,1,0),
                new LocationDelta(1,-1,0)
            };

            foreach (LocationDelta offset in RelativeLocationsToCheck)
            {
                serial = FindAnyDoorAtLocation(locHome + offset);
                if (serial != 0 && GetCreationLocationOfItem(serial).Equals(locHome))
                    return serial;
                else
                    serial = 0;
            }

            return serial;
        }

        // TODO: export COMMAND_getObjectsAt from core, and check isDoor on each item. or COMMAND_getObjectsInRangeWithFlags
        private static int FindAnyDoorAtLocation(Location loc)
        {
            foreach (ItemRange doorrange in DoorRanges)
                for (int id = doorrange.Begin; id <= doorrange.End; id++)
                {
                    int serial = Server.getFirstObjectOftype(loc, id);
                    while (serial != 0)
                    {
                        Location l = Server.getLocation(serial);
                        if (l.X == loc.X && l.Y == loc.Y && l.Z == loc.Z) return serial;
                        serial = Server.getNextObjectOfType(loc, id, serial);
                    }
                }
            return 0;
        }

        static Location GetCreationLocationOfItem(int serial)
        {
            Item item;
            if (Server.TryFindObject(serial, out item))
            {
                return item.CreationLocation;
            }
            return Location.Zero;
        }

        public static void setHomeAndHeavy(int serial)
        {
            if (serial > 0)
            {
                Server.setObjVar(serial, "home", Server.getLocation(serial));
                if (!isHeavy(serial))
                    MakeHeavy(serial);
            }
        }

        public static bool isAtCreationLocation(int serial)
        {
            Item item;
            if (serial != 0 && Server.TryFindObject(serial, out item))
                return item.CreationLocation == item.Location;
            else
                return false;
        }

        public static bool isHeavy(int serial)
        {
            return Server.getWeight(serial) >= 100;
        }

        public static void MakeHeavy(int serial)
        {
            int newweight;

            string result = Server.addScript(serial, "heavy");
            if (!string.IsNullOrEmpty(result))
                Console.WriteLine("Decorate Error: Failed to set heavy on {0} Message: {1}", serial, result);
            else if ((newweight = Server.getWeight(serial)) != 255)
                Console.WriteLine("Decorate Error: Item not heavy: {0}. Weight: {1}", serial, newweight);
        }

    }
}
