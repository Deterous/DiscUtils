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
using System.IO;

namespace LibIRD.DiscUtils.Streams
{
    public static class StreamUtilities
    {
        /// <summary>
        /// Validates standard buffer, offset, count parameters to a method.
        /// </summary>
        /// <param name="buffer">The byte array to read from / write to.</param>
        /// <param name="offset">The starting offset in <c>buffer</c>.</param>
        /// <param name="count">The number of bytes to read / write.</param>
        public static void AssertBufferParameters(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), offset, "Offset is negative");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "Count is negative");

            if (buffer.Length < offset + count)
                throw new ArgumentException("buffer is too small", nameof(buffer));
        }

        #region Stream Manipulation

        /// <summary>
        /// Read bytes until buffer filled or throw EndOfStreamException.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="buffer">The buffer to populate.</param>
        /// <param name="offset">Offset in the buffer to start.</param>
        /// <param name="count">The number of bytes to read.</param>
        public static void ReadExact(Stream stream, byte[] buffer, int offset, int count)
        {
            int originalCount = count;

            while (count > 0)
            {
                int numRead = stream.Read(buffer, offset, count);

                if (numRead == 0)
                    throw new EndOfStreamException("Unable to complete read of " + originalCount + " bytes");

                offset += numRead;
                count -= numRead;
            }
        }

        /// <summary>
        /// Read bytes until buffer filled or throw EndOfStreamException.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The data read from the stream.</returns>
        public static byte[] ReadExact(Stream stream, int count)
        {
            byte[] buffer = new byte[count];

            ReadExact(stream, buffer, 0, count);

            return buffer;
        }

        /// <summary>
        /// Read bytes until buffer filled or EOF.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="buffer">The buffer to populate.</param>
        /// <param name="offset">Offset in the buffer to start.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        public static int ReadMaximum(Stream stream, byte[] buffer, int offset, int count)
        {
            int totalRead = 0;

            while (count > 0)
            {
                int numRead = stream.Read(buffer, offset, count);

                if (numRead == 0)
                {
                    return totalRead;
                }

                offset += numRead;
                count -= numRead;
                totalRead += numRead;
            }

            return totalRead;
        }

        #endregion
    }
}
