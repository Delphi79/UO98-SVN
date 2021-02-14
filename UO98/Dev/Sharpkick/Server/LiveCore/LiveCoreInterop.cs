using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Sharpkick
{
    partial class Server
    {
        unsafe private partial class LiveCore : UODemo.Core, ICore
        {
            public void SendSystemMessage(class_Player* player, byte* message)
            { UnsafeNativeMethods.SendSystemMessage(player, message); }

            public int createGlobalObjectAt(int ItemID, Location* Location)
            { return UnsafeNativeMethods.createGlobalObjectAt(ItemID, Location); }

            public int setObjVarInt(int serial, byte* name, int value)
            { return UnsafeNativeMethods.setObjVarInt(serial, name, value); }

            public int setObjVarString(int serial, byte* name, byte* value)
            { return UnsafeNativeMethods.setObjVarString(serial, name, value); }

            public int setObjVarLocation(int serial, byte* name, Location* value)
            { return UnsafeNativeMethods.setObjVarLocation(serial, name, value); }

            public void removeObjVar(int serial, byte* varName)
            { UnsafeNativeMethods.removeObjVar(serial, varName); }

            public bool hasObjVarOfType(int serial, byte* varName, VariableType varType)
            { return UnsafeNativeMethods.hasObjVarOfType(serial, varName, varType); }

            public int getObjVarInt(int serial, byte* varName)
            { return UnsafeNativeMethods.getObjVarInt(serial, varName); }

            public byte* getObjVarString(int serial, byte* varName)
            { return UnsafeNativeMethods.getObjVarString(serial, varName); }

            public bool getObjVarLocation(int serial, byte* varName, Location* locationResult)
            { return UnsafeNativeMethods.getObjVarLocation(serial, varName, locationResult); }

            public byte* addScript(int serial, byte* scriptName, int executeCreation)
            { return UnsafeNativeMethods.addScript(serial, scriptName, executeCreation); }

            public bool hasScript(int serial, byte* scriptName)
            { return UnsafeNativeMethods.hasScript(serial, scriptName); }

            public bool detachScript(int serial, byte* scriptName)
            { return UnsafeNativeMethods.detachScript(serial, scriptName); }

            public int getFirstObjectOfType(Location* location, int itemId)
            { return UnsafeNativeMethods.getFirstObjectOfType(location, itemId); }

            public int getNextObjectOfType(Location* location, int itemId, int lastItemSerial)
            { return UnsafeNativeMethods.getNextObjectOfType(location, itemId, lastItemSerial); }

            public void MakeGameMaster(PlayerObject* Target)
            { UnsafeNativeMethods.MakeGameMaster(Target); }

            public void UnmakeGameMaster(PlayerObject* Target)
            { UnsafeNativeMethods.UnmakeGameMaster(Target); }

            public int IsGameMaster(PlayerObject* Target)
            { return UnsafeNativeMethods.IsGameMaster(Target); }

            public void OpenBank(class_Player* Target, class_Player* ShowTo)
            { UnsafeNativeMethods.OpenBank(Target, ShowTo); }

            private class UnsafeNativeMethods
            {
                [DllImport("sidekick.dll", CallingConvention = CallingConvention.Winapi)]
                public static extern void SendSystemMessage(class_Player* player, byte* message);

                [DllImport("sidekick.dll", CallingConvention = CallingConvention.Winapi)]
                public static extern int createGlobalObjectAt(int ItemID, Location* Location);

                [DllImport("sidekick.dll", CallingConvention = CallingConvention.Winapi)]
                public static extern int setObjVarInt(int serial, byte* name, int value);

                [DllImport("sidekick.dll", CallingConvention = CallingConvention.Winapi)]
                public static extern int setObjVarString(int serial, byte* name, byte* value);

                [DllImport("sidekick.dll", CallingConvention = CallingConvention.Winapi)]
                public static extern int setObjVarLocation(int serial, byte* name, Location* value);

                [DllImport("sidekick.dll", CallingConvention = CallingConvention.Winapi)]
                public static extern void removeObjVar(int serial, byte* varName);

                [DllImport("sidekick.dll", CallingConvention = CallingConvention.Winapi)]
                public static extern bool hasObjVarOfType(int serial, byte* varName, VariableType varType);

                [DllImport("sidekick.dll", CallingConvention = CallingConvention.Winapi)]
                public static extern int getObjVarInt(int serial, byte* varName);

                [DllImport("sidekick.dll", CallingConvention = CallingConvention.Winapi)]
                public static extern byte* getObjVarString(int serial, byte* varName);

                [DllImport("sidekick.dll", CallingConvention = CallingConvention.Winapi)]
                public static extern bool getObjVarLocation(int serial, byte* varName, Location* locationResult);

                [DllImport("sidekick.dll", CallingConvention = CallingConvention.Winapi)]
                public static extern byte* addScript(int serial, byte* scriptName, int executeCreation);

                [DllImport("sidekick.dll", CallingConvention = CallingConvention.Winapi)]
                public static extern bool hasScript(int serial, byte* scriptName);

                [DllImport("sidekick.dll", CallingConvention = CallingConvention.Winapi)]
                public static extern bool detachScript(int serial, byte* scriptName);

                [DllImport("sidekick.dll", CallingConvention = CallingConvention.Winapi)]
                public static extern int getFirstObjectOfType(Location* location, int itemId);

                [DllImport("sidekick.dll", CallingConvention = CallingConvention.Winapi)]
                public static extern int getNextObjectOfType(Location* location, int itemId, int lastItemSerial);

                [DllImport("sidekick.dll", CallingConvention = CallingConvention.Winapi)]
                public static extern void MakeGameMaster(PlayerObject* Target);

                [DllImport("sidekick.dll", CallingConvention = CallingConvention.Winapi)]
                public static extern void UnmakeGameMaster(PlayerObject* Target);

                [DllImport("sidekick.dll", CallingConvention = CallingConvention.Winapi)]
                public static extern int IsGameMaster(PlayerObject* Target);

                [DllImport("sidekick.dll", CallingConvention = CallingConvention.Winapi)]
                public static extern void OpenBank(class_Player* Target, class_Player* ShowTo);
            }
        }
    }
}