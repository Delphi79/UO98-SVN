using Sharpkick.Network;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sharpkick;

namespace Sharpkick_Tests
{
    ///<summary>
    /// Tests for the Packet Mocks themselves.
    ///</summary>
    [TestClass()]
    public class MockIPacketsTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            Server.MockCore(new MockCore());
        }

        System.Net.IPAddress TestUserIP = new System.Net.IPAddress(new byte[] { 1, 2, 3, 4 });

        [TestMethod()]
        unsafe public void Test_SocketObject_SendPacket()
        {
            MockClient client = new MockClient(42, TestUserIP);

            BaseServerPacketMock mockPacket = new MockServer_1B_LoginConfirm(44, 1, 12, 23, 34, 1, 54, 33);

            IPackets engine = Server.PacketEngine;

            ((MockPacketEngine)engine).ClearLast(client);

            fixed (byte* p = mockPacket.PacketData)
                engine.SocketObject_SendPacket(client.Socket, p, mockPacket.Length);

            string result = ((MockPacketEngine)engine).VerifySent(client, mockPacket.PacketData);
            Assert.AreEqual(string.Empty, result);

        }

        [TestMethod()]
        unsafe public void Test_SocketObject_RemoveFirstPacket()
        {
            MockClient client = new MockClient(42, TestUserIP);

            string testusername1 = "foo";
            string testusername2 = "bar";
            string testpass = "pass";

            Assert.AreEqual(0u, client.SocketDataLength, "Data length should be zero after socket creation.");

            MockClient_80_Login packet1 = new MockClient_80_Login(client.Socket, testusername1, testpass);
            client.Enqueue(packet1);

            Assert.AreEqual(packet1.Length, client.SocketDataLength, "Socket should contain packet, datalength should equal packet length.");

            MockClient_80_Login packet2 = new MockClient_80_Login(client.Socket, testusername2, testpass);
            client.Enqueue(packet2);

            Assert.AreEqual(packet1.Length + packet2.Length, client.SocketDataLength, "Socket should contain 2 packet, datalength should equal sum of packet lengths.");

            Server.PacketEngine.SocketObject_RemoveFirstPacket(client.Socket, packet1.Length);

            Assert.AreEqual(packet2.Length, client.SocketDataLength, "Data length should be length of packet2 after packet1 removal.");

            Packet80_LoginRequest packetOut = client.ProcessOnly(packet2) as Packet80_LoginRequest;

            Assert.AreEqual(testusername2, packetOut.Username, "Couldn't verify username for packet2.");

            Server.PacketEngine.SocketObject_RemoveFirstPacket(client.Socket, packet2.Length);

            Assert.AreEqual(0u, client.SocketDataLength, "Data length should be zero after both packets removed.");
        }

    }
}
