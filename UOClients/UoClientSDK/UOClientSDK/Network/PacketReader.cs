using System;
using System.Text;

namespace UoClientSDK.Network
{
    class PacketReader
    {
        public ClientVersion Version { get; private set; }

        readonly PacketBuffer Buffer;
        ushort _Position=0;
        public ushort Position { get { return _Position; } }
        public readonly ushort Length;

        public byte this[int index] { get { return index < Length ? Buffer[index] : (byte)0; } }

        public PacketReader(ClientVersion version, PacketBuffer buffer, ushort packetlength)
        {
            Version = version;
            Buffer = buffer;
            Length = packetlength;
        }

        public byte ReadByte()
        {
            if (Length < Position + 1) return 0;
            return Buffer[_Position++];
        }

        public short ReadShort()
        {
            if (Length < Position + 2) return 0;
            short value = (short)(((short)(ReadByte() << 8)) | (short)ReadByte());
            return value;
        }

        public ushort ReadUShort()
        {
            return (ushort)ReadShort();
        }

        public int ReadInt()
        {
            if (Length < Position + 4) return 0;
            int value = (ReadByte() << 24) | (ReadByte() << 16) | (ReadByte() << 8) | ReadByte();
            return value;
        }

        public uint ReadUInt()
        {
            return (uint)ReadInt();
        }

        public Serial ReadSerial()
        {
            return new Serial { Value = ReadInt() };
        }

        public bool ReadBool()
        {
            return ReadByte() != 0;
        }

        public string ReadNullString()
        {
            return ReadNullString((ushort)(Length - Position));
        }

        public string ReadNullString(ushort maxLen)
        {
            ushort read;
            StringBuilder sbText = new StringBuilder();
            for (read = 0; read < Length - Position; )
            {
                char c = (char)Buffer[Position + (read++)];
                if (c == (char)0) break;
                sbText.Append(c);
            }
            _Position += read;
            return sbText.ToString();
        }

        public string ReadFixedString(ushort fixedlength)
        {
            ushort beginIndex = Position;
            string text = ReadNullString(fixedlength);
            _Position = Math.Max(_Position, (ushort)(beginIndex + fixedlength));
            return text;
        }

    }
}
