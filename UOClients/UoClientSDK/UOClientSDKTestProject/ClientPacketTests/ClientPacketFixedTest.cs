using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UoClientSDK;
using UoClientSDK.Network.ClientPackets;

namespace UOClientSDKTestProject
{
    
    
    /// <summary>
    ///This is a test class for ClientPacketFixedTest and is intended
    ///to contain all ClientPacketFixedTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ClientPacketFixedTest
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


        ///// <summary>
        /////A test for GetBytes exception
        /////</summary>
        //[TestMethod()]
        //[ExpectedException(typeof(InvalidClientPacketException))]
        //public void GetBytesBadIDExceptionTest()
        //{
        //    LoginRequest.LoginRequestArgs ImproperlyConstructedPacketLayoutContainingNoPacketIDByte = new LoginRequest.LoginRequestArgs();
        //    LoginRequest packet = new LoginRequest(ClientVersion.vMAX, ImproperlyConstructedPacketLayoutContainingNoPacketIDByte);
        //    byte[] bytes = packet.GetBytes();
        //}
    }
}
