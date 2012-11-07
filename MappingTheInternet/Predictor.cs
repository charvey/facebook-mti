using MappingTheInternet.Data;
using MappingTheInternet.Graph;
using MappingTheInternet.Models;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

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

            double[][] predictions = new double[InputData.Paths.Length][];

            Stopwatch sw = new Stopwatch();
            Timer timer = new Timer(17);
            int maxi = int.MinValue;
            timer.Elapsed += new ElapsedEventHandler((o, e) =>
            {
                double p = (100.0 * maxi) / InputData.Paths.Length;
                var elapsed = sw.Elapsed;
                var elapsedString = elapsed.ToString(@"hh\:mm\:ss");
                var remaining = (maxi > 0) ? TimeSpan.FromSeconds(elapsed.TotalSeconds * ((100.0 - p) / p)) : TimeSpan.MaxValue;
                var remaingString = remaining == TimeSpan.MaxValue ? "N/A" : remaining.ToString(@"hh\:mm\:ss");

                Console.Title = string.Format("{0}% of future predicted. Running Time: {1}, Remaining Time: {2}", p.ToString("00.0"), elapsedString, remaingString);

            });

            sw.Start();
            timer.Start();
            var partitioner = Partitioner.Create(Enumerable.Range(0, InputData.Paths.Length));
            predictions = partitioner.AsParallel().Select(i =>
            {
                var prediction = PredictPath(i);

                if (i > maxi) maxi = i;                

                return new Tuple<int, double[]>(i, prediction);
            }).OrderBy(p => p.Item1).Select(p => p.Item2).ToArray();
            timer.Stop();
            sw.Stop();

            Logger.Log("Future predicted", Logger.TabChange.Decrease);

            return predictions;
        }

        private double[] PredictPath(int i, int trainStart = 0, int trainEnd = 14, int predictStart = 15, int predictEnd = 19)
        {
            //TODO determine health of path

            var path = ToPath(InputData.Paths[i]);
            double[] pastRecord = Enumerable.Range(trainStart, trainEnd).Select(t => IsOptimumPath(path, t) ? 1.0 : 0.0).ToArray();
            double average = pastRecord.Average();
            double[] result = new double[predictEnd - predictStart + 1];

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
            Func<ConnectionSchedule, double> weight = (e) => e.Schedule[t];
            return Graph.PathLength(path, weight) <= Graph.OptimumLength(path.First(), path.Last(), weight);
        }

        private double[][] EmptyPredictions(int predictStart = 15, int predictEnd = 19)
        {
            return Enumerable.Repeat((Object)null, InputData.Paths.Length).Select((o) => new double[predictEnd - predictStart + 1]).ToArray();
        }
    }
}
