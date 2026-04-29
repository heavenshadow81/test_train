using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;
using DG.Tweening;

public class ACMManager : PlayManager_PlayGround
{
    [Header("게임 설정")]
    public List<Sprite> animals;
    private Sprite[] answers;
    private Sprite[] selectors;
    [SerializeField] Card_ACM[] answerSlots; // 정답 셀
    [SerializeField] Card_ACM[] selectSlots; // 선택 셀 
    private int currentAnswerIndex = 0; // 현재 검사 중인 정답 인덱스

    protected override void Init()
    {
        base.Init();

        CardInitialize();
    }

    void CardInitialize()
    {
        isTouchable = false;
        isPerfect = true;
        currentAnswerIndex = 0;
        StartCoroutine(InitializeCards());
    }

    IEnumerator InitializeCards()
    {
        SelectRandomAnswers();

        // answerSlots의 카드를 순서대로 앞면을 보여주고 다시 뒷면으로 돌리기
        for (int i = 0; i < answerSlots.Length; i++)
        {
            yield return StartCoroutine(ShowHint(answerSlots[i], answers[i]));
        }

        // answerSlots 작업 완료 후, selectSlots를 보이고 돌아가도록 설정
        foreach (var selectSlot in selectSlots)
        {
            selectSlot.SetCardAlphaAndScale();
            selectSlot.TurnCard();
        }
    }

    IEnumerator ShowHint(Card_ACM hintCard, Sprite answer)
    {
        isTouchable = false;
        hintCard.TurnCard();
        yield return new WaitForSeconds(0.1f); // 앞면을 보여주기 위해 잠시 대기
        hintCard.SetCardForward(answer);
        yield return new WaitForSeconds(0.5f); // 앞면을 충분히 보여주기 위해 잠시 대기
        hintCard.TurnCard();
        yield return new WaitForSeconds(0.1f); // 뒷면을 보여주기 위해 잠시 대기
        hintCard.SetCardBack();
        yield return new WaitForSeconds(0.3f);
        isTouchable = true;
    }

    private void SelectRandomAnswers()
    {
        // 랜덤 숫자 생성기 생성
        System.Random random = new System.Random();

        // answers 배열 초기화
        answers = new Sprite[3];

        List<Sprite> tempAnimals = new List<Sprite>(animals);

        for (int i = 0; i < 3; i++)
        {
            // animals 리스트에서 랜덤 인덱스 선택
            int randomIndex = random.Next(tempAnimals.Count);

            // 선택된 스프라이트를 answers 배열에 추가
            answers[i] = tempAnimals[randomIndex];

            // 선택된 스프라이트를 tempAnimals 리스트에서 제거
            tempAnimals.RemoveAt(randomIndex);
        }

        SetSelectors();
    }

    void SetSelectors()
    {
        // selectors 배열 초기화
        selectors = new Sprite[4];

        // answers 배열의 3개 스프라이트를 selectors 배열에 추가
        for (int i = 0; i < answers.Length; i++)
        {
            selectors[i] = answers[i];
        }

        // 겹치지 않는 animals에서 랜덤으로 하나의 스프라이트 선택하여 selectors 배열에 추가
        System.Random random = new System.Random();
        Sprite randomSprite;
        do
        {
            int randomIndex = random.Next(animals.Count);
            randomSprite = animals[randomIndex];
        } while (answers.Contains(randomSprite));
        selectors[selectors.Length - 1] = randomSprite;

        SetSlots();
    }

    void SetSlots()
    {
        // 카드 뒷면 설정
        foreach (Card_ACM answerSlot in answerSlots)
        {
            answerSlot.SetCardBack();
        }

        // selectors 배열 섞기
        System.Random random = new System.Random();
        selectors = selectors.OrderBy(a => random.Next()).ToArray();

        // selectSlots에 스프라이트 설정
        for (int i = 0; i < selectSlots.Length; i++)
        {
            selectSlots[i].SetCardForward(selectors[i]);
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
                Card_ACM selectedSlot = hit.collider.GetComponent<Card_ACM>();
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

    void CheckAnswer(Card_ACM selectedSlot)
    {
        // 선택된 슬롯의 스프라이트 가져옴
        Sprite selectedSprite = selectedSlot.GetSprite();

        if (selectedSprite == answers[currentAnswerIndex])
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
        Vector3 targetPosition = answerSlots[currentAnswerIndex].transform.position;

        // 현재 위치를 저장
        Vector3 originalPosition = touched.transform.position;

        // 타겟 위치로 이동
        touched.transform.DOMove(targetPosition, 0.3f).OnComplete(() =>
        {
            SoundMGR.Instance.SoundPlay("PlayGround_Moving");

            // 이동 완료 후 이펙트 생성
            GameObject particle = Instantiate(effect, targetPosition, Quaternion.identity);
            Destroy(particle, 1f); // 1초 후 이펙트 제거

            // 이동 완료 후 정답 슬롯에 선택된 글자를 설정
            answerSlots[currentAnswerIndex].SetCardForward(answers[currentAnswerIndex]);

            // 다음 정답 슬롯을 가리키도록 인덱스 증가
            currentAnswerIndex++;

            // 터치 활성화
            isTouchable = true;

            Card_ACM touchCard = touched.GetComponent<Card_ACM>();
            touchCard.SetCardAlphaAndScale();


            // 원래 위치로 돌아가기
            touched.transform.DOMove(originalPosition, 0f).OnComplete(() =>
            {          
                // 모든 정답이 맞았는지 확인
                if (currentAnswerIndex == answers.Length)
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

                    Invoke("CorrectsAll", 2f);
                }
            });
        });
    }

    public override void WrongAnswer(GameObject touched)
    {
        isPerfect = false;

        SoundMGR.Instance.SoundPlay("PlayGround_WrongAnswer");

        // 정답 슬롯의 위치를 타겟 위치로 설정
        Vector3 targetPosition = answerSlots[currentAnswerIndex].transform.position;

        // 현재 위치를 저장
        Vector3 originalPosition = touched.transform.position;

        // 타겟 위치로 이동
        touched.transform.DOMove(targetPosition, 0.3f).OnComplete(() =>
        {
            StartCoroutine(ShowHint(answerSlots[currentAnswerIndex], answers[currentAnswerIndex]));
            // 원래 위치로 돌아가기
            touched.transform.DOMove(originalPosition, 0.5f);
        });
    }

    void CorrectsAll()
    {
        ChangeGauge();
        CardInitialize();
    }
}
