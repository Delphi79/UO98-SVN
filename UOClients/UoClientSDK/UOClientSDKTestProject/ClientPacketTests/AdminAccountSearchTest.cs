using Microsoft.VisualStudio.TestTools.UnitTesting;
using UoClientSDK;
using UoClientSDK.Network;
using UoClientSDK.Network.ClientPackets;

namespace UOClientSDKTestProject
{
    
    
    /// <summary>
    ///This is a test class for AdminAccountSearchTest and is intended
    ///to contain all AdminAccountSearchTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AdminAccountSearchTest
    {

        /// <summary>
        ///A test for AdminAccountSearch Constructor
        ///</summary>
        [TestMethod()]
        public void AdminAccountSearchConstructorTest()
        {
            ClientVersion version = ClientVersion.vMAX;
            AdminAccountSearch.AcctSearchType type = AdminAccountSearch.AcctSearchType.Username;
            string term = "a username";
            AdminAccountSearch target = new AdminAccountSearch(version, type, term);
            
            PacketWriter writer=target.GetWriter();

            ClientPacketId packetidActual=(ClientPacketId)writer.Packet[0];
            ushort packetlen = (ushort)((writer.Packet[1] << 8) | writer.Packet[2]);
            RemoteAdminSubCommand commandidActual = (RemoteAdminSubCommand)writer.Packet[3];
            AdminAccountSearch.AcctSearchType typeActual = (AdminAccountSearch.AcctSearchType)writer.Packet[4];

            string termActual = System.Text.Encoding.ASCII.GetString(writer.Packet, 5, packetlen - 6);

            ClientPacketId packetidExpected=ClientPacketId.RemoteAdmin;
            RemoteAdminSubCommand commandidExpected=RemoteAdminSubCommand.AccountSearch;
            AdminAccountSearch.AcctSearchType typeExpected = type;
            string termExpected = term;

            Assert.AreEqual(packetidExpected, packetidActual);
            Assert.AreEqual(commandidExpected, commandidActual);
            Assert.AreEqual(termExpected, termActual);
        }
    }
}
