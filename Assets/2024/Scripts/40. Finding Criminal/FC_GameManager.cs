using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;
using TMPro;

public class FC_GameManager : TouchManager_3DTouch
{
    int score = 0; //점수
    [SerializeField] TextMeshProUGUI scoreText; //점수텍스트
    [SerializeField] Animator buildingAnim; //빌딩 애니메이션

    [SerializeField] GameObject spawner; //스포너 오브젝트
    [SerializeField] GameObject[] endObj; //엔딩 오브젝트

    [SerializeField] MagicLife magicUI; //매직랜드 UI 스크립트
    bool touchOff; //터치온오프

    private void OnEnable()
    {
        touchOff = true;
        FC_Spawner.floor = 1;
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
            if (!touchOff)
            {
                if (hit.collider.tag == "Enemy")
                {
                    //파티클이 생성될 미세한 위치 조정
                    Vector3 enemyPos = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y + 4f, 90f);

                    //파티클 생성 후 오브젝트삭제
                    GameObject particle = Instantiate(effect[0], enemyPos, Quaternion.identity);
                    Destroy(particle, 2f);
                    Destroy(hit.collider.gameObject);

                    //스코어 상승 함수
                    ScoreUp();

                    SoundMGR.Instance.SoundPlay("으악");
                    SoundMGR.Instance.SoundPlay("펑");
                }
                else if (hit.collider.tag == "Doll")
                {
                    //체스 오브젝트의 화남 애니메이션 재생
                    Animator anim = hit.collider.gameObject.GetComponent<Animator>();
                    anim.SetTrigger("Angry");
                    SoundMGR.Instance.SoundPlay("띠융");

                    //라이프 줄어드는 함수 실행
                    magicUI.LifeDelete();

                    //콜라이더 비활성화
                    hit.transform.GetComponent<BoxCollider>().enabled = false;
                }
            }
        }
    }

    void ScoreUp()
    {   
        //점수 상승 후 텍스트 표기
        score += 1;
        scoreText.text = $"{score} / 30";

        //스코어에 따라 빌딩 움직이는 애니메이션 재생
        if (score==10)
        {
            buildingAnim.SetTrigger("2F");
            //애니메이션 재생중일 때 터치불가
            touchOff = true;

            //층 수 변경
            FC_Spawner.floor = 2;

            SoundMGR.Instance.SoundPlay("데구르르");
        }
        else if(score==20)
        {
            buildingAnim.SetTrigger("3F");
            touchOff = true;

            //층 수 변경
            FC_Spawner.floor = 3;

            SoundMGR.Instance.SoundPlay("데구르르");
        }
        else if(score>=30)
        {
            //스포너중지 및 엔딩팝업 활성화
            spawner.SetActive(false);
            endObj[0].SetActive(true);
            SoundMGR.Instance.SoundPlay("win");
        }
    }

    public void TouchOn()
    {
        touchOff= false;
    }
}