using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Settings;

namespace ShipRun
{
    public class PlayerMove : MonoBehaviour
    {
        public GameObject start; //게임 시작 오브젝트
        public GameObject end; //게임 종료 오브젝트

        public Image[] Btn = new Image[2]; //버튼 UI 이미지
        public Sprite[] BtnOn = new Sprite[2]; //변경할 버튼 이미지
        Sprite[] BtnOff = new Sprite[2]; //다시 변경할 버튼 이미지 저장
        int btn; //버튼 활성화 확인 변수

        Vector3 position; //현재 플레이어 위치

        public ZoZoBasePatton<PlayerMove> zozo;
        public EnumClass stateClass;
        public GameUI gameUI;
        public ScreenProsess screenProsess;

        public EnemyMove[] enemys;


        private void Awake()
        {
            stateClass = new EnumClass();
            #region 공용 스테이트 패턴 

            ActionProcess.Enter_StateListener(Init, null, null, null);

            zozo = new ZoZoBasePatton<PlayerMove>();
            zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
            #endregion
        }
        void Init()
        {
            //start.SetActive(true); //게임 시작 오브젝트 활성화
            //end.SetActive(false); //게임 종료 오브젝트 비활성화

            btn = 0; //시작할 때 버튼 모두 활성화
            position = transform.position; //현재 위치 값 저장

            BtnOff[0] = Btn[0].GetComponent<Image>().sprite; //버튼 오프 0번에 현재 이미지 저장
            BtnOff[1] = Btn[1].GetComponent<Image>().sprite; //버튼 오프 1번에 현재 이미지 저장
        }

        // Update is called once per frame
        void Update()
        {
            if (zozo != null) zozo.MGR.Excute(
                () => 
                {
                    gameObject.transform.position = position; //플레이어 포지션을 포지션 변수로 이동

                    foreach (var item in enemys)
                    {
                        item.UpdateLogic();
                    }
                });
        }

        private void OnTriggerEnter(Collider other) //other에 물체가 닿았을 때
        {
            if(other.tag=="Ground") //태그 이름이 Ground 라면
            {
                //end.SetActive(true); //종료 화면 활성화
                stateClass.resultState = GameResult.Success;
                zozo.Change(GameState.GameResult);

               // Time.timeScale = 0; //타임 스케일 0으로
            }
        }

        public void RBtn() //오른쪽 버튼 함수
        {
            if (btn == 2 || btn == 0) //버튼 값이 2거나 0이라면
            {
                position.x -= 0.15f; //포지션 x값을 0.15 만큼 앞으로 이동
                btn = 1; //버튼 값을 1로 바꿈
                StartCoroutine(RBtnOn()); //R버튼 활성화 코루틴 실행
            }
        }
        public void LBtn() //왼쪽 버튼 함수
        {
            if (btn == 1 || btn == 0) //버튼 값이 1이거나 0이라면
            {
                position.x -= 0.15f; //포지션 x값을 0.15 만큼 앞으로 이동
                btn = 2; //버튼 값을 2로 바꿈
                StartCoroutine(LBtnOn()); //L버튼 활성화 코루틴 실행
            }
        }
        public void gameStart() //게임 시작버튼 함수
        {
            Time.timeScale = 1; //타임스케일을 1로
            start.SetActive(false); //게임 시작화면 비활성화
        }
        IEnumerator RBtnOn() //R버튼 활성화 코루틴
        {
            Btn[0].sprite = BtnOn[0]; //버튼 0번을 On으로 바꿈
            yield return new WaitForSeconds(0.3f); //0.3초 대기
            Btn[0].sprite = BtnOff[0]; //버튼 1번을 Off로 바꿈
        }
        IEnumerator LBtnOn() //L버튼 활성화 코루틴
        {
            Btn[1].sprite = BtnOn[1]; //버튼 0번을 On으로 바꿈
            yield return new WaitForSeconds(0.3f); //0.3초 대기
            Btn[1].sprite = BtnOff[1]; //버튼 1번을 Off로 바꿈
        }
    }
}
