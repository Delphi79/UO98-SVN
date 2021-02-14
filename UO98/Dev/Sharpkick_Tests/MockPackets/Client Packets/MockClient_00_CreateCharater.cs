using System;
using Sharpkick;
using Sharpkick.Network;

namespace Sharpkick_Tests
{
    class MockClient_00_CreateCharater : BaseClientPacketMock
    {
        public const uint v1_26_0len = 108;
        public const uint expectedLen = 104;

        public override bool Dynamic { get { return false; } }

        public ushort StartX { get { return (ushort)((PacketData[5] << 8) + PacketData[6]); } set { PacketData[5] = (byte)(value >> 8); PacketData[6] = (byte)value; } }
        public ushort StartY { get { return (ushort)((PacketData[7] << 8) + PacketData[8]); } set { PacketData[7] = (byte)(value >> 8); PacketData[8] = (byte)value; } }
        public byte StartZ { get { return PacketData[9]; } set { PacketData[9] = value; } }

        public byte Female { get { return PacketData[70]; } set { PacketData[70] = value; } }
        public byte Str { get { return PacketData[71]; } set { PacketData[71] = value; } }
        public byte Dex { get { return PacketData[72]; } set { PacketData[72] = value; } }
        public byte Int { get { return PacketData[73]; } set { PacketData[73] = value; } }
        public byte Skill1 { get { return PacketData[74]; } set { PacketData[74] = value; } }
        public byte Skill1val { get { return PacketData[75]; } set { PacketData[75] = value; } }
        public byte Skill2 { get { return PacketData[76]; } set { PacketData[76] = value; } }
        public byte Skill2val { get { return PacketData[77]; } set { PacketData[77] = value; } }
        public byte Skill3 { get { return PacketData[78]; } set { PacketData[78] = value; } }
        public byte Skill3val { get { return PacketData[79]; } set { PacketData[79] = value; } }

        public MockClient_00_CreateCharater(ClientVersionStruct version)
            : base(0x00, version >= ClientVersion.v1_26_0 ? v1_26_0len : expectedLen)
        {

        }

        public override void VerifyTransform(Sharpkick.Network.ClientPacketSafe resultPacket)
        {
            Packet00_CreateChar packet = resultPacket as Packet00_CreateChar;
            if (packet == null)
                throw new VerificationException("Expected Packet00_CreateChar. Unexpected underlying packet type: {0}", resultPacket.GetType());
            
            for (uint i = 0; i < expectedLen; i++)
                if(packet[i] != PacketData[i])
                    throw new VerificationException("Data verification failed at index {0}", i);
        }
    }
}
