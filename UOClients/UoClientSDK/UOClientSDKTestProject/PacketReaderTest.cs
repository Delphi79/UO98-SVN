using Microsoft.VisualStudio.TestTools.UnitTesting;
using UoClientSDK.Network;
using UoClientSDK;

namespace UOClientSDKTestProject
{
    
    
    /// <summary>
    ///This is a test class for PacketReader and is intended
    ///to contain all PacketReader Unit Tests
    ///</summary>
    [TestClass()]
    public class PacketReaderTest
    {

        /// <summary>
        ///A test for ReadNullString
        ///</summary>
        [TestMethod()]
        public void ReadNullStringTest()
        {
            string toEncode = "Hello world!";

            byte[] buffer = new byte[1000];
            int bufferPosition = 22;

            Utility.WriteStringToBuffer(toEncode, buffer, bufferPosition);

            PacketBuffer pBuffer = new PacketBuffer(buffer);
            PacketReader reader = new PacketReader(ClientVersion.vMAX, pBuffer, (ushort)buffer.Length);

            for (int i = 0; i < bufferPosition; i++) reader.ReadByte(); // advance

            int positionExpected = bufferPosition + toEncode.Length + 1;

            string expected = toEncode;
            string actual;
            
            actual = reader.ReadNullString();
            Assert.AreEqual(positionExpected, reader.Position);
            Assert.AreEqual(expected, actual);

            buffer = new byte[toEncode.Length];
            bufferPosition = 0;

            Utility.WriteStringToBuffer(toEncode, buffer, bufferPosition);

            pBuffer = new PacketBuffer(buffer);
            reader = new PacketReader(ClientVersion.vMAX, pBuffer, (ushort)buffer.Length);

            positionExpected = bufferPosition + toEncode.Length;

            actual = reader.ReadNullString();
            Assert.AreEqual(positionExpected, reader.Position);
            Assert.AreEqual(expected, actual);

        }

        /// <summary>
        ///A test for ReadFixedString
        ///</summary>
        [TestMethod()]
        public void ReadFixedStringTest()
        {
            string toEncode = "Hello world!";

            byte[] buffer = new byte[30];
            int positionExpected = 30;
            ushort length = 30;
            string expected = toEncode;
            string actual;

            Utility.WriteStringToBuffer(toEncode, buffer, 0);

            PacketBuffer pBuffer = new PacketBuffer(buffer);
            PacketReader reader = new PacketReader(ClientVersion.vMAX, pBuffer, (ushort)buffer.Length);

            actual = reader.ReadFixedString(length);
            Assert.AreEqual(positionExpected, reader.Position);
            Assert.AreEqual(expected, actual);
        }

   


    }
}
