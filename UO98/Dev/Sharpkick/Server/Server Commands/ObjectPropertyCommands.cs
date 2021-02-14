using System;
using System.Text;

namespace Sharpkick
{
    static partial class Server
    {
        public static int setHue(int serial, short hue)
        {
            return Core.setHue(serial, hue);
        }

        public static int getQuantity(int serial)
        {
            return Core.getQuantity(serial);
        }

        public static int getWeight(int serial)
        {
            return Core.getWeight(serial);
        }

        public static int SetOverloadedWeight(Serial serial, int weight)
        {
            return Core.setOverloadedWeight(serial, weight);
        }

        unsafe public static Location getLocation(int itemSerial)
        {
            return Core.getLocation(itemSerial);
        }


    }
}
