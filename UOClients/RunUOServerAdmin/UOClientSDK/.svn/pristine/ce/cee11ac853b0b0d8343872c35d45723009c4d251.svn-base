using System;
using System.IO;

namespace UoClientSDK.Network
{
    class PacketBuffer
    {
        public readonly int Capacity;
        public byte[] buffer;

        public int Length = 0;

        public byte this[int index] { get { return index < Capacity ? buffer[index] : (byte)0; } }

        public PacketBuffer(int capacity) : this(new byte[capacity]) { }
        public PacketBuffer(byte[] preConstructedBuffer)
        {
            buffer = preConstructedBuffer;
            Capacity = buffer.Length;
        }


        public bool DataAvailable { get { return Length > 0; } }

        public int Read(Stream stream)
        {
            if (Capacity - Length <= 0)
                throw new BufferOverflowException();

            int read = stream.Read(buffer, Length, Capacity - Length);
            Length += read;
            return read;
        }

        public void RemoveBufferHead(int bytesToRemove)
        {
            Length -= bytesToRemove;
            Array.Copy(buffer, bytesToRemove, buffer, 0, Length);
        }

        public void ClearIncoming()
        {
            Length = 0;
        }
    }

    public class BufferOverflowException : InvalidOperationException
    {
        public BufferOverflowException() : this("Read buffer is full") { }
        public BufferOverflowException(string message) : base(message) { }
    }
}
