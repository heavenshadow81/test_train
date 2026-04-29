using Bax.P0.Client.UnityWorld.BalloonGame;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Bax.P0.Client.UnityWorld.MonkeyGame
{
    public class MonkeyMgr : MonoBehaviour, Game.IMyGameActions
    {
        public static MonkeyMgr Instance;

        public EnumClass stateClass;
        public GameUI gameUI;
        public ScreenProsess screenProsess;

        //이동할 가지 의 Transform
        public Transform[] TreeBranchs;
        //원숭이
        public Monkey[] monkeys;
        //발판
        public FootPlace[] foots;

        public int banana = 0;

        public int reGameCnt = 0;


        public CancellationTokenSource token = new();


        public GameObject thief_BackGround;
        public GameObject thief;

        //NewInputSystem InpuAction
        public Game inputGame;

        //도둑찾기
        public async UniTask ThiefActive(Vector2 pos)
        {
            //스프라이트마스크 위치 세팅
            thief.transform.position = pos;
            //어두운화면 On
            //thief_BackGround.SetActive(true);
            //정지
            Time.timeScale = 0;
            //3초 뒤
            await UniTask.Delay(TimeSpan.FromSeconds(3), DelayType.UnscaledDeltaTime);
            //어두운화면 off
            //thief_BackGround.SetActive(false);
            Time.timeScale = 1;
        }

        private void OnEnable()
        {
            if (token != null) token.Dispose();
            token = new();

            //InputAction 세팅
            inputGame = new();
            inputGame.Enable();
            inputGame.MyGame.SetCallbacks(this);

            EnhancedTouchSupport.Enable();
            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += touch_onFingerDown;
        }

        private void touch_onFingerDown(Finger finger)
        {
            if (isDown) return;

            downProcess(finger.currentTouch.screenPosition);
        }

        private void OnDisable()
        {
            token.Cancel();
            ReadyProcess.sourceCancle?.Invoke();

            inputGame.Disable();

            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= touch_onFingerDown;
            EnhancedTouchSupport.Disable();
        }

        private void OnDestroy()
        {
            token.Cancel();
            token.Dispose();
            ReadyProcess.sourceCancle?.Invoke();
            ReadyProcess.sourceDispone?.Invoke();
        }
        public enum MonkeyMoveState
        { 
            Await , move , Result
        }

        public MonkeyMoveState monkeyMoveState = MonkeyMoveState.Await;

        public bool isDown;

        public ZoZoBasePatton<MonkeyMgr> zozo;
        private void Awake()
        {
            Instance = this;
            stateClass = new EnumClass();

            //바나나 랜덤값 0~3
            banana = UnityEngine.Random.Range(0, monkeys.Length);

            #region 공용 스테이트 패턴 

            ActionProcess.Enter_StateListener(
                () => { MonkeyBranchNumberSetting(); }, 
                null, 
                () => 
                {
                    monkeyMoveState = MonkeyMoveState.Await;
                    play().Forget();

                }, null);

            zozo = new ZoZoBasePatton<MonkeyMgr>();
            zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));

            #endregion
        }

        private async UniTask play()
        {
           int rand = banana;

           Monkey monkey = monkeys[rand];
           await UniTask.Yield();

           if (reGameCnt == 0)
           {
                //4중 한명의 원숭이가 바나나를 숨기고 
               monkey.anim.SetTrigger("hiding");
           }
           else
           {
               monkey.anim.SetTrigger("fail");
           }
        }

        //가지 번호를 저장
        public List<int> branchsNumberList = new List<int>();
        public async void MonkeyBranchNumberSetting()
        {
            monkeys[0].branchTemp.Add(0);
            monkeys[1].branchTemp.Add(2);
            monkeys[2].branchTemp.Add(4);
            monkeys[3].branchTemp.Add(5);

            int[] monkeyNum = new int[4];
            //7번 섞음
            for (int x = 0; x < 7; x++)
            {
                branchsNumberList.Clear();
                //나무의 가지 6개 가지의 번호 저장
                for (int i = 0; i < 6; i++) branchsNumberList.Add(i);
                //섞음
                shuffle(branchsNumberList).Forget();
                await UniTask.Yield();

                //원숭이 4마리 
                for (int i = 0; i < monkeys.Length; i++)
                {
                    await UniTask.Yield();

                    //2번째 돌때 부터 
                    if (x != 0)
                    {
                        //현재 가지의 번호가 저장되있음 monkeyNum
                        //현재 가지의 번호와 다음에 나올 가지의 번호가 같으면 안됨 
                        //달라질때까지 섞음
                        while (monkeyNum[i] == branchsNumberList[0])
                        { 
                            shuffle(branchsNumberList).Forget();
                        }
                        //섞기 완료됬다면 첫번째 가지 번호 저장
                        monkeyNum[i] = branchsNumberList[0];
                    }
                    else
                    {
                        //원숭이가 첫번째 돌았을때 가지번호를 차례대로 저장
                        monkeyNum[i] = branchsNumberList[0];        //0245
                    }
                    //첫번째 원숭이가 가야할 다음 가지번호 저장
                    monkeys[i].branchTemp.Add(branchsNumberList[0]);
                    //가지번호 0번 제거
                    branchsNumberList.RemoveAt(0);
                }
            }

            List<int> lastPosNumber = new List<int>();
            lastPosNumber.AddRange( new int[4] { 0, 2, 4, 5 } );            //도착해야될 가지번호 저장
            List<Monkey> tempMonkeyList = new List<Monkey>();

            for (int i = 0; i < monkeys.Length; i++)
            {
                Monkey monkey = monkeys[i];
                //원숭이의 마지막 가지번호가 0 2 4 5 중 하나라도 포함된다면 해당 번호 제거
                if (lastPosNumber.Contains(monkey.branchTemp[monkey.branchTemp.Count - 1]))
                {
                    lastPosNumber.Remove(monkey.branchTemp[monkey.branchTemp.Count - 1]);
                }
                else
                { 
                    //해당되자않는다면 원숭이리스트 저장
                    tempMonkeyList.Add(monkey);
                }
            }

            //원숭이가 하나라도 저장이 됬다면 
            if(tempMonkeyList.Count > 0) 
            {
                for (int i = 0; i < tempMonkeyList.Count; i++)
                {
                    //원숭이가 마지막으로 가야될 가지번호를 저장 후 남은 마지막 번호로 저장
                    tempMonkeyList[i].branchTemp[tempMonkeyList[i].branchTemp.Count-1] = lastPosNumber[i];
                }
            }

            //처음에 저장한 0 2 4 5 를 제거 
            for (int i = 0; i < monkeys.Length; i++)
            {
                  monkeys[i].branchTemp.RemoveAt(0);
            }
        }

        //섞기
        private async UniTask shuffle(List <int> list)
        {
            for (int i = 0; i < 10; i++)
            {
                int rest = UnityEngine.Random.Range(0, list.Count);
                int dest = UnityEngine.Random.Range(0, list.Count);
                int temp = list[rest];
                list[rest] = list[dest];
                list[dest] = temp;
            }
            await UniTask.Yield();
        }

       

        private void Update()
        {
            if (zozo != null) zozo.MGR.Excute(null); //상태가 있다면 상태의 Excute 주기적으로 실행 시킴 (Update)
        }

        //Pc용 NewInputSystem Down 1   Up 0
        public void OnDown(InputAction.CallbackContext context)
        {
            if (Settings.instance.mouseToggle.isOn == false) return;
            if (isDown) return;
            if (context.ReadValue<float>() == 1f)
            {
                downProcess(Settings.instance.MousePos());
            }

        }

        //touchScreen 용 NewInputSystem
        public void OnTouch(InputAction.CallbackContext context)
        {
            //if (isDown) return;

            //downProcess(context.ReadValue<Vector2>());
        }


        private void downProcess(Vector2 fingerPos)
        {
            //파라메타로 전달받은 위치값으로 레이케스트
            var hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(fingerPos), Vector2.zero);
            if (hit2D)
            {
                //터치한 위치에 있는 컬라이더가 발판이라면
                if (hit2D.collider.TryGetComponent<FootPlace>(out var foot))
                {
                    foot.Down();
                }
            }
        }

        public void OnIsDown(InputAction.CallbackContext context) { }
    }
}