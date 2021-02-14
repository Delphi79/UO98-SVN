using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UoClientSDK.Network.ServerPackets
{
    delegate ServerPacket ServerPacketInstanciator(PacketReader reader);

    public delegate void UnknownPacketEvent(string message, byte[] buffer);
    
    internal class ServerPacketInfo
    {
        public ServerPacketId packetid;
        public bool isFixedLen;
        public ushort FixedLength;
        public Type Type;
        public ServerPacketInstanciator Factory;
    }

    internal class ServerPacketFactory : IServerPacketFactory
    {
        private Dictionary<ServerPacketId, ServerPacketInfo> _registry;

        internal int RegisteredPacketCount { get { return _registry == null ? 0 : _registry.Count; } }

        internal event UnknownPacketEvent OnUnknownPacket;

        void InvokeOnUnknownPacket(string text, byte[] buffer)
        {
            if (OnUnknownPacket != null)
                OnUnknownPacket(text, buffer);
            else
                Console.WriteLine(text);
        }

        public ClientVersion Version { get; private set; }

        internal ServerPacketFactory(ClientVersion version)
        {
            Version = version;
            RegisterPackets();
        }

        internal ServerPacketInfo GetInfoForRegisteredPacketID(ServerPacketId packetid)
        {
            ServerPacketInfo toReturn;
            _registry.TryGetValue(packetid, out toReturn);
            return toReturn;
        }

        internal ServerPacket RecieveSinglePacketIfAvailable(PacketBuffer pBuffer)
        {
            if (pBuffer.Length <= 0) return null;
            ServerPacketId packetid = (ServerPacketId)pBuffer[0];

            ServerPacketInfo info;
            if (!_registry.TryGetValue(packetid, out info))
            {
                InvokeOnUnknownPacket(string.Format("Invalid packet {0}, clearing buffer.", packetid), pBuffer.buffer);
                pBuffer.ClearIncoming();
            }
            else
            {
                ushort packetlen;
                if (!info.isFixedLen)
                {
                    if (pBuffer.Length < 3)
                        return null;
                    packetlen = (ushort)(pBuffer[1] << 8 | pBuffer[2]);
                }
                else
                    packetlen = info.FixedLength;
                if (pBuffer.Length >= packetlen)
                {
                    ServerPacket packet = null;
                    if (info.Factory != null)
                    {
                        packet = info.Factory(new PacketReader(Version, pBuffer, packetlen));
                    }
                    pBuffer.RemoveBufferHead(packetlen);
                    return packet;
                }
            }
            return null;
        }

        private void RegisterPackets()
        {
            IEnumerable<Type> types = GetServerPacketTypes();
            RegisterPackets(types);
        }

        IEnumerable<Type> GetServerPacketTypes()
        {
            return ReflectionHelpers.GetAttributedTypesFromAssembly<ServerPacketAttribute>();
        }

        private void RegisterPackets(IEnumerable<Type> types)
        {
            _registry = new Dictionary<ServerPacketId, ServerPacketInfo>();
            
            List<PacketRegistrationException> RegistrationFailures = new List<PacketRegistrationException>();

            foreach (Type t in types)
            {
                ServerPacketAttribute attr = (ServerPacketAttribute)t.GetCustomAttributes(typeof(ServerPacketAttribute), false).FirstOrDefault();
                if (attr == null)
                    RegistrationFailures.Add(new InvalidPacketException(t, string.Format("Server Packet {0} must be adorned with ServerPacketAttribute to be registered.", t), null));
                else if (ServerPacket.IsValidForClientVersion(attr, Version))
                {
                    try
                    {
                        Register(t, attr.PacketID, attr.isFixedLength, attr.FixedLength);
                    }
                    catch (PacketRegistrationException ex)
                    {
                        RegistrationFailures.Add(ex);
                    }
                }
            }

            if (RegistrationFailures.Count > 0)
                throw new PacketRegistrationExceptions(RegistrationFailures);
        }

        void Register<T>() where T : ServerPacket
        {
            ServerPacketId packetid = ServerPacket.GetPacketID<T>();
            bool isfixedlength = !ServerPacket.GetIsDynamicLength<T>();
            ushort fixedlength = ServerPacket.GetFixedPacketLength<T>();
            Register(typeof(T), packetid, isfixedlength, fixedlength);
        }

        void Register(Type packettype, ServerPacketId PacketID, bool isFixedLength, ushort fixedlength)
        {
            MethodInfo method = packettype.GetMethod("Instantiate", BindingFlags.Static | BindingFlags.NonPublic);
            ServerPacketInstanciator factory;
            if (method == null)
                factory = null;
            else
            {
                try
                {
                    factory = Delegate.CreateDelegate(typeof(ServerPacketInstanciator), method) as ServerPacketInstanciator;
                }
                catch (Exception ex)
                {
                    throw new InvalidPacketException(packettype, string.Format("Server Packet {0} must implement or omit internal static Instantiate(PacketReader reader)", packettype), ex);
                }
            }

            if (_registry.ContainsKey(PacketID))
            {
                throw new DuplicatePacketRegistrationException(packettype, _registry[PacketID].Type);
            }
            else
            {
                _registry.Add(PacketID, new ServerPacketInfo() { packetid = PacketID, isFixedLen = isFixedLength, FixedLength = fixedlength, Type = packettype, Factory = factory });
            }
        }
    }

    public class PacketRegistrationException : Exception
    {
        public PacketRegistrationException() : base() { }
        public PacketRegistrationException(string message) : base(message) { }
        public PacketRegistrationException(string message, Exception inner) : base(message, inner) { }
    }

    public class PacketRegistrationExceptions : PacketRegistrationException
    {
        public IEnumerable<PacketRegistrationException> Exceptions { get; private set; }
        public PacketRegistrationExceptions(IEnumerable<PacketRegistrationException> exceptions)
            : base("One or more packet registrations failed, review the Exceptions property for a list of individual exceptions.")
        {
            Exceptions = exceptions;
        }

        public override string ToString()
        {
            return string.Format("{0}\n\nIndividual Exceptions:{1}\n", base.ToString(), string.Join("\n   ", Exceptions.Select(ex => ex.Message).ToArray()));
        }
    }

    public class PacketRegistrationExceptionOfType : PacketRegistrationException
    {
        public Type FailedType { get; private set; }

        public PacketRegistrationExceptionOfType(Type failedtype) : base() { FailedType = failedtype; }
        public PacketRegistrationExceptionOfType(Type failedtype, string message) : base(message) { FailedType = failedtype; }
        public PacketRegistrationExceptionOfType(Type failedtype, string message, Exception inner) : base(message, inner) { FailedType = failedtype; }
    }

    public class InvalidPacketException : PacketRegistrationExceptionOfType
    {
        public InvalidPacketException(Type failedtype, string message, Exception inner) : base(failedtype, message, inner) { }
    }

    public class DuplicatePacketRegistrationException : PacketRegistrationExceptionOfType
    {
        public Type ExistingType { get; private set; }
        
        public DuplicatePacketRegistrationException(Type failedtype, Type existingtype) : base(failedtype)
        {
            ExistingType = existingtype;
        }

        private string message = null;
        public override string Message
        {
            get
            {
                return message ?? (message = string.Format("Packet {0} conflicts with registered packet {1}", FailedType, ExistingType));
            }
        }
    }
}