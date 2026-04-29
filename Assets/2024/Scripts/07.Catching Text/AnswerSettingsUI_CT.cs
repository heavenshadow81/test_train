using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerSettingsUI_CT : MonoBehaviour
{
    [SerializeField] Sprite[] alphabets; // 알파벳 스프라이트 배열
    private GameObject[] answers; // 답변을 표시할 이미지 배열
    private Sprite[] correctAnswers; // 랜덤 선택된 답을 저장할 배열
    [SerializeField] RectTransform optionPos; // 옵션 위치를 위한 RectTransform
    Image[] options; // 답변을 표시할 이미지 배열
    public List<Vector2> occupiedPositions = new List<Vector2>(); // 사용된 위치 목록
    [SerializeField] int totalOptions = 5; // 총 옵션 개수

    private void Awake()
    {
        // "Option" 태그를 가진 게임 오브젝트 찾기
        GameObject[] optionObjects = GameObject.FindGameObjectsWithTag("Option");
        answers = GameObject.FindGameObjectsWithTag("Answer");

        // options 배열의 크기를 optionObjects 배열의 크기로 설정
        options = new Image[optionObjects.Length];

        // 각 옵션 오브젝트에서 Image 컴포넌트를 가져와 options 배열에 할당
        for (int i = 0; i < optionObjects.Length; i++)
        {
            options[i] = optionObjects[i].GetComponent<Image>();
        }
    }

    public void Init()
    {
        foreach (Image option in options)
        {
            option.gameObject.SetActive(true);      
        }

        // 사용할 수 있는 스프라이트 목록 생성
        List<Sprite> availableSprites = new List<Sprite>(alphabets);

        // 정답 이미지 배열 크기 설정
        correctAnswers = new Sprite[answers.Length]; // answers 배열 크기에 맞춤

        // 답변 이미지 설정
        for (int i = 0; i < answers.Length; i++)
        {
            // 정답 오브젝트에서 이미지 컴포넌트 받기
            Image image = answers[i].GetComponent<Image>();
            // 랜덤 인덱스 생성
            int randomIndex = Random.Range(0, availableSprites.Count);
            // 랜덤으로 선택한 스프라이트를 image에 할당
            image.sprite = availableSprites[randomIndex];
            correctAnswers[i] = image.sprite; // 정답으로 설정

            // 선택된 스프라이트는 목록에서 제거하여 중복 방지
            availableSprites.RemoveAt(randomIndex);
        }

        // 옵션 이미지 설정 및 위치 랜덤화
        occupiedPositions.Clear();
        float padding = 100f; // 옵션 간의 최소 거리 (조정 가능)

        for (int i = 0; i < totalOptions; i++)
        {
            Vector2 size = optionPos.rect.size;
            Vector2 randomPosition;

            // 새로운 위치가 겹치지 않을 때까지 반복
            do
            {
                float randomX = Random.Range(-size.x / 2, size.x / 2);
                float randomY = Random.Range(-size.y / 2, size.y / 2);
                randomPosition = new Vector2(randomX, randomY);
            } while (IsPositionOccupied(randomPosition, occupiedPositions, padding));

            // 랜덤 위치를 options[i]의 RectTransform에 할당
            options[i].gameObject.GetComponent<RectTransform>().anchoredPosition = randomPosition;
            occupiedPositions.Add(randomPosition); // 사용된 위치 목록에 추가

            if (i < correctAnswers.Length)
            {
                options[i].sprite = correctAnswers[i]; // 정답 스프라이트 할당
            }
            else
            {
                int randomIndex = Random.Range(0, availableSprites.Count);
                options[i].sprite = availableSprites[randomIndex];
                availableSprites.RemoveAt(randomIndex);
            }
        }
    }

    private bool IsPositionOccupied(Vector2 position, List<Vector2> occupiedPositions, float padding)
    {
        foreach (Vector2 occupiedPosition in occupiedPositions)
        {
            if (Vector2.Distance(position, occupiedPosition) < padding)
            {
                return true; // 충돌 발생
            }
        }
        return false; // 충돌 없음
    }

    public GameObject[] GetAnswerObject()
    {
        return answers;
    }

    public Sprite[] GetAnswerSprite()
    {
        return correctAnswers;
    }
}
