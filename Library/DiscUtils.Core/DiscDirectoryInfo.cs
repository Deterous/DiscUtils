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
using LibIRD.DiscUtils.Internal;

namespace LibIRD.DiscUtils
{
    /// <summary>
    /// Provides information about a directory on a disc.
    /// </summary>
    /// <remarks>
    /// This class allows navigation of the disc directory/file hierarchy.
    /// </remarks>
    public sealed class DiscDirectoryInfo : DiscFileSystemInfo
    {
        /// <summary>
        /// Initializes a new instance of the DiscDirectoryInfo class.
        /// </summary>
        /// <param name="fileSystem">The file system the directory info relates to.</param>
        /// <param name="path">The path within the file system of the directory.</param>
        internal DiscDirectoryInfo(DiscFileSystem fileSystem, string path)
            : base(fileSystem, path) {}

        /// <summary>
        /// Gets the full path of the directory.
        /// </summary>
        public override string FullName
        {
            get { return base.FullName + @"\"; }
        }

        /// <summary>
        /// Gets all child directories.
        /// </summary>
        /// <returns>An array of child directories.</returns>
        public DiscDirectoryInfo[] GetDirectories()
        {
            return Utilities.Map(FileSystem.GetDirectories(Path),
                p => new DiscDirectoryInfo(FileSystem, p));
        }

        /// <summary>
        /// Gets all files.
        /// </summary>
        /// <returns>An array of files.</returns>
        public DiscFileInfo[] GetFiles()
        {
            return Utilities.Map(FileSystem.GetFiles(Path), p => new DiscFileInfo(FileSystem, p));
        }
    }
}