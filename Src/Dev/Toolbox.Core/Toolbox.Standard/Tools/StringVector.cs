// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
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

        public StringVector(string value, string delimiter)
        {
            value.Verify(nameof(value)).IsNotEmpty();
            delimiter.Verify(nameof(delimiter)).IsNotEmpty();

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
            delimiter.Verify(nameof(parts)).IsNotEmpty();

            Delimiter = delimiter;
            HasRoot = hasRoot;

            _parts = parts.SelectMany(x => x.Split(Delimiter, StringSplitOptions.RemoveEmptyEntries)).ToArray();
            _deferred = new Deferred<string>(() => (HasRoot ? Delimiter : string.Empty) + string.Join(Delimiter, _parts));
        }

        /// <summary>
        /// Empty path with root
        /// </summary>
        public static StringVector Empty { get; } = new StringVector();

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
        public bool TryGet(int index, out string value)
        {
            value = string.Empty;
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
    }
}
