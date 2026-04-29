using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultProcess 
{
    private ScreenProsess screenProsess;        //각씬마다 가지고있는 UI 컴포넌트 클래스

    Image replay;                               //컨텐츠 게임종료시 나오는 replay 이미지
    Image quit;                                 //컨텐츠 게임종료시 나오는 quit 이미지    

    LoadSprite loadSprite;                      //이미지 로드 (어드레서블)

    RectTransform btnPanel;


    public GameObject ClearEffect;
    public GameObject OverEffect;



    public async UniTask Clear(float delay)
    { 
        await UniTask.Delay(System.TimeSpan.FromSeconds(delay));
        ClearEffect.SetActive(true);
    }


    public ResultProcess(ScreenProsess _screenProcess , GameObject ClearEffect = null , GameObject OverEffect = null)  //생성자 
    {
        screenProsess = _screenProcess;

        replay = screenProsess.replayBtn.GetComponent<Image>();
        quit = screenProsess.quitBtn.GetComponent<Image>();

        loadSprite = new LoadSprite("Intro");   //폴더명
        btnPanel = (RectTransform)screenProsess.btnPanel.transform;

        this.ClearEffect = ClearEffect;
        this.OverEffect = OverEffect;
    }

    public Action InitAction = () => { };
    public void Init(EnumClass stateClass)
    {
        //타이틀마다 로딩되야하는 이미지가 달라서 각각 로드
        switch (Settings.instance.IntroState)
        {
            case SceneChangeState.MagicLand:  //매직랜드일때
                UniTask.WhenAll(
                loadSprite.LoadSpriteData("ML_ending", quit),
                loadSprite.LoadSpriteData("ML_replay", replay));

                break;
            case SceneChangeState.Attraction: //어트랙션일때
                UniTask.WhenAll(
                loadSprite.LoadSpriteData("AR_ending", quit),
                loadSprite.LoadSpriteData("AR_replay", replay));
                break;
            case SceneChangeState.Zootopia:  //주토피아일때
                UniTask.WhenAll(
                loadSprite.LoadSpriteData("ZP_ending", quit),
                loadSprite.LoadSpriteData("ZP_replay", replay));
                break;
        }


       RectTransform panel = (RectTransform)screenProsess.resultPanel.transform;

        panel.DOAnchorPos(Vector2.zero, 0.3f).SetEase(Ease.InOutBack).SetDelay(0.5f).OnComplete(async () =>
        {

            screenResPosSetting(Settings.instance.screenRotation);
            btnPanel.localScale = Vector3.one;

            //배경패널 어두운색 알파값 0.5
           // await DOTween.To(() => screenProsess.resultPanel.color, x => screenProsess.resultPanel.color = x, new Color(0, 0, 0, 0.5f), 0.5f);

            screenProsess.resultPanel.color = new Color(0, 0, 0, 0.5f);
            //클리어시
            if (stateClass.resultState == GameResult.Success)
            {
                //각각 로드 
                switch (Settings.instance.IntroState)
                {
                    case SceneChangeState.MagicLand:
                        await UniTask.WhenAll(loadSprite.LoadSpriteData("ML_MC",screenProsess.btnPanel));
                        break;
                    case SceneChangeState.Zootopia:
                        await UniTask.WhenAll(loadSprite.LoadSpriteData("ZP_MC", screenProsess.btnPanel));
                        break;
                    case SceneChangeState.Attraction:
                        await UniTask.WhenAll(loadSprite.LoadSpriteData("AR_MC", screenProsess.btnPanel));
                        break;
                }
            }
            else if (stateClass.resultState == GameResult.Fail) //졌을때
            {
                //각각 로드
                switch (Settings.instance.IntroState)
                {
                    case SceneChangeState.MagicLand:
                        await UniTask.WhenAll(loadSprite.LoadSpriteData("ML_MF", screenProsess.btnPanel));
                        break;
                    case SceneChangeState.Zootopia:
                        await UniTask.WhenAll(loadSprite.LoadSpriteData("ZP_MF", screenProsess.btnPanel));
                        break;
                    case SceneChangeState.Attraction:
                        await UniTask.WhenAll(loadSprite.LoadSpriteData("AR_MF", screenProsess.btnPanel));
                        break;
                }
            }


            //다시시작 버튼이나 종료버튼 활성화 
            //await btnPanel.DOAnchorPos(Vector3.zero, 1.0f).SetEase(Ease.OutBounce);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), DelayType.UnscaledDeltaTime);
            btnPanel.anchoredPosition = Vector3.zero;
            screenProsess.replayBtn.interactable = true;
            screenProsess.quitBtn.interactable = true;


            if (stateClass.resultState == GameResult.Success)
            {
                if (Settings.instance.screenRotation == Settings.ScreenRotation.Width)
                {
                    await loadSprite.LoadEffect(screenProsess.winEffect_W);
                }
                else if (Settings.instance.screenRotation == Settings.ScreenRotation.Height)
                {
                    await loadSprite.LoadEffect(screenProsess.winEffect_H);
                }
            }
            else
            {
                if (Settings.instance.screenRotation == Settings.ScreenRotation.Width)
                {
                    await loadSprite.LoadEffect(screenProsess.loseEffect_W);
                }
                else if (Settings.instance.screenRotation == Settings.ScreenRotation.Height)
                {
                    await loadSprite.LoadEffect(screenProsess.loseEffect_H);
                }
            }

        });
    }

    private void screenResPosSetting(Settings.ScreenRotation screen)
    { 
        switch(screen) 
        {
            case Settings.ScreenRotation.Width:
                btnPanel.anchoredPosition = new Vector2(0, 1000f);
                break;
            case Settings.ScreenRotation.Height:
                btnPanel.anchoredPosition = new Vector2(1280f, 0);
                break;

        }
    }


   




    /*
    public void ScoreTime(float time)
    {
        RectTransform panel = (RectTransform)screenProsess.resultPanel.transform;

        panel.DOAnchorPos(Vector2.zero, 1).SetEase(Ease.InOutBack).SetDelay(0.5f).OnComplete(() =>
        {

            //textAni($"TIME : {Mathf.Round(time)}").Forget();

            //다시시작 버튼이나 다른씬으로 이동할수 있는 버튼 활성화 
            screenProsess.btnPanel.transform.DOScale(Vector3.zero, 0).From();
            screenProsess.btnPanel.transform.DOScale(Vector3.one, 1).SetEase(Ease.InOutBack);
            screenProsess.replayBtn.interactable = true;
            screenProsess.quitBtn.interactable = true;

        });
    }

    public void ScoreInit(float Score)
    {
        RectTransform panel = (RectTransform)screenProsess.resultPanel.transform;

        panel.DOAnchorPos(Vector2.zero, 1).SetEase(Ease.InOutBack).SetDelay(0.5f).OnComplete(() =>
        {

            textAni($"TOTAL : {Score}").Forget();

            //다시시작 버튼이나 다른씬으로 이동할수 있는 버튼 활성화 
            screenProsess.replayBtn.transform.DOScale(Vector3.zero, 0).From();
            screenProsess.replayBtn.transform.DOScale(Vector3.one, 1).SetEase(Ease.InOutBack);
            screenProsess.replayBtn.interactable = true;


            screenProsess.quitBtn.transform.DOScale(Vector3.zero, 0).From();
            screenProsess.quitBtn.transform.DOScale(Vector3.one, 1).SetEase(Ease.InOutBack);
            screenProsess.quitBtn.interactable = true;

        });
    }

    private async UniTask textAni(string text)
    {
        screenProsess.resultText.text = string.Empty;

        await UniTask.Yield();
        for (int i = 0; i < text.Length; i++)
        {
            screenProsess.resultText.text += text[i].ToString();
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
        }
    }
    */
}
