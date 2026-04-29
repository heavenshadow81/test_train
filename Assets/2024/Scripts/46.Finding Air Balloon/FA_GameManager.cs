using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FA_GameManager : TouchManager_3DTouch
{
    [SerializeField] BalloonMove ballonMove; //풍선움직임 스크립트
    [SerializeField] Image animalsIMG; //동물 이미지
    [SerializeField] Sprite[] animals; //동물 이미지들
    [SerializeField] TextMeshProUGUI question; //열기구 물음
    bool touchOn = false; //터치체크

    int score; //점수 변수
    [SerializeField] TextMeshProUGUI scoreText; //점수 텍스트
    [SerializeField] GameObject scorebox; //점수 텍스트

    [SerializeField] MagicLife life; //라이프 스크립트;

    private void OnEnable()
    {
        //동물 이미지 랜덤하게 변경
        int randomImage = Random.Range(0, animals.Length);
        animalsIMG.sprite = animals[randomImage];
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
            if (hit.collider != null& touchOn) //콜라이더가 존재하고 터치값이 true일때
            {
                if (hit.collider.tag == "target") //레이에 맞은 콜라이더의 태그가 target라면
                {
                    Result(hit.collider.gameObject); //Result함수에 레이에 맞은 오브젝트의 이름을 전달

                    //열기구 회전
                    hit.collider.gameObject.transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), 1f);

                    //열기구 비활성화
                    hit.transform.GetComponent<BoxCollider>().enabled = false;
                }
            }
            else
            {
                //print("콜라이더 없음");
            }
        }
    }

    void Result(GameObject selectedObj)
    {
        //정답 이미지의 이름과 내가 선택한 열기구의 이름이 일치하는 지 확인
      if(animalsIMG.sprite.name.Contains(selectedObj.name))
        {
            //사운드 재생
            SoundMGR.Instance.SoundPlay("따라란");

            //파티클 생성
            Instantiate(effect[0], selectedObj.transform.position, Quaternion.identity);

            //터치 비활성화
            touchOn = false;

            //스코어 상승 및 표시
            score++;
            scoreText.text = score + " / 3";

            print("정답");

            if (score < 3)
            {
                //다시 섞기
                StartCoroutine(ballonMove.ReGame());

                //동물 이미지 랜덤하게 변경
                int randomImage = Random.Range(0, animals.Length);
                animalsIMG.sprite = animals[randomImage];
            }
            else
            {
                //승리 활성화, 열기구 비활성화, 점수 비활성화
                victoryUI.gameObject.SetActive(true);
                gameCanvas.SetActive(false);
                scorebox.SetActive(false);
            }
        }
      else
        {
            //사운드 재생
            SoundMGR.Instance.SoundPlay("띠융");

            life.LifeDelete(); //라이프 감소

            print("오답");
        }


        //텍스트 초기화
        question.text = "";
    }

    public void SetQuiz()
    {
        //사운드 재생
        SoundMGR.Instance.SoundPlay("타자");

        //문제 이미지 제시 후 텍스트 표시
        animalsIMG.gameObject.SetActive(true);
        question.DOText("이 동물 열기구는 어디있을까?", 2f).OnComplete(() =>
        {
            DOVirtual.DelayedCall(1f, () =>
            {
                //텍스트 표시 후 1초 딜레이 주고 터치 가능, 문제 이미지 비활성화
                animalsIMG.gameObject.SetActive(false);
                touchOn = true;
            });
        });
    }
}
