using UnityEngine;
using System.Collections;

public class TouchMotionBarTimeDisplay : UITimeDisplay
{
    public UISlider timeBar;
    const float max_width = 1f;
    
    Color color;
    bool bOnOff;
    float fTime;

    Vector3 size
    { 
        get { return timeBar.foregroundWidget.cachedTransform.localScale; }
        set { timeBar.foregroundWidget.cachedTransform.localScale = value; } 
    }
    float iPlayTime;

    void Awake()
    {
        if (!timeBar) timeBar = GetComponentInChildren<UISlider>();
        color = timeBar.foregroundWidget.color;
        size = new Vector3( max_width, size.y, 1);
    }

    void OnDisable()
    {
        timeBar.foregroundWidget.color = color;
    }

    public override void InitTime(int _iTime)
    {
        iPlayTime = _iTime;
        size = new Vector3(max_width, size.y, 1f);
    }

    public override void ChangeTime(int _iTime)
    {
        if (iPlayTime == 0) iPlayTime = 1;
        float percentage =  (float)_iTime / iPlayTime;

        timeBar.value = percentage;
        if(percentage < 0.25f)
        {
            fTime++;
            if(fTime%3 == 0)
            {
                fTime = 0;
                timeBar.foregroundWidget.color = bOnOff ? Color.red : color;
                bOnOff = !bOnOff;
            }
        }
    }
}
