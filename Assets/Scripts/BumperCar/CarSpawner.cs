using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BumperCar
{
    public class CarSpawner : MonoBehaviour
    {
        public GameObject[] enemy; //장애물 프리팹 배열
        public GameObject[] enemySpawner; //장애물 스폰 위치 배열

       public void OnEnable()
        {
            StartCoroutine(SpawnEnemy()); //게임이 시작되면 장애물스폰 코루틴 활성화
        }

        IEnumerator SpawnEnemy() //장애물스폰 코루틴
        {
            while (true) //무조건
            {
                int rnd = Random.Range(0, enemySpawner.Length);
                //장애물 프리팹 배열 중에 랜덤으로 골라서 장애물 스폰 배열 중에 랜덤으로 장애물 스폰
                var e = Instantiate(enemy[Random.Range(0, enemy.Length)], enemySpawner[rnd].transform);
                e.transform.position = new Vector3(e.transform.position.x, enemySpawner[rnd].transform.position.y, 0);
                yield return new WaitForSeconds(Random.Range(1.5f, 2.5f)); //1.5초~2.5초 간격을 둠
            }
        }
    }
}
