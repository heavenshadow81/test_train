using Bax.P0.Client.UnityWorld.PictureGame;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static Settings;

namespace CatRock
{
    public class ButtonManager : MonoBehaviour
    {
        public SpriteRenderer playerHand; //플레이어 냥이 손 스프라이트 렌더러
        public SpriteRenderer playerHead; //플레이어 냥이 얼굴 스프라이트 렌더러

        public SpriteRenderer enemyHand; //상대방 냥이 손 스프라이트 렌더러
        public SpriteRenderer enemyHead; //상대방 냥이 얼굴 스프라이트 렌더러

        public Animator playerAnim; //플레이어 냥이 애니메이션
        public Animator enemyAnim; //상대방 냥이 애니메이션

        [SerializeField]
        Sprite[] player; //플레이어 냥이 가위바위 + 얼굴 이미지 배열

        [SerializeField]
        Sprite[] white; //흰냥이 가위바위보 + 얼굴 이미지 배열

        [SerializeField]
        Sprite[] cheese; //치즈냥이 가위바위보 + 얼굴 이미지 배열

        [SerializeField]
        Sprite[] black; //깜냥이 가위바위보 + 얼굴 이미지 배열

        public static int playerLife; //플레이어 라이프
        public static int enemyLife; //에너미 라이프

        public static bool cheeseCat; //치즈냥이 활성화값
        public static bool blackCat; //깜냥이 활성화값

        [SerializeField]
        GameObject gameOver; //게임 종료시 나올 오브젝트

        [SerializeField]
        GameObject gameClear; //게임 클리어시 나올 오브젝트

        [SerializeField]
        GameObject gameStart; //게임 시작시 나올 오브젝트

        [SerializeField]
        GameObject[] playerHeart; //플레이어 하트 UI

        [SerializeField]
        GameObject[] enemyHeart; //에너미 하트 UI

        public static bool touch; //더치 확인 값

        public EnumClass stateClass;
        public GameUI gameui;
        public ScreenProsess screenProsess;
        public ZoZoBasePatton<ButtonManager> zozo;

        private void Awake()
        {
            stateClass = new EnumClass();
            #region 공용 스테이트 패턴 

            ActionProcess.Enter_StateListener(null, null, () => 
            {
                playerHand.sprite = player[2]; //시작할 때 플레이어 냥이 보 상태
                playerHead.sprite = player[3]; //시작할 때 플레이어 냥이 기본 얼굴

                enemyHand.sprite = white[2]; //시작할 때 상대방 냥이 보 상태
                enemyHead.sprite = white[3]; //시작할 때 상대방 냥이 기본 얼굴

                playerLife = 3; //플레이어 라이프 3으로 시작
                enemyLife = 3; //상대방 라이프 3으로 시작

                //gameOver.SetActive(false); //게임 종료 오브젝트 비활성화
                //gameClear.SetActive(false); //게임 클리어 오브젝트 비활성화
                //gameStart.SetActive(true); //게임 시작 오브젝트 활성화

                cheeseCat = false; //치즈냥이 확인값 false;
                blackCat = false; //깜냥이 확인값 false;

                touch = false; //터치 확인 값 false;
            }, null);

            zozo = new ZoZoBasePatton<ButtonManager>();
            zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
            #endregion
        }

        void updateLogic()
        {
            if (playerLife == 3) //플레이어 라이프가 3이라면
            {
                playerHeart[0].SetActive(true); //플레이어 하트 활성화
                playerHeart[1].SetActive(true); //플레이어 하트 활성화
                playerHeart[2].SetActive(true); //플레이어 하트 활성화
            }
            else if (playerLife == 2) //플레이어 라이프가 2라면
            {
                playerHeart[0].SetActive(true); //플레이어 하트 활성화
                playerHeart[1].SetActive(true); //플레이어 하트 활성화
                playerHeart[2].SetActive(false); //플레이어 하트 비활성화
            }
            else if (playerLife == 1) //플레이어 라이프가 1이라면
            {
                playerHeart[0].SetActive(true); //플레이어 하트 활성화
                playerHeart[1].SetActive(false); //플레이어 하트 비활성화
                playerHeart[2].SetActive(false); //플레이어 하트 비활성화
            }
            else if (playerLife <= 0) //플레이어 라이프가 0이하 라면
            {
                playerHeart[0].SetActive(false); //플레이어 하트 비활성화
                playerHeart[1].SetActive(false); //플레이어 하트 비활성화
                playerHeart[2].SetActive(false); //플레이어 하트 비활성화

                //gameOver.SetActive(true); //게임오버 오브젝트 활성화
                stateClass.resultState = GameResult.Fail;
                zozo.Change(GameState.GameResult);
            }

            if (enemyLife == 3) //에너미 라이프가 3이라면
            {
                enemyHeart[0].SetActive(true); //에너미 하트 활성화
                enemyHeart[1].SetActive(true); //에너미 하트 활성화
                enemyHeart[2].SetActive(true); //에너미 하트 활성화
            }
            else if (enemyLife == 2) //에너미 라이프가 2라면
            {
                enemyHeart[0].SetActive(true); //에너미 하트 활성화
                enemyHeart[1].SetActive(true); //에너미 하트 활성화
                enemyHeart[2].SetActive(false); //에너미 하트 비활성화
            }
            else if (enemyLife == 1) //에너미 라이프가 1이라면
            {
                enemyHeart[0].SetActive(true); //에너미 하트 활성화
                enemyHeart[1].SetActive(false); //에너미 하트 비활성화
                enemyHeart[2].SetActive(false); //에너미 하트 비활성화
            }
            else if (enemyLife <= 0) //에너미 라이프가 0이하 라면
            {
                enemyHeart[0].SetActive(false); //에너미 하트 비활성화
                enemyHeart[1].SetActive(false); //에너미 하트 비활성화
                enemyHeart[2].SetActive(false); //에너미 하트 비활성화

                StartCoroutine(("CatChange")); //고양이 체인지 함수 활성화
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (zozo != null) zozo.MGR.Excute(updateLogic);
        }

        public void Btn() //버튼을 눌렀을 때
        {
            if (!playerAnim.GetCurrentAnimatorStateInfo(0).IsName("PlayerCat")&&touch==false) //플레이어캣 애니메이션이 재생중이 아니라면
            {
                GameObject.Find("SoundManager").GetComponent<SoundManager>().BtnSound(); //버튼 사운드 재생
                playerAnim.SetTrigger("start"); //플레이어 애니메이션 실행
                enemyAnim.SetTrigger("start"); // 애니메이션 실행
                StartCoroutine(RSP()); //가위바위보 코루틴 실행
                touch = true; //터치 확인 값 true;
            }
        }
        IEnumerator RSP()
        {
            yield return new WaitForSeconds(0.7f); //0.7초 뒤에
            if (!cheeseCat && !blackCat) //치즈캣과 블랙캣이 false라면
            {
                WhiteRandom(); //가위바위보를 랜덤 이미지로 설정

                if (name == "rock") //클릭한 객체 이름이 rock 이라면
                {
                    Rock(); //플레이어 냥이를 바위 이미지로 바꿈

                    if (enemyHand.sprite.name.Contains("Scissors")) //에너미 냥이의 이미지에 가위가 포함되어 있다면
                    {
                        playerHead.sprite = player[4]; //플레이어 냥이 웃는 얼굴로 변경
                        enemyHead.sprite = white[5]; //상대방 냥이 우는 얼굴로 변경
                        enemyMinus(); //에너미 라이프 감소
                    }
                    else if (enemyHand.sprite.name.Contains("Rock")) //에너미 냥이의 이미지에 바위가 포함되어 있다면
                    {
                        playerHead.sprite = player[3]; //플레이어 냥이 기본 얼굴로 변경
                        enemyHead.sprite = white[3]; //상대방 냥이 기본 얼굴로 변경
                        GameObject.Find("SoundManager").GetComponent<SoundManager>().Tie(); //타이사운드 재생
                    }
                    else //전부 아니라면
                    {
                        playerHead.sprite = player[5]; //플레이어 냥이 우는 얼굴로 변경
                        enemyHead.sprite = white[4]; //상대방 냥이 웃는 얼굴로 변경
                        playerMinus(); //플레이어 라이프 감소
                    }
                }
                else if (name == "scissors") //클릭한 객체 이름이 scissors 라면
                {
                    Scissors(); //플레이어 냥이를 가위 이미지로 바꿈

                    if (enemyHand.sprite.name.Contains("Scissors")) //에너미 냥이의 이미지에 가위가 포함되어 있다면
                    {
                        playerHead.sprite = player[3]; //플레이어 냥이 기본 얼굴로 변경
                        enemyHead.sprite = white[3]; //상대방 냥이 기본 얼굴로 변경
                        GameObject.Find("SoundManager").GetComponent<SoundManager>().Tie(); //타이사운드 재생
                    }
                    else if (enemyHand.sprite.name.Contains("Rock")) //에너미 냥이의 이미지에 바위가 포함되어 있다면
                    {
                        playerHead.sprite = player[5]; //플레이어 냥이 우는 얼굴로 변경
                        enemyHead.sprite = white[4]; //상대방 냥이 웃는 얼굴로 변경

                        playerMinus(); //플레이어 라이프 감소
                    }
                    else //전부 아니라면
                    {
                        playerHead.sprite = player[4]; //플레이어 냥이 웃는 얼굴로 변경
                        enemyHead.sprite = white[5]; //상대방 냥이 우는 얼굴로 변경

                        enemyMinus(); //에너미 라이프 감소
                    }
                }
                else if (name == "paper") //클릭한 객체 이름이 paper 이라면
                {
                    Paper(); //플레이어 냥이를 보 이미지로 바꿈

                    if (enemyHand.sprite.name.Contains("Scissors")) //에너미 냥이의 이미지에 가위가 포함되어 있다면
                    {
                        playerHead.sprite = player[5]; //플레이어 냥이 우는 얼굴로 변경
                        enemyHead.sprite = white[4]; //상대방 냥이 웃는 얼굴로 변경
                        playerMinus(); //플레이어 라이프 감소
                    }
                    else if (enemyHand.sprite.name.Contains("Rock")) //에너미 냥이의 이미지에 바위가 포함되어 있다면
                    {
                        playerHead.sprite = player[4]; //플레이어 냥이 웃는 얼굴로 변경
                        enemyHead.sprite = white[5]; //상대방 냥이 우는 얼굴로 변경
                        enemyMinus(); //에너미 라이프 감소
                    }
                    else //전부 아니라면
                    {
                        playerHead.sprite = player[3]; //플레이어 냥이 기본 얼굴로 변경
                        enemyHead.sprite = white[3]; //상대방 냥이 기본 얼굴로 변경
                        GameObject.Find("SoundManager").GetComponent<SoundManager>().Tie(); //타이사운드 재생
                    }
                }
            }
            else if (cheeseCat) //치즈캣이 true라면
            {
                CheeseRandom(); //가위바위보를 랜덤 이미지로 설정

                if (name == "rock") //클릭한 객체 이름이 rock 이라면
                {
                    Rock(); //플레이어 냥이를 바위 이미지로 바꿈

                    if (enemyHand.sprite.name.Contains("Scissors")) //에너미 냥이의 이미지에 가위가 포함되어 있다면
                    {
                        playerHead.sprite = player[4]; //플레이어 냥이 웃는 얼굴로 변경
                        enemyHead.sprite = cheese[5]; //상대방 냥이 우는 얼굴로 변경
                        enemyMinus(); //에너미 라이프 감소
                    }
                    else if (enemyHand.sprite.name.Contains("Rock")) //에너미 냥이의 이미지에 바위가 포함되어 있다면
                    {
                        playerHead.sprite = player[3]; //플레이어 냥이 기본 얼굴로 변경
                        enemyHead.sprite = cheese[3]; //상대방 냥이 기본 얼굴로 변경
                        GameObject.Find("SoundManager").GetComponent<SoundManager>().Tie(); //타이사운드 재생
                    }
                    else //전부 아니라면
                    {
                        playerHead.sprite = player[5]; //플레이어 냥이 우는 얼굴로 변경
                        enemyHead.sprite = cheese[4]; //상대방 냥이 웃는 얼굴로 변경
                        playerMinus(); //플레이어 라이프 감소
                    }
                }
                else if (name == "scissors") //클릭한 객체 이름이 scissors 라면
                {
                    Scissors(); //플레이어 냥이를 가위 이미지로 바꿈

                    if (enemyHand.sprite.name.Contains("Scissors")) //에너미 냥이의 이미지에 가위가 포함되어 있다면
                    {
                        playerHead.sprite = player[3]; //플레이어 냥이 기본 얼굴로 변경
                        enemyHead.sprite = cheese[3]; //상대방 냥이 기본 얼굴로 변경
                        GameObject.Find("SoundManager").GetComponent<SoundManager>().Tie(); //타이사운드 재생
                    }
                    else if (enemyHand.sprite.name.Contains("Rock")) //에너미 냥이의 이미지에 바위가 포함되어 있다면
                    {
                        playerHead.sprite = player[5]; //플레이어 냥이 우는 얼굴로 변경
                        enemyHead.sprite = cheese[4]; //상대방 냥이 웃는 얼굴로 변경
                        playerMinus(); //플레이어 라이프 감소
                    }
                    else //전부 아니라면
                    {
                        playerHead.sprite = player[4]; //플레이어 냥이 웃는 얼굴로 변경
                        enemyHead.sprite = cheese[5]; //상대방 냥이 우는 얼굴로 변경
                        enemyMinus(); //에너미 라이프 감소
                    }
                }
                else if (name == "paper") //클릭한 객체 이름이 paper 이라면
                {
                    Paper(); //플레이어 냥이를 보 이미지로 바꿈

                    if (enemyHand.sprite.name.Contains("Scissors")) //에너미 냥이의 이미지에 가위가 포함되어 있다면
                    {
                        playerHead.sprite = player[5]; //플레이어 냥이 우는 얼굴로 변경
                        enemyHead.sprite = cheese[4]; //상대방 냥이 웃는 얼굴로 변경
                        playerMinus(); //플레이어 라이프 감소
                    }
                    else if (enemyHand.sprite.name.Contains("Rock")) //에너미 냥이의 이미지에 바위가 포함되어 있다면
                    {
                        playerHead.sprite = player[4]; //플레이어 냥이 웃는 얼굴로 변경
                        enemyHead.sprite = cheese[5]; //상대방 냥이 우는 얼굴로 변경
                        enemyMinus(); //에너미 라이프 감소
                    }
                    else //전부 아니라면
                    {
                        playerHead.sprite = player[3]; //플레이어 냥이 기본 얼굴로 변경
                        enemyHead.sprite = cheese[3]; //상대방 냥이 기본 얼굴로 변경
                        GameObject.Find("SoundManager").GetComponent<SoundManager>().Tie(); //타이사운드 재생
                    }
                }
            }
            else if (blackCat) //블랙캣이 true라면
            {
                BlackRandom(); //가위바위보를 랜덤 이미지로 설정

                if (name == "rock") //클릭한 객체 이름이 rock 이라면
                {
                    Rock(); //플레이어 냥이를 바위 이미지로 바꿈

                    if (enemyHand.sprite.name.Contains("Scissors")) //에너미 냥이의 이미지에 가위가 포함되어 있다면
                    {
                        playerHead.sprite = player[4]; //플레이어 냥이 웃는 얼굴로 변경
                        enemyHead.sprite = black[5]; //상대방 냥이 우는 얼굴로 변경
                        enemyMinus(); //에너미 라이프 감소
                    }
                    else if (enemyHand.sprite.name.Contains("Rock")) //에너미 냥이의 이미지에 바위가 포함되어 있다면
                    {
                        playerHead.sprite = player[3]; //플레이어 냥이 기본 얼굴로 변경
                        enemyHead.sprite = black[3]; //상대방 냥이 기본 얼굴로 변경
                        GameObject.Find("SoundManager").GetComponent<SoundManager>().Tie(); //타이사운드 재생
                    }
                    else //전부 아니라면
                    {
                        playerHead.sprite = player[5]; //플레이어 냥이 우는 얼굴로 변경
                        enemyHead.sprite = black[4]; //상대방 냥이 웃는 얼굴로 변경
                        playerMinus(); //플레이어 라이프 감소
                    }
                }
                else if (name == "scissors") //클릭한 객체 이름이 scissors 라면
                {
                    Scissors(); //플레이어 냥이를 가위 이미지로 바꿈

                    if (enemyHand.sprite.name.Contains("Scissors")) //에너미 냥이의 이미지에 가위가 포함되어 있다면
                    {
                        playerHead.sprite = player[3]; //플레이어 냥이 기본 얼굴로 변경
                        enemyHead.sprite = black[3]; //상대방 냥이 기본 얼굴로 변경
                        GameObject.Find("SoundManager").GetComponent<SoundManager>().Tie(); //타이사운드 재생
                    }
                    else if (enemyHand.sprite.name.Contains("Rock")) //에너미 냥이의 이미지에 바위가 포함되어 있다면
                    {
                        playerHead.sprite = player[5]; //플레이어 냥이 우는 얼굴로 변경
                        enemyHead.sprite = black[4]; //상대방 냥이 웃는 얼굴로 변경
                        playerMinus(); //플레이어 라이프 감소
                    }
                    else //전부 아니라면
                    {
                        playerHead.sprite = player[4]; //플레이어 냥이 웃는 얼굴로 변경
                        enemyHead.sprite = black[5]; //상대방 냥이 우는 얼굴로 변경
                        enemyMinus(); //에너미 라이프 감소
                    }
                }
                else if (name == "paper") //클릭한 객체 이름이 paper 이라면
                {
                    Paper(); //플레이어 냥이를 보 이미지로 바꿈

                    if (enemyHand.sprite.name.Contains("Scissors")) //에너미 냥이의 이미지에 가위가 포함되어 있다면
                    {
                        playerHead.sprite = player[5]; //플레이어 냥이 우는 얼굴로 변경
                        enemyHead.sprite = black[4]; //상대방 냥이 웃는 얼굴로 변경
                        playerMinus(); //플레이어 라이프 감소
                    }
                    else if (enemyHand.sprite.name.Contains("Rock")) //에너미 냥이의 이미지에 바위가 포함되어 있다면
                    {
                        playerHead.sprite = player[4]; //플레이어 냥이 웃는 얼굴로 변경
                        enemyHead.sprite = black[5]; //상대방 냥이 우는 얼굴로 변경
                        enemyMinus(); //에너미 라이프 감소
                    }
                    else //전부 아니라면
                    {
                        playerHead.sprite = player[3]; //플레이어 냥이 기본 얼굴로 변경
                        enemyHead.sprite = black[3]; //상대방 냥이 기본 얼굴로 변경
                        GameObject.Find("SoundManager").GetComponent<SoundManager>().Tie(); //타이사운드 재생
                    }
                }
            }   
        }
        IEnumerator CatChange()
        {
            if ((enemyHead.sprite.name.Contains("white")) && enemyLife == 0) //상대방 냥이가 흰냥이고 에너미 라이프가 0이라면
            {
                GameObject.Find("SoundManager").GetComponent<SoundManager>().roundSound(); //라운드사운드 재생
                yield return new WaitForSeconds(0.8f); //0.8초 뒤에 

                if (playerLife < 3)
                    playerLife += 1; //플레이어 목숨하나 추가

                enemyLife = 3; //에너미 라이프를 3으로

                enemyHead.sprite = cheese[3]; //치즈냥이로 바꿈
                enemyHand.sprite = cheese[2]; //치즈냥이 손으로 바꿈

                cheeseCat = true; //치즈냥이 확인값 true
            }
            else if ((enemyHead.sprite.name.Contains("Cheese")) && enemyLife == 0) //상대방 냥이가 치즈냥이고 에너미라이프가 0이라면
            {
                GameObject.Find("SoundManager").GetComponent<SoundManager>().roundSound(); //라운드사운드 재생
                yield return new WaitForSeconds(0.8f); //0.8초 뒤에

                if (playerLife < 3)
                    playerLife += 1; //플레이어 목숨하나 추가

                enemyLife = 3; //에너미 라이프를 3으로

                enemyHead.sprite = black[3]; //깜냥이로 바꿈
                enemyHand.sprite = black[2]; //깜냥이 손으로 바꿈

                cheeseCat = false; //치즈냥이 확인값 false
                blackCat = true; //깜냥이 확인값 true
            }
            else if ((enemyHead.sprite.name.Contains("Black")) && enemyLife == 0) //상대방 냥이가 깜냥이고 에너미라이프가 0이라면
            {
                stateClass.resultState = GameResult.Success;
                zozo.Change(GameState.GameResult);
                //gameClear.SetActive(true); //게임종료 오브젝트 true
            }

        }
        public void GameStart() //스타트 함수
        {
            gameStart.SetActive(false); //게임시작 오브젝트 비활성화
        }
        public void enemyMinus() //에너미 라이프 감소 함수
        {
            enemyLife--; //에너피 라이프 감소
            GameObject.Find("SoundManager").GetComponent<SoundManager>().CatSound(); //캣사운드 재생
        }
        public void playerMinus() //플레이어 라이프 감소 함수
        {
            playerLife--; //플레이어 라이프 감소
            GameObject.Find("SoundManager").GetComponent<SoundManager>().SadSound(); //새드사운드 재생
        }

        public void Rock() //플레이어 바위로 이미지 변경
        {
            playerHand.sprite = player[0]; //플레이어 냥이를 바위 이미지로 바꿈
        }
        public void Scissors() //플레이어 가위로 이미지 변경
        {
            playerHand.sprite = player[1]; //플레이어 냥이를 가위 이미지로 바꿈
        }
        public void Paper() //플레이어 보로 이미지 변경
        {
            playerHand.sprite = player[2]; //플레이어 냥이를 가위 이미지로 바꿈
        }
        public void WhiteRandom() //상대방 냥이 랜덤 변경
        {
            enemyHand.sprite = white[Random.Range(0, 3)]; //흰냥이 랜덤 변경
        }
        public void CheeseRandom() //상대방 냥이 랜덤 변경
        {
            enemyHand.sprite = cheese[Random.Range(0, 3)]; //치즈냥이 랜덤 변경
        }
        public void BlackRandom() //상대방 냥이 랜덤 변경
        {
            enemyHand.sprite = black[Random.Range(0, 3)]; //깜냥이 랜덤 변경
        }
    }
}
