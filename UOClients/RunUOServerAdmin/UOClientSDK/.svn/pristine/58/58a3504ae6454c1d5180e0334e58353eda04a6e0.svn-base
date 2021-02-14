using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ComponentAce.Compression.Libs.zlib;
using System.IO;

namespace UoClientSDK.Compression
{
    class ZLibCompressor : ICompressor
    {
        public string Version { get { return zlibConst.version(); } }

        public ZLibError Compress(byte[] dest, ref int destLength, byte[] source, int sourceLength)
        {
            return Compress(dest, ref destLength, source, sourceLength, ZLibQuality.Default);
        }

        public ZLibError Compress(byte[] dest, ref int destLength, byte[] source, int sourceLength, ZLibQuality quality)
        {
            using (MemoryStream msOut = new MemoryStream(dest, true))
            using (MemoryStream msIn = new MemoryStream(source, 0, sourceLength, false))
                return Compress(msIn, msOut, ref destLength, quality);
        }

        public ZLibError Compress(Stream inStream, Stream outStream, ref int outLength, ZLibQuality quality)
        {
            ZOutputStream outZStream = new ZOutputStream(outStream, (int)quality);
            outZStream.FlushMode = zlibConst.Z_FULL_FLUSH;

            try
            {
                CopyStream(inStream, outZStream);
                outZStream.Flush();
                outStream.Flush();
                outLength = (int)outStream.Position;
            }
            // TODO: Catch exceptions, return error codes
            //catch (ZStreamException)
            //{
            //    return ZLibError.StreamError;
            //}
            finally
            {
                outZStream.Close();
                inStream.Close();
            }

            return ZLibError.Okay;
        }

        public ZLibError Decompress(byte[] dest, ref int destLength, byte[] source, int sourceLength)
        {
            using (MemoryStream msIn = new MemoryStream(source, 0, sourceLength, false))
            using (MemoryStream msOut = new MemoryStream(dest, true))
                return Decompress(msIn, msOut, ref destLength);
        }

        public ZLibError Decompress(Stream inStream, Stream outStream, ref int destLength)
        {
            ZOutputStream outZStream = new ZOutputStream(outStream);
            outZStream.FlushMode = zlibConst.Z_FULL_FLUSH;

            try
            {
                CopyStream(inStream, outZStream);
                outZStream.Flush();
                outStream.Flush();
                destLength = (int)outStream.Position;
            }
            // TODO: Catch exceptions, return error codes
            //catch (ZStreamException)
            //{
            //    return ZLibError.StreamError;
            //}
            finally
            {
                outZStream.Close();
                inStream.Close();
            }

            return ZLibError.Okay;
        }

        private static object copyLock = new object();
        static byte[] streamCopyBuffer = new byte[2000];

        public static int CopyStream(System.IO.Stream input, System.IO.Stream output)
        {
            int read = 0;

            int len;
            lock (copyLock)
            {
                while ((len = input.Read(streamCopyBuffer, 0, 2000)) > 0)
                {
                    read += len;
                    output.Write(streamCopyBuffer, 0, len);
                }
            }
            output.Flush();
            return read;
        }

    }
}
