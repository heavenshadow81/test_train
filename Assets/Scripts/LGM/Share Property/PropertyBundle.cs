using System;
using UnityEngine;

[Serializable]
public class CraneScreen
{
    [HideInInspector]
    public Vector2 tR;
    [HideInInspector]
    public Vector2 bL;
    public Transform panel;
    public Vector2 trLimit;
    public Vector2 blLimit;

    public void ScreenInit()
    {
        // 플레이 화면의 오른쪽, 위 / 왼쪽, 아래의 좌표 구하기
        tR.x = (panel.position.x + panel.localScale.x / 2) - trLimit.x;
        tR.y = (panel.position.y + panel.localScale.y / 2) - trLimit.y;
        bL.x = (panel.position.x - panel.localScale.x / 2) + blLimit.x;
        bL.y = (panel.position.y - panel.localScale.y / 2) + blLimit.y;
    }
}