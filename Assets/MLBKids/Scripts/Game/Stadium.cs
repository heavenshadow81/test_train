using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;
using DG.Tweening;

namespace ML.MLBKids
{
    public class Stadium : MonoBehaviour
    {
        public enum GameMode
        {
            None = -1, Hit, Pitch
        }

        #region Public variables
        public GameMode gameMode;
        public Timer timer;
        public BallPitcher pitcher;
        public BallPlayerPitcher playerPitcher;
        public InstantBat batPrefab;
        public GameObject baseballCatcher;
        public Camera[] cams;
        #endregion

        #region Properties
        public static Stadium instance
        {
            get; private set;
        }

        public bool isPlaying { get; private set; }
        public bool paused { get; private set; }
        #endregion

        #region Private variables
        private PageManager _pageManager = null;
        private Coroutine _gameLoop = null;
        private Vector3 _rotCam0;
        #endregion

        #region Unity methods
        public void Awake()
        {
            pitcher.gameObject.SetActive(false);
            playerPitcher.gameObject.SetActive(false);
            if (baseballCatcher != null)
                baseballCatcher.SetActive(false);
            _rotCam0 = cams[0].transform.localEulerAngles;
        }

        public void Start()
        {
            instance = this;
            Timer.onTimerEnd += _OnTimerEnd;
            _pageManager = PageManager.instance;
            for (int i = 0; i < cams.Length; i++)
            {
                cams[i].gameObject.SetActive(i == 0);
            }

            if (SceneManager.sceneCount == 1)
            {
                //KinectHelper.instance.Init();
                //KinectHelper.instance.enablesBodyTracking = true;
                //Init(gameMode);
                //Play();
                //_pageManager.ShowPage<MenuPage>();
            }
        }

        public void OnDestroy()
        {
            Timer.onTimerEnd -= _OnTimerEnd;
            instance = null;
        }

        public void Init(GameMode newMode)
        {
            gameMode = newMode;
            SoundManager.PlayConnection("Crowd_Normal");
            ScoreManager.Clear();
            ScoreManager.gameMode = gameMode;

            foreach (var cam in cams)
            {
                cam.gameObject.SetActive(cam == cams[(int)gameMode]);
            }

            if (baseballCatcher != null)
                baseballCatcher.SetActive(gameMode == GameMode.Pitch);
        }

        public void Tutorial()
        {
            if (isPlaying)
            {
                Debug.LogWarning("Game is already playing!");
                return;
            }

            TutorialPage tutorialPage = _pageManager.ShowPage<TutorialPage>();

            KinectCondition condition = KinectCondition.None;
            if (gameMode == GameMode.Hit) condition = KinectCondition.ComeUpClose;
            else if (gameMode == GameMode.Pitch) condition = KinectCondition.StandBack;

            tutorialPage.Set(gameMode, condition, true, (flag) =>
            {
                if (flag)
                    Play();
                else
                {
                    Cleanup();
                    _pageManager.GoFromMenuToAds();
                }
            });
        }

        public void Play()
        {
            if (paused)
            {
                paused = false;
                timer.Play();
            }
            else
            {
                _gameLoop = StartCoroutine(_Play());
                HandCursor.showsCursor = false;
            }
        }

        private IEnumerator _Play()
        {
            isPlaying = true;

            SoundManager.PlaySFX("WHICK");
            timer.Stop();

            yield return new WaitForSeconds(1.0f);

            switch (gameMode)
            {
                case GameMode.Hit:
                    timer.Set(timer.hitGameTime);
                    _pageManager.ShowPage<HitGameScorePage>();
                    pitcher.gameObject.SetActive(true);
                    break;
                case GameMode.Pitch:
                    timer.Set(timer.pitchGameTime);
                    _pageManager.ShowPage<PitchGameScorePage>();
                    playerPitcher.gameObject.SetActive(true);
                    break;
            }
            timer.Play();

            _gameLoop = null;
        }

        public void Pause()
        {
            if(isPlaying && !paused)
            {
                paused = true;
                timer.Stop();
            }
        }

        public void Stop()
        {
            isPlaying = false;
            timer.Stop();
            if (_gameLoop != null)
            {
                StopCoroutine(_gameLoop);
                _gameLoop = null;
            }
            switch (gameMode)
            {
                case GameMode.Hit:
                    pitcher.gameObject.SetActive(false);
                    break;
                case GameMode.Pitch:
                    playerPitcher.gameObject.SetActive(false);
                    break;
            }
            HandCursor.showsCursor = true;
        }

        private void _OnTimerEnd()
        {
            _gameLoop = StartCoroutine(_OnTimerEnd_Coroutine());
        }

        public void SetBlur(bool enabled, float animTime = 3.0f)
        {
            Camera cam = cams[(int)gameMode];
            BlurOptimized blur = cam.GetComponent<BlurOptimized>();
            if (blur != null)
            {
                if (!blur.enabled && enabled)
                    blur.enabled = true;
                if (enabled)
                    blur.blurSize = 0;

                DOTween.Kill(cam);
                if (animTime <= 0.0f)
                {
                    blur.blurSize = 3.0f;
                    blur.enabled = enabled;
                }
                else
                {
                    DOTween.To(() => blur.blurSize, val => blur.blurSize = val, enabled ? 3.0f : 0.0f, animTime).SetTarget(cam).OnComplete(() =>
                    {
                        blur.enabled = enabled;
                    });
                }
            }
        }
        
        public void Test(Camera cam, Vector3 dir)
        {
            if (DOTween.IsTweening(cam))
                DOTween.Kill(cam);

            Sequence seq = DOTween.Sequence();
            seq.SetTarget(cam);
            seq.Append(cam.transform.DOLocalRotate(_rotCam0 + new Vector3(0.0f, dir.x * 10.0f), 2.0f));
            seq.Append(cam.transform.DOLocalRotate(_rotCam0, 2.0f));
            seq.Play();
        }

        private IEnumerator _OnTimerEnd_Coroutine()
        {
            SoundManager.PlaySFX("WHICK");
            Stop();

            _pageManager.HidePage<TutorialPage>();
            SetBlur(true);

            yield return new WaitForSeconds(1.0f);

            _pageManager.HidePage<HitGameScorePage>();
            _pageManager.HidePage<PitchGameScorePage>();
            ResultPage resultPage = _pageManager.ShowPage<ResultPage>();
        }
        
        public void Cleanup()
        {
            if (isPlaying)
            {
                paused = false;
                Stop();
            }

            SetBlur(false, 0.0f);

            for (int i = 0; i < cams.Length; i++)
            {
                DOTween.Kill(cams[i]);
                cams[i].gameObject.SetActive(i == 0);
                if (i == 0)
                    cams[i].transform.localEulerAngles = _rotCam0;
            }

            if (baseballCatcher != null)
                baseballCatcher.SetActive(false);

            _pageManager.HidePage<ResultPage>();
            _pageManager.HidePage<HitGameScorePage>();
            _pageManager.HidePage<PitchGameScorePage>();
            _pageManager.HidePage<TutorialPage>();
            SoundManager.StopMusicImmediately();
            SoundManager.StopSFX();
            Ball.Clear();
            PlayerBall.Clear();
        }
        #endregion
    }
}