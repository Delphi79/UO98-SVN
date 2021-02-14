using Sharpkick.Network;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sharpkick;

namespace Sharpkick_Tests
{
    
    
    /// <summary>
    ///This is a test class for Packet80_LoginRequestTest and is intended
    ///to contain all Packet80_LoginRequestTest Unit Tests
    ///</summary>
    [TestClass()]
    public class Packet80_LoginRequestTest
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

        const string TestUserName1 = "testuser";
        const string TestUserName2 = "testuser2";
        const string TestUserPass = "testpass";
        System.Net.IPAddress TestUserIP = new System.Net.IPAddress(new byte[] { 1, 2, 3, 4 });

        /// <summary>
        ///A test for Packet80_LoginRequest Constructor
        ///</summary>
        [TestMethod()]
        unsafe public void Packet80_LoginRequest_Test()
        {
            Accounting.Configure();

            int accountid1 = TestSuccess(TestUserName1);
            int accountid2 = TestSuccess(TestUserName2);

            Assert.AreNotEqual(accountid1, accountid2, "Two consecutively created accounts have the same account id.");

            int loginresult1 = TestSuccess(TestUserName1);
            Assert.AreEqual(accountid1, loginresult1, "Login did not result in correct accountid for account {0}", TestUserName1);

            int loginresult2 = TestSuccess(TestUserName2);
            Assert.AreEqual(accountid2, loginresult2, "Login did not result in correct accountid for account {0}", TestUserName2);

            TestFail(TestUserName1, null, ALRReason.BadPass);
            TestFail(null, null, ALRReason.BadPass);
            TestFail(null, TestUserPass, ALRReason.BadPass);
            TestFail(TestUserName2, "wrongpassword", ALRReason.BadPass);
        }

        int TestCreate(string username)
        {
            int newAccountID = TestSuccess(username);

            Account account = Accounting.Get(newAccountID);
            Assert.AreEqual(username, account.Username);

            bool AccountWasCreatedInTheLast1Seconds = account.Created.AddSeconds(1.0) > DateTime.Now;
            Assert.IsTrue(AccountWasCreatedInTheLast1Seconds);

            return newAccountID;
        }

        unsafe int TestSuccess(string username)
        {
            using (MockClient client = new MockClient(42, TestUserIP))
            {
                Assert.AreEqual(0u, client.SocketDataLength, "Data length should be zero");

                MockClient_80_Login packetIn = new MockClient_80_Login(client.Socket, username, TestUserPass);
                client.Enqueue(packetIn);

                Assert.AreEqual(packetIn.Length, client.SocketDataLength, "Socket should contain packet, datalength should equal packet length.");

                Packet80_LoginRequest packetOut = null;
                try
                {
                    packetOut = client.ProcessAndCheck(packetIn) as Packet80_LoginRequest;
                }
                catch (VerificationException ex)
                {
                    Assert.Fail(ex.Message);
                }

                Assert.AreEqual(packetIn.Length, client.SocketDataLength, "Socket should still contain packet, as login should have been accepted.");

                int accountID;
                bool UsernameIsAccountID = int.TryParse(packetOut.Username, out accountID);

                Assert.IsTrue(UsernameIsAccountID, "Expected username to be an account id.");
                Assert.IsTrue(string.IsNullOrEmpty(packetOut.Password), "Password should be empty.");

                Account account = Accounting.Get(accountID);
                Assert.AreEqual(username, account.Username);

                bool passwordIsCorrect = account.Login(TestUserPass, TestUserIP);
                Assert.IsTrue(passwordIsCorrect, "Password didn't work.");

                return accountID;
            }
        }

        unsafe void TestFail(string username, string password, ALRReason expectedFailReason)
        {
            using (MockClient client = new MockClient(45, TestUserIP))
            {
                Assert.AreEqual(0u, client.SocketDataLength, "Data length should be zero");

                MockClient_80_Login packetIn = new MockClient_80_Login(client.Socket, username, password);
                client.Enqueue(packetIn);

                Assert.AreEqual(packetIn.Length, client.SocketDataLength, "Socket should contain packet, datalength should equal packet length.");

                try
                {
                    Packet80_LoginRequest packetOut = client.ProcessAndCheck(packetIn) as Packet80_LoginRequest;
                }
                catch (VerificationException ex)
                {
                    Assert.Fail(ex.Message);
                }

                Assert.AreEqual(0u, client.SocketDataLength, "Packet should have been removed.");

                MockServer_82_LoginDenied expectedPacketResponse = new MockServer_82_LoginDenied(expectedFailReason);
                string responce = ((MockPacketEngine)Server.PacketEngine).VerifySent(client, expectedPacketResponse.PacketData);
                Assert.IsTrue(string.IsNullOrEmpty(responce), "Failed to verify reject packet: {0}", responce);
            }
        }
    }
}
