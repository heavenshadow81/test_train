using UnityEngine;
using ML.T_Sports.Common;
namespace ML.T_Sports.BasketBall
{
    public enum BasketBallState
    {
        Ready,
        Play,
        Pause,
        Finish
    }
    public class BasketBallSingleManager : ContentsManagerBase
    {

        public BasketBallState state;
        public static BasketBallSingleManager instance;
        public GameObject Player_A;
        public Camera Camera_A, UI_A;

        public BasketBallTouchPoint touchs;
        public BasketBallSingleScoreManager scores;
        public Animation Finish;
        public RectTransform FinishRect;
        public GameObject Ready;
        public int PlayUser;

        

        public override void Init()
        {
            if(instance == null)
            {
                instance = this;
            }
            InitProperty(ContentsPropertyType.Time, 30, 3600, 30);
            InitProperty(ContentsPropertyType.GameMode, 0, 1, 0);
            SetPropertyValue(ContentsPropertyType.GameMode, 0);

            float sfx = GetSharedPropertyValueFloat(ContentsPropertyType.SFX);
            Common.SoundManager.instance.SetEFMVolume(sfx);
            float bgm = GetSharedPropertyValueFloat(ContentsPropertyType.BGM);
            Common.SoundManager.instance.SetBGMVolume(bgm);

            PlayUser = 1;
            state = BasketBallState.Ready;
            instance = this;

            scores.Timer = GetPropertyValueInt(ContentsPropertyType.Time);
            scores.TimeTextSet();

            
        }
        public void CheckBalls()
        {            
            if (scores.Timer <=0)
            {
                //결과창 도출
                Invoke("SetFinish", 1);
            }
        }
        public void SetFinish()
        {
            Finish.Play("Finish");
            state = BasketBallState.Finish;
            if (IsPlaying)
                IsPlaying = false;
            
        }
        public override void OnChangeSharedPropertyValue(ContentsPropertyType type, float prevValue, float newValue)
        {
            switch (type)
            {
                case ContentsPropertyType.SFX:
                    Common.SoundManager.instance.SetEFMVolume(newValue);
                    break;
                case ContentsPropertyType.BGM:
                    Common.SoundManager.instance.SetBGMVolume(newValue);
                    break;
            }
        }
        public override void OnChangePropertyValue(ContentsPropertyType type, float prevValue, float newValue)
        {
            switch (type)
            {                
                case ContentsPropertyType.GameMode:
                    if (newValue == 1)
                        UnityEngine.SceneManagement.SceneManager.LoadScene("Touch_Basketball");
                    break;
                case ContentsPropertyType.Time:
                    scores.Timer = newValue;
                    scores.TimeTextSet();
                    break;
            }
            //Debug.Log(string.Format("Changed {0}_{1} value {2} -> {3}", ContentsName, type, prevValue, newValue));
        }
        public GameObject PauseCanvas;
        public override void Pause()
        {
            base.Pause();
            PauseCanvas.SetActive(IsPaused);
        }
        public override void Play()
        {
            if (!IsPlaying)
            {
                base.Play();
                SetBasketBallPlay(GetPropertyValue(ContentsPropertyType.Time));
                state = BasketBallState.Play;
            }
        }
        public override void Stop()
        {
            base.Stop();
            SetFinish();
            
        }
        public void SetBasketBallPlay(float time)
        {
            if (scores.Score > 0)
                ResetGame();
            FinishRect.anchoredPosition = new Vector2(0, -2000);
            scores.Timer = time;
            Ready.SetActive(false);
            state = BasketBallState.Play;
        }
        
        public void ResetGame()
        {
            FinishRect.anchoredPosition = new Vector2(0, -2000);
            scores.ResetScore();
            touchs.ResetBallCount();
            state = BasketBallState.Ready;
        }

        
    }
}

