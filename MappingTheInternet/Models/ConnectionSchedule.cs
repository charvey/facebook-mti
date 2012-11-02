using System.Linq;

namespace MappingTheInternet.Models
{
    public class ConnectionSchedule
    {
        public readonly double[] Schedule = Enumerable.Repeat(double.PositiveInfinity, 20).ToArray();
    }
}
