using UnityEngine;
using UnityEngine.UI;

namespace ML.T_Sports.DodgeBall
{
    using DG.Tweening;
    using Common;
    using System;

    public class DodgeBallScoreUI : MonoBehaviour, IContentsManagerListener
    {
        public Text timeText, scoreText;
        public Image ready, finish;
        public Image plusOne;
        public Text numBallsText;
        public Image pause;

        public DodgeBallGameManager gameManager
        {
            get { return (DodgeBallGameManager)ContentsManagerBase.Current; }
        }

        public void Start()
        {
            ready.gameObject.SetActive(true);
            finish.gameObject.SetActive(false);
            plusOne.gameObject.SetActive(false);
            scoreText.text = "0";
            timeText.text = "00:00";
            numBallsText.text = "0";
            pause.gameObject.SetActive(false);

            if (gameManager != null)
            {
                gameManager.AddListener(this);
                gameManager.onScore += _OnScore;
                gameManager.onFail += _OnFail;
                gameManager.onReset += _OnReset;
                gameManager.onShoot += _OnShoot;
                gameManager.onPause += _OnPause;
            }
        }

        public void OnDestroy()
        {
            if (gameManager != null)
            {
                gameManager.RemoveListener(this);
                gameManager.onScore -= _OnScore;
                gameManager.onFail -= _OnFail;
                gameManager.onReset -= _OnReset;
                gameManager.onShoot -= _OnShoot;
                gameManager.onPause -= _OnPause;
            }
        }

        private void _OnScore()
        {
            scoreText.text = string.Format("{0:0}", gameManager.score);

            // 점수 상승 효과
            DOTween.Complete(scoreText, false);
            Sequence seq = DOTween.Sequence();
            seq.Append(scoreText.transform.DOScale(1.2f, 0.166f));
            seq.Append(scoreText.transform.DOScale(1.0f, 0.334f));
            seq.SetTarget(scoreText);
            seq.Play();

            // +1 위치 구하기
            RectTransform parent = plusOne.rectTransform.parent as RectTransform;
            Image newPlusOne = Instantiate(plusOne, parent);
            Vector3 viewport = Camera.main.WorldToViewportPoint(gameManager.lastHitPoint);
            viewport = (viewport - new Vector3(0.5f, 0.5f)) * 2.0f;
            newPlusOne.gameObject.SetActive(true);
            newPlusOne.transform.localScale = Vector3.one * 0.01f;
            newPlusOne.transform.localPosition = Vector3.Scale(viewport, new Vector3(parent.sizeDelta.x / 2, parent.sizeDelta.y / 2, 0));

            // +1 효과
            Sequence seq2 = DOTween.Sequence();
            seq2.Append(newPlusOne.transform.DOScale(plusOne.transform.localScale, 0.333f));
            seq2.Insert(0.0f, newPlusOne.transform.DOLocalMoveY(newPlusOne.transform.localPosition.y + parent.sizeDelta.y / 4, 1.25f).SetEase(Ease.InExpo));
            seq2.Insert(0.833f, newPlusOne.DOFade(0.0f, 0.166f));
            seq2.SetTarget(newPlusOne);
            seq2.OnComplete(() => Destroy(newPlusOne.gameObject));
            seq2.Play();
        }

        private void _OnFail()
        {
            // do nothing
        }

        private void _OnReset()
        {
            ready.gameObject.SetActive(true);
            finish.gameObject.SetActive(false);
            timeText.text = "00:00";
            DOTween.Kill(timeText);
            timeText.DOFade(1.0f, 0.1f);
        }

        private void _OnShoot()
        {
            int numBalls = ((DodgeBallGameManager)ContentsManagerBase.Current).numShootBalls;
            numBallsText.text = string.Format("{0:0}", numBalls);

            DOTween.Complete(numBallsText, false);
            Sequence seq = DOTween.Sequence();
            seq.Append(numBallsText.transform.DOScale(1.2f, 0.166f));
            seq.Append(numBallsText.transform.DOScale(1.0f, 0.334f));
            seq.SetTarget(numBallsText);
            seq.Play();
        }

        private void _OnPause(bool isPaused)
        {
            pause.rectTransform.SetAsLastSibling();
            pause.gameObject.SetActive(isPaused);
        }

        public void Update()
        {
            if (gameManager != null && gameManager.IsPlaying)
            {
                int min = Mathf.FloorToInt(gameManager.time / 60);
                int sec = Mathf.FloorToInt(gameManager.time % 60);
                timeText.text = string.Format("{0:#00}:{1:00}", min, sec);
            }
        }
        
        public void OnPlay()
        {
            ready.gameObject.SetActive(false);
            finish.gameObject.SetActive(false);
            scoreText.text = "0";
            timeText.text = "00:00";
            numBallsText.text = "0";
            DOTween.Kill(timeText);
            timeText.DOFade(1.0f, 0.1f);
        }

        public void OnReady()
        {
            // do nothing
        }

        public void OnStop()
        {
            DOTween.Kill(finish);
            finish.rectTransform.SetAsLastSibling();
            finish.gameObject.SetActive(true);
            finish.transform.localScale = Vector3.one * 0.001f;
            finish.transform.DOScale(1.0f, 0.75f).SetDelay(0.25f).SetTarget(finish);

            // 타이머 깜빡임
            var seq = DOTween.Sequence();
            seq.AppendInterval(0.25f);
            seq.Append(timeText.DOFade(0.666f, 0.25f));
            seq.AppendInterval(0.25f);
            seq.Append(timeText.DOFade(1.0f, 0.25f));
            seq.SetLoops(-1);
            seq.SetTarget(timeText);
            seq.Play();
        }
    }
}