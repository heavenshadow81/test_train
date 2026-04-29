using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

//Game Simple Title
// 게임 타이틀
public enum ContantsName
{ 
    Intro = 1, 
    Picture,    
    AnimalCard,
    Piano,
    sandWich,
    Slice,
    ColorPlane,
    Paint,
    Greeting,
    CatRock,
    Animalmatch,
    StarRun,
    Drum,
    Crab,
    Hippo,
    KeepFish,
    Dolphin,
    Monkey,
    SheepRevers,
    UnderTheSea,
    Rabbit,
    Penguin,
    CraneGame,
    Viking,
    Merry,
    Ferris,
    OXPlane,
    BumperCar,
    Ski,
    Balloon,
    ShipRun
}

public class ReadyProcess
{
    private ScreenProsess screenProsess;            //각씬마다 가지고있는 UI 컴포넌트 클래스

    public CancellationTokenSource source = new();  //트윈 딜레이중일때 정지시킬수있는 토큰

    public ReadyProcess(ScreenProsess _screenProsess)   //생성자
    { 
        screenProsess = _screenProsess;

        if(source != null) source.Dispose();
        source = new();

        sourceDispone = Destroy;
        sourceCancle = Disable;
    }

    public static Action sourceDispone;
    public static Action sourceCancle;

    public ContantsName contants;       //enum 컨텐츠 제목들 

    //저장할 키값
    string ruleSpriteKey = string.Empty;
    public async void GameRule(Camera cam = null, UnityAction action = null)
    {
        await UniTask.Yield();
        cam = Camera.main;

        //빌드세팅에서 씬이름을 String으로 받아옴 (씬번호)
        var Scene = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(0));
        Debug.Log(Scene);
        if (Scene == "Intro")
        {
            //카메라에 달려있는 DisPlay 번호를 변경
            cam.targetDisplay = 0;
        }
        else
        {
            //카메라에 달려있는 DisPlay 번호를 변경
            cam.targetDisplay = 0;// 0 변경 시 주모니터 변경 (주모니터와 보조모니터 Camera 인스펙터창에서 Display1,2 변경)
        }

        PanelBlack();           //검은배경 패널 0.5
        var ruleBtnImage = screenProsess.RulePanelBtn.GetComponent<UnityEngine.UI.Image>();     //image 컴포넌트 찾기
        //이미지에 등록된 스프라이트 제거
        ruleBtnImage.sprite = null;
        //파라메터로 전달받은 contants 저장
        this.contants = MyContantName();


        //컨텐츠마다 다른 어드레서블 키값 저장
        contantsNameSet(contants);
        

        //저장한 키값으로 해당하는 sprite 로드
        await screenProsess.loadSprite.LoadSpriteData(ruleSpriteKey, screenProsess.RulePanel);

        //버튼용 이미지 로드 
        switch (Settings.instance.IntroState)
        {
            case SceneChangeState.Zootopia:
                await screenProsess.loadSprite.LoadSpriteData("ZP_Play", ruleBtnImage);  //해당하는 키값으로 sprite 로드
                break;
            case SceneChangeState.MagicLand:
                await screenProsess.loadSprite.LoadSpriteData("ML_Play", ruleBtnImage); //해당하는 키값으로 sprite 로드
                break;
            case SceneChangeState.Attraction:
                await screenProsess.loadSprite.LoadSpriteData("AR_Play", ruleBtnImage); //해당하는 키값으로 sprite 로드
                break;
        }
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        //게임설명 패널 활설화
        screenProsess.RulePanel.gameObject.SetActive(true);

        //파라메터로 받아온 Action 실행 
        action?.Invoke();
    }

    private void rulekeySet(string st1)
    {
        //파라메터로 받은 키값 저장
        ruleSpriteKey = st1;
    }

    public async void PanelBackToAlphaZero()
    {
        //게임설명패널 비활성화
       screenProsess.RulePanel.gameObject.SetActive(false);
        //페이드 아웃 
       await DOTween.To(() => screenProsess.resultPanel.color, x => screenProsess.resultPanel.color = x, new Color(0, 0, 0, 0.0f), 0.5f);
        //페이드용 페널 위로 올림
       screenProsess.resultPanel.rectTransform.anchoredPosition = new Vector2(0, 1080);
    }

    public void PanelBlack()
    {
        //페이드용 패널 screen 중앙으로 위치 시킴
        screenProsess.resultPanel.rectTransform.anchoredPosition = new Vector2(0, 0);
        //페이드 인
        DOTween.To(() => screenProsess.resultPanel.color, x => screenProsess.resultPanel.color = x, new Color(0, 0, 0, 0.5f), 0.2f);
    }
    

    public ContantsName MyContantName()
    {
        ActionProcess.ActiveSceneEvent?.Invoke();

        //활성화된 씬 번호
        var Scenename = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("Scene Index : "+Scenename);

        //씬번호로 등록된 열거형의 이름
        return (ContantsName)Scenename;
    }

    private void contantsNameSet(ContantsName contants)
    {

        switch (contants)
        {
            case ContantsName.Picture:
                rulekeySet("ML_동물찾기");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.AnimalCard:
                rulekeySet("ML_동물짝궁");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.Piano:
                rulekeySet("ML_피아노");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.sandWich:
                rulekeySet("ML_샌드위치");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.Slice:
                rulekeySet("ML_과일");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.ColorPlane:
                rulekeySet("ML_색깔발판");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.Paint:
                rulekeySet("ML_페인팅");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.Greeting:
                rulekeySet("ML_세계문화투어");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.CatRock:
                rulekeySet("ML_가위바위보");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.Animalmatch:
                rulekeySet("ML_동물퍼즐");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.StarRun:
                rulekeySet("ML_별과 여행");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.Drum:
                rulekeySet("ZP_브레맨음악대");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.Crab:
                rulekeySet("ZP_꽃게");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.Hippo:
                rulekeySet("ZP_하마");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.Rabbit:
                rulekeySet("ZP_토끼 당근 모으기");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Height);
                break;
            case ContantsName.Penguin:
                rulekeySet("ZP_펭귄 슬라이드");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Height);
                break;
            case ContantsName.KeepFish:
                rulekeySet("ZP_물고기키우기");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.Dolphin:
                rulekeySet("ZP_돌고래 링 넘기");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.Monkey:
                rulekeySet("ZP_원숭이");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.SheepRevers:
                rulekeySet("ZP_양 세우기");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.UnderTheSea:
                rulekeySet("ZP_물고기 카운팅");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.CraneGame:
                rulekeySet("AR_인형뽑기");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.OXPlane:
                rulekeySet("AR_보물찾기");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Height);
                break;
            case ContantsName.BumperCar:
                rulekeySet("AR_범퍼카");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Height);
                break;
            case ContantsName.Viking:
                rulekeySet("AR_바이킹");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.Merry:
                rulekeySet("AR_회전목마");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.Ski:
                rulekeySet("AR_스키");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Height);
                break;
            case ContantsName.Ferris:
                rulekeySet("AR_관람차");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
                break;
            case ContantsName.Balloon:
                rulekeySet("AR_열기구");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Height);
                break;
            case ContantsName.ShipRun:
                rulekeySet("AR_대왕문어");
                Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Height);
                break;
        }
    }

    public async UniTask Ready(UnityAction action = null)
    {
        await UniTask.Yield();
        // 버튼 클릭시 실행될 메소드 입력
        screenProsess.RulePanelBtn.onClick.AddListener(action);
    }
    public void Disable()
    {
        source.Cancel();    
    }

    public void Destroy()
    {
        source.Cancel();
        source.Dispose();
    }


}
