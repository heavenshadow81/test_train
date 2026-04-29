using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rabbit
{
    public class RabbitSpawner : MonoBehaviour
    { 
        public GameObject[] fruit; //열매 오브젝트 배열
        public GameObject[] fruitSpawner; //열매 스폰 위치 배열
        public Animator[] anims; //나뭇가지 애니메이션 배열
        public GameObject parents; //열매가 생성 될 부모 오브젝트

        public void Spawner()
        { 
            StartCoroutine(SpawnFruit()); //시작하면 열매 스폰 코루틴 실행
        }

        IEnumerator SpawnFruit()
        {
            while (true) //무조건
            {
                int num = Random.Range(0, fruitSpawner.Length); //열매 스폰위치 랜덤 변수

                for (int i = 0; i < fruitSpawner.Length; i++)
                {
                    if (num == i) //랜덤 변수와 i가 같다면
                    {
                        anims[i].SetTrigger($"stick{i}"); //i번째 나뭇가지 애니메이션 실행
                    }
                }
                yield return new WaitForSeconds(0.5f); //0.5초 뒤에

                //열매 중에 랜덤으로 열매 스폰위치 중에 부모 변수 자식으로 랜덤 생성 
                Instantiate(fruit[Random.Range(0, fruit.Length)], fruitSpawner[num].transform).transform.parent = parents.transform;

                yield return new WaitForSeconds(Random.Range(2, 3)); //2~3초 간격을 둠
            }
        }
    }
}


