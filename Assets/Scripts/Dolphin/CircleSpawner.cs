using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dolphin
{
    public class CircleSpawner : MonoBehaviour
    {
        public GameObject[] enemy; //장애물 오브젝트 배열
        public GameObject[] enemySpawner; //장애물 스폰 위치 오브젝트 배열
        // Start is called before the first frame update


        public void CircleInit()
        {
            StartCoroutine(SpawnEnemy()); //장애물 스폰 코루틴 활성화
        }

        

        IEnumerator SpawnEnemy() //장애물 스폰 코루틴
        {
            while (true) //무조건
            {
                int num = Random.Range(0, enemySpawner.Length); //num 변수 랜덤하게 생성

                //장애물 배열에서 랜덤으로 장애물 스폰 위치 중 랜덤하게 장애물 생성
                Instantiate(enemy[Random.Range(0, enemy.Length)], enemySpawner[num].transform);

                yield return new WaitForSeconds(Random.Range(2f, 3f)); //2초에서 3초 간격을 둠
            }
        }
    }
}

