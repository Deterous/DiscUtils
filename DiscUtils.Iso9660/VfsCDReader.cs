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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibIRD.DiscUtils.Streams;
using LibIRD.DiscUtils.Vfs;

namespace LibIRD.DiscUtils.Iso9660
{
    internal class VfsCDReader : VfsReadOnlyFileSystem<ReaderDirEntry, File, ReaderDirectory, IsoContext>
    {
        private readonly Stream _data;

        /// <summary>
        /// Initializes a new instance of the VfsCDReader class.
        /// </summary>
        /// <param name="data">The stream to read the ISO image from.</param>
        /// <param name="variantPriorities">Which possible file system variants to use, and with which priority.</param>
        /// <param name="hideVersions">Hides version numbers (e.g. ";1") from the end of files.</param>
        /// <remarks>
        /// <para>
        /// The implementation considers each of the file system variants in <c>variantProperties</c> and selects
        /// the first which is determined to be present.  In this example Joliet, then Rock Ridge, then vanilla
        /// Iso9660 will be considered:
        /// </para>
        /// <code lang="cs">
        /// VfsCDReader(stream, new Iso9660Variant[] {Joliet, RockRidge, Iso9660}, true);
        /// </code>
        /// <para>The Iso9660 variant should normally be specified as the final entry in the list.  Placing it earlier
        /// in the list will effectively mask later items and not including it may prevent some ISOs from being read.</para>
        /// </remarks>
        public VfsCDReader(Stream data) : base()
        {
            _data = data;

            long pos = 0x8000; // Skip lead-in

            byte[] buffer = new byte[IsoUtilities.SectorSize];

            long svdPos = 0;

            BaseVolumeDescriptor bvd;
            do
            {
                data.Position = pos;
                int numRead = data.Read(buffer, 0, IsoUtilities.SectorSize);
                if (numRead != IsoUtilities.SectorSize)
                    break;

                bvd = new BaseVolumeDescriptor(buffer, 0);

                if (bvd.StandardIdentifier != BaseVolumeDescriptor.Iso9660StandardIdentifier)
                    throw new IOException("Volume is not ISO-9660");

                switch (bvd.VolumeDescriptorType)
                {
                    case VolumeDescriptorType.Supplementary: // Supplementary Vol Descriptor
                        svdPos = pos;
                        break;

                    case VolumeDescriptorType.Boot:
                    case VolumeDescriptorType.Primary: // Primary Vol Descriptor
                    case VolumeDescriptorType.Partition: // Volume Partition Descriptor
                    case VolumeDescriptorType.SetTerminator: // Volume Descriptor Set Terminator
                    default:
                        break;
                }

                pos += IsoUtilities.SectorSize;
            } while (bvd.VolumeDescriptorType != VolumeDescriptorType.SetTerminator);

            if (svdPos == 0)
                throw new IOException("ISO9660 file system with Joliet extension was not detected");
            
            data.Position = svdPos;
            data.Read(buffer, 0, IsoUtilities.SectorSize);

            Encoding enc = Encoding.ASCII;
            // Joliet = BigEndianUnicode
            if (buffer[88 + 0] == 0x25 && buffer[88 + 1] == 0x2F && (buffer[88 + 2] == 0x40 || buffer[88 + 2] == 0x43 || buffer[88 + 2] == 0x45))
                enc = Encoding.BigEndianUnicode;

            // Supplementary Volume Descriptor
            CommonVolumeDescriptor volDesc = new(buffer, 0, enc);

            Context = new IsoContext { VolumeDescriptor = volDesc, DataStream = _data };
            RootDirectory = new ReaderDirectory(Context, new ReaderDirEntry(volDesc.RootDirectory));
        }

        public Range<long, long>[] PathToClusters(string path)
        {
            ReaderDirEntry entry = GetDirectoryEntry(path) ?? throw new FileNotFoundException("File not found", path);

            if (entry.IsDirectory)
            {
                if (entry.Record.FileUnitSize != 0 || entry.Record.InterleaveGapSize != 0)
                    throw new NotSupportedException("Non-contiguous extents not supported");

                return
                [
                    new Range<long, long>(entry.Record.LocationOfExtent,
                        (entry.Record.DataLength + (IsoUtilities.SectorSize - 1)) / IsoUtilities.SectorSize)
                ];
            }

            int index = path.LastIndexOf('\\');
            string dir = path.Substring(0, index + 1);
            string filename = path.Substring(index + 1);

            // Get all entries for file
            ReaderDirectory rdr = new(Context, GetDirectoryEntry(dir));
            IEnumerable<ReaderDirEntry> dirEntries = rdr.GetEntriesByName(filename);

            // Return array of all clusters
            return dirEntries.Select(d => new Range<long, long>(d.Record.LocationOfExtent, d.Record.DataLength)).ToArray();

        }

        protected override File ConvertDirEntryToFile(ReaderDirEntry dirEntry)
        {
            if (dirEntry.IsDirectory)
            {
                return new ReaderDirectory(Context, dirEntry);
            }
            return new File(Context, dirEntry);
        }

        protected override string FormatFileName(string name)
        {
            int pos = name.LastIndexOf(';');
            if (pos > 0)
            {
                return name.Substring(0, pos);
                }

            return name;
        }
    }
}
