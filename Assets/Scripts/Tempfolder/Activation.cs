using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activation : MonoBehaviour {

    public GameObject menu;

	void Start()
    {
        ActivateButton();
    }
    void ActivateButton()
    {
        menu.GetComponent<UIPlayTween>().OnClick();

    }
}
