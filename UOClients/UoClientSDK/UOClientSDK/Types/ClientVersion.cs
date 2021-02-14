using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace UoClientSDK
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ClientVersionStruct
    {
        public byte Major;
        public byte Minor;
        public byte Build;
        public byte Revision;

        /// <summary>The lowest client version</summary>
        public static ClientVersionStruct Min = new ClientVersionStruct() { Major = 0, Minor = 0, Build = 0, Revision = 0 };
        /// <summary>Represents the highest value of a normal client version. Not really a max, AdminClient is higher than this.</summary>
        public static ClientVersionStruct Max = new ClientVersionStruct() { Major = 254, Minor = 255, Build = 255, Revision = 255 };
        /// <summary>Indicates the Remote Admin Client. Higher than ClientVersionStruct.Max</summary>
        public static ClientVersionStruct AdminClient = new ClientVersionStruct() { Major = 255, Minor = 0, Build = 0, Revision = 0 };
    }

    /// <summary>
    /// An immutable internally cached class representing a client version.
    /// </summary>
    public class ClientVersion : IComparable<ClientVersion> , IEquatable<ClientVersion>
    {
        public static Dictionary<string, ClientVersion> m_Intern = new Dictionary<string, ClientVersion>();

        public static ClientVersion v1_25_36f = Instantiate("1.25.36f");
        public static ClientVersion v1_26_0 = Instantiate("1.26.0");
        public static ClientVersion v1_26_2 = Instantiate("1.26.2");
        public static ClientVersion v1_26_3 = Instantiate("1.26.3");
        public static ClientVersion v1_26_4b = Instantiate("1.26.4b");
        public static ClientVersion v1_26_4i = Instantiate("1.26.4i");
        public static ClientVersion v2_0_0 = Instantiate("2.0.0");
        
        /// <summary>Stat and skill caps are displayed in the interface so the player knows what their limits are.</summary>
        public static ClientVersion v3_0_8d = Instantiate("3.0.8d");
        
        /// <summary>0x0B Edit Area -> 0x0B Damage</summary>
        public static ClientVersion v4_0_07a = Instantiate("4.0.07a");
        
        public static ClientVersion v5_0_7_0 = Instantiate("5.0.7.0");

        /// <summary>Added grid coordinates to 0x08 lift & 0x25 container content packets</summary>
        public static ClientVersion v6_0_1_7 = Instantiate("6.0.1.7");

        public static ClientVersion v7_0_0_0 = Instantiate("7.0.0.0");

        /// <summary>The lowest client version</summary>
        public static ClientVersion vMIN = Instantiate(ClientVersionStruct.Min);
        /// <summary>Represents the highest value of a normal client version. Not really a max, AdminClient is higher than this.</summary>
        public static ClientVersion vMAX = Instantiate(ClientVersionStruct.Max);

        /// <summary>Indicates the Remote Admin Client. Higher than vMAX</summary>
        public static ClientVersion vAdminClient = Instantiate(ClientVersionStruct.AdminClient);

        /// <summary>
        /// Creates an immutable ClientVersion object or retrieves an existing one from the internal cache.
        /// </summary>
        /// <param name="versionstring">A string representing a client version in the format "A.B.Cd" or "A.B.C.D" Examples: 1.25.36f, 4.0.07a, 6.0.1.7"</param>
        /// <returns>a shared ClientVersion object</returns>
        public static ClientVersion Instantiate(string versionstring)
        {
            if (!m_Intern.ContainsKey(versionstring))
                m_Intern[versionstring] = new ClientVersion(versionstring);
            return m_Intern[versionstring];
        }

        private static Dictionary<ClientVersionStruct, ClientVersion> m_byVal;
        /// <summary>
        /// Creates an immutable ClientVersion object or retrieves an existing one from the internal cache.
        /// </summary>
        /// <param name="versionstruct">A ClientVersionStruct representing the version of the ClientVersion object desired.</param>
        /// <returns>a shared ClientVersion object</returns>
        public static ClientVersion Instantiate(ClientVersionStruct versionstruct)
        {
            if (!(m_byVal ?? (m_byVal = new Dictionary<ClientVersionStruct, ClientVersion>())).ContainsKey(versionstruct))
                m_byVal[versionstruct] = Instantiate(ToString(versionstruct));
            return m_byVal[versionstruct];
        }

        /// <summary>The first digit of the client version, represented by 1 in this version string "1.25.36f"</summary>
        public byte Major { get { return Version.Major; } }
        /// <summary>The second digit of the client version, represented by 25 in this version string "1.25.36f"</summary>
        public byte Minor { get { return Version.Minor; } }
        /// <summary>The third digit of the client version, represented by 36 in this version string "1.25.36f"</summary>
        public byte Build { get { return Version.Build; } }
        /// <summary>The numeric value of the fourth digit or character of the client version, represented by f in this version string "1.25.36f" or 7 in this one "6.0.1.7"</summary>
        public byte Revision { get { return Version.Revision; } }

        public readonly ClientVersionStruct Version;
        private readonly uint UIntVal;

        private ClientVersion(string versionstring)
        {
            Version = Parse(versionstring);
            UIntVal = ToUint(Version);
        }

        private string m_string = null;
        /// <summary>
        /// Formats the client version instance to a string.
        /// </summary>
        /// <returns>A string in the standard version format</returns>
        public override string ToString()
        {
            if (m_string == null)
                m_string = ToString(Version);
            return m_string;
        }

        /// <summary>
        /// Formats the client version to a string.
        /// </summary>
        /// <param name="Version">The version representing the desired version string.</param>
        /// <returns>A string in the standard version format</returns>
        public static string ToString(ClientVersionStruct Version)
        {
            if (Version < v5_0_7_0)
            {
                if (Version.Revision <= 0)
                    return string.Format("{0}.{1}.{2}", Version.Major, Version.Minor, Version.Build);
                else
                    return string.Format("{0}.{1}.{2}{3}", Version.Major, Version.Minor, Version.Build, (char)((ushort)'a' + Version.Revision - 1));
            }
            return string.Format("{0}.{1}.{2}.{3}", Version.Major, Version.Minor, Version.Build, Version.Revision);
        }

        public static bool operator <(ClientVersion a, ClientVersion b) { return Comparer.Compare(a, b) < 0; }
        public static bool operator <=(ClientVersion a, ClientVersion b) { return Comparer.Compare(a, b) <= 0; }
        public static bool operator >(ClientVersion a, ClientVersion b) { return Comparer.Compare(a, b) > 0; }
        public static bool operator >=(ClientVersion a, ClientVersion b) { return Comparer.Compare(a, b) >= 0; }
 
        public static bool operator <(ClientVersionStruct a, ClientVersion b) { return ToUint(a) < b.UIntVal; }
        public static bool operator <=(ClientVersionStruct a, ClientVersion b) { return ToUint(a) <= b.UIntVal; }
        public static bool operator >(ClientVersionStruct a, ClientVersion b) { return ToUint(a) > b.UIntVal; }
        public static bool operator >=(ClientVersionStruct a, ClientVersion b) { return ToUint(a) >= b.UIntVal; }
        public static bool operator ==(ClientVersionStruct a, ClientVersion b) { return ToUint(a) == b.UIntVal; }
        public static bool operator !=(ClientVersionStruct a, ClientVersion b) { return ToUint(a) != b.UIntVal; }

        public override bool Equals(object obj)
        {
            if (obj is ClientVersion)
                return ToUint(this.Version) == ToUint(((ClientVersion)obj).Version);
            if(obj is ClientVersionStruct)
                return ToUint(this.Version) == ToUint((ClientVersionStruct)obj);
            return false;
        }

        static ClientVersionComparer Comparer = new ClientVersionComparer();
        class ClientVersionComparer : IComparer<ClientVersion>
        {
            public int Compare(ClientVersion x, ClientVersion y)
            {
                unchecked
                {
                    return (x == null) ? ((y == null) ? 0 : -1) : x.UIntVal == y.UIntVal ? 0 : x.UIntVal < y.UIntVal ? -1 : 1;
                }
            }
        }

        public bool Equals(ClientVersion other)
        {
            return ClientVersion.Comparer.Compare(this, other) == 0;
        }

        public int CompareTo(ClientVersion other)
        {
            return ClientVersion.Comparer.Compare(this, other);
        }

        public override int GetHashCode()
        {
            return (int)UIntVal;
        }

        /// <summary>
        /// Converts the client version to a unique integer hash.
        /// </summary>
        /// <param name="clientversion"></param>
        /// <returns></returns>
        public static uint ToUint(ClientVersionStruct clientversion)
        {
            return (uint)(clientversion.Major << 24) + (uint)(clientversion.Minor << 16) + (uint)(clientversion.Build << 8) + clientversion.Revision;
        }

        /// <summary>
        /// Parses the supplied sting to a ClientVersionStruct, if possible.
        /// </summary>
        /// <param name="VersionString">A string in the standard version format</param>
        /// <returns>A ClientVersionStruct representing the best interpretation of the VersionString, Zero's will appear in sections of parse failure.</returns>
        public static ClientVersionStruct Parse(string VersionString)
        {
            ClientVersionStruct ver=new ClientVersionStruct();

            string[] parts = VersionString.Split('.');
            byte n;
            if (parts.Length >= 1 && byte.TryParse(parts[0], out n))
            {
                ver.Major = n;
                if (parts.Length >= 2 && byte.TryParse(parts[1], out n))
                {
                    ver.Minor = n;

                    if (parts.Length >= 3)
                    {   // The third section is tricky, version such as 2.0.0g1 may be possible in versions below 5.0.7.0
                        if (byte.TryParse(parts[2], out n))
                        {   // easy
                            ver.Build = n;
                            if (parts.Length >= 4 && byte.TryParse(parts[3], out n))
                            {
                                ver.Revision = n;
                            }
                        }
                        else
                        {   // not as easy
                            // Find non-numeric character in string
                            int i = 0;
                            while (i < parts[2].Length && char.IsDigit(parts[2][i])) i++;
                            if (i > 0 && byte.TryParse(parts[2].Substring(0, i), out n)) // extract the number
                            {
                                ver.Build = n;
                                if (char.IsLetter(parts[2][i]))  // extract the letter
                                {
                                    ver.Revision = (byte)(ASCIIEncoding.ASCII.GetBytes(parts[2].Substring(i, 1))[0] - (byte)'a' + 1);
                                    // Ignoring sub revision which occurs in two client versions. Example: 2.0.0e1, 4.0.4b2
                                }
                            }
                        }
                    }
                }
            }
            return ver;
        }
    }
}
