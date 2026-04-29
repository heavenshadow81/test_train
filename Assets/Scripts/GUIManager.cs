using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using ML.SportsMiniGame.Kinect;
namespace ML.SportsMiniGame.KinectSkating
{
    //플레이 스케이팅 GUI설정
    public class GUIManager : MonoBehaviour
    {
        //public Sprite[] QuizSprites;
        public QuizZoneGUI QuizZone1;
        public QuizZoneGUI QuizZone2;
        public Text Timer;
        public PathMove pathmove;
        public Text Speed;
        public Text QuizData;

        public GameObject IntroGUI;
        public GameObject PlayGUI;
        public GameObject EndingGUI;
        public GameObject ResultGUI;

        public Animation EndingTransition2;
        //public Animation EndingTransition;
        public GameObject[] QuizObjs;


        public GameObject UserDetectGUI;
        public Image userDetectBar;
        public Animation UserDetectAnimation;
        public Animation IntroCameraAnim;
        public GameObject IntroExplanation;

    /*    public Text Stage1Timer;
        public Text Stage1Speed;
        public Text Stage1Quiz;*/

        public Image GameStage;
        public Sprite[] StageSprites;
        public float bar_fillamount = 0;
        public AudioSource UserCheckComplete;
        public IntroManager intro;

        private void Update()
        {

      /*      if (Input.GetKeyDown(KeyCode.Q))
            {
                UserCheckComplete.Play();
                UserDetectAnimation.Play("UserDetectOff");
                SetExplanation();
            }*/
            //유저 인식 패스
            if (Input.GetKeyDown(KeyCode.W))
            {
                intro.GameReady();
            }
            //레디 이후 시작점
            if (Input.GetKeyDown(KeyCode.E))
            {
                intro.GoAnimationStart();
            }
            if (KinectSkateManager.instance.playstate == PlayState.Play)
            {
                TimerSet();
                SpeedSet();
            }
        }
        public void SetStageImage(Stage stage)
        {
            if (stage == Stage.Stage1)
            {
                GameStage.sprite = StageSprites[0];
            }
            else if (stage == Stage.Stage2)
            {
                GameStage.sprite = StageSprites[1];
            }
            else if (stage == Stage.Stage3)
            {
                GameStage.sprite = StageSprites[2];
            }
        }
        //끝낼 때
        public void SetEndingGUI()
        {
            PlayGUI.SetActive(false);
            EndingGUI.SetActive(true);
            EndingTransition2.Play("EndingTransition");
            //EndingTransition.Play("EndingTransitionAnimation");
        }
        //결과 확인 UI

        public void ResultGUIOn()
        {
            ResultGUI.SetActive(true);
        }
        public void SetResult()
        {
            for (int i = 0; i < QuizObjs.Length; i++)
                QuizObjs[i].SetActive(false);
           // EndingTransition.Play("EndingTransitionAnimationOut");
    /*        if (KinectSkateManager.instance.stage == Stage.Stage1)
                Stage1Quiz.text = KinectSkateManager.instance.ScoringAnser();*/
        }
       
        void TimerSet()
        {
            float time = KinectSkateManager.instance.PlayTime;
            int min = 0;
            float sec = 0;
            string time_text;

            if (time >= 60)
            {
                min = (int)(time / 60);
                time_text = min + ":";
                sec = time - (min * 60);
                if (sec < 10)
                    time_text += "0";
                time_text = string.Format(time_text + sec.ToString("N2"));
            }
            else
            {
                time_text = min + ":";
                if (time < 10)
                    time_text += "0";
                time_text = string.Format(time_text + time.ToString("N2"));
            }
          /*  if (KinectSkateManager.instance.stage == Stage.Stage1)
                Stage1Timer.text = time_text;*/
            Timer.text = time_text;
        }
        void SpeedSet()
        {
            float speed = pathmove.movingSpeed;
            //최대 속력 35km/h
            speed = speed * 3.5f;
            if (KinectSkateManager.instance.MaxSpeed < speed)
            {
                KinectSkateManager.instance.MaxSpeed = speed;
             /*   if (KinectSkateManager.instance.stage == Stage.Stage1)
                    Stage1Speed.text = string.Format(speed.ToString("N1") + "km/h");*/
            }
            Speed.text = string.Format(speed.ToString("N1") + "km/h");
        }
        public void QuizSet(int total, int now)
        {
            QuizData.text = now.ToString() + " / " + total.ToString();
        }
        public void SetGamePlay()
        {
            IntroGUI.SetActive(false);
            PlayGUI.SetActive(true);
        }

        public void SetUserDetect()
        {
            IntroExplanation.SetActive(false);
            UserDetectGUI.SetActive(true);
            UserDetectAnimation.Play("UserDetectOn");
        }
        public void UserDetectStart()
        {
            StartCoroutine(Detecting());
        }
        IEnumerator Detecting()
        {
            while (true)
            {
                //if (KinectHelper.instance.UserDetected)
                //{
                //    bar_fillamount += 1f * Time.deltaTime;
                //    KinectHelper.instance.SetHeadPosition();
                //}
                //else
                    bar_fillamount -= 2f * Time.deltaTime;

                if (bar_fillamount < 0)
                {
                    bar_fillamount = 0;
                    //KinectHelper.instance.ResetHeadPosition();
                }

                if (bar_fillamount >= 1)
                {
                    bar_fillamount = 1;
                    userDetectBar.fillAmount = bar_fillamount;
                    break;
                }
                userDetectBar.fillAmount = bar_fillamount;
                yield return new WaitForSeconds(0.01f);
            }
            UserCheckComplete.Play();
            UserDetectAnimation.Play("UserDetectOff");
            yield return new WaitForSeconds(1f);
            //SetExplanation();
        }
        public void SetExplanation()
        {
            IntroCameraAnim.Play("UserSearchStart");
            //IntroExplanation.SetActive(true);
            KinectSkateManager.instance.playstate = PlayState.Explanation;
        }

        //초기 값으로 되돌리기
        public void SetInitGUI()
        {
            EndingGUI.SetActive(false);
            ResultGUI.SetActive(false);
            PlayGUI.SetActive(false);
            IntroGUI.SetActive(true);           
        }
    }
}

