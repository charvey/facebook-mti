using MappingTheInternet.HashFunctions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MappingTheInternet.ReductionFunctions
{
    public abstract class ReductionFunction : IReductionFunction
    {
        public static readonly ReductionFunction Preferred = new ReductionFunction1();

        public abstract HashSet<string[]> ReduceNames(Dictionary<string, int> nodeNames);
    }

    public class ReductionFunction1 : ReductionFunction
    {
        public override HashSet<string[]> ReduceNames(Dictionary<string, int> nodeNames)
        {
            var groupings = nodeNames.GroupBy(n => HashFunction.Preferred.HashName(n.Key));
            var groups = new HashSet<string[]>(groupings.Select(g => g.Select(n => n.Key).ToArray()));

            return groups;
        }
    }

    public class ReductionFunction2 : ReductionFunction
    {
        public override HashSet<string[]> ReduceNames(Dictionary<string, int> nodeNames)
        {
            var hash = new HashFunction7();

            var groupings = nodeNames.GroupBy(n => hash.HashName(n.Key));
            var groups = groupings.Select(g => g.Select(n => n.Key)).OrderBy(g=>g.Count()).ToArray();
            var wordGroupings = groups.ToDictionary(
                g => hash.HashName(g.First()).Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries),
                g => g.Aggregate("", (c, s) => c + "," + s).Remove(0, 1)).ToArray();

            int limit = 100;
            File.Delete("Reductions.txt");
            for (int i = 0; i < wordGroupings.Length; i++)
            {
                if (wordGroupings[i].Value.All(c => '0' <= c && c <= '9'))
                {
                    continue;
                }
                if (groups[i].Count() > 1)
                {
                    Logger.Log("All 1 instance items reduced");
                    break;
                }
                if (limit == 0)
                {
                    Logger.Log("Limit has been reached");
                    break;
                }

                string[] a = wordGroupings[i].Key;
                for (int j = i + 1; j < groups.Length; j++)
                {
                    string[] b = wordGroupings[j].Key;

                    double score = a.Intersect(b).Count() * 1.0 / ((a.Length + b.Length) / 2.0);

                    if (score > .5)
                    {
                        File.AppendAllText("Reductions.txt", score + " " + wordGroupings[i].Value + " may belong to " + wordGroupings[j].Value + "\n");
                        //Logger.Log(score + " " + wordGroupings[i].Value + " may belong to " + wordGroupings[j].Value);
                    }
                }

                limit--;
            }
            
            return new HashSet<string[]>(groups.Select(g=>g.ToArray()));
        }
    }
}
