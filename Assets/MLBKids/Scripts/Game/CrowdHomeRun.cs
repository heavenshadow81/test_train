using UnityEngine;

namespace ML.MLBKids
{
    public class CrowdHomeRun : MonoBehaviour
    {
        public void OnTriggerEnter(Collider other)
        {
            Ball ball = other.GetComponentInParent<Ball>();
            if (ball != null)
            {
                Stadium stadium = Stadium.instance;
                if (stadium.isPlaying)
                {
                    ScoreManager.AddScore(ScoreManager.instance.ballHomeRunScore, ScoreManager.ScoreType.HomeRun);
                }
                Destroy(ball.gameObject, 2.0f);
                SoundManager.PlaySFX("crowd_hit");
            }
        }
    }
}