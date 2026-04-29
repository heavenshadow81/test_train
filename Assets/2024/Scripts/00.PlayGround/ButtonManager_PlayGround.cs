using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

public class ButtonManager_PlayGround : ButtonManager_MJ
{
    TextMeshProUGUI retryText;

    private void Awake()
    {
        retryText = GetComponentInChildren<TextMeshProUGUI>();

        // 텍스트 비활성화
        retryText.enabled = false;
    }

    protected override void OnEnable()
    {
        StartCoroutine(EnableRoutine());
    }

    IEnumerator EnableRoutine()
    {
        btn.interactable = false;

        yield return new WaitForSeconds(3f);

        // 비활성화 했던 버튼 다시 활성화 하기
        btn.interactable = true;

        // 비활성화 했던 텍스트 다시 활성화 하기
        retryText.enabled = true;

    }

    protected override IEnumerator FadeAndHideRetryUI()
    {
        SoundMGR.Instance.SoundPlay("PlayGround_GameReset");

        yield return base.FadeAndHideRetryUI();

        // 모든 트위닝 종료
        DOTween.KillAll(true);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
