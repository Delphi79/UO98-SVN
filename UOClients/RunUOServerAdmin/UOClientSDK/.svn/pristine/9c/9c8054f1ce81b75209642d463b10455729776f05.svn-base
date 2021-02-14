using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UoClientSDK
{
    public class RaceAndGender
    {
        public static RaceAndGender empty = new RaceAndGender(ClientVersion.vMIN, RaceType.Human, GenderType.Male);
        public static RaceAndGender Empty { get { return empty; } }

        public enum RaceType { Human, Elf, Gargoyle }
        public enum GenderType { Male, Female }

        public ClientVersion ClientVersion { get; private set; }
        public RaceType Race { get; private set; }
        public GenderType Gender { get; private set; }
        public RaceAndGender(ClientVersion version, RaceType race, GenderType gender)
        {
            ClientVersion = version;
            Race = race;
            Gender = gender;
        }

        public byte ToByte()
        {
            return GetValueFor(ClientVersion, Race, Gender);
        }

        static byte GetValueFor(ClientVersion version, RaceType race, GenderType gender)
        {
            // TODO: Restrict results by ClientVersion
            switch (race)
            {
                default:
                case RaceType.Human:
                    return gender == GenderType.Male ? (byte)RaceAndGenderOld.HumanMale : (byte)RaceAndGenderOld.HumanFemale;
                case RaceType.Elf:
                    if (version < ClientVersion.v7_0_0_0)
                        return gender == GenderType.Male ? (byte)RaceAndGenderOld.ElfMale : (byte)RaceAndGenderOld.ElfFemale;
                    else
                        return gender == GenderType.Male ? (byte)RaceAndGenderNew.ElfMale : (byte)RaceAndGenderNew.ElfFemale;
                case RaceType.Gargoyle:
                    return gender == GenderType.Male ? (byte)RaceAndGenderNew.GargoyleMale : (byte)RaceAndGenderNew.GargoyleFemale;
            }
        }

    }

    /// <summary>Pre Client version 7.0 Gender enumeration</summary>
    public enum RaceAndGenderOld : byte
    {
        HumanMale = 0,
        HumanFemale = 1,
        ElfMale = 2,
        ElfFemale = 3
    }

    /// <summary>Post Client version 7.0 Gender enumeration</summary>
    public enum RaceAndGenderNew : byte
    {
        HumanMale = 0,
        HumanFemale = 1,
        ElfMale = 4,
        ElfFemale = 5,
        GargoyleMale = 6,
        GargoyleFemale = 7,
    }

}
