using UnityEngine;

public class TileGrid_HNS : MonoBehaviour
{
    public GameObject tilePrefab;
    public RectTransform patentsMap; // 부모의 RectTransform
    public int rows = 5;
    public int columns = 2;
    public float initialScale = 1f;
    public float scaleReduction = 0.15f;  // 타일 크기를 줄이는 정도
    public float spacing = 100f;  // 스페이싱을 픽셀 단위로 조정
    public float horizontalSpacing = 200f;  // 왼쪽과 오른쪽 타일 간의 간격을 픽셀 단위로

    void Start()
    {
        GenerateTileGrid();
    }

    void GenerateTileGrid()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                // 타일 생성
                GameObject tile = Instantiate(tilePrefab, patentsMap);

                // RectTransform 컴포넌트 가져오기
                RectTransform tileRect = tile.GetComponent<RectTransform>();

                // 타일 위치 설정
                float xPos = (i * horizontalSpacing) - (horizontalSpacing / 2);  // 중앙에서 시작
                float yPos = j * spacing;

                // anchoredPosition을 사용하여 위치 설정
                tileRect.anchoredPosition = new Vector2(xPos, yPos);

                // 크기 조정 (localScale 대신에 sizeDelta를 사용)
                float scale = initialScale - (j * scaleReduction);
                tileRect.sizeDelta = new Vector2(tileRect.sizeDelta.x * scale, tileRect.sizeDelta.y * scale);
            }
        }

        // 전체 트랙을 중앙으로 이동 (RectTransform의 pivot을 이용해 중앙 정렬)
        patentsMap.anchoredPosition = new Vector2(0, -rows * spacing / 2);
    }
}
