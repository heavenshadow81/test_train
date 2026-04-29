using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ML.MLBKids
{
    public class HitGameScorePage : Page
    {
        public RectTransform top, mid, bottom;
        public Image timerSlider;
        public Image clock, clock2;
        public Image strike, foul, homerun;
        public Text timerReducedText;

        private List<Tween> _countdownTweens = new List<Tween>();

        public override void Show()
        {
            if (!isShowing)
            {
                mid.gameObject.SetActive(false);
                strike.gameObject.SetActive(false);
                foul.gameObject.SetActive(false);
                homerun.gameObject.SetActive(false);
                timerReducedText.gameObject.SetActive(false);

                Vector3 pos = Vector3.zero;

                pos = top.anchoredPosition;
                pos.y = 400.0f;
                top.anchoredPosition = pos;
                pos = bottom.anchoredPosition;
                pos.y = -400.0f;
                bottom.anchoredPosition = pos;

                top.DOAnchorPosY(0, 0.25f);
                bottom.DOAnchorPosY(0, 0.25f);

                ScoreManager.onAddScore += _OnAddScore;

                clock.rectTransform.rotation = Quaternion.identity;
                clock2.color = Color.white;

                Timer.onTimer += _OnTimer;
                Timer.onTimerCountdown += _OnTimerCountdown;
                Timer.onTimerEnd += _OnTimerEnd;
                Timer.onTimerAppend += _OnTimerAppend;
            }
            base.Show();
        }

        public override void Hide()
        {
            if (isShowing)
            {
                foreach (var tween in _countdownTweens)
                {
                    tween.Kill();
                }
                _countdownTweens.Clear();

                Timer.onTimer -= _OnTimer;
                Timer.onTimerCountdown -= _OnTimerCountdown;
                Timer.onTimerEnd -= _OnTimerEnd;
                Timer.onTimerAppend -= _OnTimerAppend;
            }
            base.Hide();
        }

        private void _OnTimer(float time, float totalTime)
        {
            timerSlider.fillAmount = 1.0f - (time / totalTime) * 0.95f;
            if (clock2 != null)
            {
                var pos = clock2.rectTransform.localPosition;
                pos.x = timerSlider.preferredWidth * (timerSlider.fillAmount - 0.5f);
                clock2.rectTransform.localPosition = pos;
            }
        }

        private void _OnTimerCountdown(float time, float totalTime)
        {
            _countdownTweens.Add(clock2.DOColor(Color.red, 0.25f).SetLoops(-1, LoopType.Yoyo));
            Sequence seq = DOTween.Sequence();
            seq.Append(clock.rectTransform.DORotate(new Vector3(0, 0, -30), 0.25f).SetEase(Ease.Linear));
            seq.Append(clock.rectTransform.DORotate(new Vector3(0, 0, 30), 0.5f).SetEase(Ease.Linear));
            seq.Append(clock.rectTransform.DORotate(new Vector3(0, 0, 0), 0.25f).SetEase(Ease.Linear));
            seq.SetLoops(-1);
            _countdownTweens.Add(seq);
            timerSlider.fillAmount = 1.0f - (time / totalTime) * 0.95f;
        }

        private void _OnTimerEnd()
        {
            if (isShowing)
            {
                mid.gameObject.SetActive(true);
                mid.localScale = Vector3.one * 0.01f;
                mid.DOScale(1.0f, 0.25f);
                
                top.DOAnchorPosY(400, 0.25f);
                bottom.DOAnchorPosY(-400, 0.25f);
            }
        }

        private void _OnTimerAppend(float time, float append)
        {
            if (isShowing)
            {
                var effect = Instantiate<Text>(timerReducedText, timerReducedText.transform.parent, true);
                effect.gameObject.SetActive(true);
                effect.rectTransform.DOLocalRotate(new Vector3(0.0f, 0.0f, -50.0f), 0.66f).SetEase(Ease.Linear);
                effect.rectTransform.DOLocalMoveY(effect.rectTransform.localPosition.y - 140.0f, 0.66f).SetEase(Ease.InSine);
                Color toColor = effect.color;
                toColor.a = 0;
                effect.DOColor(toColor, 0.33f).SetDelay(0.33f);
                Destroy(effect.gameObject, 1.0f);
            }
        }

        private void _OnAddScore(int score, int add, ScoreManager.ScoreType scoreType)
        {
            switch (scoreType)
            {
                case ScoreManager.ScoreType.Strike:
                    {
                        if (strike != null)
                        {
                            var effect = Instantiate<Image>(strike, strike.transform.parent, true);
                            effect.gameObject.SetActive(true);
                            effect.rectTransform.localScale = Vector3.zero;
                            Sequence seq = DOTween.Sequence();
                            seq.Append(effect.rectTransform.DOScale(1.0f, 0.75f).SetEase(Ease.OutSine));
                            seq.Append(effect.rectTransform.DOScale(1.2f, 0.25f).SetEase(Ease.Linear));
                            seq.Insert(0.75f, effect.DOColor(new Color(1.0f, 1.0f, 1.0f, 0.0f), 0.25f));
                            seq.OnComplete(() =>
                            {
                                Destroy(effect.gameObject);
                            });
                            seq.Play();
                        }
                    }
                    break;
                case ScoreManager.ScoreType.HomeRun:
                    {
                        if (homerun != null)
                        {
                            var effect = Instantiate<Image>(homerun, strike.transform.parent, true);
                            effect.gameObject.SetActive(true);
                            effect.rectTransform.localScale = Vector3.zero;
                            Sequence seq = DOTween.Sequence();
                            seq.Append(effect.rectTransform.DOScale(1.0f, 0.75f).SetEase(Ease.OutSine));
                            seq.Append(effect.rectTransform.DOScale(1.2f, 0.25f).SetEase(Ease.Linear));
                            seq.Insert(0.75f, effect.DOColor(new Color(1.0f, 1.0f, 1.0f, 0.0f), 0.25f));
                            seq.OnComplete(() =>
                            {
                                Destroy(effect.gameObject);
                            });
                            seq.Play();
                        }
                    }
                    break;
            }
        }
    }
}