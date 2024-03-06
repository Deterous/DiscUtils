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

namespace LibIRD.DiscUtils.Vfs
{
    /// <summary>
    /// Base class for read-only file system implementations.
    /// </summary>
    /// <typeparam name="TDirEntry">The concrete type representing directory entries.</typeparam>
    /// <typeparam name="TFile">The concrete type representing files.</typeparam>
    /// <typeparam name="TDirectory">The concrete type representing directories.</typeparam>
    /// <typeparam name="TContext">The concrete type holding global state.</typeparam>
    public abstract class VfsReadOnlyFileSystem<TDirEntry, TFile, TDirectory, TContext> :
            VfsFileSystem<TDirEntry, TFile, TDirectory, TContext>
        where TDirEntry : VfsDirEntry
        where TFile : IVfsFile
        where TDirectory : class, IVfsDirectory<TDirEntry, TFile>, TFile
        where TContext : VfsContext
    {
        /// <summary>
        /// Initializes a new instance of the VfsReadOnlyFileSystem class.
        /// </summary>
        protected VfsReadOnlyFileSystem() : base() {}

        /// <summary>
        /// Opens the specified file.
        /// </summary>
        /// <param name="path">The full path of the file to open.</param>
        /// <param name="mode">The file mode for the created stream.</param>
        /// <returns>The new stream.</returns>
        public override SparseStream OpenFile(string path, FileMode mode)
        {
            return OpenFile(path, mode, FileAccess.Read);
        }
    }
}