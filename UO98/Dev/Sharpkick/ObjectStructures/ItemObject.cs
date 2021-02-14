using System;
using System.Runtime.InteropServices;

namespace Sharpkick
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x50)]
    struct ItemObject 
    {
        [FieldOffset(0x04)] public readonly ushort ObjectType;
        [FieldOffset(0x08)] public readonly ushort Hue;
        [FieldOffset(0x0A)] public readonly Location Location;
        [FieldOffset(0x10)] public readonly Location CreationLocation;
        [FieldOffset(0x16)] public readonly uint Template;
        //...
        [FieldOffset(0x40)] public readonly uint Serial;
        [FieldOffset(0x44)] public readonly byte StatusFlags;
        [FieldOffset(0x45)] public readonly byte DecayCount;
    }
}
