using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonManager_Mugunghwa : MonoBehaviour
{
    public Button Btn;

    public void OnClick_StartGame()
    {
        if (Btn != null)
        {
            Btn.interactable = false; // 버튼 클릭 비활성화
        }
        StartCoroutine(SceneChange("무궁화_게임시작"));
    }

    public void OnClick_RestartGame()
    {
        if (Btn != null)
        {
            Btn.interactable = false; // 버튼 클릭 비활성화
        }
        StartCoroutine(SceneChange("무궁화_다시하기"));
    }

    IEnumerator SceneChange(string soundName)
    {
        SoundMGR.Instance.SoundPlay(soundName);

        yield return new WaitForSeconds(1f);

        SceneManager.LoadSceneAsync("02.Mugunghwa");
    }
}
