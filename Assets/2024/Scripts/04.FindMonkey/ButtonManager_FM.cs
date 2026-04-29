using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ButtonManager_FM : MonoBehaviour
{
    public GameObject targetUI;
    public Image[] images;
    public Button btn;

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
        SoundMGR.Instance.SoundPlay("FindMonkey_Click");

        btn.interactable = false; // 버튼 클릭 비활성화

        foreach (var image in images)
        {
            image.DOFade(0, 1f).Play();
        }

        yield return new WaitForSeconds(1f);

        targetUI.SetActive(false);
        GameManager_FM.Instance.AnswerUI.Init();
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
        GameManager_FM.Instance.SetAnswer();
        GameManager_FM.Instance.AnswerUI.Init();
    }
}
