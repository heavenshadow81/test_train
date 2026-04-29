using System;
using UnityEngine;
using UnityEngine.UI;

public class CellManager_PC : MonoBehaviour
{
    public static CellManager_PC Instance { get; private set; }

    public GameObject cellPrefab; // 원본 Cell 오브젝트를 드래그하여 할당
    public GameObject parents;
    public int rows = 4; // 행의 수
    public int cols = 7; // 열의 수

    public GameObject[] cells;

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        SpawnCells();
    }

    public void SpawnCells()
    {
        cellPrefab.SetActive(true);

        // cellPrefab의 위치를 기준으로 시작
        Vector2 startPosition = cellPrefab.GetComponent<RectTransform>().anchoredPosition;

        // blurImages 배열 크기 조정
        cells = new GameObject[rows * cols];

        int index = 0; // 배열 인덱스 초기화

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                // 새로운 Cell 오브젝트를 생성
                GameObject newCell = Instantiate(cellPrefab);

                // 각 칸의 위치 계산 (기준 위치에 200을 더해가며)
                float xPos = startPosition.x + col * 200f; // x축으로 200씩 이동
                float yPos = startPosition.y - row * 200f; // y축으로 200씩 이동

                // 생성된 Cell의 위치 설정
                newCell.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);

                // 생성된 Cell을 부모의 자식으로 설정
                newCell.transform.SetParent(parents.transform, false); // 부모 오브젝트로 설정

                // 생성된 Cell을 배열에 추가
                cells[index] = newCell; // 배열에 추가
                index++; // 인덱스 증가
            }
        }

        cellPrefab.SetActive(false);
    }
}
