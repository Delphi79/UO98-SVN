using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;

namespace Sharpkick.Network
{
    class Packet00_CreateChar : ClientPacketSafe
    {

        /// <summary>
        /// Packet 0x00: Create Character
        /// </summary>
        /// <param name="packet">The Packet from which to construct this safe wrapper</param>
        public Packet00_CreateChar(ClientPacket packet) : base(packet) { }

        public ushort StartX { get { return (ushort)((Packet[5] << 8) + Packet[6]); } set { Packet[5] = (byte)(value >> 8); Packet[6] = (byte)value; } }
        public ushort StartY { get { return (ushort)((Packet[7] << 8) + Packet[8]); } set { Packet[7] = (byte)(value >> 8); Packet[8] = (byte)value; } }
        public byte   StartZ { get { return Packet[9]; } set { Packet[9] = value; } }

        public byte Female      { get { return Packet[70]; } set { Packet[70]=value; } }
        public byte Str         { get { return Packet[71]; } set { Packet[71]=value; } }
        public byte Dex         { get { return Packet[72]; } set { Packet[72]=value; } }
        public byte Int         { get { return Packet[73]; } set { Packet[73]=value; } }
        public byte Skill1      { get { return Packet[74]; } set { Packet[74]=value; } }
        public byte Skill1val   { get { return Packet[75]; } set { Packet[75]=value; } }
        public byte Skill2      { get { return Packet[76]; } set { Packet[76]=value; } }
        public byte Skill2val   { get { return Packet[77]; } set { Packet[77]=value; } }
        public byte Skill3      { get { return Packet[78]; } set { Packet[78]=value; } }
        public byte Skill3val   { get { return Packet[79]; } set { Packet[79]=value; } }

        public override bool OnReceived()
        {
            if (ClientVersion >= Network.ClientVersion.v1_26_0)
                Packet.Truncate(4); // remove the "BYTE[2] shirt color" and "BYTE[2] pants color" added in version 1.26.0

            // Fix invalid skills

            int SkillCount = Server.SkillsObject.SkillCount;

            while (Skill1 == Skill2 || Skill1 == Skill3 || Skill1 < 0 || Skill1 >= SkillCount)
                if (--Skill1 < 0) Skill1 = (byte)(Server.SkillsObject.SkillCount - 1);
            while (Skill2 == Skill1 || Skill2 == Skill3 || Skill2 < 0 || Skill2 >= SkillCount)
                if (--Skill2 < 0) Skill2 = (byte)(Server.SkillsObject.SkillCount - 1);
            while (Skill3 == Skill1 || Skill3 == Skill2 || Skill3 < 0 || Skill3 >= SkillCount)
                if (--Skill3 < 0) Skill3 = (byte)(SkillCount - 1);

            return true;
        }
    }

    class Packet02_MoveRequest : ClientPacketSafe
    {

        /// <summary>
        /// Packet 0x02: Move Request
        /// </summary>
        /// <param name="packet">The Packet from which to construct this safe wrapper</param>
        public Packet02_MoveRequest(ClientPacket packet) : base(packet) { }

        public override bool OnReceived()
        {
            if (ClientVersion > Network.ClientVersion.v1_26_0)
                Packet.Truncate(4); // remove the "BYTE[4] fastwalk prevention key" added in version 1.26.0
            
            return true;
        }
    }

    class Packet12_TextCommand : ClientPacketSafe
    {

        public enum Type : byte
        {
            Goto = 0x00,
            God = 0x01,
            Follow = 0x02,
            Page = 0x03,
            Load = 0x04,
            Bug = 0x05,
            Ignore = 0x06,
            Backup = 0x07,
            ValidateDB = 0x0F,
            CompressDB = 0x10,
            PList = 0x11,
            SaveChunk = 0x22,
            LoadChunk = 0x23,
            UseSkill = 0x24,
            CastSpell = 0x27,
            unk2F = 0x2F,
            Set = 0x37,      // (unused)
            unk42 = 0x42,
            OpenSpellbook = 0x43,
            MacroSpell = 0x56,
            uused57 = 0x57,  // unused
            OpenDoor = 0x58,
            unused5E = 0x5E, // unused
            unused68 = 0x68, // unused
            unused69 = 0x69, // unused
            unused6A = 0x6A, // unused
            GMCommand = 0x6B,
            Action = 0xC7,
            unusedD2 = 0xD2, // unused
            NextCall = 0xDA,
            unusedEF = 0xEF, // unused
            InvokeVirtues = 0xF4,

        }

        public Type CommandType { get { return (Type)Packet[3]; } }
        public string CommandText { get { return Packet.ReadAsciiStringNull(4); } }

        /// <summary>
        /// Packet 0x02: Move Request
        /// </summary>
        /// <param name="packet">The Packet from which to construct this safe wrapper</param>
        public Packet12_TextCommand(ClientPacket packet) : base(packet) { }

        public override bool OnReceived()
        {
            switch (CommandType)
            {
                case Type.UseSkill:
                case Type.CastSpell:
                case Type.OpenSpellbook:
                case Type.MacroSpell:
                case Type.OpenDoor:
                case Type.Action:
                    return true;
                case Type.InvokeVirtues:
                    return false;
                default:
                    ConsoleUtils.PushColor(ConsoleColor.Red);
                    Console.Write("Received 0x12 TextCommand type {0}: ", CommandType);
                    bool ok = Accounting.HasAccess(Packet.AccountNumber, AccountAccessFlags.Editor);
                    Console.WriteLine(ok ? "OK." : "Access Denied.");
                    if (ok && CommandType == Type.GMCommand)
                    {
                        Console.WriteLine("GM Command: {0}", CommandText);
                        Administration.GMCommand command = Administration.GMCommand.Instantiate((int)Packet.Player.Serial, CommandText);
                        command.Execute();
                    }
                    ConsoleUtils.PopColor();
                    return ok;
            }
        }
    }


    class Packet3A_SetSkillLock : ClientPacketSafe
    {

        /// <summary>
        /// Packet 0x3A: Set Skill Lock
        /// </summary>
        /// <param name="packet">The Packet from which to construct this safe wrapper</param>
        public Packet3A_SetSkillLock(ClientPacket packet) : base(packet) { }

        public override bool OnReceived()
        {
            Packet.SendSystemMessage("Skill locks are not implemented.");
            return false;
        }
    }


    class Packet93_BookHeaderChange : ClientPacketSafe
    {
        public Packet93_BookHeaderChange(ClientPacket packet) : base(packet) { }

        public override bool OnReceived()
        {
            if (ClientVersion > Network.ClientVersion.v1_26_0) // remove one byte
            {
                byte[] newData = new byte[98];
                for (uint i = 0; i < 6; i++)
                    newData[i] = Packet[i];
                for (uint i = 7; i < 98; i++)
                    newData[i-1] = Packet[i];
                Packet.Replace(newData, 99);
            }
            return true;
        }
    }

    class PacketAD_UnicodeSpeech : ClientPacketSafe
    {
        /// <summary>
        /// Packet 0x03: ASCII Speech
        /// </summary>
        /// <param name="packet">The Packet from which to construct this safe wrapper</param>
        public PacketAD_UnicodeSpeech(ClientPacket packet) : base(packet) { }

        public override bool OnReceived()
        { // Unfinished, for now UODemoDLL handles this
            //int i = 1;
            //int len = Packet.ReadUShort(i); i += 2;
            //byte type = Packet.ReadByte(i++);
            //int hue = Packet.ReadUShort(i); i += 2;
            //i += 2; // font
            //i += 4; // lang

            //if (type == 0xC0) // skip over the keywords
            //{
            //    int value = 0;
            //    int count = 0;
            //    int hold = 0;


            //    value = (Packet.ReadByte(i++) << 8) + Packet.ReadByte(i++);
            //    count = (value & 0xFFF0) >> 4;
            //    hold = value & 0xF;

            //    for (i = 0; i < count; ++i)
            //    {
            //        if ((i & 1) == 0)
            //        {
            //            hold <<= 8;
            //            hold |= Packet.ReadByte(i++);
            //            hold = 0;
            //        }
            //        else
            //        {
            //            value = Packet.ReadUShort(i); i += 2;
            //            hold = value & 0xF;
            //        }
            //    }
            //}

            //int strlen = len - i;

            

            return true;
        }
    }

    class PacketB8_ProfileReq : ClientPacketSafe    // Client version 1.25.35
    {
        public enum ReqMode : byte
        {
            Display=0x00,
            Edit=0x01,
        }

        public ReqMode Mode { get { return (ReqMode)Packet[3]; } }
        public uint Serial { get { return Packet.ReadUInt(4); } }

        public PacketB8_ProfileReq(ClientPacket packet) : base(packet) { }

        public override bool OnReceived()
        {
            Print();
            Mobile subjectMobile=Mobile.Get(Serial);
            if (subjectMobile != null)
            {
                if (Mode == ReqMode.Display)
                {
                    Packet.SendPacketToSocket(new PacketB8_ProfileResponse_1_25_35(Serial, subjectMobile.Name ?? "Unknown", subjectMobile.Profile ?? string.Empty));
                    if (Packet.IsGM)
                        unsafe
                        {
                            Server.OpenBank((int)Serial, (class_Player*)Packet.PlayerPtr);
                        }
                }
                else if (Mode == ReqMode.Edit && Packet.ReadUShort(8) == 0x0001 && subjectMobile.AccountNumber == Packet.AccountNumber) // BYTE[2] cmdType (0x0001 – Update)
                {
                    ushort len = Packet.ReadUShort(10);
                    string profileText = Packet.ReadUniStringFixed(12, len);
                    Mobile.UpdateMobileProfile(subjectMobile.Serial, profileText);
                }
            }

            return false;
        }

        public override void Print()
        {
            Console.WriteLine("ProfileRequest: Serial:{0:X4} ReqMode:{1}", Serial, Mode);
        }


    }

    class PacketBD_ClientVersion : ClientPacketSafe
    {
        /// <summary>
        /// Packet 0xBD: Client Version
        /// </summary>
        /// <param name="packet">The Packet from which to construct this safe wrapper</param>
        public PacketBD_ClientVersion(ClientPacket packet) : base(packet) { }

        string m_VersionString = null;
        public string VersionString { get { return m_VersionString ?? (m_VersionString = Packet.ReadAsciiStringNull(3)); } }

        public override bool OnReceived()
        {
            Console.WriteLine("Received ClientVersion: \"{0}\" {1}.{2}.{3}.{4}", VersionString, Version.Major, Version.Minor, Version.Build, Version.Revision);
            Packet.SetClientVersion(Version.Version);
            if (Version >= Network.ClientVersion.v2_0_0)
            {
                Packet.SendPacketToSocket(new PacketBF_0006_Map_Fel_2_0_0());
            }

            return false; // Unknown to server, remove it
        }

        ClientVersion ver;
        public ClientVersion Version
        {
            get { return ver ?? (ver = Network.ClientVersion.Instantiate(VersionString)); }
        }
    }

    /// <summary>
    /// Ultima Messenger packet - Unused.
    /// </summary>
    class PacketBB_UltimaMessenger : InvalidPacket
    {
        public PacketBB_UltimaMessenger(ClientPacket packet) : base(packet) { }
    }

    /// <summary>
    /// Select Server packet - Unused.
    /// </summary>
    class PacketA0_SelectServer : ClientPacketSafe
    {
        public PacketA0_SelectServer(ClientPacket packet) : base(packet) { }

        public override bool OnReceived()
        {
            // we should never see this packet, I think kick will send a D/C, not sure.
            Packet.SendPacketToSocket(new Packet26_KickPlayer_1_23_0(0x0000));

            return false; // Unknown to server, remove it
        }

    }

    /// <summary>
    /// Play Server packet. We will treat this as a Login packet.
    /// </summary>
    class Packet91_PlayServer : ClientPacketSafe
    {
        public Packet91_PlayServer(ClientPacket packet) : base(packet) { }

        public override bool OnReceived()
        {
            byte[] newData = new byte[62];
            newData[0] = 0x80;
            for (uint i = 1; i < 62; i++)
                newData[i] = Packet[i+4];
            newData[61] = 0x5D;
            Packet.Replace(newData, 65);
            return new Packet80_LoginRequest(Packet).OnReceived();
        }
    }

    class Packet80_LoginRequest : ClientPacketSafe
    {

        /// <summary>
        /// Packet 0x80: Login request
        /// </summary>
        /// <param name="packet">The Packet from which to construct this safe wrapper</param>
        public Packet80_LoginRequest(ClientPacket packet) : base(packet) { }

        /// <summary>
        /// The Username contained in this packet
        /// </summary>
        public string Username
        {
            get { return Packet.ReadAsciiStringFixed(1, 30); }
            set { Packet.WriteAsciiStringFixed(1, 30, value); }
        }

        /// <summary>
        /// The Password contained in this packet
        /// </summary>
        public string Password
        {
            get { return Packet.ReadAsciiStringFixed(31, 30); }
            set { Packet.WriteAsciiStringFixed(31, 30, value); }
        }

        public override void Print()
        {
            Console.WriteLine("LoginRequest: User:{0} Using Pass:{1}", Username, string.IsNullOrEmpty(Password) ? "no" : "yes");
        }

        /// <summary>
        /// Fires event to process login, or if from Local client cleans up the account name and allow direct access to any account by number.
        /// </summary>
        public override bool OnReceived()
        {
            if (IsLocal)
                Console.Write("LOCAL ");

            Print();

            LoginEventArgs args = new LoginEventArgs(Username, Password, Packet.IPAddress);
            if (IsLocal)
            {
                // Local only accepts account id's, passwords not allowed.
                Password = string.Empty;

                int accountid;
                if (Username == string.Empty) return true;       // All set, logging into default account.
                if (!int.TryParse(Username, out accountid))   // Get the account id, if not a number, set to default.
                {
                    Console.WriteLine("- invalid username ({0}) removed.", Username);
                    Username = string.Empty;
                    return true;
                }
                // Allow login to any account number from local.
                args.AccountID = accountid;
                args.Handled = true;
                args.Accepted = true;
            }
            else
                EventSink.InvokeOnLogin(args);
            if (args.Handled)
            {
                if (args.Accepted)
                {
                    Username = args.AccountID.ToString();
                    Password = string.Empty;
                    Console.WriteLine(" Accepted. AccountID:{0}", args.AccountID);
                }
                else
                {
                    Packet.SendPacketToSocket(new Packet82_LoginDenied_1_23_0(args.RejectedReason));

                    Username = "denied";
                    Password = string.Empty;
                    Console.WriteLine(" Rejected. Reason: {0}", args.RejectedReason);
                    return false;   // Remove the packet
                }
            }
            else
                Console.WriteLine("- Not Handled!");

            return true;
        }
    }

    class InvalidPacket : ClientPacketSafe
    {
        /// <summary>
        /// A packet which removes itself
        /// </summary>
        /// <param name="packet">The Packet from which to construct this safe wrapper</param>
        public InvalidPacket(ClientPacket packet) : base(packet) { }
        public override bool OnReceived()
        {
            return false;  // Remove packet
        }
    }

    class DefaultPacket : ClientPacketSafe
    {
        /// <summary>
        /// Default SafePacket wrapper
        /// </summary>
        /// <param name="packet">The Packet from which to construct this safe wrapper</param>
        public DefaultPacket(ClientPacket packet) : base(packet) { }
    }

    /// <summary>
    /// Provides a wrapper for the unsafe client packet class
    /// </summary>
    abstract class ClientPacketSafe : IDisposable
    {
        protected ClientPacket Packet;
        protected ClientVersionStruct ClientVersion { get { return Packet.ClientVersion; } }

        public byte this[uint index]
        {
            get { return Packet[index]; }
        }

        public byte Id { get { return Packet.Id; } }
        public int Length { get { return Packet.Length; } }
        public bool Dynamic { get { return Packet.Dynamic; } }

        public bool IsLocal { get { return Packet.IsLocal; } }

        public ClientPacketSafe(ClientPacket packet)
        {
            Packet = packet;
        }

        public virtual void Print()
        {
            Console.Write("{0} ", this.GetType().Name);
            Packet.Print();
        }

        /// <summary>
        /// Packet's on received handler
        /// </summary>
        /// <returns>A value of false is a request to Remove the packet from the buffer</returns>
        public virtual bool OnReceived()
        {
            return true; // Default behavior, let packet through.
        }

        public bool Removed { get { return Packet.Removed; } }
        public void Remove() { Packet.Remove(); }

        #region IDisposable Members

        public void Dispose()
        {
            if (Packet != null)
                Packet.Dispose();
            Packet = null;
        }

        #endregion
    }

    /// <summary>
    /// An unsafe received Packet class allowing direct manipulation of the underlying packet. This should be wrapped by a SafePacket class
    /// </summary>
    unsafe sealed class ClientPacket : UnsafePacket, IDisposable
    {
        public int SendPacketToSocket(ServerPacketSafe packet)
        {
            return Socket.SendPacket(packet.Data, (uint)packet.Length);
        }

        public bool Removed { get; private set; }
        public void Remove()
        {
            if (Removed) return;
            Socket.RemoveFirstPacket(Length);
            Removed = true;
        }

        public static ClientPacketSafe Instantiate(byte* pSocket, byte PacketID, uint PacketSize, bool IsPacketDynamicSized)
        {
            UODemo.ISocket socket = UODemo.Socket.Acquire(Server.Core,(struct_ServerSocket*)pSocket); //ClientSocket(pSocket);

            // Check for god mode on each packet.
            if (socket.PlayerObject != null && (socket.IsGod || socket.IsEditing) && !socket.VerifyGod)
            {
                socket.IsGod = false;
                socket.IsEditing = false;
                ConsoleUtils.PushColor(ConsoleColor.Red);
                Console.WriteLine("God/Edit Mode Disabled! Unauthorized.");
                ConsoleUtils.PopColor();
            }

            ConsoleUtils.PushColor(ConsoleColor.Yellow);
            if(MyServerConfig.PacketDebug) Console.WriteLine("Player Flags: {0}", socket.PlayerFlags);
            ConsoleUtils.PopColor();


            if (IsPacketDynamicSized && (PacketSize == 0x8000 || PacketSize == 0))
                PacketSize = ((uint)socket.Data[1] << 8) + socket.Data[2];

            if (PacketSize >= (IsPacketDynamicSized ? 3 : 1))
            {
                // TODO: These objects should be pooled.
                ClientPacket packet = new ClientPacket(socket, PacketID, PacketSize, IsPacketDynamicSized);

                if (Network.GodPackets.IsRestrictedGodPacket(PacketID))
                {
                    ConsoleUtils.PushColor(ConsoleColor.Red);
                    Console.Write("Received God Packet {0:X2}: ", PacketID);
                    try
                    {
                        if (!socket.IsGod)
                        {
                            Console.WriteLine("Removed: Unauthorized");
                            return new InvalidPacket(packet);
                        }
                        Console.WriteLine("OK.");
                    }
                    finally
                    {
                        ConsoleUtils.PopColor();
                    }
                }

                switch (PacketID)
                {
                    // Invalid Packets : These packets are unknown by the demo, therefore remove it from the buffer
                    case 0xB6: return new InvalidPacket(packet);
                    case 0xBB: return new PacketBB_UltimaMessenger(packet);

                    // Access Check Packets
                    case 0x12: return new Packet12_TextCommand(packet); // this packet may contain god commands

                    // Handled packets : We handle these packets, they do not reach the server
                    case 0xB8: return new PacketB8_ProfileReq(packet);
                    case 0xBD: return new PacketBD_ClientVersion(packet);
                    case 0x3A: return new Packet3A_SetSkillLock(packet);

                    // Transformed Packets : These packets are translated
                    case 0x00: return new Packet00_CreateChar(packet);
                    case 0x02: return new Packet02_MoveRequest(packet);
                    case 0x80: return new Packet80_LoginRequest(packet);
                    case 0xA0: return new PacketA0_SelectServer(packet);
                    case 0x91: return new Packet91_PlayServer(packet);
                    case 0x93: return new Packet93_BookHeaderChange(packet);
                    //case 0xAD: return new PacketAD_UnicodeSpeech(packet);     // Unfinished, for now UODemoDLL handles this

                    // Information: We don't change these packets

                    default:
                        if (PacketID >= 0xB6) 
                            return new InvalidPacket(packet);

                        return null;
                }
            }
            else
                UODemo.Socket.Free(socket);

            return null;    // Bad packet, length is too short.
        }

        /// <summary>This is the servers socket which received this packet.</summary>
        private UODemo.ISocket Socket { get; set; }

        public ClientVersionStruct ClientVersion { get { return Socket.Version; } }

        public void SetClientVersion(ClientVersionStruct vStruct)
        {
            Socket.SetClientVersion(vStruct);
        }

        public bool IsGM { get { return Socket.IsGm; } }
        public PlayerObject* PlayerPtr { get { return (PlayerObject*)Socket.PlayerObject; } }
        public PlayerObject Player { get { return *(PlayerObject*)Socket.PlayerObject; } }

        public void SendSystemMessage(string message)
        {
            Server.SendSystemMessage((class_Player*)Socket.PlayerObject, "Skill locks are not implemented.");
        }

        /// <summary>
        /// Replace this packet
        /// </summary>
        /// <param name="newData">The new packet data</param>
        /// <param name="oldlength">If included, this is the amount of data to replace, default is current length. Use when a new packet is bigger than the old.</param>
        public void Replace(byte[] newData, int oldlength =-1)
        {
            if (oldlength == -1) oldlength = Length;
            Socket.Replace(newData, (uint)oldlength);
            Length = newData.Length;
        }

        /// <summary>
        /// Remove trailing data from packet. Use this when additional fixed data has been added in a later packet version, but the leading data is still valid to the server
        /// </summary>
        /// <param name="amount">the number of bytes to remove</param>
        public void Truncate(ushort amount)
        {
            Socket.RemoveBytes((ushort)Length, amount);
            Length -= amount;
        }

        public uint TotalQueueLen{get{return Socket.DataLength;}}

        public bool IsLocal { get { return Socket.IsInternal; } }

        public IPAddress IPAddress { get { return Socket.ClientAddress; } }

        public uint AccountNumber { get { return Socket.AccountNumber; } }

        bool m_Dynamic;
        /// <summary>Packet is variable length</summary>
        public override bool Dynamic { get { return m_Dynamic; } }

        private ClientPacket(UODemo.ISocket socket, byte PacketID, uint PacketSize, bool IsPacketDynamicSized): base(socket.Data, PacketSize)
        {
            Socket = socket;
            Id = PacketID;
            m_Dynamic = IsPacketDynamicSized;
        }

        /// <summary>
        /// Get byte from buffer at indexed position
        /// </summary>
        /// <param name="i">buffer index</param>
        /// <returns>the byte at buffer[i]</returns>
        public byte this[uint i]
        {
            get
            {
                if (i >= Length)
                    return 0;
                return *(pData + i);
            }
            set
            {
                if (i < Length)
                    *(pData + i) = value;
            }
        }

        /// <summary>
        /// Dump packet information to console
        /// </summary>
        public void Print()
        {
            Console.WriteLine("0x{0:X2} Len {1}: ", Id, Length);
            for (uint i = 0; i < Length; i++)
            {
                Console.Write("{0:X2} ", this[i]);
                if ((i + 1) % 20 == 0 || i == Length - 1) Console.WriteLine();
                else if ((i + 1) % 5 == 0) Console.Write(' ');
            }
        }

        /// <summary>
        /// Write a fixed length string to the buffer.
        /// </summary>
        /// <param name="start">Index in buffer to begin writing</param>
        /// <param name="length">Max length to write</param>
        /// <param name="chars">The characters to write</param>
        /// <returns>Number of characters written</returns>
        public int WriteAsciiStringFixed(int start, int length, string chars)
        {
            length = Math.Min(start + length, Length) - start;
            if (chars.Length < length || chars.Length == 0)
                *(pData + start + chars.Length) = 0;    // add string terminator
            if (chars.Length == 0) return 0;             // Exit if zero length (array is null)
            fixed (char* pChars = chars.ToCharArray())
            {
                return ASCIIEncoding.Default.GetBytes(pChars, chars.Length, pData + start, length);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (Socket != null)
                UODemo.Socket.Free(Socket);
            Socket = null;
        }

        #endregion
    }
}
