using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

public class PPManger : PlayManager_PlayGround
{
    [SerializeField] LetterSlot_PP[] planets; // 행성들
    public GameObject[] fistPlanet; // 처음 시작할 행성

    [SerializeField] GameObject wrongEffect; //틀렸을 때 이펙트 

    GameObject currentPlanet; //현재 순서의 오렌지 행성 오브젝트
    List<GameObject> planetObjects = new List<GameObject>();

    public List<GameObject> correctPlanet = new List<GameObject>();

    public int score; //정답을 맞힌 수
    int fail; //실패한 수
    public int count;
    public int result = 6; //맞혀야하는 수

    protected override void Init()
    {
        base.Init();

        SettingSlot();
    }
    void SettingSlot()
    {
        foreach (var slot in planets)
        {
            // 행성 초기화
            LetterSlot_PP.Green.Clear();
            LetterSlot_PP.Orange.Clear();
            slot.check = false;

            //행성 작아졌다가 커짐
            slot.transform.DOScale(0, 0.2f).OnComplete(() =>
            {
                slot.transform.DOScale(1.6f, 0.3f);
            });
        }

        Invoke("ShowPath", 0.5f);
    }

    void ShowPath()
    {
        //초기화
        score = 0;
        fail = 0;

        //처음 시작할 행성 랜덤한 수 지정
        int rand = Random.Range(0, fistPlanet.Length);
        currentPlanet = fistPlanet[rand];

        //처음 행성 체크
        currentPlanet.GetComponent<LetterSlot_PP>().check = true;
        currentPlanet.GetComponent<Image>().color = Color.gray;
        //처음 행성 크기 커졌다가 원래 크기로
        currentPlanet.transform.DOScale(2, 0.3f).OnComplete(() =>
        {
            //LetterSlot_PP.isSlotScaleRunning = false;
            //print($"{count + 1}.{currentPlanet.name}");
            currentPlanet.GetComponent<Image>().color = Color.white;
            currentPlanet.transform.DOScale(1.6f, 0.3f);
            Invoke("SlotScale", 0.3f);
        });

        correctPlanet.Add(currentPlanet);
    }

    void SlotScale()
    {
        result = 6;

        if (gameObject.tag == "Orange")
        {
            planetObjects = LetterSlot_PP.Orange;
        }
        else if (gameObject.tag == "Green")
        {
            planetObjects = LetterSlot_PP.Green;
        }

        if (planetObjects.Count != 0 && count<6)
        {
            //충돌한 객체중에 랜덤한 오브젝트 지정
            int rand = Random.Range(0, planetObjects.Count);
            currentPlanet = planetObjects[rand];
            currentPlanet.GetComponent<LetterSlot_PP>().check = true;

            correctPlanet.Add(currentPlanet);

            Invoke("NextPlanet", 0.3f);
        }
        else
        {
            print("버그");
            count =6;
            score +=1;
        }
    }

    void NextPlanet()
    {
        count += 1;

        if (count < result)
        {
            planetObjects.Clear();
            currentPlanet.transform.GetComponent<LetterSlot_PP>().check = true;
            currentPlanet.GetComponent<Image>().color = Color.gray;
            currentPlanet.transform.DOScale(2, 0.3f).OnComplete(() =>
            {              
                currentPlanet.GetComponent<Image>().color = Color.white;
                currentPlanet.transform.DOScale(1.6f, 0.3f).OnComplete(() =>
                    {
                        SlotScale();
                    });
            });
        }
    }

    public override void HandleInput(Vector2 inputPosition)
    {
        //즉시 다시 터치 가능
        isTouchable = true;

        if (count >= result)
        {
            // 터치/마우스 위치를 월드 좌표로 변환
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

            // 터치/마우스 위치에서 카드 찾기
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            if (hit.collider != null)
            {
                // 터치한 카드가 어떤 태그를 가지고 있는지 확인
                if (hit.collider.CompareTag(TeamNameString))
                {
                    SoundMGR.Instance.SoundPlay("PlayGround_Moving");

                    LetterSlot_PP selectedSlot = hit.collider.GetComponent<LetterSlot_PP>();
                    if (selectedSlot != null)
                    {
                        CheckAnswer(selectedSlot);
                    }
                }
                
            }      
        }

    }
    void CheckAnswer(LetterSlot_PP selectedSlot)
    {
        //isTouchDisabled = true;// 터치 비활성화

        // 선택된 행성이 정답 배열의 0번인지 확인
        if (correctPlanet[0]==selectedSlot.gameObject)
        {
            // 정답 처리 메서드 호출
            CorrectAnswer(selectedSlot.gameObject);
        }
        else
        {
            // 오답 처리 메서드 호출
            WrongAnswer(selectedSlot.gameObject);
        }
    }

    public override void CorrectAnswer(GameObject touched)
    {
        //정답 맞은 수 체크하고 리스트 0번 삭제
        score++;
        correctPlanet.RemoveAt(0);

        if (score < result + 1) 
        {
            Vector3 originalPosition = touched.transform.position;

            //이펙트 생성
            GameObject particle = Instantiate(effect, originalPosition, Quaternion.identity);
            Destroy(particle, 1f); // 1초 후 이펙트 제거

            //콜라이더 비활성화 후 스케일이 0이되면 활성화
            touched.GetComponent<BoxCollider2D>().enabled = false;
            touched.transform.DOScale(0, 0.3f).OnComplete(()=>
            {
                touched.GetComponent<BoxCollider2D>().enabled = true;
            });
        }
        
        //모든 정답이 맞았는 지 체크
        if(score== result && fail==0)
        {
            SoundMGR.Instance.SoundPlay("PlayGround_Perfect");
            GameObject perfectParticle = Instantiate(perfectEffect, perfectEffectPos);
            Destroy(perfectParticle, 1.5f);

            count = 0;
            Invoke("SettingSlot", 1.5f);

            ChangeGauge();

            //정답 리스트 정리
            correctPlanet.Clear();
        }
        else if(score == result && fail > 0)
        {
            count = 0;
            Invoke("SettingSlot",0.5f);

            //정답 리스트 정리
            correctPlanet.Clear();

            ChangeGauge();

        }
    }

    public override void WrongAnswer(GameObject touched)
    {
        fail++;
        //print($"false: {touched}");

        Vector3 originalPosition = touched.transform.position;

        //이펙트 생성
        GameObject particle = Instantiate(wrongEffect, originalPosition, Quaternion.identity);
        Destroy(particle, 1f);
    }
}
