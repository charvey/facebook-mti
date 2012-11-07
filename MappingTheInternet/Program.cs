using MappingTheInternet.Data;
using System;
using System.IO;
using System.Linq;

namespace MappingTheInternet
{
    class Program
    {
        static void Main(string[] args)
        {
            int input;
            bool exit = false;
            string[] menu = new[] { "Make Prediction", "Run Analysis", "Estimate Score", "Exit" };
            while (!exit)
            {
                Console.Out.WriteLine("Make a choice");

                for (int i = 0; i < menu.Length; i++)
                {
                    Console.Out.WriteLine("{0}. {1}", i + 1, menu[i]);
                }

                if (!int.TryParse(Console.In.ReadLine(), out input))
                {
                    continue;
                }

                switch (input)
                {
                    case 1:
                        MakePrediction();
                        break;
                    case 2:
                        var analyzer = new Analyzer();
                        analyzer.Analyze();
                        break;
                    case 3:
                        EstimateScore();
                        break;
                    case 4:
                        exit = true;
                        break;
                }
            }
        }

        private static void MakePrediction()
        {
            Logger.Log("Making prediction", Logger.TabChange.Increase);

            Logger.Log("Building predictor", Logger.TabChange.Increase);
            var predictor = new Predictor();
            Logger.Log("Predictor built", Logger.TabChange.Decrease);

            Logger.Log("Predicting unknown values", Logger.TabChange.Increase);
            var predictions = predictor.Predict();
            Logger.Log("Unknown values predicted", Logger.TabChange.Decrease);

            Logger.Log("Outputting prediction", Logger.TabChange.Increase);
            File.WriteAllLines("Prediction.csv", new[] { "Prediction" }.Concat(Enumerable.Range(0, 5).SelectMany(t => predictions.Select(p => p[t].ToString()))));
            Logger.Log("Prediction outputted", Logger.TabChange.Decrease);

            Logger.Log("Prediction made", Logger.TabChange.Decrease);
        }

        private static double EstimateScore()
        {
            Logger.Log("Estimating score", Logger.TabChange.Increase);

            Logger.Log("Building predictor", Logger.TabChange.Increase);
            var predictor = new Predictor();
            Logger.Log("Predictor built", Logger.TabChange.Decrease);

            Logger.Log("Loading actual values", Logger.TabChange.Increase);
            var actual = Enumerable.Range(10, 5).SelectMany(t => InputData.Paths.Select(p => predictor.IsOptimumPath(predictor.ToPath(p), t) ? 1.0 : 0.0)).ToList();
            Logger.Log("Actual values loaded", Logger.TabChange.Decrease);

            Logger.Log("Predicting known values", Logger.TabChange.Increase);
            var predictions = predictor.Predict(0, 9, 10, 14).SelectMany(tp => tp).ToList();
            Logger.Log("Known values predicted", Logger.TabChange.Decrease);

            Logger.Log("Calculating score", Logger.TabChange.Increase);
            double score = Kaggle.Auc(actual, predictions);
            Logger.Log("Score calculated", Logger.TabChange.Decrease);

            Logger.Log("Score estimated to be: " + score, Logger.TabChange.Decrease);

            return score;
        }
    }
}
