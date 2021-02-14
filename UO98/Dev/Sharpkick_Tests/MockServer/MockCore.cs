using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpkick;
using System.Runtime.InteropServices;

namespace Sharpkick_Tests
{
    unsafe partial class MockCore : Sharpkick.ICore
    {
        private IPacketEngine _PacketEngine;
        public IPacketEngine PacketEngine { get { return _PacketEngine ?? (_PacketEngine = new MockPacketEngine()); } }

        MockWorld World = new MockWorld();

        static UnsafeBytePointerFactoryStruct bytePtrFactory = new UnsafeBytePointerFactoryStruct();

        #region ICore Members

        public void SaveWorld()
        {
            throw new NotImplementedException();
        }

        public unsafe void SendSystemMessage(Sharpkick.PlayerObject* player, byte* message)
        {
            throw new NotImplementedException();
        }

        public unsafe int createGlobalObjectAt(int ItemID, Location* Location)
        {
            ItemAndLocation itemandlocation = new Sharpkick.ItemAndLocation((ushort)ItemID, *Location);
            return (int)World.CreateItem(itemandlocation);
        }

        public bool deleteObject(Serial serial)
        {
            return World.DeleteItem(serial);
        }

        public unsafe int setObjVarInt(int serial, byte* name, int value)
        {
            if (!Exists(serial)) return 0;

            string varname = StringPointerUtils.GetAsciiString(name);

            MockObjVarAttachments.AddAttachment(serial, VariableType.Integer, varname, value);
            return 1;
        }

        public unsafe int setObjVarString(int serial, byte* name, byte* value)
        {
            if (!Exists(serial)) return 0;

            string varname = StringPointerUtils.GetAsciiString(name);
            string varvalue = StringPointerUtils.GetAsciiString(value);

            MockObjVarAttachments.AddAttachment(serial, VariableType.String, varname, varvalue);
            return 1;
        }

        public unsafe int setObjVarLocation(int serial, byte* name, Location* value)
        {
            if (!Exists(serial)) return 0;

            string varname = StringPointerUtils.GetAsciiString(name);
            Location varvalue = *value;
            MockObjVarAttachments.AddAttachment(serial, VariableType.Location, varname, varvalue);
            return 1;
        }

        public void removeObjVar(int serial, byte* varName)
        {
            string varname = StringPointerUtils.GetAsciiString(varName);
            MockObjVarAttachments.Remove(serial, varname);
        }

        public bool hasObjVarOfType(int serial, byte* varName, VariableType varType)
        {
            string varname = StringPointerUtils.GetAsciiString(varName);
            return MockObjVarAttachments.Has(serial, varType, varname);
        }

        public int getObjVarInt(int serial, byte* varName)
        {
            string varname = StringPointerUtils.GetAsciiString(varName);
            return MockObjVarAttachments.GetInt(serial, varname);
        }

        public byte* getObjVarString(int serial, byte* varName)
        {
            string varname = StringPointerUtils.GetAsciiString(varName);
            string value = MockObjVarAttachments.GetString(serial, varname);
            if (value == null)
                return null;
            return bytePtrFactory.MakePointerToTempString(value);
        }

        public bool getObjVarLocation(int serial, byte* varName, Location* locationResult)
        {
            string varname = StringPointerUtils.GetAsciiString(varName);
            Location value;
            if (MockObjVarAttachments.GetLocation(serial, varname, out value))
            {
                *locationResult = value;
                return true;
            }
            else
            {
                *locationResult = new Location();
                return false;
            }
        }

        public int setHue(Serial serial, short hue)
        {
            ItemObject item;
            if (!World.TryGetItem(serial, out item)) return 0;
            item.Hue = (ushort)hue;
            World[serial] = item;
            return 1;
        }

        public unsafe byte* addScript(int serial, byte* scriptName, int executeCreation)
        {
            if(!Exists(serial))
                return bytePtrFactory.ItemNotFound;

            string script = StringPointerUtils.GetAsciiString(scriptName);
            if (!MockScriptAttachments.IsValidScriptName(script))
                return bytePtrFactory.InvalidScriptClass;

            MockScriptAttachments.AddScript(serial, script);
            return null;
        }

        public unsafe bool hasScript(int serial, byte* scriptName)
        {
            ItemObject item;
            if (!World.TryGetItem(serial, out item))
                return false;

            return MockScriptAttachments.Has(serial, StringPointerUtils.GetAsciiString(scriptName));
        }

        public unsafe bool detachScript(int serial, byte* scriptName)
        {
            ItemObject item;
            if (!World.TryGetItem(serial, out item))
                return false;

            MockScriptAttachments.Remove(serial, StringPointerUtils.GetAsciiString(scriptName));
            
            return true;
        }

        public int getQuantity(Serial serial)
        {
            ItemObject item;
            if (!World.TryGetItem(serial, out item)) return 0;
            return item.GetQuantity();
        }

        public int getWeight(Serial serial)
        {
            ItemObject item;
            if (!World.TryGetItem(serial, out item)) return 0;

            int weight;

            byte* name = bytePtrFactory.MakePointerToTempString("overloadedWeight");
            if (hasObjVarOfType(serial, name, VariableType.Integer))
                weight = getObjVarInt(serial, name);
            else
                throw new NotImplementedException("Getting the normal weight of an item will require an mock implementation of TileData.");

            return weight * getQuantity(serial);
        }

        public int setOverloadedWeight(Serial serial, int weight)
        {
            if (!Exists(serial)) return 0;
            byte* name=bytePtrFactory.MakePointerToTempString("overloadedWeight");
            return setObjVarInt(serial, name, weight);
        }

        public unsafe int getFirstObjectOfType(Location* location, int itemId)
        {
            return World.getFirstObjectOfType(*location, itemId);
        }

        public unsafe int getNextObjectOfType(Location* location, int itemId, int lastItemSerial)
        {
            return World.getNextObjectOfType(*location, itemId, lastItemSerial);
        }

        public Location getLocation(Serial itemSerial)
        {
            ItemObject* item=ConvertSerialToObject(itemSerial);
            if (item != null)
            {
                return (*item).Location;
            }
            else
                return null;
        }

        public unsafe void MakeGameMaster(Sharpkick.PlayerObject* Target)
        {
            throw new NotImplementedException();
        }

        public unsafe void UnmakeGameMaster(Sharpkick.PlayerObject* Target)
        {
            throw new NotImplementedException();
        }

        public unsafe int IsGameMaster(Sharpkick.PlayerObject* Target)
        {
            throw new NotImplementedException();
        }

        public unsafe void OpenBank(class_Player* Target, Sharpkick.PlayerObject* ShowTo)
        {
            throw new NotImplementedException();
        }

        static ItemObject statictempitem;
        public unsafe Sharpkick.ItemObject* ConvertSerialToObject(int serial)
        {
            if (World.TryGetItem(serial, out statictempitem))
                fixed (ItemObject* toReturn = &statictempitem)
                    return toReturn;
            else
                return null;
        }

        public void OpenInfoWindow(Serial gmserial, Serial playerserial)
        {
            throw new NotImplementedException();
        }

        public bool IsItem(void* @object)
        {
            throw new NotImplementedException();
        }

        public bool IsNPC(void* @object)
        {
            throw new NotImplementedException();
        }

        public bool IsMobile(void* @object)
        {
            throw new NotImplementedException();
        }

        public bool IsPlayer(void* @object)
        {
            throw new NotImplementedException();
        }

        #endregion

        bool Exists(Serial serial)
        {
            ItemObject item;
            return World.TryGetItem(serial, out item);
        }

        private unsafe struct UnsafeBytePointerFactoryStruct
        {
            fixed byte _InvalidScriptClass[255];
            fixed byte _ItemNotFound[255];
            fixed byte _MiscBuffer[255];

            public byte* InvalidScriptClass
            {
                get
                {
                    fixed (byte* p = _InvalidScriptClass)
                    {
                        CopyNullString(p, "Failed to get script class");
                        return p;
                    }
                }
            }

            public byte* ItemNotFound
            {
                get
                {
                    fixed (byte* p = _ItemNotFound)
                    {
                        CopyNullString(p, "Item not found");
                        return p;
                    }
                }
            }

            public byte* MakePointerToTempString(string text)
            {
                fixed (byte* p = _ItemNotFound)
                {
                    CopyNullString(p, text);
                    return p;
                }
            }

            private static void CopyNullString(byte* ptrDest, string text)
            {
                byte[] textBytes = ASCIIEncoding.ASCII.GetBytes(text);
                fixed (byte* p = textBytes)
                {
                    int i = 0;
                    while (*(p + i) != 0 && i < 254 && i < textBytes.Length)
                    {
                        *(ptrDest + i) = *(p + i);
                        i++;
                    }
                    *(ptrDest + i) = 0;
                }

            }
        }
    }
}
