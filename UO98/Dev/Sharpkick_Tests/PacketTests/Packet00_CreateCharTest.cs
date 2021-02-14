using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sharpkick;
using Sharpkick.Network;

namespace Sharpkick_Tests
{
    
    
    /// <summary>
    ///This is a test class for Packet00_CreateCharTest and is intended
    ///to contain all Packet00_CreateCharTest Unit Tests
    ///</summary>
    [TestClass()]
    public class Packet00_CreateCharTest
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

        /// <summary>
        ///A test for Packet00_CreateChar Constructor
        ///</summary>
        [TestMethod()]
        unsafe public void Packet00_CreateCharConstructorTest_Normal()
        {
            using (MockClient client = new MockClient(42, TestUserIP))
            {
                client.Version = ClientVersion.v1_26_2.Version;

                Assert.AreEqual(0u, client.SocketDataLength, "Data length should be zero");

                MockClient_00_CreateCharater packetIn = new MockClient_00_CreateCharater(client.Version);

                packetIn.StartX = 43;
                packetIn.StartY = 75;
                packetIn.StartZ = 0;
                packetIn.Female = 1;
                packetIn.Str = 49;
                packetIn.Int = 50;
                packetIn.Dex = 25;
                packetIn.Skill1 = 22;
                packetIn.Skill1val = 30;
                packetIn.Skill2 = 6;
                packetIn.Skill2val = 3;
                packetIn.Skill3 = 21;
                packetIn.Skill3val = 53;

                client.Enqueue(packetIn);

                Assert.AreEqual(MockClient_00_CreateCharater.v1_26_0len, client.SocketDataLength, "Socket should contain packet, datalength should equal packet length.");

                try
                {
                    ClientPacketSafe packetOut = client.ProcessAndCheck(packetIn);
                }
                catch (VerificationException ex)
                {
                    Assert.Fail(ex.Message);
                }

                Assert.AreEqual(MockClient_00_CreateCharater.expectedLen, client.SocketDataLength, "Socket should still contain packet, length should be {0}.", MockClient_00_CreateCharater.expectedLen);
            }
        }

        [TestMethod()]
        unsafe public void Packet00_CreateCharConstructorTest_FixSkills()
        {
            using (MockClient client = new MockClient(42, TestUserIP))
            {
                Assert.AreEqual(0u, client.SocketDataLength, "Data length should be zero");

                MockClient_00_CreateCharater packetIn = new MockClient_00_CreateCharater(client.Version);

                byte skill1 = 22;
                byte skill2 = 22;
                byte skill3 = 100;

                byte skill1Val = 24;
                byte skill2Val = 34;
                byte skill3val = 153;

                packetIn.StartX = 23;
                packetIn.StartY = 15;
                packetIn.StartZ = 10;
                packetIn.Female = 0;
                packetIn.Str = 49;
                packetIn.Int = 50;
                packetIn.Dex = 25;
                packetIn.Skill1 = skill1;
                packetIn.Skill1val = skill1Val;
                packetIn.Skill2 = skill2;
                packetIn.Skill2val = skill2Val;
                packetIn.Skill3 = skill3;
                packetIn.Skill3val = skill3val;

                client.Enqueue(packetIn);

                Assert.AreEqual(packetIn.Length, client.SocketDataLength, "Socket should contain packet, datalength should equal packet length.");

                Packet00_CreateChar packetOut = client.ProcessOnly(packetIn) as Packet00_CreateChar;

                Assert.AreEqual(MockClient_00_CreateCharater.expectedLen, (uint)packetOut.Length, "Unexpected packet size.");

                Assert.AreNotEqual(packetOut.Skill1, packetOut.Skill2, "Skill1 and Skill2 should not be equal.");
                Assert.AreNotEqual(packetOut.Skill1, packetOut.Skill3, "Skill1 and Skill3 should not be equal.");
                Assert.AreNotEqual(packetOut.Skill2, packetOut.Skill3, "Skill2 and Skill3 should not be equal.");

                Assert.IsTrue(packetOut.Skill3 >= 0 && packetOut.Skill3 < Server.SkillsObject.SkillCount, "Skill3 is out of range: {0}", packetOut.Skill3);

                Assert.AreEqual(skill1Val, packetOut.Skill1val, "Skill1 value mismatch");
                Assert.AreEqual(skill2Val, packetOut.Skill2val, "Skill2 value mismatch");
                Assert.AreEqual(skill3val, packetOut.Skill3val, "Skill3 value mismatch");

                Assert.AreEqual(packetIn.Length, client.SocketDataLength, "Socket should still contain packet.");
            }
        }
    }
}
