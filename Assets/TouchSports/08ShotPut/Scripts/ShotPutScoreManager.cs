using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ML.T_Sports.Common;
namespace ML.T_Sports.ShotPut
{
    public enum PlayerState
    {
        Play,
        Finish
    }
    public class ShotPutScoreManager : MonoBehaviour
    {
        public PlayerState state;
        public int MyChance;
        public int MyScore;
        public float AddAllDistance;
        public Text ChanceText;
        public Text ScoreText;
        public Text DistanceText;

        public GameObject AngleGUI;
        public Animation Winner;
        public RectTransform RankRect;
        public BallCounter[] ballcounters;
        public int ballIdx;
        private void Awake()
        {
            DistanceText.text = "0m";
        }

        public void ResetScore()
        {
            MyScore = 0;
            AddAllDistance = 0;
            DistanceText.text = "0m";
            ballIdx = 0;
            ScoreText.text = "0";
            for (int i = 0; i < ballcounters.Length; i++)
                ballcounters[i].ResetCounter();
        }
        public void SetWinner()
        {
            AngleGUI.SetActive(false);
            Winner.Play("Winner");
        }
        public void SetAngleGUI(bool active)
        {
            AngleGUI.SetActive(active);
        }
        public void SetPlayScore()
        {
            ballIdx = 0;
            AngleGUI.SetActive(true);
            RankRect.anchoredPosition = new Vector2(0, -2000);
            ScoreText.text = "0";
            state = PlayerState.Play;
        }
        public void SetChance(int chanceValue)
        {
            MyChance = chanceValue;
            ChanceText.text = MyChance.ToString();
        }
        public void SetStopFinish()
        {
            MyChance = 0;
            state = PlayerState.Finish;
            ShotPutManager.instance.FinishCheck();
        }
        public void AddDistance(float newDistance)
        {
            MyChance -= 1;
            if (MyChance <= 0)
            {
                MyChance = 0;
                state = PlayerState.Finish;
                ShotPutManager.instance.FinishCheck();
            }
            AddAllDistance += newDistance;
            DistanceText.text = AddAllDistance.ToString("N1") + "m";

            int tmp = (int)newDistance / 5;
            //int addscore = tmp * 10;
            //MyScore += addscore;
            ScoreText.text = MyScore.ToString();
            ChanceText.text = MyChance.ToString();


            ballcounters[ballIdx].SetStrike();
            ballIdx++;
            if (ballIdx >= ballcounters.Length)
                ballIdx = 0;
        }
        public void SetBallOut()
        {
            ballcounters[ballIdx].SetBall();
            ballIdx++;
            if (ballIdx >= ballcounters.Length)
                ballIdx = 0;
            MyChance -= 1;
            if (MyChance <= 0)
            {
                MyChance = 0;
                state = PlayerState.Finish;
                ShotPutManager.instance.FinishCheck();
            }
            ChanceText.text = MyChance.ToString();
        }
    }

}
