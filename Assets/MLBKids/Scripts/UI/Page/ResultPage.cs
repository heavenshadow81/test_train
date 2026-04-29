using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ML.MLBKids
{
    public class ResultPage : Page
    {
        public Text scoreText;
        public GameObject[] fields;
        public Text[] fieldTitles;
        public Text[] fieldScores;
        public Text timerText;

        private List<Tween> _tweens = new List<Tween>();
        private float _timer = float.MaxValue;

        public override void Show()
        {
            // Shows tweens only once
            bool tween = !isShowing;
            _timer = float.MaxValue;

            base.Show();

            if (tween)
            {
                StartCoroutine(_Tween());
            }
        }

        public override void Hide()
        {
            if (isShowing)
            {
                _ClearTweens();
                StopAllCoroutines();
            }
            base.Hide();
        }

        private void _ClearTweens()
        {
            foreach (var tween in _tweens)
            {
                tween.Complete();
            }
            _tweens.Clear();
        }

        public void LateUpdate()
        {
            if (_timer < float.MaxValue)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0)
                {
                    Stadium.instance.Cleanup();
                    PageManager.instance.GoFromMenuToAds();
                }
                else
                {
                    timerText.text = Mathf.FloorToInt(_timer).ToString();
                }
            }
            else
            {
                timerText.text = "";
            }
        }

        private IEnumerator _Tween()
        {
            _ClearTweens();

            scoreText.text = "";
            var gameMode = Stadium.instance.gameMode;
            float[] scoreValues = null;
            if(gameMode == Stadium.GameMode.Hit)
            {
                fields[0].gameObject.SetActive(true);
                fields[1].gameObject.SetActive(true);
                fields[2].gameObject.SetActive(false);
                fieldTitles[0].text = "HOMERUN";
                fieldTitles[1].text = "STRIKE";
                fieldScores[0].text = "";
                fieldScores[1].text = "";
                scoreValues = new float[] { ScoreManager.instance.homerun, ScoreManager.instance.strike };
            }
            else if(gameMode == Stadium.GameMode.Pitch)
            {
                fields[0].gameObject.SetActive(true);
                fields[1].gameObject.SetActive(true);
                fields[2].gameObject.SetActive(false);
                fieldTitles[0].text = "STRIKE";
                fieldTitles[1].text = "BALL";
                fieldScores[0].text = "";
                fieldScores[1].text = "";
                scoreValues = new float[] { ScoreManager.instance.strike, ScoreManager.instance.ball };
            }
            else
            {
                fields[0].gameObject.SetActive(false);
                fields[1].gameObject.SetActive(false);
                fields[2].gameObject.SetActive(false);
                scoreValues = null;
            }

            yield return new WaitForSeconds(0.5f);

            Tween tween;
            tween = DOTween.To(x => scoreText.text = x.ToString("#,##0"), 0, ScoreManager.instance.score, 0.5f);
            _tweens.Add(tween);

            yield return new WaitForSeconds(0.7f);

            if(scoreValues != null)
            {
                for (int i = 0; i < scoreValues.Length; i++)
                {
                    float time = 0.6f / scoreValues.Length;
                    float scoreValue = scoreValues[i];
                    Text fieldScoreText = fieldScores[i];

                    tween = DOTween.To(x => fieldScoreText.text = x.ToString("0"), 0, scoreValue, time);
                    _tweens.Add(tween);

                    yield return new WaitForSeconds(time);
                }
            }
            else
            {
                yield return new WaitForSeconds(0.6f);
            }

            yield return new WaitForSeconds(0.2f);

            _tweens.Clear();
            _timer = GlobalConstants.instance.resultPageAnyActionWaitTime;
        }

        public void Replay()
        {
            Stadium stadium = Stadium.instance;
            if (stadium != null)
            {
                stadium.Cleanup();
                stadium.Init(stadium.gameMode);
                stadium.Play();
            }
        }

        public void Home()
        {
            Stadium stadium = Stadium.instance;
            if (stadium != null)
            {
                stadium.Cleanup();
            }
            PageManager.instance.ShowPage<MenuPage>();
        }
    }
}