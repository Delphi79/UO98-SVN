using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Sharpkick.Network
{
    class ClientVersion : IComparable<ClientVersion> , IEquatable<ClientVersion>
    {
        public static Dictionary<string, ClientVersion> m_Intern = new Dictionary<string, ClientVersion>();

        public static ClientVersion v1_25_36f = Instantiate("1.25.36f");
        public static ClientVersion v1_26_0 = Instantiate("1.26.0");
        public static ClientVersion v1_26_2 = Instantiate("1.26.2");
        public static ClientVersion v1_26_3 = Instantiate("1.26.3");
        public static ClientVersion v1_26_4b = Instantiate("1.26.4b");
        public static ClientVersion v1_26_4i = Instantiate("1.26.4i");
        public static ClientVersion v2_0_0 = Instantiate("2.0.0");
        public static ClientVersion v3_0_8d = Instantiate("3.0.8d");   // Stat and skill caps are displayed in the interface so the player knows what their limits are. 
        public static ClientVersion v4_0_07a = Instantiate("4.0.07a"); // 0x0B Edit Area -> 0x0B Damage: I don't think we need to do anything about this, just a note.
        public static ClientVersion v5_0_7_0 = Instantiate("5.0.7.0");
        public static ClientVersion v6_0_1_7 = Instantiate("6.0.1.7"); // Not planning to support this client and upwards yet. This added grid coords to 0x08 lift & 0x25 container content packets

        public ClientVersionStruct Version { get; private set; }

        public static ClientVersion Instantiate(string versionstring)
        {
            if (!m_Intern.ContainsKey(versionstring))
                m_Intern[versionstring] = new ClientVersion(versionstring);
            return m_Intern[versionstring];
        }

        private static Dictionary<ClientVersionStruct, ClientVersion> m_byVal = new Dictionary<ClientVersionStruct, ClientVersion>();
        public static ClientVersion Instantiate(ClientVersionStruct versionstruct)
        {
            if (!m_byVal.ContainsKey(versionstruct))
                m_byVal[versionstruct] = Instantiate(ToString(versionstruct));
            return m_byVal[versionstruct];
        }


        public byte Major { get { return Version.Major; } }
        public byte Minor { get { return Version.Minor; } }
        public byte Build { get { return Version.Build; } }
        public byte Revision { get { return Version.Revision; } }

        public uint Long { get; private set; }

        private ClientVersion(string versionstring)
        {
            Version = Parse(versionstring);
            Long = ToLong(Version);
        }

        private string m_string = null;
        public override string ToString()
        {
            if (m_string == null)
                m_string = ToString(Version);
            return m_string;
        }

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

        //public static implicit operator ClientVersionStruct(ClientVersion a){return a.Version;}

        public static bool operator <(ClientVersion a, ClientVersion b) { return Comparer.Compare(a, b) < 0; }
        public static bool operator <=(ClientVersion a, ClientVersion b) { return Comparer.Compare(a, b) <= 0; }
        public static bool operator >(ClientVersion a, ClientVersion b) { return Comparer.Compare(a, b) > 0; }
        public static bool operator >=(ClientVersion a, ClientVersion b) { return Comparer.Compare(a, b) >= 0; }
        //public static bool operator ==(ClientVersion a, ClientVersion b) { return Comparer.Compare(a, b) == 0; }
        //public static bool operator !=(ClientVersion a, ClientVersion b) { return Comparer.Compare(a, b) != 0; }

        //public static bool operator <(ClientVersion a, ClientVersionStruct b) { return a.Long < ToLong(b); }
        //public static bool operator <=(ClientVersion a, ClientVersionStruct b) { return a.Long <= ToLong(b); }
        //public static bool operator >(ClientVersion a, ClientVersionStruct b) { return a.Long > ToLong(b); }
        //public static bool operator >=(ClientVersion a, ClientVersionStruct b) { return a.Long >= ToLong(b); }
        //public static bool operator ==(ClientVersion a, ClientVersionStruct b) { return a.Long == ToLong(b); }
        //public static bool operator !=(ClientVersion a, ClientVersionStruct b) { return a.Long != ToLong(b); }

        public static bool operator <(ClientVersionStruct a, ClientVersion b) { return ToLong(a) < b.Long; }
        public static bool operator <=(ClientVersionStruct a, ClientVersion b) { return ToLong(a) <= b.Long; }
        public static bool operator >(ClientVersionStruct a, ClientVersion b) { return ToLong(a) > b.Long; }
        public static bool operator >=(ClientVersionStruct a, ClientVersion b) { return ToLong(a) >= b.Long; }
        public static bool operator ==(ClientVersionStruct a, ClientVersion b) { return ToLong(a) == b.Long; }
        public static bool operator !=(ClientVersionStruct a, ClientVersion b) { return ToLong(a) != b.Long; }

        public override bool Equals(object obj)
        {
            if (obj is ClientVersion)
                return ToLong(this.Version) == ToLong(((ClientVersion)obj).Version);
            if(obj is ClientVersionStruct)
                return ToLong(this.Version) == ToLong((ClientVersionStruct)obj);
            return false;
        }

        public static ClientVersionComparer Comparer = new ClientVersionComparer();
        public class ClientVersionComparer : IComparer<ClientVersion>
        {
            public int Compare(ClientVersion x, ClientVersion y)
            {
                unchecked
                {
                    return (x == null) ? ((y == null) ? 0 : -1) : x.Long == y.Long ? 0 : x.Long < y.Long ? -1 : 1;
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
            return (int)ToLong(this.Version);
        }

        public static uint ToLong(ClientVersionStruct clientversion)
        {
            return (uint)(clientversion.Major << 24) + (uint)(clientversion.Minor << 16) + (uint)(clientversion.Build << 8) + clientversion.Revision;
        }

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
