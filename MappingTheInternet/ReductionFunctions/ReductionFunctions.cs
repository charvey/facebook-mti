using MappingTheInternet.HashFunctions;
using System;
using System.Collections.Generic;
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
            var groupings = nodeNames.GroupBy(n => HashFunction.Preferred.HashName(n.Key));
            var groups = groupings.Select(g => g.Select(n => n.Key)).ToArray();
            var wordGroupings = groups.ToDictionary(
                g => HashFunction.Preferred.HashName(g.First()).Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries),
                g => g.Aggregate("", (c, s) => c + "," + s).Remove(0, 1)).ToArray();

            for (int i = 0; i < 1; i++)
            {
                var current = groups.Where(g => g.Count() == 1 && g.Any(s => s.Any(c => c < '0' || '9' < c))).First().Single();

                var words = HashFunction.Preferred.HashName(current).Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < groups.Length; j++)
                {
                    string[] a = wordGroupings[j].Key, b = words;

                    if (a.Count(w => b.Contains(w)) > a.Length / 2 || b.Count(w => a.Contains(w)) > b.Length / 2)
                    {
                        Logger.Log(string.Format("{0}.{1} ", i, j) + current + " may belong to " + wordGroupings[j].Value);
                    }
                }
            }
            
            return new HashSet<string[]>(groups.Select(g=>g.ToArray()));
        }
    }
}
