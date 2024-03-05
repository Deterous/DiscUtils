//
// Copyright (c) 2008-2011, Kenneth Bell
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
//

using System;

namespace LibIRD.DiscUtils.Streams
{
    public static class EndianUtilities
    {
        #region Bit Twiddling
        
        public static void WriteBytesLittleEndian(ushort val, byte[] buffer, int offset)
        {
            buffer[offset] = (byte)(val & 0xFF);
            buffer[offset + 1] = (byte)((val >> 8) & 0xFF);
        }

        public static void WriteBytesLittleEndian(uint val, byte[] buffer, int offset)
        {
            buffer[offset] = (byte)(val & 0xFF);
            buffer[offset + 1] = (byte)((val >> 8) & 0xFF);
            buffer[offset + 2] = (byte)((val >> 16) & 0xFF);
            buffer[offset + 3] = (byte)((val >> 24) & 0xFF);
        }

        public static void WriteBytesBigEndian(ushort val, byte[] buffer, int offset)
        {
            buffer[offset] = (byte)(val >> 8);
            buffer[offset + 1] = (byte)(val & 0xFF);
        }

        public static void WriteBytesBigEndian(uint val, byte[] buffer, int offset)
        {
            buffer[offset] = (byte)((val >> 24) & 0xFF);
            buffer[offset + 1] = (byte)((val >> 16) & 0xFF);
            buffer[offset + 2] = (byte)((val >> 8) & 0xFF);
            buffer[offset + 3] = (byte)(val & 0xFF);
        }

        public static ushort ToUInt16LittleEndian(byte[] buffer, int offset)
        {
            return (ushort)(((buffer[offset + 1] << 8) & 0xFF00) | ((buffer[offset + 0] << 0) & 0x00FF));
        }

        public static uint ToUInt32LittleEndian(byte[] buffer, int offset)
        {
            return (uint)(((buffer[offset + 3] << 24) & 0xFF000000U) | ((buffer[offset + 2] << 16) & 0x00FF0000U)
                          | ((buffer[offset + 1] << 8) & 0x0000FF00U) | ((buffer[offset + 0] << 0) & 0x000000FFU));
        }

        /// <summary>
        /// Primitive conversion from Unicode to ASCII that preserves special characters.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="dest">The buffer to fill.</param>
        /// <param name="offset">The start of the string in the buffer.</param>
        /// <param name="count">The number of characters to convert.</param>
        /// <remarks>The built-in ASCIIEncoding converts characters of codepoint > 127 to ?,
        /// this preserves those code points by removing the top 16 bits of each character.</remarks>
        public static void StringToBytes(string value, byte[] dest, int offset, int count)
        {
            char[] chars = value.ToCharArray(0, Math.Min(value.Length, count));

            int i = 0;
            while (i < chars.Length && i < count)
            {
                dest[i + offset] = (byte)chars[i];
                ++i;
            }

            while (i < count)
            {
                dest[i + offset] = 0;
                ++i;
            }
        }

        /// <summary>
        /// Primitive conversion from ASCII to Unicode that preserves special characters.
        /// </summary>
        /// <param name="data">The data to convert.</param>
        /// <param name="offset">The first byte to convert.</param>
        /// <param name="count">The number of bytes to convert.</param>
        /// <returns>The string.</returns>
        /// <remarks>The built-in ASCIIEncoding converts characters of codepoint > 127 to ?,
        /// this preserves those code points.</remarks>
        public static string BytesToString(byte[] data, int offset, int count)
        {
            char[] result = new char[count];

            for (int i = 0; i < count; ++i)
            {
                result[i] = (char)data[i + offset];
            }

            return new string(result);
        }

        #endregion
    }
}
