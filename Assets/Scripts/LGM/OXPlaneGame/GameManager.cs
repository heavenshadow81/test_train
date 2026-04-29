using LGM.CraneGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static Settings;

namespace LGM
{
    namespace OXPlaneGame
    {
        public class GameManager : Singleton<GameManager>
        {
            public bool getKey = false;
            public bool clickRock = true;
            public List<Line> map = new List<Line>(); // 맵
            public List<NormalPlane> trapPlane = new List<NormalPlane>();   // 함정 발판
            public GameObject gameOver;
            public GameObject gameClear;
            public GameObject doorObj;



            public ZoZoBasePatton<GameManager> zozo;
            public EnumClass stateClass;
            public GameUI gameUI;
            public ScreenProsess screenProsess;

            private void Awake()
            {
                stateClass = new EnumClass();
                #region 공용 스테이트 패턴 

                ActionProcess.Enter_StateListener(Init, null, () => {clickRock = false; }, null);

                zozo = new ZoZoBasePatton<GameManager>();
                zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
                #endregion
            }

            private void Init()
            {
                // 각 발판 마다 자신의 인덱스 위치를 ID로 저장
                for (int i = 0; i < map.Count; i++)
                {
                    for (int j = 0; j < map[i].x.Count; j++)
                    {
                        map[i].x[j].id = new Vector2Int(j, i);
                    }
                }

                int trap = 0;   // 함정 설치 장소
                int y = map.Count - 2;  // 맨끝 2번째줄
                // 각 라인마다 1개씩 함정 설치
                for (int i = 0; i < y; i++)
                {
                    // 지뢰 설치할 발판 인덱스 랜덤으로 설정
                    trap = UnityEngine.Random.Range(0, map[i].x.Count);
                    // 선택한 발판이 NormalPlane타입인지 체크
                    NormalPlane plane = map[i].x[trap] as NormalPlane;

                    if (plane != null)
                    {
                        plane.isTrap = true;    // 함정 설치
                        trapPlane.Add(plane);   // 함정 등록
                    }
                }

                // 열쇠와 함정 설치
                trap = UnityEngine.Random.Range(0, map[y].x.Count); // 함정 설치할 인덱스
                for (int i = 0; i < map[y].x.Count; i++)
                {
                    NormalPlane plane = map[y].x[i] as NormalPlane;
                    if (trap == i)  // 함정 설치
                    {
                        plane.isTrap = true;
                        trapPlane.Add(map[y].x[i] as NormalPlane);
                    }
                    else // 열쇠 매설
                    {
                        CreateKey(plane);
                    }
                }

                
            }

            // 게임 패배
            public void GameOver()
            {
                clickRock = true;
                //gameOver.SetActive(true);
                stateClass.resultState = GameResult.Fail;
                zozo.Change(GameState.GameResult);
            }
            // 게임 클리어
            public void GameClear()
            {
                clickRock = true;
                //gameClear.SetActive(true);
                stateClass.resultState = GameResult.Success;
                zozo.Change(GameState.GameResult);
            }

            // 키 생성
            private void CreateKey(NormalPlane plane)
            {
                plane.isKey = true;
            }
        }
    }
}

