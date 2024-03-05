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
using LibIRD.DiscUtils.Internal;
using LibIRD.DiscUtils.Streams;

namespace LibIRD.DiscUtils.Iso9660
{
    internal class CommonVolumeDescriptor : BaseVolumeDescriptor
    {
        public string AbstractFileIdentifier;
        public string ApplicationIdentifier;
        public string BibliographicFileIdentifier;
        public Encoding CharacterEncoding;
        public string CopyrightFileIdentifier;
        public DateTime CreationDateAndTime;
        public string DataPreparerIdentifier;
        public DateTime EffectiveDateAndTime;
        public DateTime ExpirationDateAndTime;
        public byte FileStructureVersion;
        public ushort LogicalBlockSize;
        public DateTime ModificationDateAndTime;
        public uint OptionalTypeLPathTableLocation;
        public uint OptionalTypeMPathTableLocation;
        public uint PathTableSize;
        public string PublisherIdentifier;
        public DirectoryRecord RootDirectory;

        //public string SystemIdentifier;
        public uint TypeLPathTableLocation;
        public uint TypeMPathTableLocation;
        public string VolumeIdentifier;
        public ushort VolumeSequenceNumber;
        public string VolumeSetIdentifier;
        public ushort VolumeSetSize;
        public uint VolumeSpaceSize;

        public CommonVolumeDescriptor(byte[] src, int offset, Encoding enc)
            : base(src, offset)
        {
            CharacterEncoding = enc;

            //SystemIdentifier = IsoUtilities.ReadChars(src, offset + 8, 32, CharacterEncoding);
            VolumeIdentifier = IsoUtilities.ReadChars(src, offset + 40, 32, CharacterEncoding);
            VolumeSpaceSize = IsoUtilities.ToUInt32FromBoth(src, offset + 80);
            VolumeSetSize = IsoUtilities.ToUInt16FromBoth(src, offset + 120);
            VolumeSequenceNumber = IsoUtilities.ToUInt16FromBoth(src, offset + 124);
            LogicalBlockSize = IsoUtilities.ToUInt16FromBoth(src, offset + 128);
            PathTableSize = IsoUtilities.ToUInt32FromBoth(src, offset + 132);
            TypeLPathTableLocation = EndianUtilities.ToUInt32LittleEndian(src, offset + 140);
            OptionalTypeLPathTableLocation = EndianUtilities.ToUInt32LittleEndian(src, offset + 144);
            TypeMPathTableLocation = Utilities.BitSwap(EndianUtilities.ToUInt32LittleEndian(src, offset + 148));
            OptionalTypeMPathTableLocation = Utilities.BitSwap(EndianUtilities.ToUInt32LittleEndian(src, offset + 152));
            DirectoryRecord.ReadFrom(src, offset + 156, CharacterEncoding, out RootDirectory);
            VolumeSetIdentifier = IsoUtilities.ReadChars(src, offset + 190, 318 - 190, CharacterEncoding);
            PublisherIdentifier = IsoUtilities.ReadChars(src, offset + 318, 446 - 318, CharacterEncoding);
            DataPreparerIdentifier = IsoUtilities.ReadChars(src, offset + 446, 574 - 446, CharacterEncoding);
            ApplicationIdentifier = IsoUtilities.ReadChars(src, offset + 574, 702 - 574, CharacterEncoding);
            CopyrightFileIdentifier = IsoUtilities.ReadChars(src, offset + 702, 739 - 702, CharacterEncoding);
            AbstractFileIdentifier = IsoUtilities.ReadChars(src, offset + 739, 776 - 739, CharacterEncoding);
            BibliographicFileIdentifier = IsoUtilities.ReadChars(src, offset + 776, 813 - 776, CharacterEncoding);
            CreationDateAndTime = IsoUtilities.ToDateTimeFromVolumeDescriptorTime(src, offset + 813);
            ModificationDateAndTime = IsoUtilities.ToDateTimeFromVolumeDescriptorTime(src, offset + 830);
            ExpirationDateAndTime = IsoUtilities.ToDateTimeFromVolumeDescriptorTime(src, offset + 847);
            EffectiveDateAndTime = IsoUtilities.ToDateTimeFromVolumeDescriptorTime(src, offset + 864);
            FileStructureVersion = src[offset + 881];
        }
    }
}