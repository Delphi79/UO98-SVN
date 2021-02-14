using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick.Network
{
    static class GodPackets
    {
        public static void Configure()
        {


        }

        public static bool IsRestrictedGodPacket(byte id)
        {
            switch (id)
            {
                case 0x0A:  // Edit
                case 0x0C:
                case 0x0E:
                case 0x14:
                case 0x19:
                case 0x1D:
                case 0x35:
                case 0x36:
                case 0x46:
                case 0x47:
                case 0x48:
                case 0x4A:
                case 0x58:
                case 0x61:
                case 0x62:
                case 0x67:
                case 0x79:  // Resource Query
                case 0x96:  // Game Central Monitor
                case 0x9C:
                case 0x9D:
                    return true;
                default:
                    return false;
            }
        }


    }
}
