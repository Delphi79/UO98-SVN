using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UoClientSDK.Network;

namespace UoClientSDK
{
    public delegate void EventHandler();
    public delegate void OnConnectedEvent(string server,int port);
    public delegate void OptionalStringEvent(string text = null);
    public delegate void PacketEventHandler(Network.ServerPackets.ServerPacket packet);

    public interface IClient : IDisposable
    {
        event OnConnectedEvent OnConnected;
        event OptionalStringEvent OnConnectFailed;
        event EventHandler OnDisconnected;
        event OptionalStringEvent OnReadBufferOverflow;

        event PacketEventHandler OnUnhandledPacket;
        event Network.ServerPackets.UnknownPacketEvent OnUnknownPacket;

        IPEndPoint EndPoint { get; }
        bool IsConnected { get; }

        void ConnectToServer(string server, int port, IPEndPoint localEP = null);
        void Disconnect();

        void Send(byte[] data, int length);
        void Send(Network.ClientPackets.ClientPacket Packet);
    }

    public class Client : IClient
    {
        public readonly ClientVersion Version;

        private Network.ServerPackets.ServerPacketFactory Factory;

        private Network.ServerPackets.ServerPacketHandlers _Handlers = new Network.ServerPackets.ServerPacketHandlers();
        /// <summary>
        /// Event Factory for specific packets received events.
        /// </summary>
        public Network.ServerPackets.ServerPacketHandlers Handlers { get { return _Handlers; } }

        const int PumpSleep = 10;

        const int MaxPacketSize = 512 * 1024;
        PacketBuffer Reader = new PacketBuffer(MaxPacketSize);

        /// <summary>
        /// The current connected client remote endpoint, or null if disconnected.
        /// </summary>
        public IPEndPoint EndPoint { get; private set; }

        public event OnConnectedEvent OnConnected;
        public event OptionalStringEvent OnConnectFailed;
        public event EventHandler OnDisconnected;

        /// <summary>
        /// Raised when there are no subscribers to the received packet type's OnPacket event.
        /// </summary>
        public event PacketEventHandler OnUnhandledPacket;

        public event OptionalStringEvent OnReadBufferOverflow;

        /// <summary>
        /// Raised when the packet factory cannot interpret the packet stream. Data may be lost when this occurs. 
        /// If this event is not handled, the message will be printed to Console.
        /// </summary>
        public event Network.ServerPackets.UnknownPacketEvent OnUnknownPacket;

        public Client(): this (ClientVersion.vMAX){}
        public Client(ClientVersion version)
        {
            Version = version;
            Factory = new Network.ServerPackets.ServerPacketFactory(Version);
            Factory.OnUnknownPacket += new Network.ServerPackets.UnknownPacketEvent(Factory_OnUnknownPacket);
        }

        void Factory_OnUnknownPacket(string text, byte[] buffer)
        {
            if (OnUnknownPacket != null)
                OnUnknownPacket(text, buffer);
        }

        /// <summary>
        /// Sends a "seed" packet to server if connected.
        /// </summary>
        public void SendSeed()
        {
            // TODO: persist seed.
            Send(new byte[] { 0x00, 0x00, 0x00, 0x01 }, 4);
        }

        /// <summary>
        /// Sends the packet to the server if connected.
        /// </summary>
        /// <param name="Packet">ClientPacket to send</param>
        public void Send(Network.ClientPackets.ClientPacket Packet)
        {
            Network.PacketWriter writer = Packet.GetWriter();
            Send(writer.Packet, writer.Length);
        }

        /// <summary>
        /// Sends data of 0-length to server if connected
        /// </summary>
        /// <param name="data">A array of bytes of length or greater</param>
        /// <param name="length">The number of bytes to send. Expected to be equal to or less than data[] length</param>
        public async void Send(byte[] data, int length)
        {
            if (IsConnected)
                try
                {
                    await _stream.WriteAsync(data, 0, length);
                }
                catch (System.IO.IOException)
                {
                    Disconnect();
                }
        }

        TcpClient _client;
        NetworkStream _stream;

        CancellationTokenSource ctsMessagePump;

        /// <summary>
        /// Begins an async connection to the server. OnConnected or OnConnectFailed will be raised.
        /// </summary>
        /// <param name="server">hostname or IP address</param>
        /// <param name="port">numeric port convertible to an integer</param>
        public async void ConnectToServer(string server, int port, IPEndPoint localEP=null)
        {
            await TaskEx.Run(Disconnect);

            TcpClient newClient = localEP==null ? new TcpClient() : new TcpClient(localEP);
            try
            {
                await newClient.ConnectAsync(server, port);
            }
            catch (SocketException ex)
            {
                if (OnConnectFailed != null) OnConnectFailed(ex.Message);
                return;
            }
            _client = newClient;
            _stream = _client.GetStream();
            EndPoint = newClient.Client.RemoteEndPoint as IPEndPoint;
            if (OnConnected != null) OnConnected(server, port);
            RunAsyncMessagePump();
        }

        async void RunAsyncMessagePump()
        {
            ctsMessagePump = new CancellationTokenSource();

            while (IsConnected && !ctsMessagePump.IsCancellationRequested)
            {
                Network.ServerPackets.ServerPacket packet;
                while ((packet = await TaskEx.FromResult(ReadSinglePacketIfAvailable(_stream))) != null)
                {
                    if (!Handlers.GetHandler(packet).Invoke(packet))
                        if (OnUnhandledPacket != null) OnUnhandledPacket(packet);
                }
 
                // TODO: Should ping in here if connection has been idle for a while.

                try
                {
                    await TaskEx.Delay(PumpSleep, ctsMessagePump.Token);
                }
                catch (TaskCanceledException) { }
                catch (ObjectDisposedException) { }
            }

            if (IsConnected)
                _client.Close();
          
            _client = null;
            _stream = null;
            EndPoint = null;

            if (OnDisconnected != null) OnDisconnected();

            ctsMessagePump = null;
        
        }

        Network.ServerPackets.ServerPacket ReadSinglePacketIfAvailable(NetworkStream stream)
        {
            ReadStream(stream);
            if (Reader.DataAvailable)
            {
                return Factory.RecieveSinglePacketIfAvailable(Reader);
            }
            return null;
        }

        void ReadStream(NetworkStream stream)
        {
            if (stream.DataAvailable && stream.CanRead)
            {
                try
                {
                    Reader.Read(stream);
                }
                catch (System.IO.IOException)
                {
                    Disconnect();
                }
                catch (Network.BufferOverflowException ex)
                {
                    if (OnReadBufferOverflow != null)
                        OnReadBufferOverflow(ex.Message + ": clearing buffer");
                    Reader.ClearIncoming();
                }
            }

        }

        async public void Disconnect()
        {
            if (ctsMessagePump != null)
            {
                ctsMessagePump.Cancel();
                while (ctsMessagePump != null)
                    await TaskEx.Delay(1);
            }
            _client = null;
            _stream = null;
            EndPoint = null;
        }

        /// <summary>
        /// True if we believe we are connected to a server
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return _client != null && _client.Connected;
            }
        }

        public void Dispose()
        {
            Disconnect();
            if (_Handlers != null)
                _Handlers.Dispose();
        }
    }
}
