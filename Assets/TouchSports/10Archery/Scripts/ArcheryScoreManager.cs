using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ML.T_Sports.Common;
namespace ML.T_Sports.Archery
{
    public enum PlayerState
    {
        Play,
        Finish,
        Pause,
        Ready
    }
    public class ArcheryScoreManager : MonoBehaviour
    {
        public PlayerState state;
        public int MyScore;
        public Text MyScoreText;
        public int MyChance;
        public Text MyChanceText;

        public BallCounter[] ballcounters;
        public int ballidx;
        public RectTransform FinishRect;
        public Animation FinishAnimation;
        public Animation PointUp;
        public Image GetPoint;
        public Sprite[] PointSprites;
        private void Awake()
        {
            state = PlayerState.Ready;
        }
        public void Init(int chance)
        {
            for (int i = 0; i < ballcounters.Length; i++)
                ballcounters[i].ResetCounter();
            FinishRect.anchoredPosition = new Vector2(0, -2000);
            state = PlayerState.Play;
            ballidx = 0;
            MyScoreText.text = "0";
            MyScore = 0;
            MyChance = chance;
            MyChanceText.text = MyChance.ToString();
        }

        public void SetChance(int chance)
        {
            MyChance = chance;
            MyChanceText.text = MyChance.ToString();
        }
        public void AddScore(int addScore)
        {
            MyScore += addScore;
            MyScoreText.text = MyScore.ToString();
            MyChance -= 1;
            int point = addScore - 1;
            if (addScore != 0)
            {
                GetPoint.sprite = PointSprites[point];
                PointUp.Play("GetPoint");
                ballcounters[ballidx].SetStrike();
                ballidx++;
            }
            else
            {
                ballcounters[ballidx].SetBall();
                ballidx++;
            }
            if (MyChance <= 0)
            {
                ArcheryTeamManager.instance.CheckPlayerState();
                MyChanceText.text = MyChance.ToString();
                state = PlayerState.Finish;
                FinishAnimation.Play("Finish");
                return;
            }
            MyChanceText.text = MyChance.ToString();

            
        }
    }

}
