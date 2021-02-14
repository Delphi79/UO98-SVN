using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick.Network
{
    class PacketVersionEntry
    {
        public ClientVersion Version {get;private set;}
        public ushort Length { get; private set; }
        public bool Dynamic { get; private set; }

        public PacketVersionEntry(ClientVersion clientversion, bool dynamic, ushort length)
        {
            Version = clientversion;
            Length = length;
            Dynamic = dynamic;
        }

    }

    class PacketVersions
    {
        public byte Id { get; private set; }
        //public bool ServerAccepts { get; private set; }
        private SortedDictionary<ClientVersion, PacketVersionEntry> Versions=new SortedDictionary<ClientVersion,PacketVersionEntry>();

        private PacketVersions(byte id)
        {
            Id = id;
        }

        private void Add(PacketVersionEntry clientpacketversion)
        {
            Versions[clientpacketversion.Version] = clientpacketversion;
        }

        /// <summary>
        /// Get the Version Info for this packet, for the given client version
        /// </summary>
        /// <param name="version">The version of the client</param>
        /// <returns>The packet version for the supplied client version, or null if the id is invalid for the given client version</returns>
        private PacketVersionEntry this[ClientVersion version]
        {
            get
            {
                PacketVersionEntry toReturn = null;
                foreach (ClientVersion v in Versions.Keys)
                    if (v <= version)
                        toReturn = Versions[v];
                    else
                        break;
                return toReturn;
            }
        }

        private static Dictionary<byte, PacketVersions> m_PacketNfo = new Dictionary<byte, PacketVersions>();
        
        /// <summary>
        /// Get the packet version entry, for the given packet id and client version
        /// </summary>
        /// <param name="id">Packet ID</param>
        /// <param name="version">Client Version</param>
        /// <returns>A packet entry object, or null if the id is invalid for the given client version</returns>
        public static PacketVersionEntry GetPacketInfo(byte id, ClientVersion version)
        {
            if (m_PacketNfo.ContainsKey(id))
                return m_PacketNfo[id][version];
            return null;
        }

        private static bool Configured=false;
        public static void Configure()
        {
            if (Configured) return;
            
            LoadVersions();

            Configured = true;
        }

        /// <summary>Thanks Batlin!</summary>
        private static void LoadVersions()
        {
            ClientVersion version;
            #region 1.23.0
            version = ClientVersion.Instantiate("1.23.0");
            AddNfo(0x00, new PacketVersionEntry(version, false, 0x62));
            AddNfo(0x01, new PacketVersionEntry(version, false, 0x5));
            AddNfo(0x02, new PacketVersionEntry(version, false, 0x3));
            AddNfo(0x03, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x04, new PacketVersionEntry(version, false, 0x2));
            AddNfo(0x05, new PacketVersionEntry(version, false, 0x5));
            AddNfo(0x06, new PacketVersionEntry(version, false, 0x5));
            AddNfo(0x07, new PacketVersionEntry(version, false, 0x7));
            AddNfo(0x08, new PacketVersionEntry(version, false, 0xE));
            AddNfo(0x09, new PacketVersionEntry(version, false, 0x5));
            AddNfo(0x0A, new PacketVersionEntry(version, false, 0xB));
            AddNfo(0x0B, new PacketVersionEntry(version, false, 0x10A));
            AddNfo(0x0C, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x0D, new PacketVersionEntry(version, false, 0x3));
            AddNfo(0x0E, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x0F, new PacketVersionEntry(version, false, 0x3D));
            AddNfo(0x10, new PacketVersionEntry(version, false, 0xD7));
            AddNfo(0x11, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x12, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x13, new PacketVersionEntry(version, false, 0xA));
            AddNfo(0x14, new PacketVersionEntry(version, false, 0x6));
            AddNfo(0x15, new PacketVersionEntry(version, false, 0x9));
            AddNfo(0x16, new PacketVersionEntry(version, false, 0x1));
            AddNfo(0x17, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x18, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x19, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x1A, new PacketVersionEntry(version, false, 0x13));
            AddNfo(0x1B, new PacketVersionEntry(version, false, 0x25));
            AddNfo(0x1C, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x1D, new PacketVersionEntry(version, false, 0x5));
            AddNfo(0x1E, new PacketVersionEntry(version, false, 0x4));
            AddNfo(0x1F, new PacketVersionEntry(version, false, 0x8));
            AddNfo(0x20, new PacketVersionEntry(version, false, 0x13));
            AddNfo(0x21, new PacketVersionEntry(version, false, 0x8));
            AddNfo(0x22, new PacketVersionEntry(version, false, 0x2));
            AddNfo(0x23, new PacketVersionEntry(version, false, 0x1A));
            AddNfo(0x24, new PacketVersionEntry(version, false, 0x7));
            AddNfo(0x25, new PacketVersionEntry(version, false, 0x14));
            AddNfo(0x26, new PacketVersionEntry(version, false, 0x5));
            AddNfo(0x27, new PacketVersionEntry(version, false, 0x2));
            AddNfo(0x28, new PacketVersionEntry(version, false, 0x5));
            AddNfo(0x29, new PacketVersionEntry(version, false, 0x1));
            AddNfo(0x2A, new PacketVersionEntry(version, false, 0x5));
            AddNfo(0x2B, new PacketVersionEntry(version, false, 0x2));
            AddNfo(0x2C, new PacketVersionEntry(version, false, 0x2));
            AddNfo(0x2D, new PacketVersionEntry(version, false, 0x11));
            AddNfo(0x2E, new PacketVersionEntry(version, false, 0xF));
            AddNfo(0x2F, new PacketVersionEntry(version, false, 0xA));
            AddNfo(0x30, new PacketVersionEntry(version, false, 0x5));
            AddNfo(0x31, new PacketVersionEntry(version, false, 0x1));
            AddNfo(0x32, new PacketVersionEntry(version, false, 0x2));
            AddNfo(0x33, new PacketVersionEntry(version, false, 0x2));
            AddNfo(0x34, new PacketVersionEntry(version, false, 0xA));
            AddNfo(0x35, new PacketVersionEntry(version, false, 0x28D));
            AddNfo(0x36, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x37, new PacketVersionEntry(version, false, 0x8));
            AddNfo(0x38, new PacketVersionEntry(version, false, 0x7));
            AddNfo(0x39, new PacketVersionEntry(version, false, 0x9));
            AddNfo(0x3A, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x3B, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x3C, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x3D, new PacketVersionEntry(version, false, 0x2));
            AddNfo(0x3E, new PacketVersionEntry(version, false, 0x25));
            AddNfo(0x3F, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x40, new PacketVersionEntry(version, false, 0xC9));
            AddNfo(0x41, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x42, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x43, new PacketVersionEntry(version, false, 0x229));
            AddNfo(0x44, new PacketVersionEntry(version, false, 0x2C9));
            AddNfo(0x45, new PacketVersionEntry(version, false, 0x5));
            AddNfo(0x46, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x47, new PacketVersionEntry(version, false, 0xB));
            AddNfo(0x48, new PacketVersionEntry(version, false, 0x49));
            AddNfo(0x49, new PacketVersionEntry(version, false, 0x5D));
            AddNfo(0x4A, new PacketVersionEntry(version, false, 0x5));
            AddNfo(0x4B, new PacketVersionEntry(version, false, 0x9));
            AddNfo(0x4C, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x4D, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x4E, new PacketVersionEntry(version, false, 0x6));
            AddNfo(0x4F, new PacketVersionEntry(version, false, 0x2));
            AddNfo(0x50, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x51, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x52, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x53, new PacketVersionEntry(version, false, 0x2));
            AddNfo(0x54, new PacketVersionEntry(version, false, 0xC));
            AddNfo(0x55, new PacketVersionEntry(version, false, 0x1));
            AddNfo(0x56, new PacketVersionEntry(version, false, 0xB));
            AddNfo(0x57, new PacketVersionEntry(version, false, 0x6E));
            AddNfo(0x58, new PacketVersionEntry(version, false, 0x6A));
            AddNfo(0x59, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x5A, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x5B, new PacketVersionEntry(version, false, 0x4));
            AddNfo(0x5C, new PacketVersionEntry(version, false, 0x2));
            AddNfo(0x5D, new PacketVersionEntry(version, false, 0x49));
            AddNfo(0x5E, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x5F, new PacketVersionEntry(version, false, 0x31));
            AddNfo(0x60, new PacketVersionEntry(version, false, 0x5));
            AddNfo(0x61, new PacketVersionEntry(version, false, 0x9));
            AddNfo(0x62, new PacketVersionEntry(version, false, 0xF));
            AddNfo(0x63, new PacketVersionEntry(version, false, 0xD));
            AddNfo(0x64, new PacketVersionEntry(version, false, 0x1));
            AddNfo(0x65, new PacketVersionEntry(version, false, 0x4));
            AddNfo(0x66, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x67, new PacketVersionEntry(version, false, 0x15));
            AddNfo(0x68, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x69, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x6A, new PacketVersionEntry(version, false, 0x3));
            AddNfo(0x6B, new PacketVersionEntry(version, false, 0x9));
            AddNfo(0x6C, new PacketVersionEntry(version, false, 0x12));
            AddNfo(0x6D, new PacketVersionEntry(version, false, 0x3));
            AddNfo(0x6E, new PacketVersionEntry(version, false, 0xE));
            AddNfo(0x6F, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x70, new PacketVersionEntry(version, false, 0x1C));
            AddNfo(0x71, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x72, new PacketVersionEntry(version, false, 0x5));
            AddNfo(0x73, new PacketVersionEntry(version, false, 0x2));
            AddNfo(0x74, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x75, new PacketVersionEntry(version, false, 0x23));
            AddNfo(0x76, new PacketVersionEntry(version, false, 0x10));
            AddNfo(0x77, new PacketVersionEntry(version, false, 0x10));
            AddNfo(0x78, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x79, new PacketVersionEntry(version, false, 0x9));
            AddNfo(0x7A, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x7B, new PacketVersionEntry(version, false, 0x2));
            AddNfo(0x7C, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x7D, new PacketVersionEntry(version, false, 0xD));
            AddNfo(0x7E, new PacketVersionEntry(version, false, 0x2));
            AddNfo(0x7F, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x80, new PacketVersionEntry(version, false, 0x3E));
            AddNfo(0x81, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x82, new PacketVersionEntry(version, false, 0x2));
            AddNfo(0x83, new PacketVersionEntry(version, false, 0x27));
            AddNfo(0x84, new PacketVersionEntry(version, false, 0x45));
            AddNfo(0x85, new PacketVersionEntry(version, false, 0x2));
            AddNfo(0x86, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x87, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x88, new PacketVersionEntry(version, false, 0x42));
            AddNfo(0x89, new PacketVersionEntry(version, false, 0x6D));
            AddNfo(0x8A, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x8B, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x8C, new PacketVersionEntry(version, false, 0xB));
            AddNfo(0x8D, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x8E, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x8F, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x90, new PacketVersionEntry(version, false, 0x13));
            AddNfo(0x91, new PacketVersionEntry(version, false, 0x2));
            AddNfo(0x92, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x93, new PacketVersionEntry(version, false, 0x62));
            AddNfo(0x94, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x95, new PacketVersionEntry(version, false, 0x9));
            AddNfo(0x96, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x97, new PacketVersionEntry(version, false, 0x2));
            AddNfo(0x98, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x99, new PacketVersionEntry(version, false, 0x1A));
            AddNfo(0x9A, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x9B, new PacketVersionEntry(version, false, 0x2));
            AddNfo(0x9C, new PacketVersionEntry(version, false, 0x35));
            AddNfo(0x9D, new PacketVersionEntry(version, false, 0x33));
            #endregion

            #region 1.23.37b
            version = ClientVersion.Instantiate("1.23.37b");
            AddNfo(0x00, new PacketVersionEntry(version, false, 0x64));
            // -Packet 00, new ClientPacketVersion(version, false, 0x62));
            // -Packet 1A, new ClientPacketVersion(version, false, 0x13));
            AddNfo(0x1A, new PacketVersionEntry(version, true, 0x8000));
            // -Packet 89, new ClientPacketVersion(version, false, 0x6D));
            AddNfo(0x89, new PacketVersionEntry(version, true, 0x8000));
            // -Packet 91, new ClientPacketVersion(version, false, 0x2));
            AddNfo(0x91, new PacketVersionEntry(version, false, 0x41));
            // -Packet 9B, new ClientPacketVersion(version, false, 0x2));
            AddNfo(0x9B, new PacketVersionEntry(version, false, 0x102));
            AddNfo(0x9C, new PacketVersionEntry(version, false, 0x135));
            // -Packet 9C, new ClientPacketVersion(version, false, 0x35));
            AddNfo(0x9E, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0x9F, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xA0, new PacketVersionEntry(version, false, 0x3));
            AddNfo(0xA1, new PacketVersionEntry(version, false, 0x9));
            AddNfo(0xA2, new PacketVersionEntry(version, false, 0x9));
            AddNfo(0xA3, new PacketVersionEntry(version, false, 0x9));
            AddNfo(0xA4, new PacketVersionEntry(version, false, 0x95));
            #endregion

            #region 1.25.35
            version = ClientVersion.Instantiate("1.25.35");
            // -Packet 22, new ClientPacketVersion(version, false, 0x2));
            AddNfo(0x22, new PacketVersionEntry(version, false, 0x3));
            // -Packet 6C, new ClientPacketVersion(version, false, 0x12));
            AddNfo(0x6C, new PacketVersionEntry(version, false, 0x13));
            // -Packet 77, new ClientPacketVersion(version, false, 0x10));
            AddNfo(0x77, new PacketVersionEntry(version, false, 0x11));
            AddNfo(0xA5, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xA6, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xA7, new PacketVersionEntry(version, false, 0x4));
            AddNfo(0xA8, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xA9, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xAA, new PacketVersionEntry(version, false, 0x5));
            AddNfo(0xAB, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xAC, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xAD, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xAE, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xAF, new PacketVersionEntry(version, false, 0xD));
            AddNfo(0xB0, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xB1, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xB2, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xB3, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xB4, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xB5, new PacketVersionEntry(version, false, 0x40));
            AddNfo(0xB6, new PacketVersionEntry(version, false, 0x9));
            AddNfo(0xB7, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xB8, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xB9, new PacketVersionEntry(version, false, 0x3));
            #endregion

            #region 1.26.0
            version = ClientVersion.Instantiate("1.26.0");
            // -Packet 00, new ClientPacketVersion(version, false, 0x64));
            AddNfo(0x00, new PacketVersionEntry(version, false, 0x68));
            // -Packet 02, new ClientPacketVersion(version, false, 0x3));
            AddNfo(0x02, new PacketVersionEntry(version, false, 0x7));
            // -Packet 93, new ClientPacketVersion(version, false, 0x62));
            AddNfo(0x93, new PacketVersionEntry(version, false, 0x63));
            AddNfo(0xBA, new PacketVersionEntry(version, false, 0x6));
            AddNfo(0xBB, new PacketVersionEntry(version, false, 0x9));
            AddNfo(0xBC, new PacketVersionEntry(version, false, 0x2));
            AddNfo(0xBD, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xBE, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xBF, new PacketVersionEntry(version, true, 0x8000));
            #endregion

            #region 1.26.2
            version = ClientVersion.Instantiate("1.26.2");
            AddNfo(0xC0, new PacketVersionEntry(version, false, 0x24));
            AddNfo(0xC1, new PacketVersionEntry(version, true, 0x8000));
            #endregion

            #region 1.26.3
            version = ClientVersion.Instantiate("1.26.3");
            AddNfo(0xC2, new PacketVersionEntry(version, true, 0x8000));
            #endregion

            #region 1.26.4b
            version = ClientVersion.Instantiate("1.26.4b");
            AddNfo(0xC3, new PacketVersionEntry(version, true, 0x8000));
            #endregion

            #region 1.26.4i
            version = ClientVersion.Instantiate("1.26.4i");
            // -Packet BC, new ClientPacketVersion(version, false, 0x2));
            AddNfo(0xBC, new PacketVersionEntry(version, false, 0x3));
            AddNfo(0xC4, new PacketVersionEntry(version, false, 0x6));
            #endregion

            #region 2.0.0g
            version = ClientVersion.Instantiate("2.0.0g");
            AddNfo(0xC5, new PacketVersionEntry(version, false, 0xCB));
            AddNfo(0xC6, new PacketVersionEntry(version, false, 0x1));
            #endregion

            #region 2.0.3b
            version = ClientVersion.Instantiate("2.0.3b");
            AddNfo(0xC7, new PacketVersionEntry(version, true, 0x8000));
            #endregion

            #region 2.0.4c
            version = ClientVersion.Instantiate("2.0.4c");
            AddNfo(0xC7, new PacketVersionEntry(version, false, 0x31));
            // -Packet C7, new ClientPacketVersion(version, true, 0x8000));
            AddNfo(0xC8, new PacketVersionEntry(version, false, 0x2));
            AddNfo(0xC9, new PacketVersionEntry(version, false, 0x6));
            AddNfo(0xCA, new PacketVersionEntry(version, false, 0x6));
            AddNfo(0xCB, new PacketVersionEntry(version, false, 0x7));
            AddNfo(0xCC, new PacketVersionEntry(version, true, 0x8000));
            #endregion

            #region 3.0.3a
            version = ClientVersion.Instantiate("3.0.3a");
            AddNfo(0xCD, new PacketVersionEntry(version, false, 0x1));
            AddNfo(0xCE, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xCF, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xD0, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xD1, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xD2, new PacketVersionEntry(version, false, 0x19));
            AddNfo(0xD3, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xD4, new PacketVersionEntry(version, true, 0x8000));
            #endregion

            #region 3.0.4m
            version = ClientVersion.Instantiate("3.0.4m");
            AddNfo(0xCF, new PacketVersionEntry(version, false, 0x4E));
            // -Packet CF, new ClientPacketVersion(version, true, 0x8000));
            AddNfo(0xD1, new PacketVersionEntry(version, false, 0x2));
            // -Packet D1, new ClientPacketVersion(version, true, 0x8000));
            #endregion

            #region 3.0.8z0
            version = ClientVersion.Instantiate("3.0.8z0");
            AddNfo(0xD5, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xD6, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xD7, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xD8, new PacketVersionEntry(version, true, 0x8000));
            #endregion

            #region 4.0.00o
            version = ClientVersion.Instantiate("4.0.00o");
            AddNfo(0xD9, new PacketVersionEntry(version, false, 0xCA));
            #endregion

            #region 4.0.00q
            version = ClientVersion.Instantiate("4.0.00q");
            AddNfo(0xDA, new PacketVersionEntry(version, true, 0x8000));
            #endregion

            #region 4.0.01a
            version = ClientVersion.Instantiate("4.0.01a");
            // -Packet D9, new ClientPacketVersion(version, false, 0xCA));
            AddNfo(0xD9, new PacketVersionEntry(version, false, 0x10C));
            #endregion

            #region 4.0.02a
            version = ClientVersion.Instantiate("4.0.02a");
            AddNfo(0xDB, new PacketVersionEntry(version, true, 0x8000));
            #endregion

            #region 4.0.04t
            version = ClientVersion.Instantiate("4.0.04t");
            AddNfo(0xDC, new PacketVersionEntry(version, false, 0x9));
            #endregion

            #region 4.0.07a
            version = ClientVersion.Instantiate("4.0.07a");
            // -Packet 0B, new ClientPacketVersion(version, false, 0x10A));
            AddNfo(0x0B, new PacketVersionEntry(version, false, 0x7));
            #endregion

            #region 4.0.10a
            version = ClientVersion.Instantiate("4.0.10a");
            // -Packet 16, new ClientPacketVersion(version, false, 0x1));
            AddNfo(0x16, new PacketVersionEntry(version, true, 0x8000));
            // -Packet 31, new ClientPacketVersion(version, false, 0x1));
            AddNfo(0x31, new PacketVersionEntry(version, true, 0x8000));
            #endregion

            #region 5.0.0a
            version = ClientVersion.Instantiate("5.0.0a");
            AddNfo(0xDD, new PacketVersionEntry(version, true, 0x8000));
            #endregion

            #region 5.0.2
            version = ClientVersion.Instantiate("5.0.2");
            AddNfo(0xDE, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xDF, new PacketVersionEntry(version, true, 0x8000));
            #endregion

            #region 5.0.9.0
            version = ClientVersion.Instantiate("5.0.9.0");
            AddNfo(0xE0, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xE1, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xE2, new PacketVersionEntry(version, false, 0xA));
            #endregion

            #region 6.0.01.03
            version = ClientVersion.Instantiate("6.0.01.03");
            AddNfo(0xE3, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xE4, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xE5, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xE6, new PacketVersionEntry(version, false, 0x5));
            AddNfo(0xE7, new PacketVersionEntry(version, false, 0xC));
            AddNfo(0xE8, new PacketVersionEntry(version, false, 0xD));
            AddNfo(0xE9, new PacketVersionEntry(version, false, 0x4B));
            AddNfo(0xEA, new PacketVersionEntry(version, false, 0x3));
            AddNfo(0xEB, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xEC, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xED, new PacketVersionEntry(version, true, 0x8000));
            #endregion

            #region 6.0.01.07
            version = ClientVersion.Instantiate("6.0.01.07");
            // -Packet 08, new ClientPacketVersion(version, false, 0xE));
            AddNfo(0x08, new PacketVersionEntry(version, false, 0xF));
            // -Packet 25, new ClientPacketVersion(version, false, 0x14));
            AddNfo(0x25, new PacketVersionEntry(version, false, 0x15));
            #endregion

            #region 6.0.06.0
            version = ClientVersion.Instantiate("6.0.06.0");
            AddNfo(0xEE, new PacketVersionEntry(version, false, 0x2000));
            AddNfo(0xEF, new PacketVersionEntry(version, false, 0x2000));
            AddNfo(0xF0, new PacketVersionEntry(version, true, 0x8000));
            AddNfo(0xF1, new PacketVersionEntry(version, false, 0x9));
            AddNfo(0xF2, new PacketVersionEntry(version, false, 0x19));
            #endregion

            #region 6.0.06.1
            version = ClientVersion.Instantiate("6.0.06.1");
            // -Packet EE, new ClientPacketVersion(version, false, 0x2000));
            // -Packet EF, new ClientPacketVersion(version, false, 0x2000));
            // -Packet F0, new ClientPacketVersion(version, true, 0x8000));
            // -Packet F1, new ClientPacketVersion(version, false, 0x9));
            // -Packet F2, new ClientPacketVersion(version, false, 0x19));
            #endregion

            #region 6.0.14.2
            version = ClientVersion.Instantiate("6.0.14.2");
            // -Packet B9, new ClientPacketVersion(version, false, 0x3));
            AddNfo(0xB9, new PacketVersionEntry(version, false, 0x5));
            #endregion

            #region 7.0.0.0
            version = ClientVersion.Instantiate("7.0.0.0");
            AddNfo(0xEE, new PacketVersionEntry(version, false, 0x2000));
            AddNfo(0xEF, new PacketVersionEntry(version, false, 0x2000));
            AddNfo(0xF0, new PacketVersionEntry(version, false, 0x2000));
            AddNfo(0xF1, new PacketVersionEntry(version, false, 0x2000));
            AddNfo(0xF2, new PacketVersionEntry(version, false, 0x2000));
            AddNfo(0xF3, new PacketVersionEntry(version, false, 0x18));
            #endregion
        }

        private static void AddNfo(byte packetid, PacketVersionEntry clientpacketversion)
        {
            if (!m_PacketNfo.ContainsKey(packetid))
                m_PacketNfo.Add(packetid, new PacketVersions(packetid));
            m_PacketNfo[packetid].Add(clientpacketversion);
        }

    }
}
