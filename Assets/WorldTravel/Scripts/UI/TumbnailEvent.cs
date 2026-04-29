using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TumbnailEvent : MonoBehaviour {
    public GameObject[] TravelObj;
    public RectTransform[] TravelTr;
    public Image[] Travelimages;
    public GameObject[] CheckedObj;
    //Tumbnail_Checked
    //Tumbnail_Defaulte
    int TravelIndex;
    int imgcnt;

    public Vector2 onepos;
    public Vector2[] twopos;
    public Vector2[] treepos;
    public Vector2[] fourpos;
    public Vector2[] fivepos;
    bool _setPosition;


    public Sprite[] Culture;
    public Sprite[] Art;
    public Sprite[] City;
    public Sprite[] Tour;
    public Sprite[] Nature;
    public Sprite[] History;

    public Sprite emptyImage;
    public void ResetCheck()
    {
        TravelIndex = 0;
        imgcnt = 0;
        onepos = new Vector2(0, 0);

        twopos = new Vector2[2];
        twopos[0] = new Vector2(-75, 0);
        twopos[1] = new Vector2(75, 0);

        treepos = new Vector2[3];
        treepos[0] = new Vector2(-150, 0);
        treepos[1] = new Vector2(0, 0);
        treepos[2] = new Vector2(150, 0);

        fourpos = new Vector2[4];
        fourpos[0] = new Vector2(-225, 0);
        fourpos[1] = new Vector2(-75, 0);
        fourpos[2] = new Vector2(75, 0);
        fourpos[3] = new Vector2(225, 0);

        fivepos = new Vector2[5];
        fivepos[0] = new Vector2(-300, 0);
        fivepos[1] = new Vector2(-150, 0);
        fivepos[2] = new Vector2(0, 0);
        fivepos[3] = new Vector2(150, 0);
        fivepos[4] = new Vector2(300, 0);

        _setPosition = false;
        for (int i = 0; i < CheckedObj.Length; i++)
        {
            CheckedObj[i].SetActive(false);
            TravelObj[i].SetActive(true);
        }
    }
    public void SetImagePosition(int cnt)
    {
        imgcnt = cnt;
        if (!_setPosition)
        {
            _setPosition = true;
            switch (cnt)
            {
                case 1:
                    for (int i = 0; i < cnt; i++)
                        TravelTr[i].anchoredPosition = onepos;
                    break;
                case 2:
                    for (int i = 0; i < cnt; i++)
                        TravelTr[i].anchoredPosition = twopos[i];
                    break;
                case 3:
                    for (int i = 0; i < cnt; i++)
                        TravelTr[i].anchoredPosition = treepos[i];
                    break;
                case 4:
                    for (int i = 0; i < cnt; i++)
                        TravelTr[i].anchoredPosition = fourpos[i];
                    break;
                case 5:
                    for (int i = 0; i < cnt; i++)
                        TravelTr[i].anchoredPosition = fivepos[i];
                    break;
            }
            
            for (int i = cnt; i < Travelimages.Length; i++)
                TravelObj[i].SetActive(false);
//                Travelimages[i].sprite = emptyImage;
        }
    }
    public void SetTravelimage(int theme, int travel, int idx)
    {
        switch (theme)
        {
            case 0: 
                Travelimages[idx].sprite = Culture[travel];
                break;
            case 1:
                Travelimages[idx].sprite = Art[travel];
                break;
            case 2:
                Travelimages[idx].sprite = City[travel];
                break;
            case 3:
                Travelimages[idx].sprite = Tour[travel];
                break;
            case 4:
                Travelimages[idx].sprite = Nature[travel];
                break;
            case 5:
                Travelimages[idx].sprite = History[travel];
                break;
        }
    }
    public void CheckedTravel()
    {
        CheckedObj[TravelIndex].SetActive(true);
        TravelIndex++;
        if (TravelIndex >= imgcnt)
            TravelIndex = 0;
    }
}
