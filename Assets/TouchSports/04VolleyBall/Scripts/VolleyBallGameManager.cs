using System;
using UnityEngine;

namespace ML.T_Sports.VolleyBall
{
    using Common;

    using System.Collections;

    /// <summary>
    /// 배구 게임 관리자
    /// </summary>
    public class VolleyBallGameManager : ContentsManagerBase
    {
        // public variables
        public VolleyBallShooter shooter;
        public AudioClip jumpSound, successSound, failSound;
        public AudioSource bgm;

        // game states
        public int numShootBalls = 0;
        public int score = 0;
        public int fail = 0;
        public float time = 0;
        public float totalTime = 45.0f;

        // singleton
        public static VolleyBallGameManager instance { get; private set; }

        // keepers
        private VolleyBallKeeper[] _keepers;

        // events
        public event Action onPlay;
        public event Action onShoot;
        public event Action onScore;
        public event Action onFail;
        public event Action onStop;
        public event Action<bool> onPause;
        public event Action onReset;

        

        #region Unity methods
        public override void Init()
        {
            base.Init();
            instance = this;

            InitProperty(ContentsPropertyType.Time, 30, 3600, 30);
            totalTime = GetPropertyValue(ContentsPropertyType.Time);

            _keepers = GetComponentsInChildren<VolleyBallKeeper>();
            bgm.volume = GetSharedPropertyValue(ContentsPropertyType.BGM);

            
        }

        public void Update()
        {
            if (IsPlaying)
            {
                time += Time.deltaTime;
                if (time >= totalTime)
                {
                    Stop();
                }
            }
        }

        public override void Cleanup()
        {
            instance = null;
        }

        public override void OnChangePropertyValue(ContentsPropertyType type, float prevValue, float newValue)
        {
            switch (type)
            {
                case ContentsPropertyType.Time:
                    totalTime = newValue;
                    time = 0;
                    if (onReset != null)
                        onReset();
                    break;
            }
        }

        public override void OnChangeSharedPropertyValue(ContentsPropertyType type, float prevValue, float newValue)
        {
            switch (type)
            {
                case ContentsPropertyType.BGM:
                    bgm.volume = newValue;
                    break;
            }
        }
        #endregion

        #region Game logic
        public override void Play()
        {
            if (!IsPlaying)
            {
                shooter.enabled = true;
                numShootBalls = 0;
                score = 0;
                fail = 0;
                time = 0;

                VolleyBall.Clear();

                base.Play();

                if (onPlay != null)
                    onPlay();
            }
        }

        public override void Pause()
        {
            base.Pause();

            if (onPause != null)
                onPause(IsPaused);
        }

        public override void Stop()
        {
            if (IsPlaying)
            {
                shooter.enabled = false;

                base.Stop();

                if (onStop != null)
                    onStop();
                
            }
        }

        public void OnShoot()
        {
            numShootBalls++;
            _Jump();
            if (onShoot != null)
                onShoot();
        }

        private void _Jump()
        {
            foreach (var k in _keepers)
                k.Invoke("Jump", UnityEngine.Random.Range(0.15f, 1.0f));
        }

        public void AddScore(int append)
        {
            score += append;
            if (append > 0)
            {
                if (onScore != null)
                    onScore();

                AudioSource.PlayClipAtPoint(successSound, Camera.main.transform.position, GetSharedPropertyValue(ContentsPropertyType.SFX));
            }
            else
            {
                fail += 1;
                if (onFail != null)
                    onFail();

                AudioSource.PlayClipAtPoint(failSound, Camera.main.transform.position, GetSharedPropertyValue(ContentsPropertyType.SFX));
            }
        }
        #endregion

        
    }
}