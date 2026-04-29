using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkatingButton : MonoBehaviour 
{
    void Start()
    {
        gameObject.SetActive(true);
    }
    
    public void SkatingMainMenu()
    {
        gameObject.SetActive(false);
    }

}
