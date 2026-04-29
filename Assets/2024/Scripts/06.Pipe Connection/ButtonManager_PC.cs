using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class ButtonManager_PC : MonoBehaviour
{
    public GameObject targetUI;
    public Image[] images;
    public Button btn;
    public AudioClip bgmClip;
    public GameObject car;
    AudioSource bgmAudio;

    private void Awake()
    {
        // BGM이라는 이름의 게임 오브젝트를 찾고 AudioSource를 가져옵니다.
        GameObject bgmObject = GameObject.Find("BGM");
        if (bgmObject != null)
        {
            bgmAudio = bgmObject.GetComponent<AudioSource>();
        }
        else
        {
            Debug.LogWarning("BGM 게임 오브젝트를 찾을 수 없습니다.");
        }
    }

    private void OnEnable()
    {
        btn.interactable = true;
    }

    public void OnClick_StartBtn()
    {
        StartCoroutine(FadeAndHideStartUI());
    }

    IEnumerator FadeAndHideStartUI()
    {
        btn.interactable = false; // 버튼 클릭 비활성화

        // 차를 흔들기
        SoundMGR.Instance.SoundPlay("트럭경적");
        yield return car.transform.DOShakePosition(1f, 10f, 10, 90).WaitForCompletion();

        // 흔들림이 끝난 후 X축으로 이동
        SoundMGR.Instance.SoundPlay("트럭부르릉");
        yield return car.transform.DOMoveX(12f, 2f).WaitForCompletion();

        // BGM 멈추고 클립 변경
        SoundMGR.Instance.SoundStop("트럭부르릉");
        bgmAudio.Stop();
        bgmAudio.clip = bgmClip;

        // 이미지 페이드 아웃
        foreach (var image in images)
        {
            image.DOFade(0, 1f).Play();
        }

        // 모든 이미지가 페이드 아웃 될 때까지 대기
        yield return new WaitForSeconds(1f);

        // WaterFlowManager 초기화 및 BGM 재생
        WaterFlowManager_PC.Instance.Init();
        bgmAudio.Play();
        targetUI.SetActive(false);
    }


    public void OnClick_RetryBtn()
    {
        StartCoroutine(FadeAndHideRetryUI());
    }

    IEnumerator FadeAndHideRetryUI()
    {
        SoundMGR.Instance.SoundPlay("FindMonkey_Click");

        btn.interactable = false; // 버튼 클릭 비활성화

        foreach (var image in images)
        {
            image.DOFade(0, 1f).Play();
        }

        yield return new WaitForSeconds(1f);

        targetUI.SetActive(false);
        CellManager_PC.Instance.SpawnCells();
        WaterFlowManager_PC.Instance.Init();
    }
}
