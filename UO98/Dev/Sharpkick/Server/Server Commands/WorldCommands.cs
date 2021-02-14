using System;

namespace Sharpkick
{
    static partial class Server
    {
        /// <summary>
        /// Create an item in the world
        /// </summary>
        /// <param name="ItemID">The Item ID</param>
        /// <param name="Location">The Location of the object</param>
        /// <returns>New items serial, or 0 on failure</returns>
        unsafe public static int createGlobalObjectAt(int ItemID, Location Location)
        {
            return Core.createGlobalObjectAt(ItemID, &Location);
        }

        unsafe public static bool deleteObject(Serial serial)
        {
            return Core.deleteObject(serial);
        }

        unsafe public static int getFirstObjectOftype(Location location, int itemId)
        {
            return Core.getFirstObjectOfType(&location, itemId);
        }

        unsafe public static int getNextObjectOfType(Location location, int itemId, int lastItemSerial)
        {
            return Core.getNextObjectOfType(&location, itemId, lastItemSerial);
        }

        unsafe public static bool TryFindObject(int serial, out Item item)
        {
            class_DynamicItem* pItem;
            bool toReturn = TryFindObject(serial, out pItem);
            if (toReturn)
                item = pItem;
            else
                item = null;
            return toReturn;
        }

        unsafe public static bool TryFindObject(int serial, out class_DynamicItem* item)
        {
            class_DynamicItem* ditem = Server.Core.ConvertSerialToItem(serial);
            if (ditem == null)
            {
                item = null;
                return false;
            }
            else
            {
                item = ditem;
                return true;
            }
        }

        unsafe public static bool TryFindObject(int serial, out Player player)
        {
            class_Player* pItem;
            bool toReturn = TryFindObject(serial, out pItem);
            if (toReturn)
                player = pItem;
            else
                player = null;
            return toReturn;
        }

        unsafe public static bool TryFindObject(int serial, out class_Player* player)
        {
            class_Player* playerPtr = Server.Core.ConvertSerialToPlayer(serial);
            if(playerPtr == null)
            {
                player = null;
                return false;
            }
            else
            {
                player = playerPtr;
                return true;
            }
        }


    }
}
