using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StairGame
{
    public class GameManager : TouchManager_3DTouch
    {
        public static GameManager Instance;

        // 방향을 위한 enum 정의
        public enum Direction
        {
            Up,
            Down,
            Right,
            Left
        }

        [Header("컨테이너")]
        public Player player;
        public Dice dice;
        public MagicTimer timer;

        [Header("페이드")]
        [SerializeField] Image fadeImage;

        public int StairCount { get; set; }
        public int StageCount {  get; set; }

        public int maxStageCount = 5;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            StairCount = 5;
            StageCount = 1;

            timer.OnTimerEnd += GameOver;
        }

        private void OnEnable()
        {
            fadeImage.DOFade(0f, 1f).OnComplete(() =>
            {
                player.gameObject.SetActive(true);
                isTouchable = false;
            });      
        }

        private void OnDestroy()
        {
            if (Instance != null)
            {
                Instance = null;
            }

            timer.OnTimerEnd -= GameOver;
        }

        public override void HandleInput(Vector2 pos)
        {
            Ray ray = Camera.main.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (Enum.TryParse(hit.collider.gameObject.name, out Direction direction))
                {
                    // enum 값에 따라 실행할 메서드 선택
                    switch (direction)
                    {
                        case Direction.Up:
                            ExecuteDirection(Direction.Up);
                            break;
                        case Direction.Down:
                            ExecuteDirection(Direction.Down);
                            break;
                        case Direction.Right:
                            ExecuteDirection(Direction.Right);
                            break;
                        case Direction.Left:
                            ExecuteDirection(Direction.Left);
                            break;
                        default:
                            Debug.Log("Unknown direction");
                            break;
                    }
                }
                else
                {
                    isTouchable = true;
                }
            }
            else
            { 
                isTouchable = true; 
            }
        }

        private void ExecuteDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    dice.RollUp(CheckDiceFaceFromCamera);
                    break;
                case Direction.Down:
                    dice.RollDown(CheckDiceFaceFromCamera);
                    break;
                case Direction.Right:
                    dice.RollRight(CheckDiceFaceFromCamera);
                    break;
                case Direction.Left:
                    dice.RollLeft(CheckDiceFaceFromCamera);
                    break;
            }
        }

        // Dice 방향으로 레이를 쏘고 StairCount와 비교하는 함수
        public void CheckDiceFaceFromCamera()
        {
            // 카메라에서 Dice를 향하는 레이 생성
            Vector3 direction = (dice.transform.position - Camera.main.transform.position).normalized;
            Ray ray = new Ray(Camera.main.transform.position, direction);

            // 디버그 레이 그리기 (5초 동안 빨간색으로 표시)
            Debug.DrawRay(ray.origin, direction * 100f, Color.red, 5f);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 맞은 오브젝트가 Dice인 경우에만 처리
                if (hit.collider.transform.parent.gameObject == dice.gameObject)
                {
                    string hitObjectName = hit.collider.gameObject.name;

                    // Dice의 면 이름이 StairCount와 일치하는지 확인
                    if (hitObjectName.Equals(StairCount.ToString()))
                    {
                        player.ClimbStairs();
                    }
                    else
                    {
                        isTouchable = true;
                    }
                }
                else
                {
                    isTouchable = true;
                }
            }
        }

        public void SetTouchEnable(bool isTouch)
        {
            isTouchable = isTouch;
        }

        public void GameClear()
        {
            victoryUI.SetActive(true);
            SoundMGR.Instance.SoundPlay("win");
        }

        private void GameOver()
        {
            isTouchable = false;
            player.StopJumpAnim();
            player.Col.enabled = false;
            player.Rigid.isKinematic = true;
        }
    }
}
