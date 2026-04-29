using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;
using TMPro;
using DG.Tweening;


public class MAManager : PlayManager_PlayGround
{
    [Header("게임 설정")]
    int firstNumber;
    int secondNumber;
    int answer;
    [SerializeField] Number_MA[] selectSlots;
    [SerializeField] Number_MA answerSlot;

    [Header("힌트")]
    [SerializeField] TextMeshProUGUI hintText;

    [Header("점수")]
    [SerializeField] TextMeshProUGUI scoreText;
    int score = 0;

    protected override void Init()
    {
        base.Init();

        SetQuestion();
    }

    void SetQuestion()
    {
        answerSlot.rotateAnimation.enabled = true;
        isPerfect = true;
        firstNumber = Random.Range(0, 26);
        secondNumber = Random.Range(0, 26);

        // 0 또는 1을 무작위로 생성
        int operation = Random.Range(0, 2);

        // operation이 0이면 덧셈, 1이면 뺄셈
        if (operation == 0)
        {
            hintText.text = $"{firstNumber} + {secondNumber} = ";
            AddNumber(firstNumber, secondNumber);
        }
        else
        {
            // firstNumber가 secondNumber보다 작으면 값을 교환
            if (firstNumber < secondNumber)
            {
                int temp = firstNumber;
                firstNumber = secondNumber;
                secondNumber = temp;
            }

            hintText.text = $"{firstNumber} - {secondNumber} = ";
            SubtractNumber(firstNumber, secondNumber);
        }

        SetSlots();
    }

    void SetSlots()
    {
        answerSlot.Init();

        // 정답 슬롯의 인덱스를 무작위로 선택
        int correctSlotIndex = Random.Range(0, selectSlots.Length);

        // 사용된 숫자를 추적하는 HashSet
        HashSet<int> usedNumbers = new HashSet<int>();
        usedNumbers.Add(answer); // 정답 숫자는 미리 추가

        for (int i = 0; i < selectSlots.Length; i++)
        {
            // 카드의 알파 값과 스케일을 설정하고 회전
            selectSlots[i].SetCardAlphaAndScale();
            selectSlots[i].TurnCard();

            if (i == correctSlotIndex)
            {
                // 정답 슬롯에 정답 숫자를 설정
                selectSlots[i].SetNumberText(answer);
            }
            else
            {
                // 다른 슬롯들에 랜덤 숫자를 설정
                int number;
                do
                {
                    number = Random.Range(0, 50);
                } while (usedNumbers.Contains(number)); // 숫자가 겹치지 않도록

                usedNumbers.Add(number); // 사용된 숫자에 추가
                selectSlots[i].SetNumberText(number);
            }
        }

        selectSlots[3].turnSequence.OnComplete(() =>
        {
            isTouchable = true;
        });
    }


    int AddNumber(int a, int b)
    {
        return answer = a + b;
    }

    int SubtractNumber(int a, int b)
    {
        return answer = a - b;
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
                Number_MA selectedSlot = hit.collider.GetComponent<Number_MA>();
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

    void CheckAnswer(Number_MA selectedSlot)
    {
        answerSlot.rotateAnimation.enabled = false;

        if (answer == selectedSlot.GetNumber())
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
        // 정답 슬롯의 위치를 타겟 위치로 설정
        Vector3 targetPosition = answerSlot.transform.position;

        // 현재 위치를 저장
        Vector3 originalPosition = touched.transform.position;

        // 타겟 위치로 이동
        touched.transform.DOMove(targetPosition, 0.3f).OnComplete(() =>
        {
            SoundMGR.Instance.SoundPlay("PlayGround_Moving");
            SetScore();

            // 이동 완료 후 이펙트 생성
            GameObject particle = Instantiate(effect, targetPosition, Quaternion.identity);
            Destroy(particle, 1f); // 1초 후 이펙트 제거

            // 이동 완료 후 정답 슬롯에 선택된 글자를 설정
            answerSlot.SetNumberText(answer);

            // 원래 위치로 돌아가기
            touched.transform.DOMove(originalPosition, 0f).OnComplete(() =>
            {

                // 터치 비활성화
                isTouchable = false;

                // 남은 선택 카드의 알파 값을 0으로 설정
                foreach (var selectSlot in selectSlots)
                {
                    selectSlot.SetCardAlphaAndScaleZero();
                }

                if (isPerfect)
                {
                    SoundMGR.Instance.SoundPlay("PlayGround_Perfect");
                    GameObject perfectParticle = Instantiate(perfectEffect, perfectEffectPos);
                    Destroy(perfectParticle, 2f);
                }
                else
                {
                    SoundMGR.Instance.SoundPlay("PlayGround_Correct");
                }

                Invoke("SetQuestion", 2f);

                // 터치 활성화
                isTouchable = true;

            });
        });
    }

    public override void WrongAnswer(GameObject touched)
    {
        isPerfect = false;

        SoundMGR.Instance.SoundPlay("PlayGround_WrongAnswer");

        // 정답 슬롯의 위치를 타겟 위치로 설정
        Vector3 targetPosition = answerSlot.transform.position;

        // 현재 위치를 저장
        Vector3 originalPosition = touched.transform.position;

        // 타겟 위치로 이동
        touched.transform.DOMove(targetPosition, 0.3f).OnComplete(() =>
        {
            // 원래 위치로 돌아가기
            touched.transform.DOMove(originalPosition, 0.5f);

            isTouchable = true;
            answerSlot.rotateAnimation.enabled = true;
        });
    }

    void SetScore()
    {
        score++;

        scoreText.text = score.ToString();
    }

    public void DiableInput()
    {
        touchAction.Disable();
    }
}
