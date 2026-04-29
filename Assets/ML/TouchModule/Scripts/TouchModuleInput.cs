using UnityEngine;

namespace ML.TouchModule
{
    using System;
    using System.Collections.Generic;
    using vCatchStation;

    public enum TouchSensor
    {
        None,
        VCatch
    }

    public class TouchModuleInput : MonoBehaviour
    {
        private static TouchModuleInput _instance;
        public static TouchModuleInput Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("_TouchModuleInput");
                    go.hideFlags = HideFlags.HideInHierarchy;
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<TouchModuleInput>();
                    _instance._Init();
                }
                return _instance;
            }
        }

        public bool Initialized { get; private set; }

        public TouchSensor CurrentSensor { get { if (_vCatch != null && _vCatch.Initialized) return TouchSensor.VCatch; return TouchSensor.None; } }

        private float Speed_Internal { get; set; }
        public static int touchCount { get { return Instance._touches.Count; } }
        public static Touch[] touches { get { return Instance._touches.ToArray(); } }
        public static Touch GetTouch(int i) { return Instance._touches[i]; }
        public static Vector2 mousePosition { get; private set; }
        public static float Speed { get { return Instance.Speed_Internal; } set { Instance.Speed_Internal = value; } }

        private vCatchModule _vCatch;
        private List<Touch> _touches = new List<Touch>();

        private float _delay = 0;

        public static void Init()
        {
            Instance._Init();
        }

        private void _Init()
        {
            if (Initialized) return;
            Initialized = true;

            _vCatch = new vCatchModule();
            _vCatch.Result2DS += _OnResult2DS;
            _vCatch.MiddlewareReceived += _OnMiddlewareReceived;
            _vCatch.SensorStatusChanged += _OnSensorStatusChanged;
            _vCatch.Init(ModuleID.TwoDimenSpeed);
        }

        private void _OnSensorStatusChanged(vCatchResult_SensorStatus result)
        {
            _VCatchStartGame();
        }
        
        private void _OnMiddlewareReceived()
        {
            _VCatchStartGame();
        }

        public void OnApplicationQuit()
        {
            if (_vCatch != null)
            {
                _vCatch.Cleanup();
                _vCatch = null;
            }
        }

        //public void Update()
        //{
        //    if (CurrentSensor == TouchSensor.None)
        //    {
        //        _touches.Clear();

        //        if (Input.touchSupported)
        //        {
        //            _touches.AddRange(Input.touches);
        //        }
        //        if (Input.touchSupported)
        //        {
        //            if (Input.GetMouseButtonDown(0))
        //            {
        //                // Mouse input -> touch
        //                Touch touch = new Touch();
        //                touch.position = Input.mousePosition;
        //                touch.phase = TouchPhase.Began;
        //                touch.tapCount = 1;
        //                _touches.Add(touch);
        //                mousePosition = Input.mousePosition;
        //                Speed_Internal = 0;
        //            }
        //            else if (Input.GetMouseButtonUp(0))
        //            {
        //                // Mouse input -> touch
        //                Touch touch = new Touch();
        //                touch.position = Input.mousePosition;
        //                touch.phase = TouchPhase.Ended;
        //                touch.tapCount = 1;
        //                _touches.Add(touch);
        //                mousePosition = Input.mousePosition;
        //                Speed_Internal = 0;
        //            }
        //            else if (Input.GetMouseButton(0))
        //            {
        //                // Mouse input -> touch
        //                Touch touch = new Touch();
        //                touch.position = Input.mousePosition;
        //                touch.phase = TouchPhase.Moved;
        //                touch.tapCount = 1;
        //                _touches.Add(touch);
        //                mousePosition = Input.mousePosition;
        //                Speed_Internal = 0;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        _delay += Time.deltaTime;
        //        if (_delay >= 10.0f)
        //        {
        //            _VCatchStartGame();
        //        }
        //    }
        //} 유니티에서 테스트 불가하여 이다인 업데이트문 수정

        public void Update()
        {
            _touches.Clear(); // 매 프레임 터치 목록을 초기화합니다.

            if (Input.touchSupported && Input.touchCount > 0)
            {
                // 터치 입력 처리
                _touches.AddRange(Input.touches);
                mousePosition = _touches[0].position; // 첫 번째 터치의 위치를 마우스 위치로 간주
                Speed_Internal = 0; // 속도는 기본적으로 0으로 초기화
            }
            else
            {
                // 마우스 입력 처리
                if (Input.GetMouseButtonDown(0))
                {
                    // 마우스 클릭 시작 -> 터치로 간주
                    Touch touch = new Touch
                    {
                        position = Input.mousePosition,
                        phase = TouchPhase.Began,
                        tapCount = 1
                    };
                    _touches.Add(touch);
                    mousePosition = Input.mousePosition;
                    Speed_Internal = 0;
                }
                else if (Input.GetMouseButton(0))
                {
                    // 마우스 이동 -> 터치 이동으로 간주
                    Touch touch = new Touch
                    {
                        position = Input.mousePosition,
                        phase = TouchPhase.Moved,
                        tapCount = 1
                    };
                    _touches.Add(touch);
                    mousePosition = Input.mousePosition;
                    Speed_Internal = 0;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    // 마우스 클릭 종료 -> 터치 종료로 간주
                    Touch touch = new Touch
                    {
                        position = Input.mousePosition,
                        phase = TouchPhase.Ended,
                        tapCount = 1
                    };
                    _touches.Add(touch);
                    mousePosition = Input.mousePosition;
                    Speed_Internal = 0;
                }
            }

            // vCatch 처리
            if (CurrentSensor != TouchSensor.None)
            {
                _delay += Time.deltaTime;
                if (_delay >= 10.0f)
                {
                    _VCatchStartGame();
                }
            }
        }



        public void LateUpdate()
        {
            if (_vCatch != null && _vCatch.Initialized)
                _touches.Clear();
        }
        
        private void _OnResult2DS(vCatchResult_2DS result)
        {
            // vCatchResult -> touch
            _touches.Clear();
            Touch touch = new Touch();
            touch.position = new Vector2(result.posX * Screen.width, (1.0f - result.posY) * Screen.height);
            touch.phase = TouchPhase.Began;
            touch.tapCount = 1;
            _touches.Add(touch);
            Speed_Internal = result.speed;
            mousePosition = touch.position;
            Invoke("_VCatchStartGame", 1.0f);
        }

        private void _VCatchStartGame()
        {
            if (_vCatch != null && _vCatch.Initialized)
            {
                _delay = 0;
                _vCatch.StartGame();
                _vCatch.Command(vCatchModule.vCatchStation_CMD_SetTimeout(10000));
            }
        }
    }
}