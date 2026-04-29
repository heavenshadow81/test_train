using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Crab
{
    public class CrabSpawner : MonoBehaviour
    {
        public GameObject crabHouse; //크랩의 부모가될 오브젝트
        public GameObject[] CrabSpawn; //크랩 스폰 위치 오브젝트
        public CrabMove Crab; //크랩 오브젝트

        public static int crabCount; //현재 있는 크랩의 수를 계산할 변수

        int count = 0;

        public List<CrabMove> CrabList = new List<CrabMove>();
        // Update is called once per frame
        void OnEnable()
        {
            //crabCount = 0; //시작 할 때 크랩카운트는 0
            //StartCoroutine(SpawnEnemy()); //게임이 시작되면 크랩 스폰 코루틴 활성화
            GameManager.CrabDieEvent += RemoveCrab;
        }


        private void OnDisable()
        {
            GameManager.CrabDieEvent -= RemoveCrab;
        }

        public void Spawner()
        {
            crabCount = 0; //시작 할 때 크랩카운트는 0
            StartCoroutine(SpawnEnemy()); //게임이 시작되면 크랩 스폰 코루틴 활성화
        }

        public void RemoveCrab(CrabMove crab)
        { 
            CrabList.Remove(crab);
        }


        IEnumerator SpawnEnemy() //장애물스폰 코루틴
        {
            while (true) //무조건
            {    
                int num = UnityEngine.Random.Range(0, CrabSpawn.Length); //크랩 스폰 위치에 대한 랜덤 변수
                if(CrabSpawn[num].transform.childCount < 1 && crabCount <= 10)  //크랩 스폰 위치가 비어 있고 크랩의 수가 10 이하라면
                {
                    print(count);
                    count ++;
                    CrabMove crab = Instantiate(Crab, CrabSpawn[num].transform.localPosition, Quaternion.identity); //크랩을 랜덤한 위치에 생성
                    crab.transform.SetParent(crabHouse.transform); //크랩 생성 위치를 크랩하우스 자식으로 설정
                    crab.gameObject.GetSpriteRenderer().sortingOrder = 3 + count;
                    crabCount++; //크랩의 수 증가
                   // CrabList.Add(crab);
                }
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 0.8f)); //0.5초~0.8초 간격을 둠
            }
        }
    }
}
