using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ML.T_Sports.Common;

namespace ML.T_Sports.ShotPut
{
    public enum ShotPutState
    {
        Ready,
        Play,
        Pause,
        Finish
    }
         
    public class ShotPutManager : ContentsManagerBase
    {
        public static ShotPutManager instance;
        public ShotPutState state;
        public int PlayerCount;

        public GameObject Player_A;
        public Camera Camera_A, UI_A;
        public RectTransform ScoreBoxA;

        public GameObject Player_B;
        public Camera Camera_B, UI_B;
        public RectTransform ScoreBoxB;

        public GameObject Player_C;
        public Camera Camera_C, UI_C;
        public RectTransform ScoreBoxC;

        public ShotPutScoreManager[] scores;
        public ShotPutTouchPoint touch;

        public GameObject[] VerticalLines;
        public EFMPlayer Cheers;
        public BallsOP ballsop;

        public GameObject ReadyObject;

        public GameObject[] coins;
        public GameObject[] coinChild;

        public override void Init()
        {
            InitProperty(ContentsPropertyType.Chance, 3, 10, 10);
            InitProperty(ContentsPropertyType.Player, 1, 3, 1);
            InitProperty(ContentsPropertyType.GameMode, 0, 1, 0);
            SetPropertyValue(ContentsPropertyType.Player, 1);

            int chance = GetPropertyValueInt(ContentsPropertyType.Chance);
            int playercound = GetPropertyValueInt(ContentsPropertyType.Player);
            SetPlayer(playercound);
            for (int i = 0; i < scores.Length; i++)
            {
                scores[i].SetChance(chance);
            }
            for (int i = 0; i < playercound; i++)
            {
                scores[i].SetPlayScore();
            }
            instance = this;
            state = ShotPutState.Ready;
        }
        private void Start()
        {
            float sfx = GetSharedPropertyValueFloat(ContentsPropertyType.SFX);
            Common.SoundManager.instance.SetEFMVolume(sfx);
            float bgm = GetSharedPropertyValueFloat(ContentsPropertyType.BGM);
            Common.SoundManager.instance.SetBGMVolume(bgm);

            for (int i = 0;i < coins.Length;i++)
            {
                coins[i].SetActive(false);
            }
            coins[Random.Range(0,3)].SetActive(true);
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
        public override void Play()
        {
            if (!IsPlaying)
            {
                ReadyObject.SetActive(false);
                base.Play();
                SetPlay();
                state = ShotPutState.Play;
            }
        }
        public GameObject PauseCanvas;
        public override void Pause()
        {
            base.Pause();
            PauseCanvas.SetActive(IsPaused);
        }
        public override void Stop()
        {
            if (IsPlaying)
                IsPlaying = false;
            base.Stop();
            state = ShotPutState.Finish;
            for (int i = 0; i < scores.Length; i++)
            {
                scores[i].SetStopFinish();
            }
            //여기서 엔딩 호출해야 함.
        }
        /// <summary>
        /// 옵션 변경시 실시간 값 변경
        /// </summary>
        public override void OnChangePropertyValue(ContentsPropertyType type, float prevValue, float newValue)
        {
            switch (type)
            {
                case ContentsPropertyType.Chance:
                    int playercound = GetPropertyValueInt(ContentsPropertyType.Player);
                    for (int i = 0; i < playercound; i++)
                    {
                        scores[i].SetChance((int)newValue);
                    }
                    break;
                case ContentsPropertyType.Player:
                    SetPlayer((int)newValue);
                    PlayerCount = (int)newValue;
                    break;
            }
        }
       
        /// <summary>
        /// 게임 플레이 버튼입력시 현재 옵션값으로 세팅
        /// </summary>
        public void SetPlay()
        {
            ballsop.BallReset();
            int playercound = GetPropertyValueInt(ContentsPropertyType.Player);
            int chance = GetPropertyValueInt(ContentsPropertyType.Chance);
            PlayerCount = playercound;
            for (int i = 0; i < playercound; i++)
            {
                scores[i].SetChance(chance);
            }

            //플레이어 숫자만큼만 스코어매니저 세팅
            for (int i = 0; i < playercound; i++)
            {
                scores[i].SetPlayScore();
            }

            for (int i = 0; i < scores.Length; i++)
            {
                scores[i].ResetScore();
            }
            SetPlayer(playercound);

            for (int i = 0; i < coins.Length; i++)
            {
                coins[i].SetActive(false);
            }

            for (int i = 0; i < coinChild.Length; i++)
            {
                if (coinChild[i] != null)
                {
                    coinChild[i].GetComponent<MeshRenderer>().enabled = true;
                    coinChild[i].GetComponent<BoxCollider>().enabled = true;
                }
            }
            
            coins[Random.Range(0, 3)].SetActive(true);
        }
        public void SetPlayer(int playerValue)
        {
            switch (playerValue)
            {
                case 1:
                    Player_B.SetActive(false);
                    Player_C.SetActive(false);
                    Camera_A.rect = new Rect(0, 0, 1, 1);
                    UI_A.rect = Camera_A.rect;
                    ScoreBoxA.anchoredPosition = new Vector2(0, -134);
                    ScoreBoxA.localScale = new Vector3(1,1,1);
                    touch.Player = 1;

                    for (int i = 0; i < VerticalLines.Length; i++)
                    {
                        VerticalLines[i].SetActive(false);
                    }
                    break;
                case 2:
                    Player_B.SetActive(true);
                    Player_C.SetActive(false);
                    Camera_A.rect = new Rect(0, 0, 0.5f, 1);
                    Camera_B.rect = new Rect(0.5f, 0, 1, 1);
                    UI_A.rect = Camera_A.rect;
                    UI_B.rect = Camera_B.rect;
                    ScoreBoxA.anchoredPosition = new Vector2(0, -234);
                    ScoreBoxA.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    ScoreBoxB.anchoredPosition = new Vector2(0, -234);
                    ScoreBoxB.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    touch.Player = 2;
                    VerticalLines[0].SetActive(true);
                    VerticalLines[1].SetActive(false);
                    VerticalLines[2].SetActive(false);
                    break;
                case 3:
                    Player_B.SetActive(true);
                    Player_C.SetActive(true);
                    Camera_A.rect = new Rect(0, 0, 0.33f, 1);
                    Camera_B.rect = new Rect(0.33f, 0, 0.33f, 1);
                    Camera_C.rect = new Rect(0.66f, 0, 1, 1);
                    ScoreBoxA.anchoredPosition = new Vector2(0, -326);
                    ScoreBoxA.localScale = new Vector3(0.55f, 0.55f, 0.55f);
                    ScoreBoxB.anchoredPosition = new Vector2(0, -326);
                    ScoreBoxB.localScale = new Vector3(0.55f, 0.55f, 0.55f);
                    ScoreBoxC.anchoredPosition = new Vector2(0, -326);
                    ScoreBoxC.localScale = new Vector3(0.55f, 0.55f, 0.55f);
                    UI_A.rect = Camera_A.rect;
                    UI_B.rect = Camera_B.rect;
                    UI_C.rect = Camera_C.rect;
                    touch.Player = 3;
                    VerticalLines[0].SetActive(false);
                    VerticalLines[1].SetActive(true);
                    VerticalLines[2].SetActive(true);
                    break;
            }
        }
        public void FinishCheck()
        {
            int check = 0;
            for (int i = 0; i < PlayerCount; i++)
            {
                if (scores[i].state == PlayerState.Play)
                {
                    check = 1;
                }
            }
            if (check == 0)
            {
                Debug.Log("게임 종료 ");
                StartCoroutine(SetFinish());
            }
        }
        IEnumerator SetFinish()
        {
            yield return new WaitForSeconds(1f);
            int maxScore = 0;
            int[] winneridx_score = new int[3];
            int _winnerIdx = 0;
            for (int i = 0; i < winneridx_score.Length; i++)
                winneridx_score[i] = 100;
            for (int i = 0; i < scores.Length; i++)
            {
                if (scores[i].MyScore > maxScore)
                {
                    winneridx_score[_winnerIdx] = i;
                    _winnerIdx++;
                    maxScore = scores[i].MyScore;
                }
                else if (scores[i].MyScore == maxScore)
                {
                    winneridx_score[_winnerIdx] = i;
                    _winnerIdx++;
                }
            }
            if (_winnerIdx >= 2)
            {
                //동점자가 있는경우 거리로 승자 가리기
                float max_Distance = 0;
                int LastWinnerIdx = 100;
                for (int i = 0; i < winneridx_score.Length; i++)
                {
                    if (winneridx_score[i] != 100)
                    {
                        if (max_Distance < scores[winneridx_score[i]].AddAllDistance)
                        {
                            max_Distance = scores[winneridx_score[i]].AddAllDistance;
                            LastWinnerIdx = winneridx_score[i];
                        }
                    }
                }
                if(LastWinnerIdx < scores.Length)
                    scores[LastWinnerIdx].SetWinner();

            }
            else
            {
                //단독 승자
                scores[winneridx_score[0]].SetWinner();
            }
            Cheers.EFMRandomPlay();

            if (IsPlaying)
                IsPlaying = false;
            base.Stop();
            state = ShotPutState.Finish;
        }
        public bool CheckScoreState(int idx)
        {
            if (scores[idx].state == PlayerState.Play)
                return true;
            else
                return false;
        }
        public void SetScore(int idx, float Distance)
        {
            scores[idx].AddDistance(Distance);
        }
        public void SetOut(int idx)
        {
            scores[idx].SetBallOut();
        }
        public void SetPlayerAngle(int idx, bool active)
        {
            scores[idx].SetAngleGUI(active);
        }

    }


}
