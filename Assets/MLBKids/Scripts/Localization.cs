using UnityEngine;
using System.Collections.Generic;

namespace ML.MLBKids
{
    /// <summary>
    /// Simple localization class.
    /// </summary>
    public static class Localization
    {
        public static string defaultLanguage = "ko";

        // localization texts for each language
        private static Dictionary<string, Dictionary<string, string>> _localizations = new Dictionary<string, Dictionary<string, string>>();

        public static void Load()
        {
            _localizations.Clear();

            string[] list = { "ko", "en" };
            foreach (var lang in list)
            {
                TextAsset text = Resources.Load<TextAsset>(lang);
                if (text != null)
                {
                    _localizations.Add(lang, new Dictionary<string, string>());

                    string[] lines = text.text.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        string[] keyValues = line.Split(new char[] { '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
                        if (keyValues.Length >= 2)
                        {
                            string key = keyValues[0];
                            string value = keyValues[1];
                            value = value.Replace("\\n", "\n");
                            _localizations[lang][key] = value;
                        }
                    }
                }
            }
        }

        public static string Get(string key, string lang = null)
        {
            if (lang == null)
                lang = defaultLanguage;
            if (_localizations.ContainsKey(lang))
            {
                string value = key;
                if (!_localizations[lang].TryGetValue(key, out value))
                    value = key;
                return value;
            }
            return key;
        }
    }
}