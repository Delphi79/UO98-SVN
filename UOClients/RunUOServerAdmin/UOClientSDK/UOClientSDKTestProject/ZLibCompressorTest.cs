using UoClientSDK.Compression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace UOClientSDKTestProject
{
    
    
    /// <summary>
    ///This is a test class for ZLibCompressorTest and is intended
    ///to contain all ZLibCompressorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ZLibCompressorTest
    {

        ///// <summary>
        /////A test for Compress
        /////</summary>
        //[TestMethod()]
        //public void CompressTest()
        //{
        //    ZLibCompressor target = new ZLibCompressor(); // TODO: Initialize to an appropriate value
        //    byte[] dest = null; // TODO: Initialize to an appropriate value
        //    int destLength = 0; // TODO: Initialize to an appropriate value
        //    int destLengthExpected = 0; // TODO: Initialize to an appropriate value
        //    byte[] source = null; // TODO: Initialize to an appropriate value
        //    int sourceLength = 0; // TODO: Initialize to an appropriate value
        //    ZLibQuality quality = new ZLibQuality(); // TODO: Initialize to an appropriate value
        //    ZLibError expected = new ZLibError(); // TODO: Initialize to an appropriate value
        //    ZLibError actual;
        //    actual = target.Compress(dest, ref destLength, source, sourceLength, quality);
        //    Assert.AreEqual(destLengthExpected, destLength);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        [TestMethod()]
        public void ZLibCompressStreamTest()
        {
            ZLibCompressor target = new ZLibCompressor();
            
            string originalText="Test string.";

            MemoryStream msOriginal = new MemoryStream(ASCIIEncoding.ASCII.GetBytes(originalText));

            byte[] compressbuffer=new byte[2000];
            MemoryStream msCompressed = new MemoryStream(compressbuffer);

            int outLen=0;

            ZLibError err = target.Compress(msOriginal, msCompressed, ref outLen, ZLibQuality.Default);
            Assert.AreEqual(ZLibError.Okay, err);

            MemoryStream msToDecompress = new MemoryStream(compressbuffer, 0, outLen);

            byte[] decompressbuffer = new byte[2000];
            MemoryStream msFinalResult = new MemoryStream(decompressbuffer);

            err = target.Decompress(msToDecompress, msFinalResult, ref outLen);
            Assert.AreEqual(ZLibError.Okay, err);

            string result = ASCIIEncoding.ASCII.GetString(decompressbuffer, 0, outLen);

            Assert.AreEqual(originalText, result);
        }

    }
}
