using Microsoft.VisualStudio.TestTools.UnitTesting;
using UoClientSDK.Network;
using UoClientSDK.Network.ServerPackets;
using UoClientSDK;

namespace UOClientSDKTestProject
{
    
    
    /// <summary>
    ///This is a test class for DamagePacketTest and is intended
    ///to contain all DamagePacketTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DamagePacketTest
    {
        /// <summary>
        ///A test for Instantiate
        ///</summary>
        [TestMethod()]
        public void DamagePacketInstantiateTest()
        {
            byte[] data = new byte[] { 0x0B, 0x01, 0x02, 0x03, 0x04, 0xEF, 0xFE };
            ushort length = (ushort)data.Length;

            PacketBuffer buffer = new PacketBuffer(data);
            PacketReader reader = new PacketReader(ClientVersion.vMAX, buffer, length);
            DamagePacket packet;
            packet = DamagePacket.Instantiate(reader);
            
            Assert.IsTrue(packet is DamagePacket);
            Assert.AreEqual(0x01020304, packet.Serial.Value);
            Assert.AreEqual(0xEFFE, packet.Amount);
        }
    }
}
