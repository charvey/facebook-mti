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

            var messyNames = MessyNames();

            return ReduceNames(nodeNames);
        }

        private static Dictionary<string,int> NodeNames()
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

        private static List<string> MessyNames()
        {            
            string[] lines;
            var names = new SortedSet<string>[16];

            lines = InputData.Paths;
            names[0] = new SortedSet<string>();
            foreach (var line in lines.Select(l => l.Split('|')))
            {
                foreach (var name in line)
                {
                    names[0].Add(name.Trim());
                }
            }

            for (int i = 0; i < 15; i++)
            {
                names[i+1] = new SortedSet<string>();
                lines = InputData.Trains(i);

                foreach (var line in lines.Select(l => l.Split('|')))
                {
                    names[i+1].Add(line[0].Trim());
                    names[i+1].Add(line[1].Trim());
                }
            }

            Logger.Log("The unique names in each set are: " + names.Skip(1).Aggregate("paths " + names[0].Count, (c, s) => c + ", " + s.Count));

            Logger.Log("Train 1 and 2 have " + names[1].Intersect(names[2]).Count() + " names in common");

            var trainingCommonNames = names.Skip(2).Aggregate(new SortedSet<string>(names[1]), (s, c) => { s.IntersectWith(c); return s; });

            Logger.Log("The training sets have " + trainingCommonNames.Count + " names in common");

            Logger.Log("The training and path sets have " + trainingCommonNames.Intersect(names[0]).Count() + " names in common");

            var messyNames = new List<string>();

            return messyNames;
        }

        private static HashSet<string[]> ReduceNames(Dictionary<string, int> nodeNames)
        {
            Logger.Log("Longest name is " + nodeNames.Max(n => n.Key.Length) + " characters long");

            var alphabet = nodeNames.Keys.SelectMany(s => s.AsEnumerable()).Distinct();

            Logger.Log(alphabet.Count() + " characters in alphabet \"" + alphabet.Aggregate(string.Empty, (s, c) => s + c) + "\"");

            var reducedAlphabet = nodeNames.Keys.SelectMany(s => s.ToLower().Where(c => 'a' <= c && c <= 'z')).Distinct().OrderBy(c => c);

            Logger.Log(reducedAlphabet.Count() + " characters in alphabet \"" + reducedAlphabet.Aggregate(string.Empty, (s, c) => s + c) + "\"");
            
            return new HashSet<string[]>(nodeNames.Select(n => new []{n.Key}));
        }

        private static string HashName(string name)
        {
            var specialSymbols = name.ToLower().Where(c => c < 'a' || 'z' < c).Distinct().ToArray();

            var words = name.Split(specialSymbols, StringSplitOptions.RemoveEmptyEntries);

            var sortedDistinctWords = words.Select(w => new string(w.OrderBy(c => c).ToArray())).Distinct().OrderBy(s => s);

            var hashName = sortedDistinctWords.Aggregate("", (s, c) => s + "|" + c);

            return hashName;
        }
    }
}
