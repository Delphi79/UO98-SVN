using Microsoft.VisualStudio.TestTools.UnitTesting;
using UoClientSDK;
using UoClientSDK.Network.ClientPackets;
using UoClientSDK.Network;

namespace UOClientSDKTestProject
{
    
    
    /// <summary>
    ///This is a test class for LoginRequestTest and is intended
    ///to contain all LoginRequestTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LoginRequestTest
    {
        /// <summary>
        ///A test for LoginRequest Constructor
        ///</summary>
        [TestMethod()]
        public void LoginRequestConstructorTest()
        {
            string username = "testUsername";
            string password = "testPassword";

            ClientVersion version = ClientVersion.vMAX;

            LoginRequest target = new LoginRequest(version, username, password, 5);

            PacketWriter writer = target.GetWriter();
            PacketBuffer buffer = new PacketBuffer(writer.Packet);
            PacketReader reader = new PacketReader(version, buffer, (ushort)writer.Length);

            ClientPacketId actualPacketID = (ClientPacketId)reader.ReadByte();
            string actualUsername = reader.ReadFixedString(30);
            string actualPassword = reader.ReadFixedString(30);

            Assert.AreEqual(ClientPacketId.LoginRequest, actualPacketID);
            Assert.AreEqual(username, actualUsername);
            Assert.AreEqual(password, actualPassword);
        }
    }
}
