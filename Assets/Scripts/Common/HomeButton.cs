using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class HomeButton : MonoBehaviour
{
    public string SceneName = "BigBoardMainMenu";

    public void Home()
    {
        SceneManager.LoadSceneAsync(SceneName);
    }

}
