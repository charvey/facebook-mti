using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MappingTheInternet
{
    public static class Analyzer
    {
        public static void Analyze()
        {
            HashNumberNames();
            NameAnalysis1();
            NameAnalysis2();
        }

        public static void HashNumberNames()
        {
            var numberNames = new HashSet<string>();
            var hashNumberNames = new HashSet<string>();
            int val;

            var everyName = Enumerable.Range(0, 15).SelectMany(i => InputData.Trains(i)).SelectMany(l => l.Split('|').Take(2)).Concat(InputData.Paths.SelectMany(l=>l.Split('|'))).Select(s=>s.Trim());

            foreach (var name in everyName)
            {
                if (int.TryParse(name, out val))
                {
                    numberNames.Add(name);
                    hashNumberNames.Add(NodeNameGrouper.HashName(name));
                }
            }

            Logger.Log(numberNames.Count + " names are numbers");
            Logger.Log(hashNumberNames.Count + " unique number name hashes");
        }

        public static void NameAnalysis1()
        {
            var nodeNames = NodeNameGrouper.NodeNames();

            Logger.Log("Longest name is " + nodeNames.Max(n => n.Key.Length) + " characters long");

            var alphabet = nodeNames.Keys.SelectMany(s => s.AsEnumerable()).Distinct();

            Logger.Log(alphabet.Count() + " characters in alphabet \"" + alphabet.Aggregate(string.Empty, (s, c) => s + c) + "\"");

            var reducedAlphabet = nodeNames.Keys.SelectMany(s => s.ToLower().Where(c => 'a' <= c && c <= 'z')).Distinct().OrderBy(c => c);

            Logger.Log(reducedAlphabet.Count() + " characters in alphabet \"" + reducedAlphabet.Aggregate(string.Empty, (s, c) => s + c) + "\"");

            
        }

        public static void NameAnalysis2()
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
                names[i + 1] = new SortedSet<string>();
                lines = InputData.Trains(i);

                foreach (var line in lines.Select(l => l.Split('|')))
                {
                    names[i + 1].Add(line[0].Trim());
                    names[i + 1].Add(line[1].Trim());
                }
            }

            Logger.Log("The unique names in each set are: " + names.Skip(1).Aggregate("paths " + names[0].Count, (c, s) => c + ", " + s.Count));

            Logger.Log("Train 1 and 2 have " + names[1].Intersect(names[2]).Count() + " names in common");

            var trainingCommonNames = names.Skip(2).Aggregate(new SortedSet<string>(names[1]), (s, c) => { s.IntersectWith(c); return s; });

            Logger.Log("The training sets have " + trainingCommonNames.Count + " names in common");

            Logger.Log("The training and path sets have " + trainingCommonNames.Intersect(names[0]).Count() + " names in common");
        }
    }
}
