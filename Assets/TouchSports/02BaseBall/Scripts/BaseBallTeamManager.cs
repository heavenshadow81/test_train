using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ML.T_Sports.Common;
namespace ML.T_Sports.BaseBall
{
    public class BaseBallTeamManager : ContentsManagerBase
    {
        public BasaeBallState state;
        public bool Pitching;
        public ScreenTouchPoint touch;
        public TeamScoreManager scores;
        public GameObject Ready;
        public GameObject StrikeZone;

        public Image WinnerImage;
        public Sprite WinnerA, WinnerB, Draw;
        public Animation Winner;
        public RectTransform WinnerRect;
        public Animation TeamShift;
        public GameObject TeamAReady, TeamBReady;
        public static BaseBallTeamManager instance;

        // Use this for initialization
        public override void Init()
        {
            InitProperty(ContentsPropertyType.Chance, 3, 9, 3);
            //기획자 요청으로 인한 주석 처리
           // InitProperty(ContentsPropertyType.Player, 1, 1, 1);
            InitProperty(ContentsPropertyType.GameMode, 0, 1, 1);
            SetPropertyValue(ContentsPropertyType.GameMode, 1);

            float sfx = GetSharedPropertyValueFloat(ContentsPropertyType.SFX);
            Common.SoundManager.instance.SetEFMVolume(sfx);
            float bgm = GetSharedPropertyValueFloat(ContentsPropertyType.BGM);
            Common.SoundManager.instance.SetBGMVolume(bgm);


            instance = this;
            Pitching = false;
            state = BasaeBallState.Ready;
            scores.ResetScore();
        }

        public GameObject PauseCanvas;
        public override void Pause()
        {
            base.Pause();
            PauseCanvas.SetActive(IsPaused);
        }
        //스트라이크 이후 기회가 몇번남았는지 체크 함.
        public void CheckBalls()
        {
            if (scores.BallChance == 0)
            {
                //결과창 도출
                Invoke("SetResult", 1);
            }
            else
            {
                if (scores.turn == TeamScoreManager.Turn.TeamA)
                    scores.turn = TeamScoreManager.Turn.TeamB;
                else
                    scores.turn = TeamScoreManager.Turn.TeamA;
                StartCoroutine(WaitForSecond_TeamReady());
            }
        }
        IEnumerator WaitForSecond_TeamReady()
        {
            yield return new WaitForSeconds(1f);
            SetTeamReady();
        }
        public void SetResult()
        {
            if (IsPlaying)
                IsPlaying = false;
            if (scores.ScoreA > scores.ScoreB)
            {
                WinnerImage.sprite = WinnerA;
            }
            else if (scores.ScoreA < scores.ScoreB)
            {
                WinnerImage.sprite = WinnerB;
            }
            else if (scores.ScoreA == scores.ScoreB)
            {
                WinnerImage.sprite = Draw;
            }
            //TeamAReady.SetActive(false);
            //TeamBReady.SetActive(false);
            Winner.Play("TeamWinner");

            state = BasaeBallState.Finish;
        }
        public void WinnerReset()
        {
            WinnerRect.anchoredPosition = new Vector2(0, -1000);
        }

        public override void Play()
        {
            if (!IsPlaying)
            {
                base.Play();
                SetBallChance(GetPropertyValueInt(ContentsPropertyType.Chance));
                state = BasaeBallState.Play;
                scores.SetBonusZone();
            }
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
                case ContentsPropertyType.GameMode:
                    if (newValue == 0)
                        UnityEngine.SceneManagement.SceneManager.LoadScene("02BaseBallSingleMod");
                    break;
            }
            //Debug.Log(string.Format("Changed {0}_{1} value {2} -> {3}", ContentsName, type, prevValue, newValue));
        }
        public override void Stop()
        {
            base.Stop();
            SetResult();
            //ResultCheck();

            //준비 버튼에 들어가야함.
            //SetHightSetting();
        }
        public void SetBallChance(int chance)
        {
            scores.SetChance(chance);
            Ready.SetActive(false);
            StrikeZone.SetActive(true);
            ResetGame();
            SetTeamReady();
        }

        public void SetTeamReady()
        {
            if (scores.turn == TeamScoreManager.Turn.TeamA)
            {
                TeamShift.Play("Blue");
            }
            else if (scores.turn == TeamScoreManager.Turn.TeamB)
            {
                TeamShift.Play("Red");

            }
            StartCoroutine(SetTeamReadySecond());
        }
        IEnumerator SetTeamReadySecond()
        {
            yield return new WaitForSeconds(1.2f);
            Pitching = true;
        }

        public void ResetGame()
        {
            WinnerReset();
            scores.ResetScore();            
           
            state = BasaeBallState.Ready;
        }
    }
}

