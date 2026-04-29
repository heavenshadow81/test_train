using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LGM
{
    namespace StarRun
    {
        public class ScoreManager : MonoBehaviour
        {
            public float speed = 1f;    // 타이머 속도
            public float tickPoint = 10f;   // n초당 tickPoint만큼 증가
            public TextMeshProUGUI text;    // 점수를 출력할 텍스트

            [HideInInspector]
            public float timer = 0; // 타이머
            [HideInInspector]
            public float score = 0; // 점수


            public EnumClass stateClass;
            public GameUI gameUI;
            public ScreenProsess screenProsess;
            public ZoZoBasePatton<ScoreManager> zozo;
            private void Awake()
            {
                stateClass = new EnumClass();

                #region 공용 스테이트 패턴 

                ActionProcess.Enter_StateListener(null, null, null, null);

                zozo = new ZoZoBasePatton<ScoreManager>();
                zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
                #endregion
            }

            private void Update()
            {
                if(zozo != null) 
                {
                    zozo.MGR.Excute(() =>
                    {
                        // 플레이어가 살아있을때만 실행
                        if (!LineManager.Instance.playerDie)
                        {
                            timer += Time.deltaTime * speed;    // 시간 누적
                            score = (int)timer * tickPoint;     // 점수 누적
                            text.text = ((int)score).ToString();// 점수 텍스트 표시
                        }
                        LineManager.Instance.UpdateLogic();
                        StarController.Instance.playerMoving();
                    });
                }


               
            }
        }
    }
}
