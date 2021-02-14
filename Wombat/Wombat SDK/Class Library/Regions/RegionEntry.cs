using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace JoinUO.WombatSDK
{
    public struct RegionEntry
    {
        private static RegionEntry m_Empty=new RegionEntry();
        public static RegionEntry Empty { get { return m_Empty; } }

        private string m_Type;
        public string Type
        {
            get
            {
                if (m_Type == null)
                {
                    string[] p = Name.Replace("housing_no","housing*no").Split('_');
                    if (p.Length > 0)
                        return m_Type = p[0].Replace("housing*no","housing_no").ToUpper();
                    else
                        return m_Type = string.Empty;
                }
                return m_Type;
            }
        }

        private string m_City;
        public string City
        {
            get
            {
                if (m_City == null)
                {
                    string[] p = Name.Replace("housing_no", "housing*no").Split('_');
                    if (p.Length > 1)
                        m_City = p[1].ToUpper();
                    else
                        m_City = string.Empty;
                }
                return m_City;
            }
        }

        public static RegionEntry ReadTxt(string line)
        {//     2  3606  2463    11    17     0    20   1928062771 LIBRARY_OCLLO_1 17 0 0 0 0 the Ocllo Public Library
            RegionEntry entry = new RegionEntry();

            string[] p = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            entry.m_Type = null;
            entry.m_City = null;

            int i = 0;

            bool errors = false;

            if (i < p.Length) errors |= uint.TryParse(p[i++], out entry.ID);
            if (i < p.Length) errors |= ushort.TryParse(p[i++], out entry.X);
            if (i < p.Length) errors |= ushort.TryParse(p[i++], out entry.Y);
            if (i < p.Length) errors |= ushort.TryParse(p[i++], out entry.Width);
            if (i < p.Length) errors |= ushort.TryParse(p[i++], out entry.Height);
            if (i < p.Length) errors |= short.TryParse(p[i++], out entry.ZMin);
            if (i < p.Length) errors |= short.TryParse(p[i++], out entry.ZMax);
            if (i < p.Length) errors |= int.TryParse(p[i++], out entry.Flag);
            if (i < p.Length) entry.m_Name = p[i++];
            if (i < p.Length) errors |= short.TryParse(p[i++], out entry.unk1);
            if (i < p.Length) errors |= short.TryParse(p[i++], out entry.unk2);
            if (i < p.Length) errors |= short.TryParse(p[i++], out entry.unk3);
            if (i < p.Length) errors |= byte.TryParse(p[i++], out entry.unk4);
            if (i < p.Length) errors |= short.TryParse(p[i++], out entry.unk5);
            if (i < p.Length) entry.Description = string.Join(" ", p, i, p.Length - i);

            if (errors)
                Debug.WriteLine("Error parsing Regions.txt");

            return entry;
        }

        public string WriteTxt()
        {
            return string.Format(
                "{0,5} {1,5} {2,5} {3,5} {4,5} {5,5} {6,5} {7,12} {8} {9} {10} {11} {12} {13} {14}",
                ID, X, Y, Width, Height, ZMin, ZMax, Flag, Name, unk1, unk2, unk3, unk4, unk5, Description);
        }

        public static RegionEntry ReadMul(BinaryReader reader)
        {
            RegionEntry entry = new RegionEntry();

            entry.m_Type = null;
            entry.m_City = null;
            entry.m_Name = reader.ReadFixedString(40);
            entry.ID = reader.ReadUInt16();
            entry.X = reader.ReadUInt16();
            entry.Y = reader.ReadUInt16();
            entry.Width = reader.ReadUInt16();
            entry.Height = reader.ReadUInt16();
            entry.ZMin = reader.ReadInt16();
            entry.ZMax = reader.ReadInt16();
            entry.Flag = reader.ReadInt32();
            entry.Description = reader.ReadFixedString(40);
            entry.unk1 = reader.ReadInt16();
            entry.unk2 = reader.ReadInt16();
            entry.unk3 = reader.ReadInt16();
            entry.unk4 = reader.ReadByte();
            entry.unk5 = reader.ReadInt16();

            return entry;
        }

        public void WriteMul(BinaryWriter writer)
        {
            byte[] str40=new byte[40];
            ASCIIEncoding.ASCII.GetBytes(m_Name.ToCharArray(),0,m_Name.Length,str40,0);
            writer.Write(str40);
            writer.Write((ushort)ID);
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Width);
            writer.Write(Height);
            writer.Write(ZMin);
            writer.Write(ZMax);
            writer.Write(Flag);
            if (string.IsNullOrEmpty(Description))
                str40[0] = 0;
            else
                ASCIIEncoding.ASCII.GetBytes(Description.ToCharArray(), 0, Description.Length, str40, 0);
            writer.Write(str40);
            writer.Write(unk1);
            writer.Write(unk2);
            writer.Write(unk3);
            writer.Write(unk4);
            writer.Write(unk5);
        }

        private string m_Name;

        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
                m_Type = null;
                m_City = null;
            }
        }
        public uint ID;
        public ushort X;
        public ushort Y;
        public ushort Width;
        public ushort Height;
        public short ZMin;
        public short ZMax;
        public int Flag;
        public string Description;
        public short unk1;
        public short unk2;
        public short unk3;
        public byte unk4;
        public short unk5;
    }
}
