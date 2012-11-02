using MappingTheInternet.Data;
using MappingTheInternet.Graph;
using MappingTheInternet.HashFunctions;
using MappingTheInternet.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MappingTheInternet
{
    public class Analyzer
    {
        private IEnumerable<string> EveryName
        {
            get
            {
                return InputData.TrainingSets.SelectMany(s => s).SelectMany(l => l.Split('|').Take(2)).Concat(InputData.Paths.SelectMany(l => l.Split('|'))).Select(s => s.Trim());
            }
        }

        public void Analyze()
        {
            HashFunctions();
            HashNames();

            HashNumberNames();
            
            NameAnalysis1();
            NameAnalysis2();

            Edges();

            PathLengths();
            SinglePaths();
        }

        public void HashFunctions()
        {
            var functions = new IHashFunction[] { new HashFunction1(), new HashFunction2() };
            var results = functions.Select(f => EveryName.Distinct().GroupBy(n => f.HashName(n)).Select(g => g.ToArray()).OrderBy(s=>s.Length).ToArray()).ToArray();
            
            for (int i = 1; i <= functions.Length; i++)
            {
                Logger.Log("Function " + i + " produces " + results[i-1].Length + " unique names");
            }
        }

        public void HashNames()
        {
            Logger.Log("\"FPUA - Communications\" becomes \"" + NodeNameGrouper.HashName("FPUA - Communications") + "\"");
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
                    var hash = NodeNameGrouper.HashName(name);
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

            for (int i = 0; i < 15; i++)
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

        public void Edges()
        {
            var nodeNameMapper = new NodeNameMapper();

            var graph = new Graph<ASNode, ConnectionSchedule>();

            for (int i = 0; i < 15; i++)
            {
                foreach (var names in InputData.TrainingSets[i].Select(s => s.Split('|').Select(n => n.Trim()).ToArray()))
                {
                    foreach (var name in names.Take(2))
                    {
                        if (nodeNameMapper.Get(name) == null)
                        {
                            var node = new Node<ASNode, ConnectionSchedule>(new ASNode(name));
                            nodeNameMapper.Set(name, node);
                            graph.AddNode(node);
                        }
                    }

                    var from = nodeNameMapper.Get(names[0]);
                    var to = nodeNameMapper.Get(names[1]);

                    var edge = from.GetEdge(to);
                    if (edge == null)
                    {
                        edge = new Edge<ConnectionSchedule>(new ConnectionSchedule());
                        from.AddEdge(to, edge);
                    }
                    edge.Value.Schedule[i] = double.Parse(names[2]);
                }
            }

            var changers = graph.Nodes.Count(n => n.Edges.Any(e => e.Value.Value.Schedule.Any(s => s == 0) && e.Value.Value.Schedule.Any(s => s == 1)));

            Logger.Log(changers + " nodes have edges which change weights");

            var stablePeers = graph.Nodes.Count(n => n.Edges.Any(e => e.Value.Value.Schedule.Take(15).All(d => d == 0)));

            Logger.Log(stablePeers + " nodes point to a stable peer");

            var stableCosts = graph.Nodes.Count(n => n.Edges.Any(e => e.Value.Value.Schedule.Take(15).All(d => d == 1)));

            Logger.Log(stableCosts + " nodes point to a stable costly connection");

            var stableConnection = graph.Nodes.Count(n => n.Edges.Any(e => e.Value.Value.Schedule.Take(15).All(d => d == 0 || d == 1)));

            Logger.Log(stableConnection + " nodes point to a stable connection");
        }

        public void PathLengths()
        {
            Logger.Log(
                "Path lengths range from " +
                InputData.Paths.Where(l => !string.IsNullOrEmpty(l)).Min(l => l.Count(c => c == '|') + 1) +
                " to " +
                InputData.Paths.Max(l => l.Count(c => c == '|') + 1));
            Logger.Log(InputData.Paths.Where(l => l.Count(c => c == '|') == 0).Aggregate("", (c, l) => c + '|' + l).Remove(0,1));
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
    }
}
