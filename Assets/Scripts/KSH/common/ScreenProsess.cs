using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 김상현 구현
public class ScreenProsess : MonoBehaviour
{
    public Image RulePanel; // 설명서 이미지
    public Button RulePanelBtn; // 설명서 시작 버튼

    public Image resultPanel;   // 결과 UI 이미지
    public Image btnPanel;      // 결과 UI 이미지
    public Button replayBtn;    // 다시하기 버튼
    public Button quitBtn;      // 게임 종료 버튼

    // AssetReference 어드레스불 (에셋)
    public AssetReference winEffect_W;  // 승리 파티클 (가로)
    public AssetReference loseEffect_W; // 패배 파티클 (가로)

    public AssetReference winEffect_H;  // 승리 파티클 (세로)
    public AssetReference loseEffect_H; // 패배 파티클 (세로)

    public LoadSprite loadSprite;   // 에드레서불로 사용할 정보

    private void Awake()
    {
        replayBtn.onClick.AddListener(Replay);  // Replay다시 하기 함수 연결
        quitBtn.onClick.AddListener(Quit);  // Quit 게임 종료 함수 연결

        loadSprite = new LoadSprite("Intro");   // Intro폴더와 연결
    }

    Scene Myscene;  // 
    public void Replay()
    {
        //리플레이 눌렀을때
        //0번씬이
        var IntroScene = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(1));
            //SceneManager.GetSceneByBuildIndex(0);

        //인트로라면
        if (IntroScene == "Intro")
        {
            //내씬 다시시작
            var scene = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            scene.completed += (ap) => { };
        }
        else // 0번씬이 인트로가 아니라면
        {
            // SceneManager.GetActiveScene().buildIndex : 활성화 되있는 씬번호
            // SceneManager.GetSceneByBuildIndex : 현재 씬의 씬 번호
            // 현재 씬 저장
          var scene = SceneManager.GetSceneByBuildIndex(SceneManager.GetActiveScene().buildIndex);
            // 현재 씬 제거 후 screenProsess_completed 함수 실행
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // [다시하기]
    //제거 성공했다면 진행하던 씬을 Addtive 다시 로드 
    // 인트로 씬위에 게임씬
    private async void screenProsess_completed(AsyncOperation obj)
    {
        // 처음 0번씬을 활성화
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
        //딜레이 0.4초
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.4f), DelayType.UnscaledDeltaTime);
        //진행하던 씬을 다시 로드 Addtive 모드(다시 하기)
        // Additive: 전 씬 위에 추가 로드 (전에 있던 인트로 화면을 남겨놓기 위해서 사용.)
        // Single: 전에 있던 씬 파괴 후 로드
        SceneManager.LoadSceneAsync(Settings.instance.SceneNumber, LoadSceneMode.Additive).completed += (ao)=> 
        {
            //로드 성공시 Game씬 활성화
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex( Settings.instance.SceneNumber));
        };
    }

    /// <summary>
    /// Home 인가 Quit 인가....?? by 김상현
    /// </summary>
    public void Quit()
    {
        var IntroScene = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(1));
        //인트로라면
        if (IntroScene == "Intro")
        {
            Settings.instance.fadePanel.blocksRaycasts = true;
            Settings.instance.TweeningFadeInOut(() =>
            {
                var scene = SceneManager.LoadSceneAsync(1);
                scene.completed += (Operation) =>
                {
                    Settings.instance.SettingOff();
                    Settings.instance.SoundOff();
                    Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);

                    Settings.instance.TweeningFadeInOut(() =>
                    {
                        Settings.instance.fadePanel.blocksRaycasts = false;
                        Settings.instance.IntroState = SceneChangeState.Main;
                    }, 1, 0, true);
                };

            }, 1f, 1, true);
        }
        else
        {
            Settings.instance.SceneChangeToUIOFF();
            //지금 진행중인 씬을 저장
            var scene = SceneManager.GetActiveScene();
            //씬 UnLoad
            SceneManager.LoadScene(1);
            AdminSetting.SubCamActiveT?.Invoke();
            
        }
        //SceneManager.LoadSceneAsync(0).completed += (obj) => Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
    }

}
