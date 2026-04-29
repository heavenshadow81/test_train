using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionButton : MonoBehaviour
{
    //콘텐츠 파라미터 - 타입(숫자, 알파벳, 한글)
    public void SetParameter(int type)
    {
        DinoSceneOptions.SetParameter(type);
    }

    //콘텐츠 파라미터 - 테마(공룡, 동물)
    public void SetTheme(int theme)
    {
        DinoSceneOptions.SetTheme(theme);

    }

    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
    }

    public void RetrySaveParameter()
    {
        // 숫자 영어 한글
        BubblePang.ContentsType content = ContentsController.Instance.contentsParameter.contents;

        // 쉬움 보통
        Difficulty diffculty = ContentsController.Instance.contentsParameter.difficult;

        //공룡 동물
        Theme theme = ContentsController.Instance.contentsParameter.theme;

        // 인원수
        int person = ContentsController.Instance.contentsParameter.person;

        // 테마 Scene Index
        //int themeNum = (int)ContentsController.Instance.contentsParameter.theme;

        // Static class로 각 파라미터 전달
        DinoSceneOptions.RetrySaveParameter(content, diffculty, person, theme);

        // 현 테마 Scene ReLoad
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
