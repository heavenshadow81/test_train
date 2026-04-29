using UnityEngine;
using UnityEngine.UI;

namespace ML.T_Sports.VolleyBall
{
    using Common;
    public class VolleyBallScoreboard : MonoBehaviour
    {
        public Renderer board;
        public Text teamAScore, teamBScore;
        
        public VolleyBallGameManager gameManager
        {
            get { return (VolleyBallGameManager)ContentsManagerBase.Current; }
        }

        public void Start()
        {
            teamAScore.text = teamBScore.text = "0";
        }

        public void Update()
        {
            if (gameManager != null)
            {
                teamAScore.text = string.Format("{0}", gameManager.score);
                teamBScore.text = string.Format("{0}", gameManager.fail);
            }
        }
    }
}