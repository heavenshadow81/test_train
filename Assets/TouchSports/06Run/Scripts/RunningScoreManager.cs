using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ML.T_Sports.Common;
namespace ML.T_Sports.Running
{
    public class RunningScoreManager : MonoBehaviour
    {
        /// <summary>
        /// 이 ScoreManager가 게임을 진행중이면 True 아니면 False
        /// </summary>
        public bool PlayState;
        /// <summary>
        /// 스코어박스 세팅(3개, 4개, 5개짜리)
        /// </summary>
        public GameObject[] ScoreBoxs;
        /// <summary>
        /// 스코어박스 세트의 데이터(텍스트 배열)
        /// </summary>
        public RunningScoreBox[] ScoreBoxDatas;

        /// <summary>
        /// 스코어박스 세트의 데이터로부터 받아오는 텍스트 배열(시간 기록을 위한 텍스트)
        /// </summary>
        public Text[] TimeRecode;
        /// <summary>
        /// 기록 순번 Index
        /// </summary>
        public int RecodeIdx;
        /// <summary>
        /// 기록 최대 횟수(반복 터치 횟수)
        /// </summary>
        public int RecodeCount;        

        /// <summary>
        /// 순위 애니메이션
        /// </summary>
        public Animation Rank;
        /// <summary>
        /// 순위 이미지 리셋을 위한 RectTransform
        /// </summary>
        public RectTransform RankRect;
        /// <summary>
        /// 순위에 따라 메달을 변경하기위한 변수
        /// </summary>
        public Image RankImage;
        public Sprite[] RankSprites;
        /// <summary>
        /// 중복 터치를 제한하기 위한 터치 쿨타임.
        /// 현재 1초 설정
        /// </summary>
        public float RecodeCoolTime;
        /// <summary>
        /// 터치 효과음
        /// </summary>
        public EFMPlayer positive;

        public float Timer;
        public Animation touch;
        public RunningStage MyRunningStage;

        public Text LastRecode;

        private void Awake()
        {
            RecodeCoolTime = 1;
            PlayState = false;
            RecodeIdx = 0;
        }
        public void TimeTextSet(Text PlayTimer)
        {
            int min = (int)Timer / 60;
            int sec = (int)(Timer - (min * 60));
            int underSec = (int)((Timer - (min * 60) - sec) * 100);
            string strsec = "";
            string strundersec = "";
            string strmin = "";
            if (min < 10)
            {
                strmin = "0" + min.ToString();
                PlayTimer.color = Color.yellow;
                PlayTimer.gameObject.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            }
            else
            {
                strmin = min.ToString();
            }
            if (sec < 10)
            {
                strsec = "0" + sec.ToString();
            }
            else
            {
                strsec = sec.ToString();
            }
            if (underSec < 10)
            {
                strundersec = "0" + underSec.ToString();
            }
            else
            {
                strundersec = underSec.ToString();
            }
            PlayTimer.text = strmin + ":" + strsec + ":" + strundersec;
        }
        /// <summary>
        /// 게임 플레이시 타이머 진행 및 쿨타임관리
        /// </summary>
        private void Update()
        {
            if (PlayState)
            {
                Timer += Time.deltaTime;
                TimeTextSet(TimeRecode[RecodeIdx]);
                if (RecodeIdx != 0)
                {
                    TimeRecode[RecodeIdx - 1].color = Color.white;
                    TimeRecode[RecodeIdx - 1].gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                }
                if (RecodeCoolTime > 0)
                {
                    RecodeCoolTime -= Time.deltaTime;
                }
            }
        }
        /// <summary>
        /// 결과 순위에 따른 랭크 세팅
        /// 1, 2, 3위만 랭크 효과 발생
        /// </summary>
        public void SetRank(int rank)
        {
            for (int i = 0; i < ScoreBoxs.Length; i++)
                ScoreBoxs[i].SetActive(false);
            switch (rank)
            {
                case 1:
                    RankImage.sprite = RankSprites[0];
                    Rank.Play("RankUp");
                    break;
                case 2:
                    RankImage.sprite = RankSprites[1];
                    Rank.Play("RankUp");
                    break;
                case 3:
                    RankImage.sprite = RankSprites[2];
                    Rank.Play("RankUp");
                    break;
                case 4:
                    break;
            }

            RecodeIdx = 0;
        }
        public void SetStopGame()
        {
            //StopRunning();
            //PlayState = false;
            RecodeCount = 0;
            RecodingTime();
        }
        public void StopRunning()
        {
            MyRunningStage.StopRun();
        }

        /// <summary>
        /// 터치할 때 시간을 기록함.
        /// 기록 횟수가 최대 횟수에 도달했다면 RunningManager에게 게임이 끝났는지 확인을 위한 콜백을 보냄.
        /// </summary>
        /// 
        public void RecodingTime()
        {
            if (PlayState)
            {
                if (RecodeCoolTime > 0)
                    return;
                positive.EFMRandomPlay();
                RecodeCoolTime = 1f;
                RecodeIdx++;

                touch.Play("Touch");
                if (RecodeIdx >= RecodeCount)
                {              
                    PlayState = false;
                    TimeTextSet(LastRecode);
                    LastRecode.color = Color.white;
                    LastRecode.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);

                    RunningManager.instance.CheckPlayState();
                    StopRunning();
                }
            }
        }
        /// <summary>
        /// 게임이 실행중이 아닌경우에만 작동함.
        /// 게임이 시작하는 함수.
        /// </summary>
        public void SetRunningPlay(int ChanceValue)
        {
            if (PlayState)
            {
                Debug.Log("플레이어가 이미 게임 중 입니다.");
                return;
            }
            Timer = 0;
            LastRecode.text = "00:00:00";
            MyRunningStage.GoRun(10);
            PlayState = true;
            RecodeCount = ChanceValue;
            SetScoreBox(RecodeCount, true);
        }
        /// <summary>
        /// 컨트롤러로부터 받은 ChanceValue 값에 따라 스코어박스 오브젝트를 변경함(3개짜리, 4개짜리, 5개짜리)
        /// </summary>
        public void SetScoreBox(int ChanceValue, bool t)
        {
            switch (ChanceValue)
            {
                case 3:
                    SetScoreBox(0);
                    break;
                case 4:
                    SetScoreBox(1);
                    break;
                case 5:
                    SetScoreBox(2);
                    break;
            }
        }
        /// <summary>
        /// 기록 횟수(RecodeCount)에 따라 사용할 스코어박스를 활성화 하고, 기록 데이터 초기화
        /// </summary>
        void SetScoreBox(int idx)
        {
           
            for (int i = 0; i < ScoreBoxs.Length; i++)
            {
                ScoreBoxs[i].SetActive(false);
            }
            ScoreBoxs[idx].SetActive(true);
            TimeRecode = ScoreBoxDatas[idx].GetScoreText();

            for (int i = 0; i < TimeRecode.Length; i++)
            {
                TimeRecode[i].text = "00:00:00";
                TimeRecode[i].color = Color.white;
                TimeRecode[i].gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            AllScoreBoxReset();
        }
        /// <summary>
        /// 스코어박스 리셋
        /// </summary>
        public void AllScoreBoxReset()
        {
            RecodeIdx = 0;
            RankRect.anchoredPosition = new Vector2(0,-2000f);
            for (int i = 0; i < ScoreBoxDatas.Length; i++)
                ScoreBoxDatas[i].ResetScore();
        }
    }
}

