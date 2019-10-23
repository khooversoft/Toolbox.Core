using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolbox.Standard
{
    /// <summary>
    /// Immutable string parts or vectors
    /// </summary>
    public class StringPath : IEnumerable<string>
    {
        private readonly string[] _parts;
        private readonly Deferred<string> _deferred;

        /// <summary>
        /// Default empty
        /// </summary>
        private StringPath()
            : this(Enumerable.Empty<string>(), "/", true)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parts">parts of path</param>
        /// <param name="delimiter">delimiter</param>
        /// <param name="hasRoot">has root</param>
        public StringPath(IEnumerable<string> parts, string delimiter, bool hasRoot)
        {
            _parts = parts.ToArray();
            Delimiter = delimiter;
            HasRoot = hasRoot;
            _deferred = new Deferred<string>(() => (HasRoot ? Delimiter : string.Empty) + string.Join(Delimiter, _parts));
        }

        /// <summary>
        /// Empty path with root
        /// </summary>
        public static StringPath Empty { get; } = new StringPath();

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
        public StringPath With(params string[] parts) => new StringPath(_parts.Concat(parts), Delimiter, HasRoot);

        public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)_parts).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<string>)_parts).GetEnumerator();

        /// <summary>
        /// Implicit conversion to string
        /// </summary>
        /// <param name="source">source to convert</param>
        public static implicit operator string(StringPath source) => source.ToString();

        /// <summary>
        /// Parse path
        /// </summary>
        /// <param name="path">path string</param>
        /// <param name="delimiter">delimiter</param>
        /// <param name="hasRoot">has root</param>
        /// <returns></returns>
        public static StringPath Parse(string path, string delimiter = "/") => new StringPathBuilder(delimiter).Parse(path).Build();
    }
}
