using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkiSpawner : MonoBehaviour
{
    public GameObject[] enemy; //에너미 배열
    public GameObject[] enemySpawner; //에너미 소환위치 배열
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(SpawnEnemy()); //스폰에너미 코루틴 활성화
    }

    IEnumerator SpawnEnemy() //스폰에너미 코루틴
    {
        while (true) //무조건
        {
            int num = Random.Range(0, enemySpawner.Length); //0~에너미 스포너 길이 중에 랜덤으로 

           var e =  Instantiate(enemy[Random.Range(0, enemy.Length)], enemySpawner[num].transform); //에너미 랜덤하게 생성
            e.transform.position = new Vector3(e.transform.position.x, enemySpawner[num].transform.position.y, 0);

            yield return new WaitForSeconds(Random.Range(1.5f, 2.5f)); //1.5~2.5초 대기
        }
    }
}

