using System.IO;
using System.Text;
using System.Collections.Generic;

namespace JoinUO.WombatSDK.IDX
{
	public class EntryListWH : List<EntryWH>
	{
		void ReadItemsFromStream(Stream stream, long length)
		{
			// Open the file
			BinaryReader br = new BinaryReader(stream, Encoding.ASCII);

			// Determine the number of entries in this IDX file
			// NOTE: because we do not read from the beginning, we require the length of the stream
			int NumberOfItemsInStream = (int)(length / Entry.BinarySize);

			// Read
			for (int i = 0; i < NumberOfItemsInStream; i++)
				this.Add(new EntryWH(br));
		}

		#region "Constructors"
		public EntryListWH(int InitialCapacity)
			: base(capacity: InitialCapacity)
		{
		}

		public EntryListWH(string path)
		{
			// Open the file
			FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

			// Load
			ReadItemsFromStream(fs, fs.Length);

			// Close the file
			fs.Close();
		}

		public EntryListWH(Stream stream)
			: this(stream, stream.Length - stream.Position)
		{
		}

		public EntryListWH(Stream stream, long length)
		{
			ReadItemsFromStream(stream, length);
		}
		#endregion

		#region "Public Shared Functions"
		static public EntryListWH Load(string path)
		{
			return new EntryListWH(path);
		}

		static public EntryListWH Load(Stream stream)
		{
			return new EntryListWH(stream);
		}

		static public EntryListWH Load(Stream stream, long length)
		{
			return new EntryListWH(stream, length);
		}
		#endregion

		#region "Public Functions"
		public void Save(string path)
		{
			// Open the file
			FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
			BinaryWriter wr = new BinaryWriter(fs, Encoding.ASCII);

			// Write all entries
			foreach (EntryWH entry in this)
				entry.Write(wr);

			// Close the file
			// NOTE: this will also close the FileStream
			wr.Close();
		}
		#endregion
	}
}