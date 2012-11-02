using MappingTheInternet.Data;
using MappingTheInternet.Graph;
using MappingTheInternet.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                return _graph ?? (_graph = GraphBuilder.Build(NodeNameMapper));
            }
        }

        public double[][] Predict()
        {
            Logger.Log("Predicting future", Logger.TabChange.Increase);

            var x = Graph;

            double[][] predictions = EmptyPredictions();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            foreach (int i in Enumerable.Range(0, InputData.Paths.Length))
            {
                var prediction = PredictPath(i);
                predictions[i] = prediction;

                if (i % 100 == 0)
                {
                    int p = i / 100;
                    var elapsed = sw.Elapsed;
                    var elapsedString = elapsed.ToString(@"hh\:mm\:ss");
                    var remaining = (i>0)?TimeSpan.FromSeconds(elapsed.TotalSeconds * ((100.0 - p) / p)):TimeSpan.MaxValue;
                    var remaingString = remaining == TimeSpan.MaxValue ? "N/A" : remaining.ToString(@"hh\:mm\:ss");
                    Logger.Log(string.Format("{0,2}% of future predicted. Running Time: {1}, Remaining Time: {2}", p, elapsedString, remaingString));
                }
            }
            sw.Stop();

            Logger.Log("Future predicted", Logger.TabChange.Decrease);

            return predictions;
        }

        private double[] PredictPath(int i)
        {
            //TODO determine health of path

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
            if (path.Length == 1)
            {
                return 0;
            }

            double length=0;

            var current = path.First();
            var remaining = path.Skip(1);
            var next = remaining.First();

            if (current.Edges.ContainsKey(next))
            {
                var dist = current.Edges[next].Value.Schedule[time];

                if (dist >= 0)
                {
                    length = dist;
                }
                else
                {
                    

                    length = double.PositiveInfinity;
                }
            }
            else
            {
                if (time == 0)
                {
                    
                }

                length = double.PositiveInfinity;
            }

            return length + PathLength(remaining.ToArray(), time);
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
                            var newNode = new SearchNode { Node = e.Key, Distance = double.PositiveInfinity };
                            unvisitedNodes.AddLast(newNode);
                            unvisitedNodesMap[e.Key] = newNode;
                        }

                        if (current.Distance + dist <= unvisitedNodesMap[e.Key].Distance)
                        {
                            unvisitedNodesMap[e.Key].Distance = current.Distance + dist;
                        }
                    }
                    else
                    {
                    }
                }

                visitedNodes.Add(current.Node, current.Distance);
                unvisitedNodes.RemoveFirst();
                unvisitedNodesMap.Remove(current.Node);
            } while (!visitedNodes.ContainsKey(to) && unvisitedNodes.Count > 0);

            return (visitedNodes.ContainsKey(to)) ? visitedNodes[to] : double.PositiveInfinity;
        }

        private bool IsOptimumPath(Node<ASNode, ConnectionSchedule>[] path, int t)
        {
            return PathLength(path, t) <= OptimumLength(path.First(), path.Last(), t);
        }

        private double[][] EmptyPredictions()
        {
            return Enumerable.Repeat((Object)null, InputData.Paths.Length).Select((o) => new double[5]).ToArray();
        }
    }
}
