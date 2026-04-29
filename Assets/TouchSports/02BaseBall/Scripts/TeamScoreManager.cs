using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ML.T_Sports.Common;
namespace ML.T_Sports.BaseBall
{
    public class TeamScoreManager : MonoBehaviour
    {
        public enum Turn
        {
            TeamA,
            TeamB
        }
        public Turn turn;
        public Text PlayRound;
        public int BallChance;
        public int Round;

        public Text ScoreAText;
        public Text ScoreBText;
        public int ScoreA, ScoreB;

        public Text StrikeAText, BallAText;
        public Text StrikeBText, BallBText;
        public int StrikeA, StrikeB;
        public int BallA, BallB;


        public int ComboA, ComboB;
        public int num;

        public RectTransform Strike;
        public RectTransform Ball;
        public Animation StrikeAnimation;
        public Animation BallAnimation;
        
        public GameObject[] BonusZone;
        public bool[] BonusZoneActivate;
        private void Awake()
        {
            BonusZoneActivate = new bool[BonusZone.Length];
            ResetBonusZone();
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
            PlayRound.text = Round.ToString();
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

        public void ResetScore()
        {
            ResetBonusZone();
            turn = Turn.TeamA;
            BallChance = BaseBallTeamManager.instance.GetPropertyValueInt(ContentsPropertyType.Chance);
            Round = 1;
            ComboA = 0;
            ComboB = 0;
            ScoreA = 0;
            ScoreB = 0;
            StrikeA = 0;
            StrikeB = 0;
            BallA = 0;
            BallB = 0;
            StrikeAText.text = StrikeA.ToString();
            StrikeBText.text = StrikeB.ToString();

            BallAText.text = BallA.ToString();
            BallBText.text = BallB.ToString();

            PlayRound.text = Round.ToString();
            ScoreAText.text = ScoreA.ToString();
            ScoreBText.text = ScoreB.ToString();
        }
        public void UsingBallCount()
        {
            if (turn == Turn.TeamB)
            {
                BallChance -= 1;
                if(BallChance != 0)
                    Round++;
            }
                
            PlayRound.text = Round.ToString();
        }
        public void SetScoreBoxUI(bool strike, int bonus)
        {
            if (strike)
            {
                if (turn == Turn.TeamA)
                {
                    ComboA += 1;
                    ScoreA += 10 + bonus + (100 * ComboA);
                    StrikeA += 1;
                    StrikeAnimation.Play("Strike");
                    //APoint10.Play("10Point");
                    ScoreAText.text = ScoreA.ToString();
                    StrikeAText.text = StrikeA.ToString();
                }
                else
                {
                    ComboB += 1;
                    ScoreB += 10 + bonus + (100 * ComboB);
                    StrikeB += 1;
                    StrikeAnimation.Play("Strike");
                    //BPoint10.Play("10Point");
                    ScoreBText.text = ScoreB.ToString();
                    StrikeBText.text = StrikeB.ToString();
                }
            }
            else
            {
                if (turn == Turn.TeamA)
                {
                    ComboA = 0;
                    BallA++;
                    BallAText.text = BallA.ToString();
                }     
                else
                {
                    ComboB = 0;
                    BallB++;
                    BallBText.text = BallB.ToString();
                }
                BallAnimation.Play("Strike");
            }
            ResetBonusZone();
            SetBonusZone();
            BaseBallTeamManager.instance.CheckBalls();
        }
    }
}

