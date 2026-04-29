using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

public class MTManger : PlayManager_PlayGround
{
    [SerializeField] LetterSlot_MT[] numbers; //숫자들

    [SerializeField] TextMeshProUGUI modify; //수식 텍스트
    int modifyAnswer; //수식에 대한 답
    int table; //몇단인지 체크
    int count = 1; //몇번째인지 체크

    protected override void Init()
    {
        base.Init();

        SettingSlot();
    }
    void SettingSlot()
    {
        //위치값 리스트 생성
        List <Vector3> positions = new List<Vector3>();


        //처음시작과 마지막에만 몇단인지 표기
        if (count == 10 || count == 1)
        {
            table = Random.Range(2, 10);
            modify.text = $"{table}단";
        }

        foreach (var slot in numbers)
        {
            //각 슬롯의 위치값 리스트에 저장
            positions.Add(slot.transform.position);

            //슬롯 작아졌다가 커짐
            slot.transform.DOScale(0, 0.2f).OnComplete(() =>
            {
                slot.transform.DOScale(1f, 0.3f).SetDelay(0.5f);
            });
        }

        //리스트의 순서를 랜덤하게 바꿈
        ShuffleArray(positions);

        for (int i = 0; i < positions.Count; i++)
        {
            //슬롯의 위치값을 리스트 순서대로 변경
            numbers[i].transform.position = positions[i];
        }

        Invoke("ShowTable", 0.8f);
    }

    void ShuffleArray(List<Vector3> array)
    {
        for (int i = array.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Vector3 temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    void ShowTable()
    {
        modify.text = $"{table}x{count}";
        modifyAnswer = table * count;

        //print(modifyAnswer);

        int wrong1 = modifyAnswer + Random.Range(1, 3);
        int wrong2 = modifyAnswer + Random.Range(3, 5);
        int wrong3 = modifyAnswer - Random.Range(1, 3);

        numbers[0].answer.text=modifyAnswer.ToString();
        numbers[1].answer.text= wrong1.ToString();
        numbers[2].answer.text= wrong2.ToString();
        numbers[3].answer.text= wrong3.ToString();
    }

    public override void HandleInput(Vector2 inputPosition)
    {
        isTouchable = true;

        // 터치/마우스 위치를 월드 좌표로 변환
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

        // 터치/마우스 위치에서 카드 찾기
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null)
        {
            // 터치한 카드가 어떤 태그를 가지고 있는지 확인
            if (hit.collider.CompareTag(TeamNameString))
            {
                SoundMGR.Instance.SoundPlay("PlayGround_Moving");
                LetterSlot_MT selectedSlot = hit.collider.GetComponent<LetterSlot_MT>();
                if (selectedSlot != null)
                {
                    CheckAnswer(selectedSlot);
                }
            }
        }
    }
    void CheckAnswer(LetterSlot_MT selectedSlot)
    {
        //print(modifyAnswer);
        //isTouchDisabled = true;// 터치 비활성화

        if (selectedSlot.answer.text == modifyAnswer.ToString())
        {
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
        Vector3 originalPosition = touched.transform.position;

        //이펙트 생성
        GameObject particle = Instantiate(effect, originalPosition, Quaternion.identity);
        Destroy(particle, 1f); // 1초 후 이펙트 제거

        //콜라이더 비활성화 후 스케일이 0이되면 활성화
        touched.GetComponent<BoxCollider2D>().enabled = false;
        touched.transform.DOScale(0, 0.3f).OnComplete(() =>
        {
            touched.GetComponent<BoxCollider2D>().enabled = true;
        });

        //초기화 하면서 카운트
        if (count < 9)
            count++;
        else
        {
            count = 1;
            ChangeGauge();
        }

        SettingSlot();
    }

    public override void WrongAnswer(GameObject touched)
    {
        //print($"false: {touched}");
        touched.transform.DOScale(0.7f, 0.2f).OnComplete(() =>
        {
            touched.transform.DOScale(1f, 0.2f);
        });
    }
}
