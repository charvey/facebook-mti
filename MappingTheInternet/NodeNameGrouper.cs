using MappingTheInternet.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MappingTheInternet
{
    public static class NodeNameGrouper
    {
        public static HashSet<string[]> Build()
        {
            var nodeNames = NodeNames();

            return ReduceNames(nodeNames);
        }

        public static Dictionary<string,int> NodeNames()
        {
            Logger.Log("Loading names");

            var nodeNames = new Dictionary<string, int>();
            string[] lines;
            Func<string, int> add = (n) => nodeNames[n] = (nodeNames.ContainsKey(n) ? nodeNames[n] : 0) + 1;

            for (int i = 0; i < 15; i++)
            {
                lines = InputData.Trains(i);

                foreach (var line in lines.Select(l => l.Split('|')))
                {
                    add(line[0].Trim());
                    add(line[1].Trim());
                }
            }

            lines = InputData.Paths;

            foreach (var line in lines.Select(l => l.Split('|')))
            {
                foreach (var name in line)
                {
                    add(name.Trim());
                }
            }

            Logger.Log(nodeNames.Count + " names loaded");

            return nodeNames;
        }

        private static HashSet<string[]> ReduceNames(Dictionary<string, int> nodeNames)
        {
            return new HashSet<string[]>(nodeNames.Select(n => new []{n.Key}));
        }

        public static string HashName(string name)
        {
            var specialSymbols = name.ToLower().Where(c => c < 'a' || 'z' < c).Distinct().ToArray();

            var words = name.Split(specialSymbols, StringSplitOptions.RemoveEmptyEntries);

            var sortedDistinctWords = words.Select(w => new string(w.OrderBy(c => c).ToArray())).Distinct().OrderBy(s => s);

            var hashName = sortedDistinctWords.Aggregate("", (s, c) => s + "|" + c);

            return hashName;
        }
    }
}
