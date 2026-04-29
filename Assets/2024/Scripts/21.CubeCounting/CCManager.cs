using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CCManager : PlayManager_PlayGround
{
    [Header("게임 설정")]
    [SerializeField] Slot_CC[] slots = null;
    [SerializeField] CubeManager_CC cubeManager;
    int activeCubeCount;         // 정답 개수

    [Header("UI Raycast")]
    public GraphicRaycaster raycaster; // Canvas에 부착된 GraphicRaycaster
    public EventSystem eventSystem;    // UI 이벤트 시스템

    [Header("타이머")]
    [SerializeField] CountDownTimer_CC timer;
    [SerializeField] RectTransform targetTransform;

    [SerializeField] CubeRotation_CC CubeRotation;

    protected override void Init()
    {
        base.Init();

        SetGame(true);
    }

    void SetGame(bool shouldChange)
    {
        if(shouldChange)
        {        
            StartCoroutine(SetGameRoutine());
        }       
    }

    IEnumerator SetGameRoutine()
    {
        cubeManager.Init();

        yield return new WaitForSeconds(1f);

        SetHint();
        isTouchable = true;
        CubeRotation.SetButtonsEnable(true);
    }
    void SetHint()
    {
        activeCubeCount = cubeManager.GetActiveCubeCount(); // 활성화된 큐브의 개수 가져오기
        List<int> hintNumbers = new List<int>(); // 힌트 숫자들을 저장할 리스트

        // 1. 먼저, 하나의 슬롯에 activeCubeCount를 설정합니다.
        int correctSlotIndex = Random.Range(0, slots.Length);
        hintNumbers.Add(activeCubeCount); // 정답 숫자를 리스트에 추가
        slots[correctSlotIndex].SetHintText(activeCubeCount.ToString()); // 정답 숫자를 하나의 슬롯에 설정

        // 2. 나머지 슬롯들에 대해 랜덤 숫자를 생성하여 설정합니다.
        for (int i = 0; i < slots.Length; i++)
        {
            if (i == correctSlotIndex) continue; // 이미 정답 숫자를 설정한 슬롯은 건너뜁니다.

            int minRange = Mathf.Max(3, activeCubeCount - 7); // 최소값을 0으로 제한
            //int maxRange = Mathf.Min(27, activeCubeCount + 8);                                                
            int randomHint = Random.Range(minRange, activeCubeCount + 8);

            // 중복된 숫자가 생성되지 않도록 하기 위해 체크
            while (hintNumbers.Contains(randomHint))
            {
                randomHint = Random.Range(minRange, activeCubeCount + 8);
            }

            hintNumbers.Add(randomHint); // 리스트에 숫자 추가
            slots[i].SetHintText(randomHint.ToString()); // 각 슬롯에 텍스트 설정
        }
    }

    void ClearHint()
    {
        foreach(Slot_CC slot in slots)
        {
            slot.SetHintText("");
        }
    }

    public override void HandleInput(Vector2 inputPosition)
    {
        // PointerEventData를 통해 입력 위치 설정
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = inputPosition;

        // Raycast 결과를 저장할 리스트 생성
        List<RaycastResult> results = new List<RaycastResult>();

        // GraphicRaycaster로 UI 요소에 대한 Raycast 실행
        raycaster.Raycast(pointerData, results);

        if (results.Count > 0)
        {
            foreach (RaycastResult result in results)
            {
                // 특정 태그를 가진 UI 요소인지 확인
                if (result.gameObject.CompareTag(TeamNameString))
                {
                    Slot_CC selectedSlot = result.gameObject.GetComponent<Slot_CC>();
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
        }
        else
        {
            // 아무것도 맞지 않았을 경우 다시 터치 가능
            isTouchable = true;
        }
    }

    void CheckAnswer(Slot_CC selectedSlot)
    {
        int slotValue;

        if (int.TryParse(selectedSlot.GetHintText(), out slotValue))
        {
            if (slotValue == activeCubeCount)
            {
                // 정답 처리
                CorrectAnswer(selectedSlot.gameObject);
            }
            else
            {
                // 오답 처리
                WrongAnswer(selectedSlot.gameObject);
            }
        }
    }

    public override void CorrectAnswer(GameObject touched)
    {
        CubeRotation.SetButtonsEnable(false);
        SoundMGR.Instance.SoundPlay("PlayGround_Correct");  
        timer.AddTime(10);
        ChangeGauge();
        ClearHint();

        // 클릭된 오브젝트의 스케일 애니메이션 추가
        AnimateCorrectTouchedObject(touched);


        // 이동 완료 후 이펙트 생성
        GameObject particle = Instantiate(effect, touched.transform.position, Quaternion.identity);
        Destroy(particle, 1f); // 1초 후 이펙트 제거

        // 점수 이펙트 생성
        //TextMeshProUGUI txt = timer_Plus.GetComponent<TextMeshProUGUI>();
        //txt.text = "+10";
        //txt.color = Color.blue;
        //GameObject plusEfffect = Instantiate(timer_Plus, touched.transform);
        //Destroy(plusEfffect, 1f); // 1초 후 이펙트 제거

        SpawnTimerText(touched, "+10", Color.green);

        // 이펙트 파괴 후 HandleSlotMove 호출
        StartCoroutine(HandleSlotMoveAfterDelay(1f, true)); // 1초 후 호출      
    }

    public override void WrongAnswer(GameObject touched)
    {
        SoundMGR.Instance.SoundPlay("PlayGround_WrongAnswer");

        // 클릭된 오브젝트의 스케일 애니메이션 추가
        AnimateWrongTouchedObject(touched);

        // 점수 이펙트
        //TextMeshProUGUI txt = timer_Minus.GetComponent<TextMeshProUGUI>();
        //txt.text = "-5";
        //GameObject minusEfffect = Instantiate(timer_Minus, touched.transform);
        //Destroy(minusEfffect, 1f);

        SpawnTimerText(touched, "-5", Color.red);

        timer.SubtractTime(5);

        // 이펙트 파괴 후 HandleSlotMove 호출
        StartCoroutine(HandleSlotMoveAfterDelay(1f, false)); // 1초 후 호출
    }

    private IEnumerator HandleSlotMoveAfterDelay(float delay, bool shouldChange)
    {
        yield return new WaitForSeconds(delay); // 지정된 시간만큼 대기
        SetGame(shouldChange);
        isTouchable = true;
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
        //sequence.Join(hintText.transform.DOShakePosition(0.4f, 100));
        sequence.Join(image.DOColor(Color.red, 0.4f));
        sequence.Append(image.DOColor(originColor, 0.4f));

        // 시퀀스 실행
        sequence.Play();
    }

    protected override void CheckVictory()
    {
        base.CheckVictory();

        if (stack >= maxStack)
        {
            timer.PauseTimer();
        }
    }

    void SpawnTimerText(GameObject slot, string text, Color textColor)
    {
        float duration = 1f;

        Slot_CC slotcc = slot.GetComponent<Slot_CC>();

        // 원래 위치, 색상 및 폰트 크기를 저장합니다.
        RectTransform timerTextRectTransform = slotcc.GetTimerTextComponent().GetComponent<RectTransform>();
        Vector2 originalPosition = timerTextRectTransform.anchoredPosition;
        Color originalColor = slotcc.GetTimerTextComponent().color;
        float originalFontSize = slotcc.GetTimerTextComponent().fontSize;

        // 텍스트 설정
        slotcc.SetTimerText(text);

        // 텍스트 색상 설정
        slotcc.GetTimerTextComponent().color = textColor;

        // 애니메이션 시퀀스 생성
        Sequence sequence = DOTween.Sequence();

        // 폰트 크기 변경 애니메이션 추가
        sequence.Append(slotcc.GetTimerTextComponent().DOFontSize(40, duration));

        // 위치 이동 애니메이션 추가
        sequence.Join(timerTextRectTransform.DOMove(targetTransform.position, duration).SetEase(Ease.OutBounce));

        // 애니메이션 완료 후 텍스트, 색상, 위치 복원
        sequence.OnComplete(() =>
        {
            slotcc.SetTimerText("");  // 텍스트 지우기
            slotcc.GetTimerTextComponent().color = originalColor;  // 색상 복원
            slotcc.GetTimerTextComponent().fontSize = originalFontSize;  // 폰트 크기 복원
            timerTextRectTransform.anchoredPosition = originalPosition;  // 위치 복원
        });

        // 시퀀스 실행
        sequence.Play();
    }
}
