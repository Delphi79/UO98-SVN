using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UoClientSDK;
using UoClientSDK.Network;
using UoClientSDK.Network.ClientPackets;

namespace UOClientSDKTestProject
{
    
    
    /// <summary>
    ///This is a test class for AdminUpdateAccountTest and is intended
    ///to contain all AdminUpdateAccountTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AdminUpdateAccountTest
    {
        /// <summary>
        ///A test for AdminUpdateAccount Constructor
        ///</summary>
        [TestMethod()]
        public void AdminUpdateAccountConstructorTest()
        {
            ClientVersion version = ClientVersion.vMAX;

            AdminUpdateAccount.UpdateAccountArg arg;
            arg.Username = "Test Username";
            arg.Password = "passwoOoOrd";
            arg.AccessLevel = AccessLevel.Developer;
            arg.Banned = true;
            arg.AddressRestrictions = new List<string>() { "aAaa", "bBb", "ccC" };
            ushort expectedRestrictionCount = (ushort)arg.AddressRestrictions.Count();

            AdminUpdateAccount target = new AdminUpdateAccount(version, arg);
            Assert.IsNotNull(target);

            PacketWriter writer = target.GetWriter();
            Assert.IsNotNull(writer.Packet);

            int index = 3;

            RemoteAdminSubCommand actualPacketID = (RemoteAdminSubCommand)writer.Packet[index++];

            string actualUsername = string.Empty;
            while(index < writer.Packet.Length && writer.Packet[index] != 0)
                actualUsername += (char)writer.Packet[index++];
            index++;

            string actualPassword = string.Empty;
            while(index < writer.Packet.Length && writer.Packet[index] != 0)
                actualPassword += (char)writer.Packet[index++];
            index++;

            AccessLevel actualAccessLevel = (AccessLevel)writer.Packet[index++];
            bool actualBanned = writer.Packet[index++] != 0 ? true : false;

            ushort actualRestCount = (ushort)((writer.Packet[index++] << 8) | writer.Packet[index++]);

            List<string> actualAddressRestrictions = new List<string>();
            for (int i = 0; i < actualRestCount; i++)
            {
                string restriction = string.Empty;
                while(index < writer.Packet.Length && writer.Packet[index] != 0)
                    restriction += (char)writer.Packet[index++];
                index++;
                actualAddressRestrictions.Add(restriction);
            }

            Assert.AreEqual(RemoteAdminSubCommand.UpdateAccount, actualPacketID);
            Assert.AreEqual(arg.Username, actualUsername);
            Assert.AreEqual(arg.Password, actualPassword);
            Assert.AreEqual(arg.AccessLevel, actualAccessLevel);
            Assert.AreEqual(arg.Banned, actualBanned);

            Assert.AreEqual(expectedRestrictionCount, actualRestCount);

            for (ushort i = 0; i < actualRestCount; i++)
                Assert.AreEqual(arg.AddressRestrictions.ElementAt(i), actualAddressRestrictions.ElementAt(i));

        }
    }
   
}
