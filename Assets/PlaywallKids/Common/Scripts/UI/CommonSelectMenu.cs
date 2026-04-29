using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CommonSelectMenu : MonoBehaviour {
    
    public void SelectIntro()
    {
        SceneManager.LoadScene("SpaceIntro");
    }
    
    public void ComeBackToSubMenu()
    {
        SceneManager.LoadSceneAsync("DragonParkCommonSelectMenu");
    }

    public void ComeBackToMainMenu()
    {
        
        SceneManager.LoadSceneAsync("BigBoardMainMenu");
    }

    public void SceneRearrange()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}
