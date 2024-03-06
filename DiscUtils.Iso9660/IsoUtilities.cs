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
using System.Globalization;
using System.Text;

namespace LibIRD.DiscUtils.Iso9660
{
    internal static class IsoUtilities
    {
        public const int SectorSize = 2048;

        public static uint ToUInt32FromBoth(byte[] data, int offset)
        {
            return (uint)(((data[offset + 3] << 24) & 0xFF000000U) | ((data[offset + 2] << 16) & 0x00FF0000U)
                          | ((data[offset + 1] << 8) & 0x0000FF00U) | ((data[offset + 0] << 0) & 0x000000FFU));
        }

        public static ushort ToUInt16FromBoth(byte[] data, int offset)
        {
            return (ushort)(((data[offset + 1] << 8) & 0xFF00) | ((data[offset + 0] << 0) & 0x00FF));
        }

        internal static string ReadChars(byte[] buffer, int offset, int numBytes, Encoding enc)
        {
            char[] chars;

            // Special handling for 'magic' names '\x00' and '\x01', which indicate root and parent, respectively
            if (numBytes == 1)
            {
                chars = new char[1];
                chars[0] = (char)buffer[offset];
            }
            else
            {
                Decoder decoder = enc.GetDecoder();
                chars = new char[decoder.GetCharCount(buffer, offset, numBytes, false)];
                decoder.GetChars(buffer, offset, numBytes, chars, 0, false);
            }

            return new string(chars).TrimEnd(' ');
        }

        internal static string NormalizeFileName(string name)
        {
            string[] parts = SplitFileName(name);
            return parts[0] + '.' + parts[1] + ';' + parts[2];
        }

        internal static string[] SplitFileName(string name)
        {
            string[] parts = [name, string.Empty, "1"];

            if (name.Contains("."))
            {
                int endOfFilePart = name.IndexOf('.');
                parts[0] = name.Substring(0, endOfFilePart);
                if (name.Contains(";"))
                {
                    int verSep = name.IndexOf(';', endOfFilePart + 1);
                    parts[1] = name.Substring(endOfFilePart + 1, verSep - (endOfFilePart + 1));
                    parts[2] = name.Substring(verSep + 1);
                }
                else
                {
                    parts[1] = name.Substring(endOfFilePart + 1);
                }
            }
            else
            {
                if (name.Contains(";"))
                {
                    int verSep = name.IndexOf(';');
                    parts[0] = name.Substring(0, verSep);
                    parts[2] = name.Substring(verSep + 1);
                }
            }

            if (!ushort.TryParse(parts[2], out ushort ver) || ver > 32767 || ver < 1)
            {
                ver = 1;
            }

            parts[2] = string.Format(CultureInfo.InvariantCulture, "{0}", ver);

            return parts;
        }

        /// <summary>
        /// Converts a DirectoryRecord time to UTC.
        /// </summary>
        /// <param name="data">Buffer containing the time data.</param>
        /// <param name="offset">Offset in buffer of the time data.</param>
        /// <returns>The time in UTC.</returns>
        internal static DateTime ToUTCDateTimeFromDirectoryTime(byte[] data, int offset)
        {
            try
            {
                DateTime relTime = new(
                    1900 + data[offset],
                    data[offset + 1],
                    data[offset + 2],
                    data[offset + 3],
                    data[offset + 4],
                    data[offset + 5],
                    DateTimeKind.Utc);
                return relTime - TimeSpan.FromMinutes(15 * (sbyte)data[offset + 6]);
            }
            catch (ArgumentOutOfRangeException)
            {
                // In case the ISO has a bad date encoded, we'll just fall back to using a fixed date
                return DateTime.MinValue;
            }
        }

        internal static bool IsSpecialDirectory(DirectoryRecord r)
        {
            return r.FileIdentifier == "\0" || r.FileIdentifier == "\x01";
        }
    }
}