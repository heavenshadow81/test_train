using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using static Settings;
using Random = UnityEngine.Random;
using DG.Tweening;
using Unity.VisualScripting;
using AnimalCard;

namespace Penguin
{
    [System.Serializable]
    public class Horizon //열 클래스
    {
        public GameObject[] horizon; //열 배열
    }

    public class GameManager : MonoBehaviour
    {
        public Horizon[] vertical; //행 배열
        bool all; //전부 false인지 체크값
        bool multi; //멀티터치 방지

        public GameObject Fin; //게임종료 오브젝트
        public GameObject Start; //시작화면 오브젝트
        public GameObject FinSound; //게임종료 사운드 오브젝트

        public Button[] Btn; //버튼 오브젝트 배열
        public Sprite[] img; //버튼 활성화 이미지 오브젝트 배열
        Sprite[] BtnImg = new Sprite[4]; //버튼 비활성화 이미지 배열

        public GameObject[] map = new GameObject[10]; //게임맵 오브젝트 배열

        public static bool gameStart; //게임스타트 값
        public static int score; //게임 스코어 

        List<int> radomList = new List<int>(); //랜덤 숫자 리스트


        public EnumClass stateClass;
        public GameUI gameUI;
        public ScreenProsess screenProsess;
        public ZoZoBasePatton<GameManager> zozo;

        private void Awake()
        {
            stateClass = new EnumClass();
            #region 공용 스테이트 패턴 

            ActionProcess.Enter_StateListener(Init, null, null, null);

            zozo = new ZoZoBasePatton<GameManager>();
            zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
            #endregion
        }

        void RandomNumber() //랜덤숫자 리스트 생성 함수
        {
            int Number = Random.Range(0, 10); //넘버는 0~9사이 숫자

            for (int i = 0; i < 10; i++)
            {
                if (radomList.Contains(Number)) //넘버가 랜덤리스트 안에 있다면
                {
                    Number = Random.Range(0, 10); //넘버를 0~9사이 숫자로 변경
                }
                else //아니라면
                {
                    radomList.Add(Number); //넘버를 랜덤리스트에 추가
                }
            }
        }


        public void Init()
        {
            RandomNumber(); //랜덤숫자 리스트 함수 실행

            // FinSound.SetActive(false); //게임종료 사운드 오브젝트 비활성화
            // Fin.SetActive(false); //게임종료 오브젝트 비활성화
            score = 0; //스코어는 0

            map[radomList[0]].SetActive(true); //맵 배열중 랜덤리스트 0번째에 있는 맵 활성화
            //map[0].SetActive(true); //맵 테스트

            gameStart = true; //게임스타트 true

            for (int i = 0; i < Btn.Length; i++) //버튼 수만큼 반복
            {
                BtnImg[i] = Btn[i].GetComponent<Image>().sprite; //버튼 비활성화 이미지 컴포넌트
            }


            all = false; //시작할 때 all은 비활성화
            multi = false; //시작할 때 멀티 비활성화
        }


       
        // Update is called once per frame
        void Update()
        {
            if (zozo != null) zozo.MGR.Excute(() =>
            {
                if (!gameStart && score < 3) //게임스타트가 false고 스코어가 3보다 작다면 
                {
                    Btn[0].image.sprite = BtnImg[0]; //0번 버튼 이미지를 비활성화 이미지로
                    Btn[1].image.sprite = BtnImg[1]; //1번 버튼 이미지를 비활성화 이미지로
                    Btn[2].image.sprite = BtnImg[2]; //2번 버튼 이미지를 비활성화 이미지로
                    Btn[3].image.sprite = BtnImg[3]; //3번 버튼 이미지를 비활성화 이미지로

                    /*if (map[radomList[0]] != null) //맵 배열중 랜덤리스트 0번째에 맵이 있다면
                    {
                        Destroy(map[radomList[0]]); //맵 배열 0번 삭제
                        map[radomList[1]].SetActive(true); //맵 배열 1번 활성화
                        Physics.gravity = new Vector3(-10, 0, 0); //중력을 아래 방향으로
                        GameObject.Find("SoundManager").GetComponent<FeedSound>().CollectSound(); //콜렉트 사운드 재생
                    }
                    else if (map[radomList[0]] == null) //맵 배열 0번이 비어있다면
                    {
                        Destroy(map[radomList[1]]); //맵 배열 1번 삭제
                        map[radomList[2]].SetActive(true); //맵 배열 2번 활성화
                        Physics.gravity = new Vector3(-10, 0, 0); //중력을 아래 방향으로
                        GameObject.Find("SoundManager").GetComponent<FeedSound>().CollectSound(); //콜렉트 사운드 재생
                    }
                    gameStart = true; //게임 스타트값을 true로
                    */
                }
                else if (score >= 3) //스코어가 3이상 이라면
                {
                    // Fin.SetActive(true); //게임종료 오브젝트 활성화
                    // FinSound.SetActive(true); //게임종료 사운드 활성화

                    stateClass.resultState = GameResult.Success;
                    zozo.Change(GameState.GameResult);

                    PenguinMove.position.DOMove(vertical[6].horizon[2].transform.position, 0.1f);
                }
            });


            
        }
        public void LeftBtn() //왼쪽 버튼 함수
        {
            if (!multi) //멀티가 비활성화라면
            {
                GameObject.Find("SoundManager").GetComponent<FeedSound>().BtnSound(); //버튼 사운드 재생
                GameObject.Find("Player").GetComponent<PenguinMove>().Left(); //펭귄 왼쪽방향 애니메이션 재생

                Btn[0].image.sprite = img[0]; //버튼 0번을 활성화 이미지로 변경
                Btn[1].image.sprite = BtnImg[1]; //버튼 1번을 비활성화 이미지로 변경
                Btn[2].image.sprite = BtnImg[2]; //버튼 2번을 비활성화 이미지로 변경
                Btn[3].image.sprite = BtnImg[3]; //버튼 3번을 비활성화 이미지로 변경

            
                multi = true; //멀티 활성화
                for (int i = 4; i >= 0; i--) //5번 반복
                {
                    all = true; //all 활성화
                    if (vertical[PenguinMove.instance.x].horizon[i].GetComponent<MoveCheck>().check == true) //현재있는 행에서 세로 배열중에 check된 물체가 있다면
                    {
                        if (i < PenguinMove.instance.y) //i가 y보다 크다면
                        {
                            all = false; //all 비활성화
                            PenguinMove.instance.y = i + 1; //i행에서 한칸 오른쪽으로 이동
                            
                            break; //반복 종료
                        }
                    }
                }
                if (all) //all이 활성화라면
                    PenguinMove.instance.y = 0; //0번 열로 이동

                //현재 위치에서 지정한 위치로 1.5초동안 이동 한 후 all 체크값과 multi체크 값 비활성화
                PenguinMove.position.DOMove(vertical[PenguinMove.instance.x].horizon[PenguinMove.instance.y].transform.position, 1.5f).OnComplete(() => { all = false; multi = false; });
            }
        }
        public void RightBtn() //오른쪽 버튼 함수
        {
            if (!multi) //멀티가 비활성화라면
            {
                GameObject.Find("SoundManager").GetComponent<FeedSound>().BtnSound(); //버튼 사운드 재생
                GameObject.Find("Player").GetComponent<PenguinMove>().Right(); //펭귄 오른쪽 방향 애니메이션 재생
 
                Btn[3].image.sprite = img[3]; //버튼 3번을 활성화 이미지로 변경
                Btn[0].image.sprite = BtnImg[0]; //버튼 0번을 비활성화 이미지로 변경
                Btn[1].image.sprite = BtnImg[1]; //버튼 1번을 비활성화 이미지로 변경
                Btn[2].image.sprite = BtnImg[2]; //버튼 2번을 비활성화 이미지로 변경

            
                multi = true; //멀티 활성화
                for (int i = 0; i < 5; i++) //5번 반복
                {
                    all = true; //all 활성화
                    if (vertical[PenguinMove.instance.x].horizon[i].GetComponent<MoveCheck>().check == true) //현재있는 행에서 세로 배열중에 check된 물체가 있다면
                    {
                        if (i > PenguinMove.instance.y) //i가 y보다 크다면
                        {
                            all = false; //all 비활성화
                            PenguinMove.instance.y = i - 1; //i행에서 한칸 왼쪽으로 이동
                            break; //반복 종료
                        }
                    }
                }
                if (all) //all이 활성화라면
                    PenguinMove.instance.y = 4; //4번 열로 이동

                //현재 위치에서 지정한 위치로 1초동안 이동 한 후 all 체크값과 multi체크 값 비활성화
                PenguinMove.position.DOMove(vertical[PenguinMove.instance.x].horizon[PenguinMove.instance.y].transform.position, 1f).OnComplete(() => { all = false; multi = false; });
            }
        }
        public void UpBtn()
        {
            if (!multi) //멀티가 비활성화라면
            {
                GameObject.Find("SoundManager").GetComponent<FeedSound>().BtnSound(); //버튼 사운드 재생
                GameObject.Find("Player").GetComponent<PenguinMove>().Up(); //펭귄 위 방향 애니메이션 재생

                Btn[2].image.sprite = img[2]; //버튼 2번을 활성화 이미지로 변경
                Btn[0].image.sprite = BtnImg[0]; //버튼 0번을 비활성화 이미지로 변경
                Btn[1].image.sprite = BtnImg[1]; //버튼 1번을 비활성화 이미지로 변경
                Btn[3].image.sprite = BtnImg[3]; //버튼 3번을 비활성화 이미지로 변경

                for (int i = 6; i >= 0; i--) //7번 반복
                {
                    multi = true; //멀티 활성화
                    if (vertical[i].horizon[PenguinMove.instance.y].GetComponent<MoveCheck>().check == true) //현재있는 행에서 세로 배열중에 check된 물체가 있다면
                    {
                        all = true; //all 활성화
                        if (i < PenguinMove.instance.x) //i가 x보다 작다면
                        {
                            all = false; //all 비활성화
                            PenguinMove.instance.x = i + 1; //i행에서 한칸 아래로 이동
                            break; //반복 종료
                        }
                    }
                }
                if (all) //all이 활성화 라면
                    PenguinMove.instance.x = 0; //0번 행으로 이동

                //현재 위치에서 지정한 위치로 1초동안 이동 한 후 all 체크값과 multi체크 값 비활성화
                PenguinMove.position.DOMove(vertical[PenguinMove.instance.x].horizon[PenguinMove.instance.y].transform.position, 1f).OnComplete(() => { all = false; multi = false; });
            }
        }
        public void DownBtn()
        {
            if (!multi) //멀티가 비활성화라면
            {
                GameObject.Find("SoundManager").GetComponent<FeedSound>().BtnSound(); //버튼 사운드 재생
                GameObject.Find("Player").GetComponent<PenguinMove>().Down(); //펭귄 아래 방향 애니메이션 재생

                Btn[1].image.sprite = img[1]; //버튼 1번을 활성화 이미지로 변경
                Btn[0].image.sprite = BtnImg[0]; //버튼 0번을 비활성화 이미지로 변경
                Btn[2].image.sprite = BtnImg[2]; //버튼 2번을 비활성화 이미지로 변경
                Btn[3].image.sprite = BtnImg[3]; //버튼 3번을 비활성화 이미지로 변경

                for (int i = 0; i < 7; i++) //7번 반복
                {
                    multi = true; //멀티 활성화
                    if (vertical[i].horizon[PenguinMove.instance.y].GetComponent<MoveCheck>().check == true) //현재있는 행에서 세로 배열중에 check된 물체가 있다면
                    {
                        all = true; //all 활성화
                        if (i > PenguinMove.instance.x) //i가 x보다 크다면
                        {
                            all = false; //all 비활성화
                            PenguinMove.instance.x = i - 1; //i행에서 한칸 위로 이동
                            break; //반복 종료
                        }
                    }
                }
                if (all) //all이 활성화라면
                    PenguinMove.instance.x = 6; //6번 행으로 이동

                //현재 위치에서 지정한 위치로 1초동안 이동 한 후 all 체크값과 multi체크 값 비활성화
                PenguinMove.position.DOMove(vertical[PenguinMove.instance.x].horizon[PenguinMove.instance.y].transform.position, 1f).OnComplete(() => { all = false; multi = false; });
            }
        }
        public void HomeBtn() //홈버튼 함수
        {
            SceneManager.LoadSceneAsync(0); //0번 씬으로 이동
        }
        public void StartBtn() //스타트 버튼 함수
        {
            Time.timeScale = 1; //시간이 흐르게 변경
            Start.SetActive(false); //시작화면 오브젝트 비활성화
        }

    }
}
