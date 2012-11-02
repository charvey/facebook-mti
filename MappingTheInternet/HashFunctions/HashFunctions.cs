using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MappingTheInternet.HashFunctions
{
    public abstract class HashFunction : IHashFunction
    {
        protected static char[] SpecialSymbols = " !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~".ToCharArray();

        public abstract string HashName(string name);
    }

    public class HashFunction1:HashFunction
    {
        public override string HashName(string name)
        {
            if (name.All(c => '0' <= c && c <= '9'))
                return name;

            foreach (var specialSymbol in SpecialSymbols.Where(c=>c!=' '))
            {
                name = name.Replace("" + specialSymbol, "");
            }

            var words = name.ToUpper().Split(SpecialSymbols, StringSplitOptions.RemoveEmptyEntries);

            var sortedDistinctWords = words.Select(w => new string(w.OrderBy(c => c).ToArray())).Distinct().OrderBy(s => s);

            var hashName = sortedDistinctWords.Aggregate("", (s, c) => s + "|" + c);

            return hashName;
        }
    }

    public class HashFunction2 : HashFunction
    {
        public override string HashName(string name)
        {
            if (name.All(c => '0' <= c && c <= '9'))
                return name;

            var words = name.ToUpper().Split(SpecialSymbols, StringSplitOptions.RemoveEmptyEntries);

            var sortedDistinctWords = words.Select(w => new string(w.OrderBy(c => c).ToArray())).Distinct().OrderBy(s => s);

            var hashName = sortedDistinctWords.Aggregate("", (s, c) => s + "|" + c);

            return hashName;
        }
    }
}
