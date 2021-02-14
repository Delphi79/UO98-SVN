using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace UoClientSDK
{
    /// <summary>
    /// A client for performing polling of shards using the UOGateway protocol.
    /// </summary>
    /// <remarks>The consumer of this class should ignore <see cref="Client.OnUnhandledPacket">OnUnhandledPacket</see></remarks>
    public class ShardPoller : Client
    {
        /// <summary>The maximum amount of time to wait for a response from the shard after sending the RequestUOGStatus packet</summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>Invoked when the shard sends invalid data, or when the <see cref="Timeout">Timeout</see> is reached while waiting for data.</summary>
        public event EventHandler OnInvalidResponse;

        /// <summary>Invoked when shard name is received. This data element is not part of <see cref="PollResult">PollResult</see></summary>
        public event OptionalStringEvent OnShardName;

        public delegate void PollResultEvent(PollResult result);
        /// <summary>Raised when a response is received from the server. 
        /// In the case of an invalid (un-parseable) response,<see cref="OnInvalidResponse">OnInvalidResponse</see> will be raised first, and all result fields will be zero.</summary>
        public event PollResultEvent OnPollResult;

        /// <summary>This structure holds the <see cref="OnPollResult">result of a shard poll</see></summary>
        public struct PollResult
        {
            public static PollResult OfflineResult()
            {
                PollResult result = new PollResult();
                result.PollTime = DateTime.Now;
                result.isOnline = false;
                return result;
            }

            public PollResult(int clients, TimeSpan uptime, int items, int chars, long memory,int ver)
            {
                PollTime = DateTime.Now;
                isOnline = true;
                Clients = clients;
                Uptime = uptime;
                Items = items;
                Chars = chars;
                Memory = memory;
                Ver = ver;
            }

            public DateTime PollTime;
            public bool isOnline;
            public int Clients;
            public TimeSpan Uptime;
            public int Items;
            public int Chars;
            public long Memory;
            public int Ver;
        }

        /// <summary>Constructs a new instance of ShardPoller with a default response wait timeout of 5 seconds</summary>
        public ShardPoller() : this(TimeSpan.FromSeconds(5.0)) { }

        /// <summary>Constructs a new instance of ShardPoller with a specified response wait timeout</summary>
        public ShardPoller(TimeSpan timeout)
            : base(ClientVersion.vAdminClient) 
        {
            Timeout = timeout;
            base.OnConnected += new OnConnectedEvent(ShardPoller_OnConnected);
            base.OnUnknownPacket += new Network.ServerPackets.UnknownPacketEvent(ShardPoller_OnUnknownPacket);
            Handlers.GetHandler<Network.ServerPackets.UOGStatusCompact>().OnPacket += new Network.ServerPackets.OnPacketEventHandler<Network.ServerPackets.UOGStatusCompact>(ShardPoller_OnPacket);
        }

        public string Server { get; private set; }
        public int Port { get; private set; }

        bool UseCompactInfo = false;
        /// <summary>Initiates the polling of the shard. Will raise OnConnectionFailed or OnDisconnected when complete in addition to <see cref="OnInvalidResponse">OnInvalidResponse</see>, or <see cref="OnPollResult">OnPollResult</see> if applicable.</summary>
        /// <param name="hostname">the hostname string as resolvable host or IPv4 address</param>
        /// <param name="port">The port on which to connect</param>
        /// <param name="RequestCompact">If true the poller will request the UOGStatusCompact packet, this only works if the host supports this packet.</param>
        public void BeginPollShard(string hostname, int port, bool RequestCompact = false, System.Net.IPEndPoint localEP = null)
        {
            Server = hostname;
            Port = port;
            base.ConnectToServer(hostname, port, localEP);
            UseCompactInfo = RequestCompact;
        }

        bool ResponseReceived;
        CancellationTokenSource ctsTimeout;
        async void ShardPoller_OnConnected(string server, int port)
        {
            ctsTimeout = new CancellationTokenSource();

            SendSeed();
            if (UseCompactInfo)
                Send(new Network.ClientPackets.AdminRequestUOGStatusCompact(Version));
            else
                Send(new Network.ClientPackets.AdminRequestUOGStatus(Version));
            ResponseReceived = false;
            try
            {
                await TaskEx.Delay((int)Timeout.TotalMilliseconds, ctsTimeout.Token);
            }
            catch (TaskCanceledException) { }

            if (!ResponseReceived)
                if (OnInvalidResponse != null) OnInvalidResponse();

            Disconnect();
        }

        void StopWaiting()
        {
            if(ctsTimeout != null && !ctsTimeout.IsCancellationRequested)
                ctsTimeout.Cancel();
        }

        void ShardPoller_OnPacket(Network.ServerPackets.UOGStatusCompact packet)
        {
            ResponseReceived = true;

            if (OnPollResult != null)
                OnPollResult(new PollResult(packet.Clients, packet.Age, packet.Items, packet.Mobiles, packet.Memory, 2));

            StopWaiting();
        }

        void ShardPoller_OnUnknownPacket(string text, byte[] buffer)
        {
            ResponseReceived = true;
            ProcessResponseAsString(buffer);
            StopWaiting();
        }

        void ProcessResponseAsString(byte[] buffer)
        {
            int i = 0;
            StringBuilder sb = new StringBuilder(buffer.Length);
            while (i < buffer.Length && buffer[i] != 0)
            {
                char c = (char)buffer[i++];
                if (char.IsControl(c))
                {
                    if (OnInvalidResponse != null) OnInvalidResponse();
                    return;
                }
                sb.Append(c);
            }

            PollResult result = ParseResponse(sb.ToString());

            if (OnPollResult != null)
                OnPollResult(result);
        }

        PollResult ParseResponse(string response)
        {
            string[] elements = response.Split(',');

            if (elements.Length <= 0)
            {
                if (OnInvalidResponse != null) OnInvalidResponse();
                return new PollResult(0, TimeSpan.Zero, 0, 0, 0, 0);
            }

            PollResult result = new PollResult();

            result.PollTime = DateTime.Now;

            string memstring = null;

            foreach (string element in elements)
            {
                string[] keyval = element.Split('=');
                if (keyval.Length == 2)
                {
                    switch (keyval[0].Trim().ToLowerInvariant())
                    {
                        case "name":
                            if (OnShardName != null)
                                OnShardName(keyval[1]);
                            break;
                        case "age":
                            int hours;
                            if (int.TryParse(keyval[1], out hours))
                                result.Uptime = TimeSpan.FromHours(hours);
                            else
                                result.Uptime = TimeSpan.Zero;
                            break;
                        case "clients":
                            if (int.TryParse(keyval[1], out result.Clients))
                                result.Clients--;
                            result.isOnline = true; // A shard is only considered online if clients is reported in the response.
                            break;
                        case "items":
                            int.TryParse(keyval[1], out result.Items);
                            break;
                        case "chars":
                            int.TryParse(keyval[1], out result.Chars);
                            break;
                        case "memory":
                        case "mem":
                            memstring = keyval[1].Trim();
                            while (memstring.Length > 0 && !char.IsLetterOrDigit(memstring[memstring.Length - 1]))
                                memstring = memstring.Remove(memstring.Length - 1);
                            break;
                        case "ver":
                            int.TryParse(keyval[1].Trim(), out result.Ver);
                            break;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(memstring))
                result.Memory = ParseMemoryString(memstring);
            else
                result.Memory = 0;

            return result;

        }

        long ParseMemoryString(string memstring)
        {
            int mult = 1;
            char lastchar = memstring[memstring.Length - 1];
            if (char.IsLetter(lastchar))
            {
                char magnitude = lastchar;
                switch (magnitude)
                {
                    case 'B': mult = 1;
                        break;
                    case 'K': mult = 1024;
                        break;
                    case 'M': mult = 1024 * 1024;
                        break;
                    case 'G': mult = 1024 * 1024 * 1024;
                        break;
                }
                memstring = memstring.Remove(memstring.Length - 1);
            }
            long mem;
            if (long.TryParse(memstring, out mem))
                return mem * mult;
            else
                return 0;
        }

    }
}
