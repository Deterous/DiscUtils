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
        public VfsCDReader(Stream data)
            : base(new DiscFileSystemOptions())
        {
            _data = data;

            long vdpos = 0x8000; // Skip lead-in

            byte[] buffer = new byte[IsoUtilities.SectorSize];

            long pvdPos = 0;
            long svdPos = 0;

            BaseVolumeDescriptor bvd;
            do
            {
                data.Position = vdpos;
                int numRead = data.Read(buffer, 0, IsoUtilities.SectorSize);
                if (numRead != IsoUtilities.SectorSize)
                {
                    break;
                }

                bvd = new BaseVolumeDescriptor(buffer, 0);

                if (bvd.StandardIdentifier != BaseVolumeDescriptor.Iso9660StandardIdentifier)
                {
                    throw new IOException("Volume is not ISO-9660");
                }

                switch (bvd.VolumeDescriptorType)
                {
                    case VolumeDescriptorType.Boot:
                        break;

                    case VolumeDescriptorType.Primary: // Primary Vol Descriptor
                        pvdPos = vdpos;
                        break;

                    case VolumeDescriptorType.Supplementary: // Supplementary Vol Descriptor
                        svdPos = vdpos;
                        break;

                    case VolumeDescriptorType.Partition: // Volume Partition Descriptor
                        break;
                    case VolumeDescriptorType.SetTerminator: // Volume Descriptor Set Terminator
                        break;
                }

                vdpos += IsoUtilities.SectorSize;
            } while (bvd.VolumeDescriptorType != VolumeDescriptorType.SetTerminator);

            ActiveVariant = Iso9660Variant.None;
            Iso9660Variant[] variantPriorities = [Iso9660Variant.Joliet, Iso9660Variant.RockRidge, Iso9660Variant.Iso9660];
            foreach (Iso9660Variant variant in variantPriorities)
            {
                switch (variant)
                {
                    case Iso9660Variant.Joliet:
                        if (svdPos != 0)
                        {
                            data.Position = svdPos;
                            data.Read(buffer, 0, IsoUtilities.SectorSize);
                            SupplementaryVolumeDescriptor volDesc = new SupplementaryVolumeDescriptor(buffer, 0);

                            Context = new IsoContext { VolumeDescriptor = volDesc, DataStream = _data };
                            RootDirectory = new ReaderDirectory(Context,
                                new ReaderDirEntry(Context, volDesc.RootDirectory));
                            ActiveVariant = Iso9660Variant.Iso9660;
                        }

                        break;

                    case Iso9660Variant.RockRidge:
                    case Iso9660Variant.Iso9660:
                        if (pvdPos != 0)
                        {
                            data.Position = pvdPos;
                            data.Read(buffer, 0, IsoUtilities.SectorSize);
                            PrimaryVolumeDescriptor volDesc = new PrimaryVolumeDescriptor(buffer, 0);

                            IsoContext context = new IsoContext { VolumeDescriptor = volDesc, DataStream = _data };
                            DirectoryRecord rootSelfRecord = ReadRootSelfRecord(context);

                            InitializeSusp(context, rootSelfRecord);

                            if (variant == Iso9660Variant.Iso9660
                                ||
                                (variant == Iso9660Variant.RockRidge &&
                                 !string.IsNullOrEmpty(context.RockRidgeIdentifier)))
                            {
                                Context = context;
                                RootDirectory = new ReaderDirectory(context, new ReaderDirEntry(context, rootSelfRecord));
                                ActiveVariant = variant;
                            }
                        }

                        break;
                }

                if (ActiveVariant != Iso9660Variant.None)
                {
                    break;
                }
            }

            if (ActiveVariant == Iso9660Variant.None)
            {
                throw new IOException("None of the permitted ISO9660 file system variants was detected");
            }
        }

        public Iso9660Variant ActiveVariant { get; }

        public Range<long, long>[] PathToClusters(string path)
        {
            ReaderDirEntry entry = GetDirectoryEntry(path);

            if (entry == null)
            {
                throw new FileNotFoundException("File not found", path);
            }
            
            if (entry.IsDirectory)
            {
                if (entry.Record.FileUnitSize != 0 || entry.Record.InterleaveGapSize != 0)
                {
                    throw new NotSupportedException("Non-contiguous extents not supported");
                }

                return new[]
                {
                    new Range<long, long>(entry.Record.LocationOfExtent,
                        (entry.Record.DataLength + (IsoUtilities.SectorSize - 1)) / IsoUtilities.SectorSize)
                };
            }

            int index = path.LastIndexOf('\\'); // Path.DirectorySeparatorChar ?
            string dir = path.Substring(0, index + 1);
            string filename = path.Substring(index + 1);

            // Get all entries for file
            ReaderDirectory rdr = new ReaderDirectory(Context, GetDirectoryEntry(dir));
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

        private static void InitializeSusp(IsoContext context, DirectoryRecord rootSelfRecord)
        {
            // Stage 1 - SUSP present?
            List<SuspExtension> extensions = new List<SuspExtension>();
            if (!SuspRecords.DetectSharingProtocol(rootSelfRecord.SystemUseData, 0))
            {
                context.SuspExtensions = new List<SuspExtension>();
                context.SuspDetected = false;
                return;
            }
            context.SuspDetected = true;

            SuspRecords suspRecords = new SuspRecords(context, rootSelfRecord.SystemUseData, 0);

            // Stage 2 - Init general SUSP params
            SharingProtocolSystemUseEntry spEntry =
                (SharingProtocolSystemUseEntry)suspRecords.GetEntries(null, "SP")[0];
            context.SuspSkipBytes = spEntry.SystemAreaSkip;

            // Stage 3 - Init extensions
            List<SystemUseEntry> extensionEntries = suspRecords.GetEntries(null, "ER");
            if (extensionEntries != null)
            {
                foreach (ExtensionSystemUseEntry extension in extensionEntries)
                {
                    switch (extension.ExtensionIdentifier)
                    {
                        case "RRIP_1991A":
                        case "IEEE_P1282":
                        case "IEEE_1282":
                            extensions.Add(new RockRidgeExtension(extension.ExtensionIdentifier));
                            context.RockRidgeIdentifier = extension.ExtensionIdentifier;
                            break;

                        default:
                            extensions.Add(new GenericSuspExtension(extension.ExtensionIdentifier));
                            break;
                    }
                }
            }
            else if (suspRecords.GetEntries(null, "RR") != null)
            {
                // Some ISO creators don't add the 'ER' record for RockRidge, but write the (legacy)
                // RR record anyway
                extensions.Add(new RockRidgeExtension("RRIP_1991A"));
                context.RockRidgeIdentifier = "RRIP_1991A";
            }

            context.SuspExtensions = extensions;
        }

        private static DirectoryRecord ReadRootSelfRecord(IsoContext context)
        {
            context.DataStream.Position = context.VolumeDescriptor.RootDirectory.LocationOfExtent *
                                          context.VolumeDescriptor.LogicalBlockSize;
            byte[] firstSector = StreamUtilities.ReadExact(context.DataStream, context.VolumeDescriptor.LogicalBlockSize);

            DirectoryRecord rootSelfRecord;
            DirectoryRecord.ReadFrom(firstSector, 0, context.VolumeDescriptor.CharacterEncoding, out rootSelfRecord);
            return rootSelfRecord;
        }
    }
}
