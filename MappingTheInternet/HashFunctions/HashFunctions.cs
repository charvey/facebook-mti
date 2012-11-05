using System;
using System.Linq;

namespace MappingTheInternet.HashFunctions
{
    public abstract class HashFunction : IHashFunction
    {
        public static readonly HashFunction Preferred = new HashFunction5();

        protected static char[] SpecialSymbols = " !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~".ToCharArray();

        protected string sort(string word)
        {
            var chars = word.ToCharArray();

            char t;
            int min;
            for (int i = 0; i < chars.Length; i++)
            {
                min = i;
                for (int j = i+1; j < chars.Length; j++)
                {
                    if (chars[j] < chars[min])
                    {
                        min = j;
                    }
                }
                t = chars[i];
                chars[i] = chars[min];
                chars[min] = t;
            }

            return new string(chars);
        }

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

            var sortedDistinctWords = words.Select(sort).Distinct().OrderBy(s => s);

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

            var sortedDistinctWords = words.Select(sort).Distinct().OrderBy(s => s);

            var hashName = sortedDistinctWords.Aggregate("", (s, c) => s + "|" + c);

            return hashName;
        }
    }

    public class HashFunction3 : HashFunction
    {
        public override string HashName(string name)
        {
            if (name.All(c => '0' <= c && c <= '9'))
                return name;

            foreach (var specialSymbol in SpecialSymbols.Where(c => c != ' '))
            {
                name = name.Replace("" + specialSymbol, "");
            }

            var words = name.ToUpper().Split(SpecialSymbols, StringSplitOptions.RemoveEmptyEntries).Where(w => w.Length > 2);

            var sortedDistinctWords = words.Select(sort).Distinct().OrderBy(s => s);

            var hashName = sortedDistinctWords.Aggregate("", (s, c) => s + "|" + c);

            return hashName;
        }
    }

    public class HashFunction4 : HashFunction
    {
        public override string HashName(string name)
        {
            if (name.All(c => '0' <= c && c <= '9'))
                return name;

            var words = name.ToUpper().Split(SpecialSymbols, StringSplitOptions.RemoveEmptyEntries).Where(w => w.Length > 2);

            var sortedDistinctWords = words.Select(sort).Distinct().OrderBy(s => s);

            var hashName = sortedDistinctWords.Aggregate("", (s, c) => s + "|" + c);

            return hashName;
        }
    }

    public class HashFunction5 : HashFunction
    {
        public override string HashName(string name)
        {
            if (name.All(c => '0' <= c && c <= '9'))
                return name;

            foreach (var specialSymbol in SpecialSymbols.Where(c => c != ' '))
            {
                name = name.Replace("" + specialSymbol, "");
            }

            var words = name.ToUpper().Split(SpecialSymbols, StringSplitOptions.RemoveEmptyEntries).Where(w => w.Length > 3);

            var sortedDistinctWords = words.Select(sort).Distinct().OrderBy(s => s);

            var hashName = sortedDistinctWords.Aggregate("", (s, c) => s + "|" + c);

            return hashName;
        }
    }

    public class HashFunction6 : HashFunction
    {
        public override string HashName(string name)
        {
            if (name.All(c => '0' <= c && c <= '9'))
                return name;

            var words = name.ToUpper().Split(SpecialSymbols, StringSplitOptions.RemoveEmptyEntries).Where(w => w.Length > 3);

            var sortedDistinctWords = words.Select(sort).Distinct().OrderBy(s => s);

            var hashName = sortedDistinctWords.Aggregate("", (s, c) => s + "|" + c);

            return hashName;
        }
    }

    public class HashFunction7 : HashFunction{
        public override string HashName(string name)
        {
            if (name.All(c => '0' <= c && c <= '9'))
                return name;

            foreach (var specialSymbol in SpecialSymbols.Where(c => c != ' '))
            {
                name = name.Replace("" + specialSymbol, "");
            }

            var reservedWords = new[] { "communications" }.Select(sort).Select(w=>w.ToUpper());

            var words = name.ToUpper().Split(SpecialSymbols, StringSplitOptions.RemoveEmptyEntries);         

            var sortedDistinctWords = words.Select(sort).Distinct().OrderBy(s => s);

            var importantWords = sortedDistinctWords.Where(w => !reservedWords.Contains(w));

            var largeWords = importantWords.Any(w => w.Length > 3)
                ? importantWords.Where(w => w.Length > 3)
                : importantWords.Where(w => w.Length > 2);

            var hashName = sortedDistinctWords.Aggregate("", (s, c) => s + "|" + c);

            return hashName;
        }
    }

    public class HashFunction8 : HashFunction
    {
        public override string HashName(string name)
        {
            if (name.All(c => '0' <= c && c <= '9'))
                return name;

            name = name.Replace("-", "").Replace(".", "").Replace(",", "").Replace("_", " ").ToUpper();

            var words = name.Split().Where(w => w.Length > 2);

            var reservedWords = (new[] { "INC", "LTD", "LLC" ,"SERVICES","NETWORK","AS","SYSTEM","AUTONOMOUS"}).Select(sort);
            
            var distinctWords = words.Select(sort).Distinct();

            var filteredWords = distinctWords.Except(reservedWords);

            var sortedWords = filteredWords.OrderBy(s => s);

            var hashName = sortedWords.Aggregate("", (s, c) => s + "|" + c);

            return hashName;
        }
    }
}
