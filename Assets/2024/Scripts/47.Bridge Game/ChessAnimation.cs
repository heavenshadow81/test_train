using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessAnimation : MonoBehaviour
{
    private void Awake()
    {
        DOTween.Init();
        DOTween.defaultUpdateType = UpdateType.Fixed;
    }
    public void EndingMotion()
    {
        //닷트윈 애니메이션 멈추고 포지션X값을 1로 3초동안 이동
        gameObject.GetComponent<DOTweenAnimation>().DOPause();
        gameObject.transform.DOLocalMoveX(1f, 3f);
    }
}
