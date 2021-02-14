using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace UoClientSDK.Network.ClientPackets
{
    public abstract class ClientPacket : Packet
    {
        public abstract ClientPacketId PacketID { get; }
        public ClientVersion Version { get; private set; }
        public ClientPacket(ClientVersion version)
        {
            Version = version;
        }

        internal abstract PacketWriter GetWriter();
    }
}
