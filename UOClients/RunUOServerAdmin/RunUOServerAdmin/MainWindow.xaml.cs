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
using System.ComponentModel;
using System.Windows.Media.Animation;

namespace RunUOServerAdmin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool _connected = false;
        public bool isConnected
        {
            get { return _connected; }
            private set
            {
                _connected = value;
                OnPropertyChanged("ConnectedStatus");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MyConnectionControl.OnConnection += MyConnectionControl_OnConnection;
            MyConnectionControl.OnDisconnection += MyConnectionControl_OnDisconnection;
            MyConnectionControl.OnMessage += MyConnectionControl_OnMessage;
            MyConnectionControl.OnConsole += MyConnectionControl_OnConsole;
            MyConnectionControl.OnServerInfo += new ServerConnection.ServerInfoEvent(MyConnectionControl_OnServerInfo);

            MyConnectionControl.OnSearchResults += new ServerConnection.SearchResultsEvent(MyConnectionControl_OnSearchResults);

            AccountSearch.IsEnabled = false;

            ServerInfoBox.MaxWidth = 0;

        }

        void MyConnectionControl_OnSearchResults(IEnumerable<UoClientSDK.Network.ServerPackets.AccountResult> Results)
        {
            AccountSearch.OnResults(Results);
        }

        void MyConnectionControl_OnServerInfo(ServerConnection.ServerInfo info)
        {
            ServerConnection.ServerInfo prev = lastInfo;
            lastInfo = info;

            if (info.OperatingSystem != prev.OperatingSystem) OnPropertyChanged("siOperatingSystem");
            if (info.DotNetVersion != prev.DotNetVersion) OnPropertyChanged("siDotNetVersion");
            if (info.Active != prev.Active)
            {
                OnPropertyChanged("siActive");
                OnPropertyChanged("siTotal");
            }
            if (info.Banned != prev.Banned) 
            {
                OnPropertyChanged("siBanned");
                OnPropertyChanged("siTotal");
            }
            if (info.Firewalled != prev.Firewalled) OnPropertyChanged("siFirewalled");
            if (info.Clients != prev.Clients)
            {
                OnPropertyChanged("siClients");
                OnPropertyChanged("ClientsStatus");
            }
            if (info.UpSecs != prev.UpSecs)
            {
                OnPropertyChanged("siUptime");
                OnPropertyChanged("UptimeStatus");
            }
            if (info.Memory != prev.Memory)
            {
                OnPropertyChanged("siMemory");
                OnPropertyChanged("MemoryStatus");
            }
            if (info.MobileScripts != prev.MobileScripts) OnPropertyChanged("siMobileScripts");
            if (info.Mobiles != prev.Mobiles)
            {
                OnPropertyChanged("siMobiles");
                OnPropertyChanged("MobilesStatus");
            }
            if (info.Items != prev.Items)
            {
                OnPropertyChanged("siItems");
                OnPropertyChanged("ItemsStatus");
            }
            if (info.ItemScripts != prev.ItemScripts) OnPropertyChanged("siItemScripts");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        ServerConnection.ServerInfo lastInfo;
        public string ClientsStatus { get { return string.Format("Clients:{0}", lastInfo.Clients); } }
        public string UptimeStatus { get { return string.Format("Up:{0}", TimeSpan.FromSeconds(lastInfo.UpSecs).ToString()); } }
        public string MemoryStatus { get { return string.Format("Memory: {0}", lastInfo.Memory); } }
        public string ItemsStatus { get { return string.Format("Items:{0}", lastInfo.Items); } }
        public string MobilesStatus { get { return string.Format("Mobiles:{0}", lastInfo.Mobiles); } }

        public string siEndPoint { get { return MyConnectionControl.EndPoint == null ? "Disconnected" : MyConnectionControl.EndPoint.ToString(); } }
        public string siOperatingSystem { get { return lastInfo.OperatingSystem; } }
        public string siDotNetVersion { get { return lastInfo.DotNetVersion; } }
        public string siActive { get { return lastInfo.Active.ToString(); } }
        public string siBanned { get { return lastInfo.Banned.ToString(); } }
        public string siTotal { get { return (lastInfo.Active + lastInfo.Banned).ToString(); } }
        public string siFirewalled { get { return lastInfo.Firewalled.ToString(); } }
        public string siClients { get { return lastInfo.Clients.ToString(); } }
        public string siMobiles { get { return lastInfo.Mobiles.ToString(); } }
        public string siMobileScripts { get { return lastInfo.MobileScripts.ToString(); } }
        public string siItems { get { return lastInfo.Items.ToString(); } }
        public string siItemScripts { get { return lastInfo.ItemScripts.ToString(); } }
        public string siUptime { get { return TimeSpan.FromSeconds(lastInfo.UpSecs).ToString(); } }
        public string siMemory { get { return lastInfo.Memory.ToString(); } }

        public string ConnectedStatus { get { return isConnected ? string.Format("Connected: {0}", (object)MyConnectionControl.EndPoint ?? "Unknown") : "Disconnected"; } }

        void MyConnectionControl_OnConnection(string server, int port)
        {
            ConsoleTextBox.Clear();
            ShowTabParent(ConsoleTextBox);
            isConnected = true;

            AccountSearch.IsEnabled = true;

            OnPropertyChanged("siEndPoint");

            ShowInfoButton.Visibility = System.Windows.Visibility.Visible;
            ShowInfo();
        }

        void MyConnectionControl_OnDisconnection()
        {
            isConnected = false;

            AccountSearch.IsEnabled = false;

            OnPropertyChanged("siEndPoint");

            ShowInfoButton.Visibility = System.Windows.Visibility.Hidden;
            HideInfo();

        }

        void MyConnectionControl_OnConsole(string MessageText)
        {
            ConsoleTextBox.Append(MessageText);
            ConsoleTextBox.ScrollToEnd();
        }

        void MyConnectionControl_OnMessage(string MessageText)
        {
            ConnectionTextBox.Append(MessageText + '\n');
        }

        private void Label_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            DoubleAnimation myAnimation = (DoubleAnimation)this.Resources["InfoChanged"];
            ((Label)sender).BeginAnimation(Frame.OpacityProperty, myAnimation, HandoffBehavior.SnapshotAndReplace);
        }

        void ConsoleTextBox_OnTextChanged(object sender, EventArgs e)
        {
            ShowTabParent(sender);
            TextBox box = sender as TextBox;
            if (box != null)
            {
                TabItem tab = FindTabParent(sender);
                if (tab != null && !tab.IsSelected && box.Text != string.Empty)
                    tab.Foreground = Brushes.Red;
            }
        }

        void ConsoleTab_GotFocus(object sender, RoutedEventArgs e)
        {
            TabItem tab = sender as TabItem;
            if (tab != null)
            {
                tab.Foreground = Brushes.Black;
            }
        }

        private void ButtonCloseTab_Click(object sender, RoutedEventArgs e)
        {
            HideTabParent(sender);
        }

        void ShowTabParent(object sender)
        {
            TabItem tab = FindTabParent(sender);
            if (tab != null) tab.Visibility = System.Windows.Visibility.Visible;
        }

        void HideTabParent(object sender)
        {
            TabItem tab = FindTabParent(sender);
            if (tab != null) tab.Visibility = System.Windows.Visibility.Hidden;
        }

        TabItem FindTabParent(object frameworkelement)
        {
            FrameworkElement parent = frameworkelement as FrameworkElement;
            while (parent != null && !(parent is TabItem)) parent = (parent.Parent ?? parent.TemplatedParent) as FrameworkElement;
            if (parent is TabItem)
                return (TabItem)parent;
            return null;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            MyConnectionControl.SaveSettings();
            base.OnClosing(e);
        }

        private void AccountSearch_OnAccountSearch(ServerConnection.AccountSearchArgs args)
        {
            MyConnectionControl.AccountSearch(args);
        }

        private void AccountSearch_OnAccountUpdate(UoClientSDK.Network.ClientPackets.AdminUpdateAccount.UpdateAccountArg args)
        {
            MyConnectionControl.AccountUpdate(args);
        }

        private void ShowInfoButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ptrExpandColapse.Content = string.Empty;
            if (ServerInfoBox.MaxWidth > 0)
            {
                HideInfo();
            }
            else
            {
                ShowInfo();
            }
        }

        private void ShowInfo()
        {
            if (ServerInfoBox.ActualWidth <= 0)
            {
                DoubleAnimation myAnimation = (DoubleAnimation)this.Resources["ShowServerInfo"];
                ServerInfoBox.BeginAnimation(Frame.MaxWidthProperty, myAnimation, HandoffBehavior.SnapshotAndReplace);
                myAnimation.Completed += myAnimation_Completed;
            }
        }

        void myAnimation_Completed(object sender, EventArgs e)
        {
            if (ServerInfoBox.MaxWidth > 0)
            {
                ptrExpandColapse.Content = ">";
            }
            else
            {
                ptrExpandColapse.Content = "<";
            }
        }

        private void HideInfo()
        {
            if (ServerInfoBox.ActualWidth > 0)
            {
                DoubleAnimation myAnimation = (DoubleAnimation)this.Resources["HideServerInfo"];
                myAnimation.From = ServerInfoBox.ActualWidth;
                ServerInfoBox.BeginAnimation(Frame.MaxWidthProperty, myAnimation, HandoffBehavior.SnapshotAndReplace);
                myAnimation.Completed += myAnimation_Completed;
            }
        }
    }
}
