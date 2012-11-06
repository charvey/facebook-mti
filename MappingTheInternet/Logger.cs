using System;
using System.Diagnostics;
using System.Linq;

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

        #region Progress

        private static int _total;
        private static double _progress;
        private static Stopwatch _stopwatch;

        public static void StartProgress(int total)
        {
            _total = total;
            _progress = 0;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public static void Progress(string text, int index)
        {
            if (100.0 * index / _total >= _progress)
            {
                int p = (int)(100.0 * index / _total);
                var elapsed = _stopwatch.Elapsed;
                var elapsedString = elapsed.ToString(@"hh\:mm\:ss");
                var remaining = (p > 0) ? TimeSpan.FromSeconds(elapsed.TotalSeconds * ((100.0 - p) / p)) : TimeSpan.MaxValue;
                var remainingString = remaining == TimeSpan.MaxValue ? "N/A" : remaining.ToString(@"hh\:mm\:ss");
                Logger.Log(string.Format(text+" Running Time: {1}, Remaining Time: {2}", p.ToString("#0"), elapsedString, remainingString));

                if (p == 100)
                {
                    _stopwatch.Stop();
                }
                else
                {
                    _progress = p + 1;
                }
            }
        }

        #endregion

        public static void Wait()
        {
            Console.In.ReadLine();
        }
    }
}
