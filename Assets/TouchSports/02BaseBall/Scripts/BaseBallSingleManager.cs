using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ML.T_Sports.Common;
namespace ML.T_Sports.BaseBall
{
    public enum BasaeBallState
    {
        Ready,
        Play,
        Pause,
        Finish
    }
    public class BaseBallSingleManager : ContentsManagerBase
    {
        public static BaseBallSingleManager instance;
        public BasaeBallState state;
        public GameObject Player_A;
        public Camera Camera_A, UI_A;

        public GameObject Player_B;
        public Camera Camera_B, UI_B;

        public GameObject Player_C;
        public Camera Camera_C, UI_C;
        public ScreenTouchPoint touchs;
        public ScoreManager[] scores;
        public Animation[] Winner;
        public RectTransform[] WinnerRect;
        public GameObject[] loadings;
        public RectTransform[] ScoreBoxs;
        public int PlayUser;

        public GameObject[] VerticalLine;
        //시작을 담당하는 부분이 이곳에 나뉘어있음...?..
        

        public override void Init()
        {
            InitProperty(ContentsPropertyType.Chance, 3, 10, 10);
            InitProperty(ContentsPropertyType.Player, 1, 3, 1);
            //싱글 일때 마지막 0, 팀일때 1
            InitProperty(ContentsPropertyType.GameMode, 0, 1, 0);
            SetPropertyValue(ContentsPropertyType.GameMode, 0);

            float sfx = GetSharedPropertyValueFloat(ContentsPropertyType.SFX);
            Common.SoundManager.instance.SetEFMVolume(sfx);
            float bgm = GetSharedPropertyValueFloat(ContentsPropertyType.BGM);
            Common.SoundManager.instance.SetBGMVolume(bgm);

            PlayUser = 1;
            instance = this;
            Player_B.SetActive(false);
            Player_C.SetActive(false);
            state = BasaeBallState.Ready;

            for (int i = 0; i < scores.Length; i++)
            {
               //loadings[i].SetActive(false);
                scores[i].SetChance(GetPropertyValueInt(ContentsPropertyType.Chance));
            }
            for (int i = 0; i < VerticalLine.Length; i++)
                VerticalLine[i].SetActive(false);
            SetPlayers(GetPropertyValueInt(ContentsPropertyType.Player));

            
        }
        

        public void CheckBalls()
        {
            int tmp = 0;
            for (int i = 0; i < PlayUser; i++)
            {
                if (scores[i].BallChance > 0)
                    tmp += 1;
            }
            if (tmp == 0)
            {
                //결과창 도출
                Invoke("SetResult", 1);
            }
        }
        public void SetResult()
        {
            base.Stop();
            state = BasaeBallState.Finish;
            int max = 0;
            for (int i = 0; i < scores.Length; i++)
            {
                if (scores[i].Score > max)
                {
                    max = scores[i].Score;
                }
            }
            for (int i = 0; i < scores.Length; i++)
            {
                if (scores[i].Score == max)
                {
                    Winner[i].Play("Winner");
                }
            }
            state = BasaeBallState.Finish;
            
        }
        public void WinnerReset()
        {
            for (int i = 0; i < WinnerRect.Length; i++)
                WinnerRect[i].anchoredPosition = new Vector2(0, -1000);
        }
        public GameObject PauseCanvas;
        public override void Pause()
        {
            base.Pause();
            PauseCanvas.SetActive(IsPaused);
        }

        public override void OnChangeSharedPropertyValue(ContentsPropertyType type, float prevValue, float newValue)
        {
            switch (type)
            {
                case ContentsPropertyType.SFX:
                    Common.SoundManager.instance.SetEFMVolume(newValue);
                    break;
                case ContentsPropertyType.BGM:
                    Common.SoundManager.instance.SetBGMVolume(newValue);
                    break;
            }
        }
        public override void OnChangePropertyValue(ContentsPropertyType type, float prevValue, float newValue)
        {
            switch (type)
            {
                case ContentsPropertyType.Chance:
                    loadings[0].SetActive(false);
                    for (int i = 0; i < scores.Length; i++)
                    {
                        scores[i].SetChance((int)newValue);
                    }
                    ResetGame();
                    break;
                case ContentsPropertyType.Player:
                    SetPlayers((int)newValue);
                    touchs.players = (int)newValue;
                    break;
                case ContentsPropertyType.GameMode:
                    if (newValue == 1)
                        UnityEngine.SceneManagement.SceneManager.LoadScene("12BaseBallTeamMod");
                    break;
            }
            //Debug.Log(string.Format("Changed {0}_{1} value {2} -> {3}", ContentsName, type, prevValue, newValue));
        }
        public override void Play()
        {
            if (!IsPlaying)
            {
                base.Play();
                ResetGame();
                state = BasaeBallState.Play;
                int chance = GetPropertyValueInt(ContentsPropertyType.Chance);
                loadings[0].SetActive(false);
                for (int i = 0; i < scores.Length; i++)
                {
                    scores[i].SetChance(chance);
                    scores[i].SetBonusZone();
                }
            }
        }
        public override void Stop()
        {
            SetResult();
            //ResultCheck();

            //준비 버튼에 들어가야함.
            //SetHightSetting();
        }
        public void SetPlayers(int Count)
        {
            PlayUser = Count;
            switch (Count)
            {
                case 1:
                    Player_B.SetActive(false);
                    Player_C.SetActive(false);
                    Camera_A.rect = new Rect(0, 0, 1, 1);
                    UI_A.rect = Camera_A.rect;
                    touchs.players = 1;
                    ScoreBoxs[0].anchoredPosition = new Vector2(0, 420);
                    ScoreBoxs[0].localScale = new Vector3(1f, 1f, 1f);

                    for (int i = 0; i < VerticalLine.Length; i++)
                        VerticalLine[i].SetActive(false);
                    break;
                case 2:
                    Player_B.SetActive(true);
                    Player_C.SetActive(false);
                    Camera_A.rect = new Rect(0, 0, 0.5f, 1);
                    Camera_B.rect = new Rect(0.5f, 0, 1, 1);
                    UI_A.rect = Camera_A.rect;
                    UI_B.rect = Camera_B.rect;
                    touchs.players = 2;
                    ScoreBoxs[0].anchoredPosition = new Vector2(0, 435);
                    ScoreBoxs[0].localScale = new Vector3(0.75f, 0.75f, 0.75f);
                    ScoreBoxs[1].anchoredPosition = new Vector2(0, 435);
                    ScoreBoxs[1].localScale = new Vector3(0.75f, 0.75f, 0.75f);
                    VerticalLine[0].SetActive(true);
                    VerticalLine[1].SetActive(false);
                    VerticalLine[2].SetActive(false);
                    break;
                case 3:
                    Player_B.SetActive(true);
                    Player_C.SetActive(true);
                    Camera_A.rect = new Rect(0, 0, 0.33f, 1);
                    Camera_B.rect = new Rect(0.33f, 0, 0.33f, 1);
                    Camera_C.rect = new Rect(0.66f, 0, 1, 1);
                    UI_A.rect = Camera_A.rect;
                    UI_B.rect = Camera_B.rect;
                    UI_C.rect = Camera_C.rect;
                    touchs.players = 3;
                    ScoreBoxs[0].anchoredPosition = new Vector2(0, 465);
                    ScoreBoxs[0].localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    ScoreBoxs[1].anchoredPosition = new Vector2(0, 465);
                    ScoreBoxs[1].localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    ScoreBoxs[2].anchoredPosition = new Vector2(0, 465);
                    ScoreBoxs[2].localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    VerticalLine[0].SetActive(false);
                    VerticalLine[1].SetActive(true);
                    VerticalLine[2].SetActive(true);
                    break;
            }
        }
        public void ResetGame()
        {
            WinnerReset();
            for (int i = 0; i < scores.Length; i++)
            {
                scores[i].ResetScore();
            }
           // ControllerManager.instance.SetState(2);
            state = BasaeBallState.Ready;
        }
        //강제 시작
        
    }
}

