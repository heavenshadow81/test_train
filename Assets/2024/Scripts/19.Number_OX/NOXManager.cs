using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;
using DG.Tweening;

public class NOXManager : PlayManager_PlayGround
{
    [Header("게임 설정")]
    [SerializeField] CenterBorder_NOX centerBord;
    [SerializeField] TextMeshProUGUI hintText;
    int includedIdx;

    [Header("점수")]
    [SerializeField] GameObject score_Plus;
    [SerializeField] GameObject score_Minus;
    [SerializeField] TextMeshProUGUI scoreText;
    int score = 0;

    [Header("배경 이펙트")]
    [SerializeField] GameObject backEffect;

    protected override void Init()
    {
        base.Init();

        backEffect.SetActive(true);
    }

    void CheckAnswer_O(GameObject selectedSlot)
    {
        isTouchable = false;

        int textInt = int.Parse(hintText.text);
        includedIdx = centerBord.currentCenterNumbers.IndexOf(textInt); // 인덱스 값을 저장(없으면 -1 반환)

        if (includedIdx != -1) // 만약 포함되어 있다면 (IndexOf가 -1이 아니라면)
        {
            CorrectAnswer(selectedSlot);
            GameObject particle2 = Instantiate(effect, centerBord.texts[includedIdx].transform.position, Quaternion.identity);
            Destroy(particle2, 1f); // 1초 후 이펙트 제거
        }
        else
        {
            WrongAnswer(selectedSlot);
        }
    }

    void CheckAnswer_X(GameObject selectedSlot)
    {
        int textInt = int.Parse(hintText.text);
        includedIdx = centerBord.currentCenterNumbers.IndexOf(textInt); // 인덱스 값을 저장(없으면 -1 반환)

        if (includedIdx == -1) // 만약 포함되어 있다면 (IndexOf가 -1이라면)
        {
            CorrectAnswer(selectedSlot);
        }
        else
        {
            WrongAnswer(selectedSlot);
        }
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
                if (hit.collider.gameObject.name.Contains("O"))
                {
                    CheckAnswer_O(hit.collider.gameObject);
                }
                else
                {
                    CheckAnswer_X(hit.collider.gameObject);
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

    public override void CorrectAnswer(GameObject touched)
    {
        SoundMGR.Instance.SoundPlay("PlayGround_Correct");
        SetScore(1);

        ChangeHintText(true);
        centerBord.SetText(includedIdx); // 정답 처리 로직 호출

        // 클릭된 오브젝트의 스케일 애니메이션 추가
        AnimateCorrectTouchedObject(touched);

        // 이펙트 생성
        GameObject particle = Instantiate(effect, touched.transform.position, Quaternion.identity);
        Destroy(particle, 1f); // 1초 후 이펙트 제거

        // 점수 이펙트 생성
        GameObject plusEfffect = Instantiate(score_Plus, touched.transform);
        Destroy(plusEfffect, 1f); // 1초 후 이펙트 제거

        // 이펙트 파괴 후 HandleSlotMove 호출
        StartCoroutine(HandleSlotMoveAfterDelay(1f, false)); // 1초 후 호출
    }


    public override void WrongAnswer(GameObject touched)
    {
        SoundMGR.Instance.SoundPlay("PlayGround_WrongAnswer");
        SetScore(-1);

        // 클릭된 오브젝트의 스케일 애니메이션 추가
        AnimateWrongTouchedObject(touched);

        // 센터보드 숫자 바꾸기
        int randomIdx = centerBord.GetRandomIndex();
        centerBord.SetText(randomIdx); // 정답 처리 로직 호출

        // 점수 이펙트
        GameObject minusEfffect = Instantiate(score_Minus, touched.transform);
        Destroy(minusEfffect, 1f);

        // 이펙트 파괴 후 HandleSlotMove 호출
        StartCoroutine(HandleSlotMoveAfterDelay(1f, true)); // 1초 후 호출
    }
    private IEnumerator HandleSlotMoveAfterDelay(float delay, bool shouldChange)
    {
        yield return new WaitForSeconds(delay); // 지정된 시간만큼 대기
        ChangeHintText(shouldChange);   
        isTouchable = true;                // 터치 가능하게 플래그 변경
    }

    void SetScore(int addScore)
    {
        score += addScore;

        scoreText.text = score.ToString();
    }

    private void AnimateCorrectTouchedObject(GameObject touched)
    {
        // DOTween 시퀀스를 사용하여 애니메이션 순차적으로 실행
        Sequence sequence = DOTween.Sequence();

        // 스케일 변경 애니메이션
        sequence.Append(touched.transform.DOScale(2.0f, 0.2f));

        // 스케일 복구 애니메이션
        sequence.Append(touched.transform.DOScale(1.0f, 0.2f));

        // 시퀀스 실행
        sequence.Play();
    }

    private void AnimateWrongTouchedObject(GameObject touched)
    {
        Image image = touched.GetComponent<Image>(); 
        Color originColor = image.color;
        
        // DOTween 시퀀스를 사용하여 애니메이션 순차적으로 실행
        Sequence sequence = DOTween.Sequence();

        // 흔들고 컬러 변경
        sequence.Append(touched.transform.DOShakePosition(0.4f, 100));
        sequence.Join(hintText.transform.DOShakePosition(0.4f, 100));
        sequence.Join(image.DOColor(Color.red, 0.4f));
        sequence.Append(image.DOColor(originColor, 0.4f));

        // 시퀀스 실행
        sequence.Play();
    }

    void ChangeHintText(bool shouldChange)
    {
        if(shouldChange)        // 변경 가능할때만
        {
            int randomNumber = Random.Range(0, 10);

            hintText.text = randomNumber.ToString();
        }
    }
}
