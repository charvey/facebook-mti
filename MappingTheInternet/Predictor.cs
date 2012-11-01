using MappingTheInternet.Graph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MappingTheInternet
{
    public class Predictor
    {
        private NodeNameMapper _nodeNameMapper;
        protected NodeNameMapper NodeNameMapper
        {
            get
            {
                return _nodeNameMapper ?? (_nodeNameMapper = new NodeNameMapper());
            }
        }

        private Graph<ASNode, ConnectionSchedule> _graph;
        protected Graph<ASNode, ConnectionSchedule> Graph
        {
            get
            {
                return _graph ?? (_graph = new Graph<ASNode, ConnectionSchedule>());
            }
        }

        public double[][] Predict()
        {
            BuildGraph();

            double[][] predictions = EmptyPredictions();

            foreach (int i in Enumerable.Range(0, 1/*InputData.Paths.Length*/))
            {
                var prediction = PredictPath(i);
                predictions[i] = prediction;
            }

            return predictions;
        }

        private double[] PredictPath(int i)
        {
            var path = ToPath(InputData.Paths[i]);
            double[] pastRecord = Enumerable.Range(0, 15).Select(t => IsOptimumPath(path, t) ? 1.0 : 0.0).ToArray();
            double average = pastRecord.Average();
            return new[] { average, average, average, average, average };
        }

        private Node<ASNode, ConnectionSchedule>[] ToPath(string path)
        {
            return path.Split('|').Select(n => n.Trim()).Select(n => NodeNameMapper.Get(n)).ToArray();
        }

        private double PathLength(Node<ASNode, ConnectionSchedule>[] path,int time)
        {
            if (path.Length == 0)
            {
                return 0;
            }

            double length=0;

            //TODO determine health of path
            return path.Length;
            throw new NotImplementedException();

            return length + PathLength(path.Skip(1).ToArray(), time);
        }

        private class SearchNode
        {
            public Node<ASNode, ConnectionSchedule> Node;
            public double Distance;
        }

        private double OptimumLength(Node<ASNode, ConnectionSchedule> from, Node<ASNode, ConnectionSchedule> to, int time)
        {
            var unvisitedNodes = new LinkedList<SearchNode>();
            var unvisitedNodesMap = new Dictionary<Node<ASNode,ConnectionSchedule>,SearchNode>();
            var visitedNodes= new Dictionary<Node<ASNode,ConnectionSchedule>,double>();

            SearchNode current = new SearchNode{Node=from,Distance=0};

            unvisitedNodes.AddFirst(current);
            unvisitedNodesMap[from]=current;           

            do
            {
                current = unvisitedNodes.First.Value;

                foreach (var e in current.Node.Edges)
                {
                    var dist = e.Value.Value.Schedule[time];

                    if (dist >= 0)
                    {
                        if (visitedNodes.ContainsKey(e.Key))
                        {
                            continue;
                        }
                        if (!unvisitedNodesMap.ContainsKey(e.Key))
                        {
                            var newNode = new SearchNode{Node=e.Key,Distance=double.PositiveInfinity};
                            unvisitedNodes.AddLast(newNode);
                            unvisitedNodesMap[e.Key] = newNode;
                        }

                        if (current.Distance + dist <= unvisitedNodesMap[e.Key].Distance)
                        {
                            unvisitedNodesMap[e.Key].Distance = current.Distance + dist;
                        }
                    }
                }

                visitedNodes.Add(current.Node, current.Distance);
                unvisitedNodes.RemoveFirst();
                unvisitedNodesMap.Remove(current.Node);
            } while (!visitedNodes.ContainsKey(to) && unvisitedNodes.Count > 0);

            return visitedNodes[to];
        }

        private bool IsOptimumPath(Node<ASNode, ConnectionSchedule>[] path, int t)
        {
            return PathLength(path, t) <= OptimumLength(path.First(), path.Last(), t);
        }

        private void BuildGraph()
        {
            for (int i = 0; i < 15; i++)
            {
                foreach (var names in InputData.TrainingSets[i].Select(s => s.Split('|').Select(n => n.Trim()).ToArray()))
                {
                    foreach (var name in names.Take(2))
                    {
                        if (NodeNameMapper.Get(name) == null)
                        {
                            var node = NodeNameMapper.Create(name);
                            Graph.AddNode(node);
                        }
                    }

                    var from = NodeNameMapper.Get(names[0]);
                    var to = NodeNameMapper.Get(names[1]);

                    var edge = from.GetEdge(to);
                    if (edge == null)
                    {
                        edge = new Edge<ConnectionSchedule>(new ConnectionSchedule());
                        from.AddEdge(to, edge);
                    }
                    edge.Value.Schedule[i] = double.Parse(names[2]);
                }
            }
        }

        private double[][] EmptyPredictions()
        {
            return Enumerable.Repeat((Object)null, InputData.Paths.Length).Select((o) => new double[5]).ToArray();
        }
    }
}
