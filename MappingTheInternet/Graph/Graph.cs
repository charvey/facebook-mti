using System.Collections.Generic;

namespace MappingTheInternet.Graph
{
    public class Graph<TNode,TEdge>
    {
        public HashSet<Node<TNode,TEdge>> Nodes;

        public Graph()
        {
            Nodes = new HashSet<Node<TNode, TEdge>>();
        }

        public void AddNode(Node<TNode,TEdge> node)
        {
            Nodes.Add(node);
        }
    }
}
