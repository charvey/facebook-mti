using System.IO;

namespace MappingTheInternet
{
    public static class InputData
    {
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

        private static string[][] _trains = new string[15][];
        public static string[] Trains(int i)
        {
            if (_trains[i] == null)
            {
                _trains[i] = File.ReadAllLines("data/train/train" + (i+1) + ".txt");
            }

            return _trains[i];
        }
    }
}
