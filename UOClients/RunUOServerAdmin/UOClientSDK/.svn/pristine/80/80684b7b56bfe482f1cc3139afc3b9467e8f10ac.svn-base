using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UoClientSDK;
using UoClientSDK.Network;
using UoClientSDK.Network.ServerPackets;

namespace UOClientSDKTestProject
{
    
    /// <summary>
    ///This is a test class for PacketHandlerTest and is intended
    ///to contain all PacketHandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PacketFactoryTest
    {
        static ClientVersion version = ClientVersion.v6_0_1_7;
        ServerPacketFactory handler = new ServerPacketFactory(version);


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
        ///A test for ProcessSinglePacket
        ///</summary>
        [TestMethod()]
        public void ClearReadBufferOnUnknownPacketTest()
        {
            byte[] garbagebuffer = new byte[] { 5, 4, 3, 2, 1, 4, 5 };

            int bufferDataLengthExpected = 0;

            bool eventRaised = false;
            string OnInvalidPacketMessage = null;
            string expectMessagetoContain = "clearing buffer";

            ServerPacket expected = null;
            ServerPacket actual;
            PacketBuffer reader = new PacketBuffer(20);

            handler.OnUnknownPacket += delegate(string text, byte[] buffer) { 
                eventRaised = true; 
                OnInvalidPacketMessage = text; 
                Assert.AreEqual(buffer, reader.buffer);
                for (int i = 0; i < garbagebuffer.Length; i++)
                    Assert.AreEqual(buffer[i], garbagebuffer[i]);
            };
            
            using (MemoryStream ms = new MemoryStream(garbagebuffer))
            {
                reader.Read(ms);
                actual = handler.RecieveSinglePacketIfAvailable(reader);
            }

            Assert.AreEqual(bufferDataLengthExpected, reader.Length);
            Assert.AreEqual(expected, actual);
            Assert.IsTrue(eventRaised, "Expected OnInvalidPacket event was not raised.");
            Assert.IsNotNull(OnInvalidPacketMessage, "OnInvalidPacket event did not provide a message.");
            Assert.IsTrue(OnInvalidPacketMessage.Contains(expectMessagetoContain));
        }

        /// <summary>
        ///A test for ProcessSinglePacket
        ///</summary>
        [TestMethod()]
        public void ProcessSinglePacketTest()
        {
            int logindeniedpacketlength = 2;

            byte ExtraByte1=0xAB;
            byte ExtraByte2=0xBA;

            byte LoginDeniedPacketID = 0x82;
            LoginRejectReason LoginDeniedReasonCode = LoginRejectReason.CharTransfer;

            byte[] buffer = new byte[] { LoginDeniedPacketID, (byte)LoginDeniedReasonCode, ExtraByte1, ExtraByte2 };

            int bufferDataLengthExpected = buffer.Length - logindeniedpacketlength;

            ServerPacket packet;
            PacketBuffer reader = new PacketBuffer(20);
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                reader.Read(ms);
                packet = handler.RecieveSinglePacketIfAvailable(reader);
            }

            Assert.AreEqual(bufferDataLengthExpected, reader.Length);
            Assert.IsTrue(packet is LoginDeniedPacket);
            Assert.AreEqual(LoginDeniedReasonCode, ((LoginDeniedPacket)packet).Reason);

            Assert.AreEqual(ExtraByte1, reader[0]);
            Assert.AreEqual(ExtraByte2, reader[1]);
        }

        /// <summary>
        ///A test for ProcessSinglePacket
        ///</summary>
        [TestMethod()]
        public void ProcessSinglePacketSegmentedTest()
        {
            byte LoginDeniedPacketID = 0x82;
            LoginRejectReason LoginDeniedReasonCode = LoginRejectReason.CharTransfer;

            byte[] buffer = new byte[1];

            buffer[0] = LoginDeniedPacketID;

            int bufferLengthExpected = 1;

            ServerPacket packet;
            PacketBuffer reader = new PacketBuffer(20);
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                reader.Read(ms);
                packet = handler.RecieveSinglePacketIfAvailable(reader);
            }

            Assert.AreEqual(bufferLengthExpected, reader.Length);
            Assert.IsNull(packet);

            buffer = new byte[2];
            buffer[0] = LoginDeniedPacketID;
            buffer[1] = (byte)LoginDeniedReasonCode;
            bufferLengthExpected = 0;

            reader = new PacketBuffer(20);
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                reader.Read(ms);
                packet = handler.RecieveSinglePacketIfAvailable(reader);
            }

            Assert.AreEqual(bufferLengthExpected, reader.Length);
            Assert.IsTrue(packet is LoginDeniedPacket);
            Assert.AreEqual(LoginDeniedReasonCode, ((LoginDeniedPacket)packet).Reason);
        }

        [ServerPacket((ServerPacketId)0x42)]
        public class RegisterablePacketWithInstanciator : ServerPacket
        {
            internal static RegisterablePacketWithInstanciator Instantiate(PacketReader reader)
            {
                return new RegisterablePacketWithInstanciator();
            }
        }

        [ServerPacket((ServerPacketId)0x42)]
        public class RegisterablePacketWithoutInstanciator : ServerPacket
        {
        }

        [ServerPacket((ServerPacketId)0x42)]
        public class UnRegisterablePacketWithBadInstanciator : ServerPacket
        {
            internal static RegisterablePacketWithInstanciator Instantiate(byte[] data)
            {
                return new RegisterablePacketWithInstanciator();
            }
        }

        [TestMethod()]
        public void RegisterPacketWithFactoryTest()
        {
            ServerPacketFactory_Accessor target = new ServerPacketFactory_Accessor(version);

            target.Register<RegisterablePacketWithInstanciator>();

            ServerPacketId id = ServerPacket.GetPacketID<RegisterablePacketWithInstanciator>();
            var info = target.GetInfoForRegisteredPacketID(id);
            Assert.IsNotNull(info.Factory);
        }

        [TestMethod()]
        public void RegisterPacketWithoutFactoryTest()
        {
            ServerPacketFactory_Accessor target = new ServerPacketFactory_Accessor(version);

            target.Register<RegisterablePacketWithoutInstanciator>();

            ServerPacketId id = ServerPacket.GetPacketID<RegisterablePacketWithoutInstanciator>();
            var info = target.GetInfoForRegisteredPacketID(id);
            Assert.IsNull(info.Factory);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPacketException))]
        public void RegisterBadPacketTest()
        {
            ServerPacketFactory_Accessor target = new ServerPacketFactory_Accessor(version);

            target.Register<UnRegisterablePacketWithBadInstanciator>();
        }

        [ServerPacket((ServerPacketId)0x72)]
        class ServerPacketDupe1 : ServerPacket { }
        [ServerPacket((ServerPacketId)0x72)]
        class ServerPacketDupe2 : ServerPacket { }

        /// <summary>
        ///A test for RegisterPackets
        ///</summary>
        [TestMethod()]
        public void RegisterPacketsThrowExceptionOnDupeTest()
        {
            ServerPacketFactory_Accessor target = new ServerPacketFactory_Accessor(version);
            IEnumerable<Type> types = new List<Type> { typeof(ServerPacketDupe1), typeof(ServerPacketDupe2) };

            try
            {
                target.RegisterPackets(types);
            }
            catch (PacketRegistrationException ex)
            {
                Assert.IsTrue(ex is PacketRegistrationExceptions);
                
                PacketRegistrationExceptions caught = (PacketRegistrationExceptions)ex;
                
                Assert.AreEqual(1, caught.Exceptions.Count());
                Assert.IsTrue(caught.Exceptions.FirstOrDefault() is DuplicatePacketRegistrationException);
                
                DuplicatePacketRegistrationException inner = (DuplicatePacketRegistrationException)caught.Exceptions.FirstOrDefault();
                Assert.AreEqual(typeof(ServerPacketDupe1), inner.ExistingType);
                Assert.AreEqual(typeof(ServerPacketDupe2), inner.FailedType);

                return;
            }
            Assert.Fail("Expected exception not thrown.");
        }

        const ServerPacketId PacketIDForVersioningTest = (ServerPacketId)0x73;
        [ServerPacket(PacketIDForVersioningTest, startversion: null, endversion: "1.0.0.0")]
        class ServerPacketVersion0 : ServerPacket { }
        [ServerPacket(PacketIDForVersioningTest, startversion: "1.0.0.0", endversion: "2.0.0.0")]
        class ServerPacketVersion1 : ServerPacket { }
        [ServerPacket(PacketIDForVersioningTest, startversion: "2.0.0.0")]
        class ServerPacketVersion2 : ServerPacket { }

        /// <summary>
        ///A test for RegisterPackets
        ///</summary>
        [TestMethod()]
        public void RegisterPacketsByVersionTest()
        {
            ServerPacketFactory_Accessor targetV0_0 = new ServerPacketFactory_Accessor(ClientVersion.vMIN);
            ServerPacketFactory_Accessor targetV1_0 = new ServerPacketFactory_Accessor(ClientVersion.Instantiate("1.0.0.0"));
            ServerPacketFactory_Accessor targetV1_5 = new ServerPacketFactory_Accessor(ClientVersion.Instantiate("1.5.0.0"));
            ServerPacketFactory_Accessor targetV2_0 = new ServerPacketFactory_Accessor(ClientVersion.Instantiate("2.0.0.0"));
            ServerPacketFactory_Accessor targetV3_0 = new ServerPacketFactory_Accessor(ClientVersion.Instantiate("3.0.0.0"));

            IEnumerable<Type> types = new List<Type> { typeof(ServerPacketVersion0), typeof(ServerPacketVersion1), typeof(ServerPacketVersion2) };

            targetV0_0.RegisterPackets(types);
            targetV1_0.RegisterPackets(types);
            targetV1_5.RegisterPackets(types);
            targetV2_0.RegisterPackets(types);
            targetV3_0.RegisterPackets(types);

            int expectedCount = 1;

            Assert.AreEqual(expectedCount, targetV0_0.RegisteredPacketCount);
            Assert.AreEqual(expectedCount, targetV1_0.RegisteredPacketCount);
            Assert.AreEqual(expectedCount, targetV1_5.RegisteredPacketCount);
            Assert.AreEqual(expectedCount, targetV2_0.RegisteredPacketCount);
            Assert.AreEqual(expectedCount, targetV3_0.RegisteredPacketCount);

            Assert.AreEqual(typeof(ServerPacketVersion0), targetV0_0.GetInfoForRegisteredPacketID(PacketIDForVersioningTest).Type);
            Assert.AreEqual(typeof(ServerPacketVersion1), targetV1_0.GetInfoForRegisteredPacketID(PacketIDForVersioningTest).Type);
            Assert.AreEqual(typeof(ServerPacketVersion1), targetV1_5.GetInfoForRegisteredPacketID(PacketIDForVersioningTest).Type);
            Assert.AreEqual(typeof(ServerPacketVersion2), targetV2_0.GetInfoForRegisteredPacketID(PacketIDForVersioningTest).Type);
            Assert.AreEqual(typeof(ServerPacketVersion2), targetV3_0.GetInfoForRegisteredPacketID(PacketIDForVersioningTest).Type);

        }

        class UnAdornedServerPacket : ServerPacket { }

        /// <summary>
        ///A test for RegisterPackets
        ///</summary>
        [TestMethod()]
        public void RegisterPacketsThrowExceptionOnInvalidTest()
        {
            ServerPacketFactory_Accessor target = new ServerPacketFactory_Accessor(version);
            IEnumerable<Type> types = new List<Type> { typeof(UnAdornedServerPacket), typeof(ServerPacketDupe2) };

            try
            {
                target.RegisterPackets(types);
            }
            catch (PacketRegistrationException ex)
            {
                Assert.IsTrue(ex is PacketRegistrationExceptions);
                
                PacketRegistrationExceptions caught = (PacketRegistrationExceptions)ex;
                
                Assert.AreEqual(1, caught.Exceptions.Count());
                Assert.IsTrue(caught.Exceptions.FirstOrDefault() is InvalidPacketException);

                InvalidPacketException inner = (InvalidPacketException)caught.Exceptions.FirstOrDefault();

                Assert.AreEqual(typeof(UnAdornedServerPacket), inner.FailedType);

                return;
            }
            Assert.Fail("Expected exception not thrown.");
        }
    }
}
