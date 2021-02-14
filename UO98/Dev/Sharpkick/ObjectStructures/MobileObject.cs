using System;
using System.Runtime.InteropServices;

namespace Sharpkick
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x37C)]
    unsafe struct MobileObject
    {
        [FieldOffset(0x00)] public ContainerObject ContainerObject;

        public ushort Body { get { return ContainerObject.ItemObject.ObjectType; } }
        public ushort Hue { get { return ContainerObject.ItemObject.Hue; } }
        public Location Location { get { return ContainerObject.ItemObject.Location; } }
        public uint Template { get { return ContainerObject.ItemObject.Template; } }
        public uint Serial { get { return ContainerObject.ItemObject.Serial; } }

        [FieldOffset(0x23C)] public ushort Str;
        [FieldOffset(0x23E)] public ushort Dex;
        [FieldOffset(0x240)] public ushort Int;
        [FieldOffset(0x242)] public ushort StrMod;
        [FieldOffset(0x244)] public ushort DexMod;
        [FieldOffset(0x246)] public ushort IntMod;

        [FieldOffset(0x254)] public int CurHP;
        [FieldOffset(0x258)] public int MaxHP;
        [FieldOffset(0x25C)] public int CurFat;
        [FieldOffset(0x260)] public int MaxFat;
        [FieldOffset(0x264)] public int CurMana;
        [FieldOffset(0x268)] public int MaxMana;

        [FieldOffset(0x286)] public ushort Fame;
        [FieldOffset(0x288)] public ushort Karma;
        [FieldOffset(0x28A)] public ushort Satiety;

        [FieldOffset(0x364)] private byte* m_RealName;
        public string RealName { get { return StringPointerUtils.GetAsciiString(m_RealName, 30); } }

        #region MobileFlags
        [FieldOffset(0x379)] public MobileFlags m_Flags;
        public static MobileFlags GetMobileFlags(MobileObject* mobile)
        {
            if (mobile == null) return 0;
            return (*mobile).m_Flags;
        }

        public static bool GetMobileFlags(MobileObject* mobile, MobileFlags toGet)
        {
            if (mobile == null) return false;
            return ((*mobile).m_Flags & toGet) == toGet;
        }

        public static void SetMobileFlags(MobileObject* mobile, MobileFlags toSet, bool Value = true)
        {
            if (mobile != null)
            {
                if (Value) (*mobile).m_Flags |= toSet;
                else (*mobile).m_Flags &= ~toSet;
            }
        }

        public static void ClearMobileFlags(MobileObject* mobile, MobileFlags toClear)
        {
            SetMobileFlags(mobile, toClear, false);
        }
        #endregion
    }
}
