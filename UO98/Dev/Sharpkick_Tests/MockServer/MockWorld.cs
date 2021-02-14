using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpkick;

namespace Sharpkick_Tests
{
    class MockWorld
    {
        static Serial nextSerial = 1000;

        Dictionary<Serial, ItemObject> WorldObjects = new Dictionary<Serial, ItemObject>();

        public ItemObject this[Serial serial]
        {
            get
            {
                return WorldObjects[serial];
            }
            set
            {
                WorldObjects[serial] = value;
            }
        }

        public uint CreateItem(ItemAndLocation itemandlocation)
        {
            Serial serial = nextSerial++;

            ItemObject item = new ItemObject();
            item.ObjectType=itemandlocation.ItemID;
            item.Location=itemandlocation.Location;
            item.Serial = serial;

            WorldObjects.Add(serial, item);

            return serial;
        }

        public bool DeleteItem(Serial serial)
        {
            if (WorldObjects.ContainsKey(serial))
            {
                MockScriptAttachments.DeleteAllFor(serial);
                MockObjVarAttachments.DeleteAllFor(serial);
                ObjectPropertyExtensions.Purge(WorldObjects[serial]);
                WorldObjects.Remove(serial);
                return true;
            }
            else
                return false;
        }

        public int getFirstObjectOfType(Location location, int itemId)
        {
            return (int)ItemsOfTypeAtLocation(location, itemId).FirstOrDefault().Serial;
        }

        public int getNextObjectOfType(Location location, int itemId, Serial serial)
        {
            List<ItemObject> items=new List<ItemObject>(ItemsOfTypeAtLocation(location, itemId));
            List<Serial> serials = new List<Serial>(items.Select(item => (Serial)item.Serial));

            int indexFirst = serials.IndexOf(serial);
            if (indexFirst < 0 || serials.Count - 1 == indexFirst)
                return 0;
            else
                return serials[indexFirst + 1];

        }

        IEnumerable<ItemObject> ItemsOfTypeAtLocation(Location location, int itemId)
        {
            return WorldObjects.Values.Where(item => item.ObjectType == itemId && item.Location == location);
        }

        public bool TryGetItem(Serial index, out ItemObject item)
        {
            return WorldObjects.TryGetValue(index, out item);
        }
    }
}
