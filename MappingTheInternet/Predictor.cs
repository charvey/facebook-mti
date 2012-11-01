using MappingTheInternet.Graph;
using System;
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

            foreach (int i in Enumerable.Range(0, InputData.Paths.Length))
            {
                
            }

            return predictions;
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
                            var node = new Node<ASNode, ConnectionSchedule>(new ASNode(name));
                            NodeNameMapper.Set(name, node);
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
            return Enumerable.Repeat((Object)null, 5).Select((o) => new double[InputData.Paths.Length]).ToArray();
        }
    }
}
