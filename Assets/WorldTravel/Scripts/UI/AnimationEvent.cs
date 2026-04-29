using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour {
    public GameObject _text;
    public void TextOn()
    {
        if(_text != null)
            _text.SetActive(true);
    }

    public void TextOff()
    {
        if (_text != null)
            _text.SetActive(false);
    }
}
