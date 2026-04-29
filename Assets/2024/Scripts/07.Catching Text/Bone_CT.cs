using UnityEngine;
using DG.Tweening;

public class Bone_CT : MonoBehaviour
{
    private float moveDuration;        // 이동 시간

    public RectTransform optionsPos; // 범위를 설정할 RectTransform
    private float phaseOffset; // 사인 곡선의 위상 차이

    private float minX, maxX; // X축 범위

    Sequence moveSequence;

    public void Init()
    {
        // 랜덤한 값 설정
        moveDuration = Random.Range(5f, 13f);
        phaseOffset = Random.Range(0f, 3 * Mathf.PI); // 사인 곡선의 위상 차이 설정

        // X축 범위 설정
        minX = -optionsPos.sizeDelta.x / 2;
        maxX = optionsPos.sizeDelta.x / 2;

        // 애니메이션 시작
        MoveBone();
    }

    public void MoveBone()
    {
        // optionsPos의 범위 내에서 Y 위치 설정
        float minY = -optionsPos.sizeDelta.y / 2;
        float maxY = optionsPos.sizeDelta.y / 2;

        // 뼈의 위치를 위아래로 사인 곡선 형태로 애니메이션
        moveSequence = DOTween.Sequence();
        float time = 0f; // 시간 초기화

        // 애니메이션 루프
        moveSequence.AppendCallback(() =>
        {
            // 사인 곡선에 따라 Y 위치 계산
            float y = Mathf.Lerp(minY, maxY, (Mathf.Sin(time + phaseOffset) + 1) / 2);
            GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x, y);

            // X 위치를 사인 곡선 형태로 업데이트
            float x = Mathf.Lerp(minX, maxX, (Mathf.Sin(time + phaseOffset * 2) + 1) / 2); // 위상 차이를 두 배로 해서 다르게 설정
            GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

            time += Time.deltaTime / 10; // 시간 증가
        }).SetLoops(-1); // 무한 반복
    }

    public void StopBone()
    {
        // 이동 애니메이션을 멈춤
        if (moveSequence != null)
        {
            moveSequence.Kill(); // 시퀀스 종료
            moveSequence = null; // 시퀀스 참조를 null로 설정
        }
    }
}
