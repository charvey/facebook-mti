using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MappingTheInternet.HashFunctions
{
    public class HashFunction1:IHashFunction
    {
        public string HashName(string name)
        {
            if (name.All(c => '0' <= c && c <= '9'))
                return name;

            var specialSymbols = name.ToLower().Where(c => !(('a' <= c && c <= 'z') || ('0' <= c && c <= '9'))).Distinct().ToArray();

            foreach (var specialSymbol in specialSymbols)
            {
                name = name.Replace("" + specialSymbol, "");
            }

            var words = name.ToUpper().Split(specialSymbols, StringSplitOptions.RemoveEmptyEntries);

            var sortedDistinctWords = words.Select(w => new string(w.OrderBy(c => c).ToArray())).Distinct().OrderBy(s => s);

            var hashName = sortedDistinctWords.Aggregate("", (s, c) => s + "|" + c);

            return hashName;
        }
    }
}
