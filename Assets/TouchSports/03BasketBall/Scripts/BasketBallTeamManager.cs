using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ML.T_Sports.Common;

namespace ML.T_Sports.BasketBall
{
    public class BasketBallTeamManager : ContentsManagerBase
    {
        public BasketBallState state;
        public static BasketBallTeamManager instance;
        public BasketBallTeamScoreManager[] scores;
        public Animation Winner;
        public Image WinnerImage;
        public Sprite[] WinnerSpeite;
        public RectTransform WinnerRect;
        public GameObject[] Ready;
        public GameObject Explanation;
        public int PlayUser;
        public GameObject ReadyObject;

        public float Timer;
        public Text TimerText;
        public override void Init()
        {
            InitProperty(ContentsPropertyType.Time, 30, 3600, 30);
            InitProperty(ContentsPropertyType.GameMode, 0, 1, 1);
            SetPropertyValue(ContentsPropertyType.GameMode, 1);

            float sfx = GetSharedPropertyValueFloat(ContentsPropertyType.SFX);
            Common.SoundManager.instance.SetEFMVolume(sfx);
            float bgm = GetSharedPropertyValueFloat(ContentsPropertyType.BGM);
            Common.SoundManager.instance.SetBGMVolume(bgm);


            SetTimer(GetPropertyValue(ContentsPropertyType.Time));
            PlayUser = 1;
            state = BasketBallState.Ready;
            instance = this;
        }
        private void Update()
        {
            if (state == BasketBallState.Play)
            {
                PlayTime();
            }
        }
        void ResetTimer()
        {
            Timer = 0;
            TimerText.text = "00:00";
        }
        public void PlayTime()
        {
            Timer -= Time.deltaTime;
            TimeTextSet();
            BasketBallTeamManager.instance.CheckBalls();
        }
        public void TimeTextSet()
        {
            int Min = (int)Timer / 60;
            int Sec = (int)Timer % 60;
            if (Timer < 0)
            {
                TimerText.text = "00:00";
                BasketBallSingleManager.instance.state = BasketBallState.Finish;
            }
            else
            {
                string strMin = "";
                string strSec = "";
                if (Min < 10)
                {
                    strMin = "0" + Min.ToString();
                }
                else
                {
                    strMin = Min.ToString();
                }
                if (Sec < 10)
                {
                    strSec = "0" + Sec.ToString();
                }
                else
                {
                    strSec = Sec.ToString();
                }
                TimerText.text = strMin + ":" + strSec;
            }
        }
        public void SetTimer(float time)
        {
            Timer = time;
            TimeTextSet();
        }
        public void CheckBalls()
        {
            if (Timer <= 0)
            {
                //결과창 도출
                Invoke("SetFinish", 1);
            }
        }
        public void SetFinish()
        {
            if (scores[0].Score > scores[1].Score)
            {
                WinnerImage.sprite = WinnerSpeite[0];
            }
            else if (scores[0].Score < scores[1].Score)
            {
                WinnerImage.sprite = WinnerSpeite[1];
            }
            else if (scores[0].Score == scores[1].Score)
            {
                WinnerImage.sprite = WinnerSpeite[2];
            }
            Winner.Play("Winner");
            state = BasketBallState.Finish;
            if (IsPlaying)
                IsPlaying = false;
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
                case ContentsPropertyType.Time:
                    SetTimer(newValue);
                    break;
                case ContentsPropertyType.GameMode:
                    if (newValue == 1)
                        UnityEngine.SceneManagement.SceneManager.LoadScene("03BasketBallSingleMod");
                    break;
            }
            //Debug.Log(string.Format("Changed {0}_{1} value {2} -> {3}", ContentsName, type, prevValue, newValue));
        }
        public override void Play()
        {
            if (!IsPlaying)
            {
                base.Play();
                ReadyObject.SetActive(false);
                SetBasketBallPlay(GetPropertyValue(ContentsPropertyType.Time));
                state = BasketBallState.Play;
            }
        }

        public GameObject PauseCanvas;
        public override void Pause()
        {
            base.Pause();
            PauseCanvas.SetActive(IsPaused);
        }
        public override void Stop()
        {
            base.Stop();
            SetFinish();
        }
        //플레이 준비
        public void SetBasketBallPlay(float time)
        {
            //두 팀중 한팀이라도 점수가 있다면(리셋 상황)
            if (scores[0].Score > 0 || scores[1].Score > 0)
                ResetGame();

            WinnerRect.anchoredPosition = new Vector2(0, -2000);
            Timer = time;
            Ready[0].SetActive(false);
            Ready[1].SetActive(false);
            Explanation.SetActive(false);
            state = BasketBallState.Play;
        }

        public void ResetGame()
        {
            WinnerRect.anchoredPosition = new Vector2(0, -2000);
            scores[0].ResetScore();
            scores[1].ResetScore();
            ResetTimer();
            state = BasketBallState.Ready;
        }
    }
}


