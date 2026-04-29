using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class UserData
    {
        //NFC코드, 이름, 나이, 성별, 키, 몸무게, 학년, 반, 한마디, 게임레벨
        public string NFC_Code;
        public string Name;
        public int Age;
        public string Gender;
        public float Height;
        public float Weight;
        public int ClassLevel;
        public int ClassNumber;
        public string Saying;
        public int GameLevel;
    }
    public class UserSpeedSkateData
    {
        //NFC코드, 학년, 스피드스케이팅 레벨, 스테이지1 ~ 3 점수
        public string NFC_Code;
        public int ClassLevel;
        public int StageLevel;
        public int Stage1Score;
        public int Stage2Score;
        public int Stage3Score;
    }
    public class UserDB : MonoBehaviour
    {
        public UserData userdata;
        public UserSpeedSkateData skatedata;
        public static UserDB insetance; 

        private void Awake()
        {
            InitValues();
        }
        public void InitValues()
        {
            insetance = this;
            GetUserDataFromServer();
        }
        void GetUserDataFromServer()
        {
            userdata = new UserData();
            userdata.NFC_Code = "000101010";
            userdata.Name = "홍길동";
            userdata.Age = 12;
            userdata.Gender = "남";
            userdata.Height = 155;
            userdata.Weight = 45;
            userdata.ClassLevel = 5;
            userdata.ClassNumber = 3;
            userdata.Saying = "다덤벼";
            userdata.GameLevel = 1;

            skatedata = new UserSpeedSkateData();
            skatedata.NFC_Code = "000101010";
            skatedata.StageLevel = 3;
            skatedata.Stage1Score = 0;
            skatedata.Stage2Score = 0;
            skatedata.Stage3Score = 0;
        }
        public Grade GetUserGrade()
        {
            if (userdata.ClassLevel == 1 || userdata.ClassLevel == 2)
            {
                return Grade.Level_1;
            }
            else if (userdata.ClassLevel == 3 || userdata.ClassLevel == 4)
            {
                return Grade.Level_2;
            }
            else
            {
                return Grade.Level_3;
            }
        }
        public Stage GetUserStage()
        {
            if (skatedata.StageLevel == 1)
            {
                return Stage.Stage1;
            }
            else if (skatedata.StageLevel == 2)
            {
                return Stage.Stage2;
            }
            else
            {
                return Stage.Stage3;
            }

        }

    }
}

