using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ML.T_Sports.Common;
namespace ML.T_Sports.BasketBall
{
    public class BasketBallTeamScoreManager : MonoBehaviour
    {
        public int Score;
        public Text ScoreText;


        public Animation GoalAnimation;
        public Animation NoGoalAnimation;

        public Animation PlusScore;
        public Image PlusScoreImage;
        public Sprite Plus2, Plus3;
       
        
        public void ResetScore()
        {
            Score = 0;
            ScoreText.text = Score.ToString();
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

                PlusScore.Play("TeamPlusScore");
            }
            else
            {
                NoGoalAnimation.Play("NoGoal");
            }
            ScoreText.text = Score.ToString();
        }
    }
}
