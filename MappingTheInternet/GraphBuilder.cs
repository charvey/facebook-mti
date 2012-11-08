using MappingTheInternet.Data;
using MappingTheInternet.Graph;
using MappingTheInternet.Models;
using System.Linq;

namespace MappingTheInternet
{
    public static class GraphBuilder
    {
        public static Graph<ASNode, ConnectionSchedule> Build(NodeNameMapper nodeNameMapper)
        {
            Logger.Log("Building graph", Logger.TabChange.Increase);

            var graph = new Graph<ASNode, ConnectionSchedule>();

            for (int i = 0; i < InputData.TrainingSets.Length; i++)
            {
                foreach (var names in InputData.TrainingSets[i].Select(s => s.Split('|').Select(n => n.Trim())))
                {
                    foreach (var name in names.Take(2))
                    {
                        if (nodeNameMapper.Get(name) == null)
                        {
                            var node = nodeNameMapper.Create(name);
                            graph.AddNode(node);
                        }
                    }

                    var from = nodeNameMapper.Get(names.ElementAt(0));
                    var to = nodeNameMapper.Get(names.ElementAt(1));

                    var edge = from.GetEdge(to);
                    if (edge == null)
                    {
                        edge = new Edge<ConnectionSchedule>(new ConnectionSchedule());
                        from.AddEdge(to, edge);
                    }
                    edge.Value.Schedule[i] = double.Parse(names.ElementAt(2));
                }
            }

            Logger.Log(graph.Nodes.Count + " nodes and " + graph.Nodes.Sum(n => n.Edges.Count) + " edges added from training sets");

            foreach (var name in InputData.Paths.SelectMany(p => p.Split('|').Select(n => n.Trim())))
            {
                if (nodeNameMapper.Get(name) == null)
                {
                    var node = nodeNameMapper.Create(name);
                    graph.AddNode(node);
                }
            }

            Logger.Log(graph.Nodes.Count + " nodes added from paths");

            Logger.Log("Graph built", Logger.TabChange.Decrease);

            return graph;
        }
    }
}
