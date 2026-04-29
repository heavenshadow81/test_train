using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

public class BG_Manger : PlayManager_PlayGround
{
    [SerializeField] BG_BatMove[] bats; //박쥐들
    [SerializeField] GameObject[] effects; //효과들
    [SerializeField] GameObject[] results; //승부결과들

    [SerializeField] TextMeshProUGUI timer;
    int time = 30;

    [SerializeField] TextMeshProUGUI[] teamText;

    public static int orange = 20;
    public static int green = 20;

    protected override void Init()
    {
        base.Init();

        SettingSlot();
    }
    void SettingSlot()
    {
        Timer();
    }
    private void OnEnable()
    {
        time = 30;
        orange = 20; green = 20;
    }

    public override void HandleInput(Vector2 inputPosition)
    {
        // 터치 활성화
        isTouchable = true;

        // 터치/마우스 위치를 월드 좌표로 변환
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

        // 터치/마우스 위치에서 카드 찾기
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null)
        {
            BG_BatMove selectedBat = hit.collider.transform.parent.GetComponent<BG_BatMove>();
            GameObject bat = hit.collider.gameObject;
            GameObject batParent = hit.collider.transform.parent.gameObject;

            if (selectedBat != null)
            {
                SoundMGR.Instance.SoundPlay("PlayGround_Moving");

                //색상바뀌는 함수 실행
                selectedBat.ColorChange();
                bat.GetComponent<BoxCollider2D>().enabled = false;

                teamText[0].text = orange.ToString();
                teamText[1].text = green.ToString();

                Vector3 pos = batParent.transform.position;
                batParent.transform.DOMove(new Vector3(pos.x * -1, pos.y, pos.z), 1).OnComplete(() =>
                {
                    batParent.transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = true;
                });
            }
        }
    }

    void Timer()
    {
        if (time > 0)
        {
            time--;
            timer.text = time.ToString();

            Invoke("Timer", 1);
        }
        else
        {
            for (int i = 0; i < bats.Length; i++)
            {
                bats[i].gameObject.SetActive(false);
                Instantiate(effects[0], bats[i].transform.position, Quaternion.identity);
            }

            //0초가 되었을 때 결과 값에 따른 창
            if (orange < green)
            {
                SoundMGR.Instance.SoundPlay("PlayGround_Victory");
                results[0].SetActive(true);
            }
            else if (orange > green)
            {
                SoundMGR.Instance.SoundPlay("PlayGround_Victory");
                results[1].SetActive(true);
            }
            else
            {
                SoundMGR.Instance.SoundPlay("PlayGround_WrongAnswer");
                results[2].SetActive(true);
            }
        }
    }

    public override void CorrectAnswer(GameObject bat)
    {
    }

    public override void WrongAnswer(GameObject bat)
    {
    }
}
