using System;
using System.Runtime.InteropServices;
using Sharpkick.Network;

namespace Sharpkick
{
    /// <summary>
    /// Core commands, imported calls from the actual server.
    /// </summary>
    static partial class Server
    {
        private static ICore _Core = null;
        public static ICore Core { get { return _Core ?? (_Core = new LiveCore()); } }

        public static IPacketEngine PacketEngine { get { return Core.PacketEngine; } }

        public static void MockCore(ICore MockCoreInstance)
        {
            _Core=MockCoreInstance;
        }

     }

}
