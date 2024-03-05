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
using System.Text;
using System.Text.RegularExpressions;

namespace LibIRD.DiscUtils.Internal
{
    internal static class Utilities
    {
        /// <summary>
        /// Converts between two arrays.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the source array.</typeparam>
        /// <typeparam name="U">The type of the elements of the destination array.</typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="func">The function to map from source type to destination type.</param>
        /// <returns>The resultant array.</returns>
        public static U[] Map<T, U>(ICollection<T> source, Func<T, U> func)
        {
            U[] result = new U[source.Count];
            int i = 0;

            foreach (T sVal in source)
            {
                result[i++] = func(sVal);
            }

            return result;
        }

        #region Path Manipulation

        /// <summary>
        /// Extracts the directory part of a path.
        /// </summary>
        /// <param name="path">The path to process.</param>
        /// <returns>The directory part.</returns>
        public static string GetDirectoryFromPath(string path)
        {
            string trimmed = path.TrimEnd('\\');

            int index = trimmed.LastIndexOf('\\');
            if (index < 0)
            {
                return string.Empty; // No directory, just a file name
            }

            return trimmed.Substring(0, index);
        }

        /// <summary>
        /// Extracts the file part of a path.
        /// </summary>
        /// <param name="path">The path to process.</param>
        /// <returns>The file part of the path.</returns>
        public static string GetFileFromPath(string path)
        {
            string trimmed = path.Trim('\\');

            int index = trimmed.LastIndexOf('\\');
            if (index < 0)
            {
                return trimmed; // No directory, just a file name
            }

            return trimmed.Substring(index + 1);
        }

        /// <summary>
        /// Combines two paths.
        /// </summary>
        /// <param name="a">The first part of the path.</param>
        /// <param name="b">The second part of the path.</param>
        /// <returns>The combined path.</returns>
        public static string CombinePaths(string a, string b)
        {
            if (string.IsNullOrEmpty(a) || (b.Length > 0 && b[0] == '\\'))
            {
                return b;
            }
            if (string.IsNullOrEmpty(b))
            {
                return a;
            }
            return a.TrimEnd('\\') + '\\' + b.TrimStart('\\');
        }

        /// <summary>
        /// Resolves a relative path into an absolute one.
        /// </summary>
        /// <param name="basePath">The base path to resolve from.</param>
        /// <param name="relativePath">The relative path.</param>
        /// <returns>The absolute path. If no <paramref name="basePath"/> is specified
        /// then relativePath is returned as-is. If <paramref name="relativePath"/>
        /// contains more '..' characters than the base path contains levels of 
        /// directory, the resultant string be the root drive followed by the file name.
        /// If no the basePath starts with '\' (no drive specified) then the returned
        /// path will also start with '\'.
        /// For example: (\TEMP\Foo.txt, ..\..\Bar.txt) gives (\Bar.txt).
        /// </returns>
        public static string ResolveRelativePath(string basePath, string relativePath)
        {
            if (string.IsNullOrEmpty(basePath))
            {
                return relativePath;
            }

            if (!basePath.EndsWith(@"\"))
                basePath = Path.GetDirectoryName(basePath);

            string merged = Path.GetFullPath(Path.Combine(basePath, relativePath));

            if (basePath.StartsWith(@"\") && merged.Length > 2 && merged[1].Equals(':'))
            {
                return merged.Substring(2);
            }

            return merged;
        }

        public static string ResolvePath(string basePath, string path)
        {
            if (!path.StartsWith("\\", StringComparison.OrdinalIgnoreCase))
            {
                return ResolveRelativePath(basePath, path);
            }
            return path;
        }

        #endregion
        
        #region Filesystem Support

        /// <summary>
        /// Converts a 'standard' wildcard file/path specification into a regular expression.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to convert.</param>
        /// <returns>The resultant regular expression.</returns>
        /// <remarks>
        /// The wildcard * (star) matches zero or more characters (including '.'), and ?
        /// (question mark) matches precisely one character (except '.').
        /// </remarks>
        public static Regex ConvertWildcardsToRegEx(string pattern)
        {
            if (!pattern.Contains("."))
            {
                pattern += ".";
            }

            string query = "^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", "[^.]") + "$";
            return new Regex(query, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        public static FileAttributes FileAttributesFromUnixFileType(UnixFileType fileType)
        {
            return fileType switch
            {
                UnixFileType.Fifo => FileAttributes.Device | FileAttributes.System,
                UnixFileType.Character => FileAttributes.Device | FileAttributes.System,
                UnixFileType.Directory => FileAttributes.Directory,
                UnixFileType.Block => FileAttributes.Device | FileAttributes.System,
                UnixFileType.Regular => FileAttributes.Normal,
                UnixFileType.Link => FileAttributes.ReparsePoint,
                UnixFileType.Socket => FileAttributes.Device | FileAttributes.System,
                _ => 0,
            };
        }

        #endregion
    }
}
