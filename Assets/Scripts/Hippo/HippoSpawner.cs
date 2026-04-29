using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Hippo
{
    public class HippoSpawner : MonoBehaviour
    {
        public GameObject[] HippoSpawn; //하마 스폰 위치 오브젝트
        public GameObject Hippo; //하마 오브젝트

        public static int hippoCount; //현재 있는 하마의 수를 계산할 변수

        // Update is called once per frame
        void OnEnable()
        {
            hippoCount = 0;
            StartCoroutine(SpawnEnemy()); //게임이 시작되면 하마스폰 코루틴 활성화
        }

        private void Update()
        {
            //print($"hippoCount:{hippoCount} / childCount{HippoSpawn[5].transform.childCount}");
        }

        IEnumerator SpawnEnemy() //장애물스폰 코루틴
        {
            while (true) //무조건
            {    
                int num = UnityEngine.Random.Range(0, HippoSpawn.Length); //하마 스폰 위치에 대한 랜덤 변수
                if( HippoSpawn[num].transform.childCount < 1 && hippoCount <= 10)  //하마 스폰 위치가 비어 있고 하마의 수가 10 이하라면
                {
                    Instantiate(Hippo, HippoSpawn[num].transform); //하마를 랜덤 위치에 생성
                    hippoCount++; //하마의 수 증가
                }
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 0.8f)); //0.5초~0.8초 간격을 둠
            }
        }
    }
}
