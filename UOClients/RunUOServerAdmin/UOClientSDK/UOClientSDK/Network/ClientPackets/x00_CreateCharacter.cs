using System.Net;
using System.Runtime.InteropServices;

namespace UoClientSDK.Network.ClientPackets
{
    public class CreateCharacter : ClientPacket
    {
        const ClientPacketId ID = ClientPacketId.CreateCharacter;
        public override ClientPacketId PacketID { get { return ID; } }

        public CreateCharacter(ClientVersion version,
                                string charname,
                                ClientFlag clientflags,
                                Profession profession,
                                RaceAndGender raceandgender,
                                CharStats stats,
                                SkillValuePair[] startSkills,
                                Hue skincolor,
                                HairStyle hair,
                                Hue haircolor,
                                BeardStyle beard,
                                Hue beardcolor,
                                ushort locationindex,
                                ushort charslotnum,
                                IPAddress clientip,
                                Hue shirtcolor,
                                Hue pantscolor)
            : base(version)
        {
            CharName = charname;
            ClientFlags = clientflags;
            LoginCount = 0; // ?
            Profession = profession;
            RaceAndGender = raceandgender;
            Stats = stats;
            StartSkills = startSkills;
            SkinColor = skincolor;
            HairStyle = hair;
            HairColor = haircolor;
            FacialHairStyle = beard;
            FacialHairColor = beardcolor;
            StartLocationIndex = locationindex;
            CharSlotNum = charslotnum;
            ClientIP = clientip;
            ShirtColor = shirtcolor;
            PantsColor = pantscolor;
        }

        const uint pattern1 = 0xedededed;
        const uint pattern2= 0xffffffff;
        const byte pattern3= 0x00;
        public string CharName{get;private set;}
        const ushort unknown0=0x00;
        public ClientFlag ClientFlags{get;private set;}
        const uint unknown1=0x00;
        public uint LoginCount{get;private set;}
        public Profession Profession{get;private set;}
        public RaceAndGender RaceAndGender{get;private set;}
        public CharStats Stats{get;private set;}
        public SkillValuePair[] StartSkills{get;private set;}
        public Hue SkinColor{get;private set;}
        public HairStyle HairStyle{get;private set;}
        public Hue HairColor{get;private set;}
        public BeardStyle FacialHairStyle{get;private set;}
        public Hue FacialHairColor{get;private set;}
        public ushort StartLocationIndex{get;private set;}
        const ushort unknown3=0x00;
        public ushort CharSlotNum{get;private set;}
        public IPAddress ClientIP{get;private set;}
                public Hue ShirtColor{get;private set;}
        public Hue PantsColor{get;private set;}

        internal override PacketWriter GetWriter()
        {
            PacketWriter writer = new PacketWriter((ushort)(1 + 103));
            writer.Write(PacketID);

            writer.Write(pattern1);
            writer.Write(pattern2);
            writer.Write(pattern3);
            writer.WriteStringFixed(CharName, 30);
            writer.Write(unknown0);
            writer.Write((int)ClientFlags);
            writer.Write(unknown1);
            writer.Write(LoginCount);
            writer.Write((byte)Profession);
            writer.Skip(15); //unknown2[15];

            writer.Write(RaceAndGender.ToByte());

            writer.Write(Stats.Strength);
            writer.Write(Stats.Dexterity);
            writer.Write(Stats.Intelligence);

            writer.Write(StartSkills.Length > 0 ? (byte)StartSkills[0].Skill : (byte)0);
            writer.Write(StartSkills.Length > 0 ? (byte)StartSkills[0].Value : (byte)0);
            writer.Write(StartSkills.Length > 1 ? (byte)StartSkills[1].Skill : (byte)0);
            writer.Write(StartSkills.Length > 1 ? (byte)StartSkills[1].Value : (byte)0);
            writer.Write(StartSkills.Length > 2 ? (byte)StartSkills[2].Skill : (byte)0);
            writer.Write(StartSkills.Length > 2 ? (byte)StartSkills[2].Value : (byte)0);

            writer.Write((ushort)SkinColor);
            writer.Write((ushort)HairStyle);
            writer.Write((ushort)HairColor);
            writer.Write((ushort)FacialHairStyle);
            writer.Write((ushort)FacialHairColor);

            writer.Write(StartLocationIndex);
            writer.Write(unknown3);
            writer.Write(CharSlotNum);

            byte[] addressbytes = ClientIP.GetAddressBytes();
            writer.Write(addressbytes[0]);
            writer.Write(addressbytes[1]);
            writer.Write(addressbytes[2]);
            writer.Write(addressbytes[3]);

            writer.Write((ushort)ShirtColor);
            writer.Write((ushort)PantsColor);

            return writer;
        }

        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        //public unsafe struct CreateCharacterArgs
        //{
        //    public CreateCharacterArgs(string charname, ClientFlag clientflags, Profession profession, RaceAndGender raceandgender, CharStats stats, SkillValuePair[] startSkills,
        //        Hue skincolor, HairStyle hair, Hue haircolor, BeardStyle beard, Hue beardcolor, ushort locationindex, ushort charslotnum, IPAddress clientip, Hue shirtcolor, Hue pantscolor
        //        )
        //    {
        //        pattern1 = 0xedededed;
        //        pattern2 = 0xffffffff;
        //        pattern3 = 0x00;
        //        ClientFlags = clientflags;
        //        LoginCount = 0;
        //        Profession = profession;
        //        RaceAndGenderByte = raceandgender.ToByte();
        //        Strength = stats.Strength;
        //        Dexterity = stats.Dexterity;
        //        Intelligence = stats.Intelligence;
        //        Skill1 = startSkills[0].Skill;
        //        Skill1Value = startSkills[0].Value;
        //        Skill2 = startSkills[1].Skill;
        //        Skill2Value = startSkills[1].Value;
        //        Skill3 = startSkills[2].Skill;
        //        Skill3Value = startSkills[2].Value;
        //        SkinColor = skincolor;
        //        HairStyle = hair;
        //        HairColor = haircolor;
        //        FacialHairStyle = beard;
        //        FacialHairColor = beardcolor;
        //        StartLocationIndex = locationindex;
        //        CharSlotNum = charslotnum;
        //        ShirtColor = shirtcolor;
        //        PantsColor = pantscolor;

        //        CharName = charname;
        //        ClientIPAddress = clientip;
        //    }

        //    public RaceAndGender RaceAndGender
        //    {
        //        set { RaceAndGenderByte = value.ToByte(); }
        //    }

        //    public string CharName
        //    {
        //        get { fixed (byte* sbuffer = CharNameBuffer) return BytePointerUtils.ReadAsciiStringFixed(sbuffer, 30); }
        //        set { fixed (byte* Dest = CharNameBuffer)    BytePointerUtils.WriteAsciiStringFixed(value, Dest, 30); }
        //    }

        //    public IPAddress ClientIPAddress
        //    {
        //        get { fixed (byte* ip = ClientIpAddressBytes) return new IPAddress(new byte[] { *ip, *(ip + 1), *(ip + 2), *(ip + 3) }); }
        //        set
        //        {
        //            byte[] ipbytes = value.GetAddressBytes();
        //            fixed (byte* clientipaddressbytes = ClientIpAddressBytes)
        //                for (int i = 0; i < 4; i++)
        //                    *(clientipaddressbytes + i) = ipbytes[i];
        //        }
        //    }

        //    uint pattern1;
        //    uint pattern2;
        //    byte pattern3;
        //    fixed byte CharNameBuffer[30];
        //    fixed byte unknown0[2];
        //    public ClientFlag ClientFlags;
        //    fixed byte unknown1[4];
        //    public uint LoginCount;
        //    public Profession Profession;
        //    fixed byte unknown2[15];
        //    public byte RaceAndGenderByte;
        //    public byte Strength;
        //    public byte Dexterity;
        //    public byte Intelligence;
        //    public Skill Skill1;
        //    public byte Skill1Value;
        //    public Skill Skill2;
        //    public byte Skill2Value;
        //    public Skill Skill3;
        //    public byte Skill3Value;
        //    public Hue SkinColor;
        //    public HairStyle HairStyle;
        //    public Hue HairColor;
        //    public BeardStyle FacialHairStyle;
        //    public Hue FacialHairColor;
        //    public ushort StartLocationIndex;
        //    fixed byte unknown3[2];
        //    public ushort CharSlotNum;
        //    fixed byte ClientIpAddressBytes[4];
        //    public Hue ShirtColor;
        //    public Hue PantsColor;
        //}
    }
}
