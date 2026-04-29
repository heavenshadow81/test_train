using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace ML.SportsMiniGame.KinectSkating
{
    public class ResultManager : MonoBehaviour
    {
        
        public GameObject ResultGUI;
        public GameObject TimerObj;
        public GameObject QuizObj;

        public GameObject CorrectObj;
        public GameObject ScoreObj;
        public GameObject RenderTextureCam;

        public GameObject RenderGUI;
        public Animation WinLoseAnimation;
        public Image Background;

        public Image BlackOut;
        public Image Win_Lose;
        public Sprite win, lose;

        public Text Timer;
        public Text TotalQuiz;
        public Text CorrectQuiz;

        public Text ScoreText;
        public bool EndingStart;
        public bool BackGroundState;

        public bool WinLoseState;
        public bool TimeState;
        public bool ScoreState;

        public Transform CameraTr;
        public Transform[] PodiumCamPos;
        public float speed;

        public ParticleSystem Powder;
        public AudioSource EndingGUIOn;
        public AudioSource ScoreCount;

        public EFMsPlayer cheer;

        [SerializeField]
        TransitionTrigger endingtransition;

        private void Awake()
        {
            InitValues();
        }
        public void InitValues()
        {
            EndingStart = false;
            BackGroundState = false;
            WinLoseState = false;
            TimeState = false;
            ScoreState = false;
        }
        public void SetPodiumCameraWork()
        {
            CameraTr.position = PodiumCamPos[0].position;
            RenderTextureCam.SetActive(false);
            RenderGUI.SetActive(false);
            StartCoroutine(CameraWork());
        }
        //엔딩에서 카메라 움직임
        IEnumerator CameraWork()
        {
            while (true)
            {
                CameraTr.position = Vector3.Slerp(CameraTr.position, PodiumCamPos[1].position, Time.deltaTime * speed);
                float dist = Vector3.Distance(CameraTr.position, PodiumCamPos[1].position);
                if (dist < 0.5f)
                {
                    break;
                }
                yield return new WaitForSeconds(0.01f);
            }
            Powder.Play();
            yield return new WaitForSeconds(2.5f);
            StartCoroutine(ResultProcess());
        }
        //승, 패 확인 및 적용
        public void SetWinLose(bool winlose)
        {
            if (winlose)
                Win_Lose.sprite = win;
            else
                Win_Lose.sprite = lose;
            Win_Lose.rectTransform.anchoredPosition = new Vector3(-254, 900, 0);
        }
        public void ResultProcessing()
        {
            if (!EndingStart)
            {
                EndingStart = true;
                ResultGUI.SetActive(true);
            }
        }
        IEnumerator BackgroundImageInit()
        {
            Color blackoutColor = new Color(0,0,0,0);
            BlackOut.color = blackoutColor;
            Color col = new Color(1,1,1,0);
            Background.color = col;
            while (true)
            {
                if (blackoutColor.a < 0.8)
                {
                    blackoutColor.a += Time.deltaTime * 3;
                    BlackOut.color = blackoutColor;
                }

                col.a += Time.deltaTime;
                Background.color = col;
                if (col.a >= 1)
                    break;
                yield return new WaitForSeconds(0.01f);
            }
            BackGroundState = true;
        }        
        IEnumerator TimerCount()
        {
            float time = KinectSkateManager.instance.PlayTime;
            int min = 0;
            int sec = 0;
            int undersec = 0;

            if (time >= 60)
            {
                min = (int)(time / 60);
                sec = (int)(time - (min * 60));
                undersec = (int)(((time - (min * 60)) - sec) * 100);
            }
            else
            {
                min = 0;
                sec = (int)(time - (min * 60));
                undersec = (int)(((time - (min * 60)) - sec) * 100);
            }
            int min_tmp = 0;
            int sec_tmp = 0;
            int undersec_tmp = 0;
            while (true)//00:00.00
            {
                if (min_tmp < min)
                    min_tmp += 1;
                else
                    min_tmp = min;

                if (sec_tmp < sec)
                    sec_tmp += Random.Range(1, 7);
                else
                    sec_tmp = sec;

                if (undersec_tmp < undersec)
                    undersec_tmp += Random.Range(1, 7);
                else
                    undersec_tmp = undersec;

                string sectext = "";
                string undersectext = "";
                if (sec_tmp < 10)
                    sectext = "0" + sec_tmp.ToString();
                else
                    sectext = sec_tmp.ToString();

                if (undersec_tmp < 10)
                    undersectext = "0" + undersec_tmp.ToString();
                else
                    undersectext = undersec_tmp.ToString();

                string timetext = string.Format(min_tmp+":"+ sectext + "."+ undersectext) ;
                Timer.text = timetext;
                if (min_tmp >= min && sec_tmp >= sec && undersec_tmp >= undersec)
                    break;
                yield return new WaitForSeconds(0.01f);
            }

            min_tmp = min;
            sec_tmp = sec;
            undersec_tmp = undersec;
            TimeState = true;
        }
        IEnumerator CountQuiz(int count, Text CountText)
        {
            int tmp = 0;
            string str = "";
            while (true)
            {
                str = "";
                str = tmp.ToString() + "문제";
                CountText.text = str;
                if (tmp < count)
                {
                    tmp++;
                }
                else
                    break;
                yield return new WaitForSeconds(0.1f);
            }
            tmp = count;
            str = tmp.ToString() + "문제";
            CountText.text = str;
        }
        IEnumerator CountScore(int count)
        {
            int tmp = 0;
            string str = "";
            while (true)
            {
                str = "";
                str = tmp.ToString() + "점";
                ScoreText.text = str;
                if (tmp < count)
                {
                    tmp += Random.Range(1,11);
                }
                else
                    break;
                yield return new WaitForSeconds(0.01f);
            }
            tmp = count;
            str = tmp.ToString() + "점";
            ScoreText.text = str;
            ScoreCount.Stop();
            cheer.PlayRandomClips();
        }
        IEnumerator ResultProcess()
        {
            int quiz = KinectSkateManager.instance.QuizLengthCount();
            int correct = KinectSkateManager.instance.ScoringAnser();
            float time = KinectSkateManager.instance.PlayTime;
            float maxtime = quiz * 25;
            //승 패 체크
            ResultProcessing();

            StartCoroutine(BackgroundImageInit());
            while (!BackGroundState)
            {
                yield return new WaitForSeconds(0.1f);
            }
            EndingGUIOn.Play();
            WinLoseAnimation.Play("WinLose");
            yield return new WaitForSeconds(1f);
            TimerObj.SetActive(true);
            ScoreCount.Play();
            StartCoroutine(TimerCount());
            while (!TimeState)
            {
                yield return new WaitForSeconds(0.1f);
            }
            QuizObj.SetActive(true);
            CorrectObj.SetActive(true);
            StartCoroutine(CountQuiz(quiz, TotalQuiz));
            StartCoroutine(CountQuiz(correct, CorrectQuiz));
            yield return new WaitForSeconds(0.5f);
            ScoreObj.SetActive(true);

            float acc = correct / quiz;
            time = time / 10;
            maxtime = maxtime / 10;
            float score = acc * 100 ;
            score = score - time + maxtime;

            //보너스 : 추가 점수요소 * 15
            float tmp = (acc * 100) - score;
            float bonus = (tmp * -1) * 15;

            //최종 스코어는 정답률 * 100 + 시간 보너스
            float lastScore = (acc * 100) + bonus;
            if (lastScore < 0)
                lastScore = 0;
            StartCoroutine(CountScore((int)lastScore));
            yield return new WaitForSeconds(2f);

            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);

            //재시작을 여기에
            QuizObj.SetActive(false);
            CorrectObj.SetActive(false);
            TimerObj.SetActive(false);
            ScoreObj.SetActive(false);
            RenderTextureCam.SetActive(true);
            RenderGUI.SetActive(true);
            InitValues();
            ResultGUI.SetActive(false);
            FindObjectOfType<PathMovePlayAI>().InitValues();
            endingtransition.gameObject.SetActive(true);
            KinectSkateManager.instance.InitStage();
            print("재시작");
        }
    }
}