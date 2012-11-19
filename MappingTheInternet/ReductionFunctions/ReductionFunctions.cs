using MappingTheInternet.HashFunctions;
using System;
using System.Collections.Generic;
using System.IO;
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
            public string Hash;
            public List<KeyValuePair<string, int>> Instances;
            public string[] Words;
        }

        private Dictionary<string,double> WordScore;

        protected string Filename = "Reductions_3.txt";

        public override HashSet<string[]> ReduceNames(Dictionary<string, int> nodeNames)
        {
            var hash = new HashFunction8();
            var groups = nodeNames.GroupBy(n => hash.HashName(n.Key)).Select(g => new Group
            {
                Hash = g.Key,
                Instances = g.ToList(),
                Words = hash.HashName(g.First().Key).Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray()
            }).OrderBy(g => g.Instances.Count).ToArray();

            var numbers = groups.Where(g => g.Hash.All(c => '0' <= c && c <= '9')).Select(g => Convert.ToInt32(g.Hash));
            groups = groups.Where(g => !g.Hash.All(c => '0' <= c && c <= '9')).ToArray();

            {
                var wordFrequency = new Dictionary<string, int>();
                var wordCount = 0;
                foreach (var group in groups)
                {
                    foreach (var word in group.Words)
                    {
                        if (!wordFrequency.ContainsKey(word))
                        {
                            wordFrequency[word] = 0;
                        }
                        wordFrequency[word] += group.Instances.Count;
                        wordCount += group.Instances.Count;
                    }
                }
                WordScore = wordFrequency.ToDictionary(k => k.Key, k => wordCount/(1.0*k.Value));
            }

            Logger.Log(string.Format("The {0} groups have an average size {1}",
                groups.Length, groups.Average(g => g.Instances.Count)));
            bool reduced;
            int pass = 0;
            do
            {
#if DEBUG
                Filename = "Reductions_3_" + pass + ".txt";
                lock (Filename)
                {
                    File.Delete(Filename);
                }
#endif

                var reductions = Logger.Batch(groups.Length - 1, i => ReduceName(groups, i, 25), "reduced this pass")
                    .Where(p => p != null).OrderBy(p => p.Item1).ToList();

                reduced = reductions.Count > 0;

                foreach (var reduction in reductions)
                {
                    groups[reduction.Item2].Instances.AddRange(groups[reduction.Item1].Instances);
                    groups[reduction.Item1] = null;
                }

                groups = groups.Where(g => g != null).OrderBy(g => g.Instances.Count()).ToArray();

#if DEBUG
                lock (Filename)
                {
                    File.AppendAllText(Filename, ("~~~ " + reductions.Count + " ~~~ reductions were made in this pass" + "\n"));
                }
#endif

                Logger.Log(reductions.Count + " reductions were made");
                Logger.Log(string.Format("The {0} groups have an average size {1}",
                    groups.Length, groups.Average(g => g.Instances.Count())));
                pass++;
            } while (reduced);

            var numberNameGroups = numbers.OrderBy(n => n).Select(n => new[] {n.ToString()});
            var textNameGroups = groups.Select(g => g.Instances.Select(inst => inst.Key).Distinct().ToArray());

            return new HashSet<string[]>(numberNameGroups.Concat(textNameGroups));
        }

        private Tuple<int, int> ReduceName(Group[] groups, int i, double threshold)
        {
            string[] a = groups[i].Words;
            int maxScoreIndex = i + 1;
            double[] scores = new double[groups.Length - (i + 1)];
            for (int j = i + 1; j < groups.Length; j++)
            {
                if (groups[j] == null)
                    continue;

                string[] b = groups[j].Words;

                scores[j - (i + 1)] = 0;

                int bs = 0;
                for (int ia = 0; ia < a.Length; ia++)
                {
                    for (int ib = bs; ib < b.Length; ib++)
                    {
                        if (a[ia] == b[ib])
                        {
                            //scores[j - (i + 1)]++;
                            scores[j - (i + 1)] += WordScore[a[ia]]*1000;

                            bs = ib + 1;
                            break;
                        }
                    }
                }

                scores[j - (i + 1)] /= (a.Length + b.Length)/2.0;

                /*
                if (scores[j - (i + 1)] > .5)
                {
#if DEBUG
                    lock (Filename)
                    {
                        File.AppendAllText(Filename, scores[j - (i + 1)] + " " + groups[i].instances.Aggregate("", (c, s) => c + "," + s).Remove(0, 1) +
                            " may belong to " + groups[j].instances.Aggregate("", (c, s) => c + "," + s).Remove(0, 1) + "\n");
                    }
#endif
                }
                */
                if (scores[j - (i + 1)] > scores[maxScoreIndex - (i + 1)])
                {
                    maxScoreIndex = j;
                }
            }

            var stdDev = scores.StdDev();
            var score = scores[maxScoreIndex - (i + 1)];
            var zScore = score / stdDev;
            string message = "does not belong to";
            Tuple<int, int> result = null;

            if (zScore > threshold)
            {
                message = "most likely belongs to";
                result = new Tuple<int, int>(i, maxScoreIndex);
            }
#if DEBUG
            var from = groups[i].Instances.Aggregate("", (c, s) => c + "," + s).Remove(0, 1);
            var to = groups[maxScoreIndex].Instances.Aggregate("", (c, s) => c + "," + s).Remove(0, 1);
            lock (Filename)
            {
                File.AppendAllText(Filename, string.Format("{0}:{1} {2} {3} {4}\n", score, zScore, from, message, to));
            }
#endif
            return result;
        }
    }
}
