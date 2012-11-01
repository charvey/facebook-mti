using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MappingTheInternet
{
    public static class Logger
    {
        public static void Log(string text)
        {
            //File.AppendAllText("log", text + "\n");
            Console.Out.WriteLine(text);
        }

        public static void Wait()
        {
            Console.In.ReadLine();
        }
    }
}
