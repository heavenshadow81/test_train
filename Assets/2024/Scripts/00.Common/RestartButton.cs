using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public void Regame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void KartRegame(string name)
    {
        SceneManager.LoadScene(name);
    }
}

