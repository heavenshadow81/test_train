using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class DishManager : MonoBehaviour
{
    [SerializeField] GameObject[] starParticle; //파티클 프리팹
    [SerializeField] GameObject[] playObj; //플레이에 필요한 오브젝트들
    [SerializeField] Transform candyPos; //캔디 소환될 위치
    Vector3 originPos; //처음 접시 위치

    [SerializeField] TextMeshProUGUI[] scoreText; //스코어 텍스트
    [SerializeField] Transform[] cameraPosition; //카메라 이동 위치

    [SerializeField] MagicTimer timer; //타이머 스크립트

    int score = 0; //받은 캔디 수


    private void OnEnable()
    {
        originPos = transform.position; //접시 처음위치 저장
        timer.OnTimerEnd += EndGame;
    }

    private void OnDisable()
    {
        timer.OnTimerEnd -= EndGame;
    }


    private void OnCollisionEnter(Collision collision)
    {
        GameObject candy = collision.gameObject;

        //캔디 오브젝트와 충돌하면
        if (collision.transform.tag == "Point")
        {
            //파티클 생성 후 삭제
            GameObject particle = Instantiate(starParticle[0], collision.transform.position, Quaternion.identity);
            Destroy(particle, 2f);

            //떨어지는 캔디 오브젝트 삭제
            Destroy(collision.gameObject);

            ScoreCheck(); //스코어 체크

            for (int i = 0; i < candyPos.childCount; i++)
            {
                if (candyPos.GetChild(i).childCount == 0)
                {
                    //캔디 오브젝트 박스콜라이더, 중력 비활성화 및 접시위에 생성
                    candy.GetComponent<BoxCollider>().enabled = false;
                    candy.GetComponent<Rigidbody>().useGravity = false;
                    candy.transform.GetChild(0).GetComponent<DOTweenAnimation>().autoPlay = false;
                    GameObject realCandy = Instantiate(candy, candyPos.GetChild(i).transform.position,
                    Quaternion.Euler(Random.Range(0f, 30f), Random.Range(0f, 30f), Random.Range(0f, 30f))); // X, Y, Z 축에 랜덤 회전);
                    realCandy.transform.parent = candyPos.GetChild(i).transform;

                    break;
                }
            }
        }
    }

    void ScoreCheck()
    {
        //스코어 상승 후 표기
        score++;
        scoreText[0].text = score + "   Candies";
        scoreText[1].text = score + "  Candies";
    }

    public void StartAnimation()
    {
        SoundMGR.Instance.SoundPlay("smile");

        timer.PauseTimer();

        //메인카메라 변수 저장
        Camera cam = Camera.main;

        //카메라 이동 후 캔디스폰 및 클릭 스크립트 활성화
        cam.transform.DOMove(cameraPosition[0].position,3f).SetDelay(2f).OnStart(()=> SoundMGR.Instance.SoundPlay("CameraMove"));
        cam.transform.DORotateQuaternion(cameraPosition[0].rotation, 3f).SetDelay(2f).OnComplete(()=>
        {
            playObj[0].SetActive(true);
            playObj[1].SetActive(true);

            timer.ResumeTimer();
        });
    }

    void EndGame()
    {
        //타이머가 종료되면 플레이에 필요한 오브젝트 비활성화
        playObj[0].SetActive(false);
        playObj[1].SetActive(false);
        scoreText[0].transform.parent.gameObject.SetActive(false);

        Camera cam = Camera.main;

        //접시 가운데 위치로 이동
        gameObject.transform.DOMove(originPos, 1f);

        SoundMGR.Instance.SoundPlay("CameraMove");

        cam.transform.DOMove(cameraPosition[1].position, 2f);
        cam.transform.DORotateQuaternion(cameraPosition[1].rotation, 2f).OnComplete(() =>
        {
            //접시 뚜껑 오브젝트 활성화 및 파티클 생성
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            Invoke("ParticleParty", 1f);
            SoundMGR.Instance.SoundPlay("샤라랑");
        });
    }

    void ParticleParty()
    {
        //파티클 생성 후 삭제
        Instantiate(starParticle[1], gameObject.transform.GetChild(1).transform.position, Quaternion.identity);
    }
}
