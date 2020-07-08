// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KHooversoft.Toolbox.Graph
{
    public static class GraphCycleExtensions
    {
        private enum VisitState
        {
            NotVisited,
            Visiting,
            Visited
        };

        public static IList<IList<T>> FindCycles<T>(this IEnumerable<T> nodes, Func<T, IEnumerable<T>> edges)
        {
            var cycles = new List<IList<T>>();
            var visited = new Dictionary<T, VisitState>();

            foreach (var node in nodes)
            {
                cycles.AddRange(FindCycles(node, edges, visited));
            }

            return cycles;
        }

        private static void TryPush<T>(T node, Func<T, IEnumerable<T>> lookup, Stack<KeyValuePair<T, IEnumerator<T>>> stack, Dictionary<T, VisitState> visited, List<List<T>> cycles)
        {
            if (!visited.TryGetValue(node, out VisitState state))
            {
                state = VisitState.NotVisited;
            }

            switch (state)
            {
                case VisitState.Visited:
                    break;

                case VisitState.Visiting:
                    stack.Count.VerifyAssert(x => x > 0, "Stack is empty");

                    var list = stack.Select(pair => pair.Key)
                        .TakeWhile(parent => !EqualityComparer<T>.Default.Equals(parent, node))
                        .ToList();

                    list.Add(node);
                    list.Reverse();
                    list.Add(node);
                    cycles.Add(list);
                    break;

                default:
                    visited[node] = VisitState.Visiting;
                    stack.Push(new KeyValuePair<T, IEnumerator<T>>(node, lookup(node).GetEnumerator()));
                    break;

            }
        }

        private static List<List<T>> FindCycles<T>(T root, Func<T, IEnumerable<T>> lookup, Dictionary<T, VisitState> visited)
        {
            var stack = new Stack<KeyValuePair<T, IEnumerator<T>>>();
            var cycles = new List<List<T>>();

            TryPush(root, lookup, stack, visited, cycles);
            while (stack.Count > 0)
            {
                var pair = stack.Peek();

                if (!pair.Value.MoveNext())
                {
                    stack.Pop();
                    visited[pair.Key] = VisitState.Visited;
                    pair.Value.Dispose();
                }
                else
                {
                    TryPush(pair.Value.Current, lookup, stack, visited, cycles);
                }
            }
            return cycles;
        }
    }
}
