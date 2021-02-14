using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UOClientSDKTestProject
{
    static class Utility
    {
        public static int WriteStringToBuffer(string text, byte[] buffer, int position)
        {
            byte[] bString = Encoding.ASCII.GetBytes(text);
            int i;
            for (i = 0; i < bString.Length && (i + position < buffer.Length); i++)
                buffer[i + position] = bString[i];
            if (i + position >= buffer.Length)
                return i + position;
            buffer[i + position] = 0;
            return i + 1;
        }

        public static int WriteUShort(ushort val, byte[] buffer, int position)
        {
            buffer[position] = (byte)((val & 0xFF00) >> 8);
            buffer[position + 1] = (byte)(val & 0xFF);
            return 2;
        }

        public static int WriteUInt(uint val, byte[] buffer, int position)
        {
            buffer[position] = (byte)((val & 0xFF000000) >> 24);
            buffer[position + 1] = (byte)((val & 0xFF0000) >> 16);
            buffer[position + 2] = (byte)((val & 0xFF00) >> 8);
            buffer[position + 3] = (byte)(val & 0xFF);
            return 4;
        }

    }
}
