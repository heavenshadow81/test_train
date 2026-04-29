using UnityEngine;
using UnityEngine.UI;

namespace ML.MLBKids
{
    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour
    {
        public string key;

        public void OnEnable()
        {
            Text text = GetComponent<Text>();
            if (string.IsNullOrEmpty(key))
                key = text.text;
            text.text = Localization.Get(key);
        }
    }
}