using System;
using System.Runtime.InteropServices;

namespace Sharpkick
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x5C)] 
    struct ContainerObject
    {
        [FieldOffset(0x00)] public ItemObject ItemObject;

        public ushort Body { get { return ItemObject.ObjectType; } }
        public ushort Hue { get { return ItemObject.Hue; } }
        public Location Location { get { return ItemObject.Location; } }
        public uint Template { get { return ItemObject.Template; } }
        public uint Serial { get { return ItemObject.Serial; } }

    }
}
