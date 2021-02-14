using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace JoinUO.WombatSDK.IDX
{
	public class EntryList : List<Entry>
	{
		void ReadFromStream(Stream stream, long length)
		{
			// Open the file
			BinaryReader br = new BinaryReader(stream, Encoding.ASCII);

			// Determine the number of entries in this IDX file
			// NOTE: because we do not read from the beginning, we require the length of the stream
			int NumberOfItemsInStream = (int)(length / Entry.BinarySize);

			// Read
			for (int i = 0; i < NumberOfItemsInStream; i++)
				this.Add(new Entry(br));
		}

		#region "Constructors"
		public EntryList(int InitialCapacity)
			: base(capacity: InitialCapacity)
		{
		}

		public EntryList(string path)
		{
			// Open the file
			FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

			// Load
			ReadFromStream(fs, fs.Length);

			// Close the file
			fs.Close();
		}

		public EntryList(Stream stream)
			: this(stream, stream.Length - stream.Position)
		{
		}

		public EntryList(Stream stream, long length)
		{
			ReadFromStream(stream, length);
		}
		#endregion

		#region "Public Shared Functions"
		static public EntryList Load(string path)
		{
			return new EntryList(path);
		}

		static public EntryList Load(Stream stream)
		{
			return new EntryList(stream);
		}

		static public EntryList Load(Stream stream, long length)
		{
			return new EntryList(stream, length);
		}
		#endregion

		#region "Public Functions"
		public void Save(string path)
		{
			// Open the file
			FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
			BinaryWriter wr = new BinaryWriter(fs, Encoding.ASCII);

			// Write all entries
			foreach (Entry entry in this)
				entry.Write(wr);

			// Close the file
			// NOTE: this will also close the FileStream
			wr.Close();
		}
		#endregion
	}
}