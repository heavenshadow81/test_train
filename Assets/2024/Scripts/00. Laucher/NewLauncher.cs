using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewLauncher : MonoBehaviour
{
    [SerializeField] private string[] arguments = null;
    [SerializeField] private string targetArgument = "Mogencelab0058";
    [SerializeField] private GameObject messageUI; 

    private void Awake()
    {
        arguments = Environment.GetCommandLineArgs();
    }

    private void Start()
    {

#if LICENSEKEY_CERTIFICATION_NOT
        LoadNextSceneAsync();
#endif 
        if (arguments.Length > 1)
        {
            bool isValidArgument = false;
            foreach (string arg in arguments)
            {
                if (arg.Equals(targetArgument, StringComparison.OrdinalIgnoreCase))
                {
                    isValidArgument = true;
                    break;
                }
            }

            if (isValidArgument)
            {
                LoadNextSceneAsync();
            }
            else
            {
                ShowMessageUI();
            }
        }
        else
        {
            ShowMessageUI();
        }
    }

    private void LoadNextSceneAsync()
    {
        StartCoroutine(LoadSceneCoroutine());
    }

    private System.Collections.IEnumerator LoadSceneCoroutine()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private void ShowMessageUI()
    {
        if (messageUI != null)
        {
            messageUI.SetActive(true);
        }
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        // 에디터에서는 종료되지 않으므로 에디터 플레이모드를 중지
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 실제 빌드된 어플리케이션 종료
        Application.Quit();
#endif
    }
}
