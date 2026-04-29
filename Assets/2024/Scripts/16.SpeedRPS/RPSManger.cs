using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

using static InvBaseItem;
using Unity.VisualScripting;

public class RPSManger : PlayManager_PlayGround
{
    [SerializeField] GameObject[] RPSBtn; // 가위바위보 오브젝트
    [SerializeField] GameObject RPSGroup; // 가위바위보그룹 오브젝트
    [SerializeField] GameObject RPSBG; // 가위바위보 배경 오브젝트

    [SerializeField] Image RPSIMG; // 선택한 가위바위보 이미지
    Dictionary<string, Sprite> selectRPS; //가위바위보 선택에 따른 이미지 변화
    [SerializeField] Sprite[] RPSImages; //가위바위보 이미지

    [SerializeField] GameObject[] effects; //이펙트

    int score;
    public static bool end; //게임 종료 여부 체크

    private void OnEnable()
    {
        end = false;

        selectRPS = new Dictionary<string, Sprite>()
        {
            { "Rock", RPSImages[0] },
            { "Paper", RPSImages[1] },
            { "Scissors", RPSImages[2] }
        };
    }

    protected override void Init()
    {
        base.Init();

        SettingSlot();
    }
    void SettingSlot()
    {
        StartCoroutine(CenterRPS.instance.RPSStart());
        RPSGroup.SetActive(true);
    }

    public override void HandleInput(Vector2 inputPosition)
    {
        // 터치 활성화
        isTouchable = true;

        // 터치/마우스 위치를 월드 좌표로 변환
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

        // 터치/마우스 위치에서 카드 찾기
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null && CenterRPS.instance.gameCheck)
        {
            // 터치한 카드가 어떤 태그를 가지고 있는지 확인
            if (hit.collider.CompareTag(TeamNameString))
            {
                SoundMGR.Instance.SoundPlay("PlayGround_Moving");

                Image selectedSlot = hit.collider.transform.GetComponent<Image>();
                if (selectedSlot != null)
                {
                    //더블터치 방지 콜라이더 비활성화
                    for (int i = 0; i < RPSBtn.Length; i++)
                    {
                        RPSBtn[i].transform.GetComponent<BoxCollider2D>().enabled = false;
                    }

                    //사이즈 작아졌다가 돌아가는 클릭 
                    selectedSlot.transform.parent.transform.DOScale(1.5f, 0.2f).OnComplete(() =>
                    {
                        selectedSlot.transform.parent.transform.DOScale(1.7f, 0.2f);
                    });

                    //어떤 버튼을 눌렀지에 따라 이미지 변경
                    if (selectRPS.ContainsKey(selectedSlot.sprite.name))
                        RPSIMG.sprite = selectRPS[selectedSlot.sprite.name];

                    //클릭한 버튼의 위치
                    Vector3 originalPosition = selectedSlot.transform.position;
                    //가위바위보가 최종적으로 이동할 위치 
                    Vector3 endPosition = RPSBG.transform.position;

                    //이펙트 생성
                    GameObject particle = Instantiate(effect, originalPosition, Quaternion.identity);
                    Destroy(particle, 1f); // 1초 후 이펙트 제거

                    //내가 클릭한 버튼의 위치로 이동
                    RPSBG.transform.position = originalPosition;
                    //가위바위보 배경 활성화
                    RPSBG.SetActive(true);

                    RPSBG.transform.DOMove(endPosition, 0.5f).OnComplete(() => CheckAnswer(selectedSlot));
                }
            }
        }
    }
    void CheckAnswer(Image selectedSlot)
    {    
        if (CenterRPS.instance.speedCheck)
        {
            if (selectedSlot.sprite.name == CenterRPS.instance.RPSIMG.sprite.name)
            {
                print("비김");

                Vector3 originalPosition = RPSBG.transform.position;

                //이펙트 생성
                GameObject particle = Instantiate(effects[2], originalPosition, Quaternion.identity);
                Destroy(particle, 2f); // 2초 후 이펙트 제거

                Invoke("ReGame", 1f);
                CenterRPS.instance.gameCheck = false;
            }
            else if ((selectedSlot.sprite.name == "Rock" && CenterRPS.instance.RPSIMG.sprite.name == "Scissors") ||
                     (selectedSlot.sprite.name == "Scissors" && CenterRPS.instance.RPSIMG.sprite.name == "Paper") ||
                     (selectedSlot.sprite.name == "Paper" && CenterRPS.instance.RPSIMG.sprite.name == "Rock"))
            {
                // 정답 처리 메서드 호출
                CorrectAnswer(selectedSlot.gameObject);
                CenterRPS.instance.speedCheck = false;
                CenterRPS.instance.gameCheck = false;
            }
            else
            {
                WrongAnswer(selectedSlot.gameObject);
                CenterRPS.instance.gameCheck = false;
            }
        }
        else
        {
            // 오답 처리 메서드 호출
            WrongAnswer(selectedSlot.gameObject);
        }
    }

    public override void CorrectAnswer(GameObject touched)
    {
        SoundMGR.Instance.SoundPlay("PlayGround_RightAnswer");
        print("이김");

        Vector3 originalPosition = RPSBG.transform.position;

        //이펙트 생성
        if (score < 4 && !end)
        {
            GameObject particle = Instantiate(effects[0], originalPosition, Quaternion.identity);
            Destroy(particle, 2f); // 2초 후 이펙트 제거
        }
        else CenterRPS.instance.isCoroutineRunning = true;

        ChangeGauge();

        score++;

        Invoke("ReGame", 1f);

    }


    public override void WrongAnswer(GameObject touched)
    {
        SoundMGR.Instance.SoundPlay("PlayGround_WrongAnswerr");

        print("짐");

        Vector3 originalPosition = RPSBG.transform.position;

        if (score < 4 && !end)
        {
            //이펙트 생성
            GameObject particle = Instantiate(effects[1], originalPosition, Quaternion.identity);
            Destroy(particle, 2f); // 2초 후 이펙트 제거
        }

        Invoke("ReGame", 1f);

    }

    void ReGame()
    {
        if (score < 4 && !end)
        {

            //콜라이더 활성화
            for (int i = 0; i < RPSBtn.Length; i++)
            {
                RPSBtn[i].transform.GetComponent<BoxCollider2D>().enabled = true;
            }

            //가위바위보 배경들 비활성화
            RPSBG.SetActive(false);

            if (!CenterRPS.instance.isCoroutineRunning)
            { 
                CenterRPS.instance.CenterBG.SetActive(false);
                CenterRPS.instance.RPSIMG.gameObject.SetActive(false);
                StartCoroutine(CenterRPS.instance.RPSStart());
            }
        }
        else
        {
            end = true;
        }
    }
}
