using MappingTheInternet.Data;
using System;
using System.Collections.Generic;
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
                        {
                            var predictor = new Predictor();
                            var predictions = predictor.Predict();

                            OutputResult(predictions);
                        }
                        break;
                    case 2:
                        {
                            var analyzer = new Analyzer();
                            analyzer.Analyze();
                        }
                        break;
                    case 3:
                        {
                            InputData.FullData = false;
                            var predictor = new Predictor();
                            var predictions = predictor.Predict();
                            InputData.FullData = true;

                            Console.Out.WriteLine(EstimateScore(predictions));                           
                        }
                        break;

                    case 4:
                        exit = true;
                        break;

                }
            }
        }

        private static void OutputResult(double[][] predictions)
        {
            Logger.Log("Outputting Result", Logger.TabChange.Increase);
            File.WriteAllLines("Predictions.txt", new[] { "Predictions" }.Concat(Enumerable.Range(0, 5).SelectMany(t => predictions.Select(p => p[t].ToString()))));
            Logger.Log("Result Outputted", Logger.TabChange.Decrease);
        }

        private static double EstimateScore(double[][] predictions)
        {
            Logger.Log("Estimating Score", Logger.TabChange.Increase);

            var actual = Enumerable.Range(0, 10000).Select(i => (double)i).ToList();
            var predicted = predictions.SelectMany(tp => tp).ToList();
            double score = Kaggle.Auc(actual, predicted);

            Logger.Log("Score Estimated", Logger.TabChange.Decrease);

            return score;
        }
    }
}
