using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dolphin 
{ 
    public class UnCircle : MonoBehaviour
    {
        void OnTriggerEnter(Collider other) // other와 물체가 부딪혔을 때
        {
            if (other.gameObject.CompareTag("Player")) //other의 태그가 Player라면
            {
                DGameManager.fishLife -= 1; //피쉬라이프 값을 -1
                GameObject.Find("SoundManager").GetComponent<DolphinSound>().BumpSound(); //범프 사운드 재생
            }
        }
    }
}

