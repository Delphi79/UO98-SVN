using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UoClientSDK
{
    public enum AccessLevel : byte
    {
        Player = 0,
        Counselor,
        GameMaster,
        Seer,
        Administrator,
        Developer,
        Owner
    }

    [Flags]
    public enum ClientFlag : int
    {
        T2A = 0x00,
        UOR = 0x01,
        T3D = 0x02,
        LBR = 0x04,
        AOS = 0x08,
        SE = 0x10,
        SA = 0x20,
        UO3D = 0x40,
        reserved = 0x80,
        ThreeDClient = 0x100,
    }

    public enum Profession : byte
    {

    }

    public enum Skill : byte
    {

    }

    public enum Hue : ushort
    {

    }

    public enum HairStyle : ushort
    {

    }

    public enum BeardStyle : ushort
    {

    }
}
