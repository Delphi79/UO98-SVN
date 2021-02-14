using Microsoft.VisualStudio.TestTools.UnitTesting;
using UoClientSDK.Network;
using UoClientSDK.Network.ServerPackets;
using UoClientSDK;

namespace UOClientSDKTestProject
{
    
    
    /// <summary>
    ///This is a test class for ConsoleDataPacketTest and is intended
    ///to contain all ConsoleDataPacketTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConsoleDataPacketTest
    {
        /// <summary>
        ///A test for Instantiate
        ///</summary>
        [TestMethod()]
        public void ConsoleDataPacketInstantiateWithStringTest()
        {
            string toEncode="Console string.";

            ConsoleDataPacket packet = BuildConsoleDataPacket(toEncode);

            Assert.IsNotNull(packet);
            Assert.AreEqual(toEncode, packet.Text);
        }

        /// <summary>
        ///A test for Instantiate
        ///</summary>
        [TestMethod()]
        public void ConsoleDataPacketInstantiateWithCharTest()
        {

            char toEncode = 'H';
            string expected = new string(toEncode, 1);

            ConsoleDataPacket packet = BuildConsoleDataPacket(toEncode);

            Assert.IsNotNull(packet);
            Assert.AreEqual(expected, packet.Text);
        }

        public static ConsoleDataPacket BuildConsoleDataPacket(string text)
        {
            byte[] data = BuildConsoleDataPacketBuffer(text);

            PacketBuffer buffer = new PacketBuffer(data);
            PacketReader reader = new PacketReader(ClientVersion.vMAX, buffer, (ushort)data.Length);

            return ConsoleDataPacket.Instantiate(reader);
        }

        public static byte[] BuildConsoleDataPacketBuffer(string text)
        {
            ServerPacketId packetid = ServerPacketId.AdminConsoleData;

            ushort packetLen = (ushort)(4 + text.Length);

            byte[] packetBytes = new byte[packetLen];
            packetBytes[0] = (byte)packetid;
            packetBytes[1] = (byte)((packetLen & 0xFF00) << 8);
            packetBytes[2] = (byte)(packetLen & 0xFF);
            packetBytes[3] = (byte)ConsoleDataPacket.DataType.String;
            Utility.WriteStringToBuffer(text, packetBytes, 4);

            return packetBytes;
        }

        public static ConsoleDataPacket BuildConsoleDataPacket(char text)
        {
            ServerPacketId packetid = ServerPacketId.AdminConsoleData;

            ushort packetLen = (ushort)(4 + 1);

            byte[] packetBytes = new byte[packetLen];
            packetBytes[0] = (byte)packetid;
            packetBytes[1] = (byte)((packetLen & 0xFF00) << 8);
            packetBytes[2] = (byte)(packetLen & 0xFF);
            packetBytes[3] = (byte)ConsoleDataPacket.DataType.Char;
            packetBytes[4] = (byte)text;

            PacketBuffer buffer = new PacketBuffer(packetBytes);
            PacketReader reader = new PacketReader(ClientVersion.vMAX, buffer, (ushort)packetBytes.Length);

            return ConsoleDataPacket.Instantiate(reader);
        }

    }
}
