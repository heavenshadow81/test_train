using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;


public class SSG_GameManager : TouchManager_3DTouch
{
    [SerializeField] Transform portal; //포탈 오브젝트   
    [SerializeField] GameObject[] stoneQuiz; //문제로 나올 석상 프리팹들
    string correctName; //정답 석상이름

    [SerializeField] GameObject stones; //스톤부모 오브젝트
    
    [Header("UI")]
    [SerializeField] MagicLife life; //라이프 스크립트
    [SerializeField] Transform QuestionUI; //질문 UI
    [SerializeField] Transform ScoreUI; //스코어 UI
    [SerializeField] TextMeshProUGUI ScoreText; //스코어 UI
    int score; //정답맞힌 수



    private void OnEnable()
    {
        StartCoroutine(QuizTime());
    }
    public override void HandleInput(Vector2 pos)
    {   
        // 마우스 또는 터치 입력을 사용하여 Ray 생성
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hit;

        // Raycast로 오브젝트 감지
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null) //콜라이더가 존재하고 터치값이 true일때
            {

                if (hit.collider.tag == "target") //레이에 맞은 콜라이더의 태그가 target라면
                {
                   if(correctName.Contains(hit.collider.name))
                    {
                        //콜라이더 비활성화 및 정답, 점수 함수 실행
                        print("정답");
                        ScoreResult();
                        Correct(hit.collider.transform);
                        hit.collider.transform.GetComponent<BoxCollider>().enabled = false;
                        SoundMGR.Instance.SoundPlay("correct");
                    }
                   else
                    {
                        //콜라이더 비활성화 및 오답함수 실행
                        print("오답");
                        Wrong(hit.collider.transform);
                        hit.collider.transform.GetComponent<BoxCollider>().enabled = false;
                        SoundMGR.Instance.SoundPlay("wrong");

                        isTouchable = true;
                    }     
                }
            }
            else
            {
                isTouchable = true;
                //print("콜라이더 없음");
            }
        }
        else
        {
            isTouchable = true;
        }
    }

    IEnumerator QuizTime()
    {        
        yield return new WaitForSeconds(1f);
        //포탈 열리고 석상 보여주고 포탈 닫힘
        SoundMGR.Instance.SoundPlay("portal");
        portal.DOScaleX(1, 1);
        yield return new WaitForSeconds(1f);
        StoneQuizAnim();
        yield return new WaitForSeconds(7f);
        portal.DOScaleX(0, 1);
        SoundMGR.Instance.SoundPlay("portal");

        //보기 석상 제시됨
        yield return new WaitForSeconds(1f);
        stones.SetActive(true);
        QuestionUI.DOLocalMoveY(400, 1f);
        ScoreUI.DOLocalMoveY(650, 1f);
        stones.GetComponent<Animator>().SetBool("Disappear", false);
        SoundMGR.Instance.SoundPlay("데구르르");

        isTouchable = true;
    }

    void StoneQuizAnim()
    {
        //랜덤한 석상 프리팹 생성, 석상 이름 저장
        int random = Random.Range(0, stoneQuiz.Length);
        Transform stone = Instantiate(stoneQuiz[random]).transform;
        correctName = stone.name;

        //석상 포탈 앞으로 나왔다가 다시 들어가면 사라지는 애니메이션
        stone.DOLocalMoveZ(-3, 1).OnComplete(() =>
        {
            SoundMGR.Instance.SoundPlay("stoneAnim");

            stone.DOShakePosition(5, 5, 1, 20).SetEase(Ease.Linear);
            stone.DOShakeRotation(5, 180, 10,180).OnComplete(()=>
            {
                stone.DOLocalMoveZ(15, 1).OnComplete(() =>Destroy(stone.gameObject));
            });
        });
    }

    void Correct(Transform stone)
    {
        if (score < 5)
        {
            //애니메이션 해제 후 점프 애니메이션 재생
            stones.GetComponent<Animator>().enabled = false;
            stone.DOLocalMoveY(1f, 0.3f).SetLoops(4, LoopType.Yoyo);
            stone.DOScaleY(1.2f, 0.3f).SetLoops(4, LoopType.Yoyo).OnComplete(() =>
            {
                SoundMGR.Instance.SoundPlay("데구르르");
                stones.GetComponent<Animator>().enabled = true;
                stones.GetComponent<Animator>().SetBool("Disappear", true);
                ScoreUI.DOLocalMoveY(430, 1f);
                QuestionUI.DOLocalMoveY(650, 1f).OnComplete(() =>
                {
                    //퀴즈 코루틴 실행 및 박스콜라이더 활성화
                    StartCoroutine(QuizTime());
                    stone.GetComponent<BoxCollider>().enabled = true;
                });
            });
        }
    }

    void Wrong(Transform stone)
    {
        life.LifeDelete(); //라이프 감소
        
        //고개 젓는 애니메이션 실행
        stones.GetComponent<Animator>().enabled = false;
        stone.DORotate(new Vector3(0, 30, 0), 0.3f).SetLoops(4, LoopType.Yoyo).OnComplete(() =>
        {
            stones.GetComponent<Animator>().enabled = true;
            stone.GetComponent<BoxCollider>().enabled = true;
        });  
    }
    
    void ScoreResult()
    {
        //스코어 표시
        score++;
        ScoreText.text = score + " / 5";

        if(score >= 5)
        {
            //UI비활성화 및 활성화
            ScoreUI.gameObject.SetActive(false);
            QuestionUI.gameObject.SetActive(false);
            stones.SetActive(false);

            victoryUI.SetActive(true);
        }
    }
}
