using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

namespace LGM
{

    namespace CraneGame
    {
        public class TimeManager : Singleton<TimeManager>
        {
            public float time = 201;    // 제한 시간
            public float speed = 1;     // 흘러가는 시간 속도
            public TextMeshProUGUI text;// 남은 시간을 보여줄 텍스트

            public void CurTime()
            {
                // 제한 시간이 0보다 작아지면 GameOverEvent 실행
                if (time <= 0)
                {
                    DollManager.Instance.stateClass.resultState = GameResult.Fail;
                    DollManager.Instance.zozo.Change(GameState.GameResult);
                    //GamEndEvent(CraneManager.Instance.gameOver);  // 게임 종료 UI 활성화
                    return;
                }
                if (DollManager.Instance.doll.Count > 0)
                {
                    time -= Time.deltaTime * speed;  // speed만큼 제한 시간 감소
                    text.text = "남은 시간: " + (int)time + "초";     // 남은 시간 텍스트로 출력
                }
            }

           
            // 게임 종료 이벤트
            public void GamEndEvent(GameObject ui)
            {
                CraneManager cManager = CraneManager.Instance;
                cManager.touchRefuse = true;    // 터치/클릭 잠금
                //ui.SetActive(true); // 게임 종료 UI 활성화
            }
        }
    }
}

