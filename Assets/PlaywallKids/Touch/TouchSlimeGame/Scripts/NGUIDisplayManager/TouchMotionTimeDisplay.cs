using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchMotionTimeDisplay : UITimeDisplay
{

    public UIAtlas numericsAtlas;
    public UISprite[] times;
    public UISprite imgClock;
    public string fileName;

    int time;
    int iSwitchPos;
    bool bRight;
    Vector3 fClockAngle;
    void Awake()
    {
        if(numericsAtlas != null)
        {
            for(int i = 0 ; i < times.Length; ++i)
            {   times[i].atlas = numericsAtlas;  }
        }

        fClockAngle = Vector3.zero;
    }
    
    void Update()
    {
        ClockUpdate();
    }

    void ClockUpdate()
    {
        if (bRight)
        {
            if (fClockAngle.z > -15f)
            {
                fClockAngle.z -= 20f * Time.deltaTime;
            }
            else
            {
                fClockAngle.z = -15f;
                bRight = false;
            }
        }
        else
        {
            if (fClockAngle.z < 15)
                fClockAngle.z += 20 * Time.deltaTime;
            else
            {
                fClockAngle.z = 15;
                bRight = true;
            }
        }

        imgClock.cachedTransform.localRotation = Quaternion.Euler( fClockAngle );
    }

    public override void InitTime(int _iTime)
    {
        iSwitchPos = 0;
        List<int> temp = NumericSplit.Split(_iTime);

        if (temp.Count < 3) //  adjust Time position
        {
            float w = temp.Count == 2 ? times[0].width * 0.4f : 0f;
            for (int i = 0; i < temp.Count; ++i)
            {
                 int n = (i == 0 ? 1 : -1);
                 Vector3 pos = times[i].cachedTransform.localPosition;
                 pos.x = 0 + (n * w);
                 times[i].cachedTransform.localPosition = pos;
            }
        }
       
        for (int i = 0; i < times.Length; ++i)
        {
            if(i < temp.Count)
            {
                times[i].cachedGameObject.SetActive(true);
                times[i].spriteName = temp[i].ToString();
            }else
            {   times[i].cachedGameObject.SetActive(false);  }
        }

        imgClock.cachedGameObject.SetActive(true);
    }

    public override void ChangeTime(int _iTime)
    {
        if (time == _iTime) return;
        time = _iTime;

        List<int> temp = NumericSplit.Split(_iTime);

        if ( temp.Count < 3 ) //  UISprite of time numeric confirmation of location
        {
            if ((iSwitchPos & 0x01 << temp.Count) == 0)
            {
                iSwitchPos |= 0x01 << temp.Count;
                float w = temp.Count == 2 ? times[0].width * 0.4f : 0f;
                for (int i = 0; i < times.Length; ++i)
                {
                    if (temp.Count > i)
                    {
                        int n = (i == 0 ? 1 : -1);
                        Vector3 pos = times[i].cachedTransform.localPosition;
                        pos.x = 0 + (n * w);
                        times[i].cachedTransform.localPosition = pos;
                    }
                    else
                    {
                        if (times[i].cachedGameObject.activeInHierarchy)
                        {   times[i].cachedGameObject.SetActive(false);  }
                    }
                }
            }
        }

        for (int i = 0; i < temp.Count; ++i)
        {
            times[i].spriteName = fileName + temp[i].ToString(); // change numeric UISprite
        }

    }
    

}
