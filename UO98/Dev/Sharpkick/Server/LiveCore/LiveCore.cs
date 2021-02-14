using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick
{
    static partial class Server
    {
        private partial class LiveCore : ICore
        {
            public LiveCore()
            {
                InitializePacketEngine();
            }
        }
    }
}
