using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace JoinUO
{
	namespace WombatSDK
	{
		public class Light : MUL.File
		{
			public override bool HasHeader { get { return false; } }
			public override MUL.ItemSizeSource ItemSizeSource { get { return MUL.ItemSizeSource.IndexFile; } }

			#region "Constructors"
			public Light(String path)
				: base(path)
			{
			}

			public Light(Stream stream)
				: base(stream)
			{
			}
			#endregion

			#region "Public Functions"
			public byte[] ReadEntry(IDX.EntryWH entry)
			{
				// Only read from valid entries
				if (entry.lookup < 0 || entry.length <= 0)
					return null;

				// Detect errors
				int actuallength = entry.width * entry.height;
				if (actuallength != entry.length)
				{
					// Only read if the width & height fit the data length
					if (actuallength > entry.length)
						return null;

					// NOTE: light.mul contains a mismatch at 2400075
					// Fix the width & height
					short xy = (short)Math.Sqrt(entry.length);
					if (xy * xy != entry.length)
						return null;
					entry.width = xy;
					entry.height = xy;
				}

				// Go to the entry data
				brMUL.BaseStream.Seek(entry.lookup, SeekOrigin.Begin);

				// Allocate a byte buffer and read it into memory
				byte[] x = new byte[actuallength];
				brMUL.BaseStream.Read(x, 0, actuallength);

				return x;
			}
			#endregion
		}
	}
}