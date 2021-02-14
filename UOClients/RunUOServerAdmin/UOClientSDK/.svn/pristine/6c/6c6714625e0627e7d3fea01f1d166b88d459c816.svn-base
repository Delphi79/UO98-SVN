using UoClientSDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using UOClientSDKTestProject.Servers;
using UoClientSDK.Network.ServerPackets;

namespace UOClientSDKTestProject
{
    
    
    /// <summary>
    ///This is a test class for ClientTest and is intended
    ///to contain all ClientTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ClientTest
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

        static BaseServer echoserver = null;

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            echoserver = new EchoServer();
            Thread.Sleep(250); // give server time to start up.
        }
        
        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            if (echoserver != null) echoserver.Stop();
        }

        /// <summary>
        ///A test for ConnectToServer
        ///</summary>
        [TestMethod()]
        public void OnConnectedTest()
        {
            bool connected = false;
            int ThisThreadID = Thread.CurrentContext.ContextID;
            int OnConnectedThreadID = -1;

            using (Client target = CreateClient())
            {
                target.OnConnected += delegate(string server, int port) { connected = true; OnConnectedThreadID = Thread.CurrentContext.ContextID; };
                target.ConnectToServer(IPAddress.Loopback.ToString(), echoserver.Port);

                WaitForTrue(ref connected);

            }

            Assert.IsTrue(connected);
            Assert.AreEqual(ThisThreadID, OnConnectedThreadID, "OnConnected Executed on a different thread!");
        }

        [TestMethod()]
        public void OnPacketTest()
        {
            using (Client client = CreateClient())
            {
                bool connected = false;
                bool gotpacket = false;
                List<ServerPacket> Packets = new List<ServerPacket>();

                int ThisThreadID = Thread.CurrentContext.ContextID;
                int OnConnectedThreadID = -1;
                int OnPacketThreadID = -1;

                client.OnConnected += delegate(string server, int port) { connected=true; OnConnectedThreadID = Thread.CurrentContext.ContextID; };
                client.OnUnhandledPacket += delegate(ServerPacket packet) { gotpacket = true; Packets.Add(packet); OnPacketThreadID = Thread.CurrentContext.ContextID; };

                client.ConnectToServer(IPAddress.Loopback.ToString(), echoserver.Port);

                WaitForTrue(ref connected);

                if (!connected)
                    Assert.Inconclusive("Unable to connect to server.");
                else
                {
                    byte LoginDeniedPacketID = 0x82;
                    LoginRejectReason LoginDeniedReasonCode1 = LoginRejectReason.BadPass;
                    LoginRejectReason LoginDeniedReasonCode2 = LoginRejectReason.Idle;

                    byte[] TwoLoginDeniedPackets = new byte[] { LoginDeniedPacketID, (byte)LoginDeniedReasonCode1, LoginDeniedPacketID, (byte)LoginDeniedReasonCode2 };
                    client.Send(TwoLoginDeniedPackets, 4);

                    Assert.IsTrue(Packets.Count == 0, "We should not have been able to receive a packet this fast, the packet pump may be running synchronously.");

                    WaitForTrue(ref gotpacket);

                    Assert.IsTrue(gotpacket);

                    Assert.IsTrue(Packets.Count > 0);
                    Assert.IsTrue(Packets[0] is LoginDeniedPacket);
                    Assert.AreEqual(LoginDeniedReasonCode1, ((LoginDeniedPacket)Packets[0]).Reason);

                    Assert.IsTrue(Packets.Count > 1);
                    Assert.IsTrue(Packets[1] is LoginDeniedPacket);
                    Assert.AreEqual(LoginDeniedReasonCode2, ((LoginDeniedPacket)Packets[1]).Reason);

                    Assert.AreEqual(ThisThreadID, OnConnectedThreadID, "OnConnected Executed on a different thread!");
                    Assert.AreEqual(ThisThreadID, OnPacketThreadID, "OnPacket Executed on a different thread!");
                }

            }
        }

        void WaitForTrue(ref bool trigger, int maxwait = 1000)
        {
            int countdown = maxwait/10;
            while (!trigger && countdown-- > 0) Thread.Sleep(maxwait / 100);
        }

        Client CreateClient()
        {
            Client client = new Client();
            return client;
        }
    }
}
