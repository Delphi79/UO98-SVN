using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick_Tests
{
    class MockServer_1B_LoginConfirm : BaseServerPacketMock
    {
        const int PlayerIDIndex = 1;
        const int BodyIndex = 5;
        const int XIndex = 7;
        const int YIndex = 9;
        const int ZIndex = 11;
        const int DirectionIndex = 13;
        const int FlagIndex = 23;
        const int NotoHueIndex = 24;

        public MockServer_1B_LoginConfirm(int playerid, ushort body, ushort xLoc, ushort yLoc, short zLoc, byte direction, byte flag, byte notohue)
            : base(0x1B, 37)
        {
            PlayerID = playerid;
            Body = body;
            X = xLoc;
            Y = yLoc;
            Z = zLoc;
            Direction = direction;
            Flag = flag;
            NotoHue = notohue;
        }

        public int PlayerID
        {
            get { return getDataInt(PlayerIDIndex); }
            set { SetData(PlayerIDIndex, value); }
        }

        public ushort Body
        {
            get { return getDataUShort(BodyIndex); }
            set { SetData(BodyIndex, value); }
        }

        public ushort X
        {
            get { return getDataUShort(XIndex); }
            set { SetData(XIndex, value); }
        }

        public ushort Y
        {
            get { return getDataUShort(YIndex); }
            set { SetData(YIndex, value); }
        }

        public short Z
        {
            get { return getDataShort(ZIndex); }
            set { SetData(ZIndex, value); }
        }

        public byte Direction
        {
            get { return getDataByte(DirectionIndex); }
            set { SetData(DirectionIndex, value); }
        }

        public byte Flag
        {
            get { return getDataByte(FlagIndex); }
            set { SetData(FlagIndex, value); }
        }

        public byte NotoHue
        {
            get { return getDataByte(NotoHueIndex); }
            set { SetData(NotoHueIndex, value); }
        }
    }
}
