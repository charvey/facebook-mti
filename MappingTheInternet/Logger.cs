using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MappingTheInternet
{
    public static class Logger
    {
        public static void Log(string text)
        {
            Console.Out.WriteLine(text);
        }

        public static void Wait()
        {
            Console.In.ReadLine();
        }
    }
}
