using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;
using static Settings;

namespace LGM
{
    namespace StarRun
    {
        public class LineManager : Singleton<LineManager>
        {
            private GameView screen = new GameView();   // 화면 크기 정보
            private LineRenderer lines;
            public GameObject gamOverUI;
            [HideInInspector]
            public bool playerDie = false;  // 플레이어 사망 여부
            public float speed; // 선 내려가는 속도
            public float acellSpeed;    // 가속
            public float width; // 선 두께

            private void Awake()
            {
                lines = GetComponent<LineRenderer>();
            }
            private void Start()
            {
                //Settings.instance.ContantSettingPanelPos(ScreenRotation.Height);
                lines.startWidth = width;   // 라인 두께 설정
                screen.InitSize();  // 화면 사이즈 초기화
            }

            public void UpdateLogic()
            {
                if (!playerDie) // 플레이어가 살아있는 동안
                {
                    speed += Time.deltaTime * acellSpeed;   // 라인의 이동속도를 점차 증가
                }

                DropLine(lines, speed); // LineRenderer를 왼쪽으로 이동 시킴

                // 화면 왼쪽 끝 기준 -20만큼 이동 시 랜덤 위치 재설정
                if (lines.GetPosition(1).x <= -screen.height - 20)
                {
                    SetRePoint();   // 라인이 끝에 닿으면 라인 오른쪽 맨끝 새로운 위치에 라인 추가
                }
                if (LineCastHit(lines, width))  // 플레이어 라인 충돌 체크
                {
                    DieEvent(); // 라인 충돌 시 플레이어 패배
                }
            }


            private void Update()
            {
               
            }
            // 플레이어 사망 이벤트
            private void DieEvent()
            {
                StarController ctr = StarController.Instance;
                Instantiate(ctr.diePrefab, ctr.transform.position, Quaternion.identity);    // 사망 시 파티클 생성
                Destroy(ctr.gameObject);    // 플레이어 삭제
                speed = 0;  // 라인 움직임 정지
                playerDie = true;   // 플레이어 사망

                var mgr = FindObjectOfType<ScoreManager>();
                mgr.stateClass.resultState = GameResult.Fail;
                mgr.zozo.Change(GameState.GameResult);

                //gamOverUI.SetActive(true);  // 게임 오버 UI 활성화
            }
            // LineRenderer에 있는 position.y 값을 _speed만큼 이동 (플레이어 대신 라인이 왼쪽으로 이동)
            private void DropLine(LineRenderer _lines, float _speed)
            {
                for (int i = 0; i < _lines.positionCount; i++)
                {
                    
                    Vector3 _temp = _lines.GetPosition(i);  // LineRenderer의 i번째 라인의 위치 값
                    _temp.x -= Time.deltaTime * _speed; // 라인을 _speed만큼 왼쪽으로 이동
                    _lines.SetPosition(i, _temp);   // 변경된 위치값 저장
                }
            }

            // 랜덤한 위치에 새로운 라인 추가
            private void SetRePoint()
            {
                // 0번 인덱스를 지우고 맨끝에 새로운 점 추가하기
                List<Vector3> temp = new List<Vector3>();
                for (int i = 1; i < lines.positionCount; i++)
                {
                    temp.Add(lines.GetPosition(i)); // LineRenderer의 i번째 라인의 위치 값
                }
                float width = Random.Range(-screen.width + 2, screen.width - 2);    // 새로 추가할 라인의 y축 랜덤 좌표
                float height = Random.Range(6, 8f); // 라인이 시작할 x축 랜덤 좌표
                temp.Add(new Vector3(temp[temp.Count - 1].x + height, width));  // 끝 점 추가
                lines.SetPositions(temp.ToArray()); // array타입으로 변환 후 저장
            }

            // 라인에 충돌했는지 체크
            private bool LineCastHit(LineRenderer _lines, float _width)
            {
                for (int i = 0; i < _lines.positionCount - 1; i++)
                {
                    Vector2 start = _lines.GetPosition(i);  // 라인 위치 좌표
                    Vector2 startL = new Vector2(start.x, start.y - _width / 2) + Vector2.up;   // 왼쪽 라인 시작 점
                    Vector2 startR = new Vector2(start.x, start.y + _width / 2) - Vector2.up;   // 오른쪽 라인 시작 점
                    Vector2 end = _lines.GetPosition(i + 1);// start 다음 라인 위치 좌표
                    Vector2 endL = new Vector2(end.x, end.y - _width / 2) + Vector2.up; // 왼쪽 라인 끝 점
                    Vector2 endR = new Vector2(end.x, end.y + _width / 2) - Vector2.up; // 오른쪽 라인 끝 점

                    // 디버그용 라인 그리기
                    Debug.DrawLine(startL, endL, Color.red);
                    Debug.DrawLine(startR, endR, Color.red);

                    // Linecast에 충돌 시 true(사망) 반환
                    if (Physics2D.Linecast(startL, endL) ||
                        Physics2D.Linecast(startR, endR)) 
                    {
                        return true;
                    }
                }
                return false;   // 충돌하지 않음으로 false 반환
            }
        }
    }
}

