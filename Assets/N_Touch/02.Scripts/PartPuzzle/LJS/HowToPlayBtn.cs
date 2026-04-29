using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class HowToPlayBtn : MonoBehaviour {

    public void ChangeHowtoplayScene()
    {
        SceneManager.LoadScene("HowToPlay");
    }
}
