using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace FindShapePuzzle
{
    public class PuzzleManager : MonoBehaviour
    {
        [SerializeField] private PuzzleBox box;  
        [SerializeField] private Puzzle[] puzzles = null;  
        public Puzzle[] Puzzles
        {
            get { return puzzles; }
        }

        [SerializeField] private GameObject carpet;     
        [SerializeField] private int rows = 5;  
        [SerializeField] private int columns = 6;  
        [SerializeField] private float spacing = 2.5f;  // 퍼즐 간 간격

        // 퍼즐을 카펫 안에 그리드 형태로 랜덤하게 배치하는 메서드
        public void PlacePuzzlesOnCarpet()
        {
            // 퍼즐 배열을 섞기
            ShufflePuzzles();

            // 카펫의 크기를 감지 (Collider나 Renderer 중 하나를 사용할 수 있음)
            Collider carpetCollider = carpet.GetComponent<Collider>();          
            Bounds carpetBounds = carpetCollider.bounds;    // 카펫의 바운드 (크기와 위치 정보) 가져오기

            // 그리드의 시작점 계산 (카펫 중앙에서 시작)
            Vector3 startPosition = new Vector3(
                carpetBounds.min.x + (carpetBounds.size.x - (columns - 1) * spacing) / 2,
                carpetBounds.center.y + 0.5f,  // Y축은 카펫의 높이
                carpetBounds.min.z + (carpetBounds.size.z - (rows - 1) * spacing) / 2
            );

            int totalPuzzles = Mathf.Min(puzzles.Length, rows * columns);  // 총 퍼즐 수
            int puzzleIndex = 0;

            // 퍼즐들을 그리드 안에 배치
            for (int i = 0; i < rows; i++)  // 행 (Z축)
            {
                for (int j = 0; j < columns; j++)  // 열 (X축)
                {
                    if (puzzleIndex >= totalPuzzles) break;  // 퍼즐이 다 배치되면 종료

                    Vector3 position = new Vector3(
                        startPosition.x + j * spacing,  // X축 간격 계산
                        startPosition.y,                // Y축 (카펫 위)
                        startPosition.z + i * spacing   // Z축 간격 계산
                    );

                    puzzles[puzzleIndex].gameObject.transform.position = position;
                    puzzles[puzzleIndex].gameObject.transform.rotation = Quaternion.identity; 
                    puzzleIndex++;
                }
            }
        }

        // 퍼즐 배열을 랜덤으로 섞는 메서드 (Fisher-Yates Shuffle 알고리즘)
        private void ShufflePuzzles()
        {
            for (int i = 0; i < puzzles.Length; i++)
            {
                int randomIndex = Random.Range(i, puzzles.Length);
                Puzzle temp = puzzles[i];
                puzzles[i] = puzzles[randomIndex];
                puzzles[randomIndex] = temp;
            }
        }

        public void ToggleAllPuzzlesTag()
        {
            for(int i = 0;i < puzzles.Length;i++)
            {
                puzzles[i].ToggleTag();
            }
        }

        public void SortingPuzzle()
        {
            box.MoveBoxUnderDesk();

            for( int i = 0; i< puzzles.Length;i++)
            {
                Puzzles[i].SetOriginTransform();
            }     
        }

        private void Update()
        {
            if(Input.GetKeyUp(KeyCode.Space))
            {
                SortingPuzzle();
            }
        }
    }
}
