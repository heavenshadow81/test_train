using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StairGame
{
    public class StairSpawner : MonoBehaviour
    {
        [SerializeField] GameObject[] stairs = null;
        Vector3 firstOffSet = new Vector3(-11.04f, 0.9f, 6.38f);
        Vector3 stairOffSet = new Vector3(-2.20f, 0.9f, 1.26f);
        Vector3 baseOffSet = new Vector3(-2.20f, 0.93f, 1.26f);

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Player player = other.GetComponent<Player>();

                player.StopJumpAnim();

                player.transform.position = transform.position;

                int randNum;
                do
                {
                    randNum = Random.Range(1, 7);
                } while (randNum == GameManager.Instance.StairCount);

                GameManager.Instance.StairCount = randNum;

                SpawnStair(GameManager.Instance.StairCount, 1f);
            }
        }


        private void SpawnStair(int stairCount, float delayTime)
        {
            StartCoroutine(SpawnStairWithDelay(stairCount, delayTime));
        }

        private IEnumerator SpawnStairWithDelay(int stairCount, float delayTime)
        {
            Vector3 prevObjectPos = transform.parent.position;
            GameObject spawnObject;

            if(GameManager.Instance.StageCount < GameManager.Instance.maxStageCount)
            {
                if (stairCount == 1)
                {
                    // 계단이 하나인 경우
                    spawnObject = Instantiate(stairs[1], prevObjectPos + firstOffSet, stairs[1].transform.rotation);
                    GameManager.Instance.SetTouchEnable(true);
                }
                else
                {
                    // 첫 번째 계단
                    spawnObject = Instantiate(stairs[0], prevObjectPos + firstOffSet, stairs[0].transform.rotation);
                    prevObjectPos = spawnObject.transform.position;

                    // 중간 계단들
                    for (int i = 1; i < stairCount - 1; i++)
                    {
                        yield return new WaitForSeconds(delayTime); // 1초 대기
                        spawnObject = Instantiate(stairs[0], prevObjectPos + stairOffSet, stairs[0].transform.rotation);
                        prevObjectPos = spawnObject.transform.position;
                    }

                    // 마지막 계단
                    yield return new WaitForSeconds(delayTime); // 1초 대기
                    Instantiate(stairs[1], prevObjectPos + baseOffSet, stairs[1].transform.rotation);
                    GameManager.Instance.SetTouchEnable(true);
                }
            }
            else
            {
                if (stairCount == 1)
                {
                    // 계단이 하나인 경우
                    spawnObject = Instantiate(stairs[2], prevObjectPos + firstOffSet, stairs[2].transform.rotation);
                    GameManager.Instance.SetTouchEnable(true);
                }
                else
                {
                    // 첫 번째 계단
                    spawnObject = Instantiate(stairs[0], prevObjectPos + firstOffSet, stairs[0].transform.rotation);
                    prevObjectPos = spawnObject.transform.position;

                    // 중간 계단들
                    for (int i = 1; i < stairCount - 1; i++)
                    {
                        yield return new WaitForSeconds(delayTime); // 1초 대기
                        spawnObject = Instantiate(stairs[0], prevObjectPos + stairOffSet, stairs[0].transform.rotation);
                        prevObjectPos = spawnObject.transform.position;
                    }

                    // 마지막 계단
                    yield return new WaitForSeconds(delayTime); // 1초 대기
                    Instantiate(stairs[2], prevObjectPos + baseOffSet, stairs[2].transform.rotation);
                    GameManager.Instance.SetTouchEnable(true);
                }
            }     
        }
    }
}
