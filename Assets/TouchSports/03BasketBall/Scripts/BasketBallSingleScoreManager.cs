using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ML.T_Sports.Common;
namespace ML.T_Sports.BasketBall
{
    public class BasketBallSingleScoreManager : MonoBehaviour
    {       
        public int Score;
        public Text ScoreText;

        public float Timer;
        public Text TimerText;
        
        public Animation GoalAnimation;
        public Animation NoGoalAnimation;

        public Animation PlusScore;
        public Image PlusScoreImage;
        public Sprite Plus2, Plus3;
        private void Awake()
        {
            Timer = 30;
            TimeTextSet();
        }
        private void Update()
        {
            if (BasketBallSingleManager.instance.state == BasketBallState.Play)
            {
                PlayTime();
            }
        }
        public void PlayTime()
        {
            Timer -= Time.deltaTime;
            TimeTextSet();
            BasketBallSingleManager.instance.CheckBalls();
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
        public void ResetScore()
        {
            Score = 0;
            ScoreText.text = Score.ToString();
            Timer = 0;
            TimerText.text = "00:00";

        }
        public void SetScoreBoxUI(bool goal, bool clean)
        {
            if (goal)
            {
                if (clean)
                {
                    PlusScoreImage.sprite = Plus3;
                    Score += 3;
                }
                else
                {
                    PlusScoreImage.sprite = Plus2;
                    Score += 2;
                }
                GoalAnimation.Play("Goal");

                PlusScore.Play("PlusScore");
            }
            else
            {
                NoGoalAnimation.Play("NoGoal");
            }
            ScoreText.text = Score.ToString();
        }
    }
}

