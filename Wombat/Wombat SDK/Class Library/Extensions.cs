using System;
using System.IO;

public static class Extensions
{
    public static string ReadFixedString(this BinaryReader reader, int length)
    {
        string read = new string(reader.ReadChars(40));
        int i = read.IndexOf((char)0);
        if (i == 0) return string.Empty;
        if (i > 0 && i < read.Length) read = read.Substring(0, i);
        return read;
    }

	public static int SetBit(int ToModify, int BitToSet)
	{
		return ToModify | (1 << BitToSet);
	}

	public static int SetHighestBit(int ToModify)
	{
		return SetBit(ToModify, 31);
	}

	public static bool IsBitSet(int ToTest, int BitToTest)
	{
		return (ToTest & (1 << BitToTest)) != 0;
	}

	public static bool IsHighestBitSet(int ToTest)
	{
		return IsBitSet(ToTest, 31);
	}
}

