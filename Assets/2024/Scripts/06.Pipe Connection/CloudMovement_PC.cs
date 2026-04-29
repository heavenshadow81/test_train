using UnityEngine;
using DG.Tweening;

public class CloudMovement_PC : MonoBehaviour
{
    private float moveDistanceY = 0.5f; // 위아래 이동할 거리
    private float moveDurationY;          // 위아래 이동 시간
    private float moveSpeedX;             // 왼쪽으로 이동할 속도
    private Ease easeType = Ease.InOutSine; // easing 타입

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position; // 원래 위치 저장

        // 랜덤한 값 설정
        moveDurationY = Random.Range(2f, 3f);  // 위아래 이동 시간
        moveSpeedX = Random.Range(10f, 12f);    // 왼쪽으로 이동 속도

        MoveCloud();
        MoveCloudHorizontally();
    }

    void MoveCloud()
    {
        // 구름의 위치를 위아래로 애니메이션
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveY(originalPosition.y + moveDistanceY, moveDurationY).SetEase(easeType))
                .Append(transform.DOMoveY(originalPosition.y - moveDistanceY, moveDurationY).SetEase(easeType))
                .SetLoops(-1, LoopType.Yoyo); // 무한 반복
    }

    void MoveCloudHorizontally()
    {
        // 구름을 왼쪽으로 계속 이동
        transform.DOMoveX(transform.position.x - 13f, moveSpeedX).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // 화면을 벗어나면 다시 오른쪽 끝으로 이동
                transform.position = new Vector3(originalPosition.x + 10f, transform.position.y, transform.position.z);
                MoveCloudHorizontally(); // 다시 왼쪽으로 이동
            });
    }
}
