using UnityEngine;
using DG.Tweening;

public class VictoryMoving : MonoBehaviour
{
    public DOTweenAnimation object1;
    public DOTweenAnimation object2;
    public DOTweenAnimation object3;
    public DOTweenAnimation object4;

    void Start()
    {
        // 2번, 3번, 4번 오브젝트 비활성화
        object2.gameObject.SetActive(false);
        object3.gameObject.SetActive(false);
        object4.gameObject.SetActive(false);

        // Sequence 생성
        Sequence mySequence = DOTween.Sequence();

        // 첫 번째 오브젝트의 애니메이션을 시퀀스에 추가
        mySequence.Append(object1.tween);

        // 첫 번째 애니메이션이 완료된 후 2번, 3번, 4번 오브젝트를 활성화하고 애니메이션 실행
        mySequence.AppendCallback(() =>
        {
            // 2번, 3번, 4번 오브젝트 활성화
            object2.gameObject.SetActive(true);
            object3.gameObject.SetActive(true);
            object4.gameObject.SetActive(true);

            // 2번 오브젝트의 애니메이션 1초 후 실행
            DOVirtual.DelayedCall(1f, () => object2.DOPlay());

            // 3번, 4번 오브젝트의 애니메이션 즉시 실행
            object3.DOPlay();
            object4.DOPlay();
        });

        // 2번, 3번, 4번 애니메이션의 길이만큼 딜레이 추가
        mySequence.AppendInterval(Mathf.Max(object2.tween.Duration() + 1f, Mathf.Max(object3.tween.Duration(), object4.tween.Duration())));

        // 시퀀스 시작
        mySequence.Play();
    }
}
