using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Khooversoft.Toolbox.Standard;
using KHooversoft.Toolbox.Graph;

namespace Khooversoft.Toolbox.Run
{
    /// <summary>
    /// Record a sequence of activity instructions
    /// </summary>
    public class Sequence : IActivityCommon, IEnumerable<IActivityCommon>
    {
        private readonly IList<IActivityCommon> _list = new List<IActivityCommon>();

        /// <summary>
        /// Construct sequence
        /// </summary>
        public Sequence() { }

        public Sequence(string nameSpace) => Namespace = nameSpace.VerifyNotNull(nameof(nameSpace));

        public string? Namespace { get; }

        /// <summary>
        /// Add common activity to sequence
        /// </summary>
        /// <param name="node">common activity</param>
        public void Add(IActivityCommon node)
        {
            node.VerifyNotNull(nameof(node));

            _list.Add(node);
        }

        public IEnumerator<IActivityCommon> GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

        /// <summary>
        /// Add common activity to sequence
        /// </summary>
        /// <param name="subject">sequence to add to</param>
        /// <param name="rvalue">activity to add</param>
        /// <returns>sequence</returns>
        public static Sequence operator +(Sequence subject, IActivityCommon rvalue)
        {
            subject.Add(rvalue);
            return subject;
        }
    }
}
