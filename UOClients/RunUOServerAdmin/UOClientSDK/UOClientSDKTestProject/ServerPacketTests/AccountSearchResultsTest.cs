using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UoClientSDK;
using UoClientSDK.Network;
using UoClientSDK.Network.ServerPackets;

namespace UOClientSDKTestProject
{
    
    
    /// <summary>
    ///This is a test class for AccountSearchResultsTest and is intended
    ///to contain all AccountSearchResultsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AccountSearchResultsTest
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

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Instantiate
        ///</summary>
        [TestMethod()]
        public void AccountSearchResultsInstantiateTest()
        {
            string accountName = "TestAccountName";
            string password = "testPass";
            AccessLevel accesslevel = (AccessLevel)1;
            bool banned = true;
            uint lastloginticks = 56473;

            AccountSearchResults packet = BuildAccountSearchResultsPacket(accountName, password, accesslevel, banned, lastloginticks, null, null);
            Assert.IsNotNull(packet);
            
            AccountResult result=packet.Accounts.FirstOrDefault();
            Assert.IsNotNull(result);

            Assert.AreEqual(accountName, result.Username);
            
        }

        AccountSearchResults BuildAccountSearchResultsPacket(string accountName, string password, UoClientSDK.AccessLevel accessLevel, bool banned, uint lastloginticks, IEnumerable<IPAddress> addresses, IEnumerable<IPAddress> restrictions)
        {
            byte[] buffer = new byte[4000];
            buffer[0] = (byte)ServerPacketId.AdminAccountSearchResults;

            int index = 3;
            buffer[index++] = 1; // count

            index += Utility.WriteStringToBuffer(accountName, buffer, index);
            index += Utility.WriteStringToBuffer(password, buffer, index);
            buffer[index++] = (byte)accessLevel;
            buffer[index++] = banned ? (byte)1 : (byte)0;

            index += Utility.WriteUInt(lastloginticks, buffer, index);

            // addresses
            buffer[index++] = 0;
            buffer[index++] = 0;

            // restrictions
            buffer[index++] = 0;
            buffer[index++] = 0;

            ushort length = (ushort)(index);
            Utility.WriteUShort(length, buffer, 1);

            PacketBuffer pBuffer = new PacketBuffer(buffer);
            PacketReader reader = new PacketReader(ClientVersion.vMAX, pBuffer, (ushort)index);

            return AccountSearchResults.Instantiate(reader);
        }
    }
}
