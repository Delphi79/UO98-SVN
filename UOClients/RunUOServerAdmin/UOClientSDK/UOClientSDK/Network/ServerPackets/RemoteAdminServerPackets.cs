using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UoClientSDK.Compression;
using System.Net;

namespace UoClientSDK.Network.ServerPackets
{
    [RemoteAdminServerPacket(ServerPacketId.AdminCompressed)]
    public class AdminCompressedPacket : ServerPacket
    {
        internal static ServerPacket Instantiate(PacketReader reader)
        {
            if (!ReadHead<AdminCompressedPacket>(reader)) return null;

            int uncompressedsize = reader.ReadUShort();

            byte[] CompData = new byte[reader.Length - reader.Position];
            for (int i = 0; i < CompData.Length; i++)
                CompData[i] = reader.ReadByte();

            byte[] InternalPacketOfExactLength = new byte[uncompressedsize];
            Compression.Compression.Unpack(InternalPacketOfExactLength, ref uncompressedsize, CompData, CompData.Length);

            if (InternalPacketOfExactLength.Length > 0)
            {
                switch (InternalPacketOfExactLength[0])
                {
                    case 0x02: return LoginResponsePacket.Instantiate(ConstructReaderForInternalPacket(reader.Version, InternalPacketOfExactLength));
                    case 0x03: return ConsoleDataPacket.Instantiate(ConstructReaderForInternalPacket(reader.Version, InternalPacketOfExactLength));
                    case 0x04: return ServerInfoPacket.Instantiate(ConstructReaderForInternalPacket(reader.Version, InternalPacketOfExactLength));
                    case 0x05: return AccountSearchResults.Instantiate(ConstructReaderForInternalPacket(reader.Version, InternalPacketOfExactLength));
                    case 0x08: return AdminMessageBox.Instantiate(ConstructReaderForInternalPacket(reader.Version, InternalPacketOfExactLength));
                }
            }
            return null;
        }

        static PacketReader ConstructReaderForInternalPacket(ClientVersion version, byte[] BufferOfExactLength)
        {
            PacketBuffer buffer = new PacketBuffer(BufferOfExactLength);
            return new PacketReader(version, buffer, (ushort)BufferOfExactLength.Length);
        }
    }

    [RemoteAdminServerPacket(ServerPacketId.AdminLoginResponse, 2)]
    public class LoginResponsePacket : ServerPacket
    {
        public LoginResponse LoginResponse { get; private set; }

        public LoginResponsePacket(LoginResponse loginresponse)
        {
            LoginResponse = loginresponse;
        }

        internal static LoginResponsePacket Instantiate(PacketReader reader)
        {
            if (!ReadHead<LoginResponsePacket>(reader)) return null;
            return new LoginResponsePacket((LoginResponse)reader.ReadByte());
        }
    }

    [RemoteAdminServerPacket(ServerPacketId.AdminConsoleData)]
    public class ConsoleDataPacket : ServerPacket
    {
        public enum DataType : byte
        {
            String=0x02,
            Char=0x03
        }

        public string Text { get; private set; }

        public ConsoleDataPacket(string text)
        {
            Text = text;
        }

        internal static ConsoleDataPacket Instantiate(PacketReader reader)
        {
            if (!ReadHead<ConsoleDataPacket>(reader)) return null;

            DataType type = (DataType)reader.ReadByte();

            string text;
            if (type == DataType.String)
                text = reader.ReadNullString();
            else if (type == DataType.Char)
                text = new string((char)reader.ReadByte(), 1);
            else
                return null;

            return new ConsoleDataPacket(text);
        }
    }

    [RemoteAdminServerPacket(ServerPacketId.AdminServerInfo)]
    public class ServerInfoPacket : ServerPacket
    {
        public int Active { get; private set; }
        public int Banned { get; private set; }
        public int Firewalled { get; private set; }
        public int Clients { get; private set; }
        public int Mobiles { get; private set; }
        public int MobileScripts { get; private set; }
        public int Items { get; private set; }
        public int ItemScripts { get; private set; }
        public uint UpSecs { get; private set; }
        public uint Memory { get; private set; }
        public string DotNetVersion { get; private set; }
        public string OperatingSystem { get; private set; }

        internal static ServerInfoPacket Instantiate(PacketReader reader)
        {
            if (!ReadHead<ServerInfoPacket>(reader)) return null;

            ServerInfoPacket packet = new ServerInfoPacket();

            packet.Active = reader.ReadInt();
            packet.Banned = reader.ReadInt();
            packet.Firewalled = reader.ReadInt();
            packet.Clients = reader.ReadInt();
            packet.Mobiles = reader.ReadInt();
            packet.MobileScripts = reader.ReadInt();
            packet.Items = reader.ReadInt();
            packet.ItemScripts = reader.ReadInt();

            packet.UpSecs = reader.ReadUInt();
            packet.Memory = reader.ReadUInt();

            packet.DotNetVersion = reader.ReadNullString();
            packet.OperatingSystem = reader.ReadNullString();

            return packet;
        }
    }

    [RemoteAdminServerPacket(ServerPacketId.AdminAccountSearchResults)]
    public class AccountSearchResults : ServerPacket
    {
        public IEnumerable<AccountResult> Accounts { get; private set; }

        internal static AccountSearchResults Instantiate(PacketReader reader)
        {
            if (!ReadHead<AccountSearchResults>(reader)) return null;

            byte count = reader.ReadByte();

            List<AccountResult> results = new List<AccountResult>(count);
            for (int i = 0; i < count; i++)
            {
                AccountResult account = new AccountResult()
                {
                    Username = reader.ReadNullString(),
                    Password = reader.ReadNullString(),
                    AccessLevel = (AccessLevel)reader.ReadByte(),
                    Banned = reader.ReadBool(),
                    LastLogin = DateTime.MinValue + TimeSpan.FromTicks(reader.ReadUInt()),  // TODO: This doesn't work, uint.MaxValue is only 7 hours. Fix protocol.
                };

                ushort ipCount = reader.ReadUShort();
                List<string> addresses = new List<string>(ipCount);
                for (int j = 0; j < ipCount; j++)
                    addresses.Add(reader.ReadNullString());

                ushort restCount = reader.ReadUShort();
                List<string> restrictions = new List<string>(restCount);
                for (int j = 0; j < restCount; j++)
                    restrictions.Add(reader.ReadNullString());

                account.Addresses = addresses;
                account.AddressRestrictions = restrictions;

                results.Add(account);

            }
            return new AccountSearchResults() { Accounts = results };
        }

    }

    [RemoteAdminServerPacket(ServerPacketId.AdminMessageBox)]
    public class AdminMessageBox : ServerPacket
    {
        public IEnumerable<AccountResult> Accounts { get; private set; }

        public string Message { get; private set; }
        public string Caption { get; private set; }

        public AdminMessageBox(string message = "", string caption = "")
        {
            Message = message;
            Caption = caption;
        }

        internal static AdminMessageBox Instantiate(PacketReader reader)
        {
            if (!ReadHead<AdminMessageBox>(reader)) return null;

            string message = reader.ReadNullString();
            string caption = reader.ReadNullString();

            return new AdminMessageBox(message, caption);
        }
    }

    [RemoteAdminServerPacket(ServerPacketId.AdminUOGStatusCompact)]
    public class UOGStatusCompact : ServerPacket
    {
        public int Clients { get; private set; }
        public int Items { get; private set; }
        public int Mobiles { get; private set; }
        public TimeSpan Age { get; private set; }
        public long Memory { get; set; }

        internal static UOGStatusCompact Instantiate(PacketReader reader)
        {
            if (!ReadHead<UOGStatusCompact>(reader)) return null;

            UOGStatusCompact packet = new UOGStatusCompact();
            packet.Clients = reader.ReadInt();
            packet.Items = reader.ReadInt();
            packet.Mobiles = reader.ReadInt();
            packet.Age = TimeSpan.FromSeconds(reader.ReadInt());
            packet.Memory = reader.ReadInt();
            packet.Memory = (packet.Memory << 32) | reader.ReadUInt();

            return packet;
        }
    }
    
    public struct AccountResult
    {
        public string Username;
        public string Password;

        public AccessLevel AccessLevel;
        public bool Banned;
        public DateTime LastLogin;
        public IEnumerable<string> Addresses;
        public IEnumerable<string> AddressRestrictions;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    class RemoteAdminServerPacketAttribute : ServerPacketAttribute
    {
        public RemoteAdminServerPacketAttribute(ServerPacketId packetId, ushort fixedLength = 0)
            : base(packetId, fixedLength)
        {
            StartVersion = ClientVersion.vAdminClient;
            EndVersion = ClientVersion.vAdminClient;
        }
    }

    public enum LoginResponse : byte
    {
        NoUser = 0,
        BadIP,
        BadPass,
        NoAccess,
        OK
    }

}
