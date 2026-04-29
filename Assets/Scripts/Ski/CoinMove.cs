using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CoinMove : MonoBehaviour
{
    public GameObject Bombeffect; //부딪혔을 때 소환 될 이펙트
    float speed; //속도

    void Update()
    {
        if (!SGameManager.stageOne && !SGameManager.stageTwo && !SGameManager.stageThree) //스테이지가 모두 비활성화 라면
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime); //앞으로 이동
            speed = 10; //속도 10
        }
        else if (SGameManager.stageOne) //스테이지 1이 활성화 되어있다면
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime); //앞으로 이동
            speed = 12; //속도 12
        }
        else if (SGameManager.stageTwo) //스테이지 2가 활성화 되어있다면
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime); //앞으로 이동
            speed = 15; //속도 15
        }
        else if (SGameManager.stageThree) //스테이지 3이 활성화 되어있다면
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime); //앞으로 이동
            speed = 18; //속도 18
        }
    }

    void OnTriggerEnter(Collider other) //other에 부딪혔을 때
    {
        if (other.gameObject.tag == "Player") //other의 태그가 Player라면

        {
            GameObject.Find("SoundManager").GetComponent<SkiSound>().CorrectSound(); //코렉트 사운드 재생
            SGameManager.Score = SGameManager.Score + 100; //스코어 +100점 
            Instantiate(Bombeffect, transform.position, transform.rotation); //부딪힌 위치에 이펙트 생성
        }
        Destroy(gameObject); //나 자신 파괴
    }

}

