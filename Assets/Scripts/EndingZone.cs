using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class EndingZone : MonoBehaviour
    {
        public ResultManager resultManager;
        public bool WinLossSet;
        private void Start()
        {
            WinLossSet = false;
        }
        public void OnTriggerEnter(Collider other)
        {
            if (!WinLossSet)
            {
                if (other.tag == "Player")
                {
                    Debug.Log("플레이어 승");
                    WinLossSet = true;
                    resultManager.SetWinLose(true);
                    this.gameObject.SetActive(false);
                }
                else if (other.tag == "AI")
                {
                    Debug.Log("플레이어 패");
                    WinLossSet = true;
                    resultManager.SetWinLose(false);
                    this.gameObject.SetActive(false);
                }
            }
            
        }
    }
}

