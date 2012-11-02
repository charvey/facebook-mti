using System;
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

        public static void Wait()
        {
            Console.In.ReadLine();
        }
    }
}
