using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpkick.Network;

namespace Sharpkick_Tests
{

    public class VerificationException : Exception
    {
        public VerificationException(string message) : base(message) { }
        public VerificationException(string messageformat, params object[] args) : base(string.Format(messageformat, args)) { }
    }

    abstract class BasePacketMock
    {
        public byte PacketID { get; protected set; }
        public uint Length { get; protected set; }

        public abstract byte[] PacketData { get; }

        public int WriteAsciiNull(int start, string value)
        {
            int i = start;
            if (!string.IsNullOrEmpty(value))
            {
                ASCIIEncoding.ASCII.GetBytes(value, 0, value.Length, PacketData, i);
                i += ASCIIEncoding.ASCII.GetByteCount(value);
            }
            PacketData[i++] = 0x00;
            return i - start;
        }

    }

    abstract class BaseClientPacketMock : BasePacketMock
    {
        byte[] _Data;
        public override byte[] PacketData { get { return _Data; } }
        
        public abstract bool Dynamic { get; }

        public abstract void VerifyTransform(ClientPacketSafe resultPacket);

        unsafe public BaseClientPacketMock(byte packetid, uint length)
        {
            PacketID = packetid;
            Length = length;

            _Data = new byte[length];
            _Data[0] = packetid;
        }
    }

    abstract class BaseServerPacketMock : BasePacketMock
    {
        byte[] _Data;
        public override byte[] PacketData { get { return _Data; } }

        public BaseServerPacketMock(byte packetid, uint length)
        {
            PacketID = packetid;
            Length = length;
            _Data = new byte[length];
            _Data[0] = packetid;
        }
     
        unsafe protected int getDataInt(int index)
        {
            if (index >= PacketData.Length) return 0;
            fixed (byte* data = PacketData)
                return *(int*)(data + index);
        }

        unsafe protected short getDataShort(int index)
        {
            if (index >= PacketData.Length) return 0;
            fixed (byte* data = PacketData)
                return *(short*)(data + index);
        }

        unsafe protected ushort getDataUShort(int index)
        {
            if (index >= PacketData.Length) return 0;
            fixed (byte* data = PacketData)
                return *(ushort*)(data + index);
        }

        protected byte getDataByte(int index)
        {
            if (index >= PacketData.Length) return 0;
            return PacketData[index];
        }

        unsafe protected void SetData(int index, int value)
        {
            if (index < PacketData.Length)
                fixed (byte* data = PacketData)
                    *(int*)(data + index) = value;
        }

        unsafe protected void SetData(int index, short value)
        {
            if (index < PacketData.Length)
                fixed (byte* data = PacketData)
                    *(short*)(data + index) = value;
        }

        unsafe protected void SetData(int index, ushort value)
        {
            if (index < PacketData.Length)
                fixed (byte* data = PacketData)
                    *(ushort*)(data + index) = value;
        }

        unsafe protected void SetData(int index, byte value)
        {
            if (index < PacketData.Length)
                PacketData[index] = value;
        }
    }
}
