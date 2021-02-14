using System;
using System.Runtime.InteropServices;

namespace UoClientSDK.Network
{
    public class PacketWriter
    {
        public ushort Capacity { get { return (ushort)(Packet == null ? 0 : Packet.Length); } }

        public readonly byte[] Packet;
        public ushort Length { get; private set; }

        public PacketWriter(int capacity) : this((ushort)capacity) { }
        public PacketWriter(ushort capacity)
        {
            Packet = new byte[capacity];
            Length = 0;
        }

        public void Skip(ushort bytesToAdvance)
        {
            if (Length + bytesToAdvance < Capacity)
                Length += bytesToAdvance;
            else
                throw new BufferOverflowException("Packet Writer Capacity Exceeded");
        }

        public void Write(byte value)
        {
            if(Length < Capacity)
                Packet[Length++] = value;
            else
                throw new BufferOverflowException("Packet Writer Capacity Exceeded");
        }

        public void Write(bool value)
        {
            Write(value ? (byte)1 : (byte)0);
        }

        public void Write(ClientPackets.ClientPacketId value)
        {
            Write((byte)value);
        }

        public void Write(short value)
        {
            Write((byte)((value & 0xFF00) >> 8));
            Write((byte)(value & 0xFF));
        }

        public void Write(ushort value)
        {
            Write((short)value);
        }

        public void Write(int value)
        {
            Write((byte)((value & 0xFF00000000) >> 24));
            Write((byte)((value & 0xFF0000) >> 16));
            Write((byte)((value & 0xFF00) >> 8));
            Write((byte)(value & 0xFF));
        }

        public void Write(uint value)
        {
            Write((int)value);
        }

        public void WriteStringNull(string text)
        {
            WriteStringFixed(text, text.Length + 1);
        }

        public void WriteStringFixed(string text, int length)
        {
            for(int i = 0; i < length; i++)
                if(text != null && i < text.Length)
                    Write((byte)text[i]);
                else
                    Write((byte)0);
        }
        
        public unsafe void WriteStruct<T>(T layout) where T : struct
        {
            int structLen = Marshal.SizeOf(layout);
            if(Length + structLen <= Capacity)
            {
                fixed(byte* bytes = &Packet[Length])
                    Marshal.StructureToPtr(layout, new IntPtr(bytes), true);
                Length += (ushort)structLen;
            }
            else
                throw new BufferOverflowException("Packet Writer Capacity Exceeded");
        }

    }
}
