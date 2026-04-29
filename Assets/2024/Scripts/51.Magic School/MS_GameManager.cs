using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;


public class MS_GameManager : TouchManager_3DTouch
{
    [SerializeField] Transform[] spawners; //스포너 포지션
    [SerializeField] GameObject stone; //매직스톤 프리팹
    [SerializeField] GameObject stoneParent; //스톤 부모 오브젝트
    List<GameObject> stones = new List<GameObject>(); //생성된 스톤 저장할 리스트

    int score; //점수 변수
    [SerializeField] TextMeshProUGUI scoreText; //점수 텍스트

    [SerializeField] Animator stoneSpawner; //스톤스포너 애니메이터

    [SerializeField] GameObject boss; //보스 오브젝트
    [SerializeField] Slider bossSlider; //보스 hp 슬라이더 UI
    int bossHP = 30; //보스 hp

    [SerializeField] MagicTimer timer; //타이머 스크립트

    private void OnEnable()
    {
        Invoke("StartStone", 2f);
    }


    public override void HandleInput(Vector2 pos)
    {
        isTouchable = true;
        // 마우스 또는 터치 입력을 사용하여 Ray 생성
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hit;

        // Raycast로 오브젝트 감지
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null) //콜라이더가 존재하고 터치값이 true일때
            {
                SoundMGR.Instance.SoundPlay("fireball");

                if (hit.collider.tag == "Enemy") //레이에 맞은 콜라이더의 태그가 Enemy라면
                {
                    //게임매니저 위치에 파이어 파티클 생성
                    GameObject fire = Instantiate(effect[0], gameObject.transform.position, Quaternion.identity);

                    //박스 콜라이더 비활성화
                    hit.collider.transform.GetComponent<BoxCollider>().enabled = false;

                    //타겟을 따라가는 코루틴 실행
                    StartCoroutine(MoveParticle(fire, hit.transform));

                    SpawnStone();

                    ScoreResult();

                    
                }
                else if(hit.collider.tag =="Finish")
                {
                    //게임매니저 위치에 파이어 파티클 생성
                    GameObject fire = Instantiate(effect[0], gameObject.transform.position, Quaternion.identity);

                    //타겟을 따라가는 함수 실행
                    BossParticle(fire, hit.transform);

                    //박스 콜라이더 비활성화 후 클릭시 좌우 흔들린 후 다시 활성화
                    hit.collider.transform.GetComponent<BoxCollider>().enabled = false;

                    StartCoroutine(BossHP()); //보스 hp 감소 및 효과
                }
            }
            else
            {
                //print("콜라이더 없음");
            }
        }
    }

   IEnumerator MoveParticle(GameObject particle, Transform target)
    {
        while (particle != null && target != null)
        {
            //매 프레임마다 타겟의 위치로 이동
            particle.transform.DOMove(target.position, 0.6f).OnComplete(() =>
            {
                //fire 파티클 및 타겟 삭제 후 터지는 파티클 생성
                Destroy(particle);
                Destroy(target.gameObject);
                GameObject fire = Instantiate(effect[1], target.position, Quaternion.identity);
                Destroy(fire,2f);

                SoundMGR.Instance.SoundPlay("brokenStone");
            });

            //짧은 시간 기다린 후 다시 실행
            yield return null;
        }
    }

    void BossParticle(GameObject particle, Transform target)
    {
            //매 프레임마다 타겟의 위치로 이동
            particle.transform.DOMove(target.position, 0.3f).OnComplete(() =>
            {
                //fire 파티클 및 타겟 삭제 후 터지는 파티클 생성
                Destroy(particle);
                GameObject fire = Instantiate(effect[2], target.position, Quaternion.identity);
                Destroy(fire, 2f);

                SoundMGR.Instance.SoundPlay("brokenStone");

                //자식 오브젝트 흔들림
                target.GetChild(0).transform.DOLocalMoveY(-21f, 0.1f).SetLoops(2, LoopType.Yoyo).OnComplete(()=>
                {
                    target.GetComponent<BoxCollider>().enabled = true; //타겟 박스콜라이더 활성화
                });
            });
    }

    void StartStone()
    {
        //스포너에 스톤 생성
        for (int i = 0; i < spawners.Length; i++)
        {
            GameObject magicstone = Instantiate(stone, spawners[i].position, Quaternion.identity);
            magicstone.transform.parent = stoneParent.transform;
            stones.Add(magicstone);
        }

        CancelInvoke("StartStone");
    }

    void SpawnStone()
    {
        //랜덤한 스포너 위치에 스톤 생성
        int random = Random.Range(0, spawners.Length);

        GameObject magicstone = Instantiate(stone, spawners[random].position, Quaternion.identity);
        magicstone.transform.parent = stoneParent.transform;
        stones.Add(magicstone);

        //생성될때 파티클 생성
        GameObject fire = Instantiate(effect[2], spawners[random].position, Quaternion.identity);
        Destroy(fire, 2f);
    }

    void ScoreResult()
    {
        //스코어 상승 후 텍스트에 반영
        score++;
        scoreText.text = score + " / 20";

        if (score > 19)
        {
            StartCoroutine(BossStage());
        }
    }

    IEnumerator BossStage()
    {
        SoundMGR.Instance.SoundPlay("brokenStone");

        //스포너 사라지는 애니메이션 재생
        stoneSpawner.SetTrigger("Close");
        for(int i = 0; i < stones.Count; i++)
        {
            if (stones[i] != null)
            {
                //남은 스톤 오브젝트 위치에 파티클 터지고 삭제
                GameObject fire = Instantiate(effect[1], stones[i].transform.position, Quaternion.identity);
                Destroy(fire, 2f);
                Destroy(stones[i]);
            }
        }
        //2초 뒤에 카메라 흔들리고 보스스톤 등장
        yield return new WaitForSeconds(2.0f);

        SoundMGR.Instance.SoundPlay("collapse");

        gameCanvas.SetActive(false); //점수판 삭제

        Camera.main.transform.DOMoveX(1,0.2f).SetLoops(10,LoopType.Yoyo).OnComplete(()=>
        {
            boss.transform.GetComponent<BoxCollider>().enabled = true; //박스 콜라이더 활성화
        });
        boss.SetActive(true);
    }

    IEnumerator BossHP()
    {
        //hp감소 및 표기
        bossHP--;
        bossSlider.value = bossHP;

        if(bossHP <= 0)
        {
            timer.PauseTimer(); //타이머 멈춤

            //생성될때 파티클 생성
            GameObject fire = Instantiate(effect[1], boss.transform.position, Quaternion.identity);
            Destroy(fire, 2f);

            //보스 삭제 및 승리 팝업 표시
            boss.SetActive(false);

            yield return new WaitForSeconds(1f);
            victoryUI.SetActive(true);
        }
        else if(bossHP <= 10)
        {
            //HP바 색상 변경
            bossSlider.fillRect.GetComponent<Image>().color = Color.red;
        }
        else if (bossHP <= 20)
        {
            //HP바 색상 변경
            bossSlider.fillRect.GetComponent<Image>().color = Color.white;
        }
    }
}
