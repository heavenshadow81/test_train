using DG.Tweening;
using UnityEngine;

public class LeftBatWings : MonoBehaviour
{
    public float flapSpeed = 0.5f; // 속도 조절 변수

    public void FlapWings()
    {
        // 날개가 푸드덕거릴 각도
        float flapAngle = -90f;

        // 푸드덕거리기 애니메이션
        Sequence flapSequence = DOTween.Sequence();

        // 다섯 번 반복
        for (int i = 0; i < 6; i++)
        {
            flapSequence.Append(transform.DORotate(new Vector3(0, flapAngle, 0), flapSpeed).SetEase(Ease.OutQuad))
                        .Append(transform.DORotate(new Vector3(0, 0, 0), flapSpeed).SetEase(Ease.OutQuad)) // 원래 위치로 돌아옴
                        .Append(transform.DORotate(new Vector3(0, -flapAngle, 0), flapSpeed).SetEase(Ease.OutQuad))
                        .Append(transform.DORotate(new Vector3(0, 0, 0), flapSpeed).SetEase(Ease.OutQuad)); // 원래 위치로 돌아옴
        }

        // 애니메이션을 순차적으로 실행
        flapSequence.Play();
    }
}