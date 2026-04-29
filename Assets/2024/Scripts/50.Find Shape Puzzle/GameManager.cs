using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FindShapePuzzle
{
    public class GameManager : TouchManager_3DTouch
    {
        public static GameManager Instance;

        [SerializeField] private PuzzleManager puzzleManager;
        public GameObject magicCircles;
        [SerializeField] Transform hintZone;      
        [SerializeField] Transform answerZone;
        [SerializeField] MagicLife life;
        [SerializeField] Transform[] frames = null;
        public CameraMove cam;
        [SerializeField] GameObject candySpawner;
        [SerializeField] List<Puzzle> framePuzzles = null;

        private GameObject hintPuzzle;
        Puzzle selectedPuzzle;
        [SerializeField] private string hintPuzzleName;
        private int answerStack = 0;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void OnDestroy()
        {
            if (Instance != null)
            {
                Instance = null;
            }
        }

        public void SetPuzzleQuiz()
        {
            // 기존의 hintPuzzle이 존재하면 파괴
            if (hintPuzzle != null)
            {
                Destroy(hintPuzzle);
                hintPuzzle = null;
            }

            puzzleManager.PlacePuzzlesOnCarpet();

            if(selectedPuzzle != null)
            {
                selectedPuzzle.Fade.FadeIn(0f);
            }

            int randomIdex = Random.Range(0, puzzleManager.Puzzles.Length);

            hintPuzzle = Instantiate(puzzleManager.Puzzles[randomIdex].gameObject, hintZone.position, hintZone.rotation);
            Rigidbody rigidBody = hintPuzzle.GetComponent<Rigidbody>();
            rigidBody.isKinematic = true;
            hintPuzzle.GetComponent<Puzzle>().MoveAndRotate(10f);
            puzzleManager.ToggleAllPuzzlesTag();
            hintPuzzle.tag = "Untagged";

            hintPuzzleName = hintPuzzle.name;
        }

        public override void HandleInput(Vector2 pos)
        {
            Ray ray = Camera.main.ScreenPointToRay(pos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.transform.parent != null && hit.collider.transform.parent.CompareTag("Puzzle"))
                {                   
                    selectedPuzzle = hit.collider.transform.parent.GetComponent<Puzzle>();
                    puzzleManager.ToggleAllPuzzlesTag();

                    if (hintPuzzleName.Contains(selectedPuzzle.gameObject.name))
                    {                      
                        selectedPuzzle.MoveToAnswerZone(answerZone.position);
                    }
                    else
                    {
                        selectedPuzzle.TouchWrongAnswer();
                        life.LifeDelete();

                        DOVirtual.DelayedCall(1f, () =>
                        {
                            puzzleManager.ToggleAllPuzzlesTag();
                        });
                    }

                    isTouchable = true;
                }
                else
                {
                    // 터치한 오브젝트가 적합하지 않으면 즉시 다시 터치 가능
                    isTouchable = true;
                }
            }
            else
            {
                // 터치한 오브젝트가 적합하지 않으면 즉시 다시 터치 가능
                isTouchable = true;
            }
        }

        public void SpawnCandy()
        {
            hintPuzzle.SetActive(false);
            magicCircles.SetActive(false);
            puzzleManager.SortingPuzzle();
            cam.MoveCam(() => candySpawner.SetActive(true));

            for(int i = 0; i < framePuzzles.Count; i++)
            {
                framePuzzles[i].Fade.FadeOut(0f);
                framePuzzles[i].Fade.FadeIn(5f, GameClear);
            }

        }

        private void GameClear()
        {
            victoryUI.SetActive(true);
            SoundMGR.Instance.bgmSource.Stop();
            SoundMGR.Instance.SoundPlay("win");
        }

        public void SavePuzzleInFrame(System.Action onComplete = null)
        {
            // hintPuzzle을 frames[answerStack]의 자식으로 인스턴스화하고 Puzzle 컴포넌트 가져오기
            Puzzle framePuzzle = Instantiate(hintPuzzle, frames[answerStack]).transform.GetComponent<Puzzle>();
            framePuzzles.Add(framePuzzle);

            // 위치 및 회전 초기화
            framePuzzle.transform.localPosition = Vector3.zero;
            framePuzzle.transform.localRotation = Quaternion.identity;

            // Rigidbody를 키네마틱으로 설정
            framePuzzle.GetComponent<Rigidbody>().isKinematic = true;

            // answerStack이 5가 되기 전까지는 SetPuzzleQuiz, 5일 때는 onComplete 호출
            framePuzzle.Fade.FadeOut(0f);
            if (answerStack < 4)
            {
                 SetPuzzleQuiz();
            }
            else
            {
                onComplete?.Invoke();
            }

            // answerStack 증가
            answerStack++;
        }
    }
}
