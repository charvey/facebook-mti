using System;
using System.Linq;

namespace MappingTheInternet
{
    public static class Extensions
    {
        public static double StdDev(this double[] data)
        {
            double avg = data.Average();

            return Math.Sqrt(data.Sum(x => Math.Pow(x - avg, 2)) / data.Length);
        }
    }
}
