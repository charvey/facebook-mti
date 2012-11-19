using System.Collections.Generic;

namespace MappingTheInternet.Graph
{
    public class Node<TNode,TEdge>
    {
        public readonly TNode Value;
        public readonly Dictionary<Node<TNode, TEdge>, Edge<TEdge>> Edges;

        public Node(TNode value)
        {
            Value = value;
            Edges = new Dictionary<Node<TNode, TEdge>, Edge<TEdge>>();
        }

        public Edge<TEdge> GetEdge(Node<TNode,TEdge> node)
        {
            if (Edges.ContainsKey(node))
                return Edges[node];

            return null;
        }

        public void AddEdge(Node<TNode, TEdge> node, Edge<TEdge> edge)
        {
            Edges[node] = edge;
        }
    }
}
