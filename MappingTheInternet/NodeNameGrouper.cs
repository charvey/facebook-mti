using MappingTheInternet.Data;
using MappingTheInternet.HashFunctions;
using MappingTheInternet.ReductionFunctions;
using System;
using System.Collections.Generic;
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
            Logger.Log("Loading names", Logger.TabChange.Increase);

            var nodeNames = new Dictionary<string, int>();
            Func<string, int> add = (n) => nodeNames[n] = (nodeNames.ContainsKey(n) ? nodeNames[n] : 0) + 1;

            foreach (var trainingSet in InputData.TrainingSets)
            {
                foreach (var line in trainingSet.Select(l => l.Split('|')))
                {
                    add(line[0].Trim());
                    add(line[1].Trim());
                }
            }

            foreach (var line in InputData.Paths.Select(l => l.Split('|')))
            {
                foreach (var name in line)
                {
                    add(name.Trim());
                }
            }

            Logger.Log(nodeNames.Count + " names loaded", Logger.TabChange.Decrease);

            return nodeNames;
        }

        private static HashSet<string[]> ReduceNames(Dictionary<string, int> nodeNames)
        {
            Logger.Log("Reducing names", Logger.TabChange.Increase);

            var groups = ReductionFunction.Preferred.ReduceNames(nodeNames);

            Logger.Log("Names reduced to "+groups.Count+" groups", Logger.TabChange.Decrease);

            return groups;
        }
    }
}
