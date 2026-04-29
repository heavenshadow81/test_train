using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ButtonManager_MJ : MonoBehaviour
{
    [SerializeField] protected Image[] images;
    [SerializeField] protected Button btn;

    protected virtual void OnEnable()
    {
        // 비활성화 했던 버튼 다시 활성화 하기
        btn.interactable = true;

        // 페이드 했던 이미지 다시 보이게 하기
        foreach (Image image in images)
        {
            Color color = image.color;
            color.a = 1;
            image.color = color;
        }
    }

    public void OnClick_StartBtn()
    {
        StartCoroutine(FadeAndHideStartUI());
    }

    protected virtual IEnumerator FadeAndHideStartUI()
    {
        btn.interactable = false; // 버튼 클릭 비활성화

        foreach (var image in images)
        {
            image.DOFade(0, 1f).Play();
        }

        yield return new WaitForSeconds(1f);
    }

    public void OnClick_RetryBtn()
    {
        StartCoroutine(FadeAndHideRetryUI());
    }

    protected virtual IEnumerator FadeAndHideRetryUI()
    {
        btn.interactable = false; // 버튼 클릭 비활성화

        foreach (var image in images)
        {
            image.DOFade(0, 1f).Play();
        }

        yield return new WaitForSeconds(1f);
    }
}
