using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasurePlaneController : SQPlane
{
    public Sprite treasureEmpty;    // 빈 상자 이미지
    public GameObject treaure;

    protected override void OnMouseDown()
    {
        if (!isClick)
        {
            isClick = true;
            SQManager.Instance.TreasurePlaneDownEvent();
            treaure.GetComponent<SpriteRenderer>().sprite = treasureEmpty;
        }
    }
}
