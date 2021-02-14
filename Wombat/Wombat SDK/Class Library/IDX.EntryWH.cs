using System.IO;

namespace JoinUO.WombatSDK.IDX
{
	public class EntryWH : Entry
	{
		#region "width/height Properties"
		public short width
		{
			get
			{
				return (short)(extra & 0xFFFF);
			}

			set
			{
				extra &= unchecked((int)0xFFFF0000);
				extra |= (ushort)value;
			}
		}

		public short height
		{
			get
			{
				return (short)((extra >> 16) & 0xFFFF);
			}

			set
			{
				extra &= (int)0x0000FFFF;
				extra |= (int)(value << 16);
			}
		}
		#endregion

		#region "Constructors"
		public EntryWH() : base() { }
		public EntryWH(int initial_lookup, int initial_length, int initial_extra) : base(initial_lookup, initial_length, initial_extra) { }
		public EntryWH(BinaryReader initial_SourceStream) : base(initial_SourceStream) { }
		#endregion
	}
}