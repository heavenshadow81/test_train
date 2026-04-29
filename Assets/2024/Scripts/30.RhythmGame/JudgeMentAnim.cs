using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DoTween 사용을 위해 필요


namespace RhythmGame
{
    public class JudgeMentAnim : MonoBehaviour
    {
        private Tween scaleTween;

        private void OnEnable()
        {
            // 오브젝트가 활성화될 때 애니메이션을 새로 시작
            AnimateScale();
        }

        private void OnDisable()
        {
            // 오브젝트가 비활성화될 때 기존 애니메이션을 중지
            if (scaleTween != null && scaleTween.IsActive())
            {
                scaleTween.Kill(); // 기존 트윈 중지
            }
        }

        private void AnimateScale()
        {
            // 스케일을 0으로 설정하고 애니메이션 시작
            transform.localScale = Vector3.zero;

            // 스케일을 0.4초 동안 100으로 커졌다가 다시 0으로 줄어드는 애니메이션
            scaleTween = transform.DOScale(120f, 0.4f).OnComplete(() =>
            {
                // 다시 0으로 스케일 줄이기
                transform.DOScale(0f, 0f);
                gameObject.SetActive(false);
            });
        }
    }
}

