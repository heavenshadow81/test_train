using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : SQPlane
{
    public bool isTrap;

    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        if (!isClick)
        {
            if (isTrap)
            {
                manager.TrapEvent();    // 함정 이벤트
            }
            else
            {
                isClick = true;
                manager.PlaneDownEvent();
            }
        }
    }
}
