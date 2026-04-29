using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TravelButton : MonoBehaviour {

    public RectTransform Target;
    Image TargetImage;
    RectTransform _myTr;
    // Use this for initialization
    void Start()
    {
        _myTr = this.GetComponent<RectTransform>();
    }
    // Update is called once per frame
    void Update()
    {
        _myTr.anchoredPosition = Target.anchoredPosition;
    }
}