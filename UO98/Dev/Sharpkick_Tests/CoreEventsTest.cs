using Sharpkick;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sharpkick.Tests;

namespace Sharpkick_Tests
{
    
    
    /// <summary>
    ///This is a test class for CoreEventsTest and is intended
    ///to contain all CoreEventsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CoreEventsTest
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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            Server.MockCore(new MockCore());
        }

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
        /// Tests that OnPulse works, and that the Initialization routines are run.
        ///</summary>
        [TestMethod()]
        public void FirstPulseTest()
        {
            int InitialPulses = Server.TimeManager.PulseNum;

            Assert.AreEqual(InitialPulses, 0);
            Assert.IsFalse(Sharpkick.Main.Initialized);

            CoreEvents.OnPulse();
            int ResultPulses = Server.TimeManager.PulseNum;
            int Expected = InitialPulses + 1;

            Assert.IsTrue(Sharpkick.Main.Initialized);

            Assert.AreEqual(ResultPulses, Expected);
        }
    }
}
