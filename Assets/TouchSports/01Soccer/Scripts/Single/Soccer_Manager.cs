using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace ML.T_Sports.Soccer
{
    using Common;
    //게임 페이즈 설정 가장 중요한것은 Ready 상태일때 공이 발사가 가능 하며, 그외에는 발사 불가능.
    public enum GameState
    {
        Shooting,
        Ready,
        End,
        End001,
        Loading
    }

    public class Soccer_Manager : ContentsManagerBase
    {
        public GameState gamestate;
        public GUI_Goal_Script[] Goal_Texture;
        public Animation Goal_Effect;
        public Soccer_Result result;
        public Soccer_Ball_Cleaner cleaner;
        
        public int Shooting_Counter;

        public int score;
        public int goal_Count;
        public int nogoal_Count;

        public Text Score;
        public Text Chance_Text;

        public GameObject LoadingObject;
        int Chance;
        public AudioSource bgm2;

        public bool compare;

        public static Soccer_Manager instance
        {
            get;
            private set;
        }
        //공통 인터페이스 부분
        #region CommonInterface

        public override void Init()
        {
            base.Init();
            instance = this;
            compare = false;
            gamestate = GameState.Loading;

            InitProperty(ContentsPropertyType.Chance, 1, 10, 10);
            InitProperty(ContentsPropertyType.GameMode, 0, 1, 1);
            SetPropertyValue(ContentsPropertyType.GameMode, 0);

            float sfx = GetSharedPropertyValueFloat(ContentsPropertyType.SFX);
            SoundManager.instance.SetEFMVolume(sfx);
            float bgm = GetSharedPropertyValueFloat(ContentsPropertyType.BGM);
            SoundManager.instance.SetBGMVolume(bgm);
            bgm2.volume = bgm;
        }

        public override void OnChangeSharedPropertyValue(ContentsPropertyType type, float prevValue, float newValue)
        {
            switch (type)
            {
                case ContentsPropertyType.SFX:
                    SoundManager.instance.SetEFMVolume(newValue);
                    break;
                case ContentsPropertyType.BGM:
                    bgm2.volume = newValue;
                    SoundManager.instance.SetBGMVolume(newValue);
                    break;
            }
        }
        //

        public override void Stop()
        {
            cleaner.Invoke("Allclear", 0.2f);

            if (IsPlaying)
            {
                base.Stop();
                gamestate = GameState.End;
                result.SetValues(goal_Count, nogoal_Count);

            }
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
                Debug.Log("Started");
                LoadingObject.SetActive(false);
                score = 0;
                goal_Count = 0;
                nogoal_Count = 0;
                Shooting_Counter = (int)GetPropertyValue(ContentsPropertyType.Chance);
                Chance_Text.text=Shooting_Counter.ToString();
                Chance = 0;
                //Invoke("turnready", 1.0f);
                if (gamestate == GameState.End001)
                {
                    result.Disapear();
                    for (int i = 0; i < Goal_Texture.Length; i++)
                    {
                        Goal_Texture[i].reflash();
                    }
                }
                gamestate = GameState.Ready;

            }
        }


        public override void OnChangePropertyValue(ContentsPropertyType type, float prevValue, float newValue)
        {
            switch (type)
            {
                case ContentsPropertyType.GameMode:
                    if (newValue == 1)
                        UnityEngine.SceneManagement.SceneManager.LoadScene("Touch_Soccer"); //이다인 씬 이름 수정
                    break;
            }
            Debug.Log(string.Format("Changed {0}_{1} value {2} -> {3}", ContentsName, type, prevValue, newValue));
        }
#endregion
        // Use this for initialization
        // 초기 버젼 리셋을 위한 부분 **현재 사용 안함.
        public void Restart()
        {
            cleaner.Invoke("Allclear", 1.0f);

            if (gamestate == GameState.Shooting)
            {
                gamestate = GameState.End;
                result.SetValues(goal_Count, nogoal_Count);
                Invoke("allscorereset", 6.0f);
                result.Invoke("CountStart", 1.0f);
                result.Invoke("ScoreCount", 1.5f);

            }
            else if (gamestate == GameState.End)
            {
                Invoke("allscorereset", 1.0f);

            }

        }
        // 모든 스코어를 영점으로 리셋, 
        public void allscorereset()
        {
            Chance = 0;
            score = 0;
            goal_Count = 0;
            nogoal_Count = 0;
        }
        // 득점시 불러오는 함수,Ball_Ctrl에서 호출
        public void AddPoint()
        {
            goal_Count++;
            Goal_Texture[Chance].Goal();
            score = score + 10;
            Goal_Effect.Play("Goal_Animation");
            if (Chance < (Shooting_Counter - 1))
            {
                Chance++;
            }
            else
            {
                Debug.Log("Content_Finished");
                gamestate = GameState.End;
                result.SetValues(goal_Count, nogoal_Count);
            }
            compare = true;
        }
        // 실점시 불러오는 함수,Ball_Ctrl에서 호출

        public void DePoint()
        {
            nogoal_Count++;
            Goal_Texture[Chance].NoGoal();
            Goal_Effect.Play("NoGoal_Animation");

            if (Chance < (Shooting_Counter - 1))
            {
                Chance++;
            }
            else
            {
                Debug.Log("Content_Finished");
                gamestate = GameState.End;
                result.SetValues(goal_Count, nogoal_Count);
            }
            compare = true;

        }
        // Update is called once per frame
        void Update()
        {
            Score.text = score.ToString();

            if (gamestate == GameState.End)
            {
                base.Stop();
                Restart();
                gamestate = GameState.End001;
            }
            Chance_Text.text = (Shooting_Counter - Chance).ToString();

        }
    }
}