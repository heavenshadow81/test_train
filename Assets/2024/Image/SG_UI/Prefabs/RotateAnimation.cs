using DG.Tweening;
using UnityEngine;

public class RotateAnimation : MonoBehaviour
{
    // 회전 속도를 조절하기 위한 변수 (초 단위)
    public float rotationDuration = 1f;
    Sequence rotationSequence;

    void OnEnable()
    {
        // 45도까지 회전했다가 -45도까지 회전하는 애니메이션
        rotationSequence = DOTween.Sequence();

        rotationSequence.Append(transform.DORotate(new Vector3(0, 0, 45), rotationDuration).SetEase(Ease.InOutSine))
                        .Append(transform.DORotate(new Vector3(0, 0, -45), rotationDuration).SetEase(Ease.InOutSine))
                        .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDisable()
    {
        if (rotationSequence != null && rotationSequence.IsActive())
        {
            rotationSequence.Kill();
            transform.rotation = Quaternion.identity;
        }
    }
}
