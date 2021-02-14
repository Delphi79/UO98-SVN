using UoClientSDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace UOClientSDKTestProject
{
    
    
    /// <summary>
    ///This is a test class for ShardPollerTest and is intended
    ///to contain all ShardPollerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ShardPollerTest
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
        ///A test for BeginPollShard using compact packet. Tests a local server on port 2593 if online, otherwise uses a mock responder
        ///</summary>
        [TestMethod()]
        public void BeginPollShardTestCompact()
        {
            BeginPollShardTest(true);
        }

        /// <summary>
        ///A test for BeginPollShard. Tests a local server on port 2593 if online, otherwise uses a mock responder
        ///</summary>
        [TestMethod()]
        public void BeginPollShardTest()
        {
            BeginPollShardTest(false);
        }

        public void BeginPollShardTest(bool useCompact)
        {
            int port = 2593; // TODO: Initialize to an appropriate value

            Servers.UOGPollResponder server=null;
            try
            {
                server = new Servers.UOGPollResponder(port);
                Thread.Sleep(750); // give server time to start up.
            }
            catch (System.Net.Sockets.SocketException){ server = null; }

            try
            {
                ShardPoller target = new ShardPoller(); // TODO: Initialize to an appropriate value
                string hostname = "localhost"; // TODO: Initialize to an appropriate value

                bool Connected = false;
                bool ConnectFailed = false;
                string FailMessage = null;
                bool InvalidResponse = false;
                bool ValidResponse = false;
                bool ReceivedName = false;
                string ShardName = null;
                ShardPoller.PollResult Result = new ShardPoller.PollResult();

                target.OnConnected += delegate(string actualHostname, int actualPort)
                {
                    Connected = true;
                    Assert.AreEqual(hostname, actualHostname);
                    Assert.AreEqual(port, actualPort);
                };

                target.OnConnectFailed += delegate(string message) { ConnectFailed = true; FailMessage = message; };
                target.OnInvalidResponse += delegate() { InvalidResponse = true; };
                target.OnPollResult += delegate(ShardPoller.PollResult result) { ValidResponse = true; Result = result; };
                target.OnShardName += delegate(string name) { ReceivedName = true; ShardName = name; };

                target.BeginPollShard(hostname, port, useCompact);

                DateTime timeout = DateTime.Now + target.Timeout + TimeSpan.FromSeconds(1.0);
                while (!(ValidResponse || ConnectFailed) && timeout > DateTime.Now)
                    Thread.Sleep(50);
                Thread.Sleep(50);

                Assert.IsFalse(ConnectFailed, string.Format("Using {0} server: {1}", server != null ? "internal mock" : "external", FailMessage));
                Assert.IsTrue(Connected);
                Assert.IsFalse(InvalidResponse);
                Assert.IsTrue(ValidResponse);

                Assert.IsTrue(Result.isOnline);

                if (!useCompact)
                {
                    Assert.IsTrue(ReceivedName);
                    Assert.IsNotNull(ShardName);
                }
                else
                    Assert.IsFalse(ReceivedName);

            }
            finally
            {
                if (server != null)
                    server.Stop();
            }
        }

    }
}
