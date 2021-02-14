using Sharpkick.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sharpkick;

namespace Sharpkick_Tests
{
    
    
    /// <summary>
    ///This is a test class for ScriptsTest and is intended
    ///to contain all ScriptsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ScriptsTest
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


        [TestMethod()]
        public void Test_Scripts_AddWhenValid()
        {
            bool expected = true;
            bool actual;
            actual = Scripts.Test_Scripts_AddWhenValid();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Test_Scripts_AddWhenInvalidScript()
        {
            bool expected = true;
            bool actual;
            actual = Scripts.Test_Scripts_AddWhenInvalidScript();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Test_Scripts_AddWhenInvalidItem()
        {
            bool expected = true;
            bool actual;
            actual = Scripts.Test_Scripts_AddWhenInvalidItem();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Test_Scripts_DeleteWhenValid()
        {
            bool expected = true;
            bool actual;
            actual = Scripts.Test_Scripts_DeleteWhenValid();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Test_Scripts_DeleteWhenDoesntHave()
        {
            bool expected = true;
            bool actual;
            actual = Scripts.Test_Scripts_DeleteWhenDoesntHave();
            Assert.AreEqual(expected, actual);
        }
       
        [TestMethod()]
        public void Test_Scripts_DeleteWhenInvalidItem()
        {
            bool expected = true;
            bool actual;
            actual = Scripts.Test_Scripts_DeleteWhenInvalidItem();
            Assert.AreEqual(expected, actual);
        }
       
        [TestMethod()]
        public void Test_Scripts_DeleteWhenInvalidScript()
        {
            bool expected = true;
            bool actual;
            actual = Scripts.Test_Scripts_DeleteWhenInvalidScript();
            Assert.AreEqual(expected, actual);
        }
    }
}
