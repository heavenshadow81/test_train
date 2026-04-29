using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dolphin
{
    public class DolphinFish : MonoBehaviour
    {
        public GameObject Bombeffect; //생성할 프리랩 오브젝트

        void OnTriggerEnter(Collider other) //부딪힌 물체 other 감지
        {
            if (other.gameObject.tag == "Player") //other의 태그가 Player라면
            {
                GameObject.Find("SoundManager").GetComponent<DolphinSound>().CollectSound(); //콜렉트 사운드 재생
                Instantiate(Bombeffect,new Vector3(0,3.2f,0), transform.rotation).transform.parent = transform.parent; //현재 위치에 프리팹 생성
                DGameManager.fishLife += 1; //피쉬 라이프 +1
                Destroy(gameObject); //나 자신 삭제
            }
        }
    }
}
