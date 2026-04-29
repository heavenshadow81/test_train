using UnityEngine;
using DG.Tweening;

namespace ML.MLBKids
{
    public class StrikeZone : MonoBehaviour
    {
        public SpriteRenderer innerRange, outerRange;
        public GameObject effect;

        public void Awake()
        {
            if (effect != null)
                effect.gameObject.SetActive(false);
        }

        public void OnTriggerEnter(Collider other)
        {
            PlayerBall ball = other.GetComponentInParent<PlayerBall>();
            if (ball != null && !ball.hit)
            {
                ball.hit = true;

                Rigidbody rgd = ball.GetComponentInChildren<Rigidbody>();
                if (rgd != null && rgd.linearVelocity.magnitude < 1.0f)
                {
                    Debug.Log("Ball is too slow. ignored..");
                    return;
                }

                Bounds innerBounds = innerRange.bounds;
                Bounds outerBounds = outerRange.bounds;
                innerBounds.size = new Vector3(innerBounds.size.x, innerBounds.size.y, 10000.0f);
                outerBounds.size = new Vector3(outerBounds.size.x, outerBounds.size.y, 10000.0f);

                Debug.Log("inner : " + innerBounds.ToString("0.00"));
                Debug.Log("outer : " + outerBounds.ToString("0.00"));

                var ballPosInStrikeZone = ball.transform.position;

                int score = 0;
                ScoreManager.ScoreType scoreType = ScoreManager.ScoreType.Normal;
                if (innerBounds.Contains(ballPosInStrikeZone))
                {
                    scoreType = ScoreManager.ScoreType.Strike;
                    score = ScoreManager.instance.ballStrikeNiceScore;
                }
                else if(outerBounds.Contains(ballPosInStrikeZone))
                {
                    scoreType = ScoreManager.ScoreType.Nice;
                    score = ScoreManager.instance.ballStrikeNormalScore;
                }
                else
                {
                    scoreType = ScoreManager.ScoreType.Ball;
                    score = ScoreManager.instance.ballBallScore;
                }

                ScoreManager.AddScore(score, scoreType);

                if (scoreType != ScoreManager.ScoreType.Ball)
                {
                    SoundManager.PlaySFX(string.Format("catcher_mitt_strong{0}", (Random.Range(0.0f, 1.0f) < 0.5f ? "2" : "")), false, 0, 1.0f, 1.0f);
                    SoundManager.PlaySFX("crowd_hit");
                    var go = Instantiate<GameObject>(effect, transform, true);
                    go.SetActive(true);
                    go.transform.position = ball.transform.position;
                    Destroy(go, 0.5f);
                }
                
                if (scoreType != ScoreManager.ScoreType.Ball)
                {
                    Rigidbody rigidbody = ball.GetComponent<Rigidbody>();
                    rigidbody.isKinematic = true;

                    Sequence seq = DOTween.Sequence();
                    seq.SetTarget(ball);
                    seq.Append(ball.transform.DOScale(ball.transform.localScale * 3.0f, 0.3f).SetEase(Ease.OutBounce));
                    seq.AppendInterval(0.2f);
                    seq.OnComplete(() => { if (ball.gameObject != null) Destroy(ball.gameObject); });
               
                    if (ball.effectPrefab != null)
                    {
                        var effect = Instantiate<GameObject>(ball.effectPrefab);
                        effect.transform.position = ball.transform.position;
                        Destroy(effect, 5.0f);
                    }

                    if (ball.effectPrefab2 != null)
                    {
                        var effect = Instantiate<GameObject>(ball.effectPrefab2);
                        effect.transform.position = ball.transform.position;
                        Destroy(effect, 5.0f);
                    }
                }
            }
        }
    }
}