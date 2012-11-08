using MappingTheInternet.HashFunctions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MappingTheInternet.ReductionFunctions
{
    public abstract class ReductionFunction : IReductionFunction
    {
        public static readonly ReductionFunction Preferred = new ReductionFunction3();

        public abstract HashSet<string[]> ReduceNames(Dictionary<string, int> nodeNames);
    }

    public class ReductionFunction3 : ReductionFunction
    {
        private class Group
        {
            public string hash;
            public List<KeyValuePair<string, int>> instances;
            public string[] words;
        }

        private readonly string Filename = "Reductions_3.txt";

        public override HashSet<string[]> ReduceNames(Dictionary<string, int> nodeNames)
        {
            var hash = new HashFunction8();
            var groups = nodeNames.GroupBy(n => hash.HashName(n.Key)).Select(g => new Group
            {
                hash = g.Key,
                instances = g.ToList(),
                words = hash.HashName(g.First().Key).Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray()
            }).OrderBy(g => g.instances.Count()).ToArray();

            var numbers = groups.Where(g => g.hash.All(c => '0' <= c && c <= '9')).Select(g => Convert.ToInt32(g.hash));
            groups = groups.Where(g => !g.hash.All(c => '0' <= c && c <= '9')).ToArray();

#if DEBUG
            lock (Filename)
            {
                File.Delete(Filename);
            }
#endif

            int pass = 0;
            bool reduced;
            do
            {
                reduced = false;
                pass++;

                double beforeAverage = groups.Average(g => g.instances.Count());
                int beforeCount = groups.Length;

                Logger.Log(string.Format("The {0} groups have an average size {1} before pass {2}", beforeCount, beforeAverage, pass), Logger.TabChange.Increase);

                var reductions = Logger.Batch(groups.Length, (i) => ReduceName(groups, i), "reduced this pass")
                    .Where(p => p != null).OrderBy(p => p.Item1).ToList();

                reduced = reductions.Count > 0;

                foreach (var reduction in reductions)
                {
                    groups[reduction.Item2].instances.AddRange(groups[reduction.Item1].instances);
                    groups[reduction.Item1] = null;
                }

                groups = groups.Where(g => g != null).OrderBy(g => g.instances.Count()).ToArray();

                double afterAverage = groups.Average(g => g.instances.Count());
                int afterCount = groups.Length;

                Logger.Log(string.Format("The {0} groups have an average size {1} after pass {2}", afterCount, afterAverage, pass), Logger.TabChange.Decrease);
            } while (reduced);

            var numberNameGroups = numbers.OrderBy(n => n).Select(n => new string[] { n.ToString() });
            var textNameGroups = groups.Select(g => g.instances.Select(inst => inst.Key).Distinct().ToArray());

            return new HashSet<string[]>(numberNameGroups.Concat(textNameGroups));
        }

        private Tuple<int,int> ReduceName(Group[] groups, int i)
        {
            string[] a = groups[i].words;
            double maxScore = 0;
            int maxScoreIndex = i;
            for (int j = i + 1; j < groups.Length; j++)
            {
                if (groups[j] == null)
                    continue;

                string[] b = groups[j].words;

                double score = 0;

                for (int ia = 0; ia < a.Length; ia++)
                {
                    int bs = 0;
                    for (int ib = bs; ib < b.Length; ib++)
                    {
                        if (a[ia] == b[ib])
                        {
                            score++;
                            bs = ib + 1;
                            break;
                        }
                    }
                }

                score /= (a.Length + b.Length) / 2.0;

                if (score > .5)
                {
#if DEBUG
                    lock (Filename)
                    {
                        File.AppendAllText(Filename, score + " " + groups[i].instances.Aggregate("", (c, s) => c + "," + s).Remove(0, 1) +
                            " may belong to " + groups[j].instances.Aggregate("", (c, s) => c + "," + s).Remove(0, 1) + "\n");
                    }
#endif
                }

                if (score > maxScore)
                {
                    maxScore = score;
                    maxScoreIndex = j;
                }
            }
            
            if (maxScore > .5)
            {
#if DEBUG
                lock (Filename)
                {
                    File.AppendAllText(Filename, maxScore + " " + groups[i].instances.Aggregate("", (c, s) => c + "," + s).Remove(0, 1) +
                            " most likely belongs to " + groups[maxScoreIndex].instances.Aggregate("", (c, s) => c + "," + s).Remove(0, 1) + "\n");
                }
#endif

                return new Tuple<int, int>(i, maxScoreIndex);
            }
            else
            {
#if DEBUG
                lock (Filename)
                {
                    File.AppendAllText(Filename, maxScore + " " + groups[i].instances.Aggregate("", (c, s) => c + "," + s).Remove(0, 1) +
                            " does not belong to any groups\n");
                }
#endif
                return null;
            }
        }
    }
}
