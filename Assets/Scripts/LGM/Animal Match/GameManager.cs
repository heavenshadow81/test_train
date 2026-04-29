using Bax.P0.Client.UnityWorld.PictureGame;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Settings;

namespace LGM
{
    namespace AnimalMatch
    {
        public class GameManager : Singleton<GameManager>
        {
            [HideInInspector]
            public int successCount = 4;
            [HideInInspector]
            public int gameClear = 0;
            public GameObject clearUI;
            // 모든 카드 정보
            public Dictionary<CardType, PairCard> dicCard = new Dictionary<CardType, PairCard>();
            public Transform[] a;   // 위에 배치될 카드 위치 정보
            public Transform[] b;   // 아래에 배치될 카드 위치 정보
            public Transform[] compares;


            public EnumClass stateClass;
            public GameUI gameUI;
            public ScreenProsess screenProsess;
            public ZoZoBasePatton<GameManager> zozo;

            private void Awake()
            {
                #region 공용 스테이트 패턴 
                stateClass = new EnumClass();
                ActionProcess.Enter_StateListener(
                () =>
                {
                    // Resources파일에 있는 카드 정보 가져오기
                    GameObject[] _a = Resources.LoadAll<GameObject>("Animal Match/Prefabs/Parts A");
                    GameObject[] _b = Resources.LoadAll<GameObject>("Animal Match/Prefabs/Parts B");

                    // 가져온 정보를 Dictionary타입으로 저장
                    for (int i = 0; i < (int)CardType.Max; i++)
                    {
                        dicCard.Add((CardType)i, new PairCard((CardType)i, _a[i], _b[i]));
                    }
                },
                () => 
                {
                    //Settings.instance.ContantSettingPanelPos(ScreenRotation.Width);
                    // list 내의 중복되지않은 n개의 값을 리스트로 반환
                    List<KeyValuePair<CardType, PairCard>> randomCard =
                            dicCard.ToList().GetRandomNotOverlap(4);
                    List<Transform> randomPosA = a.ToList().GetRandomNotOverlap(4);
                    List<Transform> randomPosB = b.ToList().GetRandomNotOverlap(4);

                    // 랜덤한 위치에 랜덤한 카드 생성
                    for (int i = 0; i < randomPosA.Count; i++)
                    {
                        Instantiate(randomCard[i].Value.A, randomPosA[i]);
                        Instantiate(randomCard[i].Value.B, randomPosB[i]);
                    }

                }, null, 
                async () => 
                {
                    //파티클
                   UIEFFECT();
                });

                zozo = new ZoZoBasePatton<GameManager>();
                zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
                #endregion
            }

            private async UniTask UIEFFECT()
            {
                await UniTask.Delay(System.TimeSpan.FromSeconds(1.5f) , DelayType.UnscaledDeltaTime);
                clearUI.SetActive(true);
            }

        }
    }
}

