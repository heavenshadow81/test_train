using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BalloonMove : MonoBehaviour
{
    [SerializeField] List<Transform> balloonPositions; //이동할 위치 리스트
    [SerializeField] GameObject[] balloons; //열기구
    List<Transform> availablePositions; //이동 가능한 포지션
    [SerializeField] FA_GameManager gameManager; //게임매니저 스크립트

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveBalloonsRandomly());
    }

    IEnumerator MoveBalloonsRandomly()
    {
        int count = 0; //반복횟수 지정

        while (count<5)
        {
            count++; //횟수 증가
            availablePositions = new List<Transform>(balloonPositions); //포지션 초기화

            foreach (GameObject balloon in balloons)
            {
                if (availablePositions.Count > 0)
                {
                    //랜덤으로 포지션 선택
                    int randomIndex = Random.Range(0, availablePositions.Count);
                    Transform randomPosition = availablePositions[randomIndex];

                    int n = Random.Range(1, 5); //어느면을 보여줄지 랜덤하게 결정
                    if (count==5)
                    {
                        n = 1; //마지막 순서에는 무조건 동물이 안보이게
                    }   

                    //열기구를 무작위 포지션으로 이동, 회전
                    balloon.transform.DOMove(randomPosition.position, 2);
                    balloon.transform.DORotateQuaternion(Quaternion.Euler(0, 180 * n, 0), 2f);

                    // 사용된 포지션은 리스트에서 제거
                    availablePositions.RemoveAt(randomIndex);
                }
            }

            //사운드 재생
            SoundMGR.Instance.SoundPlay("섞기");

            //2초 대기 후 다시 이동
            yield return new WaitForSeconds(2f);    
        }

        gameManager.SetQuiz();
    }

    public IEnumerator ReGame()
    {
        yield return new WaitForSeconds(1f);

        //사운드 재생
        SoundMGR.Instance.SoundPlay("섞기");

        //전체 정면 보여주고 다시 섞기 시작
        foreach (GameObject balloon in balloons)
        {
            //박스콜라이더 활성화
            balloon.transform.GetComponent<BoxCollider>().enabled = true;

            balloon.transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), 2f).OnComplete(() =>
            {
                DOVirtual.DelayedCall(1f, () =>
                {
                    StartCoroutine(MoveBalloonsRandomly());
                });
            });
        }    
    }
}
