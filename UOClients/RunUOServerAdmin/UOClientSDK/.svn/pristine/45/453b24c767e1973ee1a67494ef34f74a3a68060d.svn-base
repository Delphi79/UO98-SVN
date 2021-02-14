using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace UoClientSDK.Network.ClientPackets
{
    public enum RemoteAdminSubCommand : byte
    {
        AdminLoginRequest = 0x02,
        AdminServerInfoRequest = 0x04,
        AccountSearch = 0x05,
        RemoveAccount = 0x06,
        UpdateAccount = 0x07,
        RequestUOGStatusCompact = 0xFE,
        RequestUOGStatus = 0xFF,
    }

    public abstract class RemoteAdminClientPacket : ClientPacket
    {
        public abstract RemoteAdminSubCommand SubCommandID { get; }

        public override ClientPacketId PacketID { get { return ClientPacketId.RemoteAdmin; } }

        public RemoteAdminClientPacket(ClientVersion version) : base(version) { }

        protected PacketWriter RemoteAdminWriter(ushort DataLength)
        {
            ushort remoteAdminHeaderSize = 3;
            ushort remoteAdminSubCommandSize = 1;
            ushort remoteAdminPacketSize = (ushort)(DataLength + remoteAdminSubCommandSize + remoteAdminHeaderSize);
            PacketWriter writer = new PacketWriter(remoteAdminPacketSize);
            writer.Write(ClientPacketId.RemoteAdmin);
            writer.Write((ushort)remoteAdminPacketSize);
            writer.Write((byte)SubCommandID);
            return writer;
        }
    }

    public class AdminLogin : RemoteAdminClientPacket
    {
        public override RemoteAdminSubCommand SubCommandID { get { return RemoteAdminSubCommand.AdminLoginRequest; } }

        string Username;
        string Password;

        public AdminLogin(ClientVersion version, string username, string password) : base(version)
        {
            Username = username;
            Password = password;
        }

        internal override PacketWriter GetWriter()
        {
            PacketWriter writer = RemoteAdminWriter((ushort)(60));
            writer.WriteStringFixed(Username, 30);
            writer.WriteStringFixed(Password, 30);
            return writer;
        }
    }

    public class AdminServerInfoRequest : RemoteAdminClientPacket
    {
        public override RemoteAdminSubCommand SubCommandID { get { return RemoteAdminSubCommand.AdminServerInfoRequest; } }

        public AdminServerInfoRequest(ClientVersion version) : base(version) { }

        internal override PacketWriter GetWriter()
        {
            PacketWriter writer = RemoteAdminWriter((ushort)0);
            return writer;
        }
    }

    public class AdminAccountSearch : RemoteAdminClientPacket
    {
        public override RemoteAdminSubCommand SubCommandID { get { return RemoteAdminSubCommand.AccountSearch; } }

        public string SearchTerm { get; private set; }
        public AcctSearchType SearchType { get; private set; }

        public AdminAccountSearch(ClientVersion version, AcctSearchType type, string term) : base(version) 
        {
            SearchType = type;
            SearchTerm = term ?? string.Empty;
        }

        internal override PacketWriter GetWriter()
        {
            PacketWriter writer = RemoteAdminWriter((ushort)(1 + SearchTerm.Length + 1));
            
            writer.Write((byte)SearchType);
            writer.WriteStringNull(SearchTerm);

            return writer;
        }

        public enum AcctSearchType : byte
        {
            Username = 0,
            IP = 1,
        }
    }

    public class AdminUpdateAccount : RemoteAdminClientPacket
    {
        public override RemoteAdminSubCommand SubCommandID { get { return RemoteAdminSubCommand.UpdateAccount; } }

        public struct UpdateAccountArg
        {
            public string Username;
            public string Password;

            public AccessLevel AccessLevel;
            public bool Banned;
            public IEnumerable<string> AddressRestrictions;
        }

        UpdateAccountArg Arg;

        public AdminUpdateAccount(ClientVersion version, UpdateAccountArg arg) : base(version) 
        {
            Arg = arg;
        }

        internal override PacketWriter GetWriter()
        {
            int restLen = 0;
            foreach(string rest in Arg.AddressRestrictions)
                restLen += rest.Length + 1;

            int length = Arg.Username.Length + 1 + Arg.Password.Length + 1 + 1 + 1 + 2 + restLen;

            PacketWriter writer = RemoteAdminWriter((ushort)(length));
            
            writer.WriteStringNull(Arg.Username);
            writer.WriteStringNull(Arg.Password);

            writer.Write((byte)Arg.AccessLevel);
            writer.Write(Arg.Banned);

            int restCount = Arg.AddressRestrictions.Count();
            writer.Write((ushort)restCount);

            foreach(string restriction in Arg.AddressRestrictions)
                writer.WriteStringNull(restriction);

            return writer;
        }

    }

    public class AdminRequestUOGStatusCompact : RemoteAdminClientPacket
    {
        public override RemoteAdminSubCommand SubCommandID { get { return RemoteAdminSubCommand.RequestUOGStatusCompact; } }

        public AdminRequestUOGStatusCompact(ClientVersion version) : base(version) { }

        internal override PacketWriter GetWriter()
        {
            PacketWriter writer = RemoteAdminWriter((ushort)0);
            return writer;
        }
    }

    public class AdminRequestUOGStatus : RemoteAdminClientPacket
    {
        public override RemoteAdminSubCommand SubCommandID { get { return RemoteAdminSubCommand.RequestUOGStatus; } }

        public AdminRequestUOGStatus(ClientVersion version) : base(version) { }

        internal override PacketWriter GetWriter()
        {
            PacketWriter writer = RemoteAdminWriter((ushort)0);
            return writer;
        }
    }
}
