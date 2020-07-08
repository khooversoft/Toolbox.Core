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
    /// Vector string builder
    /// 
    /// Path is a string composed of part separated by a delimiter
    /// </summary>
    public class StringVectorBuilder : IEnumerable<string>
    {
        private readonly List<string> _parts;

        public StringVectorBuilder(string delimiter = "/")
        {
            _parts = new List<string>();
            Delimiter = delimiter;
        }

        /// <summary>
        /// Get or set part
        /// </summary>
        /// <param name="index">index of part</param>
        /// <returns>value at index</returns>
        public string this[int index]
        {
            get => _parts[index];
            set => _parts[index] = value;
        }

        /// <summary>
        /// Count of parts
        /// </summary>
        public int Count => _parts.Count;

        /// <summary>
        /// Delimiter
        /// </summary>
        public string Delimiter { get; set; }

        /// <summary>
        /// Has root, if true places delimiter in root
        /// </summary>
        public bool HasRoot { get; set; }

        /// <summary>
        /// Clear parts collection
        /// </summary>
        /// <returns></returns>
        public StringVectorBuilder Clear()
        {
            _parts.Clear();
            return this;
        }

        /// <summary>
        /// Set delimiter
        /// </summary>
        /// <param name="delimiter">delimiter to use</param>
        /// <returns>this</returns>
        public StringVectorBuilder SetDelimiter(string delimiter)
        {
            Delimiter = delimiter;
            return this;
        }

        /// <summary>
        /// Set has root
        /// </summary>
        /// <param name="hasRoot">use root if true</param>
        /// <returns>this</returns>
        public StringVectorBuilder SetHasRoot(bool hasRoot)
        {
            HasRoot = hasRoot;
            return this;
        }

        /// <summary>
        /// Parse values
        /// </summary>
        /// <param name="values"></param>
        /// <returns>this</returns>
        public StringVectorBuilder Parse(params string[] values)
        {
            string value = string.Join(Delimiter, values).Trim();
            HasRoot = value.Length >= Delimiter.Length && value.Substring(0, Delimiter.Length) == Delimiter;

            _parts.Clear();
            _parts.AddRange(value.Split(Delimiter, StringSplitOptions.RemoveEmptyEntries));

            return this;
        }

        /// <summary>
        /// Try get part at index
        /// </summary>
        /// <param name="index">index of part</param>
        /// <param name="value">returned value</param>
        /// <returns>true if part exist, false if not</returns>
        public bool TryGet(int index, out string value)
        {
            value = string.Empty;
            if (index < 0 || index >= _parts.Count) return false;

            value = _parts[index];
            return true;
        }

        /// <summary>
        /// Add part to 
        /// </summary>
        /// <param name="values"></param>
        /// <returns>this</returns>
        public StringVectorBuilder Add(params string[] values)
        {
            _parts.AddRange(values.SelectMany(x => x.Split(Delimiter, StringSplitOptions.RemoveEmptyEntries)));
            return this;
        }

        /// <summary>
        /// Remove part at index
        /// </summary>
        /// <param name="index">index to remove</param>
        /// <returns>this</returns>
        public StringVectorBuilder RemoveAt(int index)
        {
            _parts.RemoveAt(index);
            return this;
        }

        /// <summary>
        /// Build string path
        /// </summary>
        /// <returns>string path</returns>
        public StringVector Build() => new StringVector(_parts, Delimiter, HasRoot);

        public IEnumerator<string> GetEnumerator() => _parts.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _parts.GetEnumerator();

        /// <summary>
        /// + operator to add part
        /// </summary>
        /// <param name="builder">builder</param>
        /// <param name="value">value</param>
        /// <returns>builder</returns>
        public static StringVectorBuilder operator +(StringVectorBuilder builder, string value) => builder.Add(value);

        /// <summary>
        /// + operator to add parts
        /// </summary>
        /// <param name="builder">builder</param>
        /// <param name="values">value</param>
        /// <returns>builder</returns>
        public static StringVectorBuilder operator +(StringVectorBuilder builder, IEnumerable<string> values) => builder.Add(values.ToArray());
    }
}
