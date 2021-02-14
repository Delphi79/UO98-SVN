using System.Runtime.InteropServices;

namespace UoClientSDK.Network.ClientPackets
{
    public class LoginRequest : ClientPacket
    {
        const ClientPacketId ID = ClientPacketId.LoginRequest;
        public override ClientPacketId PacketID { get { return ID; } }

        public string AccountName { get; private set; }
        public string Password { get; private set; }
        public byte NextLoginKey { get; private set; }

        public LoginRequest(ClientVersion version, string username, string password, byte nextloginkey)
            : base(version)
        {
            AccountName = username;
            Password = password;
            NextLoginKey = nextloginkey;
        }

        internal override PacketWriter GetWriter()
        {
            PacketWriter writer = new PacketWriter((ushort)(1 + 61));
            writer.Write(PacketID);
            writer.WriteStringFixed(AccountName, 30);
            writer.WriteStringFixed(Password, 30);
            writer.Write(NextLoginKey);
            return writer;
        }
    }
}
