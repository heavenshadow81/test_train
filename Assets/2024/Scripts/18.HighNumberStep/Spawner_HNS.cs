using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Spawner_HNS : MonoBehaviour
{
    [SerializeField] GameObject rowSlotPrefab; // RowSlot_HNS가 할당된 프리팹
    [SerializeField] int numberOfRows = 6; // 생성할 줄의 개수
    [SerializeField] float initialScale = 1.0f; // 처음 생성되는 줄의 스케일
    [SerializeField] float scaleReductionFactor = 0.85f; // 위로 갈수록 줄어드는 스케일 비율
    [SerializeField] float verticalSpacing = 160; // 줄 간의 세로 간격
    [SerializeField] int poolSize = 8; // 오브젝트 풀 크기

    [SerializeField] private Queue<GameObject> rowPool = new Queue<GameObject>(); // 오브젝트 풀
    public List<GameObject> rows = new List<GameObject>(); // 현재 활성화된 줄을 저장할 리스트
    [SerializeField] private List<Vector2> slotPositions = new List<Vector2>(); // 초기 위치 저장 리스트
    [SerializeField] private List<Vector3> slotScales = new List<Vector3>(); // 초기 스케일 저장 리스트

    [SerializeField] HNSManager manager;

    // 오브젝트 풀 초기화
    public void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject rowSlot = Instantiate(rowSlotPrefab, transform);
            rowSlot.SetActive(false); // 풀에 추가할 때 비활성화
            rowPool.Enqueue(rowSlot);
        }
    }

    // 줄 생성
    public void GenerateRows()
    {
        float accumulatedHeight = 0;

        for (int i = 0; i < numberOfRows; i++)
        {
            GameObject rowSlot = GetPooledObject();

            RectTransform rectTransform = rowSlot.GetComponent<RectTransform>();

            float scale = initialScale * Mathf.Pow(scaleReductionFactor, i);
            rectTransform.localScale = new Vector3(scale, scale, 1);

            Vector2 position;
            if (i == 0)
            {
                position = new Vector2(0, 0);
            }
            else
            {
                accumulatedHeight += verticalSpacing * Mathf.Pow(scaleReductionFactor, i - 1);
                position = new Vector2(0, accumulatedHeight);
            }

            rectTransform.anchoredPosition = position;

            // 초기 위치 및 스케일 저장
            slotPositions.Add(position);
            slotScales.Add(rectTransform.localScale);

            rowSlot.SetActive(true);
            rows.Add(rowSlot);
        }

        RowSlot_HNS firstRowSlot = rows[0].GetComponent<RowSlot_HNS>();
        firstRowSlot.SetCollderEnables();
    }

    // 정답 클릭 시 처리
    public void HandleSlotMove()
    {
        if (rows.Count == 0)
            return;

        // 맨 밑의 줄을 풀에 반환

        RowSlot_HNS firstRowSlot = rows[0].GetComponent<RowSlot_HNS>();
        firstRowSlot.SetCollderDisables();

        GameObject lastRow = rows[0];
        rows.RemoveAt(0);
        ReturnPooledObject(lastRow);

        // 모든 애니메이션이 완료된 후 실행될 콜백을 위한 시퀀스 설정
        Sequence sequence = DOTween.Sequence();

        // 나머지 줄 한 칸씩 내리기
        for (int i = 0; i < rows.Count; i++)
        {
            RectTransform rectTransform = rows[i].GetComponent<RectTransform>();
            sequence.Join(rectTransform.DOAnchorPos(slotPositions[i], 0.5f)); // 한 칸씩 아래 위치로 이동
            sequence.Join(rectTransform.DOScale(slotScales[i], 0.5f)); // 스케일도 함께 변경
        }

        // 애니메이션이 모두 완료된 후 실행할 작업을 OnComplete에 정의
        sequence.OnComplete(() =>
        {
            // 맨 위에 새 줄 생성 및 추가
            GameObject newRow = GetPooledObject();
            RectTransform newRectTransform = newRow.GetComponent<RectTransform>();

            // 맨 위 줄의 스케일과 위치 설정 (초기 값 사용)
            newRectTransform.localScale = slotScales[slotScales.Count - 1];
            newRectTransform.anchoredPosition = slotPositions[slotPositions.Count - 1];

            newRow.SetActive(true);
            rows.Add(newRow);

            RowSlot_HNS firstRowSlot = rows[0].GetComponent<RowSlot_HNS>();
            firstRowSlot.SetCollderEnables();

            manager.SetTouchabled();
        });

        // 시퀀스 실행
        sequence.Play();
    }

    // 풀에서 오브젝트 가져오기
    GameObject GetPooledObject()
    {
        if (rowPool.Count > 0)
        {
            return rowPool.Dequeue();
        }
        else
        {
            // 풀에 남은 오브젝트가 없으면 새로 생성 (풀 사이즈에 맞추는 것이 좋음)
            GameObject newRow = Instantiate(rowSlotPrefab, transform);
            newRow.SetActive(false);
            rowPool.Enqueue(newRow);
            return rowPool.Dequeue();
        }
    }

    // 오브젝트를 풀에 반환
    void ReturnPooledObject(GameObject obj)
    {
        obj.SetActive(false);
        rowPool.Enqueue(obj);
    }
}
