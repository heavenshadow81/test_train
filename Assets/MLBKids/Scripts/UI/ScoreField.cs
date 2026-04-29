using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ML.MLBKids
{
    public class ScoreField : MonoBehaviour
    {
        public Text scoreLabel;
        public Text scoreEffect;
        
        private Tween _scoreTween;
        private List<Text> _scoreEffects = new List<Text>();

        public void OnEnable()
        {
            scoreEffect.gameObject.SetActive(false);
            scoreLabel.text = ScoreManager.currentScore.ToString("#,##0");

            ScoreManager.onAddScore += _OnAddScore;
        }

        public void OnDisable()
        {
            if (_scoreTween != null)
            {
                _scoreTween.Kill();
                _scoreTween = null;
            }
            foreach (var scoreEff in _scoreEffects)
            {
                if (scoreEff.gameObject.activeSelf)
                    DOTween.Kill(scoreEff);
                Destroy(scoreEff.gameObject);
            }
            _scoreEffects.Clear();

            ScoreManager.onAddScore -= _OnAddScore;
        }

        private void _OnAddScore(int score, int add, ScoreManager.ScoreType scoreType)
        {
            if (_scoreTween != null)
            {
                _scoreTween.Kill();
                _scoreTween = null;
            }
            scoreLabel.text = string.Format((score - add).ToString("#,##0"));
            _scoreTween = DOTween.To(x => scoreLabel.text = x.ToString("#,##0"), score - add, score, add / 50.0f);
            _ShowScoreEffect(add, scoreType);
        }

        private void _ShowScoreEffect(int score, ScoreManager.ScoreType scoreType)
        {
            if (score == 0) return;

            // Prepare the score effect properties
            Color scoreEffectColor = Color.white;
            int scoreEffectSize = scoreEffect.fontSize;
            float animSpeed = 1.0f; // [0~1]
            switch (scoreType)
            {
                case ScoreManager.ScoreType.Nice:
                    scoreEffectColor = Color.yellow;
                    scoreEffectSize += 30;
                    animSpeed = 0.8f;
                    break;
                case ScoreManager.ScoreType.HomeRun:
                case ScoreManager.ScoreType.Strike:
                    scoreEffectColor = Color.green;
                    scoreEffectSize += 70;
                    animSpeed = 0.5f;
                    break;
            }

            // Spawn a new score effect.
            Text newScoreEffect = null;
            foreach (var scoreEff in _scoreEffects)
            {
                if (!scoreEff.gameObject.activeSelf)
                {
                    newScoreEffect = scoreEff;
                    break;
                }
            }
            if (newScoreEffect == null)
            {
                newScoreEffect = Instantiate<Text>(scoreEffect, scoreEffect.rectTransform.parent);
                _scoreEffects.Add(newScoreEffect);
            }

            // Set properties of score effect.
            newScoreEffect.gameObject.SetActive(true);
            newScoreEffect.name = string.Format("score(+{0})", score);
            newScoreEffect.text = score.ToString("+0");
            newScoreEffect.rectTransform.SetSiblingIndex(newScoreEffect.rectTransform.parent.childCount - 1);
            newScoreEffect.rectTransform.localPosition = scoreEffect.rectTransform.localPosition;
            newScoreEffect.color = scoreEffectColor - new Color(0, 0, 0, 1);
            newScoreEffect.fontSize = scoreEffectSize;

            // Add tweens.
            Sequence seq = DOTween.Sequence();
            seq.SetTarget(newScoreEffect);
            seq.Insert(0, newScoreEffect.DOColor(scoreEffectColor, 0.1f));
            seq.Insert(0, newScoreEffect.rectTransform.DOLocalMoveY(scoreEffect.rectTransform.localPosition.y + (scoreEffectSize - 20) * (2.0f - animSpeed), 0.5f));
            seq.Insert(0.5f, newScoreEffect.rectTransform.DOLocalMoveY(scoreEffect.rectTransform.localPosition.y + 200 * (2.0f - animSpeed), 0.25f).SetEase(Ease.OutSine));
            seq.Insert(0.5f, newScoreEffect.DOColor(scoreEffectColor - new Color(0, 0, 0, 1), 0.2f).SetEase(Ease.OutSine));
            seq.OnComplete(() => { newScoreEffect.gameObject.SetActive(false); });
            seq.timeScale = animSpeed;
            seq.Play();
        }
    }
}