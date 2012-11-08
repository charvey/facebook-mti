using System;
using System.Linq;

namespace MappingTheInternet
{
    public static class Extensions
    {
        public static double StdDev(this double[] data)
        {
            double avg = data.Average();

            double sum = data.Sum(x => Math.Pow(x - avg, 2));

            return Math.Sqrt(sum / data.Length);
        }
    }
}
