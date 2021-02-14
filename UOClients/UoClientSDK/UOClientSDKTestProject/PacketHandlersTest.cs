using Microsoft.VisualStudio.TestTools.UnitTesting;
using UoClientSDK;
using UoClientSDK.Network.ServerPackets;

namespace UOClientSDKTestProject
{
    /// <summary>
    ///This is a test class for PacketHandlersTest and is intended
    ///to contain all PacketHandlersTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PacketHandlersTest
    {
        /// <summary>
        ///A test for GetHandler
        ///</summary>
        [TestMethod()]
        public void PacketHandlersGetHandlerTest()
        {
            ServerPacketHandlers target = new ServerPacketHandlers();
            ServerPacket PacketInstance = new LoginCompletePacket();

            IPacketHandler handler = target.GetHandler(PacketInstance);

            Assert.IsTrue(handler is PacketHandler<LoginCompletePacket>, "A handler of type LoginCompletePacket was expected, the obtained handler was of type {0}", handler.GetType());

            Assert.IsFalse(handler.Invoke(PacketInstance), "Invoke should return false when there is no subscribed handlers");

            bool invokedHandler = false;
            target.GetHandler<LoginCompletePacket>().OnPacket += delegate(LoginCompletePacket packet){invokedHandler=true;};

            Assert.IsTrue(handler.Invoke(PacketInstance), "Invoke should return true when there are any subscribed handlers");
            Assert.IsTrue(invokedHandler, "The expected handler was not invoked");
        }


        /// <summary>
        ///A test for Dispose
        ///</summary>
        [TestMethod()]
        public void DisposeTest()
        {
            ServerPacketHandlers handlers = new ServerPacketHandlers();
            ServerPacket PacketInstance = new DamagePacket(new Serial(), 0);

            IPacketHandler handler = handlers.GetHandler<DamagePacket>();
            Assert.IsFalse(handler.Invoke(PacketInstance), "Invoke should return false when there is no subscribed handlers");

            handlers.GetHandler<DamagePacket>().OnPacket += delegate(DamagePacket packet) { };
            Assert.IsTrue(handler.Invoke(PacketInstance), "Invoke should return true when there are any subscribed handlers");

            handlers.Dispose();

            Assert.IsFalse(handler.Invoke(PacketInstance), "Invoke should return false after dispose, as there should be no subscribed handlers");

        }
    }
}
