using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.MLBKids
{
    public class Timer : MonoBehaviour
    {
        #region Public variables
        public float time;
        public bool timerEnabled = false;

        #region Constants
        public float hitGameTime = 50.0f;
        public float pitchGameTime = 50.0f;
        public float countdownTime = 10.0f;
        #endregion
        #endregion

        #region Private variables
        private float _totalTime;
        private AudioSource _timerSound;
        #endregion

        #region Events
        // delegate
        public delegate void OnTimer(float time, float totalTime);
        public delegate void OnTimerCountdown(float time, float totalTime);
        public delegate void OnTimerEnd();
        public delegate void OnTimerAppend(float time, float append);

        // event
        public static event OnTimer onTimer;
        public static event OnTimerCountdown onTimerCountdown;
        public static event OnTimerEnd onTimerEnd;
        public static event OnTimerAppend onTimerAppend;
        #endregion

        public void Set(float newTotalTime)
        {
            time = 0;
            _totalTime = newTotalTime;
            if (onTimer != null)
                onTimer(time, _totalTime);
        }

        public void Play()
        {
            timerEnabled = true;
        }

        public void Stop()
        {
            timerEnabled = false;
            SoundManager.StopSFXObject(_timerSound);
            _timerSound = null;
        }

        public void AppendTime(float given)
        {
            if (timerEnabled)
            {
                time = time + given;
                if (time > _totalTime)
                    time = _totalTime;
                if (onTimerAppend != null)
                    onTimerAppend(time, given);
            }
        }

        public void Update()
        {
            if (timerEnabled)
            {
                time += Time.deltaTime;
                if (time >= _totalTime)
                    time = _totalTime;
                if (_timerSound == null && time + countdownTime >= _totalTime)
                {
                    _timerSound = SoundManager.PlaySFX("snd_stopwatch_start_run_stop");
                    if (onTimerCountdown != null)
                        onTimerCountdown(time, _totalTime);
                }
                if(time >= _totalTime)
                    time = _totalTime;
                if (onTimer != null)
                    onTimer(time, _totalTime);
                if (time >= _totalTime)
                {
                    Stop();
                    if (onTimerEnd != null)
                        onTimerEnd();
                }
            }
        }
    }
}