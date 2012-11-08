using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace MappingTheInternet
{
    public static class Logger
    {
        private static int TabSize = 0;
        private const string Tab = "  ";

        public enum TabChange
        {
            None,Increase,Decrease
        }

        public static void Log(string text, TabChange tabChange = TabChange.None)
        {
            if (tabChange == TabChange.Decrease)
            {
                TabSize--;
            }

            //File.AppendAllText("log", text + "\n");
            var fullTab = Enumerable.Repeat(Tab, TabSize).Aggregate("", (c, s) => c + s);
            var width = Console.WindowWidth - fullTab.Length - 1;
            for (int i = 0; i < text.Length; i += width)
            {
                Console.Out.WriteLine(fullTab + text.Substring(i, Math.Min((text.Length - i), width)));
            }

            if (tabChange == TabChange.Increase)
            {
                TabSize++;
            }
        }

        public static List<T> Batch<T>(int total, Func<int,T> operation, string text)
        {
            Stopwatch sw = new Stopwatch();
            Timer timer = new Timer(17);
            int maxi = int.MinValue;
            timer.Elapsed += new ElapsedEventHandler((o, e) =>
            {
                double p = (100.0 * maxi) / total;
                var elapsed = sw.Elapsed;
                var elapsedString = elapsed.ToString(@"hh\:mm\:ss");
                var remaining = (maxi > 0) ? TimeSpan.FromSeconds(elapsed.TotalSeconds * ((100.0 - p) / p)) : TimeSpan.MaxValue;
                var remaingString = remaining == TimeSpan.MaxValue ? "N/A" : remaining.ToString(@"hh\:mm\:ss");

                Console.Title = string.Format("{0}% "+text+". Running Time: {1}, Remaining Time: {2}", p.ToString("00.0"), elapsedString, remaingString);
            });

            sw.Start();
            timer.Start();
            var partitioner = Partitioner.Create(Enumerable.Range(0, total));
            var results = partitioner.AsParallel().Select(i =>
            {
                var result = operation(i);

                if (i > maxi) maxi = i;

                return new Tuple<int, T>(i, result);
            }).OrderBy(t => t.Item1).Select(t => t.Item2).ToList();
            timer.Stop();
            sw.Stop();

            return results;
        }

        public static void Wait()
        {
            Console.In.ReadLine();
        }
    }
}
