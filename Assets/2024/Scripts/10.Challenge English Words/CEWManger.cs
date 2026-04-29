using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.InputSystem.EnhancedTouch;
using LGM.AnimalMatch;

public class CEWManger : PlayManager_PlayGround
{
    [Header("게임 설정")]
    private string[] words;
    private string[] KR_Words;
    [SerializeField] LetterSlot_CEW[] answerSlots; // 정답 셀
    [SerializeField] LetterSlot_CEW[] selectSlots; // 선택 셀 
    private string answerWord;
    private string[] answerWords;
    private int currentAnswerIndex = 0; // 현재 정답 슬롯 인덱스
    private string selectedLetter_Temp;
    private List<int> usedIndices = new List<int>(); // 이미 사용된 단어 인덱스를 저장할 리스트

    [Header("힌트")]
    private int answerIdx;
    [SerializeField] TextMeshProUGUI hintText;

    void Awake()
    {
        words = new string[]
    {
        "Ball", "Doll", "Fish", "Bear", "Frog", "Star", "Moon", "Lion",
        "Duck", "Book", "Tree", "Rain", "Snow", "Milk", "Cake", "Bird",
        "Bath", "King", "Ring", "Lamb", "Ship", "Drum", "Jump", "Flag",
        "Nest", "Girl", "Game", "Kite", "Wolf", "Hand"
    };

        KR_Words = new string[]
    {
        "공", "인형", "물고기", "곰", "개구리", "별", "달", "사자",
        "오리", "책", "나무", "비", "눈", "우유", "케이크", "새",
        "목욕", "왕", "반지", "어린 양", "배", "드럼", "뛰어오름", "깃발",
        "둥지", "소녀", "게임", "연", "늑대", "손"
    };
    }

    protected override void Init()
    {
        base.Init();

        SettingWord();
    }

    void SettingWord()
    {
        SettingSlot();
        SetupAnswerSlots();
        SetupHint();
        SetupChoiceSlots();
    }

    void SettingSlot()
    {
        foreach (var slot in answerSlots)
        {
            slot.Init();

            // 카드의 현재 회전을 초기화
            slot.transform.DORotate(new Vector3(0, 180, 0), 0.5f).OnComplete(() =>
            {
                // Y축을 기준으로 180도 회전하는 애니메이션 추가
                slot.transform.DORotate(Vector3.zero, 0f);
            });
        }

        foreach (var slot in selectSlots)
        {
            slot.Init();
            slot.gameObject.SetActive(true);

            // 카드의 현재 회전을 초기화
            slot.transform.DORotate(new Vector3(0, 180, 0), 0.5f).OnComplete(() =>
            {
                // Y축을 기준으로 180도 회전하는 애니메이션 추가
                slot.transform.DORotate(Vector3.zero, 0f);
            });
        }

        isPerfect = true;
    }

    void SetupAnswerSlots()
    {
        // 이미 사용된 단어 인덱스를 제외하고 랜덤으로 단어 인덱스 선택 
        do
        {
            answerIdx = Random.Range(0, words.Length);
        } while (usedIndices.Contains(answerIdx));

        // 사용된 단어 인덱스를 리스트에 추가
        usedIndices.Add(answerIdx);

        // 랜덤으로 단어 선택
        answerWord = words[answerIdx];
        answerWords = answerWord.ToCharArray().Select(c => c.ToString()).ToArray();

        // 모든 정답 슬롯에 "?" 표시
        for (int i = 0; i < answerSlots.Length; i++)
        {
            answerSlots[i].SetLetter("?");
        }
        currentAnswerIndex = 0; // 첫번째 정답 슬롯으로 초기화
    }

    void SetupHint()
    {
        hintText.text = KR_Words[answerIdx];
    }

    void SetupChoiceSlots()
    {
        List<char> choices = new List<char>(answerWord.ToCharArray());
        // 랜덤으로 나머지 알파벳 추가
        while (choices.Count < selectSlots.Length)
        {
            char randomChar = (char)Random.Range('a', 'z' + 1);
            if (!choices.Contains(randomChar))
            {
                choices.Add(randomChar);
            }
        }

        // 선택 셀 랜덤 배치
        for (int i = 0; i < selectSlots.Length; i++)
        {
            int randomIndex = Random.Range(0, choices.Count);
            selectSlots[i].SetLetter(choices[randomIndex].ToString());
            choices.RemoveAt(randomIndex);
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
                LetterSlot_CEW selectedSlot = hit.collider.GetComponent<LetterSlot_CEW>();
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

    void CheckAnswer(LetterSlot_CEW selectedSlot)
    {
        // 선택된 슬롯의 글자를 가져옴
        string selectedLetter = selectedSlot.GetLetter();

        // 현재 정답 인덱스가 정답 단어의 길이보다 작고,
        // 선택된 글자가 현재 정답 인덱스의 글자와 일치하는지 확인
        if (currentAnswerIndex < answerWords.Length && selectedLetter == answerWords[currentAnswerIndex])
        {
            // 선택된 글자를 임시 변수에 저장
            selectedLetter_Temp = selectedLetter;

            // 정답 처리 메서드 호출
            CorrectAnswer(selectedSlot.gameObject);
        }
        else
        {
            // 오답 처리 메서드 호출
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
            answerSlots[currentAnswerIndex].SetLetter(selectedLetter_Temp);

            // 다음 정답 슬롯을 가리키도록 인덱스 증가
            currentAnswerIndex++;

            // 터치 활성화
            isTouchable = true;

            touched.SetActive(false);

            // 원래 위치로 돌아가기
            touched.transform.DOMove(originalPosition, 2f).OnComplete(() =>
            {             
                // 모든 정답이 맞았는지 확인
                if (currentAnswerIndex == answerWords.Length)
                {
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

                    ChangeGauge();
                    SettingWord();
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
            // 원래 위치로 돌아가기
            touched.transform.DOMove(originalPosition, 0.3f).OnComplete(() =>
            {
                // 터치 활성화
                isTouchable = true;
            });
        });
    }
}
