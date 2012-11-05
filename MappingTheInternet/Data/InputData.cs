using System.IO;
using System.Linq;

namespace MappingTheInternet.Data
{
    public static class InputData
    {
        private static bool _fullData = true;
        public static bool FullData
        {
            get
            {
                return _fullData;
            }
            set
            {
                _trainingSets = null;
                _fullData = value;
            }
        }

        private static string[] _paths;
        public static string[] Paths
        {
            get
            {
                if (_paths == null)
                {
                    _paths = File.ReadAllLines("data/paths.txt");
                }

                return _paths;
            }
        }

        private static string[][] _trainingSets;
        public static string[][] TrainingSets
        {
            get
            {
                if (_trainingSets == null)
                {
                    _trainingSets = Enumerable.Range(0, FullData ? 15 : 10).Select(i => File.ReadAllLines("data/train/train" + (i + 1) + ".txt")).ToArray();
                }

                return _trainingSets;
            }
        }
    }
}
