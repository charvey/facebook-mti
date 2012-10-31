using System.IO;
using System.Linq;

namespace MappingTheInternet
{
    class Program
    {
        static void Main(string[] args)
        {
            Analyzer.Analyze();

            var predictions = Predictor.Predict();

            OutputResult(predictions);

            Logger.Wait();
        }

        private static void OutputResult(double[][] predictions)
        {
            Logger.Log("Outputting Result");
            File.WriteAllLines("Predictions.txt", new[] { "Predictions" }.Concat(predictions.SelectMany(ps => ps.Select(p => p.ToString()))));
            Logger.Log("Result Outputted");
        }
    }
}
