using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ML.T_Sports.Common;
using ML.T_Sports.BaseBall;
namespace ML.T_Sports.Main
{
    public enum Mode
    {
        Single,
        Team
    }
    //터치 메인
    public class MainManager : MonoBehaviour
    {
        public string LoadSceneName;
        public BGMFade bgmFade;
        public ParticleSystem LoadingIcon;

        public Animation ModeChangeAnimation;
        public Image ModeImage;
        public Sprite SingleMode, TeamMode;
        public Mode mode;
        public bool isChanging;
        public bool isLoading;
        public EFMPlayer SelectSound;

        public float SFXSound;
        public float BGMSound;
        public Slider BGMSlider, SFXSlider;
        public Text BGMValue, SFXValue;
        private ContentsProperties sharedProps;
        public  void Awake()
        {
            mode = Mode.Single;
            isChanging = false;
            isLoading = false;

            if (Display.displays.Length > 0)
                Display.displays[0].Activate();
            if (Display.displays.Length > 1)
                Display.displays[1].Activate();

            sharedProps = new ContentsProperties();
            sharedProps.InitProperty(ContentsPropertyType.BGM, 0, 1, 0.5f);
            sharedProps.InitProperty(ContentsPropertyType.SFX, 0, 1, 0.5f);

            sharedProps.LoadPropertyValues("Shared");
            SFXSound = sharedProps.GetPropertyValue(ContentsPropertyType.SFX);
            BGMSound = sharedProps.GetPropertyValue(ContentsPropertyType.BGM);
            SFXSlider.value = SFXSound;
            BGMSlider.value = BGMSound;
            Debug.Log("SFXSound " + SFXSound);
            Debug.Log("BGMSound " + BGMSound);
        }

        public void SetSFXSound()
        {
            float newValue = SFXSlider.value;
            float val = newValue * 100;
            SFXValue.text = val.ToString("N0") + "%";
            sharedProps.SetPropertyValue(ContentsPropertyType.SFX, newValue);
            sharedProps.SavePropertyValues("Shared");
            Common.SoundManager.instance.SetEFMVolume(newValue);
        }
        public void SetBGMSound( )
        {
            float newValue = BGMSlider.value;
            float val = newValue * 100;
            BGMValue.text = val.ToString("N0") + "%";
            sharedProps.SetPropertyValue(ContentsPropertyType.BGM, newValue);
            sharedProps.SavePropertyValues("Shared");
            Common.SoundManager.instance.SetBGMVolume(newValue);
        }
        public void ExitTouchSports()
        {
            Application.Quit();
        }
        public void ModeChange()
        {
            if (isChanging)
            {
                Debug.Log("Mode is Changing Now...");
                return;
            }
            SelectSound.EFMRandomPlay();
            isChanging = true;
            StartCoroutine(ModeisChanging());
            if (mode == Mode.Single)
            {
                ModeImage.sprite = TeamMode;
                mode = Mode.Team;
                ModeChangeAnimation.Play("FlipToTeam");
            }
            else if (mode == Mode.Team)
            {
                ModeImage.sprite = SingleMode;
                mode = Mode.Single;
                ModeChangeAnimation.Play("FlipToSingle");
            }
        }
        IEnumerator ModeisChanging()
        {
            yield return new WaitForSeconds(0.7f);
            isChanging = false;
        }

        //콘텐츠 버튼 입력시 불러올 씬 번호 저장 및 확인 창 출력
        public void SelectContents(string SceneName)
        {
            if (isLoading)
            {
                Debug.Log("이미 실행 중 입니다.");
                return;
            }
            SelectSound.EFMRandomPlay();
            if (SceneName == "Empty")
                return;


            isLoading = true;
            LoadSceneName = SceneName;
            StartCoroutine(Loading());
        }
        IEnumerator Loading()
        {
            LoadingIcon.Play();
            bgmFade.BGMFadeOut();
            yield return new WaitForSeconds(Random.Range(1.5f, 2f));
            SceneManager.LoadScene(LoadSceneName);
        }


    }
}
