using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using ML.SportsMiniGame.Kinect;
namespace ML.SportsMiniGame.KinectSkating
{
    public enum Stage
    {
        Stage1,
        Stage2,
        Stage3
    }
    public enum Grade
    {
        Level_1,
        Level_2,
        Level_3
    }
    public enum PlayState
    {
        Loading,
        Intro,
        UserSearch,
        Explanation,
        Ready,
        Play,
        Ending,
        Result
    }
    public class Quiz
    {
        //문항 번호
        public int QuizNumber;
        //선택지
        public string Select1, Select2;
        //정답 번호(1 or 2)
        public int Answer;
        public int QuizType;
        public string ImageName;
        public Quiz(int number, string select1, string select2, int anser, int type, string name)
        {
            QuizNumber = number;
            Select1 = select1;
            Select2 = select2;
            Answer = anser;
            QuizType = type;
            ImageName = name;
        }        

    }
    public class KinectSkateManager : MonoBehaviour
    {
        public static KinectSkateManager instance;
        public GUIManager guimanager;
        public Quiz[] quiz;
        public int QuizIdx;
        public Grade grade;
        public Stage stage;
        public PlayState playstate;
        public int[] userSelectAnser;
        public int correctCount;
        public float PlayTime;

        public Animation QuizAnimation;
     //   public Booster booster;
        public PathMove pathmove;
        public GameObject[] QuizZones;
        public CameraWork camwork;
        public GameObject Podium;

        public Transform TV_camera;
        public Transform PodiumCameraPos;
        public Transform EndingLookingTargetz;
        public ML.SportsMiniGame.Common.TV_Camera looktarget;
        public EFMsPlayer Bhoos;
        public EFMsPlayer Cheers;
        public AudioSource QuestionSound;

        public ParticleSystem Happy;
        public ParticleSystem Sad;
        public float MaxSpeed;
        public Animation finish;
        public ParticleRateController ratecontroller;
        public bool QuizType1, QuizType2;
        public BGMAudioFade IntroBGM;
        public BGMAudioFade PlayBGM;
        public BGMAudioFade EngingBGM;

        public GameObject EndingZoneObj;
        public GameObject StartLineObj;
        public QuizManager quizManager;

        public GameObject[] QuizType1Obj;
        public GameObject QuizType2Obj;
        public ResultManager result;
        public AudioSource ButtonCheck;

        public MotionGuide RunGuide, LineGuide;

        public IntroManager introManager;



        private void Awake()
        {
            QuizType1 = false;
            QuizType2 = false;
            instance = this;
            playstate = PlayState.Loading;
        }
        private void Start()
        {
            //InitQuiz();
            IntroBGM.VoluemeFadeUpStart();
            SetStage(3);
        }

        public void FixedUpdate()
        {
            if (playstate == PlayState.Play)
            {
                PlayTime += Time.deltaTime;
               //KinectHelper.instance._TrackingInclination();
            }

        }

        public void SetStage(int idx)
        {
            if (idx == 1)
            {
                stage = Stage.Stage1;
            }
            else if (idx == 2)
            {
                stage = Stage.Stage2;
            }
            else if (idx == 3)
            {
                stage = Stage.Stage3;
            }
            ButtonCheck.Play();
            InitQuiz();
            guimanager.SetUserDetect();
        }
        //입력된 사용자 정보에따라 문제타입(국기-전통의상-수도), 문제갯수, 문제 내용 세팅
        void InitQuiz()
        {
            int QuizCount = 0;
            //stage = UserDB.insetance.GetUserStage();

            if (stage == Stage.Stage1)
                QuizCount = 4;            
            else if (stage == Stage.Stage2)
                QuizCount = 6;
            else if (stage == Stage.Stage3)
                QuizCount = 8;
            quiz = new Quiz[QuizCount];
            userSelectAnser = new int[QuizCount];
            GetQuiz();
            guimanager.SetStageImage(stage);
            guimanager.QuizSet(userSelectAnser.Length, 1);
            grade = UserDB.insetance.GetUserGrade();
        }
        public void GetQuiz()
        {
            quizManager.GetQuizData(stage);
            StartCoroutine(QuizSet());

            // quiz(퀴즈넘버, 1번, 2번, 정답번호, 퀴즈타입)
            /* quiz[0] = new Quiz(0, "독일", "인도", 1, 0, "GE#Germany&India");
             quiz[1] = new Quiz(1, "프랑스", "그리스", 2, 0, "GE#France&Greece");
             quiz[2] = new Quiz(2, "인도", "일본", 1, 0, "GE#India&Japan");
             quiz[3] = new Quiz(5, "스위스", "프랑스", 1, 1, "GE#Swiss&France");
             quiz[4] = new Quiz(6, "싱가포르", "네팔", 2, 1, "GE#Singapore&Nepal");
             quiz[5] = new Quiz(7, "스위스", "스페인", 2, 1, "GE#Swiss&Spain");

               quiz[5] = new Quiz(5, "스위스", "프랑스", 1, 1, "GE#Swiss&France");
               quiz[6] = new Quiz(6, "싱가포르", "네팔", 2, 1, "GE#Singapore&Nepal");
               quiz[7] = new Quiz(7, "스위스", "스페인", 2, 1, "GE#Swiss&Spain");
               quiz[8] = new Quiz(8, "북한", "브라질", 2, 1, "GE#NorthKorea&Brazil");
               quiz[9] = new Quiz(9, "대한민국", "미국", 1, 1, "GE#Korea&UnitedStates");*/
        }
        IEnumerator QuizSet()
        {
            yield return new WaitForSeconds(0.5f);
            quiz = quizManager.SetQuiz();
        }
        //문제 등장
        public void QuizOn()
        {
            StartLineObj.SetActive(false);
            int now = QuizIdx + 1;
            //이번 문제가 마지막 문제라면
            if (now == quiz.Length)
            {
                Debug.Log("엔딩존 등장");
                EndingZoneObj.SetActive(true);
            }
            guimanager.QuizSet(userSelectAnser.Length, now);
            QuestionSound.Play();
            if (!QuizZones[0].activeSelf)
            {
                for (int i = 0; i < QuizZones.Length; i++)
                {
                    QuizZones[i].SetActive(true);
                }
            }
            StartCoroutine(QuizOnTheGame());
            //StartCoroutine(PlayObjsOn());
        }
        public IEnumerator PlayObjsOn()
        {
            yield return new WaitForSeconds(3f);
            for (int i = 0; i < QuizZones.Length; i++)
            {
                QuizZones[i].SetActive(true);
            }
        }

        public IEnumerator QuizOnTheGame()
        {            
            yield return new WaitForSeconds(0.1f);
            int tmp = (QuizIdx + 1) % 2;
            if (tmp != 0)
            {
                guimanager.QuizZone1.QuizZoneSet(quiz[QuizIdx]);
            }
            else
            {
                guimanager.QuizZone2.QuizZoneSet(quiz[QuizIdx]);
            }
            if (!QuizType1 && quiz[QuizIdx].QuizType == 0)
            {
                for (int i = 0; i < QuizType1Obj.Length; i++)
                {
                    QuizType1Obj[i].SetActive(true);
                    QuizType2Obj.SetActive(false);
                }
                QuizType1 = true;
                QuizAnimation.Play("QuizImageInit");
                yield return new WaitForSeconds(3.1f);
            }
            if (!QuizType2 && quiz[QuizIdx].QuizType == 1)
            {
                for (int i = 0; i < QuizType1Obj.Length; i++)
                {
                    QuizType1Obj[i].SetActive(false);
                    QuizType2Obj.SetActive(true);
                }
                QuizType2 = true;
                QuizAnimation.Play("QuizImageInit");
                yield return new WaitForSeconds(3.1f);
            }
            if (quiz[QuizIdx].QuizType == 0)
            {
                QuizAnimation.Play("SubQuizInit");
            }
            else if (quiz[QuizIdx].QuizType == 1)
            {
                QuizAnimation.Play("SubQuizInit_2");
            }
        }
        public IEnumerator EndingSet()
        {
            PlayBGM.VoluemeFadeDownStart();
            finish.Play("Finish");
            yield return new WaitForSeconds(1f);
            playstate = PlayState.Ending;
            yield return new WaitForSeconds(1f);
            Debug.Log("QuizZones 제거");
            for (int i = 0; i < QuizZones.Length; i++)
                QuizZones[i].SetActive(false);
            yield return new WaitForSeconds(5f);
            Debug.Log("QuizZones 제거");
            Podium.SetActive(true);
            guimanager.SetEndingGUI();
            EngingBGM.VoluemeFadeUpStart();
            Debug.Log("엔딩 GUI On, 볼륨 Fade");
            yield return new WaitForSeconds(1f);
            Debug.Log("Podium 활성화");
            //여기부터 ResultManager로 전환
            playstate = PlayState.Result;
            yield return new WaitForSeconds(1.5f);
            guimanager.ResultGUIOn();
            //yield return new WaitForSeconds(1.5f);
            result.SetPodiumCameraWork();
            //  camwork.SetCameraPodium();
            //  looktarget.SetLookTarget(PodiumCameraPos.position, EndingLookingTarget);
            //  yield return new WaitForSeconds(2f);
            guimanager.SetResult();

        }

        //선택지에 캐릭터가 닿았을 때 기록을 위해 호출
        public bool UserSelected(int selectNum)
        {
            userSelectAnser[QuizIdx] = selectNum;
            //StartCoroutine(QuizOnTheGame());
            
            if (selectNum == quiz[QuizIdx].Answer)
            {
               // booster.BoostUp();
                pathmove.maxSpeed += 0.6f;
                //정답
                Cheers.PlayRandomClips();
                int tmp = QuizIdx + 1;
                if (tmp >= quiz.Length)
                {
                    StartCoroutine(EndingSet());
                }
                StartCoroutine(QuizOut());
                QuizIdx++;
                Happy.Play();
                ratecontroller.SetParticleRate(0.2f);
                return true;
            }
            else
            {
               // booster.BoostDown();
                pathmove.maxSpeed -= 0.25f;
                Bhoos.PlayRandomClips();
                if (!RunGuide.isActive)
                    LineGuide.GuideStart(3f);
                int tmp = QuizIdx + 1;
                if (tmp >= quiz.Length)
                {
                    StartCoroutine(EndingSet());
                }
                StartCoroutine(QuizOut());
                QuizIdx++;
                Sad.Play();
                ratecontroller.SetParticleRate(-0.2f);
                return false;
            }

           
        }
        IEnumerator QuizOut()
        {
            yield return new WaitForSeconds(1f);
            QuizAnimation.Play("QuizOut");
        }
        //사용자가 선택한 정답(배열)과 퀴즈의 정답을 비교하여 맞은 갯수 확인
        public int ScoringAnser()
        {
            for (int i = 0; i < quiz.Length; i++)
            {
                if (quiz[i].Answer == userSelectAnser[i])
                    correctCount += 1;
            }
            return correctCount;
        }
        public int QuizLengthCount()
        {
            return quiz.Length;
        }

        public void InitStage()
        {
            QuizType1 = false;
            QuizType2 = false;
            instance = this;
            playstate = PlayState.Loading;
            IntroBGM.VoluemeFadeUpStart();
            QuizIdx = 0;
            
            //포듐 비활성화
            Podium.SetActive(false);
            //엔딩존 비활성화
            EndingZoneObj.SetActive(false);
            PlayTime = 0;
            //퀴즈 설정
            SetStage(3);
            pathmove.InitValues();
            introManager.InitState();
            guimanager.SetInitGUI();
            //퀴즈존 비활성화
            for(int i = 0; i < QuizZones.Length; i++)
            {
                QuizZones[i].SetActive(false);
            }
            
        }
    }
}

