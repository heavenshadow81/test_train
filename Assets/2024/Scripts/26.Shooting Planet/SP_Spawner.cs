using DG.Tweening;
using ML.PlaywallKids.TwoDimensionDrawScene;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SP_Spawner : MonoBehaviour
{
    int timer = 60;
    [SerializeField] TextMeshProUGUI timerText; //타이머 텍스트
    [SerializeField] GameObject timeOver; //타임오버 오브젝트

    [SerializeField] GameObject[] spawnPosition; //스포너 위치
    [SerializeField] GameObject[] planets; //행성 프리팹들

    void OnEnable()
    {
        timer = 60;
        PlanetSpawn();
    }
    void PlanetSpawn()
    {
        if (timer > 0)
        {
            //타이머 텍스트 현재 남은 시간 표시
            timerText.text = timer.ToString();
            timer--;

            int rand = Random.Range(0, spawnPosition.Length);
            Vector3 pos = spawnPosition[rand].transform.position;

            //랜덤한 위치에 랜덤한 에일리언 생성
            GameObject enemy = Instantiate(planets[Random.Range(0, planets.Length)], spawnPosition[rand].transform);

            //생성되는 위치에 따라 다른방향으로 이동
            if (rand > 3)
            {
                //2초 뒤에 10초동안 왼쪽으로 이동
                enemy.transform.DOMoveX(-40, 15f).SetDelay(1).OnComplete(()=>
                {
                    //행성 삭제
                    Destroy(enemy);
                });
            }
            else
                //2초 뒤에 10초동안 오른쪽으로 이동
                enemy.transform.DOMoveX(40, 15f).SetDelay(2).OnComplete(() =>
                {
                    //행성 삭제
                    Destroy(enemy);
                });


            Invoke("PlanetSpawn", 1);
        }
        else
        {
            SoundMGR.Instance.SoundPlay("PlayGround_WrongAnswer");
            timeOver.SetActive(true);

            for(int i = 0; i < spawnPosition.Length; i++)
            {
                spawnPosition[i].SetActive(false);
            }
        }
        
    }
}
