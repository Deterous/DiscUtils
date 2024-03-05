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

namespace LibIRD.DiscUtils.Streams
{
    /// <summary>
    /// Represents a range of values.
    /// </summary>
    /// <typeparam name="TOffset">The type of the offset element.</typeparam>
    /// <typeparam name="TCount">The type of the size element.</typeparam>
    /// <remarks>
    /// Initializes a new instance of the Range class.
    /// </remarks>
    /// <param name="offset">The offset (i.e. start) of the range.</param>
    /// <param name="count">The size of the range.</param>
    public class Range<TOffset, TCount>(TOffset offset, TCount count)
    {

        /// <summary>
        /// Gets the size of the range.
        /// </summary>
        public TCount Count { get; } = count;

        /// <summary>
        /// Gets the offset (i.e. start) of the range.
        /// </summary>
        public TOffset Offset { get; } = offset;
    }
}