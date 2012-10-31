using MappingTheInternet.Graph;
using System;
using System.Linq;

namespace MappingTheInternet
{
    public static class Predictor
    {
        public static double[][] Predict()
        {
            var nodeNameMapper = new NodeNameMapper();

            var graph = new Graph<ASNode, ConnectionSchedule>();

            for (int i = 0; i < 15; i++)
            {
                foreach (var names in InputData.Trains(i).Select(s=>s.Split('|').Select(n=>n.Trim()).ToArray()))
                {
                    foreach (var name in names.Take(2))
                    {
                        if (nodeNameMapper.Get(name) == null)
                        {
                            var node = new Node<ASNode, ConnectionSchedule>(new ASNode(name));
                            nodeNameMapper.Set(name, node);
                            graph.AddNode(node);
                        }
                    }

                    var from = nodeNameMapper.Get(names[0]);
                    var to = nodeNameMapper.Get(names[1]);

                    var edge = from.GetEdge(to);
                    if (edge == null)
                    {
                        edge = new Edge<ConnectionSchedule>(new ConnectionSchedule());
                        from.AddEdge(to, edge);
                    }
                    edge.Value.Schedule[i] = double.Parse(names[2]);
                }
            }

            var changers = graph.Nodes.Count(n => n.Edges.Any(e => e.Value.Value.Schedule.Any(s => s == 0) && e.Value.Value.Schedule.Any(s => s == 1)));

            Logger.Log(changers + " nodes have edges which change weights");

            var stablePeers = graph.Nodes.Count(n => n.Edges.Any(e => e.Value.Value.Schedule.Take(15).All(d => d == 0)));

            Logger.Log(stablePeers + " nodes point to a stable peer");

            var stableCosts = graph.Nodes.Count(n => n.Edges.Any(e => e.Value.Value.Schedule.Take(15).All(d => d == 1)));

            Logger.Log(stableCosts + " nodes point to a stable costly connection");

            var stableConnection = graph.Nodes.Count(n => n.Edges.Any(e => e.Value.Value.Schedule.Take(15).All(d => d==0||d == 1)));

            Logger.Log(stableConnection + " nodes point to a stable connection");

            double[][] predictions = Enumerable.Repeat((Object)null, 5).Select((o) => new double[10000]).ToArray();

            return predictions;
        }
    }
}
