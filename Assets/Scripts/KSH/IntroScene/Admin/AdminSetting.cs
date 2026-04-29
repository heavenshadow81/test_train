using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdminSetting : Settings
{

    public static Action SubCamActiveT = () => { };
    public static Action SubCamActiveF = () => { };


    public override void Awake()
    {
        base.Awake();
    }
    public override void EnableProcess()
    {
        /*------------Debug.Log(Screen.width);*/

        //ºôµå¼¼ÆÃ¿¡ µî·ÏµÇÀÖ´Â ¾À ¹øÈ£¿¡ ¸Â´Â ¾ÀÀÌ¸§À» ºÒ·¯¿È
        if (System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(0)) != "Intro")
        { 
            SubCamActiveT = () => 
            { 
                if(SubCam != null) 
                    SubCam.gameObject.SetActive(true); 
            };
            SubCamActiveF = () => { if(SubCam != null) SubCam.gameObject.SetActive(false); };
        }
    }

    public override void ContantSettingPanelPos(ScreenRotation screen)
    {
        SceneChangeToUIOFF();
        screenRotation = screen;

    }
    public override void Back()
    {
        SceneChangeToUIOFF();
        SoundMGR.Instance.SoundPlay("0.¼³Á¤_¹é");

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            int i = (int)introChange;
            if (i == 0) return;

            i -= 1;
            introChange = (IntroChange)i;
            //Debug.Log("0");
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
        else
        { 
            var scene = SceneManager.GetActiveScene();

            SceneManager.UnloadSceneAsync(scene).completed += (ao) => 
            { SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(0)); };
            SubCamActiveT?.Invoke();
            Time.timeScale = 1;
        }
    }

    public override void Home()
    {
        SceneChangeToUIOFF();

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            int i = (int)introChange;
            if (i == 0) return;
           
            IntroState = SceneChangeState.Main;
        }
        else
        { 
            var scene = SceneManager.GetActiveScene();

            SceneManager.UnloadSceneAsync(scene).completed += (ao) =>
            { SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(0)); };
            SubCamActiveT?.Invoke();
        }
    }

    
    public override async UniTask SceneChangeAsync(int SceneNumber, Action changeComplateAction = null)
    {
        await UniTask.Yield();
        this.SceneNumber = SceneNumber;
        
        var CurScene = SceneManager.GetActiveScene();
        var PrevScene = SceneManager.GetSceneByBuildIndex(0);

        AsyncOperation scene = null;

        if (CurScene == PrevScene)
        {
            Debug.Log("°°Àº¾À");
        }
        else
        {
            var RemoveSc = SceneManager.GetActiveScene();
            SceneManager.UnloadSceneAsync(RemoveSc).completed += (ao) =>
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(0));
            };
        }


        //ÄÁÅÙÃ÷ ¾ÀÀ» Intro¾À À§¿¡ ¿Ã¸²
        scene = SceneManager.LoadSceneAsync(SceneNumber, LoadSceneMode.Additive);

        while (!scene.isDone) await UniTask.Yield();
        ActionProcess.ActiveSceneEvent = () => { SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(SceneNumber)); };

        scene.completed += async (ao) =>
        {
            //introProcess.LoadScene = SceneManager.GetSceneByBuildIndex(SceneNumber);
            await UniTask.Delay(TimeSpan.FromSeconds(1), DelayType.UnscaledDeltaTime);
            Debug.Log(SceneManager.GetSceneByBuildIndex(SceneNumber).name);

            fadePanel.blocksRaycasts = false;
            Time.timeScale = 1;
            changeComplateAction?.Invoke();
            //¾À È°¼ºÈ­

        };





    }

}
