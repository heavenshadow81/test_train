using UnityEngine;
using Windows.Kinect;
using System.Runtime.InteropServices;

namespace ML.PlaywallKids.Interaction
{
    public class CoordinateMapperManager : MonoBehaviour
    {
        // Member Variable
        private KinectSensor _sensor;
        private CoordinateMapper _coordinateMapper;
        private MultiSourceFrameReader _multiSourceFrameReader;
        private DepthSpacePoint[] _depthCoordinatesArray;               // depth 영역과 color 영역의 위치를 매칭시키기 위해 필요한 것으로 판단됨?ㅡ.ㅡ?
        private Body[] _bodyDataArray;                                  // Skeleton Data 객체

        private byte[] _bodyIndexBufferArray;                           // depth 영역에서 사람 영역인지 아닌지를 판별하기 위한 배열(그림자 영상 출력을 위한 사전 데이터로 사용 - 빅보드 : 하트)
        private byte[] _colorBufferArray;                               // color Bitmap을 저장하기 위한 배열(Color 영상 출력을 위해 사용 - 빅보드 : 등불)
        private ushort[] _depthBufferArray;                             // depth Data를 저장하기 위한 배열(안쓰일 것 같음?ㅡ.ㅡ?)

        public bool IsSensorOpened()
        {
            return _sensor != null && _sensor.IsOpen && _sensor.IsAvailable;
        }

        public CoordinateMapper GetCoordinateMapper()
        {
            return _coordinateMapper;
        }

        public Body[] GetBodyDataBuffer()
        {
            return _bodyDataArray;
        }

        public byte[] GetColorImageBuffer()
        {
            return _colorBufferArray;
        }

        public byte[] GetBodyIndexBuffer()
        {
            return _bodyIndexBufferArray;
        }

        public ushort[] GetDepthBuffer()
        {
            return _depthBufferArray;
        }

        public DepthSpacePoint[] GetDepthCoordinates()
        {
            return _depthCoordinatesArray;
        }

        // 유니티 엔진이 시작하기 위해 모든 준비가 끝나면 컴포넌트에서 가장 먼저 호출
        void Awake()
        {
            OnEnable();
        }
        
        public void OnEnable()
        {
            if (_sensor == null|| !_sensor.IsOpen)
            {
                _colorBufferArray = new byte[Constants.DEFAULT_COLOR_WIDTH * Constants.DEFAULT_COLOR_HEIGHT * 4];             // RGBA 크기 1pixel당 4byte
                _bodyIndexBufferArray = new byte[Constants.DEFAULT_DEPTH_WIDTH * Constants.DEFAULT_DEPTH_HEIGHT];             // player 0,1 1pixel당 1byte
                _depthBufferArray = new ushort[Constants.DEFAULT_DEPTH_WIDTH * Constants.DEFAULT_DEPTH_HEIGHT];
                _depthCoordinatesArray = new DepthSpacePoint[Constants.DEFAULT_COLOR_WIDTH * Constants.DEFAULT_COLOR_HEIGHT];             // depth영역과 color 영역 매칭

                InitializeDefaultSensor();  // 키넥트 초기화 함수 호출

                Debug.Log("Initializing Kinect Sensor...");
            }
        }

        // Update is called once per frame
        void Update()
        {
            //OnEnable();
            if (_multiSourceFrameReader == null) return;

            var multiSourceFrame = _multiSourceFrameReader.AcquireLatestFrame();

            if (multiSourceFrame != null)
            {
                using (var bodyFrame = multiSourceFrame.BodyFrameReference.AcquireFrame())
                using (var depthFrame = multiSourceFrame.DepthFrameReference.AcquireFrame())
                using (var colorFrame = multiSourceFrame.ColorFrameReference.AcquireFrame())
                using (var bodyIndexFrame = multiSourceFrame.BodyIndexFrameReference.AcquireFrame())
                {
                    if (depthFrame != null)
                    {
                        var depthData = GCHandle.Alloc(_depthBufferArray, GCHandleType.Pinned);
                        depthFrame.CopyFrameDataToIntPtr(depthData.AddrOfPinnedObject(), (uint)_depthBufferArray.Length * sizeof(ushort));
                        depthData.Free();
                    }

                    if (colorFrame != null)
                    {
                        var colorData = GCHandle.Alloc(_colorBufferArray, GCHandleType.Pinned);
                        colorFrame.CopyConvertedFrameDataToIntPtr(colorData.AddrOfPinnedObject(), (uint)_colorBufferArray.Length, ColorImageFormat.Rgba);
                        colorData.Free();
                    }

                    if (bodyIndexFrame != null)
                    {
                        var bodyIndexData = GCHandle.Alloc(_bodyIndexBufferArray, GCHandleType.Pinned);
                        bodyIndexFrame.CopyFrameDataToIntPtr(bodyIndexData.AddrOfPinnedObject(), (uint)_bodyIndexBufferArray.Length);
                        bodyIndexData.Free();
                    }

                    if (bodyFrame != null)
                    {
                        if (_bodyDataArray == null) _bodyDataArray = new Body[_sensor.BodyFrameSource.BodyCount];
                        bodyFrame.GetAndRefreshBodyData(_bodyDataArray);
                    }
                }
            }
            else
            {
                //UnityEngine.Debug.Log("MultiSourceFrame is Null");
                //return;
            }
        }

        void OnDisable()
        {
            _depthBufferArray = null;
            _colorBufferArray = null;
            _bodyIndexBufferArray = null;

            if (_depthCoordinatesArray != null)
            {
                _depthCoordinatesArray = null;
            }

            if (_multiSourceFrameReader != null)
            {
                _multiSourceFrameReader.Dispose();
                _multiSourceFrameReader = null;
            }

            if (_sensor != null)
            {
                if (_sensor.IsOpen)
                    _sensor.Close();
                _sensor = null;
            }

            Debug.Log("Releasing Kinect Sensor...");
        }

        void OnApplicationQuit()
        {
            if (_sensor != null)
            {
                OnDisable();
            }
        }

        // 키넥트 초기화 함수
        private void InitializeDefaultSensor()
        {
            // 센서 초기화 
            if (_sensor == null|| !_sensor.IsOpen)
            {
                _sensor = KinectSensor.GetDefault();
                if (_sensor != null || !_sensor.IsOpen)
                {
                    _coordinateMapper = _sensor.CoordinateMapper;
                }
                else
                {
                    Debug.LogError("Kinect Error");
                }
            }

            // 센서 상태 확인, 열기
            if (_sensor != null)
            {
                if (!_sensor.IsOpen)
                {
                    _sensor.Open();
                    if (_sensor.IsOpen)
                        _multiSourceFrameReader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.BodyIndex | FrameSourceTypes.Body);
                    else
                        Debug.LogError("Failed To Open Kinect");
                }
            }
            else
            {
                Debug.LogError("Kinect Device Is Not Available");
            }
        }
        //콘텐츠 실행시 키넥트 상태 확인...!
        public void CheckKinectState()
        {
            OnEnable();
        }
    }
}