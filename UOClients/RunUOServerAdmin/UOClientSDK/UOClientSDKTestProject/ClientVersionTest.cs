using UoClientSDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UOClientSDKTestProject
{
    
    
    /// <summary>
    ///This is a test class for ClientVersionTest and is intended
    ///to contain all ClientVersionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ClientVersionTest
    {
        /// <summary>
        /// This is a test that (int)uint.MaxValue does not lose data and results in a negative number, and is convertible back.
        ///</summary>
        [TestMethod()]
        public void UIntCastTest()
        {
            uint testval=uint.MaxValue;

            int result = (int)testval;

            Assert.IsTrue(result < 1);
            Assert.AreEqual(testval, (uint)result);
        }

        /// <summary>
        ///A test for op_LessThan
        ///</summary>
        [TestMethod()]
        public void op_LessThanTest()
        {
            ClientVersion a = ClientVersion.vMIN;

            ClientVersion b = ClientVersion.v6_0_1_7;
            ClientVersion B = ClientVersion.v6_0_1_7;

            Assert.IsTrue(a < b);
            Assert.IsFalse(b < a);

            Assert.IsFalse(b < B);
            Assert.IsFalse(B < b);
        }

        /// <summary>
        ///A test for op_LessThanOrEqual
        ///</summary>
        [TestMethod()]
        public void op_LessThanOrEqualTest()
        {
            ClientVersion a = ClientVersion.v7_0_0_0;

            ClientVersion b = ClientVersion.vMAX;
            ClientVersion B = ClientVersion.vMAX;

            Assert.IsTrue(a <= b);
            Assert.IsFalse(b <= a);

            Assert.IsTrue(b <= B);
            Assert.IsTrue(B <= b);
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod()]
        public void EqualsTest()
        {
            ClientVersion a = ClientVersion.v4_0_07a;

            ClientVersion b = ClientVersion.v5_0_7_0;
            ClientVersion B = ClientVersion.v5_0_7_0;

            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(b.Equals(a));

            Assert.IsTrue(b.Equals(B));
            Assert.IsTrue(B.Equals(b));
        }

        /// <summary>
        ///A test for op_GreaterThanOrEqual
        ///</summary>
        [TestMethod()]
        public void op_GreaterThanOrEqualTest()
        {
            ClientVersion a = ClientVersion.v1_26_4b;

            ClientVersion b = ClientVersion.v1_26_4i;
            ClientVersion B = ClientVersion.v1_26_4i;

            Assert.IsFalse(a >= b);
            Assert.IsTrue(b >= a);

            Assert.IsTrue(b >= B);
            Assert.IsTrue(B >= b);
        }

        /// <summary>
        ///A test for op_GreaterThan
        ///</summary>
        [TestMethod()]
        public void op_GreaterThanTest()
        {
            ClientVersion a = ClientVersion.v1_26_0;

            ClientVersion b = ClientVersion.v1_26_2;
            ClientVersion B = ClientVersion.v1_26_2;

            Assert.IsFalse(a > b);
            Assert.IsTrue(b > a);

            Assert.IsFalse(b > B);
            Assert.IsFalse(B > b);
        }
    }
}
