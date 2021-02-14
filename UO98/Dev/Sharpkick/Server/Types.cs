using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Sharpkick
{

    struct ItemRange
    {
        public int Begin;
        public int End;
        public ItemRange(int begin, int end)
        {
            Begin = begin;
            End = end;
        }
    }

    struct ItemAndLocation
    {
        public ushort ItemID;
        public Location Location;

        public ItemAndLocation(ushort itemid, Location location)
        {
            ItemID = itemid;
            Location = location;
        }

        public override string ToString()
        {
            return string.Format("{0} @ {1}", ItemID, Location);
        }
    }

    enum Facing
    {
        EastWest,
        NorthSouth,
    }

    struct CharInfo
    {
        public CharInfo(string charname, string password)
        {
            CharName = charname;
            Password = password;
        }
        public string CharName;
        public string Password;
    }
}
