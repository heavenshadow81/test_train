using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ML.MLBKids
{
    public class PitchGameScorePage : Page
    {
        public RectTransform top, mid, bottom;
        public Image timerSlider;
        public Image clock, clock2;
        public Image strike, ball;
        public Text messageText;

        public string pitchMsg = "사용자 위치에서 공을 던져주세요.";
        public string strikeNiceMsg = "훌륭해요!";
        public string strikeMsg = "잘했어요!";
        public string ballMsg = "다시 한번 도전!";
        
        private List<Tween> _countdownTweens = new List<Tween>();

        private ulong _trackingId;
        private float _trackingWaitTime = 0;

        public override void Show()
        {
            if (!isShowing)
            {
                mid.gameObject.SetActive(false);
                strike.gameObject.SetActive(false);
                ball.gameObject.SetActive(false);

                Vector3 pos = Vector3.zero;

                pos = top.anchoredPosition;
                pos.y = 400.0f;
                top.anchoredPosition = pos;
                pos = bottom.anchoredPosition;
                pos.y = -400.0f;
                bottom.anchoredPosition = pos;

                top.DOAnchorPosY(0, 0.25f);
                bottom.DOAnchorPosY(0, 0.25f);

                clock.rectTransform.rotation = Quaternion.identity;
                clock2.color = Color.white;

                messageText.text = "";

                ScoreManager.onAddScore += _OnAddScore;

                Timer.onTimer += _OnTimer;
                Timer.onTimerCountdown += _OnTimerCountdown;
                Timer.onTimerEnd += _OnTimerEnd;

                Stadium.instance.playerPitcher.onBallPitch += _OnBallPtich;
                Stadium.instance.playerPitcher.onBallSpawn += _OnBallSpawn;

                //if (KinectHelper.instance.isOpen)
                //    KinectHelper.instance.onBodyTracked += _OnBodyTracked;
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
                
                ScoreManager.onAddScore -= _OnAddScore;

                Timer.onTimer -= _OnTimer;
                Timer.onTimerCountdown -= _OnTimerCountdown;
                Timer.onTimerEnd -= _OnTimerEnd;

               // KinectHelper.instance.onBodyTracked -= _OnBodyTracked;

                Stadium.instance.playerPitcher.onBallPitch -= _OnBallPtich;
                Stadium.instance.playerPitcher.onBallSpawn -= _OnBallSpawn;
            }
            base.Hide();
        }

        public void Update()
        {
            //if (KinectHelper.instance.isOpen)
            //{
            //    if (Stadium.instance.isPlaying && !Stadium.instance.paused)
            //    {
            //        if (_trackingId == 0)
            //        {
            //            _trackingWaitTime += Time.deltaTime;
            //            if (_trackingWaitTime >= 2.0f)
            //            {
            //                Stadium.instance.Pause();
            //                TutorialPage tutorialPage = PageManager.instance.ShowPage<TutorialPage>();
            //                tutorialPage.Set(Stadium.instance.gameMode, KinectCondition.StandBack, false, (flag) =>
            //                {
            //                    if (flag)
            //                    {
            //                        _trackingId = KinectHelper.instance.trackingId;
            //                        _trackingWaitTime = 0;
            //                        PageManager.instance.ShowPage<PitchGameScorePage>();
            //                        Stadium.instance.Play();
            //                    }
            //                    else
            //                    {
            //                        Stadium.instance.Cleanup();
            //                        PageManager.instance.GoFromMenuToAds();
            //                    }
            //                });
            //                Hide();
            //            }
            //        }
            //    }
            //}
        }

        private void _OnAddScore(int score, int add, ScoreManager.ScoreType scoreType)
        {
            switch (scoreType)
            {
                case ScoreManager.ScoreType.Strike:
                case ScoreManager.ScoreType.Nice:
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
                    break;
                case ScoreManager.ScoreType.Ball:
                    {
                        var effect = Instantiate<Image>(ball, strike.transform.parent, true);
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
                    break;
            }

            if (Stadium.instance.playerPitcher.table.activeSelf)
            {
                if (scoreType == ScoreManager.ScoreType.Strike)
                    messageText.text = strikeNiceMsg;
                else if (scoreType == ScoreManager.ScoreType.Nice)
                    messageText.text = strikeMsg;
                else if (scoreType == ScoreManager.ScoreType.Ball)
                    messageText.text = ballMsg;
            }
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
                messageText.text = "";
            }
        }

        private void _OnBodyTracked(ulong trackingId)
        {
            _trackingId = trackingId;
            if (_trackingId != 0)
                _trackingWaitTime = 0;
        }

        private void _OnBallPtich()
        {
            messageText.text = "";
        }

        private void _OnBallSpawn()
        {
            messageText.text = pitchMsg;
        }
    }
}