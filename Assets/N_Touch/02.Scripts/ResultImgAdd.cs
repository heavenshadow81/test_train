using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultImgAdd : MonoBehaviour
{
    [SerializeField]
    GameObject gameUi;
    void OnEnable()
    {
        //진행중인 게임 오브젝트 비활성화
        gameUi.SetActive(false);
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
