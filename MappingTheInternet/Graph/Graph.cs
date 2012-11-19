using System;
using System.Collections.Generic;
using System.Linq;

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

        private class SearchNode
        {
            public Node<TNode, TEdge> Node;
            public double Distance;
        }

        public double PathLength(Node<TNode, TEdge>[] path, Func<TEdge, double> getWeight)
        {
            if (path.Length == 1)
            {
                return 0;
            }

            double length = 0;

            var current = path.First();
            var remaining = path.Skip(1).ToArray();
            var next = remaining[0];

            length = current.Edges.ContainsKey(next)
                         ? getWeight(current.Edges[next].Value)
                         : double.PositiveInfinity;

            return length + PathLength(remaining, getWeight);
        }

        public double OptimumLength(Node<TNode, TEdge> from, Node<TNode, TEdge> to, Func<TEdge, double> getWeight)
        {
            var unvisitedNodes = new LinkedList<SearchNode>();
            var unvisitedNodesMap = new Dictionary<Node<TNode, TEdge>, SearchNode>();
            var visitedNodes = new Dictionary<Node<TNode, TEdge>, double>();

            var current = new SearchNode { Node = from, Distance = 0 };

            unvisitedNodes.AddFirst(current);
            unvisitedNodesMap[from] = current;

            do
            {
                current = unvisitedNodes.First.Value;

                foreach (var e in current.Node.Edges)
                {
                    if (visitedNodes.ContainsKey(e.Key))
                    {
                        continue;
                    }

                    if (!unvisitedNodesMap.ContainsKey(e.Key))
                    {
                        var newNode = new SearchNode { Node = e.Key, Distance = double.PositiveInfinity };
                        unvisitedNodes.AddLast(newNode);
                        unvisitedNodesMap[e.Key] = newNode;
                    }

                    double dist = getWeight(e.Value.Value);

                    if (current.Distance + dist <= unvisitedNodesMap[e.Key].Distance)
                    {
                        unvisitedNodesMap[e.Key].Distance = current.Distance + dist;
                    }
                }

                visitedNodes.Add(current.Node, current.Distance);
                unvisitedNodes.RemoveFirst();
                unvisitedNodesMap.Remove(current.Node);
            } while (!visitedNodes.ContainsKey(to) && unvisitedNodes.Count > 0);

            return (visitedNodes.ContainsKey(to)) ? visitedNodes[to] : double.PositiveInfinity;
        }
    }
}
