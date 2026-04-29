using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FC_Spawner : MonoBehaviour
{
    public static int floor;

    [SerializeField] GameObject[] chessObjects; //체스 프리팹
    [SerializeField] Transform[] spawnPos; //스폰 될 위치 

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ChessSpawn());
    }

   IEnumerator ChessSpawn()
    {
        while (true) 
        {
            yield return new WaitForSeconds(0.5f);

            int chess = Random.Range(0, chessObjects.Length);

            int spawnMin = (floor - 1) * 3;  // floor에 따라 시작 인덱스 설정
            int spawnMax = spawnMin + 6;     // floor에 따라 끝 인덱스 설정

            int spawn = Random.Range(spawnMin, spawnMax);

            if (spawnPos[spawn].childCount == 0)
            {
                Instantiate(chessObjects[chess], spawnPos[spawn]);
            }
        }
    }

}
