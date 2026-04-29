using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CenterBorder_NOX : MonoBehaviour
{
    [SerializeField] Color[] colors = new Color[7];
    public TextMeshProUGUI[] texts;

    public List<int> currentCenterNumbers = new List<int>(); // 현재 가운데 숫자들

    private void Awake()
    {
        texts = GetComponentsInChildren<TextMeshProUGUI>();

        InitializedCenterText();
    }

    void InitializedCenterText()
    {
        // 색상 배열을 섞음 (Fisher-Yates shuffle 알고리즘)
        colors = ShuffleColors(colors);

        // 텍스트에 색상과 랜덤 숫자 할당
        for (int i = 0; i < texts.Length; i++)
        {
            // 색상 인덱스 선택 (순환적으로 선택)
            Color selectedColor = colors[i % colors.Length];
            texts[i].color = selectedColor;

            // 0부터 9까지의 랜덤 숫자 생성 및 텍스트로 설정
            int randomNumber = Random.Range(0, 10);
            texts[i].text = randomNumber.ToString();
            currentCenterNumbers.Add(randomNumber);
        }
    }

    // Fisher-Yates shuffle 알고리즘으로 색상 배열 섞기
    private Color[] ShuffleColors(Color[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Color temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
        return array;
    }

    public int GetRandomIndex()
    {
        return Random.Range(0, texts.Length);
    }

    public void SetText(int index)
    {
        // 인덱스가 유효한지 확인
        if (index < 0 || index >= currentCenterNumbers.Count)
        {
            //Debug.LogError($"Index {index} is out of range. It must be between 0 and {currentCenterNumbers.Count - 1}.");
            return;
        }

        // 새로운 랜덤 숫자 생성
        int randomNumber = Random.Range(0, 10);

        // 리스트에서 특정 인덱스의 값을 새로운 숫자로 변경
        currentCenterNumbers[index] = randomNumber;

        // 텍스트 UI 업데이트
        texts[index].text = randomNumber.ToString();
    }
}
