using System;
using Sharpkick;
using Sharpkick.Network;

namespace Sharpkick_Tests
{
    class MockClient_80_Login : BaseClientPacketMock
    {
        public override bool Dynamic { get { return false; } }

        unsafe public string Username
        {
            get
            {
                fixed (byte* p = PacketData)
                    return StringPointerUtils.GetAsciiString(p + 1, 30);
            }
        }

        unsafe public string Password
        {
            get
            {
                fixed (byte* p = PacketData)
                    return StringPointerUtils.GetAsciiString(p + 31, 30);
            }
        }

        unsafe public MockClient_80_Login(ClientSocketStruct* socket, string username, string password)
            : base(0x80, 62)
        {
            WriteAsciiNull(1, username ?? string.Empty);
            WriteAsciiNull(31, password ?? string.Empty);
        }

        public override void VerifyTransform(ClientPacketSafe resultPacket)
        {
            Packet80_LoginRequest packet = resultPacket as Packet80_LoginRequest;
            if (packet == null)
                throw new VerificationException("Expected Packet80_LoginRequest. Unexpected underlying packet type: {0}", resultPacket.GetType());

            int accountid;
            bool userAsExpected = packet.Username == "denied" || int.TryParse(packet.Username, out accountid);
            bool passCleared = string.IsNullOrEmpty(packet.Password);

            if (!userAsExpected || !passCleared)
                throw new VerificationException("Data unexpected in Packet80_LoginRequest. user: {0}  pass: {1}", packet.Username, packet.Password);

        }
    }

}
