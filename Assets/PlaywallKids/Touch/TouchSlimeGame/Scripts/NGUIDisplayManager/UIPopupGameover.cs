using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIPopupGameover : MonoBehaviour {

    public void Ok()
    {
        string scene = "InteractionTouchSlime";
        SceneManager.LoadScene(scene);

    }

    public void Cancel()
    {

        Debug.Log("close");
        //Application.loadedLevel("");
    }
}
