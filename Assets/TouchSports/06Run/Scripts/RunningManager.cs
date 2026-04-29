using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ML.T_Sports.Common;

namespace ML.T_Sports.Running
{
    public enum RunningState
    {
        Ready,
        Play,
        Pause,
        Finish
    }
    public class RunningManager : ContentsManagerBase
    {
        public static RunningManager instance;
        public RunningState state;

        /// <summary>
        /// 각 플레이어의 ScoreManager
        /// </summary>
        public RunningScoreManager[] Scores;

        /// <summary>
        /// 화면을 분할 하게 될 플레이어 수
        /// </summary>
        public int PlayerCount;

        public GameObject[] TouchsButton;

        public RunningTouchPoint touch;

        /// <summary>
        /// 화면분할을 위한 플레이어 오브젝트와, 카메라
        /// </summary>
        public GameObject Player_A;
        public Camera Camera_A;
        public Camera UICamera_A;

        public GameObject Player_B;
        public Camera Camera_B;
        public Camera UICamera_B;

        public GameObject Player_C;
        public Camera Camera_C;
        public Camera UICamera_C;

        public GameObject Player_D;
        public Camera Camera_D;
        public Camera UICamera_D;

        public Common.SoundManager sound;
        /// <summary>
        /// 터치 반복 횟수
        /// </summary>
        public int Chance;

        
        public override void Init()
        {
            InitProperty(ContentsPropertyType.Chance, 3, 5, 3);
            InitProperty(ContentsPropertyType.Player, 1, 4, 4);
            InitProperty(ContentsPropertyType.GameMode, 0, 1, 0);            

            instance = this;
            Chance = 3;

            SetPlayerCamera(GetPropertyValueInt(ContentsPropertyType.Player));
            touch.Player = GetPropertyValueInt(ContentsPropertyType.Player);
            SetChance(GetPropertyValueInt(ContentsPropertyType.Chance));

            float sfx = GetSharedPropertyValueFloat(ContentsPropertyType.SFX);
            sound.SetEFMVolume(sfx);
            float bgm = GetSharedPropertyValueFloat(ContentsPropertyType.BGM);
            sound.SetBGMVolume(bgm);
            state = RunningState.Ready;

        }

        public GameObject PauseCanvas;
        public override void Pause()
        {
            base.Pause();
            PauseCanvas.SetActive(IsPaused);
        }
        /// <summary>
        /// 플레이 버튼 누르면 호출 됨
        /// 세팅된 플레이어 숫자만큼 Scores에게 게임 플레이를 명령
        /// </summary>
        public override void Play()
        {
            if (!IsPlaying)
            {
                base.Play();
                SetRunningPlay();
                state = RunningState.Play;
            }
        }
        public override void Stop()
        {
            if (IsPlaying)
            {
                IsPlaying = false;
                base.Stop();
                state = RunningState.Finish;
                for (int i = 0; i < Scores.Length; i++)                    
                {
                    Scores[i].SetStopGame();
                }
            }
        }
        public void SetRunningPlay()
        {
            touch.Player = GetPropertyValueInt(ContentsPropertyType.Player);
            for (int i = 0; i < PlayerCount; i++)
                Scores[i].SetRunningPlay(Chance);
            for (int i = 0; i < TouchsButton.Length; i++)
                TouchsButton[i].SetActive(true);
        }

        /// <summary>
        /// 컨트롤러로부터 변경된 터치 반복 횟수
        /// 이 값은 각 플레이어 ScoreManager에게 즉시 전달함.
        /// </summary>
        public override void OnChangePropertyValue(ContentsPropertyType type, float prevValue, float newValue)
        {
            if (state == RunningState.Finish || state == RunningState.Ready)
            {
                switch (type)
                {
                    case ContentsPropertyType.Chance:
                        SetChance((int)newValue);
                        break;
                    case ContentsPropertyType.Player:
                        touch.Player = (int)newValue;
                        SetPlayerCamera((int)newValue);
                        break;
                }
            }            
            //Debug.Log(string.Format("Changed {0}_{1} value {2} -> {3}", ContentsName, type, prevValue, newValue));
        }

        public override void OnChangeSharedPropertyValue(ContentsPropertyType type, float prevValue, float newValue)
        {
            switch (type)
            {
                case ContentsPropertyType.SFX:
                    sound.SetEFMVolume(newValue);
                    break;
                case ContentsPropertyType.BGM:
                    sound.SetBGMVolume(newValue);
                    break;
            }
        }
        public void SetChance(int ChanceValue)
        {
            Chance = ChanceValue;
            for (int i = 0; i < PlayerCount; i++)
            {
                Scores[i].SetScoreBox(Chance, true);
            }
        }

        /// <summary>
        /// 플레이어 수 변경에 따른 카메라 분할 세팅
        /// </summary>
        public void SetPlayerCamera(int players)
        {
            PlayerCount = players;
            switch (PlayerCount)
            {
                case 1:
                    Player_B.SetActive(false);
                    Player_C.SetActive(false);
                    Player_D.SetActive(false);
                    Camera_A.rect = new Rect(0, 0, 1, 1);
                    UICamera_A.rect = Camera_A.rect;
                    break;
                case 2:
                    Player_B.SetActive(true);
                    Player_C.SetActive(false);
                    Player_D.SetActive(false);
                    Camera_A.rect = new Rect(0, 0, 0.5f, 1);
                    Camera_B.rect = new Rect(0.5f, 0, 1, 1);
                    UICamera_A.rect = Camera_A.rect;
                    UICamera_B.rect = Camera_B.rect;
                    break;
                case 3:
                    Player_B.SetActive(true);
                    Player_C.SetActive(true);
                    Player_D.SetActive(false);
                    Camera_A.rect = new Rect(0, 0, 0.33f, 1);
                    Camera_B.rect = new Rect(0.33f, 0, 0.33f, 1);
                    Camera_C.rect = new Rect(0.66f, 0, 1, 1);
                    UICamera_A.rect = Camera_A.rect;
                    UICamera_B.rect = Camera_B.rect;
                    UICamera_C.rect = Camera_C.rect;
                    break;
                case 4:
                    Player_B.SetActive(true);
                    Player_C.SetActive(true);
                    Player_D.SetActive(true);
                    Camera_A.rect = new Rect(0, 0, 0.25f, 1);
                    Camera_B.rect = new Rect(0.25f, 0, 0.25f, 1);
                    Camera_C.rect = new Rect(0.5f, 0, 0.25f, 1);
                    Camera_D.rect = new Rect(0.75f, 0, 1, 1);
                    UICamera_A.rect = Camera_A.rect;
                    UICamera_B.rect = Camera_B.rect;
                    UICamera_C.rect = Camera_C.rect;
                    UICamera_D.rect = Camera_D.rect;
                    break;
            }
        }

        /// <summary>
        /// 게임 종료 시점을 찾기위한 함수.
        /// 각 ScoreManager에서 자신의 게임이 종료될 때 콜백으로 호출됨.
        /// 모든 ScoreManager의 게임이 종료되었다면 결과 호출
        /// </summary>
        public void CheckPlayState()
        {
            int check = 0;
            for (int i = 0; i < Scores.Length; i++)
            {
                if (Scores[i].PlayState == true)
                {
                    check += 1;
                }
            }
            if (check == 0)
            {
                SetResult();
            }
        }

        /// <summary>
        /// 최종 결과 도출.
        /// 1,2,3등까지만 찾아내며, 가장 빠른 사람이 1등
        /// </summary>
        public void SetResult()
        {
            for (int i = 0; i < TouchsButton.Length; i++)
                TouchsButton[i].SetActive(false);

            if (IsPlaying)
            {
                IsPlaying = false;
                base.Stop();
                state = RunningState.Finish;
            }            

            float min = 100000;
            int gold_idx = 0;
            int silver_idx = 0;
            int bronze_idx = 0;
            for (int i = 0; i < PlayerCount; i++)
            {
                if (min > Scores[i].Timer)
                {
                    min = Scores[i].Timer;
                    gold_idx = i;
                }
            }
            min = 100000;
            for (int i = 0; i < PlayerCount; i++)
            {
                if (i == gold_idx)
                    continue;

                if (min > Scores[i].Timer)
                {
                    min = Scores[i].Timer;
                    silver_idx = i;
                }
            }
            min = 100000;
            for (int i = 0; i < PlayerCount; i++)
            {
                if (i == gold_idx || i == silver_idx)
                    continue;
                if (min > Scores[i].Timer)
                {
                    min = Scores[i].Timer;
                    bronze_idx = i;
                }
            }
            if (PlayerCount == 1)
            {
                Scores[gold_idx].SetRank(1);
            }
            else if (PlayerCount == 2)
            {
                Scores[gold_idx].SetRank(1);
                Scores[silver_idx].SetRank(2);
            }
            else
            {
                Scores[gold_idx].SetRank(1);
                Scores[silver_idx].SetRank(2);
                Scores[bronze_idx].SetRank(3);
            }

            
         /*   Debug.Log("gold_idx" + gold_idx);
            Debug.Log("silver_idx" + silver_idx);
            Debug.Log("bronze_idx" + bronze_idx);*/
        }

        /// <summary>
        /// 모든 값 초기화(새 게임을 위한 세팅)
        /// </summary>
        public void ResetScore()
        {
            for (int i = 0; i < Scores.Length; i++)
                Scores[i].AllScoreBoxReset();
        }

    }
}
