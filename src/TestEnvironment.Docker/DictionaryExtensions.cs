using System.Collections.Generic;
using System.Linq;

namespace TestEnvironment.Docker
{
    public static class DictionaryExtensions
    {
        public static IDictionary<string, string> MergeDictionaries(this IDictionary<string, string> dictionary, IDictionary<string, string> other)
        {
            if (other is null) return dictionary;

            var nonExistentEnvironmentVariables = other.Where(e => !dictionary.ContainsKey(e.Key));
            return dictionary.Concat(nonExistentEnvironmentVariables).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}
