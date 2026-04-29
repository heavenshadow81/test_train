using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ML.T_Sports.Running
{
    public class RunningTouchPoint : MonoBehaviour
    {
        public int Player;
        // Use this for initialization
        void Start()
        {
            Player = 1;
        }

        // Update is called once per frame
        void Update()
        {
            int TouchCount = TouchModule.TouchModuleInput.touchCount;
            if (TouchCount != 0)
            {
                Touch[] touchs = TouchModule.TouchModuleInput.touches;
                for (int i = 0; i < touchs.Length; i++)
                {
                    if (touchs[i].phase != TouchPhase.Began) continue;
                    Vector3 pos = touchs[i].position;
                    float areaA = 1920 / Player;
                    float areaB = areaA * 2;
                    float areaC = areaA * 3;
                    float areaD = 1920;
                    int idx = 0;
                    if (pos.x < areaA)
                    {
                        idx = 0;
                    }
                    else if (pos.x >= areaA && pos.x < areaB)
                    {
                        idx = 1;
                    }
                    else if (pos.x >= areaB && pos.x < areaC)
                    {
                        idx = 2;
                    }
                    else if (pos.x >= areaC && pos.x < areaD)
                    {
                        idx = 3;
                    }

                    //게임중이라면 각 터치 영역 idx 에게 진행할 내용.
                    if(RunningManager.instance.state == RunningState.Play)
                    {
                        RunningManager.instance.Scores[idx].RecodingTime();
                    }
                }
            }
        }
        
    }
}
