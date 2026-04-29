using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager_CT : ButtonManager_MJ
{
    [SerializeField] GameObject targetUI;

    protected override IEnumerator FadeAndHideStartUI()
    {
        SoundMGR.Instance.SoundPlay("CatchingText_Click"); // 사운드 재생

        // 초기화
        GameManager_CT.Instance.Init();

        yield return base.FadeAndHideStartUI(); // 기본 동작 호출 

        targetUI.SetActive(false);
    }

    protected override IEnumerator FadeAndHideRetryUI() // 오버라이드
    {
        SoundMGR.Instance.SoundPlay("CatchingText_Click"); // 사운드 재생
        SoundMGR.Instance.SoundStop("CatchingText_Ending");

        // 초기화
        GameManager_CT.Instance.Init();

        yield return base.FadeAndHideRetryUI(); // 기본 동작 호출

        SoundMGR.Instance.bgmSource.Play();
        targetUI.SetActive(false);
    }
}
