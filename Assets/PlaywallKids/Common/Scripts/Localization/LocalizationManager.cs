using UnityEngine;
using System.Collections.Generic;

public static class LocalizationManager
{
    class StringData
    {
        public string strKor, strEng;
        public StringData(string kor, string eng)
        {
            strKor = kor;
            strEng = eng;
        }
    }

    public static LocalizationLanguage curLanguage
    {
        get  {
            return BigboardServer.cachedSituationalInfo.localization;
        }
    }

    private static List<LocalizationBigboard> listLocalizationBigboard;

    private static Dictionary<LocalizationKey, StringData> _dictionaryStringData = new Dictionary<LocalizationKey, StringData>();

    static LocalizationManager()
    {
        listLocalizationBigboard = new List<LocalizationBigboard>();
        LoadStringData();
    }

    public static string GetData(LocalizationKey key)
    {
        if (_dictionaryStringData.ContainsKey(key))
        {
            switch (curLanguage)
            {
                case LocalizationLanguage.Korean: return _dictionaryStringData[key].strKor;
                case LocalizationLanguage.English: return _dictionaryStringData[key].strEng;
            }
            return "";
        }
        else
        {
            return string.Format("#[{0}]", (int)key);
        }
    }

    public static GameObject AddLocalization(GameObject go, LocalizationKey key, LocalizationDataType type = LocalizationDataType.String)
    {
        LocalizationBigboard item = go.GetComponent<LocalizationBigboard>();
        if (item == null)   {
            item = go.AddComponent<LocalizationBigboard>();
        }
        item.curKey = key;
        item.dataType = type;

        if(listLocalizationBigboard.Contains(item) == false)
            listLocalizationBigboard.Add(item);

        return go;
    }

    public static void RemoveItme(LocalizationBigboard item)
    {
        if (listLocalizationBigboard.Contains(item))
            listLocalizationBigboard.Remove(item);
    }

    public static void AllReplace()
    {
        foreach( LocalizationBigboard item in listLocalizationBigboard)
        {
            if (item != null)
                item.Replace();
        }
    }

    private static void LoadStringData()
    {
        _dictionaryStringData = new Dictionary<LocalizationKey, StringData>();
        TextAsset text = (TextAsset)Resources.Load("Data/Localization", typeof(TextAsset));
        
        Debug.Log(text.text);
        Dictionary<string, object> jsonData = MiniJSON.Json.Deserialize(text.text) as Dictionary<string, object>;
        if (jsonData != null && jsonData["input"] != null)
        {
            List<object> list = jsonData["input"] as List<object>;
            for (int i = 0; i < list.Count; i++)
            {
                Dictionary<string, object> item = list[i] as Dictionary<string, object>;
                
                int id = int.Parse(item["ID"].ToString());
                string korean = item["Korean"].ToString();
                string english = item["English"].ToString();

                LocalizationKey key = (LocalizationKey)id;
                               
                _dictionaryStringData.Add(key, new StringData(korean, english));
            }
        }
    }
}
