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

            double[][] predictions = Enumerable.Repeat((Object)null, 5).Select((o) => new double[10000]).ToArray();

            return predictions;
        }
    }
}
