using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public static Settings instance;

    public RectTransform SettingPanel;
    public RectTransform SoundPanel;

    public Slider soundSlider;
    public TextMeshProUGUI SoundSliderText;

    public int pageIdx;
    public int pageMax;
    public enum ScreenRotation
    {
        Width,
        Height,
    }

    public ScreenRotation screenRotation;

    private Image settingBtn;


    [Header("SettingPanel Btn")]

    public Button ExitBtn;
    public Button BackBtn;
    public Button HomeBtn;
    public Button SoundBtn;
    public Button QuitBtn;


    [Header("ResultPanel Btn")]
    public Button ReGameBtn;
    public Button GameExitBtn;

    [Header("Mixer")]
    public AudioMixer mixer;

    [NonSerialized] public int titleIdx = 0;
    [SerializeField] private IntroProcess introProcess;

    [SerializeField] private SceneChangeState introState;
    public SceneChangeState IntroState
    {
        get => introState;
        set
        {
            introState = value;
            //컴포넌트 찾기
            introProcess = FindObjectOfType<IntroProcess>();
            //현재 활성화된 씬이 0 번일경우
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                switch (introState)
                {
                    case SceneChangeState.Main:
                        introProcess.isJoin = false;
                        //입장패널 활성화
                        introProcess.JoinPanel.SetActive(true);
                        //타이틀 선택패널 비활성화
                        introProcess.JoinToTitleSelectPanel.SetActive(false);
                        //해당 타이틀 패널들 비활성화
                        foreach (var item in introProcess.TitlePanels) item.SetActive(false);
                        //대기 이미지 로드 
                        introProcess.loadSprite.LoadSpriteData("wait", introProcess.waitToJoin[titleIdx]);
                        //컨텐츠가 가로인지 세로인지 선택해서 세팅UI 위치 변경  
                        ContantSettingPanelPos(ScreenRotation.Width);
                        introProcess.isJoin = false;

                        break;
                    case SceneChangeState.Title:
                        //입장패널 비활성화
                        introProcess.JoinPanel.SetActive(false);
                        //타이틀 선택패널 활성화
                        introProcess.JoinToTitleSelectPanel.SetActive(true);
                        //해당 타이틀 패널들 비활성화
                        foreach (var item in introProcess.TitlePanels) item.SetActive(false);
                        //대기 이미지 로드 
                        introProcess.loadSprite.LoadSpriteData("wait", introProcess.waitToJoin[titleIdx]);

                        break;
                    case SceneChangeState.MagicLand:
                        //입장패널 비활성화
                        //introProcess.JoinPanel.SetActive(false);
                        //타이틀 선택패널 비활성화
                        //introProcess.JoinToTitleSelectPanel.SetActive(false);
                        //해당 타이틀 패널들 비활성화
                        //foreach (var item in introProcess.TitlePanels) item.SetActive(false);
                        //입장할 타이틀 패널 활성화
                        //introProcess.TitlePanels[0].SetActive(true);
                        break;
                    case SceneChangeState.Zootopia:
                        //입장패널 비활성화
                        //introProcess.JoinPanel.SetActive(false);
                        //타이틀 선택패널 비활성화
                        //introProcess.JoinToTitleSelectPanel.SetActive(false);
                        //해당 타이틀 패널들 비활성화
                        //foreach (var item in introProcess.TitlePanels) item.SetActive(false);
                        //입장할 타이틀 패널 활성화
                        //introProcess.TitlePanels[1].SetActive(true);
                        break;
                    case SceneChangeState.Attraction:
                        //입장패널 비활성화
                        //introProcess.JoinPanel.SetActive(false);
                        //타이틀 선택패널 비활성화
                        //introProcess.JoinToTitleSelectPanel.SetActive(false);
                        //해당 타이틀 패널들 비활성화
                        //foreach (var item in introProcess.TitlePanels) item.SetActive(false);
                        //입장할 타이틀 패널 활성화
                        //introProcess.TitlePanels[2].SetActive(true);
                        break;
                }
            }

        }
    }


    public IntroChange introChange;

    //페이드 인아웃용 캔버스그룹
    public CanvasGroup fadePanel;

                                //0. (alpha 0.5)  1. (alpha 1.0)  2. 세팅패널 열림
    public enum SettingState {  transparentOn , transparentOff , SettingPanel  }

    [SerializeField] private SettingState settingST;
    public SettingState SettingST
    {
        get => settingST;
        set 
        { 
            settingST = value;
            switch (settingST)
            {
                case SettingState.transparentOn: transparentToPanelActive(0.5f , false); break;
                case SettingState.transparentOff: transparentToPanelActive(1f , false); break;
                case SettingState.SettingPanel: transparentToPanelActive(1f, true); break;
            }
        }
    }


    public Camera SubCam;   // 보조 모니터용 카메라(터치 모니터)
    //터치시 마우스 입력 마는 용도의 토글
    public Toggle mouseToggle;
    public void OnEnable() { EnableProcess(); }

    public virtual void EnableProcess() { }


    //세팅 버튼 Alpha 값 조절 및 활성화/비활성화
    private void transparentToPanelActive(float Alpha , bool Active)
    {
        settingBtn.color = new Color(1, 1, 1, Alpha);
        SettingPanel.gameObject.SetActive(Active);
    }


    //sound 볼륨 조절 
    //오디오믹서에 연결
    public void SetSoundVolume(float volume)
    {
        mixer.SetFloat("volume", Mathf.Log10(volume) * 20);
        SoundSliderText.text = string.Format($"{Mathf.RoundToInt(volume * 100)}");
    }

    #region SettingPanel

    //세팅UI 켜고 끄기
    public void SettingOn()
    {
        SoundMGR.Instance.SoundPlay("0.설정_버튼");
        SoundOff();

        switch (SettingST)
        {
            case SettingState.transparentOn: SettingST = SettingState.transparentOff; break;
            case SettingState.transparentOff: SettingST = SettingState.SettingPanel; break;
            case SettingState.SettingPanel: SettingST = SettingState.transparentOn; break;
        }
    }
    //세팅 UI 끄기
    public void SettingOff()
    {
        SoundMGR.Instance.SoundPlay("0.설정_버튼");
        SettingST = SettingState.transparentOn;
        SoundOff();
    }

    //씬체인지 후 세팅UI 숨김
    public void SceneChangeToUIOFF()
    {
        SettingST = SettingState.transparentOn;
        SoundOff();
    }

    //세팅 뒤로가기
    public virtual async void Back()
    {
        //뒤로가기 버튼 이전 화면으로 가야됨 
        //일단 무조건 Intro 씬으로 전환 후 IntroProsess 접근 
        //UI Off
        SceneChangeToUIOFF();
        SoundMGR.Instance.SoundPlay("0.설정_백");

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            // if (IntroState != IntroStateSetting.Main || IntroState != IntroStateSetting.Title)
            {
                //현재 Intro씬 상태번호
                int i = (int)introChange;
                //0번 - 입장패널이 활성화 중일때 
                if (i == 0) return;
                i -= 1;
                //변환
                introChange = (IntroChange)i;
                switch (introChange)
                {
                    case IntroChange.Main:
                        IntroState = SceneChangeState.Main;
                        break;
                    case IntroChange.Title:
                        IntroState = SceneChangeState.Title;
                        break;
                    case IntroChange.Game:
                        break;
                }
            }
        }
        //뒤로가기 - 인트로씬이 아니라면
        if (SceneManager.GetActiveScene().buildIndex != 1)
        {
            //timeScale = 1 안넣으면 게임멈출수 있슴.
            Time.timeScale = 1;
            fadePanel.blocksRaycasts = true;
            //페이드 아웃 후 씬 체인지
            TweeningFadeInOut(() => 
            {
                tk().Forget();
            }, 1, 1, true);
        }
    }

    private async UniTask tk()
    {
        //씬 체인지 무조건 Intro씬으로 이동
        await SceneChangeAsync(1 , () => 
        {
            //타이틀 선택 전 패널로 돌아감
            scene_completed();
            //페이드 인 
            TweeningFadeInOut(() => { fadePanel.blocksRaycasts = false; }, 1, 0, true);
        });
    }


    private void scene_completed()
    {
        //컴포넌트 찾기
        introProcess = FindObjectOfType<IntroProcess>();
        ContantSettingPanelPos(ScreenRotation.Width);
        
        //인트로씬 상태별 활성화 시킬 패널설정
        switch (IntroState)
        {
            case SceneChangeState.Main:
                //입장패널 활성화
                introProcess.JoinPanel.SetActive(true);
                introProcess.JoinToTitleSelectPanel.SetActive(false);
                foreach (var item in introProcess.TitlePanels) item.SetActive(false);
                break;
            case SceneChangeState.MagicLand:
                //입장패널 비활성화
                introProcess.JoinPanel.SetActive(false);
                //해당 타이틀 패널 활성화
                introProcess.TitlePanels[0].SetActive(true);
                //이동시킬 컨텐츠 패널 저장
                introProcess.rect = (RectTransform)introProcess.titleContentPanel[0].transform;
                break;
            case SceneChangeState.Zootopia:
                //입장패널 비활성화
                introProcess.JoinPanel.SetActive(false);
                //해당 타이틀 패널 활성화
                introProcess.TitlePanels[1].SetActive(true);
                //이동시킬 컨텐츠 패널 저장
                introProcess.rect = (RectTransform)introProcess.titleContentPanel[1].transform;
                break;
            case SceneChangeState.Attraction:
                //입장패널 비활성화
                introProcess.JoinPanel.SetActive(false);
                //해당 타이틀 패널 활성화
                introProcess.TitlePanels[2].SetActive(true);
                //이동시킬 컨텐츠 패널 저장
                introProcess.rect = (RectTransform)introProcess.titleContentPanel[2].transform;
                break;
        }
        introProcess.RectMenuPosition(pageIdx);
    }

    //홈버튼 입력
    public virtual void Home()
    {
        SoundMGR.Instance.SoundPlay("0.설정_홈");

        //현재 활성화된 씬이 인트로가 아니라면
        if (SceneManager.GetActiveScene().buildIndex != 1)
        {
            //화면 잠금
            fadePanel.blocksRaycasts = true;
            //페이드아웃
            TweeningFadeInOut(() => 
            {
                //0번씬으로 체인지
                var scene = SceneManager.LoadSceneAsync(1);
                //성공시
                scene.completed += (Operation) =>
                {
                    //세팅패널들 비활성화
                    SettingOff();
                    SoundOff();
                    //페이드 인
                    TweeningFadeInOut(() =>
                    {
                        //화면잠금해제
                        fadePanel.blocksRaycasts = false;
                        //IntroState = SceneChangeState.Main; 
                    }, 1, 0, true);
                    //입장패널 활성화 상태로 이동
                    introState = SceneChangeState.Main;
                };
            
            }, 1f, 1,true);

        }
        //현재 씬이 인트로 라면
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if(IntroState == SceneChangeState.Main)
            {
                SceneManager.LoadSceneAsync("SmartPangMenu");
                IntroState = SceneChangeState.Main;
                gameObject.SetActive(false);
            }
            else
                IntroState = SceneChangeState.Main;  
        }
    }
    public void GameEnd()
    {
        SoundMGR.Instance.SoundPlay("0.설정_종료");
        Application.Quit();
    }

    #endregion SettingPanel

    #region SoundPanel
    public void SoundOn()
    {
        SoundMGR.Instance.SoundPlay("0.설정_사운드");
        SoundPanel.gameObject.SetActive(true);
    }
    public void SoundOff()
    {
        //SoundMGR.Instance.SoundPlay("0.설정_사운드");
        SoundPanel.gameObject.SetActive(false);
    }
    #endregion

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (instance != null)
            { 
                Destroy(gameObject);
            }
        }

        //soundSlider.onValueChanged.RemoveListener(SoundMixer);
        soundSlider.onValueChanged.AddListener(SetSoundVolume);
        introProcess = FindObjectOfType<IntroProcess>();
        settingBtn = transform.GetChild(0).GetComponent<Image>();
        SettingST = SettingState.transparentOn;

       
    }

    //가로컨텐츠 세로컨텐츠 구분
    //위치 , 스케일 , 방향 세팅
    public virtual void ContantSettingPanelPos(ScreenRotation screen)
    {
        SceneChangeToUIOFF();
        screenRotation = screen;

        //0 0  중앙으로 세팅
        switch (screen)
        {
            //컨텐츠가 가로일때
            case ScreenRotation.Width:
                //x y 값 세팅
                SettingPanel.anchoredPosition = new Vector2(0f, 350f);
                //스케일값 1,1,1
                SettingPanel.transform.localScale = Vector2.one;
                //로테이션값 0,0,0
                SettingPanel.transform.rotation = Quaternion.Euler(0, 0, 0);
                //x y 값 세팅
                SoundPanel.anchoredPosition = new Vector2(0, 0);
                //스케일값 1,1,1
                SoundPanel.transform.localScale = Vector2.one;
                //로테이션 값 0,0,0
                SoundPanel.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            //컨텐츠가 세로일때
            case ScreenRotation.Height:
                //x y 값 세팅
                SettingPanel.anchoredPosition = new Vector2(780f, -50f);
                //스케일값 0.9 , 0.9 , 0.9
                SettingPanel.transform.localScale = Vector2.one * 0.9f;
                //로테이션값 0 , 0 , -0.9
                SettingPanel.transform.rotation = Quaternion.Euler(0, 0, -90f);
                //x y 값 세팅
                SoundPanel.anchoredPosition = new Vector2(350f, -50f);
                //스케일값 0.9 , 0.9 , 0.9
                SoundPanel.transform.localScale = Vector2.one * 0.9f;
                //로테이션값 0 , 0 , -0.9
                SoundPanel.transform.rotation = Quaternion.Euler(0, 0, -90f);
                break;
        }
    }


    /// <summary>
    /// 트윈이End = finalAction ///
    /// 시간 = time ///
    /// 투명값 = Alpha ///
    /// Update(true) = 게임이 일시정지 되었어도 트윈이 실행됨
    /// </summary>
    public async void TweeningFadeInOut(Action finalAction = null, float time = 0.5f, float Alpha = 1, bool Update = false)
    {
        await DOTween.To(() => fadePanel.alpha, x => fadePanel.alpha = x, Alpha, time).SetUpdate(Update);
        finalAction?.Invoke();
    }


    public int SceneNumber;

    //씬체인지
    public virtual async UniTask SceneChangeAsync(int SceneNumber, Action changeComplateAction = null)
    {
        await UniTask.Yield();

         //씬 체인지(Single) 일반적인 사용방법
         //이전씬 파괴하고 다음씬을 로드함
        var scene = SceneManager.LoadSceneAsync(SceneNumber,LoadSceneMode.Single);
        //씬이 전환될때까지 대기 (대략 While문에서 추가적인 로직을 시행할수있음.)
        //Ex) 로딩
        while (!scene.isDone) await UniTask.Yield();

        ActionProcess.ActiveSceneEvent = () => { SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(SceneNumber)); };
        scene.completed += async (ao) =>
        {
            //introProcess.LoadScene = SceneManager.GetSceneByBuildIndex(SceneNumber);
            await UniTask.Delay(TimeSpan.FromSeconds(1), DelayType.UnscaledDeltaTime);

            Debug.Log(SceneManager.GetSceneByBuildIndex(SceneNumber).name);

            changeComplateAction?.Invoke();
            //씬 활성화
            
        };
    }


    public Vector2 MousePos()
    {
        return Input.mousePosition;
/*        //에디터에서 실행중이라면
#if UNITY_EDITOR
        return Input.mousePosition;
        ////주모니터 오른쪽
        ////서브모니터 왼쪽  컨텐츠모니터
        //if (Input.mousePosition.x < 0f)
        //{
        //    return new Vector2((Input.mousePosition.x + 1920), Input.mousePosition.y);
        //}
        //else if (Input.mousePosition.x > 0f)
        //{
        //    //주모니터 왼쪽
        //    //서브모니터 오른쪽   컨텐츠모니터
        //    if (Input.mousePosition.x > 1920f)
        //    {
        //        return new Vector2((Input.mousePosition.x - 1920), Input.mousePosition.y);
        //    }
        //}

        //return Vector2.zero;
#else
            //에디터에서 실행중이 아니라면

            //주모니터 오른쪽
            //서브모니터 왼쪽  컨텐츠모니터
            if (Input.mousePosition.x < 0f)
            {
                return new Vector2((Input.mousePosition.x + 1920), Input.mousePosition.y);
            }
            else if (Input.mousePosition.x > 0f)
            {
                //주모니터 왼쪽
                //서브모니터 오른쪽   컨텐츠모니터
                if (Input.mousePosition.x > 1920f)
                {
                    return new Vector2((Input.mousePosition.x - 1920), Input.mousePosition.y);
                }
            }

            return Vector2.zero;
#endif*/

    }


}