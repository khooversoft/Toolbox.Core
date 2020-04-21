// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Immutable string parts or vectors
    /// </summary>
    public class StringVector : IEnumerable<string>
    {
        private readonly string[] _parts;
        private readonly Deferred<string> _deferred;

        /// <summary>
        /// Default empty
        /// </summary>
        private StringVector()
            : this(Enumerable.Empty<string>(), "/", true)
        {
        }

        /// <summary>
        /// Default empty
        /// </summary>
        public StringVector(string value)
            : this(value, "/")
        {
        }

        /// <summary>
        /// Default empty
        /// </summary>
        public StringVector(bool hasRoot)
            : this(Enumerable.Empty<string>(), "/", hasRoot)
        {
        }

        public StringVector(string value, string delimiter)
        {
            value.VerifyNotEmpty(nameof(value));
            delimiter.VerifyNotEmpty(nameof(delimiter));

            Delimiter = delimiter;
            HasRoot = value.Length > 0 && value[0] == '/' ? true : false;

            _parts = value.Split(Delimiter, StringSplitOptions.RemoveEmptyEntries).ToArray();
            _deferred = new Deferred<string>(() => (HasRoot ? Delimiter : string.Empty) + string.Join(Delimiter, _parts));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parts">parts of path</param>
        /// <param name="delimiter">delimiter</param>
        /// <param name="hasRoot">has root</param>
        public StringVector(IEnumerable<string> parts, string delimiter, bool hasRoot)
        {
            delimiter.VerifyNotNull(nameof(parts));

            Delimiter = delimiter;
            HasRoot = hasRoot;

            _parts = parts.SelectMany(x => x.Split(Delimiter, StringSplitOptions.RemoveEmptyEntries)).ToArray();
            _deferred = new Deferred<string>(() => (HasRoot ? Delimiter : string.Empty) + string.Join(Delimiter, _parts));
        }

        /// <summary>
        /// Empty path with root
        /// </summary>
        public static StringVector Empty { get; } = new StringVector();

        public static StringVector EmptyNoRoot { get; } = new StringVector(false);

        public string this[int index] => _parts[index];

        public int Count => _parts.Length;

        public string Delimiter { get; }

        public bool HasRoot { get; }

        /// <summary>
        /// Try get part at index
        /// </summary>
        /// <param name="index">index of part</param>
        /// <param name="value">returned value</param>
        /// <returns>true if part exist, false if not</returns>
        public bool TryGet(int index, [MaybeNullWhen(returnValue: false)] out string? value)
        {
            value = default!;
            if (index < 0 || index >= _parts.Length) return false;
            value = _parts[index];
            return true;
        }

        /// <summary>
        /// Convert parts to string
        /// </summary>
        /// <returns>path</returns>
        public override string ToString() => _deferred.Value;

        /// <summary>
        /// Create new immutable string path with parts added
        /// </summary>
        /// <param name="parts"></param>
        /// <returns>new immutable string path object</returns>
        public StringVector With(params string[] parts) => new StringVector(_parts.Concat(parts), Delimiter, HasRoot);

        public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)_parts).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<string>)_parts).GetEnumerator();

        /// <summary>
        /// Implicit conversion to string
        /// </summary>
        /// <param name="source">source to convert</param>
        public static implicit operator string(StringVector source) => source.ToString();

        /// <summary>
        /// Parse path
        /// </summary>
        /// <param name="path">path string</param>
        /// <param name="delimiter">delimiter</param>
        /// <param name="hasRoot">has root</param>
        /// <returns></returns>
        public static StringVector Parse(string path, string delimiter = "/") => new StringVectorBuilder(delimiter).Parse(path).Build();

        /// <summary>
        /// Add operator, concatenate two string vectors
        /// </summary>
        /// <param name="subject">subject</param>
        /// <param name="rvalue">value to concatenate</param>
        /// <returns>new result string vector</returns>
        public static StringVector operator +(StringVector subject, StringVector rvalue) => subject.With(rvalue);

        /// <summary>
        /// Add operator, concatenate a string to a vector
        /// </summary>
        /// <param name="subject">subject</param>
        /// <param name="rvalue">value to concatenate</param>
        /// <returns>new result string vector</returns>
        public static StringVector operator +(StringVector subject, string rvalue) => subject.With(rvalue);
    }
}
