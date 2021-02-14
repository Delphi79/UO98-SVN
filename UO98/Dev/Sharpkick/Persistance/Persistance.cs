using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;

namespace Sharpkick
{
    static class Persistance
    {
        public static string GetSavePathname(string filename)
        {
            if (!Directory.Exists(Server.ServerConfiguration.SavePath))
                Directory.CreateDirectory(Server.ServerConfiguration.SavePath);

            return Path.Combine(Server.ServerConfiguration.SavePath, filename);
        }

        public static string GetDataPathname(string filename)
        {
            if (!Directory.Exists(Server.ServerConfiguration.DataPath))
                Directory.CreateDirectory(Server.ServerConfiguration.DataPath);

            return Path.Combine(Server.ServerConfiguration.DataPath, filename);
        }

        private static Dictionary<IPAddress, IPAddress> _ipAddressTable;

        /// <summary>
        /// Reduces IPAddress object count by reusing existing object references
        /// </summary>
        /// <param name="ipAddress">An IP address</param>
        /// <returns>A reference to an existing object of the same IP, or the object provided</returns>
        public static IPAddress Intern(IPAddress ipAddress)
        {
            if (_ipAddressTable == null)
            {
                _ipAddressTable = new Dictionary<IPAddress, IPAddress>();
            }

            IPAddress interned;

            if (!_ipAddressTable.TryGetValue(ipAddress, out interned))
            {
                interned = ipAddress;
                _ipAddressTable[ipAddress] = interned;
            }

            return interned;
        }

        public static void Intern(ref IPAddress ipAddress)
        {
            ipAddress = Intern(ipAddress);
        }

        public static class Xml
        {
            public static int GetXMLInt32(string intString, int defaultValue)
            {
                try
                {
                    return XmlConvert.ToInt32(intString);
                }
                catch
                {
                    int val;
                    if (int.TryParse(intString, out val))
                        return val;

                    return defaultValue;
                }
            }

            public static DateTime GetXMLDateTime(string dateTimeString, DateTime defaultValue)
            {
                try
                {
                    return XmlConvert.ToDateTime(dateTimeString, XmlDateTimeSerializationMode.Local);
                }
                catch
                {
                    DateTime d;

                    if (DateTime.TryParse(dateTimeString, out d))
                        return d;

                    return defaultValue;
                }
            }

            public static TimeSpan GetXMLTimeSpan(string timeSpanString, TimeSpan defaultValue)
            {
                try
                {
                    return XmlConvert.ToTimeSpan(timeSpanString);
                }
                catch
                {
                    return defaultValue;
                }
            }

            public static string GetAttribute(XmlElement node, string attributeName)
            {
                return GetAttribute(node, attributeName, null);
            }

            public static string GetAttribute(XmlElement node, string attributeName, string defaultValue)
            {
                if (node == null)
                    return defaultValue;

                XmlAttribute attr = node.Attributes[attributeName];

                if (attr == null)
                    return defaultValue;

                return attr.Value;
            }

            public static string GetText(XmlElement node, string defaultValue)
            {
                if (node == null)
                    return defaultValue;

                return node.InnerText;
            }

            public static int GetAddressValue(IPAddress address)
            {
#pragma warning disable 618
                return (int)address.Address;
#pragma warning restore 618
            }

            public static long GetLongAddressValue(IPAddress address)
            {
#pragma warning disable 618
                return address.Address;
#pragma warning restore 618
            }

            /// <summary>
            /// Deserializes a list of IPAddress values from an xml element.
            /// </summary>
            /// <param name="node">The XmlElement from which to deserialize this subnode.</param>
            /// <param name="addressNodeName">The note within the XmlElement which contains the address list.</param>
            /// <returns>Address list, or null</returns>
            public static List<IPAddress> LoadAddressList(XmlElement node, string addressNodeName)
            {
                List<IPAddress> list = null;
                XmlElement addressList = node[addressNodeName];

                if (addressList != null)
                {
                    int count = GetXMLInt32(GetAttribute(addressList, "count", "0"), 0);

                    list = new List<IPAddress>(count);

                    foreach (XmlElement ip in addressList.GetElementsByTagName("ip"))
                    {
                        IPAddress address;

                        if (IPAddress.TryParse(GetText(ip, null), out address))
                        {
                            list.Add(Persistance.Intern(address));
                        }
                    }
                }

                return list;
            }

        }
    }
}
