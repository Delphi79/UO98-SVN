using Sharpkick.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sharpkick;

namespace Sharpkick_Tests
{
    
    
    /// <summary>
    ///This is a test class for ObjVarsTest and is intended
    ///to contain all ObjVarsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ObjVarsTest
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
        public void Test_ObjVar_Int()
        {
            bool expected = true;
            bool actual;
            actual = ObjVars.Test_ObjVar_Int();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Test_ObjVar_Location()
        {
            bool expected = true;
            bool actual;
            actual = ObjVars.Test_ObjVar_Location();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Test_ObjVar_String()
        {
            bool expected = true;
            bool actual;
            actual = ObjVars.Test_ObjVar_String();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Test_ObjVar_MiscBehavior()
        {
            bool expected = true;
            bool actual;
            actual = ObjVars.Test_ObjVar_MiscBehavior();
            Assert.AreEqual(expected, actual);
        }
    }
}
