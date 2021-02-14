using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Sharpkick.Network
{
    /// <summary>
    /// A packet we construct to send to the client, usually to a specific client version. This has no underlying buffer on the server as it was not created by the server.
    /// </summary>
    /// <remarks>Name these classes with a post-pend of the client version they are for, example Packet3A_SkillInfo_1_25_36f</remarks>
    abstract class ServerPacketSafe : Packet, UODemo.IServerPacket
    {
        protected byte[] m_Data;
        public override byte[] Data { get { return m_Data; } }

        public ServerPacketSafe(byte PacketId, int length)
        {
            length = Math.Max(length, 1);
            m_Data = new byte[length];
            Id = m_Data[0] = PacketId;
            Length = length;
            if (Dynamic)
            {
                m_Data[1] = (byte)(length >> 8);
                m_Data[2] = (byte)length;
            }
        }

        /// <summary>
        /// Write a Unsigned Integer to the buffer
        /// </summary>
        /// <param name="start">Buffer index at which to write</param>
        /// <param name="value">The uint to write</param>
        /// <returns>bytes written, always 4</returns>
        public int WriteUInt(int start, uint value)
        {
            Data[start] = (byte)(value >> 24);
            Data[start + 1] = (byte)(value >> 16);
            Data[start + 2] = (byte)(value >> 8);
            Data[start + 3] = (byte)value;
            return 4;
        }

        /// <summary>
        /// Write a Unsigned Short to the buffer
        /// </summary>
        /// <param name="start">Buffer index at which to write</param>
        /// <param name="value">The ushort to write</param>
        /// <returns>bytes written, always 2</returns>
        public int WriteUShort(int start, ushort value)
        {
            Data[start] = (byte)(value >> 8);
            Data[start + 1] = (byte)value;
            return 2;
        }


        /// <summary>
        /// Write a Ascii string to the buffer
        /// </summary>
        /// <param name="start">Buffer index at which to write</param>
        /// <param name="value">The string to write</param>
        /// <returns>bytes written</returns>
        public int WriteAsciiNull(int start, string value)
        {
            int i = start;
            if (!string.IsNullOrEmpty(value))
            {
                ASCIIEncoding.ASCII.GetBytes(value, 0, value.Length, Data, i);
                i += ASCIIEncoding.ASCII.GetByteCount(value);
            }
            Data[i++] = 0x00;
            return i - start;
        }

        /// <summary>
        /// Write a BigEndianUnicode string to the buffer
        /// </summary>
        /// <param name="start">Buffer index at which to write</param>
        /// <param name="value">The string to write</param>
        /// <returns>bytes written</returns>
        public int WriteUnicodeNull(int start, string value)
        {
            int i = start;
            if (!string.IsNullOrEmpty(value))
            {
                Encoding.BigEndianUnicode.GetBytes(value, 0, value.Length, Data, i);
                i += Encoding.BigEndianUnicode.GetByteCount(value);
            }
            Data[i++] = 0x00;
            Data[i++] = 0x00;
            return i - start;
        }
    }

    /// <summary>
    /// A packet intercepted from the server to the client. This has an underlying data buffer on the server.
    /// </summary>
    unsafe abstract class ServerPacket : UnsafePacket
    {
        byte* m_Socket;
        private uint* m_pLength;
        private byte** m_ppData;

        // Called when the packet has been intercepted. Return false to remove the packet (not yet implemented)
        public abstract bool OnSending();
        public ServerPacket(byte* socket, byte** ppdata, uint* pLength) : base(*ppdata, *pLength) 
        { 
            m_Socket = socket;
            m_pLength = pLength;
            m_ppData = ppdata;
        }

        private UODemo.ISocket m_ClientSocket = null;
        protected UODemo.ISocket Socket { get { return m_ClientSocket ?? (m_ClientSocket = UODemo.Socket.Acquire(Server.Core, (struct_ServerSocket*)m_Socket)); /*ClientSocket(m_Socket))*/; } }

        public static ServerPacket Instantiate(byte* socket, byte** ppData, uint* pDataLen)
        {
            switch (**ppData)
            {
                case 0x1B: return new Packet1B_LoginConfirm(socket, ppData, pDataLen);
                case 0x3A: return new Packet3A_SkillInfo(socket, ppData, pDataLen);
                case 0x20: return new Packet20_MobileUpdate(socket, ppData, pDataLen);
                case 0x81: return new Packet81_OldCharList(socket, ppData, pDataLen);
                case 0x88: return new Packet88_OpenPaperDoll(socket, ppData, pDataLen);
                case 0x93: return new Packet93_BookHeaderOld(socket, ppData, pDataLen);
                default: return new Network.DefaultServerPacket(socket, ppData, pDataLen);
            }
        }

        protected void ReplacePacket(ServerPacketSafe newPacket)
        {
            unsafe
            {
                fixed (byte* pNewData = newPacket.Data)
                {
                    Server.PacketEngine.ReplaceServerPacketData(m_ppData, m_pLength, pNewData, newPacket.Dynamic ? (uint)((newPacket.Data[1] << 8) + newPacket.Data[2]) : (uint)newPacket.Length);
                }
            }
        }
    }

    class DefaultServerPacket : ServerPacket
    {
        public override bool Dynamic { get { return false; } }
        unsafe public DefaultServerPacket(byte* socket, byte** ppdata, uint* pDataLen) : base(socket, ppdata, pDataLen) { }
        public override bool OnSending() { return true; }

    }

    //0x1B Login Confirm: Char Location and body type (37 bytes) 
    //·        BYTE cmd 
    //·        BYTE[4] player id 
    //·        BYTE[4] unknown1 
    //·        BYTE[2] bodyType 
    //·        BYTE[2] xLoc 
    //·        BYTE[2] yLoc 
    //·        BYTE[2] zLoc 
    //·        BYTE direction 
    //·        BYTE[2] unknown2 
    //·        BYTE[4] unknown3 (usually has FF somewhere in it) 
    //·        BYTE[4] unknown4 
    //·        BYTE flag byte 
    //·        BYTE highlight color 
    //·        BYTE[7] unknown5 
    class Packet1B_LoginConfirm : ServerPacket
    {
        public override bool Dynamic { get { return false; } }

        public uint Serial { get { return ReadUInt(1); } }
        // unk 4
        public ushort Body { get { return ReadUShort(9); } }
        public short X { get { return ReadShort(11); } }
        public short Y { get { return ReadShort(13); } }
        public short Z { get { return ReadSByte(15); } }
        public Direction Direction { get { return (Direction)ReadByte(17); } }
        // unk 2
        // unk 4
        // unk 4
        public MobileFlags Flag { get { return (MobileFlags)ReadByte(27); } }
        public ushort Hue { get { return ReadUShort(38); } }
        // unk 7

        unsafe public Packet1B_LoginConfirm(byte* socket, byte** ppdata, uint* pLength) : base(socket, ppdata, pLength) { }

        public override bool OnSending()
        {
            Account acct=Accounting.Get((int)Socket.AccountNumber);
            if (acct != null)
            {
                acct.VerifyMobile(Serial);
                unsafe
                {
                    if (Socket.PlayerObject != null)
                    {
                        if (acct.HasAccess(AccountAccessFlags.Admin))
                        {
                            Server.MakeGameMaster((PlayerObject*)Socket.PlayerObject);
                            Player.SetPlayerFlag((class_Player*)Socket.PlayerObject, PlayerFlags.IsEditing); 
                            ConsoleUtils.PushColor(ConsoleColor.Red);
                            Console.WriteLine("GM Login.");
                            ConsoleUtils.PopColor();
                        }
                        else
                        {
                            Server.UnmakeGameMaster((PlayerObject*)Socket.PlayerObject);
                            Player.ClearPlayerFlag((class_Player*)Socket.PlayerObject, PlayerFlags.IsEditing | PlayerFlags.IsGod | PlayerFlags.IsGM);
                        }
                    }
                }
            }
            return true;
        }
    }

    //0x20 Draw Game Player (19 bytes) 
    //·        BYTE cmd 
    //·        BYTE[4] creature id 
    //·        BYTE[2] bodyType 
    //·        BYTE unknown1 (0) 
    //·        BYTE[2] skin color / hue 
    //·        BYTE flag byte 
    //·        BYTE[2] xLoc 
    //·        BYTE[2] yLoc 
    //·        BYTE[2] unknown2 (0)
    //·        BYTE direction 
    //·        BYTE zLoc 
    /// <summary>
    /// Send from server to client for information of the connected client character only.
    /// </summary>
    class Packet20_MobileUpdate : ServerPacket
    {
        public override bool Dynamic { get { return false; } }

        public uint Serial { get { return ReadUInt(1); } }
        public ushort Body { get { return ReadUShort(5); } }
        public ushort Hue { get { return ReadUShort(8); } }
        public MobileFlags Flag { get { return (MobileFlags)ReadByte(9); } }
        public short X { get { return ReadShort(10); } }
        public short Y { get { return ReadShort(12); } }
        public Direction Direction { get { return (Direction)ReadByte(14); } }
        public sbyte Z { get { return ReadSByte(15); } }

        unsafe public Packet20_MobileUpdate(byte* socket, byte** ppdata, uint* pLength) : base(socket, ppdata, pLength) { }

        public override bool OnSending()
        {
            return true;
        }
    }

    //0x3A: Send Skills (Variable) 
    //BYTE cmd 
    //BYTE[2] blockSize  = 1028
    //BYTE Type (0x00= full list, 0xFF = single skill update) 
    //Repeat next until done - 46 to 50 skills 
    //·         BYTE[2] id # of skill (0x01 - 0x2e) 
    //·         BYTE[2] skill Value * 10 
    //BYTE[2] null (00 00)  (ONLY IF TYPE == 0x00) 

    class Packet3A_SkillInfo : ServerPacket
    {
        public override bool Dynamic { get { return true; } }

        unsafe public Packet3A_SkillInfo(byte* socket, byte** ppdata, uint* pLength) : base(socket, ppdata, pLength) { }

        public override bool OnSending()
        {
            int i = 3;
            byte type = ReadByte(i++);
            if (type == 0xFF || type == 0x00)
            {
                if (Socket.Version >= ClientVersion.v1_25_36f)
                {
                    List<Tuple<ushort, ushort>> Skills = new List<Tuple<ushort, ushort>>();
                    bool single = type == 0xFF;
                    bool done = single;
                    ushort highest = 0;
                    do
                    {
                        ushort id = ReadUShort(i); i += 2;
                        if (id == 0x0000)
                            done = true;
                        else
                        {
                            ushort value = ReadUShort(i); i += 2;
                            Skills.Add(new Tuple<ushort, ushort>(id, value));
                        }
                        if (id > highest) highest = id;
                    } while (!done);

                    if(!single) // add skills for version 1.25.36f and up
                    {
                        while (highest < 49)
                            Skills.Add(new Tuple<ushort, ushort>(++highest, 0));
                    }

                    if (Socket.Version >= ClientVersion.v3_0_8d)
                        ReplacePacket(new Packet3A_SkillInfo_3_0_8d(Skills));   // new packet version includes Caps, Skill Locks, and Base value.
                    else if (Socket.Version >= ClientVersion.v1_26_2)
                        ReplacePacket(new Packet3A_SkillInfo_1_26_2(Skills));   // new packet version includes Skill Locks, and Base value.
                    else
                        ReplacePacket(new Packet3A_SkillInfo_1_25_36f(Skills)); // Only adds new skills
                }
            }
            return true;
        }
    }

    // rebuild for adding skills
    class Packet3A_SkillInfo_1_25_36f : ServerPacketSafe
    {
        public override bool Dynamic { get { return true; } }

        public Packet3A_SkillInfo_1_25_36f(List<Tuple<ushort, ushort>> Skills) : base(0x3A, Skills.Count > 1 ? 400 : 40)
        {
            int i = 3;
            bool single = Skills.Count == 1;
            m_Data[i++] = single ? (byte)0xFF : (byte)0x00;
            foreach (Tuple<ushort, ushort> skill in Skills)
            {
                i += WriteUShort(i, skill.Item1); // id
                i += WriteUShort(i, skill.Item2); // value
            }
            if (!single)
            {   // end
                i += WriteUShort(i, 0); // id
            }
            WriteUShort(1, (ushort)i);  // block length
        }
    }

    // 0x03 Server Version - Send Skills (Variable)  Client Ver 1.26.2
    //BYTE cmd
    //BYTE[2] blockSize
    //BYTE Type (0x00= full list, 0xFF = single skill update)
    //Repeat next until done - 46 skills
    //·         BYTE[2] id # of skill (0x01 - 0x2e)
    //·         BYTE[2] skill Value * 10
    //·         BYTE[2] Unmodified Value * 10
    //·         BYTE skillLock (0=up, 1=down, 2=locked)
    //BYTE[2] null (00 00)  (ONLY IF TYPE == 0x00)
    class Packet3A_SkillInfo_1_26_2 : ServerPacketSafe
    {
        public override bool Dynamic { get { return true; } }

        public Packet3A_SkillInfo_1_26_2(List<Tuple<ushort, ushort>> Skills) : base(0x3A, Skills.Count > 1 ? 400 : 40)
        {
            int i = 3;
            bool single = Skills.Count == 1;
            m_Data[i++] = single ? (byte)0xFF : (byte)0x00;
            foreach (Tuple<ushort, ushort> skill in Skills)
            {
                i += WriteUShort(i, skill.Item1); // id
                i += WriteUShort(i, skill.Item2); // value
                i += WriteUShort(i, skill.Item2); // base
                m_Data[i++] = 2;
            }
            if (!single)
            {   // end
                i += WriteUShort(i, 0); // id
            }
            WriteUShort(1, (ushort)i);  // block length
        }
    }

    // 0x03 Server Version - Send Skills (Variable)  Client Ver 3.0.8d Date: May 28, 2002
    //Server Version: 
    //BYTE[1] cmd 
    //BYTE[2] blockSize 
    //BYTE[1] Type (0x00= full list, 0xFF = single skill update, 0x02 full list with skillcap, 0xDF single skill update with cap) Repeat next until done - 46 skills 
    //BYTE[2] id # of skill (0x01 - 0x2e) 
    //BYTE[2] skill Value * 10 
    //BYTE[2] Unmodified Value * 10 
    //BYTE skillLock (0=up, 1=down, 2=locked) 
    //If (Type==2 || Type==0xDF) 
    //· BYTE[2] SkillCap 
    //BYTE[2] null (00 00) (ONLY IF TYPE == 0x00) 

    class Packet3A_SkillInfo_3_0_8d : ServerPacketSafe
    {
        public override bool Dynamic { get { return true; } }

        public Packet3A_SkillInfo_3_0_8d(List<Tuple<ushort, ushort>> Skills)
            : base(0x3A, Skills.Count > 1 ? 500 : 50)
        {
            int i = 3;
            bool single = Skills.Count == 1;
            m_Data[i++] = single ? (byte)0xDF : (byte)0x02;
            foreach (Tuple<ushort, ushort> skill in Skills)
            {
                i += WriteUShort(i, skill.Item1); // id
                i += WriteUShort(i, skill.Item2); // value
                i += WriteUShort(i, skill.Item2); // base
                m_Data[i++] = 2;                    // lock
                i += WriteUShort(i, 1000);        // cap
            }
            if (!single)
            {   // end
                i += WriteUShort(i, 0); // id
            }
            WriteUShort(1, (ushort)i);  // block length
        }
    }

    class Packet81_OldCharList : ServerPacket
    {
        public override bool Dynamic { get { return true; } }

        public int numChars { get; private set; }

        List<CharInfo> Chars;

        struct ServerInfo
        {
            public ServerInfo(int id, string name) { Id = id; Name = name; }
            public int Id;
            public string Name;
        }

        public struct LocationInfo
        {
            public LocationInfo(int id, string city, string name) { Id = id; City = city; Name = name; }
            public int Id;
            public string City;
            public string Name;
        }
        public static List<LocationInfo> Locations=null;

        static List<ServerInfo> Servers = new List<ServerInfo>(new ServerInfo[] { new ServerInfo(0, "Test 1"), new ServerInfo(1, "Test 2") });

        unsafe public Packet81_OldCharList(byte* socket, byte** ppdata, uint* pLength) : base(socket, ppdata, pLength)
        {
            byte* data = *ppdata;
            if (Id != 0x81) Console.WriteLine("ERROR: Data is not a valid 0x81 packet."); // just for testing
            numChars = data[3];
            //unk_205=Data[4];
            Chars = new List<CharInfo>(5);
            for (int i=0; i < 5;i++)
                Chars.Add(new CharInfo(ReadAsciiStringFixed((60 * i) + 5, 30), ReadAsciiStringFixed((60 * i) + 35, 30)));
            int p = 60 * 5 + 5; // position
            int numServers = data[p++];
            for (int i = 0; i < numServers; i++)
            {
                byte id = data[p++];
                string name = ReadAsciiStringFixed(p, 0x10); p += 0x10;
            }
            int numLocations = data[p++];
            if (Locations == null)
            {   // we'll capture this the first time.
                Locations = new List<LocationInfo>();
                for (int i = 0; i < numLocations; i++)
                {
                    byte id = data[p++];
                    string city = ReadAsciiStringFixed(p, 0x1F); p += 0x1F;
                    string name = ReadAsciiStringFixed(p, 0x1F); p += 0x1F;
                    Locations.Add(new LocationInfo(id, city, name));
                }
            }
        }

        public override bool OnSending()
        {
            Console.WriteLine("Detected client version: {0}", ClientVersion.ToString(Socket.Version));
            if (Socket.Version >= ClientVersion.v1_26_4i)
            {
                ReplacePacket(new PacketA9_CharList_1_25_35(Chars));
            }

            return true;
        }
    }

    

    //0x88 Open Paperdoll (66 bytes) 
    //·        BYTE cmd 
    //·        BYTE[4] charid 
    //·        BYTE[60] text 
    //·        BYTE flag byte
    class Packet88_OpenPaperDoll : ServerPacket
    {
        public override bool Dynamic { get { return false; } }

        public uint Serial { get { return ReadUInt(1); } }
        public string Name { get { return ReadAsciiStringFixed(5, 60); } }
        public MobileFlags Flag { get { return (MobileFlags)ReadByte(65); } }

        unsafe public Packet88_OpenPaperDoll(byte* socket, byte** ppdata, uint* pLength) : base(socket, ppdata, pLength) { }

        public override bool OnSending()
        {
            if (Name != null)
            {
                string[] namea = Name.Split(','); // removing title
                if (namea.Length >= 1)
                {
                    Mobile.UpdateMobileName(Serial, namea[0]);
                    if (namea.Length >= 2)
                        Mobile.UpdateMobileTitle(Serial, namea[1].TrimStart());
                }
            }
            return true;
        }
    }

    class Packet93_BookHeaderOld : ServerPacket
    {
        public override bool Dynamic { get { return false; } }
        unsafe public Packet93_BookHeaderOld(byte* socket, byte** ppdata, uint* pLength) : base(socket, ppdata, pLength) { }

        public override bool OnSending()
        {
            if (Socket.Version >= ClientVersion.v1_26_0)
            {
                int i = 1;
                uint serial = ReadUInt(i); i += 4;
                BookWriteableFlag writable = (BookWriteableFlag)ReadByte(i); i++;
                ushort numPages = ReadUShort(i); i += 2;
                string title = ReadAsciiStringFixed(i, 60); i += 60;
                string author = ReadAsciiStringFixed(i, 30); i += 30;
                ReplacePacket(new Packet93_BookHeaderOld_1_26_0(serial, writable, numPages, title, author));
            }
            return true;
        }
    }

    class Packet93_BookHeaderOld_1_26_0 : ServerPacketSafe
    {
        public override bool Dynamic { get { return false; } }
        public Packet93_BookHeaderOld_1_26_0(uint BookSerial, BookWriteableFlag writable, ushort numPages, string title, string author) : base(0x93, 99)
        {
            int i = 1;
            i += WriteUInt(i, BookSerial);
            m_Data[i++] = (byte)writable;
            m_Data[i++] = 0x01; // unknown (this is the extra byte for 1.26.0 I think)
            i += WriteUShort(i, numPages);
            WriteAsciiNull(i, title.Length <= 60 ? title : title.Substring(0, 59)); i += 60;
            WriteAsciiNull(i, author.Length <= 30 ? author : author.Substring(0, 29)); i += 30;
        }
    }

    class Packet26_KickPlayer_1_23_0 : ServerPacketSafe
    {
        public override bool Dynamic { get { return false; } }
        public Packet26_KickPlayer_1_23_0(uint gmserial) : base(0xA8, 5)
        {
            WriteUInt(1, gmserial);
        }
    }

    //0xA8 Game Server List (Variable # of bytes) - Not used
    //BYTE cmd 
    //BYTE[2] blockSize 
    //BYTE System Info Flag 
    //·         0xCC - Don't send 
    //·         0x64 - Send Video card 
    //·         ?? - 
    //BYTE[2] # of servers 
    //Then each server -- 
    //    ·         BYTE[2] serverIndex (0-based) 
    //    ·         BYTE[32] serverName 
    //    ·         BYTE percentFull 
    //    ·         BYTE timezone 
    //    ·         BYTE[4] pingIP 
    class PacketA8_GameServerList_1_25_35 : ServerPacketSafe
    {
        private struct ServerEntry
        {
            public ServerEntry(string name, byte percentfull, sbyte timezone, IPAddress ipaddress)
            {
                Name = name;
                percentFull = percentfull;
                timeZone = timezone;
                IPAddress = ipaddress;
            }
            public string Name;
            public byte percentFull;
            public sbyte timeZone;
            public IPAddress IPAddress;
        }

        private static List<ServerEntry> Servers = new List<ServerEntry>(
            new ServerEntry[] { new ServerEntry("UO98", 0, -5, new IPAddress(new byte[] { 192, 168, 42, 119 }))
            });

        public override bool Dynamic { get { return true; } }
        public PacketA8_GameServerList_1_25_35() : base(0xA8, 6 + Servers.Count * 40)
        {
            int i = 3;
            m_Data[i++] = 0x5D;        // 0xCC - Don't send 
            i += WriteUShort(i, (ushort)Servers.Count);
            for(ushort servernum = 0;servernum<Servers.Count;servernum++)
            {
                ServerEntry server=Servers[servernum];
                i += WriteUShort(i, servernum);
                WriteAsciiNull(i, server.Name); i += 32;
                m_Data[i++] = server.percentFull;
                m_Data[i++] = (byte)server.timeZone;
                byte[] ipbytes = server.IPAddress.GetAddressBytes();
                m_Data[i++] = ipbytes[0];
                m_Data[i++] = ipbytes[1];
                m_Data[i++] = ipbytes[2];
                m_Data[i++] = ipbytes[3];
            }
        }
    }


    //0xA9 Characters / Starting Locations (Variable # of bytes) 
    //BYTE cmd 
    //BYTE[2] blockSize 
    //BYTE # of characters 
    //Following repeated 5 times 
    //    ·         BYTE[30] character name 
    //    ·         BYTE[30] character password 
    //BYTE number of starting locations 
    //Following for as many locations as you have 
    //    ·         BYTE locationIndex (0-based) 
    //    ·         BYTE[31] town (general name) 
    //    ·         BYTE[31] exact name 
    class PacketA9_CharList_1_25_35 : ServerPacketSafe
    {
        public override bool Dynamic { get { return true; } }

        public PacketA9_CharList_1_25_35(List<CharInfo> chars) : base(0xA9, 4 + 5 * 60 + 1 + Packet81_OldCharList.Locations.Count * 63)
        {
            int i = 3;
            m_Data[i++] = (byte)chars.Count;
            for (int j = 0; j < 5; j++)
            {
                WriteAsciiNull(i, (j >= chars.Count) ? string.Empty : chars[j].CharName); i += 30;
                WriteAsciiNull(i, (j >= chars.Count) ? string.Empty : chars[j].Password); i += 30;
            }
            m_Data[i++] = (byte)Packet81_OldCharList.Locations.Count;
            for (int j = 0; j < Packet81_OldCharList.Locations.Count; j++)
            {
                m_Data[i++] = (byte)Packet81_OldCharList.Locations[j].Id;
                WriteAsciiNull(i, Packet81_OldCharList.Locations[j].City); i += 31;
                WriteAsciiNull(i, Packet81_OldCharList.Locations[j].Name); i += 31;
            }
        }
    }


    //0x82 Login Denied (2 bytes) 
    //·        BYTE cmd 
    //·        BYTE why 
    //    ·        0x00 = unknown user 
    //    ·        0x01 = account already in use 
    //    ·        0x02 = account disabled 
    //    ·        0x03 = password bad 
    //    ·        0x04 and higher = communications failed 
    class Packet82_LoginDenied_1_23_0 : ServerPacketSafe
    {
        public override bool Dynamic { get { return false; } }

        public Packet82_LoginDenied_1_23_0(ALRReason reason) : base(0x82, 2)
        {
            m_Data[1] = (byte)reason;
        }
    }

    //0x8C Connect to Game Server (11 bytes)  - Not used
    //·        BYTE cmd 
    //·        BYTE[4] gameServer IP 
    //·        BYTE[2] gameServer port 
    //·        BYTE[4] new key 
    class Packet8C_ConnectToGameServer_1_23_0 : ServerPacketSafe
    {
        static IPAddress ip = IPAddress.Parse("192.168.42.119");
        static ushort Port = 10016;

        public override bool Dynamic { get { return false; } }

        public Packet8C_ConnectToGameServer_1_23_0() : base(0x8C, 11)
        {
#pragma warning disable 618
            uint addr = (uint)ip.Address;
#pragma warning restore 618
            WriteUInt(1, addr);
            WriteUShort(5, Port);
            m_Data[7]=(byte)addr;
            m_Data[8] = (byte)(addr >> 8);
            m_Data[9] = (byte)(addr >> 16);
            m_Data[10] = (byte)(addr >> 24);
        }


    }
    //0xB8 Char Profile (Variable # of bytes) [Apr-19-1999]
    //·        BYTE cmd 
    //·        BYTE[2] blockSize
    //·        BYTE[4] id
    //·        BYTE[?] character name (not unicode, null terminated.)
    //·        BYTE[2] (0x0000) (a non-unicode title string?)
    //·        BYTE[?][2] profile (in unicode, ? can be 0)
    //·        BYTE[2] (0x0000)
    //·        BYTE[2] terminator (0x3300)
    class PacketB8_ProfileResponse_1_25_35 : ServerPacketSafe
    {
        public override bool Dynamic { get { return true; } }
        public PacketB8_ProfileResponse_1_25_35(uint Serial, string Name, string Profile)
            : base(0xB8, 7 + ASCIIEncoding.ASCII.GetByteCount(Name) + 1 + 2 + Encoding.BigEndianUnicode.GetByteCount(Profile) + 2 + 2)
        {
            int i = 3;
            i += WriteUInt(i, Serial);
            i += WriteAsciiNull(i, Name);
            m_Data[i++] = 0x00;
            m_Data[i++] = 0x00;
            i += WriteUnicodeNull(i, Profile);
            m_Data[i++] = 0x33;
            m_Data[i++] = 0x00;
        }
    }

    class PacketBF_0006_Map_Fel_2_0_0 : ServerPacketSafe
    {
        public override bool Dynamic { get { return true; } }
        public PacketBF_0006_Map_Fel_2_0_0()
            : base(0xBF, 6)
        {
            m_Data[3] = 0x00;
            m_Data[4] = 0x08;
            m_Data[5] = 0x00;
        }
    }

}
