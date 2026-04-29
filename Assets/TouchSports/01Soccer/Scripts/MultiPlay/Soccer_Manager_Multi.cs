using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ML.T_Sports.Common;

namespace ML.T_Sports.Soccer
{
    using Common;
    //게임 페이즈 정의는 싱글 플레이 정의 부분과 공유
    //팀 차례의 정의
    public enum TeamTurn
    {
        team01,
        team02
    }
    

    public class Soccer_Manager_Multi : ContentsManagerBase
    {
        public GameState gamestate;

        public Animation Goal_Effect;
        public Animation TeamShift_Ani;
        public Soccer_Ball_Cleaner_Multi cleaner;
        
        public int score_tm01;
        public int score_tm02;

        public Text Score_tm01;
        public Text Score_tm02;

        public TeamTurn team;

        public int time;
        public Text Timer;

        public Animation result_Ani;
        public GameObject LoadingObject;

        public int teamwon;

        public AudioSource bgm2;
        public bool compare;

        public static Soccer_Manager_Multi instance
        {
            get;
            private set;
        }
        //<Interface>Soccer
        //공통 인터페이스 부분
        #region CommonInterface
        public override void Init()
        {
            base.Init();
            compare = false;

            instance = this;
            gamestate = GameState.Loading;

            InitProperty(ContentsPropertyType.Time, 30, 3600, 30);
            InitProperty(ContentsPropertyType.GameMode, 0, 1, 1);
            SetPropertyValue(ContentsPropertyType.GameMode, 1);

            StartCoroutine(InitSound());            
        }
        IEnumerator InitSound()
        {
            yield return new WaitForSeconds(1);
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
        public override void Stop()
        {
            cleaner.Allclear();
            if (IsPlaying)
            {
                base.Stop();
                gamestate = GameState.End;
                StopAllCoroutines();

            }
        }
        public override void Play()
        {
            if (!IsPlaying)
            {
                base.Play();
                Debug.Log("Started");
                LoadingObject.SetActive(false);

                score_tm01 = 0;
                score_tm02 = 0;
                team = TeamTurn.team01;
                time = (int)GetPropertyValue(ContentsPropertyType.Time);

                if (gamestate == GameState.End001)
                {
                    result_Dis();
                }

                gamestate = GameState.Ready;

                TimerStart();
            }
        }
        public GameObject PauseCanvas;
        public override void Pause()
        {
            base.Pause();
            PauseCanvas.SetActive(IsPaused);
        }
        public override void OnChangePropertyValue(ContentsPropertyType type, float prevValue, float newValue)
        {
            switch (type)
            {
                case ContentsPropertyType.GameMode:
                    if (newValue == 0)
                        UnityEngine.SceneManagement.SceneManager.LoadScene("01SoccerSingleMod");
                    break;
            }
            Debug.Log(string.Format("Changed {0}_{1} value {2} -> {3}", ContentsName, type, prevValue, newValue));
        }
#endregion

        // Use this for initialization
        void Start()
        {
            gamestate = GameState.Loading;
        }
        public void StartButtonPushed()
        {

            LoadingObject.SetActive(false);
            if (gamestate == GameState.Loading)
            {
                Debug.Log("what");
                allscorereset();

            }
        }
        //타이머 기능 스크립트
        #region Timer
        public void TimerStart()
        {
            StopCoroutine("Time");
            Debug.Log("tictok_First");

            StartCoroutine("Time");
        }

        IEnumerator Time()
        {
            while (time>0)
            {
                //Debug.Log("tictok");
                time--;
                if(time <= 0)
                {
                    time = 0;
                    Stop();
                }
                yield return new WaitForSeconds(1.0f);
            }
        }
#endregion
        public void Restart()
        {
            cleaner.Invoke("Allclear", 0.5f);

            if (gamestate == GameState.Shooting)
            {
                gamestate = GameState.End;
                Invoke("allscorereset", 0.5f);
                Invoke("TimerStart", 3.0f);
            }
            else if (gamestate == GameState.End)
            {
                Invoke("allscorereset", 0.5f);
                Invoke("TimerStart", 3.0f);
            }


        }

        public void allscorereset()
        {
            score_tm01 = 0;
            score_tm02 = 0;
            gamestate = GameState.Ready;

        }
        //득점 스크립트, 득점후 바로 팀 쉬프트 기능이 돌도록 설계
        public void AddPoint()
        {
            if (team == TeamTurn.team01)
            {
                team = TeamTurn.team02;

                score_tm01 = score_tm01 + 1;
            }
            else if (team == TeamTurn.team02)
            {
                team = TeamTurn.team01;

                score_tm02 = score_tm02 + 1;
            }
           Invoke("TeamShift",1.0f);
            Goal_Effect.Play("Goal_Animation");
            compare = true;

        }
        //실점 스크립트, 실점후 바로 팀 쉬프트 기능이 돌도록 설계

        public void DePoint()
        {
            Goal_Effect.Play("NoGoal_Animation");
            if (team == TeamTurn.team01)
            {
                team = TeamTurn.team02;
            }
            else if (team == TeamTurn.team02)
            {
                team = TeamTurn.team01;
            }
            Invoke("TeamShift", 1.0f);
            compare = true;

        }

        // Update is called once per frame
        void Update()
        {

            Score_tm01.text = score_tm01.ToString();
            Score_tm02.text = score_tm02.ToString();
            // 시간을 초 분으로 나누어 표기
            if ((time / 60) < 10)
            {
                if ((time % 60) < 10)
                {
                    Timer.text = "0" + (time / 60).ToString() + " : 0" + (time % 60).ToString();
                }
                else
                {
                    Timer.text = "0" + (time / 60).ToString() + " : " + (time % 60).ToString();
                }
            }
            else
            {
                if ((time % 60) < 10)
                {
                    Timer.text =  (time / 60).ToString() + " : 0" + (time % 60).ToString();
                }
                else
                {
                    Timer.text = (time / 60).ToString() + " : " + (time % 60).ToString();
                }
            }

            if (Input.GetKey(KeyCode.A))
            {
                time = 1;
            }

            if(gamestate == GameState.End)
            {
                Invoke("CompareScore", 3.0f);
                gamestate = GameState.End001;
                cleaner.Allclear();

            }
        }
        //팀 점수를 비교하여 더 높은 팀에 승리 판정
        public void CompareScore()
        {
            if (score_tm01 > score_tm02)
            {
                result_Ani.Play("Team_Blue_Win");
                teamwon = 1;
            }
            else if(score_tm01 < score_tm02)
            {
                teamwon = 2;
                result_Ani.Play("Team_Red_Win");
            }
            else
            {
                result_Ani.Play("Team_Draw");
                teamwon = 0;
            }
        }
        //승리 판정후, 승리한 팀을 표기
        public void result_Dis()
        {
            if (result_Ani.IsPlaying("Team_Blue_Win")||teamwon ==1)
            {
                result_Ani.Play("Team_BlueDisa");
            }
            else if(result_Ani.IsPlaying("Team_Red_Win")||teamwon == 2)
            {
                result_Ani.Play("Team_RedDisa");
            }
            else if (result_Ani.IsPlaying("Team_Draw") || teamwon == 0)
            {
                result_Ani.Play("Team_DrawDisa");
            }

        }

        //차례가 끝난후 팀을 바꿔주는 스크립트.
        public void TeamShift()
        {

            if (team == TeamTurn.team01)
            {
                TeamShift_Ani.Play("Blue_Team_Ready");
            }
            else if(team ==TeamTurn.team02)
            {
                TeamShift_Ani.Play("Red_Team_Ready");
            }
            
        }
    }
}