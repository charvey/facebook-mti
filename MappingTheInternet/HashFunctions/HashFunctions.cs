using System;
using System.Collections.Generic;
using System.Linq;

namespace MappingTheInternet.HashFunctions
{
    public abstract class HashFunction : IHashFunction
    {
        public static readonly HashFunction Preferred = new HashFunction8();

        protected static char[] SpecialSymbols = " !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~".ToCharArray();

        protected static string sort(string word)
        {
            var chars = word.ToCharArray();

            char t;
            int min;
            for (int i = 0; i < chars.Length; i++)
            {
                min = i;
                for (int j = i + 1; j < chars.Length; j++)
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

    public class HashFunction8 : HashFunction
    {
        private static IEnumerable<string> reservedWords = (new[] { "INC", "LTD", "LLC", "SERVICES", "NETWORK", "AS", "SYSTEM", "AUTONOMOUS" }).Select(sort);

        public override string HashName(string name)
        {
            string id = "#";
            int number;
            if (int.TryParse(name, out number))
            {
                id = number.ToString();
                name = string.Empty;
            }

            var cleanName = name.Replace(".", "").Replace(",", "").Replace("_", " ").ToUpper();

            var basicWords = cleanName.Split().Select(sort).Distinct();

            var extraWords = basicWords.Where(w => w.Contains('-')).SelectMany(w => w.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries));

            var distinctWords = basicWords.Concat(extraWords).Distinct();

            var filteredWords = distinctWords.All(w => reservedWords.Contains(w))
                ? distinctWords
                : distinctWords.Except(reservedWords);

            var mainWords = filteredWords.Any(w => w.Length > 2)
                ? filteredWords.Where(w => w.Length > 2)
                : filteredWords.Where(w => w.Length > 1);

            var sortedWords = mainWords.OrderBy(s => s);

            var hashName = sortedWords.Aggregate(id, (s, c) => s + "|" + c);

            return hashName;
        }
    }
}
