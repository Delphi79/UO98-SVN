using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick.Network
{
    abstract class Packet
    {
        /// <summary>Packet ID</summary>
        public byte Id { get; protected set; }

        /// <summary>Length of data, excludes ID byte</summary>
        public int Length { get; protected set; }

        public abstract bool Dynamic { get; }

        public abstract byte[] Data { get; }
    }

    /// <summary>
    /// A packet intercepted from server. This packet has an underlying server buffer
    /// </summary>
    unsafe abstract class UnsafePacket : Packet
    {
        protected virtual byte* pData { get; private set; }

        public override byte[] Data
        {
            get
            {
                byte[] data = new byte[Length];
                for (int i = 0; i < Length; i++) data[i] = *(pData + i);
                return data;
            }
        }

        public UnsafePacket(byte* data, uint length)
        {
            length = Math.Max(length, 1);
            Length = (int)length;
            Id = *data;
            pData = data;
        }

        /// <summary>
        /// Read a fixed length string from the packet buffer
        /// </summary>
        /// <param name="start">Index in buffer to begin reading</param>
        /// <param name="length">The length of the string</param>
        /// <returns>The string read</returns>
        public string ReadAsciiStringFixed(int start, int length)
        {
            length = Math.Min(start + length, Length) - start;
            return StringPointerUtils.GetAsciiString(pData + start, length);
        }

        public string ReadUniStringFixed(int start, int length)
        {
            int i = start;
            int end = start + (length << 1);
            int c;

            StringBuilder sb = new StringBuilder();

            while ((i + 1) < end && (c = ((Data[i++] << 8) | Data[i++])) != 0)
                sb.Append((char)c);

            return sb.ToString();
        }

        public string ReadAsciiStringNull(int start)
        {
            byte* pStr = pData + start;
            return StringPointerUtils.GetAsciiString(pStr);
        }

        public uint ReadUInt(int start)
        {
            return ((uint)*(pData + start) << 24) + ((uint)*(pData + start + 1) << 16) + ((uint)*(pData + start + 2) << 8) + (*(pData + start + 3));
        }

        public ushort ReadUShort(int start)
        {
            return (ushort)(((uint)*(pData + start) << 8) + (*(pData + start + 1)));
        }

        public short ReadShort(int start)
        {
            return (short)(((int)*(pData + start) << 8) + (*(pData + start + 1)));
        }

        public byte ReadByte(int start)
        {
            return (*(pData + start));
        }

        public sbyte ReadSByte(int start)
        {
            return (sbyte)(*(pData + start));
        }

    }

}
