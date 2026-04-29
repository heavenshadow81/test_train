using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class TeamManager_SQ : PlayManager_PlayGround
{
    [Header("게임 설정")]
    [SerializeField] Scale_SQ[] scales = null;  
    [SerializeField] Image[] quizIimages = null;

    [Header("정답 설정")]
    [SerializeField] AnswerSlot_SQ answerSlot;
    [SerializeField] Scale_SQ selectedScale;

    [Header("점수")]
    [SerializeField] TextMeshProUGUI scoreText;
    int currentScore;

    protected override void Init()
    {
        base.Init();

        SetScales();
        SetQuiz();
    }

    private void SetGame()
    {
        answerSlot.PlayAnimation();
        SetScales();
        SetQuiz();
    }

    private void SetScales()
    {
        bool areScalesIdentical = true;

        while (areScalesIdentical)
        {
            foreach (var scale in scales)
            {
                scale.SetScale();
            }

            // 두 저울의 이미지를 비교하여 동일한지 확인
            areScalesIdentical = CheckScalesForIdenticalImages(scales);
        }
    }

    private bool CheckScalesForIdenticalImages(Scale_SQ[] scales)
    {
        // 두 저울의 이미지를 비교
        if (scales.Length < 2) return false;

        var firstScaleImages = scales[0].GetImages();
        var secondScaleImages = scales[1].GetImages();

        if (firstScaleImages.Length != secondScaleImages.Length)
        {
            return false;
        }

        for (int i = 0; i < firstScaleImages.Length; i++)
        {
            if (firstScaleImages[i].sprite != secondScaleImages[i].sprite)
            {
                return false; // 다른 이미지가 발견되면 동일하지 않음
            }
        }

        return true; // 모든 이미지가 동일하면 true 반환
    }

    private void SetQuiz()
    {
        int randomIdx = Random.Range(0, scales.Length);
        selectedScale = scales[randomIdx];

        for (int i = 0; i < quizIimages.Length; i++) 
        {
            quizIimages[i].sprite = selectedScale.GetImages()[i].sprite;
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
            Slot_SQ selectedSlot = hit.collider.gameObject.GetComponent<Slot_SQ>();
            // 터치한 카드가 어떤 태그를 가지고 있는지 확인
            if (hit.collider.CompareTag(TeamNameString))
            {
                answerSlot.StopAnimation();
                CheckAnswer(selectedSlot);
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

    void CheckAnswer(Slot_SQ selectedSlot)
    {
        selectedSlot.MoveSlot(() =>
        {
            if (selectedSlot.GetText() == selectedScale.GetAnswerString())
            {
                // 정답 처리
                CorrectAnswer(selectedSlot.gameObject);
            }
            else
            {
                // 오답 처리
                WrongAnswer(selectedSlot.gameObject);
            }
        });
    }

    public override void CorrectAnswer(GameObject touched)
    {
        StartCoroutine(CorrectRoutine(touched));
    }

    private IEnumerator CorrectRoutine(GameObject touched)
    {
        SoundMGR.Instance.SoundPlay("PlayGround_Correct");
        foreach (var scale in scales)
        {
            scale.ShakeBar();
        }
        SetScore();  // 점수 변경
        answerSlot.SetTextAnswer(); // 정답 슬롯 텍스트 변경 및 이펙트 생성
        CheckVictory();

        yield return new WaitForSeconds(1f);

        answerSlot.SetTextOrigin();  // 정답 슬롯 텍스트 변경 및 이펙트 제거
        Slot_SQ selectedSlot = touched.GetComponent<Slot_SQ>();

        // ReturnMoveSlot 호출 시 콜백 전달
        selectedSlot.ReturnMoveSlot(() =>
        {
            isTouchable = true;
            SetGame();
        });
    }

    public override void WrongAnswer(GameObject touched)
    {
        SoundMGR.Instance.SoundPlay("PlayGround_WrongAnswer");
        foreach (var scale in scales)
        {
            scale.ShakeBar(scale.GetBarZRotation());
        }
        answerSlot.ActiveWrongEffect();  // 오답 이펙트 생성

        Slot_SQ selectedSlot = touched.GetComponent<Slot_SQ>();
        selectedSlot.ReturnMoveSlot(); // 슬롯 되돌아가기
    }

    void SetScore()
    {
        // scoreText의 텍스트를 정수로 변환
        currentScore = int.Parse(scoreText.text);

        // 현재 점수에서 새로운 점수를 뺌
        currentScore --;

        // 결과를 다시 텍스트로 변환하여 scoreText에 표시
        scoreText.text = currentScore.ToString();
    }

    public void SetEnableTouch()
    {
        isTouchable = true;
    }

    protected override void CheckVictory()
    {
        if(currentScore <= 0)
        {
            touchAction.Disable();
            SoundMGR.Instance.SoundPlay("PlayGround_Victory");
            gameCanvas.SetActive(false);
            victoryUI.SetActive(true);
        }
    }
}
