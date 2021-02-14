using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace JoinUO.WombatSDK.IDX
{
	public class Entry
	{
		public const int BinarySize = 12;

		public int lookup { get; set; }
		public int length { get; set; }
		public int extra { get; set; }

		#region "Specialiazed Properties"
		public bool IsPatched
		{
			get
			{
				return length != -1 && Extensions.IsHighestBitSet(length);
			}
		}
		#endregion

		#region "Constructors"
		public Entry()
			: this(-1, -1, -1)
		{
		}

		public Entry(int initial_lookup, int initial_length, int initial_extra)
		{
			lookup = initial_lookup;
			length = initial_length;
			extra = initial_extra;
		}

		public Entry(BinaryReader initial_SourceStream)
		{
			Read(initial_SourceStream);
		}
		#endregion

		#region "Public Functions"
		public void Read(BinaryReader SourceStream)
		{
			lookup = SourceStream.ReadInt32();
			length = SourceStream.ReadInt32();
			extra = SourceStream.ReadInt32();
		}

		public void Write(BinaryWriter TargetStream)
		{
			if (IsPatched)
				throw new NotSupportedException("Patched entries cannot be saved!");

			TargetStream.Write(lookup);
			TargetStream.Write(length);
			TargetStream.Write(extra);
		}

		public void ApplyPatch(Entry patch)
		{
			// NOTE: patching also means setting the highest bit of "length"
			lookup = patch.lookup;
			length = Extensions.SetHighestBit(patch.length);
			extra = patch.extra;
		}
		#endregion
	}
}