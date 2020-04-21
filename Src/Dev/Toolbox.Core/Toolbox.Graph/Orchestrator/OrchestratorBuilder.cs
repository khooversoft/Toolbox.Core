// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;

namespace KHooversoft.Toolbox.Graph
{
    public class OrchestratorBuilder<TKey, TNode, TEdge>
        where TNode : IGraphNode<TKey>, IJob
        where TEdge : IGraphEdge<TKey>
    {
        public OrchestratorBuilder(GraphMap<TKey, TNode, TEdge> graph)
        {
            graph.VerifyNotNull(nameof(graph));

            Graph = graph;
        }

        public GraphMap<TKey, TNode, TEdge> Graph { get; }

        public OrchestratorHost<TKey, TNode, TEdge> Build(IJobHost? jobHost = null, int? maxJobParallel = null)
        {
            jobHost ??= new JobHost();
            return new OrchestratorHost<TKey, TNode, TEdge>(Graph, jobHost, maxJobParallel);
        }
    }
}
