using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowSlot_HNS : MonoBehaviour
{
    [SerializeField] Slot_HNS[] slots;
    [SerializeField] Slot_HNS correctSlot;

    private void Awake()
    {
        // 초기화: 자식 오브젝트에 있는 Slot_HNS 컴포넌트를 가져오기
        slots = GetComponentsInChildren<Slot_HNS>();
    }

    private void OnEnable()
    {
        // 오브젝트가 활성화될 때마다 질문을 생성
        MakeQuestion();
    }

    void MakeQuestion()
    {
        // 두 개의 랜덤 숫자 생성 (곱셈 문제를 위해)
        int number1 = Random.Range(1, 10);
        int number2 = Random.Range(1, 10);
        int multipleValue = Multiplication(number1, number2);
        string multiplication = $"{number1}x{number2}";

        int randomNumber;

        // randomNumber와 multipleValue가 같지 않을 때까지 반복하여 랜덤 숫자 생성
        do
        {
            randomNumber = Random.Range(1, 82);
        } while (randomNumber == multipleValue);

        // 슬롯에 랜덤으로 값 할당
        int randomSlotIndex = Random.Range(0, 2);

        // 첫 번째 슬롯에 숫자, 두 번째 슬롯에 곱셈 문제 넣기
        slots[randomSlotIndex].SetText(randomNumber.ToString()); // 숫자 슬롯
        slots[1 - randomSlotIndex].SetText(multiplication); // 곱셈 문제 슬롯

        // 정답을 가진 슬롯 판단
        if (randomNumber > multipleValue)
        {
            correctSlot = slots[randomSlotIndex];
        }
        else
        {
            correctSlot = slots[1 - randomSlotIndex];
        }
    }

    int  Multiplication(int a, int b)
    {
        return (a * b);
    }

    public string GetCorrectText()
    {
        return correctSlot.GetText();
    }

    public void SetCollderEnables()
    {
        BoxCollider2D[] colliders = GetComponentsInChildren<BoxCollider2D>();

        foreach(BoxCollider2D collider in colliders)
        {
            collider.enabled = true;
        }
    }

    public void SetCollderDisables()
    {
        BoxCollider2D[] colliders = GetComponentsInChildren<BoxCollider2D>();

        foreach (BoxCollider2D collider in colliders)
        {
            collider.enabled = false;
        }
    }
}
