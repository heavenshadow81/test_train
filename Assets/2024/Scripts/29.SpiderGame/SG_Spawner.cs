using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class SG_Spawner : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] GameObject[] arrivalPos; //도착 지점
    [SerializeField] GameObject[] spawnPosition; //스포너 위치

    [SerializeField] GameObject spider; //거미 프리팹
    [SerializeField] TextMeshProUGUI[] score; //점수 표기
    [SerializeField] TextMeshProUGUI scoreMinus; //점수마이너스 표기
    [SerializeField] GameObject textPos; //점수포지션
    public int timer = 30;

    void OnEnable()
    {
        timer = 30;
        Invoke("SpiderSpawn", 4);
    }

    void SpiderSpawn()
    {
        //타이머가 0보다 크다면 스파이더 스폰
        if (timer > 0)
        {
            //speed값 시간에 따라 랜덤하게
            int speed = Random.Range(4, 7);

            if (timer > 17)
            {
                speed = Random.Range(5, 9);
            }

            timer--;

            //속도 랜덤하게
            int time = Random.Range(4, 7);
            Vector3 pos = spawnPosition[Random.Range(0, spawnPosition.Length)].transform.position;

            //랜덤한 위치에 랜덤한 에일리언 생성
            GameObject enemy = Instantiate(spider, pos, Quaternion.identity);
            enemy.transform.DOMove(arrivalPos[Random.Range(0, arrivalPos.Length)].transform.position, speed).OnComplete(()=>
            {
                if(enemy.tag=="Orange")
                {
                    if (SG_Manger.orange > 0 & timer>0)
                    {
                        Score();
                        SG_Manger.orange--;
                    }
                }
                else if(enemy.tag =="Green")
                {
                    if (SG_Manger.green > 0 & timer > 0)
                    {
                        Score();
                        SG_Manger.green--;
                    }
                }

                score[0].text= SG_Manger.orange.ToString();
                score[1].text = SG_Manger.green.ToString();

                Destroy(enemy);
            });

            Invoke("SpiderSpawn",1);
        }
    }

    void Score()
    {
        float originPosX = scoreMinus.transform.position.x;

        scoreMinus.text = "- 1";
        scoreMinus.transform.DOMoveX(textPos.transform.position.x, 0.5f).SetEase(Ease.Linear).OnComplete(() =>

        {
            scoreMinus.text = "";
            scoreMinus.transform.DOMoveX(originPosX, 0);

        });
    }
}