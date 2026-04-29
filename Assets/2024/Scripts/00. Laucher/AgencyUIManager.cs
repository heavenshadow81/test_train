using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AgencyUIManager : MonoBehaviour
{
	[SerializeField] Button playButton;
	[SerializeField] Button registerButton;
	[SerializeField] Button logOutButton;
    [SerializeField] GameObject registerUI;

    private void Start()
    {
        playButton.onClick.AddListener(() => LoadNextSceneAsync(1));
        registerButton.onClick.AddListener(OnRegisterButtonClicked);
        logOutButton.onClick.AddListener(OnLogOutButtonClicked);
    }

    private void OnRegisterButtonClicked()
    {
        registerUI.SetActive(true);
        gameObject.SetActive(false);
    }

    private void OnLogOutButtonClicked()
    {
        PlayerPrefs.DeleteKey("BAXLoginID");
        PlayerPrefs.DeleteKey("BAXPassword");

        LoadNextSceneAsync(0);
    }

    private void LoadNextSceneAsync(int sceneIdx)
    {
        StartCoroutine(LoadSceneCoroutine(sceneIdx));
    }

    private System.Collections.IEnumerator LoadSceneCoroutine(int sceneIdx)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIdx);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
