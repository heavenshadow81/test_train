using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;
using TMPro;
using DG.Tweening;

public class HNSManager : PlayManager_PlayGround
{
    [Header("게임 설정")]
    [SerializeField] Spawner_HNS spawner; // Spawner 클래스 참조

    [Header("점수")]
    [SerializeField] GameObject score_Plus;
    [SerializeField] GameObject score_Minus;
    [SerializeField] TextMeshProUGUI scoreText;
    int score = 0;

    protected override void Init()
    {
        base.Init();

        spawner.InitializePool(); // 오브젝트 풀 초기화
        spawner.GenerateRows(); // 줄 생성 시작
    }

    public override void HandleInput(Vector2 inputPosition)
    {
        // 터치/마우스 위치를 월드 좌표로 변환
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

        // 터치/마우스 위치에서 카드 찾기
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null)
        {
            // 터치한 카드가 어떤 태그를 가지고 있는지 확인
            if (hit.collider.CompareTag(TeamNameString))
            {            
                Slot_HNS selectedSlot = hit.collider.GetComponent<Slot_HNS>();
                if (selectedSlot != null)
                {
                    CheckAnswer(selectedSlot);
                }
            }
            else
            {
                // 터치한 오브젝트가 적합하지 않으면 즉시 다시 터치 가능
                isTouchable = true;
            }
        }
        else
        {
            // 아무것도 맞지 않았을 경우 다시 터치 가능
            isTouchable = true;
        }
    }

    void CheckAnswer(Slot_HNS selectedSlot)
    {
        RowSlot_HNS rowSlot = spawner.rows[0].GetComponent<RowSlot_HNS>();

        if (rowSlot.GetCorrectText() == selectedSlot.GetText())
        {
            CorrectAnswer(selectedSlot.gameObject);
        }
        else
        {
            WrongAnswer(selectedSlot.gameObject);
        }
    }

    public override void CorrectAnswer(GameObject touched)
    {
        SoundMGR.Instance.SoundPlay("PlayGround_Correct");
        SetScore(1);

        // 클릭된 오브젝트의 스케일 애니메이션 추가
        AnimateTouchedObject(touched);

        // 이동 완료 후 이펙트 생성
        GameObject particle = Instantiate(effect, touched.transform.position, Quaternion.identity);
        Destroy(particle, 1f); // 1초 후 이펙트 제거

        GameObject plusEfffect = Instantiate(score_Plus, touched.transform);
        Destroy(plusEfffect, 1f); // 1초 후 이펙트 제거

        // 이펙트 파괴 후 HandleSlotMove 호출
        StartCoroutine(HandleSlotMoveAfterDelay(1f)); // 1초 후 호출
    }

    public override void WrongAnswer(GameObject touched)
    {
        SoundMGR.Instance.SoundPlay("PlayGround_WrongAnswer");
        SetScore(-1);

        // 클릭된 오브젝트의 스케일 애니메이션 추가
        AnimateTouchedObject(touched);

        GameObject minusEfffect = Instantiate(score_Minus, touched.transform);
        Destroy(minusEfffect, 1f);

        // 이펙트 파괴 후 HandleSlotMove 호출
        StartCoroutine(HandleSlotMoveAfterDelay(1f)); // 1초 후 호출
    }

    private IEnumerator HandleSlotMoveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 지정된 시간만큼 대기
        spawner.HandleSlotMove(); // 정답 처리 로직 호출
    }

    void SetScore(int addScore)
    {
        score += addScore;

        scoreText.text = score.ToString();
    }

    private void AnimateTouchedObject(GameObject touched)
    {
        // DOTween 시퀀스를 사용하여 애니메이션 순차적으로 실행
        Sequence sequence = DOTween.Sequence();

        // 작아지는 애니메이션
        sequence.Append(touched.transform.DOScale(0.8f, 0.2f));

        // 다시 커지는 애니메이션
        sequence.Append(touched.transform.DOScale(1.0f, 0.2f));

        // 시퀀스 실행
        sequence.Play();
    }

    public void SetTouchabled()
    {
        isTouchable = true;
    }
}
