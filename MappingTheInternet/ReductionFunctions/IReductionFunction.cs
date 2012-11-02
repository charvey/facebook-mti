using System.Collections.Generic;

namespace MappingTheInternet.ReductionFunctions
{
    public interface IReductionFunction
    {
        HashSet<string[]> ReduceNames(Dictionary<string, int> nodeNames);
    }
}
