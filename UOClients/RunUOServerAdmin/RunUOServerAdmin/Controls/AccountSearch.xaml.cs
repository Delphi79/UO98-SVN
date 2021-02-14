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
using UoClientSDK.Network.ClientPackets;
using UoClientSDK.Network.ServerPackets;
using System.Net;
using System.Windows.Media.Animation;

namespace RunUOServerAdmin
{
    public delegate void AccountSearchRequest(ServerConnection.AccountSearchArgs args);
    public delegate void AccountUpdateEvent(AdminUpdateAccount.UpdateAccountArg args);

    /// <summary>
    /// Interaction logic for AccountSearch.xaml
    /// </summary>
    public partial class AccountSearch : UserControl
    {
        public event AccountSearchRequest OnAccountSearch;
        public event AccountUpdateEvent OnAccountUpdate;

        public AccountSearch()
        {
            InitializeComponent();
            ClearForm();
        }

        List<Hyperlink> UsernameLinks = new List<Hyperlink>();
        List<Hyperlink> CopyLinks = new List<Hyperlink>();
        Dictionary<string, AccountResult> Accounts = new Dictionary<string, AccountResult>();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClearResults();
            Search();
        }

        private void FormSave_Click(object sender, RoutedEventArgs e)
        {
            AccountResult account;
            if (TryGetAccount(FormUsername.Content as string, out account))
            {
                bool OKpass = false;
                bool OKaccess = false;

                string newPass = FormPassword.Text;
                if (!string.IsNullOrWhiteSpace(newPass))
                    OKpass = true;

                UoClientSDK.AccessLevel newAccessLevel;
                if (Enum.TryParse(FormAccessLevel.Text, out newAccessLevel))
                    OKaccess = true;

                bool newBanned = FormBanned.IsChecked ?? false;

                if (!OKpass || !OKaccess)
                {
                    string message = "Please fix the following errors:";
                    if (!OKpass) message += "\nPassword is required.";
                    if (!OKaccess) message += "\nAccess Level is not valid.";
                    MessageBox.Show(message, "Save Account Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    bool passChanged = newPass != account.Password;
                    bool accessChanged = newAccessLevel != account.AccessLevel;
                    bool bannedChanged = newBanned != account.Banned;

                    if (passChanged || accessChanged || bannedChanged)
                    {
                        string message = string.Format("The following parameters for account {0} will be updated:", account.Username);

                        if (passChanged) message += string.Format("\n\tPassword       -> {0}", newPass);
                        if (accessChanged) message += string.Format("\n\tAccess Level -> {0}", newAccessLevel.ToString());
                        if (bannedChanged) message += string.Format("\n\tAccount will be {0}", newBanned ? "Banned" : "Unbanned");

                        if (MessageBox.Show(message, "Confirm account update", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.OK)
                        {
                            if (OnAccountUpdate != null)
                            {
                                AdminUpdateAccount.UpdateAccountArg arg = new AdminUpdateAccount.UpdateAccountArg()
                                {
                                    Username = account.Username,
                                    Password = newPass,
                                    AccessLevel = newAccessLevel,
                                    Banned = newBanned,
                                    AddressRestrictions = account.AddressRestrictions,
                                };
                                OnAccountUpdate(arg);

                                account.Password = "(hidden)";
                                account.AccessLevel = newAccessLevel;
                                account.Banned = newBanned;
                                Accounts[account.Username] = account;
                                EnableFormEdit = false;

                                if (bannedChanged)
                                    OnResults(Accounts.Values.ToArray());
                            }
                            else
                                MessageBox.Show("Save OK, but not implemented.");
                        }
                    }
                    else
                        MessageBox.Show("Nothing changed!");
                }
            }
        }

        public void OnResults(IEnumerable<AccountResult> accounts)
        {
            ClearResults();

            Accounts.Clear();
            foreach(AccountResult account in accounts)
                Accounts[account.Username] = account;

            foreach (AccountResult result in accounts)
            {
                var copy = MakeCopyImageLink(result.Username);

                string displayName = result.Username;
                if (result.Banned) displayName += " (banned)";
                var link = new Hyperlink(new Run(displayName));
                link.TargetName = result.Username;
                link.Click += new RoutedEventHandler(AccountLink_Click);
                
                ResultsBlock.Inlines.Add(copy);
                ResultsBlock.Inlines.Add(new Run(" ")); 
                ResultsBlock.Inlines.Add(link);
                ResultsBlock.Inlines.Add(new Run("\n")); 

                CopyLinks.Add(copy);
                UsernameLinks.Add(link);
            }
        }

        Hyperlink MakeCopyImageLink(string texttoCopy)
        {
            Image copyimage = new Image();
            copyimage.Width = 15;
            copyimage.Height = 13;
            copyimage.Source = (BitmapImage)this.Resources["ClipboardIconImage"];
            var copy = new Hyperlink(new InlineUIContainer(copyimage));
            copy.TextDecorations = null;
            
            copy.ToolTip = "Copy to clipboard";
            copy.TargetName = texttoCopy;
            copy.Click += new RoutedEventHandler(copy_Click);
            copy.IsEnabled = true;

            return copy;
        }

        void copy_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;
            if (link != null)
            {
                InlineUIContainer cont = link.Inlines.FirstInline as InlineUIContainer;
                if (cont != null && cont.Child is Image)
                {
                    DoubleAnimation myAnimation = (DoubleAnimation)this.Resources["CopyClickAni"];
                    cont.Child.BeginAnimation(Image.OpacityProperty, myAnimation, HandoffBehavior.SnapshotAndReplace);
                }
                
                Clipboard.SetData(DataFormats.Text, link.TargetName);
            }
        }

        void ClearResults()
        {
            Accounts.Clear();
            UsernameLinks.Clear();
            CopyLinks.Clear();
            ResultsBlock.Text = string.Empty;
        }

        void AccountLink_Click(object sender, RoutedEventArgs e)
        {
            ResetUsernameLinks();
            Hyperlink link = sender as Hyperlink;
            if (link != null)
            {
                link.Foreground = Brushes.Red;
                DisplayAccount=link.TargetName;
            }
            else
                DisplayAccount = null;
        }

        private string _DisplayAccount;
        string DisplayAccount
        {
            get { return _DisplayAccount; }
            set
            {
                AccountResult account;
                if (TryGetAccount(value, out account))
                {
                    _DisplayAccount = value;
                    FillForm(account);
                }
                else
                {
                    _DisplayAccount = null;
                    ClearForm();
                }
            }
        }

        bool TryGetAccount(string accountname, out AccountResult account)
        {
            if (string.IsNullOrEmpty(accountname) || !Accounts.ContainsKey(accountname))
            {
                account = new AccountResult();
                return false;
            }
            else
            {
                account = Accounts[accountname];
                return true;
            }
        }

        void ClearForm()
        {
            FormEnableEdit.IsEnabled = false;
            EnableFormEdit = false;

            FormUsername.Content = string.Empty;
            FormPassword.Text = string.Empty;
            FormLastLogin.Content = string.Empty;
            
            FormAccessLevel.Text = string.Empty;
            FormAccessLevel.Items.Clear();

            FormBanned.IsChecked = false;

            AddressesBlock.Text = RestrictionsBlock.Text = string.Empty;
        }

        void FillForm(AccountResult account)
        {
            FormEnableEdit.IsEnabled = true;
            EnableFormEdit = false;

            FormUsername.Content = account.Username;
            FormPassword.Text = account.Password;
            FormLastLogin.Content = account.LastLogin.ToString();

            FormAccessLevel.Items.Clear();
            var Levels = Enum.GetNames(typeof(UoClientSDK.AccessLevel)).Select(level => new ComboBoxItem() { Content = level.ToString() });
            foreach (var level in Levels)
                FormAccessLevel.Items.Add(level);
            FormAccessLevel.Text = account.AccessLevel.ToString();
            
            FormBanned.IsChecked = account.Banned;

            FillAddresses(AddressesBlock, account.Addresses);
            FillAddresses(RestrictionsBlock, account.AddressRestrictions);
        }

        private void FormEnableEdit_Checked(object sender, RoutedEventArgs e)
        {
            EnableFormEdit = true;
        }

        private void FormEnableEdit_Unchecked(object sender, RoutedEventArgs e)
        {
            EnableFormEdit = false;
        }

        private void FormPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            FormPassword.Text = string.Empty;
        }

        private void FormPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(FormPassword.Text))
            {
                AccountResult account;
                if (TryGetAccount(FormUsername.Content as string, out account))
                    FormPassword.Text = account.Password;
            }
        }

        private void FormReset_Click(object sender, RoutedEventArgs e)
        {
            DisplayAccount = FormUsername.Content as string;
        }

        private bool _EnableFormEdit;
        bool EnableFormEdit
        {
            get { return _EnableFormEdit; }
            set
            {
                _EnableFormEdit = value;

                if (FormEnableEdit.IsChecked != value)
                    FormEnableEdit.IsChecked = value;

                FormPassword.IsEnabled = value;
                FormAccessLevel.IsEnabled = value;
                FormBanned.IsEnabled = value;

                FormReset.IsEnabled = value;
                FormSave.IsEnabled = value;
            }
        }


        void FillAddresses(TextBlock textblock, IEnumerable<string> addresses)
        {
            textblock.Text = string.Empty;
            foreach (string address in addresses)
            {
                var copy = MakeCopyImageLink(address);
                var link = new Hyperlink(new Run(address));
                link.TargetName = address;
                link.Click += new RoutedEventHandler(AddressLink_Click);

                textblock.Inlines.Add(copy);
                textblock.Inlines.Add(new Run(" "));
                textblock.Inlines.Add(link);
                textblock.Inlines.Add(new Run("\n"));
            }
        }

        void AddressLink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;
            if (link != null)
            {
                ClearResults();
                SearchTerm.Text = link.TargetName;
                ByIP.IsChecked = true;
                Search();
            }
        }

        Dictionary<ServerConnection.AccountSearchArgs, Tuple<DateTime, IEnumerable<AccountResult>>> ResultCache =
            new Dictionary<ServerConnection.AccountSearchArgs, Tuple<DateTime, IEnumerable<AccountResult>>>();
        TimeSpan CacheDuration = TimeSpan.FromMinutes(5.0);

        void Search()
        {
            if (OnAccountSearch != null)
            {
                ServerConnection.AccountSearchArgs args = new ServerConnection.AccountSearchArgs();
                args.SearchType = (bool)(ByIP.IsChecked ?? false) ? AdminAccountSearch.AcctSearchType.IP : AdminAccountSearch.AcctSearchType.Username;
                args.SearchTerm = SearchTerm.Text;

                Throttle();

                OnAccountSearch(args);
            }
        }

        async void Throttle()
        {
            IsEnabled = false;

            Cursor = Cursors.Wait;
            Mouse.OverrideCursor = Cursor;

            await System.Threading.Tasks.TaskEx.Delay(2000);
            IsEnabled = true;
            ClearProperty(this, "Cursor");

            Mouse.OverrideCursor = null;

            foreach (Hyperlink link in UsernameLinks)
                link.IsEnabled = true;
            foreach (Hyperlink link in CopyLinks)
                link.IsEnabled = true;
        }

        void ResetUsernameLinks()
        {
            foreach (Hyperlink link in UsernameLinks)
               ClearProperty(link, "Foreground");
        }

        void ClearProperty(DependencyObject element, string propName)
        {
            DependencyProperty prop = FindProperty(element, propName);
            if (prop != null)
                element.ClearValue(prop);
        }

        DependencyProperty FindProperty(DependencyObject element, string propName)
        {
            LocalValueEnumerator lve = element.GetLocalValueEnumerator();
            while (lve.MoveNext())
                if (lve.Current.Property.Name == propName)
                    return lve.Current.Property;

            return null;
        }

        private void SearchTerm_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box=sender as TextBox;
            if (box != null && box.Text.Length>1)
            {
                bool isip = true;
                for (int i = 0; i < box.Text.Length; i++)
                {
                    if (char.IsLetter(box.Text[i]))
                    {
                        ByName.IsChecked = true;
                        isip=false;
                        break;
                    }
                    isip &= char.IsDigit(box.Text[i]) || box.Text[i]=='.'; 
                }
                if (isip)
                    ByIP.IsChecked = true;
            }
        }
    }
}
