using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace UoClientSDK.Network.ServerPackets
{
    public delegate void OnPacketEventHandler<TServerPacket>(TServerPacket packet) where TServerPacket : ServerPacket;

    public sealed class ServerPacketHandlers : IDisposable
    {
        Dictionary<Type, IPacketHandler> Handlers = new Dictionary<Type, IPacketHandler>();

        internal IPacketHandler GetHandler(ServerPacket PacketInstance)
        {
            if (Handlers.ContainsKey(PacketInstance.GetType()))
                return (IPacketHandler)Handlers[PacketInstance.GetType()];

            MethodInfo method = GetType().GetMethod("GetHandler", BindingFlags.Instance | BindingFlags.Public);
            method = method.MakeGenericMethod(PacketInstance.GetType());
            return (IPacketHandler)method.Invoke(this,null);
        }

        /// <summary>
        /// Obtain the handler for a packet of type TServerPacket. Used to subscribe to the packets received handler.
        /// </summary>
        /// <typeparam name="TServerPacket"></typeparam>
        /// <returns></returns>
        public PacketHandler<TServerPacket> GetHandler<TServerPacket>() where TServerPacket : ServerPacket
        {
            if(Handlers.ContainsKey(typeof(TServerPacket)))
                return (PacketHandler<TServerPacket>)Handlers[typeof(TServerPacket)];
           
            PacketHandler<TServerPacket> handler=new PacketHandler<TServerPacket>();
            Handlers[typeof(TServerPacket)]=handler;
            return handler;
        }

        public void Dispose()
        {
            foreach (IDisposable handler in Handlers.Values)
                try
                {
                    handler.Dispose();
                }
                catch { }
            Handlers.Clear();
        }
    }

    internal interface IPacketHandler : IDisposable
    {
        bool Invoke(ServerPacket packet);
    }

    public sealed class PacketHandler<TServerPacket> : IPacketHandler where TServerPacket : ServerPacket
    {
        /// <summary>
        /// Raised when the packet of type TServerPacket is received. If this is not subscribed, <see cref="Client.OnUnhandledPacket">OnUnhandledPacket</see> will be raised.
        /// </summary>
        public event OnPacketEventHandler<TServerPacket> OnPacket;
        
        /// <summary>
        /// Invokes the handler. This should not normally be used outside of the UOClientSDK internals.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public bool Invoke(ServerPacket packet)
        {
            if (OnPacket != null && packet is TServerPacket)
            {
                OnPacket((TServerPacket)packet);
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            OnPacket = null;
        }
    }
}
