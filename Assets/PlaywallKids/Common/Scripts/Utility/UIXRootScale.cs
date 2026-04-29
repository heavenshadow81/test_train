using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The extension class which scales UI components.
/// </summary>
namespace ML.PlaywallKids.Common
{
    public class UIXRootScale : MonoBehaviour
    {
        public float scale = 1.0f;
        public static UIXRootScale instance { get; private set; }

        public void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                scale = CommonSettings.uiScale;
                SceneManager.sceneLoaded += _OnSceneLoaded;
            }
            else
            {
                Destroy(this);
            }
        }

        public void OnDestroy()
        {
            if (instance == this)
            {
                SceneManager.sceneLoaded -= _OnSceneLoaded;
                instance = null;
            }
        }

        private void _OnSceneLoaded(Scene s, LoadSceneMode m)
        {
            UIRoot[] list = FindObjectsOfType<UIRoot>();
            for (int i = 0; i < list.Length; i++)
            {
                UIRoot root = list[i];
                root.manualHeight = Mathf.RoundToInt(root.manualHeight / scale);
            }
        }
    }
}