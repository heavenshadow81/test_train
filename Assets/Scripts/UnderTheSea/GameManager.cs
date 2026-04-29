using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using static Settings;

namespace UnderTheSea
{
    public class GameManager : MonoBehaviour
    {
        public GameObject start; //시작화면 오브젝트
        public GameObject end; //게임종료 오브젝트
        public GameObject fail; //게임실패 오브젝트
        public GameObject quizText; //퀴즈 문구 오브젝트

        public TextMeshProUGUI[] quiz; //퀴즈 텍스트 
        List<int> quizList = new List<int>(); //퀴즈순서 리스트

        public Animator[] jelly; //젤리피쉬 애니메이터

        bool result; //퀴즈결과 값 



        public ZoZoBasePatton<GameManager> zozo;
        public EnumClass stateClass;
        public GameUI gameUI;
        public ScreenProsess screenProsess;

        private void Awake()
        {
            stateClass = new EnumClass();
            #region 공용 스테이트 패턴 

            ActionProcess.Enter_StateListener(Init, null, game, null);

            zozo = new ZoZoBasePatton<GameManager>();
            zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
            #endregion
        }

        // Start is called before the first frame update
        void Init()
        {

         //   start.SetActive(true); //시작할 때 시작화면 활성화
          //  end.SetActive(false); //시작할 때 종료화면 비활성화
         //   fail.SetActive(false); //시작할 때 실패화면 비활성화
            quizText.SetActive(false); //시작할 때 퀴즈 오브젝트 비활성화

            quizRandom(); //퀴즈 랜덤 함수 실행

            for (int i = 0; i < 3; i++) //3회 반복
                quiz[quizList[i]].text = FishSpawner.fishList[i].ToString(); //퀴즈정답을 퀴즈텍스트에 저장

            result = false;
        }
        public void quizRandom() //퀴즈답 순서 랜덤 배정 함수
        {
            int currentNumber = Random.Range(0, quiz.Length); //현재 숫자는 0~3 중에 랜덤으로 배정

            for (int i = 0; i < 3;) //3회반복
            {
                if (quizList.Contains(currentNumber)) //퀴즈리스트에 현재 숫자가 있다면
                {
                    currentNumber = Random.Range(0, quiz.Length); //다시 랜덤으로 지정
                }
                else
                {
                    quizList.Add(currentNumber); //현재 숫자를 퀴즈 리스트에 추가
                    i++; //반복 개수 상승
                }
            }
        }
        // Update is called once per frame
        void Update()
        {
            if(FishSpawner.quiz) //퀴즈가 활성화 되었다면
            {
                StartCoroutine(Quiz()); //퀴즈 코루틴 활성화
            }
        }
        public void game() //게임시작 함수
        {
           // start.SetActive(false); //시작화면 비활성화
            GameObject.Find("FishSpawner").GetComponent<FishSpawner>().GameStart(); //피쉬스포너에 게임스타트 함수 실행
        }
        IEnumerator Quiz() //퀴즈 코루틴
        {
            yield return new WaitForSeconds(3); //3초 뒤에

            if (FishSpawner.quiz) //퀴즈가 활성화 되었다면
            {
                jelly[0].SetTrigger("Quiz"); //젤리1 퀴즈 애니메이션 실행
                jelly[1].SetTrigger("Quiz"); //젤리2 퀴즈 애니메이션 실행
                jelly[2].SetTrigger("Quiz"); //젤리3 퀴즈 애니메이션 실행
                quizText.SetActive(true); //퀴즈 오브젝트 활성화

                GameObject.Find("SoundManager").GetComponent<SoundManager>().Jelly(); //젤리사운드 함수 실행
            }
        }

        public void jellyBtn() //젤리버튼 함수
        {
            FishSpawner.quiz = false; //퀴즈 비활성화
            if (!result)
            {
                if (EventSystem.current.currentSelectedGameObject.name == quiz[quizList[0]].gameObject.transform.parent.name) //클릭한 버튼의 이름과 정답 버튼의 이름이 같다면
                {
                    stateClass.resultState = GameResult.Success;
                    zozo.Change(GameState.GameResult);
                    //end.SetActive(true); //게임종료 활성화
                    GameObject.Find("SoundManager").GetComponent<SoundManager>().Correct(); //코렉트 사운드 함수 실행
                    result = true; //결과값을 활성화
                }
                else //아니라면
                {
                    GameObject.Find("SoundManager").GetComponent<SoundManager>().Wrong(); //롱 사운드 함수 실행
                    StartCoroutine(Fail()); //패일 코루틴 실행
                    result = true; //결과값을 활성화
                }
            }
        }

        IEnumerator Fail() //패일 코루틴
        {
            quiz[quizList[0]].gameObject.transform.parent.gameObject.transform.DOScale(1.1f, 1); //정답 커지기
            quiz[quizList[0]].gameObject.transform.parent.gameObject.transform.DOShakeRotation(1,45,6); //정답 흔들기
            quiz[quizList[1]].gameObject.transform.parent.gameObject.SetActive(false); //오답 1번 비활성화
            quiz[quizList[2]].gameObject.transform.parent.gameObject.SetActive(false); //오답 2번 비활성화

            yield return new WaitForSeconds(1.5f); //1.5초 대기

            // fail.SetActive(true); //실패화면 활성화
            stateClass.resultState = GameResult.Fail;
            zozo.Change(GameState.GameResult);
        }
    }
}
