using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//결과 창이 처음 활성화 되면 실행될 함수
public class ResultInitNFCPlayers : MonoBehaviour {
    /* NFC 관련 주석
    //플레이어 이름들..
    [SerializeField]
    InputPlayerName_NFC name_NFC;
    */
    [SerializeField]
    GameObject deactive;
	void OnEnable()
    {
        //이름 초기화!! NPC
        //name_NFC?.Init();
        //
        //진행중인 게임 오브젝트 비활성화
        deactive.SetActive(false);
        StartCoroutine(ResultRing());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
    //등장시 애니메이션 효과를 넣자!!
    IEnumerator ResultRing()
    {
        if (transform.position.y < -333)
        {
            while (true)
            {
                transform.position.Set(0, 0, 0);
                transform.Translate(Vector3.down);

                yield return new WaitForSeconds(0.1f);
            }
        }
        
        
        yield return new WaitForSeconds(0.5f);
    }
}
