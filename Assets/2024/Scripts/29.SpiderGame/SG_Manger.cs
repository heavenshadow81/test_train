using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

public class SG_Manger : PlayManager_PlayGround
{
    [SerializeField] GameObject[] effects; //효과들
    [SerializeField] GameObject[] results; //승부결과들

    [SerializeField] TextMeshProUGUI timer;
    int time = 30;

    [SerializeField] TextMeshProUGUI[] teamText;

    public static int orange;
    public static int green;

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
        orange = 0; green = 0;
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
            GameObject spider = hit.collider.gameObject;

            //거미의 태그가 오렌지라면
            if (spider != null & spider.tag == "Orange" & time >0)
            {
                //사운드 재생
                SoundMGR.Instance.SoundPlay("PlayGround_Moving");

                //파티클 생성 후 파괴, 거미 오브젝트 파괴
                GameObject particle = Instantiate(effects[0], spider.transform.position, Quaternion.identity);
                Destroy(spider);
                Destroy(particle, 1f);

                //오렌지 텍스트 활성화
                orange++;
                teamText[0].text = orange.ToString();
            }
            else if (spider != null & spider.tag == "Green" & time > 0)
            {
                //사운드 재생
                SoundMGR.Instance.SoundPlay("PlayGround_Moving");

                //파티클 생성 후 파괴, 거미 오브젝트 파괴
                GameObject particle = Instantiate(effects[1], spider.transform.position, Quaternion.identity);
                Destroy(spider);
                Destroy(particle, 1f);

                //그린 텍스트 활성화
                green++;
                teamText[1].text = green.ToString();
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
            //0초가 되었을 때 결과 값에 따른 창
            if (orange > green)
            {
                SoundMGR.Instance.SoundPlay("PlayGround_Victory");
                results[0].SetActive(true);
            }
            else if (orange < green)
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

    public override void CorrectAnswer(GameObject spider)
    {
    }

    public override void WrongAnswer(GameObject spider)
    {
    }
}
