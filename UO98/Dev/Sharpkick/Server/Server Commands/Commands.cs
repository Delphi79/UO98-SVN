using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Sharpkick.Network;

namespace Sharpkick
{
    static partial class Server
    {
        public static void SaveWorld()
        {
            Core.SaveWorld();
        }
    }

}
