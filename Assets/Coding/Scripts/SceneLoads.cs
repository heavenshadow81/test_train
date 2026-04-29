using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoads : MonoBehaviour
{
    public void SceneLoad(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
    }
}
