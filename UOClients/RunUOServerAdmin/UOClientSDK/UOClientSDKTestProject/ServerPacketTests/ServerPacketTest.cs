using UoClientSDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UoClientSDK.Network.ServerPackets;

namespace UOClientSDKTestProject
{
    /// <summary>
    ///This is a test class for ServerPacketTest and is intended
    ///to contain all ServerPacketTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ServerPacketTest
    {
        const ServerPacketId FixedLenPacketId = (ServerPacketId)0x34;
        const ushort FixedLenPacketLen = 44;

        const ServerPacketId DynamicLenPacketId = (ServerPacketId)0x34;

        [ServerPacket(FixedLenPacketId, FixedLenPacketLen)]
        class FixedLenPacket : ServerPacket { }

        [ServerPacket(DynamicLenPacketId)]
        class DynamicLenPacket : ServerPacket { }

        /// <summary>
        ///A test for PacketID
        ///</summary>
        [TestMethod()]
        public void ServerPacketPacketIDTest()
        {
            ServerPacket target = new FixedLenPacket();
            ServerPacketId expected = FixedLenPacketId;
            ServerPacketId actual = target.PacketID;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for PacketLength (Fixed)
        ///</summary>
        [TestMethod()]
        public void ServerPacketPacketLengthFixedTest()
        {
            ServerPacket target = new FixedLenPacket();
            ushort expected = FixedLenPacketLen;
            ushort actual = target.PacketLength;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for PacketLength (Dynamic)
        ///</summary>
        [TestMethod()]
        public void ServerPacketPacketLengthDynamicTest()
        {
            ServerPacket target = new DynamicLenPacket();
            ushort expected = 0;
            ushort actual = target.PacketLength;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsDynamicLength (Fixed)
        ///</summary>
        [TestMethod()]
        public void ServerPacketIsDynamicLengthFalseTest()
        {
            ServerPacket target = new FixedLenPacket();
            bool expected = false;
            bool actual;
            actual = target.IsDynamicLength;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsDynamicLength (Dynamic)
        ///</summary>
        [TestMethod()]
        public void ServerPacketIsDynamicLengthTrueTest()
        {
            ServerPacket target = new DynamicLenPacket();
            bool expected = true;
            bool actual;
            actual = target.IsDynamicLength;
            Assert.AreEqual(expected, actual);
        }

    }
}
