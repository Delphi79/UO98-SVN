using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick
{
    static partial class Server
    {
        unsafe public static void OpenInfoWindow(Serial gmserial, Serial playerserial)
        {
            Core.OpenInfoWindow(gmserial, playerserial);
        }
    }
}
