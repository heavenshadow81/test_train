using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 숫자를 UI로 출력하는 클래스
/// </summary>
public class NumericsDisplayer : MonoBehaviour
{
    public UISprite[] numArray;
    public string fileName;

    public bool Active
    {
        get { return gameObject.activeSelf; }
        set { gameObject.SetActive(value); }
    }

    void Awake()
    {
        ChangePoint(this, 0);
    }

    public static void ChangePoint(NumericsDisplayer ui, int point)
    {
        if (point < 0) return;
        if (ui != null)
            ui.ChangeNumerics(NumericSplit.Split(point));
    }

    public void ChangeNumerics(List<int> _pointList)
    {
        if (_pointList == null || _pointList.Count == 0)
        {
            numArray[0].spriteName = fileName + "0";
            for (int i = 1; i < numArray.Length; ++i)
                numArray[i].cachedGameObject.SetActive(false);
            return;
        }

        for (int i = 0; i < numArray.Length; ++i)
        {
            if (_pointList.Count > i)
            {
                if (!numArray[i].cachedGameObject.activeInHierarchy)
                    numArray[i].cachedGameObject.SetActive(true);

                numArray[i].spriteName = fileName + _pointList[i].ToString();
            }
            else
            {
                if (numArray[i].cachedGameObject.activeInHierarchy)
                    numArray[i].cachedGameObject.SetActive(false);
            }
        }
        _pointList.Clear();
        _pointList = null;
    }
}