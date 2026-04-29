using UnityEngine;
using System.Collections;

public class TouchMotionCountDownController : MonoBehaviour {
    public UIAtlas numericsAtlas;
    public UISprite countDown;
    public string spriteName = "";
    int iCountDown;

    public bool Active
    {
        get
        {
            return this.gameObject.activeInHierarchy;
        }

        set
        {
            this.gameObject.SetActive(value);
        }
    }

    void Awake()
    {
        if(numericsAtlas != null)
        {   countDown.atlas = numericsAtlas; }

        this.gameObject.SetActive(true);
    }

    void OnEnable()
    {
        iCountDown = 0;
    }

    public void ChangeNumeric(int _time)
    {
        if (iCountDown == _time) return;
        iCountDown = _time;
        countDown.spriteName = spriteName + _time.ToString();
    }
}
