using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class ListSceneBtn : MonoBehaviour {

    public void ListScene()
    {
        SceneManager.LoadScene("SelectPuzzle");
    }
}
