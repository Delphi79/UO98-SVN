using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace JoinUO.WombatSDK
{
    class RegionsMul : MUL.File
    {
        public override bool HasHeader { get { return true; } }
        public override MUL.ItemSizeSource ItemSizeSource
        {
          get { return MUL.ItemSizeSource.Item; }
        }

        MUL.Header m_Header = null;
        protected override MUL.Header GetHeader() { return m_Header; }
        protected override void ReadHeader() { m_Header = new RegionMulHeader(brMUL.ReadUInt16()); }

        protected override int GetFixedItemSize() { return 0x6B; }

        List<RegionMulItem> m_Items = null;

        public int Count { get { return m_Items == null ? 0 : m_Items.Count; } }
        public RegionMulItem this[int index] { get { return m_Items == null || index < 0 || index >= Count ? null : m_Items[index]; } }
        public IEnumerable<RegionMulItem> Items { get { return m_Items; } }

        public RegionsMul(string path) : base(path)
        {
            m_Items=new List<RegionMulItem>();
            while (brMUL.BaseStream.Position < brMUL.BaseStream.Length)
                m_Items.Add(new RegionMulItem(brMUL));
        }
    }

    class RegionMulHeader : MUL.Header
    {
        public override int Size { get { return 4; } }
        public ushort Version { get; private set; }
        public RegionMulHeader(ushort version) { Version = version; }
    }

    class RegionMulItem : MUL.Item
    {
        private RegionEntry m_Entry;

        public RegionMulItem(RegionEntry entry)
        {
            m_Entry = entry;
        }

        public RegionMulItem(BinaryReader br)
        {
            m_Entry = RegionEntry.ReadMul(br);
        }
    }
}