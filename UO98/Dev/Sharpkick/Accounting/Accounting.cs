using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Xml;
using System.IO;
using System.Net;

namespace Sharpkick
{
    /// <summary>
    /// A user account
    /// </summary>
    class Account
    {
        /// <summary>
        /// Construct a new user account, not subject to validation of username or password requirements
        /// </summary>
        /// <param name="id">The account ID</param>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        /// <param name="IPAddress">IPAddress of creating client</param>
        public Account(int id, string username, string password, IPAddress IPAddress)
        {
            Id = id;
            Username = username;
            SetPassword(password);
            Created = DateTime.UtcNow;
            AddIP(IPAddress);
        }

        /// <summary>Account Id, corresponds to the Servers internal account id for this user.</summary>
        public int Id { get; private set; }
        
        /// <summary>The account username</summary>
        public string Username { get; private set; }

        /// <summary>The encoded password string for this account</summary>
        public string PassHash { get; private set; }

        /// <summary>The last time a login was processed</summary>
        public DateTime LastLogin { get; private set; }
        
        /// <summary>The date the account was created</summary>
        public DateTime Created { get; private set; }

        /// <summary>This accounts access level, determines access to God Client and other features</summary>
        public AccountAccessFlags Access { get; set; }

        public bool HasAccess(AccountAccessFlags flag) { return (Access & flag) == flag; } 

        /// <summary>
        /// Check password, and set LastLogin
        /// </summary>
        /// <param name="password">The account password</param>
        /// <param name="IPAddress">IPAddress of creating client</param>
        /// <returns>True if login was successful</returns>
        public bool Login(string password, IPAddress IPAddress)
        {
            if (!VerifyPass(password))
                return false;

            LastLogin=DateTime.UtcNow;

            AddIP(IPAddress);

            return true;
        }

        private List<IPAddress> m_IPAddresses = null;
        public List<IPAddress> IPAddresses { get { return m_IPAddresses ?? (m_IPAddresses = new List<IPAddress>()); } }
        private void AddIP(IPAddress IPAddress)
        {
            if (IPAddress!=null && !IPAddresses.Contains(IPAddress)) IPAddresses.Add(IPAddress);
        }

        private Dictionary<uint, Mobile> m_Mobiles = null;
        private Dictionary<uint, Mobile> Mobiles { get { return m_Mobiles ?? (m_Mobiles = new Dictionary<uint, Mobile>()); } }
        public void VerifyMobile(uint serial)
        {
            Mobile.TouchMobile(serial);
            if (!Mobiles.ContainsKey(serial))
            {
                Mobiles.Add(serial, new Mobile(serial));
                Console.WriteLine("-Mobile {0} added to account {1}.", serial, Username);
            }
            Mobiles[serial].AccountNumber = Id;
        }

        /// <summary>
        /// Check the users password
        /// </summary>
        /// <param name="password">Password to check</param>
        /// <returns>True if the password is correct for this account</returns>
        bool VerifyPass(string password)
        {
            return PassHash == Accounting.HashSHA1(password);
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="oldPass">The users current password for verification</param>
        /// <param name="newPass">The new password. Must pass validation restrictions</param>
        /// <returns>True if successful</returns>
        public bool ChangePassword(string oldPass, string newPass)
        {
            if (!VerifyPass(oldPass) || !Accounting.ValidPassword(newPass)) return false;
            SetPassword(newPass);
            return true;
        }

        /// <summary>
        /// Sets the users password
        /// </summary>
        /// <param name="newPass">The new password</param>
        private void SetPassword(string newPass)
        {
            PassHash = Accounting.HashSHA1(newPass);
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="ele">The XmlElement containing the data for this account</param>
        /// <param name="version">account serialization version</param>
        public Account(XmlElement ele, int version)
        {
            Id = Persistance.Xml.GetXMLInt32(Persistance.Xml.GetText(ele["id"], string.Empty), -1);
            Username = Persistance.Xml.GetText(ele["username"], string.Empty);
            PassHash = Persistance.Xml.GetText(ele["password"], string.Empty);
            Created = Persistance.Xml.GetXMLDateTime(Persistance.Xml.GetText(ele["created"], null), DateTime.UtcNow);

            AccountAccessFlags accessflags;
            if (Enum.TryParse<AccountAccessFlags>(Persistance.Xml.GetText(ele["accessflags"], "0"), out accessflags))
                Access = accessflags;
            else
                Access = 0;
            
            LastLogin = Persistance.Xml.GetXMLDateTime(Persistance.Xml.GetText(ele["lastlogin"], null), DateTime.UtcNow);
            m_IPAddresses = Persistance.Xml.LoadAddressList(ele, "ips");
            m_Mobiles = Mobile.LoadMobileDict(ele, "mobiles") ?? m_Mobiles;
            if (m_Mobiles != null)
                foreach (Mobile m in new List<Mobile>(m_Mobiles.Values))
                    m.AccountNumber = Id;
        }

        public const int AccountSaveVersion=0;
        public void Serialize(XmlTextWriter writer)
        {
            writer.WriteStartElement("account");

            writer.WriteStartElement("id");
            writer.WriteString(Id.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("username");
            writer.WriteString(Username);
            writer.WriteEndElement();

            writer.WriteStartElement("password");
            writer.WriteString(PassHash);
            writer.WriteEndElement();

            writer.WriteStartElement("created");
            writer.WriteString(XmlConvert.ToString(Created, XmlDateTimeSerializationMode.Utc));
            writer.WriteEndElement();

            if (Access > 0)
            {
                writer.WriteStartElement("accessflags");
                writer.WriteString(Access.ToString());
                writer.WriteEndElement();
            }

            writer.WriteStartElement("lastlogin");
            writer.WriteString(XmlConvert.ToString(LastLogin, XmlDateTimeSerializationMode.Utc));
            writer.WriteEndElement();

            if (m_IPAddresses!=null && m_IPAddresses.Count > 0)
            {
                writer.WriteStartElement("ips");

                writer.WriteAttributeString("count", m_IPAddresses.Count.ToString());

                for (int i = 0; i < m_IPAddresses.Count; ++i)
                {
                    writer.WriteStartElement("ip");
                    writer.WriteString(m_IPAddresses[i].ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            if (m_Mobiles != null && m_Mobiles.Count > 0)
            {
                writer.WriteStartElement("mobiles");

                writer.WriteAttributeString("count", m_Mobiles.Count.ToString());

                foreach (Mobile m in m_Mobiles.Values)
                    if (m.AccountNumber == Id)
                    {
                        writer.WriteStartElement("mobile");

                        writer.WriteStartElement("serial");
                        writer.WriteString(m.Serial.ToString());
                        writer.WriteEndElement();

                        if (m.Name != null)
                        {
                            writer.WriteStartElement("name");
                            writer.WriteString(m.Name);
                            writer.WriteEndElement();
                        }
                        if (m.Title != null)
                        {
                            writer.WriteStartElement("title");
                            writer.WriteString(m.Title);
                            writer.WriteEndElement();
                        }
                        if (m.Profile != null)
                        {
                            writer.WriteStartElement("profile");
                            writer.WriteString(m.Profile);
                            writer.WriteEndElement();
                        }
                        if (m.LastSeen > DateTime.MinValue)
                        {
                            writer.WriteStartElement("lastseen");
                            writer.WriteString(XmlConvert.ToString(m.LastSeen, XmlDateTimeSerializationMode.Utc));
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }

                writer.WriteEndElement();
            }


            writer.WriteEndElement(); // </account>
        }
    }

    /// <summary>
    /// The users service. Responsible for account creation, validation, and persistence
    /// </summary>
    static class Accounting
    {
        const int MaxAccountsPerIP = 3;

        public static bool CheckCreateAccount(IPAddress ipaddress)
        {
            int count = 0;
            foreach (Account acct in m_byId.Values)
            {
                if (acct.IPAddresses != null && acct.IPAddresses.Contains(ipaddress))
                    count++;
            }
            return count < MaxAccountsPerIP;
        }

        /// <summary>Has the account database been loaded</summary>
        public static bool Loaded { get; private set; }

        /// <summary>The next userid to use (try)</summary>
        private static int m_NextID = 1000;

        /// <summary>User accounts by Id</summary>
        private static Dictionary<int, Account> m_byId = new Dictionary<int, Account>();
        /// <summary>User accounts by username</summary>
        private static Dictionary<string, Account> m_byUsername = new Dictionary<string, Account>();

        public static Account Get(int id)
        {
            if (m_byId.ContainsKey(id))
                return m_byId[id];
            return null;
        }

        public static bool HasAccess(uint accountid, AccountAccessFlags flag) 
        {
            Account account = Get((int)accountid);
            return account != null && account.HasAccess(flag);
        } 



        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="username">username to add</param>
        /// <param name="password">password</param>
        /// <param name="IPAddress">IPAddress of creating client</param>
        /// <returns>True if user was added, false if the user could not be added.</returns>
        /// <remarks>Username must be unique. Username and Password must follow restrictions enforced by <see cref="ValidUsername">ValidUsername</see> and <see cref="ValidPassword">ValidPassword</see></remarks>
        public static bool Add(string username, string password, IPAddress IPAddress)
        {
            if (!ValidUsername(username) || !ValidPassword(password) || m_byUsername.ContainsKey(username)) return false;   // User not valid, Password not valid, or exists.

            int id;
            while (m_byId.ContainsKey(id=m_NextID++)); // Get next available account ID
            Account account = new Account(id, username, password, IPAddress);
            return InternalAdd(account);
        }

        /// <summary>
        /// Adds the account to to the database.
        /// </summary>
        /// <param name="account">The account to add</param>
        private static bool InternalAdd(Account account)
        {
            if (m_byId.ContainsKey(account.Id) || m_byUsername.ContainsKey(account.Username))
                return false;

            m_byId.Add(account.Id, account);
            m_byUsername.Add(account.Username, account);
            return true;
        }

        /// <summary>
        /// Determines if the supplied username is an allowable string.
        /// </summary>
        /// <param name="username">The username to check</param>
        /// <returns>True if ok</returns>
        /// <remarks>Username must be at least 4 characters, may not start with a digit, and must not begin or end in whitespace.</remarks>
        public static bool ValidUsername(string username)
        {
            return username!=null && username.Length>=4 && !char.IsDigit(username[0]) && username.Trim()==username;
        }

        /// <summary>
        /// Determines if the supplied password is an allowable string.
        /// </summary>
        /// <param name="password">The password to check</param>
        /// <returns>True if valid</returns>
        /// <remarks>Password must be at least 4 characters, and must not begin or end in whitespace.</remarks>
        public static bool ValidPassword(string password)
        {
            return password != null && password.Length >= 4 && password.Trim() == password;
        }

        /// <summary>
        /// Save the account database
        /// </summary>
        public static void Save()
        {
            string filePath = Persistance.GetSavePathname("accounts.xml");

            using (StreamWriter sw = new StreamWriter(filePath))
            {
                XmlTextWriter writer = new XmlTextWriter(sw);

                writer.Formatting = Formatting.Indented;
                writer.IndentChar = '\t';
                writer.Indentation = 1;

                writer.WriteStartDocument(true);

                writer.WriteStartElement("accounts");

                writer.WriteAttributeString("version", Account.AccountSaveVersion.ToString());
                writer.WriteAttributeString("count", m_byId.Count.ToString());

                foreach (Account a in m_byId.Values)
                    a.Serialize(writer);

                writer.WriteEndElement();

                writer.Close();
            }

        }

        private static void Load()
        {
            string filePath = Persistance.GetSavePathname("accounts.xml");

            if (!File.Exists(filePath))
            {
                Loaded = true;
                Console.WriteLine("Accounts: No account file found.");
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlElement root = doc["accounts"];
            int version = Persistance.Xml.GetXMLInt32(Persistance.Xml.GetAttribute(root, "version"), -1);
            int size = Persistance.Xml.GetXMLInt32(Persistance.Xml.GetAttribute(root, "count"), -1);
            foreach (XmlElement element in root.GetElementsByTagName("account"))
            {
                try
                {
                    Account account = new Account(element, version);
                    InternalAdd(account);
                }
                catch
                {
                    Console.WriteLine("Account load failed");
                }
            }
            Loaded = true;

            if (m_byId.Count != size) Console.WriteLine("Accounts: Warning: Loaded {0} of {1} accounts.", m_byId.Count, size);
            else Console.WriteLine("Accounts: Loaded {0} accounts.", m_byId.Count);
        }

        static bool Configured = false;
        /// <summary>
        /// Initialize login event sink, and loading of accounts.
        /// </summary>
        public static void Configure()
        {
            if (!Configured)
            {
                EventSink.OnLogin += new OnLoginEventHandler(EventSink_OnLogin);
                if (!Loaded) Load();
                Server.Core.OnAfterSave += new OnAfterSaveEventHandler(EventSink_OnAfterSave);

                Server.Core.OnGetAccess += new OnGetAccessEventHandler(ref Core_OnGetAccess);

                Configured = true;
            }
        }

        static void Core_OnGetAccess(ref GetAccountAccessArgs args)
        {
            Account account = Get((int)args.AccountNumber);
            if (account != null)
            {
                args.Handled = true;
                args.AccessFlags = account.Access;
            }
        }

        static void EventSink_OnAfterSave()
        {
            Save();
        }

        /// <summary>
        /// Validation of login or account creation
        /// </summary>
        /// <param name="e">The account login parameters</param>
        private static void EventSink_OnLogin(LoginEventArgs e)
        {
            e.Handled = true;
            Account acct = null;
            if(string.IsNullOrEmpty(e.Username))
            {
                e.Accepted = false;
                e.RejectedReason = ALRReason.BadPass;
                Console.Write("-Account Login Username Empty @ {0}", e.IPAddress.AsString());
                return;
            }
            if (string.IsNullOrEmpty(e.Password))
            {
                e.Accepted = false;
                e.RejectedReason = ALRReason.BadPass;
                Console.Write("-Account Login Password Empty @ {0}", e.IPAddress.AsString());
                return;
            }
            else if (m_byUsername.ContainsKey(e.Username))
            {
                acct = m_byUsername[e.Username];
                Console.Write("-Account Login: {0} @ {1}", e.Username, e.IPAddress.AsString());
            }
            else if (!CheckCreateAccount(e.IPAddress))
            {
                e.Accepted = false;
                e.RejectedReason = ALRReason.Invalid;
                Console.Write("-New account Denied: {0} @ {1}", e.Username, e.IPAddress.AsString());
                return;
            }
            else if (Add(e.Username, e.Password, e.IPAddress))
            {
                acct = m_byUsername[e.Username];
                Console.Write("-Created new account: {0} @ {1}", e.Username, e.IPAddress.AsString());
            }

            if (acct != null)
            {
                if (acct.Login(e.Password, e.IPAddress))
                {
                    e.Accepted = true;
                    e.AccountID = acct.Id;
                }
                else
                {
                    e.Accepted = false;
                    e.RejectedReason = ALRReason.BadPass;
                }
            }
            else
            {
                e.Accepted = false;
                e.RejectedReason = ALRReason.BadComm;
            }
        }

        #region Password encryption
        private static SHA1CryptoServiceProvider m_SHA1HashProvider;
        private static byte[] m_HashBuffer;
        public static string HashSHA1(string phrase)
        {
            if (m_SHA1HashProvider == null)
                m_SHA1HashProvider = new SHA1CryptoServiceProvider();

            if (m_HashBuffer == null)
                m_HashBuffer = new byte[256];

            int length = Encoding.ASCII.GetBytes(phrase, 0, phrase.Length > 256 ? 256 : phrase.Length, m_HashBuffer, 0);
            byte[] hashed = m_SHA1HashProvider.ComputeHash(m_HashBuffer, 0, length);

            return BitConverter.ToString(hashed);
        }
        #endregion

    }
}
