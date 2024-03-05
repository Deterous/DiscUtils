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
using System.Text;

namespace LibIRD.DiscUtils.Iso9660
{
    internal class DirectoryRecord
    {
        public uint DataLength;
        public byte ExtendedAttributeRecordLength;
        public string FileIdentifier;
        public byte FileUnitSize;
        public FileFlags Flags;
        public byte InterleaveGapSize;
        public uint LocationOfExtent;
        public DateTime RecordingDateAndTime;
        public byte[] SystemUseData;
        public ushort VolumeSequenceNumber;

        public static int ReadFrom(byte[] src, int offset, Encoding enc, out DirectoryRecord record)
        {
            int length = src[offset + 0];

            record = new DirectoryRecord
            {
                ExtendedAttributeRecordLength = src[offset + 1],
                LocationOfExtent = IsoUtilities.ToUInt32FromBoth(src, offset + 2),
                DataLength = IsoUtilities.ToUInt32FromBoth(src, offset + 10),
                RecordingDateAndTime = IsoUtilities.ToUTCDateTimeFromDirectoryTime(src, offset + 18),
                Flags = (FileFlags)src[offset + 25],
                FileUnitSize = src[offset + 26],
                InterleaveGapSize = src[offset + 27],
                VolumeSequenceNumber = IsoUtilities.ToUInt16FromBoth(src, offset + 28)
            };
            byte lengthOfFileIdentifier = src[offset + 32];
            record.FileIdentifier = IsoUtilities.ReadChars(src, offset + 33, lengthOfFileIdentifier, enc);

            int padding = (lengthOfFileIdentifier & 1) == 0 ? 1 : 0;
            int startSystemArea = lengthOfFileIdentifier + padding + 33;
            int lenSystemArea = length - startSystemArea;
            if (lenSystemArea > 0)
            {
                record.SystemUseData = new byte[lenSystemArea];
                Array.Copy(src, offset + startSystemArea, record.SystemUseData, 0, lenSystemArea);
            }

            return length;
        }
    }
}