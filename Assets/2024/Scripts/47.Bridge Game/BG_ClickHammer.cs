using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BG_ClickHammer : TouchManager_3DTouch
{
    [SerializeField] HammerMove hammerMove; //해머움직임 스크립트
    public static bool touchOn = false; //터치여부 값

    public override void HandleInput(Vector2 pos)
    {
        isTouchable = true;

        if(touchOn)
        {
            //망치 어택 실행 및 터치잠금
            hammerMove.Attack();
            touchOn = false;

            SoundMGR.Instance.SoundPlay("화살");
        }
    }
}
