using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FS_GameManager : TouchManager_3DTouch
{
    [SerializeField] EffectRaycastLaser[] lasers; //레이저 스크립트
    public List<Transform> correctObj = new List<Transform>();
    [SerializeField] Transform cameraPos; //카메라 포지션

    Vector3 originCamera; //처음 카메라 위치
    Quaternion originCameraRot; //처음 카메라 위치

    bool touchOn = false; //터치체크

    int select = 5; //5번의 선택 기회 제공을 위한 변수 
    int remain = 5; //남아있는 수

    int score; //점수 변수
    [SerializeField] TextMeshProUGUI scoreText; //점수 텍스트
    [SerializeField] Image scoreBox; //스코어 박스 이미지

    [SerializeField] MagicLife life; //라이프 스크립트;

    private void OnEnable()
    {
        originCamera = Camera.main.transform.position; //카메라 처음 위치 저장
        originCameraRot = Camera.main.transform.rotation; //카메라 처음 위치 저장
        Invoke("SetLaser", 1f); //1초 후 레이저 코루틴 실행
    }

    void SetLaser()
    {
        SoundMGR.Instance.SoundPlay("Laser");

        scoreBox.DOFade(1, 1f); //스코어박스 페이드인

        if (score < 15) //15미만이라면 
        {
            remain = 5; //5개의 레이저 발사
        }
        else
        {
            remain = 20 - score; //남아있는 수 만큼 발사
        }

        if (score < 20)
        {
            for (int i = 0; i < remain; i++)
            {
                StartCoroutine(lasers[i].StartLaserSequence()); //레이저쏘는 코루틴 실행
            }
        }

        //초기화
        select = 5;
        correctObj.Clear();
    }

    public void QuizTime()
    {
        scoreBox.DOFade(0, 1f); //스코어박스 페이드아웃

        //카메라 클로즈업
        Camera.main.transform.DOMove(cameraPos.position, 1f);
        Camera.main.transform.DORotateQuaternion(cameraPos.rotation, 1f).OnComplete(()=>
        {
            //SoundMGR.Instance.SoundStop("Laser");
            DOVirtual.DelayedCall(3f, () => touchOn = true); //3초 뒤 터치 가능
        });

        for (int i = 0; i < remain; i++)
        {
            correctObj.Add(lasers[i].currentTarget);
        }
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
            if (hit.collider != null && touchOn) //콜라이더가 존재하고 터치값이 true일때
            {
                if (hit.collider.tag == "Enemy") //레이에 맞은 콜라이더의 태그가 Enemy라면
                {
                    Result(hit.collider.gameObject); //Result함수에 레이에 맞은 오브젝트의 이름을 전달

                    if (gameObject.activeSelf)
                    {
                        StartCoroutine(ColliderFalse(hit.collider.transform.GetComponent<BoxCollider>()));
                    }
                }
                else
                {
                    print("터치안됨");
                }

            }
            else
            {
                //print("콜라이더 없음");
            }
        }
    }

    void Result(GameObject selectName)
    {
        print(select);

        if (select > 0) //select변수가 0보다 크다면
        {
            select--; //기회 감소
            
            bool isCorrect = false; //정답체크를 위한 불값

            for (int i = 0; i < remain; i++)
            {     
                if (correctObj[i].name == selectName.name) //클릭한 오브젝트가 정답오브젝트와 이름이 같다면
                {
                    isCorrect = true; //불값을 true로
                    correctObj.RemoveAt(i); //정답 리스트에서 제거
                }
            }

            if (isCorrect) //정답 체크가 되었다면
            {
                score++; //점수 상승
                scoreText.text = $"{score} / 20"; //점수 텍스트 표시

                Instantiate(effect[0], selectName.transform); //정답 파티클 생성
                SoundMGR.Instance.SoundPlay("정답");
            }
            else
            {
                Instantiate(effect[1], selectName.transform); //오답 파티클 생성
                life.LifeDelete(); //라이프 감소
                SoundMGR.Instance.SoundPlay("띠융");
            } 
        }

        if (score == 20) //스코어가 20이 되면
        {
            victoryUI.SetActive(true); //빅토리 UI활성화
            gameObject.SetActive(false); //게임매니저 비활성화
            scoreBox.gameObject.SetActive(false); //텍스트박스 비활성화
        }

        if (select==0)
        {
            //카메라 클로즈다운
            Camera.main.transform.DOMove(originCamera, 1f);
            Camera.main.transform.DORotateQuaternion(originCameraRot, 1f);

            Invoke("SetLaser", 1f); //1초 후 레이저 코루틴 실행
            touchOn = false; //터치 잠금
        }

    }

    IEnumerator ColliderFalse(BoxCollider collider)
    {
        collider.enabled = false;
        yield return new WaitForSeconds(5f);
        collider.enabled = true;
    }
}
