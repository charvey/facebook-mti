using System;
using System.Collections.Generic;
using System.Linq;

namespace MappingTheInternet.HashFunctions
{
    public abstract class HashFunction : IHashFunction
    {
        public static readonly HashFunction Preferred = new HashFunction8();

        protected static char[] SpecialSymbols = " !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~".ToCharArray();

        protected static string Sort(string word)
        {
            var chars = word.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                int min = i;
                for (int j = i + 1; j < chars.Length; j++)
                {
                    if (chars[j] < chars[min])
                    {
                        min = j;
                    }
                }
                char t = chars[i];
                chars[i] = chars[min];
                chars[min] = t;
            }

            return new string(chars);
        }

        public abstract string HashName(string name);
    }

    public class HashFunction8 : HashFunction
    {
        protected static readonly IEnumerable<string> ReservedWords = (new[] { "INC", "LTD", "LLC", "SERVICES", "NETWORK", "AS", "SYSTEM", "AUTONOMOUS" }).Select(Sort);

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

            var words = cleanName.Split();

            var hypenedWords = words.Where(w => w.Contains('-'));

            var extraWords = hypenedWords.SelectMany(w => (new[] { w.Replace("-", "") }).Concat(w.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries)));

            var allWords = words.Concat(extraWords);

            var sortedWords = allWords.Select(Sort);

            var distinctWords = sortedWords.Distinct().ToList();

            var filteredWords = distinctWords.All(w => ReservedWords.Contains(w))
                ? distinctWords.ToList()
                : distinctWords.Except(ReservedWords).ToList();

            var mainWords = filteredWords.Any(w => w.Length > 2)
                                ? filteredWords.Where(w => w.Length > 2)
                                : filteredWords.Any(w => w.Length > 1)
                                      ? filteredWords.Where(w => w.Length > 1)
                                      : filteredWords.Where(w => w.Length > 0);

            var orderedWords = mainWords.OrderBy(s => s);

            var hashName = orderedWords.Aggregate(id, (s, c) => s + "|" + c);

            return hashName;
        }
    }
}
