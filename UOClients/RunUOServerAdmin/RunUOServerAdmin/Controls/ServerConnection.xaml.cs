using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UoClientSDK;
using UoClientSDK.Network.ClientPackets;
using UoClientSDK.Network.ServerPackets;

namespace RunUOServerAdmin
{
    /// <summary>
    /// Interaction logic for ServerConnection.xaml
    /// </summary>
    public partial class ServerConnection : UserControl
    {
        Dictionary<string, int> LastPorts = new Dictionary<string, int>();
        Dictionary<string, string> LastUsernames = new Dictionary<string, string>();

        public delegate void TextMessageEvent(string MessageText);
        public delegate void ServerConnectionEvent(string server, int port);
        public delegate void ServerInfoEvent(ServerInfo info);
        public delegate void SearchResultsEvent(IEnumerable<AccountResult> Results);
        public delegate void EmptyEvent();

        public struct ServerInfo
        {
            public int Active;
            public int Banned;
            public int Firewalled;
            public int Clients;
            public int Mobiles;
            public int MobileScripts;
            public int Items;
            public int ItemScripts;
            public uint UpSecs;
            public uint Memory;
            public string DotNetVersion;
            public string OperatingSystem;
        }

        public event TextMessageEvent OnMessage;
        public event TextMessageEvent OnConsole;

        public event ServerInfoEvent OnServerInfo;
        public event SearchResultsEvent OnSearchResults;

        public event ServerConnectionEvent OnConnection;
        public event EmptyEvent OnDisconnection;

        void InvokeOnMessage(string message)
        {
            if (OnMessage != null)
                OnMessage(message);
        }
        void InvokeOnMessage(string message, params object[] args)
        {
            if (OnMessage != null)
                OnMessage(string.Format(message, args));
        }
        void InvokeOnConsole(string message)
        {
            if (OnConsole != null)
                OnConsole(message);
        }
        void InvokeOnConnection(string server, int port)
        {
            if (OnConnection != null)
                OnConnection(server, port);
        }
        void InvokeOnDisconnection()
        {
            if (OnDisconnection != null)
                OnDisconnection();
        }
        void InvokeOnServerInfo(ServerInfo info)
        {
            if (OnServerInfo != null)
                OnServerInfo(info);
        }
        void InvokeOnSearchResults(IEnumerable<AccountResult> results)
        {
            if (OnSearchResults!= null)
                OnSearchResults(results);
        }

        UoClientSDK.Client UOClient;

        public ServerConnection()
        {
            InitializeComponent();

            LoadSettings();
            
            InitializeUOClient();

            cmbServerName.SelectionChanged += new SelectionChangedEventHandler(cmbServerName_SelectionChanged);
        }

        void cmbServerName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox)
            {
                ComboBoxItem item = ((ComboBox)sender).SelectedItem as ComboBoxItem;
                if (item != null)
                {
                    string server = item.Content.ToString();
                    if (LastPorts.ContainsKey(server)) txtPort.Text = LastPorts[server].ToString();
                    if (LastUsernames.ContainsKey(server))
                    {
                        txtPass.Focus();
                        if (txtUser.Text != LastUsernames[server])
                        {
                            txtUser.Text = LastUsernames[server];
                            txtPass.Password = string.Empty;
                        }
                        else
                            txtPass.SelectAll();
                    }
                }
            }
        }

        void InitializeUOClient()
        {
            if (UOClient != null)
                UOClient.Dispose();

            UOClient = new UoClientSDK.Client(ClientVersion.vAdminClient);
            UOClient.OnConnected += new UoClientSDK.OnConnectedEvent(UOClient_OnConnected);
            UOClient.OnDisconnected += new UoClientSDK.EventHandler(UOClient_OnDisconnected);
            UOClient.OnConnectFailed += new UoClientSDK.OptionalStringEvent(UOClient_OnConnectFailed);
            UOClient.OnUnhandledPacket += new UoClientSDK.PacketEventHandler(UOClient_OnPacket);
            UOClient.OnUnknownPacket += new UnknownPacketEvent(UOClient_OnUnknownPacket);
            UOClient.OnReadBufferOverflow += new OptionalStringEvent(UOClient_OnReadBufferOverflow);

            UOClient.Handlers.GetHandler<ServerInfoPacket>().OnPacket += new OnPacketEventHandler<ServerInfoPacket>(ServerConnection_OnPacket);
            UOClient.Handlers.GetHandler<ConsoleDataPacket>().OnPacket += new OnPacketEventHandler<ConsoleDataPacket>(ServerConnection_OnPacket);
            UOClient.Handlers.GetHandler<LoginResponsePacket>().OnPacket += new OnPacketEventHandler<LoginResponsePacket>(ServerConnection_OnPacket);
            UOClient.Handlers.GetHandler<LoginDeniedPacket>().OnPacket += new OnPacketEventHandler<LoginDeniedPacket>(ServerConnection_OnPacket);
            UOClient.Handlers.GetHandler<AccountSearchResults>().OnPacket += new OnPacketEventHandler<AccountSearchResults>(ServerConnection_OnPacket);
            UOClient.Handlers.GetHandler<AdminMessageBox>().OnPacket += new OnPacketEventHandler<AdminMessageBox>(ServerConnection_OnPacket);
        }

        void UOClient_OnReadBufferOverflow(string text = null)
        {
            InvokeOnMessage("Read buffer overflow: {0}\n", text);
        }

        void UOClient_OnUnknownPacket(string message, byte[] buffer)
        {
            InvokeOnMessage("Unknown Packet: {0}\n", message);
        }

        void ServerConnection_OnPacket(AdminMessageBox packet)
        {
            MessageBox.Show(packet.Message, packet.Caption);
        }

        void ServerConnection_OnPacket(AccountSearchResults packet)
        {
            InvokeOnSearchResults(packet.Accounts);
        }

        void ServerConnection_OnPacket(LoginDeniedPacket packet)
        {
            InvokeOnMessage("Login denied");
            UOClient.Disconnect();
        }

        void ServerConnection_OnPacket(LoginResponsePacket packet)
        {
            LoginResponse resp = ((LoginResponsePacket)packet).LoginResponse;
            InvokeOnMessage("Admin Login {1}: {0}", resp, resp == LoginResponse.OK ? "Success" : "Failed");
            if (resp != LoginResponse.OK)
                UOClient.Disconnect();
            else
                RunGetServerInfo();
        }

        void ServerConnection_OnPacket(ConsoleDataPacket packet)
        {
            ConsoleDataPacket data = (ConsoleDataPacket)packet;
            InvokeOnConsole(data.Text);
        }

        void ServerConnection_OnPacket(ServerInfoPacket packet)
        {
            ServerInfoPacket data = (ServerInfoPacket)packet;
            ServerInfo info = new ServerInfo()
            {
                Active = packet.Active,
                Banned = packet.Banned,
                Clients = packet.Clients,
                Firewalled = packet.Firewalled,
                Items = packet.Items,
                ItemScripts = packet.ItemScripts,
                Memory = packet.Memory,
                Mobiles = packet.Mobiles,
                MobileScripts = packet.MobileScripts,
                DotNetVersion = packet.DotNetVersion,
                OperatingSystem = packet.OperatingSystem,
                UpSecs = packet.UpSecs,
            };

            InvokeOnServerInfo(info);

        }

        void UOClient_OnPacket(ServerPacket packet)
        {
            InvokeOnMessage("Received unhandled packet: {0} {1}", packet.PacketID, packet.GetType());
        }

        void UOClient_OnConnected(string server, int port)
        {
            LastPorts[server] = port;
            LastUsernames[server] = txtUser.Text;

            InvokeOnMessage("Connected: {0}:{1}", server, port);
            Status = ConnectionStatus.Connected;

            InvokeOnConnection(server, port);

            UOClient.SendSeed();
            AdminLogin login = new AdminLogin(ClientVersion.vAdminClient, txtUser.Text, txtPass.Password);
            UOClient.Send(login);
            AddToServerListIfNotExists(server);
            SaveSettings();
        }

        bool GetInfoRunning = false;
        async void RunGetServerInfo()
        {
            if (UOClient != null && Status == ConnectionStatus.Connected && !GetInfoRunning)
            {
                GetInfoRunning = true;
                UOClient.Send(new AdminServerInfoRequest(ClientVersion.vAdminClient));
                await TaskEx.Delay(5000);
                GetInfoRunning = false;
                RunGetServerInfo();
            }
        }

        void UOClient_OnDisconnected()
        {
            InvokeOnMessage("Disconnected.");
            InvokeOnDisconnection();
            Status = ConnectionStatus.Disconnected;
        }

        void UOClient_OnConnectFailed(string reason = null)
        {
            InvokeOnMessage("Connect Failed: {0}", reason ?? "Could not connect to host.");
            Status = ConnectionStatus.Disconnected;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int port;
            bool portOk = int.TryParse(txtPort.Text, out port);
            bool serverOk = !string.IsNullOrWhiteSpace(cmbServerName.Text);
            if (!serverOk || !portOk)
            {
                if (!serverOk) InvokeOnMessage("Server name is empty!");
                if (!portOk) InvokeOnMessage("Port is Invalid!");
            }
            else
            {
                Status = ConnectionStatus.Busy;
                UOClient.ConnectToServer(cmbServerName.Text, port);
            }
        }

        private void ConnectStatus_Click(object sender, RoutedEventArgs e)
        {
            if (UOClient != null && UOClient.IsConnected)
            {
                Status = ConnectionStatus.Busy;
                UOClient.Disconnect();
            }
        }

        public IPEndPoint EndPoint
        {
            get
            {
                if (UOClient == null) return null;
                return UOClient.EndPoint;
            }
        }

        public ConnectionStatus Status
        {
            get
            {
                if (!ConnectStatus.IsChecked == null) return ConnectionStatus.Busy;
                if (ConnectStatus.IsChecked == true) return ConnectionStatus.Disconnected;
                if (ConnectStatus.IsChecked == false) return ConnectionStatus.Connected;
                return ConnectionStatus.Busy;
            }
            set
            {
                switch (value)
                {
                    case ConnectionStatus.Disconnected:
                        ConnectStatus.IsChecked = true;
                        ConnectStatus.IsEnabled = false;
                        ConnectStatus.ToolTip = "Disconnected";
                        break;
                    case ConnectionStatus.Connected:
                        ConnectStatus.IsChecked = false;
                        ConnectStatus.IsEnabled = true;
                        ConnectStatus.ToolTip = "Click to disconnect";
                        break;
                    default:
                        ConnectStatus.IsChecked = null;
                        ConnectStatus.IsEnabled = false;
                        ConnectStatus.ToolTip = "Busy";
                        break;
                }
            }

        }

        public void SaveSettings()
        {
            for(int i=0;i<cmbServerName.Items.Count;i++)
            {
                ComboBoxItem item = cmbServerName.Items[i] as ComboBoxItem;
                if (item != null)
                {
                    string server = item.Content.ToString();
                    App.SaveRegistryValue(string.Format("server{0}", i), server);
                    int port;
                    if (LastPorts.TryGetValue(server, out port))
                        App.SaveRegistryValue(string.Format("port{0}", i), port.ToString());
                    string user;
                    if (LastUsernames.TryGetValue(server, out user))
                        App.SaveRegistryValue(string.Format("user{0}", i), user.ToString());
                }
            }
        }

        void LoadSettings()
        {
            cmbServerName.Items.Clear();
            
            string server;
            for (int i = 0; ; i++)
            {
                server = App.LoadRegistryValue(string.Format("server{0}", i));
                if (server == null) break;

                AddToServerListIfNotExists(server, false);

                string portstring = App.LoadRegistryValue(string.Format("port{0}", i));
                if (portstring != null)
                {
                    int port;
                    if (int.TryParse(portstring, out port)) LastPorts[server] = port;
                }
                string user = App.LoadRegistryValue(string.Format("user{0}", i));
                if (user != null) LastUsernames[server] = user;

                if (i == 0)
                {
                    cmbServerName.Text = server;
                    if (LastPorts.ContainsKey(server)) txtPort.Text = LastPorts[server].ToString();
                    if (LastUsernames.ContainsKey(server))
                    {
                        txtUser.Text = LastUsernames[server];
                        txtPass.Focus();
                    }
                }
            }
        }

        void AddToServerListIfNotExists(string server, bool moveToTop=true)
        {
            for (int i = 0; i < cmbServerName.Items.Count; i++)
            {
                ComboBoxItem item = cmbServerName.Items[i] as ComboBoxItem;
                if (item.Content.ToString().Trim().Equals(server.Trim(), StringComparison.InvariantCultureIgnoreCase))
                {
                    if (moveToTop)
                    {
                        cmbServerName.Items.Remove(item);
                        cmbServerName.Items.Insert(0, item);
                        cmbServerName.Text = item.Content.ToString();
                    }
                    return;
                }
            }

            ComboBoxItem newitem = new ComboBoxItem() { Content = server.Trim() };
            if (moveToTop)
            {
                cmbServerName.Items.Insert(0, newitem);
                cmbServerName.Text = newitem.Content.ToString();
            }
            else
                cmbServerName.Items.Add(newitem);
        }

        public enum ConnectionStatus
        {
            Disconnected,
            Connected,
            Busy,
        }

        public struct AccountSearchArgs
        {
            public AdminAccountSearch.AcctSearchType SearchType;
            public string SearchTerm;
        }

        internal void AccountSearch(AccountSearchArgs args)
        {
            if (UOClient != null && Status == ConnectionStatus.Connected)
                UOClient.Send(new AdminAccountSearch(ClientVersion.vAdminClient, args.SearchType, args.SearchTerm));
            else
                OnSearchResults(new List<AccountResult>());
        }

        internal void AccountUpdate(AdminUpdateAccount.UpdateAccountArg arg)
        {
            if (UOClient != null && Status == ConnectionStatus.Connected)
                UOClient.Send(new AdminUpdateAccount(ClientVersion.vAdminClient, arg));
        }

    }
}
