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

using System.IO;
using LibIRD.DiscUtils.Streams;
using LibIRD.DiscUtils.Vfs;

namespace LibIRD.DiscUtils.Iso9660
{
    /// <summary>
    /// Class for reading existing ISO images.
    /// </summary>
    public class CDReader : VfsFileSystemFacade
    {
        /// <summary>
        /// Initializes a new instance of the CDReader class.
        /// </summary>
        /// <param name="data">The stream to read the ISO image from.</param>
        /// <param name="joliet">Whether to read Joliet extensions.</param>
        public CDReader(Stream data)
            : base(new VfsCDReader(data)) {}

        /// <summary>
        /// Converts a file name to the list of clusters occupied by the file's data.
        /// </summary>
        /// <param name="path">The path to inspect.</param>
        /// <returns>The clusters.</returns>
        /// <remarks>Note that in some file systems, small files may not have dedicated
        /// clusters.  Only dedicated clusters will be returned.</remarks>
        public Range<long, long>[] PathToClusters(string path)
        {
            return GetRealFileSystem<VfsCDReader>().PathToClusters(path);
        }

        /// <summary>
        /// Detects if a stream contains a valid ISO file system.
        /// </summary>
        /// <param name="data">The stream to inspect.</param>
        /// <returns><c>true</c> if the stream contains an ISO file system, else false.</returns>
        public static bool Detect(Stream data)
        {
            byte[] buffer = new byte[IsoUtilities.SectorSize];

            if (data.Length < 0x8000 + IsoUtilities.SectorSize)
            {
                return false;
            }

            data.Position = 0x8000;
            int numRead = StreamUtilities.ReadMaximum(data, buffer, 0, IsoUtilities.SectorSize);
            if (numRead != IsoUtilities.SectorSize)
            {
                return false;
            }

            BaseVolumeDescriptor bvd = new(buffer, 0);

            return bvd.StandardIdentifier == BaseVolumeDescriptor.Iso9660StandardIdentifier;
        }
    }
}