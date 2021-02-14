using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpkick;

namespace Sharpkick_Tests
{
    class MockServer_82_LoginDenied : BaseServerPacketMock
    {
        public MockServer_82_LoginDenied(ALRReason reason) : base(0x82,2)
        {
            PacketData[1] = (byte)reason;
        }
    }
}
