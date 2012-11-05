using MappingTheInternet.Data;
using MappingTheInternet.Graph;
using MappingTheInternet.HashFunctions;
using MappingTheInternet.Models;
using MappingTheInternet.ReductionFunctions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MappingTheInternet
{
    public class Analyzer
    {
        #region Data

        private IEnumerable<string> _everyName;
        protected IEnumerable<string> EveryName
        {
            get
            {
                if (_everyName == null)
                {
                    _everyName = InputData.TrainingSets.SelectMany(s => s).SelectMany(l => l.Split('|').Take(2)).Concat(InputData.Paths.SelectMany(l => l.Split('|'))).Select(s => s.Trim()).ToList();
                }

                return _everyName;
            }
        }

        private IEnumerable<string> _everyDistinctName;
        protected IEnumerable<string> EveryDistinctName
        {
            get
            {
                if (_everyDistinctName == null)
                {
                    _everyDistinctName = EveryName.Distinct().ToList();
                }

                return _everyDistinctName;
            }
        }

        private IEnumerable<Node<ASNode, ConnectionSchedule>[]> _everyPath;
        protected IEnumerable<Node<ASNode, ConnectionSchedule>[]> EveryPath
        {
            get
            {
                if (_everyPath == null)
                {
                    _everyPath = InputData.Paths.Select(p=>p.Split(new []{'|'},StringSplitOptions.RemoveEmptyEntries).Select(n=>NodeNameMapper.Get(n.Trim())).ToArray()).ToList();
                }

                return _everyPath;
            }
        }

        #endregion

        #region Objects

        private NodeNameMapper _nodeNameMapper;
        protected NodeNameMapper NodeNameMapper
        {
            get
            {
                if (_nodeNameMapper == null)
                {
                    _nodeNameMapper = new NodeNameMapper();
                }

                return _nodeNameMapper;
            }
        }

        private Graph<ASNode, ConnectionSchedule> _graph;
        protected Graph<ASNode, ConnectionSchedule> Graph
        {
            get
            {
                if (_graph == null)
                {
                    _graph = GraphBuilder.Build(NodeNameMapper);

                }

                return _graph;
            }
        }

        #endregion

        public void Analyze()
        {
            Logger.Log("Analyzing data", Logger.TabChange.Increase);

            HashFunctions();
            HashNumberNames();

            ReductionFunctions();

            NameAnalysis1();
            NameAnalysis2();

            Edges();

            BrokenPaths();
            PathLengths();
            SinglePaths();

            Logger.Log("Data analyzed", Logger.TabChange.Decrease);
        }

        #region Hashes

        public void HashFunctions()
        {
            Logger.Log("Analyzing hash functions", Logger.TabChange.Increase);

            var hashes = new IHashFunction[] { new HashFunction1(), new HashFunction2(), new HashFunction3(), new HashFunction4(), new HashFunction5(), new HashFunction6(), new HashFunction7() };

            for (int i = 1; i <= hashes.Length; i++)
            {
                Logger.Log("Analyzing hash function #" + i, Logger.TabChange.Increase);

                var results = EveryDistinctName.GroupBy(n => hashes[i-1].HashName(n)).Select(g => g.ToArray()).OrderBy(s => s.Length).ToArray();

                Logger.Log("Function #" + i + " produces " + results.Length + " unique names");

                File.WriteAllLines("Hash_Function_" + i + ".txt", results.Select(group => group.Aggregate(hashes[i - 1].HashName(group.First()) + " (" + group.Length + "): ", (ag, c) => ag + ',' + "\"" + c + "\"")));

                Logger.Log("Hash function #" + i + " analyzed", Logger.TabChange.Decrease);
            }

            Logger.Log("Hash functions analyzed", Logger.TabChange.Decrease);
        }
        
        public void HashNumberNames()
        {
            var numberNames = new HashSet<string>();
            var hashNumberNames = new HashSet<string>();
            int val;

            var hash23689 = "";
            foreach (var name in EveryName.Distinct())
            {
                if (int.TryParse(name, out val))
                {
                    numberNames.Add(name);
                    var hash = HashFunction.Preferred.HashName(name);
                    hashNumberNames.Add(hash);

                    if (hash == "|23689")
                    {
                        hash23689 += (hash23689 == "" ? "" : ",") + "\"" + name + "\"";
                    }
                }
            }
            Logger.Log("The following have hash \"|23689\": " + hash23689);

            Logger.Log(numberNames.Count + " names are numbers");
            Logger.Log(hashNumberNames.Count + " unique number name hashes");
        }

        #endregion

        #region Reductions

        public void ReductionFunctions()
        {
            Logger.Log("Analyzing reduction functions", Logger.TabChange.Increase);

            var reduces = new IReductionFunction[] { new ReductionFunction1(), new ReductionFunction2() };

            for (int i = 1; i <= reduces.Length; i++)
            {
                Logger.Log("Analyzing reduction function #" + i, Logger.TabChange.Increase);

                var names = EveryName.GroupBy(n => n).ToDictionary(g => g.Key, g => g.Count());
                var results = reduces[i - 1].ReduceNames(names);

                Logger.Log("Function #" + i + " produces " + results.Count() + " groups");

                File.WriteAllLines("Reduction_Function_" + i + ".txt", results.OrderBy(g => g.Count()).Select(group => group.Aggregate("", (c, s) => c + ",\"" + s + "\"").Remove(0, 1)));

                Logger.Log("Reduction function #" + i + " analyzed", Logger.TabChange.Decrease);
            }

            Logger.Log("Reduction functions analyzed", Logger.TabChange.Decrease);
        }

        #endregion

        #region Names

        public void NameAnalysis1()
        {
            var nodeNames = NodeNameGrouper.NodeNames();

            Logger.Log("Longest name is " + nodeNames.Max(n => n.Key.Length) + " characters long");

            var alphabet = nodeNames.Keys.SelectMany(s => s.AsEnumerable()).Distinct().OrderBy(c => c);

            Logger.Log(alphabet.Count() + " characters in alphabet \"" + alphabet.Aggregate(string.Empty, (s, c) => s + c) + "\"");

            var reducedAlphabet = nodeNames.Keys.SelectMany(s => s.ToLower().Where(c => 'a' <= c && c <= 'z')).Distinct().OrderBy(c => c);

            Logger.Log(reducedAlphabet.Count() + " characters in alphabet \"" + reducedAlphabet.Aggregate(string.Empty, (s, c) => s + c) + "\"");
        }

        public void NameAnalysis2()
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

            for (int i = 0; i < InputData.TrainingSets.Length; i++)
            {
                names[i + 1] = new SortedSet<string>();
                lines = InputData.TrainingSets[i];

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

        #endregion

        #region Edges

        public void Edges()
        {
            Logger.Log("Analyzing Edges", Logger.TabChange.Increase);

            var changers = Graph.Nodes.Count(n => n.Edges.Any(e => e.Value.Value.Schedule.Any(s => s == 0) && e.Value.Value.Schedule.Any(s => s == 1)));

            Logger.Log(changers + " nodes have edges which change weights");

            var stablePeers = Graph.Nodes.Count(n => n.Edges.Any(e => e.Value.Value.Schedule.Take(InputData.TrainingSets.Length).All(d => d == 0)));

            Logger.Log(stablePeers + " nodes point to a stable peer");

            var stableCosts = Graph.Nodes.Count(n => n.Edges.Any(e => e.Value.Value.Schedule.Take(InputData.TrainingSets.Length).All(d => d == 1)));

            Logger.Log(stableCosts + " nodes point to a stable costly connection");

            var stableConnection = Graph.Nodes.Count(n => n.Edges.Any(e => e.Value.Value.Schedule.Take(InputData.TrainingSets.Length).All(d => d == 0 || d == 1)));

            Logger.Log(stableConnection + " nodes point to a stable connection");

            Logger.Log("Edges analyzed", Logger.TabChange.Decrease);
        }

        #endregion

        #region Paths

        public void BrokenPaths()
        {
            Logger.Log("Analyzing broken paths", Logger.TabChange.Increase);

            var missingLinks = new Dictionary<string, Dictionary<int, Dictionary<string,int>>>();

            Func<string, int, string, bool> add = (from, time, to) =>
            {
                if (!missingLinks.ContainsKey(from))
                {
                    missingLinks[from] = new Dictionary<int, Dictionary<string, int>>();
                }
                if (!missingLinks[from].ContainsKey(time))
                {
                    missingLinks[from][time] = new Dictionary<string, int>();
                }
                if (!missingLinks[from][time].ContainsKey(to))
                {
                    missingLinks[from][time][to] = 0;
                }
                missingLinks[from][time][to]++;

                return true;
            };

            foreach (var path in EveryPath)
            {
                for (int i = 1; i < path.Length; i++)
                {
                    if (!path[i - 1].Edges.ContainsKey(path[i]))
                    {
                        add(path[i - 1].Value.Name, int.MinValue, path[i].Value.Name);
                    }
                    else
                    {
                        var edge = path[i - 1].Edges[path[i]];

                        for (int t = 0; t < InputData.TrainingSets.Length; t++)
                        {
                            if (edge.Value.Schedule[t] == double.PositiveInfinity)
                            {
                                add(path[i - 1].Value.Name, t, path[i].Value.Name);
                            }
                        }
                    }
                }
            }

            var brokenLinks = missingLinks.Sum(froms=>froms.Value.Count);
            var brokenCases = missingLinks.Sum(froms=>froms.Value.Sum(times=>times.Value.Sum(counts=>counts.Value)));

            Logger.Log(brokenLinks + " broken links found over " + brokenCases + " broken cases");

            File.WriteAllText(
                "Missing_Links.txt",
                missingLinks
                    .OrderByDescending(ml => ml.Value.Sum(times => times.Value.Sum(counts => counts.Value)))
                    .Aggregate("",
                        (c, s) =>
                            c + s.Key + "\n" +
                            s.Value.OrderBy(w => w.Key)
                            .Aggregate("", (ws, w) => ws + w.Value.OrderBy(n => n.Key)
                                .Aggregate("",
                                    (ns, n) =>
                                        ns + "[" + (w.Key != int.MinValue ? w.Key.ToString("00") : "!!") + "]" + "[" + n.Value.ToString("000") + "]" + "\t" + n.Key + "\n"
                            ))));

            Logger.Log("Broken paths analyzed", Logger.TabChange.Decrease);
        }

        public void PathLengths()
        {
            Logger.Log(
                "Path lengths range from " +
                InputData.Paths.Where(l => !string.IsNullOrEmpty(l)).Min(l => l.Count(c => c == '|') + 1) +
                " to " +
                InputData.Paths.Max(l => l.Count(c => c == '|') + 1));
            Logger.Log(InputData.Paths.Where(l => l.Count(c => c == '|') == 0).Aggregate("", (c, l) => c + '|' + l).Remove(0, 1));
        }

        public void SinglePaths()
        {
            var singlePaths = InputData.Paths.Select((p, i) => new Tuple<int, string>(i, p)).Where(p => p.Item2.Count(c => c == '|') == 0).ToList();
            Logger.Log("There are " + singlePaths.Count + " paths with a single node.");
            foreach (var singlePath in singlePaths)
            {
                Logger.Log(string.Format("{0}. {1}", singlePath.Item1, singlePath.Item2));
            }
        }

        #endregion
    }
}
