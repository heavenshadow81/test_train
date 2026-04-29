using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckCountry_Anim : MonoBehaviour {
    public RectTransform Target;
    Image TargetImage;
    RectTransform _myTr;
    Image _myImage;
    Image _checkImage;
	// Use this for initialization
	void Start () {
        _myTr = this.GetComponent<RectTransform>();
        TargetImage = Target.GetComponent<Image>();
        _myImage = this.GetComponent<Image>();
        if(this.transform.childCount > 0)
            _checkImage = this.transform.GetChild(0).GetComponent<Image>();
    }	
	// Update is called once per frame
	void Update ()
    {
        _myTr.anchoredPosition = Target.anchoredPosition;
        if(_myImage != null && TargetImage != null)
            _myImage.color = TargetImage.color;
        if(_checkImage != null)
            _checkImage.color = TargetImage.color;
    }
}
