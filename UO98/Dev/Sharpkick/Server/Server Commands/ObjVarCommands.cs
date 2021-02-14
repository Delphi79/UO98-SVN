using System;
using System.Text;

namespace Sharpkick
{
    enum VariableType
    {
        Integer = 0,
        String = 1,
        UNKNOWN_2 = 2,
        Location = 3,
        Object = 4,
        List = 5,
        UNKNOWN_6 = 6,
        Unknown = 7
    }

    static partial class Server
    {
        unsafe public static int setObjVar(int serial, string name, int value)
        {
            fixed (byte* pName = ASCIIEncoding.ASCII.GetBytes(name))
                return Core.setObjVarInt(serial, pName, value);
        }

        unsafe public static int setObjVar(int serial, string name, string value)
        {
            fixed (byte* pName = ASCIIEncoding.ASCII.GetBytes(name))
            fixed (byte* pVal = ASCIIEncoding.ASCII.GetBytes(value ?? string.Empty))
                return Core.setObjVarString(serial, pName, pVal);
        }

        unsafe public static int setObjVar(int serial, string name, Location value)
        {
            fixed (byte* pName = ASCIIEncoding.ASCII.GetBytes(name))
                return Core.setObjVarLocation(serial, pName, &value);
        }

        unsafe public static void removeObjVar(int serial, string name)
        {
            fixed (byte* pName = ASCIIEncoding.ASCII.GetBytes(name))
                Core.removeObjVar(serial, pName);
        }

        unsafe public static bool hasObjVarOfType(int serial, string name, VariableType varType)
        {
            fixed (byte* pName = ASCIIEncoding.ASCII.GetBytes(name))
                return Core.hasObjVarOfType(serial, pName, varType);
        }

        unsafe public static int getObjVarInt(int serial, string name)
        {
            fixed (byte* pName = ASCIIEncoding.ASCII.GetBytes(name))
                return Core.getObjVarInt(serial, pName);
        }

        unsafe public static string getObjVarString(int serial, string name)
        {
            byte* chars;
            fixed (byte* pName = ASCIIEncoding.ASCII.GetBytes(name))
                chars = Core.getObjVarString(serial, pName);
            return StringPointerUtils.GetAsciiString(chars);
        }

        unsafe public static bool getObjVarLocation(int serial, string name, out Location locationResult)
        {
            fixed (Location* pLocation = &locationResult)
            fixed (byte* pName = ASCIIEncoding.ASCII.GetBytes(name))
                return Core.getObjVarLocation(serial, pName, pLocation);
        }

    }
}
