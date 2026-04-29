using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace ML.T_Sports.VolleyBall
{
    /// <summary>
    /// 배구 점수, 시간 표시하는 UI
    /// </summary>
    public class VolleyBallScoreUI : MonoBehaviour
    {
        public Image fade;
        public Text timeText;
        public Text scoreText;
        public Text numBallsText;
        public Image ball;
        public Image success, fail;
        public Image ready, finish;
        public Image pause;

        public void Start()
        {
            fade.gameObject.SetActive(true);
            fade.color = Color.black;
            fade.DOFade(0.0f, 1.5f).SetDelay(0.5f);
            success.gameObject.SetActive(false);
            fail.gameObject.SetActive(false);
            ready.gameObject.SetActive(true);
            finish.gameObject.SetActive(false);
            pause.gameObject.SetActive(false);

            if (VolleyBallGameManager.instance != null)
            {
                VolleyBallGameManager.instance.onScore += _OnScore;
                VolleyBallGameManager.instance.onFail += _OnFail;
                VolleyBallGameManager.instance.onPlay += _OnPlay;
                VolleyBallGameManager.instance.onStop += _OnStop;
                VolleyBallGameManager.instance.onPause += _OnPause;
                VolleyBallGameManager.instance.onShoot += _OnShoot;
                VolleyBallGameManager.instance.onReset += _OnReset;
                scoreText.text = "0";
                numBallsText.text = "0";
            }
        }

        public void Update()
        {
            if (VolleyBallGameManager.instance != null)
            {
                float time = Mathf.Max(0, VolleyBallGameManager.instance.totalTime - VolleyBallGameManager.instance.time);
                int min = Mathf.FloorToInt(time / 60);
                int sec = Mathf.FloorToInt(time % 60);
                timeText.text = string.Format("{0:#00}:{1:00}", min, sec);
            }
        }

        public void OnDestroy()
        {
            if (VolleyBallGameManager.instance != null)
            {
                VolleyBallGameManager.instance.onScore -= _OnScore;
                VolleyBallGameManager.instance.onFail -= _OnFail;
                VolleyBallGameManager.instance.onPlay -= _OnPlay;
                VolleyBallGameManager.instance.onStop -= _OnStop;
                VolleyBallGameManager.instance.onPause -= _OnPause;
                VolleyBallGameManager.instance.onShoot -= _OnShoot;
                VolleyBallGameManager.instance.onReset -= _OnReset;
            }
        }

        private void _OnScore()
        {
            scoreText.text = string.Format("{0:0}", VolleyBallGameManager.instance.score);

            DOTween.Complete(scoreText, false);
            Sequence seq = DOTween.Sequence();
            seq.Append(scoreText.transform.DOScale(1.2f, 0.166f));
            seq.Append(scoreText.transform.DOScale(1.0f, 0.334f));
            seq.SetTarget(scoreText);
            seq.Play();

            _ShowEffect(success, new Vector3(-300.0f, -200.0f));
        }

        private void _OnFail()
        {
            _ShowEffect(fail, new Vector3(-300.0f, 200.0f));
        }

        private void _OnPlay()
        {
            ready.gameObject.SetActive(false);
            finish.gameObject.SetActive(false);
            scoreText.text = "0";
            numBallsText.text = "0";
        }

        private void _OnStop()
        {
            DOTween.Kill(finish);
            finish.rectTransform.SetAsLastSibling();
            finish.gameObject.SetActive(true);
            finish.transform.localScale = Vector3.one * 0.001f;
            finish.transform.DOScale(1.0f, 0.75f).SetDelay(0.25f).SetTarget(finish);
        }

        private void _OnPause(bool isPaused)
        {
            pause.rectTransform.SetAsLastSibling();
            pause.gameObject.SetActive(isPaused);
        }

        private void _OnReset()
        {
            ready.gameObject.SetActive(true);
            finish.gameObject.SetActive(false);
        }

        private void _OnShoot()
        {
            int numBalls = VolleyBallGameManager.instance.numShootBalls;
            numBallsText.text = string.Format("{0:0}", numBalls);

            DOTween.Complete(numBallsText, false);
            Sequence seq = DOTween.Sequence();
            seq.Append(numBallsText.transform.DOScale(1.2f, 0.166f));
            seq.Append(numBallsText.transform.DOScale(1.0f, 0.334f));
            seq.SetTarget(numBallsText);
            seq.Play();
        }

        private void _ShowEffect(Image prefab, Vector3 move)
        {
            Image effectImage = Instantiate(prefab, prefab.transform.parent);
            effectImage.gameObject.SetActive(true);
            effectImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

            Vector3 originalPos = effectImage.transform.localPosition;
            effectImage.transform.localPosition = originalPos - move;

            Sequence seq = DOTween.Sequence();
            seq.Append(effectImage.DOFade(1.0f, 0.166f));
            seq.Insert(0.0f, effectImage.transform.DOLocalMove(originalPos - move * 0.2f, 0.166f).SetEase(Ease.InQuad));
            seq.Insert(0.166f, effectImage.transform.DOLocalMove(originalPos + move * 0.2f, 0.834f).SetEase(Ease.Linear));
            seq.Insert(1.0f, effectImage.transform.DOLocalMove(originalPos + move, 0.166f).SetEase(Ease.OutQuad));
            seq.Insert(1.0f, effectImage.DOFade(0.0f, 0.166f));
            seq.Play();
            Destroy(effectImage.gameObject, 2.0f);
        }
    }
}