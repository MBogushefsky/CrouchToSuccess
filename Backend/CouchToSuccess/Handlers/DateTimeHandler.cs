using System;
using System.Collections.Generic;

namespace CouchToSuccess.Handlers
{
    public class DictionaryHandler
    {
        public static Dictionary<string, int> AddTwoDictionaries(Dictionary<string, int> dictionary1, Dictionary<string, int> dictionary2)
        {
            Dictionary<string, int> resultDictionary = new Dictionary<string, int>();
            foreach (KeyValuePair<string, int> dictionaryPair in dictionary1)
            {
                if (resultDictionary.ContainsKey(dictionaryPair.Key))
                {
                    resultDictionary[dictionaryPair.Key] += dictionaryPair.Value;
                }
                else
                {
                    resultDictionary[dictionaryPair.Key] = dictionaryPair.Value;
                }
            }
            foreach (KeyValuePair<string, int> dictionaryPair in dictionary2)
            {
                if (resultDictionary.ContainsKey(dictionaryPair.Key))
                {
                    resultDictionary[dictionaryPair.Key] += dictionaryPair.Value;
                }
                else
                {
                    resultDictionary[dictionaryPair.Key] = dictionaryPair.Value;
                }
            }
            return resultDictionary;
        }
    }
}