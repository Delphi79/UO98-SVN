#define UseOptionalAttributeBugWorkAround // A problem with optional attribute constructors and nulls, see: http://stackoverflow.com/questions/3436848/default-value-for-attribute-constructor

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UoClientSDK.Network.ServerPackets
{
    public abstract class ServerPacket : Packet
    {
        static Dictionary<Type, ServerPacketAttribute> PacketInfoCache = new Dictionary<Type, ServerPacketAttribute>();

        public ServerPacketId PacketID { get { return PacketInfo.PacketID; } }
        public ushort PacketLength { get { return PacketInfo.FixedLength; } }
        public bool IsDynamicLength { get { return !PacketInfo.isFixedLength; } }

        private ServerPacketAttribute PacketInfo
        {
            get
            {
                Type T = this.GetType();
                ServerPacketAttribute attr;
                if (PacketInfoCache.TryGetValue(T, out attr))
                    return attr;

                Func<ServerPacketAttribute> GenMethod = FindServerPacketAttributeOnType<ServerPacket>;
                MethodInfo method = typeof(ServerPacket).GetMethod(GenMethod.Method.Name, BindingFlags.Static | BindingFlags.NonPublic);
                MethodInfo generic = method.MakeGenericMethod(T);

                return (ServerPacketAttribute)generic.Invoke(this, null);
            }
        }

        public static ServerPacketId GetPacketID<T>() where T : ServerPacket
        {
            return FindServerPacketAttributeOnType<T>().PacketID;
        }

        public static ushort GetFixedPacketLength<T>() where T : ServerPacket
        {
            return FindServerPacketAttributeOnType<T>().FixedLength;
        }

        public static bool GetIsDynamicLength<T>() where T : ServerPacket
        {
            return !FindServerPacketAttributeOnType<T>().isFixedLength;
        }

        public static bool IsValidForClientVersion<T>(ClientVersion version) where T : ServerPacket
        {
            ServerPacketAttribute attr=FindServerPacketAttributeOnType<T>();
            return IsValidForClientVersion(attr, version);
        }

        internal static bool IsValidForClientVersion(ServerPacketAttribute attr, ClientVersion version)
        {
            return version >= attr.StartVersion && ((attr.EndVersion == ClientVersion.vMAX && version == attr.EndVersion) || attr.StartVersion == attr.EndVersion || version < attr.EndVersion);
        }

        private static ServerPacketAttribute FindServerPacketAttributeOnType<T>() where T : ServerPacket
        {
            ServerPacketAttribute attr;
            if (PacketInfoCache.TryGetValue(typeof(T), out attr))
                return attr;

            ServerPacketAttribute idattr = (ServerPacketAttribute)typeof(T).GetCustomAttributes(typeof(ServerPacketAttribute), false).FirstOrDefault();
            if (idattr == null) throw new InvalidOperationException(string.Format("PacketID is not defined on {0}", typeof(T)));
            PacketInfoCache[typeof(T)] = idattr;
            return idattr;
        }

        /// <summary>
        /// Reads the PacketID and Length (if dynamic length). Verifies the packet ID and required packet length.
        /// </summary>
        /// <typeparam name="T">Type of packet</typeparam>
        /// <param name="reader">an instance of a packet reader for this packet</param>
        /// <returns>true if the packet id, length (if dynamic) and available data in the reader are ok</returns>
        internal static bool ReadHead<T>(PacketReader reader) where T : ServerPacket
        {
            bool IsDynamicLength=GetIsDynamicLength<T>();
            int minLen = IsDynamicLength ? 3 : GetFixedPacketLength<T>();
            
            if (reader.Length < minLen) return false;

            if (reader.ReadByte() != (byte)GetPacketID<T>()) return false;

            if(IsDynamicLength)
            {
                ushort len = reader.ReadUShort();
                if (len < reader.Length) return false;
            }
            return true;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    class ServerPacketAttribute : Attribute
    {
        public ServerPacketId PacketID { get; private set; }
        public bool isFixedLength { get; private set; }
        public ushort FixedLength { get; private set; }

        /// <summary>The inclusive version at which this packet is valid.</summary>
        public ClientVersion StartVersion { get; protected set; }
        /// <summary>The exclusive version at which this packet is no longer valid.</summary>
        public ClientVersion EndVersion { get; protected set; }


#if UseOptionalAttributeBugWorkAround // See: http://stackoverflow.com/questions/3436848/default-value-for-attribute-constructor
        public ServerPacketAttribute(ServerPacketId packetId) : this(packetId, 0, null, null) { }
        public ServerPacketAttribute(ServerPacketId packetId, ushort fixedLength) : this(packetId, fixedLength, null, null) { }
        public ServerPacketAttribute(ServerPacketId packetId, ushort fixedLength, string startversion) : this(packetId, fixedLength, startversion, null) { }
        public ServerPacketAttribute(ServerPacketId packetId, string startversion) : this(packetId, 0, startversion, null) { }
        public ServerPacketAttribute(ServerPacketId packetId, string startversion, string endversion) : this(packetId, 0, startversion, endversion) { }
        public ServerPacketAttribute(ServerPacketId packetId, ushort fixedLength, string startversion, string endversion)
#else
        public ServerPacketAttribute(ServerPacketId packetId, ushort fixedLength = 0, string startversion = null, string endversion=null)
#endif
        {
            PacketID = packetId;
            isFixedLength = fixedLength > 0;
            FixedLength = fixedLength;

            StartVersion = startversion==null ? ClientVersion.Instantiate(ClientVersionStruct.Min) : ClientVersion.Instantiate(startversion);
            EndVersion = endversion == null ? ClientVersion.Instantiate(ClientVersionStruct.Max) : ClientVersion.Instantiate(endversion);
        }
    }
}
