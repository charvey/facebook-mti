using MappingTheInternet.Data;
using MappingTheInternet.Graph;
using MappingTheInternet.Models;
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
                return _graph ?? (_graph = GraphBuilder.Build(NodeNameMapper));
            }
        }

        public Predictor()
        {
            var g = Graph;
        }

        public double[][] Predict(int trainStart = 0, int trainEnd = 14, int predictStart = 15, int predictEnd = 19)
        {
            Logger.Log("Predicting future", Logger.TabChange.Increase);

            double[][] predictions = Logger.Batch(InputData.Paths.Length, i => PredictPath(i), "of future predicted").ToArray();

            Logger.Log("Future predicted", Logger.TabChange.Decrease);

            return predictions;
        }

        private double[] PredictPath(int i, int trainStart = 0, int trainEnd = 14, int predictStart = 15, int predictEnd = 19)
        {
            //TODO determine health of path

            var path = ToPath(InputData.Paths[i]);
            double[] pastRecord = Enumerable.Range(trainStart, trainEnd).Select(t => IsOptimumPath(path, t) ? 1.0 : 0.0).ToArray(),
                     result = new double[predictEnd - predictStart + 1];
            double average = pastRecord.Average();

            for (int j = 0; j <= predictEnd - predictStart; j++)
            {
                result[j] = average;
            }

            return result;
        }

        public Node<ASNode, ConnectionSchedule>[] ToPath(string path)
        {
            return path.Split('|').Select(n => n.Trim()).Select(n => NodeNameMapper.Get(n)).ToArray();
        }

        public bool IsOptimumPath(Node<ASNode, ConnectionSchedule>[] path, int t)
        {
            Func<ConnectionSchedule, double> weight = e => e.Schedule[t];
            return Graph.PathLength(path, weight) <= Graph.OptimumLength(path.First(), path.Last(), weight);
        }
    }
}
