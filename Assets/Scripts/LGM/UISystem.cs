using UnityEngine;
using UnityEngine.SceneManagement;

public class UISystem : MonoBehaviour
{
    // 씬 로드
    public void LoadScene()
    {
        Time.timeScale = 1;
        // 자신 프로젝트를 불러와 다시시작 실행
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 게임 종료
    public void ExitGame()
    {
        SceneManager.LoadSceneAsync(0).completed += 
            (obj) => Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
        SceneManager.LoadScene("Intro");
    }
}
