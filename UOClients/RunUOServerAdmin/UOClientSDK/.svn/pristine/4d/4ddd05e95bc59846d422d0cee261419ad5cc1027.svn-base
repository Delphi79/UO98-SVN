using Microsoft.VisualStudio.TestTools.UnitTesting;
using UoClientSDK.Compression;
using UoClientSDK.Network;
using UoClientSDK.Network.ServerPackets;
using UoClientSDK;

namespace UOClientSDKTestProject
{
    
    
    /// <summary>
    ///This is a test class for AdminCompressedPacketTest and is intended
    ///to contain all AdminCompressedPacketTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AdminCompressedPacketTest
    {


        /// <summary>
        ///A test for Instantiate
        ///</summary>
        [TestMethod()]
        public void AdminCompressedPackeInstantiateTest()
        {
            string text = "Hello there!";

            byte[] data = BuildAdminCompressedConsoleDataPacketBuffer(text);

            PacketBuffer buffer = new PacketBuffer(data);
            PacketReader reader = new PacketReader(ClientVersion.vMAX, buffer, (ushort)data.Length);

            ServerPacket actual = AdminCompressedPacket.Instantiate(reader);

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual is ConsoleDataPacket);

            Assert.AreEqual(text, ((ConsoleDataPacket)actual).Text);
            
        }

        public static byte[] BuildAdminCompressedConsoleDataPacketBuffer(string text)
        {

            byte[] internalbytes = ConsoleDataPacketTest.BuildConsoleDataPacketBuffer(text);

            byte[] CompData = new byte[(int)(internalbytes.Length * 1.001) + 20];
            int compLen = 0;
            ZLibError error = Compression.Pack(CompData, ref compLen, internalbytes, internalbytes.Length, ZLibQuality.Default);

            int packetLen = 1 + 2 + 2 + compLen;
            byte[] packetData = new byte[packetLen];
            int index = 0;
            packetData[index++] = (byte)ServerPacketId.AdminCompressed;
            packetData[index++] = (byte)((packetLen & 0xFF00) >> 8);
            packetData[index++] = (byte)(packetLen & 0xFF);
            packetData[index++] = (byte)((internalbytes.Length & 0xFF00) >> 8);
            packetData[index++] = (byte)(internalbytes.Length & 0xFF);

            for (int i = 0; i < compLen; i++)
                packetData[index + i] = CompData[i];

            return packetData;
        }

    }
}
