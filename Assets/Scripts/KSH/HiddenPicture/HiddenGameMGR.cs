using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using static Bax.P0.Client.UnityWorld.PictureGame.HiddenGameMGR;

namespace Bax.P0.Client.UnityWorld.PictureGame
{
    public class HiddenGameMGR : MonoBehaviour, Game.IMyGameActions
    {
        //x y 값 
        public struct ArrnumXY { public float x; public float y; }

        public static HiddenGameMGR Instance;

        public Camera Cam;

        FindStuff stuff;
        public EnumClass stateClass;
        public GameUI gameUI;
        public ScreenProsess screenProsess;
        //배경 이미지 랜더러
        public SpriteRenderer BG;
        //로딩이미지 랜더러
        public SpriteRenderer fadeSpRender;
        public Transform pullingParent;

        
        //이미지 로드
        public LoadSprite loadSprite;
        
        public PullingMGR<Highlight> pulling;
        public Highlight pullingPrefabs;
        public List<Highlight> highlights = new List<Highlight>();


        private int hiddenstage;

        //스테이지 번호
        public int HiddenStage
        { 
            get => hiddenstage;
            set
            { 
                if (hiddenstage >= 4) hiddenstage = 4;
                else hiddenstage = value;
            }
        }
        //최대 스테이지 번호
        public int hiddenMaxStage;

        //체크 번호
        public int checkIdx;
        //최대 체크 번호
        public int MaxCheckIdx;
        //게임 클리어 여부
        public bool GameClear = false;

        //동물 이름 저장용
        public List<string> animalNames = new List<string>();
        //터치해야하는 게임오브젝트 
        public HiddenFindData[] L;
        //터치할 필요없는 빈 이미지 랜더러
        public SpriteRenderer[] AnimalNoColl;
        //우측 찾아야하는 이미지 게임오브젝트
        public List<FinderStuff> R = new List<FinderStuff>();

        public Game GameInput;

        public ZoZoBasePatton<HiddenGameMGR> zozo;

        public TextMeshProUGUI textText;

        public enum LoadingState
        {
            Game, Loading
        }
        public LoadingState loadingState = LoadingState.Game;

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += onFingerDown;

            GameInput = new();
            GameInput.Enable();
            GameInput.MyGame.SetCallbacks(this);
            
        }
        private void OnDisable()
        {
            ReadyProcess.sourceCancle?.Invoke();
            GameInput.Disable();
            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= onFingerDown;
            EnhancedTouchSupport.Disable();
        }

        private void OnDestroy()
        {
            ReadyProcess.sourceCancle?.Invoke();
            ReadyProcess.sourceDispone?.Invoke();
            GameInput.Disable();
        }
        private void onFingerDown(Finger obj)
        {
            if (stateClass.state == GameState.GamePlay)
            {
                DownOn(obj.currentTouch.screenPosition);
            }
        }
        public void OnDown(InputAction.CallbackContext context)
        {
            if (stateClass.state == GameState.GamePlay)
            {
                if (Settings.instance.mouseToggle.isOn == false) return;

                DownOn(Settings.instance.MousePos());
            }
        }

        public void OnTouch(InputAction.CallbackContext context) 
        {
            //var v = context.ReadValue<Vector2>();
            //var hit2D = Physics2D.Raycast(Cam.ScreenToWorldPoint(v), Vector2.zero);

            ////터치한 위치에 찾아야할 데이타가 있다면
            //if (hit2D.collider.TryGetComponent<HiddenFindData>(out var ob))
            //{
            //    ob.DownProcess();
            //}
        }

        public void OnIsDown(InputAction.CallbackContext context) { }


        public void DownOn(Vector2 pos)
        {
            if (Cam == null) return;
            //터치한 위치에 레이캐스트
            var hit2D = Physics2D.Raycast(Cam.ScreenToWorldPoint(pos), Vector2.zero);
            if (hit2D)
            {
                //터치한 위치에 찾아야할 데이타가 있다면
                if (hit2D.collider.TryGetComponent<HiddenFindData>(out var ob))
                {
                    ob.DownProcess();
                }
            }
        }

        private void Awake()
        {
            Instance = this;
            stateClass = new EnumClass();
            loadSprite = new LoadSprite("HiddenPicture");               //로드 할 어드레서블 객체 생성(폴더명)
            pulling = new PullingMGR<Highlight>(pullingPrefabs, 5);     //맞췄을때 똥그라미
                                                                        //stateMachine.StateChange(GameState.GameIntro);

            #region 공용 스테이트 패턴 

            ActionProcess.Enter_StateListener(() =>
            {
                BgImageSet(0).Forget();
                LRSpriteSetting().Forget();
               // Cam.targetDisplay = 1;
            }, null, null, null);

            zozo = new ZoZoBasePatton<HiddenGameMGR>();
            zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
            #endregion
        }

        public async UniTask LRSpriteSetting()
        {
            //터치 가능 한 게임오브젝트 5개 컬라이더 활성화 
            for (int i = 0; i < L.Length; i++) L[i].boxCollider2D.enabled = true;

            //왼쪽 생성될 이미지들 최대한 똑같이 겹치지 않도록  (이미지들이 가로 세로 전부 다양해서 일부 겹칠수있음)
            //x 위치 저장용
            List<float> arrNumbersX = new List<float>();
            //y 위치 저장용
            List<float> arrNumbersY = new List<float>();
            //이차원리스트 
            List<List<ArrnumXY>> arrnumXies = new List<List<ArrnumXY>>();
            //동물이미지파일의 이름이 들어갈 리스트 비움
            animalNames.Clear();
            //열거형에 저장되있는 파일명 string 으로 전부 저장
            var names = Enum.GetNames(typeof(FindStuff));
            // 동물 이름 리스트 저장
            animalNames.AddRange(names);

            //동물 이름 섞기
            for (int i = 0; i < 10; i++)
            {
                int r = UnityEngine.Random.Range(0, animalNames.Count);
                int d = UnityEngine.Random.Range(0, animalNames.Count);

                var temp = animalNames[r];
                animalNames[r] = animalNames[d];
                animalNames[d] = temp;
            }

            //왼쪽 부터 우측까지 
            //-3.5 ~ 4.0 
            //증가값 저장
            for (float i = -3.5f; i <= 4f; i += 0.8f)
            { 
                arrNumbersX.Add(i);
            }
            //위에서 아래까지 
            //-4 ~ 4
            //증가값 저장
            for (float i = -4f; i <= 4f; i += 0.5f)
            { 
                arrNumbersY.Add(i);
            }

            //2차원 리스트 저장 
            for (int i = 0; i < arrNumbersX.Count; i++)
            {
                List<ArrnumXY> tempList = new List<ArrnumXY>();

                for (int j = 0; j < arrNumbersY.Count; j++)
                {
                    ArrnumXY xY = new ArrnumXY();
                    xY.x = arrNumbersX[i];
                    xY.y = arrNumbersY[j];
                    tempList.Add(xY);
                }
                arrnumXies.Add(tempList);
            }

            //우측 이미지 로드
            await rLoaing();
            //좌측 이미지 로드
            await lLoding(arrnumXies);


            //동물 이름 랜덤으로 섞고 , 0~ 4 번 찾아야될 동물 이미지도 로드 완료 

            //찾아야될 동물 이름 제거 
            for (int i = 0; i < L.Length; i++) animalNames.RemoveAt(0);

            //나머지 이미지 로드 준비 
            for (int i = 0; i < AnimalNoColl.Length; i++)
            {
                Vector2 pos = Vector2.zero;
                //위치 계산
                PosCur(ref pos , arrnumXies);
                //최대값 넘어가도 다시 0번부터 시작
                string name = (animalNames[i % animalNames.Count]);
                
                //이미지 로드
                await loadSprite.LoadSpriteData($"{name}", AnimalNoColl[i]);
                //위치 세팅
                AnimalNoColl[i].transform.localPosition = pos;
            }

            //이미지 크기에 맞게 컬라이더 크기 세팅
            for(int i=0;i<L.Length;i++) 
            {
                L[i].CollSizeSet();
            }


        }

        //랜덤위치 뽑아내고 , 해당 위치 가지고있는 idx 제거
        public void PosCur(ref Vector2 pos , List<List<ArrnumXY>> arrnumXies)
        {
            int x = UnityEngine.Random.Range(0, arrnumXies.Count);
            int y = UnityEngine.Random.Range(0, arrnumXies[x].Count);

            pos = new Vector2(arrnumXies[x][y].x, arrnumXies[x][y].y);
            arrnumXies[x].Remove(arrnumXies[x][y]);
            if (arrnumXies[x].Count <= 0)
                arrnumXies.Remove(arrnumXies[x]);
        }



        //우측 찾아야되는 흰색 라인그려져있는 동물 이미지 로드
        /// <summary>
        /// 우측 이미지 로딩
        /// </summary>
        /// <returns></returns>
        private async UniTask rLoaing()
        {
            for (int i = 0; i < R.Count; i++)
            {
                //처음  0 ~ 4 까지 5개만 문제로 출제 
                await loadSprite.LoadSpriteData($"{animalNames[i]}WLine", R[i].spriteRenderer);
                //회색으로 변환
                R[i].spriteRenderer.color = Color.gray;
                // 0 번 부터 4번까지 해당되는 동물 이름 저장 
                var dd = Enum.Parse(typeof(FindStuff), animalNames[i]);
                R[i].stuff = (FindStuff)dd;
            }
            await UniTask.Yield();
        }

        /// <summary>
        /// 좌측 이미지 로딩
        /// </summary>
        /// <param name="arrnumXies"></param>
        /// <returns></returns>
        private async UniTask lLoding(List<List<ArrnumXY>> arrnumXies)
        {
            for (int i = 0; i < L.Length; i++)
            {
                Vector2 pos = Vector2.zero;
                PosCur(ref pos, arrnumXies);

                // 0 번 부터 4번까지 해당되는 동물 이름 저장 
                var dd = Enum.Parse(typeof(FindStuff), animalNames[i]);
                L[i].stuff = (FindStuff)dd;

                //동물 이미지 로드
                await loadSprite.LoadSpriteData($"{animalNames[i]}", L[i].spRender);

                //로드된 이미지 위치값 세팅
                L[i].transform.localPosition = pos;

            }
        }

        /// <summary>
        /// 배경이미지 로드
        /// </summary>
        /// <param name="stage"></param>
        /// <returns></returns>
        public async UniTask BgImageSet(int stage)
        {
            await loadSprite.LoadSpriteData($"ST{stage}", BG);
        }

        //화면전환
        public async UniTask StageChage(int stage)
        {
            loadingState = LoadingState.Loading;
            //로딩이미지
            await loadingOnOff("On", 1);

            //맵에 생성되있는 동그라미 제거
            foreach (var item in highlights) pulling.ReturnObj(item);
            //동그라미 리스트 비움
            highlights.Clear();
            //전부 체크 안함으로 변경 
            foreach (var item in L) item.B_Check = false;
            //왼쪽 오른쪽 이미지 세팅
            LRSpriteSetting().Forget();
            //배경이미지 세팅
            BgImageSet(stage).Forget();
            //로딩이미지 세팅
            await loading_ImgLoad();

            await UniTask.Delay(TimeSpan.FromSeconds(0.4f), DelayType.UnscaledDeltaTime);
            //로딩이미지
            loadingOnOff("Off", 0).Forget();

            loadingState = LoadingState.Game;
        }

        //로딩이미지 페이드인아웃
        private async UniTask loadingOnOff(string onoff,float alpha)
        {
            await DOTween.To(() => fadeSpRender.color, x => fadeSpRender.color = x, new Color(1, 1, 1, alpha), 0.7f);
        }

        //로딩이미지 로드
        private async UniTask loading_ImgLoad()
        {
            for (int i = 0; i <= 60; i++)
            {
                //0.02초 마다 다음 이미지 로딩
                await UniTask.Delay(TimeSpan.FromSeconds(0.02f), DelayType.UnscaledDeltaTime);
                await loadSprite.LoadSpriteData($"Loading/Loading_{i}", fadeSpRender);
            }
        }
        
        public void Update()
        {
            if (zozo != null) zozo.MGR.Excute(null);
        }
    }
}