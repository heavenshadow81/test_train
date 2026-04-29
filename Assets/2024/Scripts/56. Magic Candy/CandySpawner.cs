using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandySpawner : MonoBehaviour
{
    [SerializeField] Transform[] creatPosition; //캔디가 생성될 위치
    [SerializeField] Transform[] candies; //생성될 캔디 프리팹들
    private List<Transform> availablePositions; // 사용 가능한 위치 목록

    // Start is called before the first frame update
    void Start()
    {
        availablePositions = new List<Transform>(creatPosition); // 위치 배열을 리스트로 변환
        StartCoroutine(CandySpawn());
    }

    IEnumerator CandySpawn()
    {
        while (true) //무한 반복
        {
            // 사용 가능한 위치가 없으면 위치 리스트를 초기화
            if (availablePositions.Count == 0)
            {
                availablePositions = new List<Transform>(creatPosition);
            }

            int randomeCandy = Random.Range(0, candies.Length); // 랜덤한 캔디 모양
            int randomePos = Random.Range(0, availablePositions.Count); // 랜덤한 위치

            // 캔디 생성 후 위치를 사용 가능한 위치 목록에서 제거
            if (creatPosition[randomePos].childCount == 0)
            {
                Instantiate(candies[randomeCandy], creatPosition[randomePos].transform);
                availablePositions.RemoveAt(randomePos);
            }

            yield return new WaitForSeconds(1.5f); //1.5초 기다림
        }
    }
}
