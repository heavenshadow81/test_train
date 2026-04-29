using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;


public class CAManager : PlayManager_PlayGround
{
    [Header("게임 설정")]

    [SerializeField] GameObject[] selectSlots;

    [SerializeField] GameObject[] slotEdge;
    [SerializeField] Number_CA answerSlot;

    [SerializeField] CAManager[] setting;
    [SerializeField] GameObject checkEffect;

    [SerializeField] GameObject greenMap;

    [SerializeField] GameObject[] prefabs;

    [Header("입력")]
    [SerializeField] TextMeshProUGUI inputText;

    bool touch;

    protected override void Init()
    {
        base.Init();

        SetQuestion();
    }

    void SetQuestion()
    {
        //answerSlot.rotateAnimation.enabled = true;
        isPerfect = true;

        //입력창 활성화
        answerSlot.transform.DOScale(2, 0.5f);
        inputText.text = "?";

        //정답 활성화
        GameManager_CA.instance.lnit();
        GameManager_CA.instance.arithmeticText.gameObject.SetActive(true);

        SetSlots();
    }

    void SetSlots()
    {
        answerSlot.Init();

        for (int i = 0; i < selectSlots.Length; i++)
        {
            selectSlots[i].transform.GetComponent<Image>().color = Color.white;
            selectSlots[i].transform.DOScale(2, 0.5f);
        }

        touch = true;
    }

    public override void HandleInput(Vector2 inputPosition)
    {
        isTouchable = true;

        // 터치/마우스 위치를 월드 좌표로 변환
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

        // 터치/마우스 위치에서 카드 찾기
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null && touch)
        {
            // 터치한 카드가 어떤 태그를 가지고 있는지 확인
            if (hit.collider.CompareTag(TeamNameString))
            {
                SoundMGR.Instance.SoundPlay("PlayGround_Click");

                //선택한 버튼의 숫자를 가져옴
                TextMeshProUGUI selectedSlot = hit.collider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                if (selectedSlot != null)
                {
                    //입력값 리스트에 저장
                    GameManager_CA.instance.input.Add(int.Parse(selectedSlot.text));
                    GameManager_CA.instance.CheckResult();

                    inputText.text = selectedSlot.text;
                    CheckAnswer(selectedSlot);

                    //선택한 버튼 스프라이트 색상 노란색으로 바꾸고 작아지게
                    hit.collider.transform.GetComponent<Image>().color = Color.yellow;
                    hit.collider.transform.DOScale(1.7f, 0.1f);

                    //파티클 생성 후 제거
                    GameObject particle = Instantiate(effect, answerSlot.transform.position, Quaternion.identity);
                    Destroy(particle, 1f);

                    touch = false;

                }
            }
        }

        void CheckAnswer(TextMeshProUGUI selectedSlot)
        {
            slotEdge[0].SetActive(true);

            answerSlot.rotateAnimation.enabled = false;

            if (GameManager_CA.instance.input.Count > 1)
            {
                GameObject particle = Instantiate(checkEffect);
                Destroy(particle, 0.5f);

                if (GameManager_CA.instance.result)
                {
                    CorrectAnswer(selectedSlot.gameObject);
                }
                else
                {
                    WrongAnswer(selectedSlot.gameObject);
                }
            }
        }
    }


    public override void CorrectAnswer(GameObject touched)
    {

        SoundMGR.Instance.SoundPlay("PlayGround_GameReset");

        for (int i = 0; i < selectSlots.Length; i++)
        {
            selectSlots[i].transform.GetComponent<Image>().color = Color.white;
            selectSlots[i].transform.DOScale(2f, 0.1f);
        }

        GameManager_CA.instance.clearText.text = GameManager_CA.instance.clear.ToString();


        //협동 게임 이라 하나의 매니저에서만 빅토리 체크
        setting[1].ChangeGauge();

        if (GameManager_CA.instance.clear == 0)
        {
            setting[0].slotEdge[0].SetActive(false);
            setting[1].slotEdge[0].SetActive(false);

            GameManager_CA.instance.correctText.gameObject.SetActive(false);
            greenMap.SetActive(false);
        }
        else
        {
            Invoke("Settings", 2f);

            //파티클 생성 후 제거
            GameObject particle = Instantiate(prefabs[0]);
            Destroy(particle, 2f);

            //흰색 비활성화 후 초록색 활성화
            setting[0].slotEdge[0].SetActive(false);
            setting[1].slotEdge[0].SetActive(false);

            setting[0].slotEdge[1].SetActive(true);
            setting[1].slotEdge[1].SetActive(true);
        }
    }

    public override void WrongAnswer(GameObject touched)
    {
        //isPerfect = false;

        //파티클 생성 후 제거
        GameObject particle = Instantiate(prefabs[1]);
        Destroy(particle, 2f);

        SoundMGR.Instance.SoundPlay("PlayGround_WrongAnswer");

        Invoke("Settings", 2f);

        //흰색 비활성화 후 빨간색 활성화
        setting[0].slotEdge[0].SetActive(false);
        setting[1].slotEdge[0].SetActive(false);

        setting[0].slotEdge[2].SetActive(true);
        setting[1].slotEdge[2].SetActive(true);
    }

    void Settings()
    {
        SoundMGR.Instance.SoundPlay("PlayGround_Moving");

        setting[0].SetQuestion();
        setting[1].SetQuestion();

        setting[0].slotEdge[1].SetActive(false);
        setting[1].slotEdge[1].SetActive(false);
        setting[0].slotEdge[2].SetActive(false);
        setting[1].slotEdge[2].SetActive(false);
    }

    public void DiableInput()
    {
        touchAction.Disable();
    }
}
