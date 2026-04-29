using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ML.T_Sports.Common;
namespace ML.T_Sports.BaseBall
{
    public class ScoreManager : MonoBehaviour
    {
        public BallCounter[] ballcounters;
        public int BallChance;
        public int Score;

        public int idx;
        public Text BallChanceText;
        public Text ScoreText;
        public int Combo;
        public int num;

        public RectTransform Strike;
        public RectTransform Ball;
        public Animation StrikeAnimation;
        public Animation BallAnimation;
        public Animation Point10;
        public Image PointIamge;
        public Sprite Point5spt, Point10spt;

        public GameObject[] BonusZone;
        public bool[] BonusZoneActivate;
        //점수 관리자 시작
        void Awake()
        {
            BonusZoneActivate = new bool[BonusZone.Length];
            ResetBonusZone();
            Combo = 0;
            BallChance = 10;
            idx = 0;
        }

        public void ResetBonusZone()
        {
            for (int i = 0; i < BonusZoneActivate.Length; i++)
            {
                BonusZoneActivate[i] = false;
                BonusZone[i].SetActive(false);
            }
        }
        public void SetBonusZone()
        {
            int idx = Random.Range(0, BonusZoneActivate.Length);
            BonusZoneActivate[idx] = true;
            BonusZone[idx].SetActive(true);
        }
        public int GetActivateZone()
        {
            int idx = 0;
            for (int i = 0; i < BonusZoneActivate.Length; i++)
            {
                if (BonusZoneActivate[i])
                {
                    idx = i;
                }
            }
            return idx;
        }
        public void SetChance(int chance)
        {
            BallChance = chance;
            BallChanceText.text = BallChance.ToString();
        }
        public void ResetScore()
        {
            ResetBonusZone();
            Combo = 0;
            BallChance = BaseBallSingleManager.instance.GetPropertyValueInt(ContentsPropertyType.Chance);
            idx = 0;
            Score = 0;
            BallChanceText.text = BallChance.ToString();
            ScoreText.text = Score.ToString();
            for (int i = 0; i < ballcounters.Length; i++)
                ballcounters[i].ResetCounter();
        }
        public void UsingBallCount()
        {
            BallChance -= 1;
            BallChanceText.text = BallChance.ToString();
        }
        public void SetScoreBoxUI(bool strike, int bonus)
        {
            if (strike)
            {
                if (bonus == 0)
                    PointIamge.sprite = Point5spt;
                else
                    PointIamge.sprite = Point10spt;
                Combo += 1;
                Score += 5 + bonus;// + (100 * Combo);
                ballcounters[idx].SetStrike();
             //   Ball.position = new Vector3(0,200,-2000);
              //  Strike.position = new Vector3(0, 200, -2000);
                StrikeAnimation.Play("Strike");
                Point10.Play("10Point");
            }
            else
            {
                Combo = 0;
                ballcounters[idx].SetBall();
              //  Strike.position = new Vector3(0, 200, -2000);
             //   Ball.position = new Vector3(0, 200, -2000);
                BallAnimation.Play("Strike");
            }
            ResetBonusZone();
            SetBonusZone();

            idx++;
            ScoreText.text = Score.ToString();
            BaseBallSingleManager.instance.CheckBalls();
        }
    }

}
