using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestMoveScene : MonoBehaviour {


    public void MoveScene()
    {
        if (transform.name== "AnimalPuzzle")
        {
            SceneManager.LoadScene("AnimalPuzzle");
        }
        else if(transform.name == "ShadowPuzzle")
        {
            SceneManager.LoadScene("ShadowPuzzle");
        }
    }
}
