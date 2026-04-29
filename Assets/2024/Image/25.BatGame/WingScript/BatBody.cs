using DG.Tweening;
using UnityEngine;

public class BatBody : MonoBehaviour
{
    public float tiltAngle = 40f; // 까딱거릴 각도
    public float tiltSpeed = 0.07f; // 속도 조절 변수
    public int shakeRepeats = 5;

    public void ShakeBody()
    {
        // 까딱거리기 애니메이션
        Sequence shakeSequence = DOTween.Sequence();

        // 다섯 번 반복
        for (int i = 0; i < shakeRepeats; i++)
        {
            shakeSequence.Append(transform.DORotate(new Vector3(tiltAngle, 0, 0), tiltSpeed).SetEase(Ease.OutQuad))
                        .Append(transform.DORotate(new Vector3(-tiltAngle, 0, 0), tiltSpeed).SetEase(Ease.OutQuad))
                        .Append(transform.DORotate(Vector3.zero, tiltSpeed).SetEase(Ease.OutQuad)); // 원래 위치로 돌아옴
        }

        // 애니메이션을 순차적으로 실행
        shakeSequence.Play();
    }
}