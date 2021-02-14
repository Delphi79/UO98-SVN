using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Runtime.InteropServices;

namespace Sharpkick
{
    class Mobile
    {
        protected static Dictionary<uint, Mobile> AllMobiles = new Dictionary<uint, Mobile>();

        public static Mobile Get(uint serial)
        {
            if (AllMobiles.ContainsKey(serial))
                return AllMobiles[serial];
            return null;
        }

        /// <summary>Servers Serial for this Mobile</summary>
        public uint Serial { get; private set; }
        /// <summary>Characters latest captured name</summary>
        public string Name { get; private set; }
        /// <summary>Characters latest captured title</summary>
        public string Title { get; private set; }

        /// <summary>Characters profile data (Paperdoll scroll)</summary>
        public string Profile { get; private set; }
        /// <summary>Last seen</summary>
        public DateTime LastSeen { get; private set; }
        /// <summary>Account Number if Player</summary>
        public int AccountNumber { get; set; }

        public Mobile(uint serial)
        {
            Serial = serial;
            AccountNumber = -1;
            LastSeen = DateTime.MinValue;
            AllMobiles[serial] = this;
        }

        /// <summary>
        /// Updates the LastSeen time
        /// </summary>
        public void Touch()
        {
            LastSeen = DateTime.UtcNow;
        }

        /// <summary>
        /// Ensures that the Mobile is in the AllMobiles dictionary, and updates the LastSeen time
        /// </summary>
        public static void TouchMobile(uint serial)
        {
            if (!AllMobiles.ContainsKey(serial))
                new Mobile(serial); // constructor adds
            else
                AllMobiles[serial].Touch();
        }

        public static void UpdateMobileName(uint serial, string Name)
        {
            TouchMobile(serial);
            if (AllMobiles[serial].Name != Name)
                AllMobiles[serial].Name = Name;
        }

        public static void UpdateMobileTitle(uint serial, string Title)
        {
            TouchMobile(serial);
            if (AllMobiles[serial].Title != Title)
                AllMobiles[serial].Title = Title;
        }

        public static void UpdateMobileProfile(uint serial, string Profile)
        {
            TouchMobile(serial);
            if (AllMobiles[serial].Profile != Profile)
                AllMobiles[serial].Profile = Profile;
        }

        /// <summary>
        /// Deserializes a dictionary of Mobiles from an xml element.
        /// </summary>
        /// <param name="node">The XmlElement from which to deserialize this subnode.</param>
        /// <param name="mobileNodeName">The note within the XmlElement which contains the address list.</param>
        /// <returns>Mobile dictionary, or null</returns>
        public static Dictionary<uint, Mobile> LoadMobileDict(XmlElement node, string mobileNodeName)
        {
            Dictionary<uint, Mobile> dict = null;
            XmlElement mobileList = node[mobileNodeName];

            if (mobileList != null)
            {
                int count = Persistance.Xml.GetXMLInt32(Persistance.Xml.GetAttribute(mobileList, "count", "0"), 0);

                dict = new Dictionary<uint, Mobile>(count);

                foreach (XmlElement mobile in mobileList.GetElementsByTagName("mobile"))
                {
                    uint serial = (uint)Persistance.Xml.GetXMLInt32(Persistance.Xml.GetText(mobile["serial"], "0"), 0);
                    string name = Persistance.Xml.GetText(mobile["name"], null);
                    string title = Persistance.Xml.GetText(mobile["title"], null);
                    string profile = Persistance.Xml.GetText(mobile["profile"], null);
                    DateTime lastseen = Persistance.Xml.GetXMLDateTime(Persistance.Xml.GetText(mobile["lastseen"], null), DateTime.UtcNow);
                    int accountnum = Persistance.Xml.GetXMLInt32(Persistance.Xml.GetText(mobile["serial"], "-1"), -1);
                    if (serial > 0)
                    {
                        Mobile m = new Mobile(serial);
                        m.Name = name;
                        m.Title = title;
                        m.Profile = profile;
                        m.LastSeen = lastseen;
                        m.AccountNumber = accountnum;
                        dict[serial] = m;
                    }
                }
            }

            return dict;
        }
    }
}
