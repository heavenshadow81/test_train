using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

namespace UnderTheSea
{
    public class FishSpawner : MonoBehaviour
    {
        public GameObject[] fishSpawn; //피쉬 스폰 위치 오브젝트
        public GameObject[] fish; //피쉬 오브젝트 배열
        GameObject fishes; //생성된 물고기 오브젝트 

        int fishCount; //생성된 피쉬의 수를 계산할 변수

        public static bool quiz; //퀴즈 활성화 값

        public static List<int> fishList = new List<int>(); //생성할 물고기 숫자와 오답처리될 숫자 

        // Update is called once per frame
        void OnEnable()
        {
            fishList.Clear(); //피쉬리스트 초기화
            fishRandom(); //피쉬랜덤 함수 실행

            quiz = false; //시작할 때 퀴즈 비활성화
            fishCount = 0; //생성된 피쉬 카운트 초기화
        }
        public void fishRandom() //생성할 물고기 숫자 랜덤지정 함수
        {
            int currentNumber = UnityEngine.Random.Range(7, 20); //현재 숫자를 7~20사이로 지정

            for (int i = 0; i < 3; ) //3회 반복
            {
                if (fishList.Contains(currentNumber)) //현재 숫자가 피쉬리스트안에 있다면
                {
                    currentNumber = UnityEngine.Random.Range(7, 20); //다시 랜덤
                }
                else
                {
                    fishList.Add(currentNumber); //숫자를 리스트에 저장
                    i++; //반복횟수 개수
                }
            }
        }
        IEnumerator FishSpawn() //피쉬스폰 코루틴
        {
            while (true) //무조건
            {
                if (fishList[0] > fishCount && !quiz) // 생성할 물고기 숫자가 현재 생성된 물고기 보다 적고 퀴즈가 비활성화 라면 
                {
                    int num = UnityEngine.Random.Range(0, fishSpawn.Length); //피쉬 스폰 위치에 대한 랜덤 변수
                    int fishNum = UnityEngine.Random.Range(0, fish.Length); //물고기 종류에 대한 랜덤 변수

                    fishes = Instantiate(fish[fishNum], fishSpawn[num].transform); //피쉬를 랜덤 위치에 생성
                    fishCount++; //피쉬의 수 증가

                    fishes.GetSpriteRenderer().sortingOrder = 5+fishCount;

                }
                else if (fishList[0] == fishCount) //생성할 물고기 숫자와 현재 생성된 물고기 숫자가 같다면
                {
                    quiz = true; //퀴즈 활성화
                }

                yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 2f)); //1초~2초 간격을 둠
            }
        }

        public void GameStart() //게임시작 함수
        {
            StartCoroutine(FishSpawn()); //게임이 시작되면 피쉬스폰 코루틴 활성화
        }
    }
}
