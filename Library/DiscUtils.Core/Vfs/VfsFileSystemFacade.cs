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
using LibIRD.DiscUtils.Streams;

namespace LibIRD.DiscUtils.Vfs
{
    /// <summary>
    /// Base class for the public facade on a file system.
    /// </summary>
    /// <remarks>
    /// The derived class can extend the functionality available from a file system
    /// beyond that defined by DiscFileSystem.
    /// </remarks>
    public abstract class VfsFileSystemFacade : DiscFileSystem
    {
        private readonly DiscFileSystem _wrapped;

        /// <summary>
        /// Initializes a new instance of the VfsFileSystemFacade class.
        /// </summary>
        /// <param name="toWrap">The actual file system instance.</param>
        protected VfsFileSystemFacade(DiscFileSystem toWrap)
        {
            _wrapped = toWrap;
        }

        /// <summary>
        /// Gets a value indicating whether the file system is thread-safe.
        /// </summary>
        public override bool IsThreadSafe
        {
            get { return _wrapped.IsThreadSafe; }
        }

        /// <summary>
        /// Gets the file system options, which can be modified.
        /// </summary>
        public override DiscFileSystemOptions Options
        {
            get { return _wrapped.Options; }
        }

        /// <summary>
        /// Gets the root directory of the file system.
        /// </summary>
        public override DiscDirectoryInfo Root
        {
            get { return new DiscDirectoryInfo(this, string.Empty); }
        }

        /// <summary>
        /// Indicates if a directory exists.
        /// </summary>
        /// <param name="path">The path to test.</param>
        /// <returns>true if the directory exists.</returns>
        public override bool DirectoryExists(string path)
        {
            return _wrapped.DirectoryExists(path);
        }

        /// <summary>
        /// Indicates if a file exists.
        /// </summary>
        /// <param name="path">The path to test.</param>
        /// <returns>true if the file exists.</returns>
        public override bool FileExists(string path)
        {
            return _wrapped.FileExists(path);
        }

        /// <summary>
        /// Indicates if a file or directory exists.
        /// </summary>
        /// <param name="path">The path to test.</param>
        /// <returns>true if the file or directory exists.</returns>
        public override bool Exists(string path)
        {
            return _wrapped.Exists(path);
        }

        /// <summary>
        /// Gets the names of subdirectories in a specified directory.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <returns>Array of directories.</returns>
        public override string[] GetDirectories(string path)
        {
            return _wrapped.GetDirectories(path);
        }

        /// <summary>
        /// Gets the names of subdirectories in a specified directory matching a specified
        /// search pattern.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="searchPattern">The search string to match against.</param>
        /// <returns>Array of directories matching the search pattern.</returns>
        public override string[] GetDirectories(string path, string searchPattern)
        {
            return _wrapped.GetDirectories(path, searchPattern);
        }

        /// <summary>
        /// Gets the names of subdirectories in a specified directory matching a specified
        /// search pattern, using a value to determine whether to search subdirectories.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="searchPattern">The search string to match against.</param>
        /// <param name="searchOption">Indicates whether to search subdirectories.</param>
        /// <returns>Array of directories matching the search pattern.</returns>
        public override string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            return _wrapped.GetDirectories(path, searchPattern, searchOption);
        }

        /// <summary>
        /// Gets the names of files in a specified directory.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <returns>Array of files.</returns>
        public override string[] GetFiles(string path)
        {
            return _wrapped.GetFiles(path);
        }

        /// <summary>
        /// Gets the names of files in a specified directory.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="searchPattern">The search string to match against.</param>
        /// <returns>Array of files matching the search pattern.</returns>
        public override string[] GetFiles(string path, string searchPattern)
        {
            return _wrapped.GetFiles(path, searchPattern);
        }

        /// <summary>
        /// Gets the names of files in a specified directory matching a specified
        /// search pattern, using a value to determine whether to search subdirectories.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="searchPattern">The search string to match against.</param>
        /// <param name="searchOption">Indicates whether to search subdirectories.</param>
        /// <returns>Array of files matching the search pattern.</returns>
        public override string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return _wrapped.GetFiles(path, searchPattern, searchOption);
        }

        /// <summary>
        /// Gets the names of all files and subdirectories in a specified directory.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <returns>Array of files and subdirectories matching the search pattern.</returns>
        public override string[] GetFileSystemEntries(string path)
        {
            return _wrapped.GetFileSystemEntries(path);
        }

        /// <summary>
        /// Gets the names of files and subdirectories in a specified directory matching a specified
        /// search pattern.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="searchPattern">The search string to match against.</param>
        /// <returns>Array of files and subdirectories matching the search pattern.</returns>
        public override string[] GetFileSystemEntries(string path, string searchPattern)
        {
            return _wrapped.GetFileSystemEntries(path, searchPattern);
        }

        /// <summary>
        /// Opens the specified file.
        /// </summary>
        /// <param name="path">The full path of the file to open.</param>
        /// <param name="mode">The file mode for the created stream.</param>
        /// <returns>The new stream.</returns>
        public override SparseStream OpenFile(string path, FileMode mode)
        {
            return _wrapped.OpenFile(path, mode);
        }

        /// <summary>
        /// Opens the specified file.
        /// </summary>
        /// <param name="path">The full path of the file to open.</param>
        /// <param name="mode">The file mode for the created stream.</param>
        /// <param name="access">The access permissions for the created stream.</param>
        /// <returns>The new stream.</returns>
        public override SparseStream OpenFile(string path, FileMode mode, FileAccess access)
        {
            return _wrapped.OpenFile(path, mode, access);
        }

        /// <summary>
        /// Gets the length of a file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>The length in bytes.</returns>
        public override long GetFileLength(string path)
        {
            return _wrapped.GetFileLength(path);
        }

        /// <summary>
        /// Gets an object representing a possible file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>The representing object.</returns>
        /// <remarks>The file does not need to exist.</remarks>
        public override DiscFileInfo GetFileInfo(string path)
        {
            return new DiscFileInfo(this, path);
        }

        /// <summary>
        /// Gets an object representing a possible directory.
        /// </summary>
        /// <param name="path">The directory path.</param>
        /// <returns>The representing object.</returns>
        /// <remarks>The directory does not need to exist.</remarks>
        public override DiscDirectoryInfo GetDirectoryInfo(string path)
        {
            return new DiscDirectoryInfo(this, path);
        }

        /// <summary>
        /// Gets an object representing a possible file system object (file or directory).
        /// </summary>
        /// <param name="path">The file system path.</param>
        /// <returns>The representing object.</returns>
        /// <remarks>The file system object does not need to exist.</remarks>
        public override DiscFileSystemInfo GetFileSystemInfo(string path)
        {
            return new DiscFileSystemInfo(this, path);
        }

        /// <summary>
        /// Provides access to the actual file system implementation.
        /// </summary>
        /// <typeparam name="T">The concrete type of the actual file system.</typeparam>
        /// <returns>The actual file system instance.</returns>
        protected T GetRealFileSystem<T>()
            where T : DiscFileSystem
        {
            return (T)_wrapped;
        }
    }
}
