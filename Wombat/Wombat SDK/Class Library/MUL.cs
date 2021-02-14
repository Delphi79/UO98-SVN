using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace JoinUO.WombatSDK
{
	namespace MUL
	{
		public abstract class Header
		{
			/// <summary>Size of header in Bytes</summary>
			abstract public int Size { get; }
		}

		public abstract class ItemType : INotifyPropertyChanged
		{
			public event PropertyChangedEventHandler PropertyChanged;

			protected void RaisePropertyChanged(object sender, string propertyName)
			{
				if (PropertyChanged != null)
					PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
			}

			private void RaisePropertyChanged(string propertyName)
			{
				RaisePropertyChanged(this, propertyName);
			}
		}

		public abstract class Item
		{
			public object Content { get; set; }
			public virtual int BinarySize
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public virtual void Read(BinaryReader SourceStream)
			{
				throw new NotImplementedException();
			}

			public virtual void Write(BinaryWriter TargetStream)
			{
				throw new NotImplementedException();
			}
		}

		public abstract class ItemList : List<Item>
		{
		}

		public enum ItemSizeSource
		{
			IndexFile, Item
		}

		public abstract class File
		{
			private bool WeOwnTheStream = false;
			protected BinaryReader brMUL;
			protected IDX.EntryListWH EntryListWH;

			#region "Public, Abstract Properties"
			abstract public bool HasHeader { get; }
			abstract public ItemSizeSource ItemSizeSource { get; }
			#endregion

			#region "Public Properties"
			public Header Header
			{
				get
				{
					return GetHeader();
				}
			}

			public int HeaderSize
			{
				get
				{
					Header _Header = GetHeader();
					if (_Header == null)
						return 0;
					return _Header.Size;
				}
			}

			private bool _IsIndexed;
			public bool IsIndexed
			{
				get
				{
					return _IsIndexed;
				}
			}

			public int FixedItemSize
			{
				get
				{
					return GetFixedItemSize();
				}
			}
			#endregion

			#region "Overridable Support Functions"
			virtual protected void ReadHeader()
			{
				// This function should be overriden when we have a header
				// If we reach here than your class should override this function!
				// That's why we throw an exception to make you aware of the error...
				if (HasHeader)
					throw new NotSupportedException("ReadHeader must be overriden!");
			}

			virtual protected Header GetHeader()
			{
				// This function should be overriden when we have a header
				// If we reach here than your class should override this function!
				// That's why we throw an exception to make you aware of the error...
				if (HasHeader)
					throw new NotSupportedException("GetHeader must be overriden!");
				return null;
			}

			virtual protected int GetFixedItemSize()
			{
				// This function should be overriden when we don't use indexes
				// If we reach here than your class should override this function!
				// That's why we throw an exception to make you aware of the error...
				if (!_IsIndexed)
					throw new NotSupportedException("GetFixedItemSize must be overriden!");
				return 0;
			}
			#endregion

			#region "Constructors"
			public File(string path)
				: this(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				WeOwnTheStream = true;
			}

			public File(Stream stream)
			{
				brMUL = new BinaryReader(stream, Encoding.ASCII);

				_IsIndexed = false;
				ReadHeader();
			}

			public File(string MULpath, string IDXpath)
				: this(new FileStream(MULpath, FileMode.Open, FileAccess.Read, FileShare.Read), new FileStream(IDXpath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
			}

			public File(Stream MULstream, Stream IDXstream)
				: this(MULstream, MULstream.Length - MULstream.Position, IDXstream, IDXstream.Length - IDXstream.Position)
			{
			}

			public File(Stream MULstream, long MULlength, Stream IDXstream, long IDXlength)
			{
				brMUL = new BinaryReader(MULstream, Encoding.ASCII);
				EntryListWH = IDX.EntryListWH.Load(IDXstream, IDXlength);

				_IsIndexed = true;
				ReadHeader();
			}
			#endregion

			#region "Destructor"
			~File()
			{
				if (WeOwnTheStream)
					brMUL.Close();
			}
			#endregion
		}
	}
}
