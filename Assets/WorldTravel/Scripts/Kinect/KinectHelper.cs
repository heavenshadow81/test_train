using System.Runtime.InteropServices;
using UnityEngine;
using Windows.Kinect;
using ML.MapoContents.WorldTravel;
namespace ML.MapoContents.Kinect
{
    public class KinectHelper : MonoBehaviour
    {
        #region Properties
        // singleton객체
        public static KinectHelper instance { get; private set; }

        // sensor state
        public bool isOpen { get { return _sensor != null; } }

        // body tracking
        public bool enablesBodyTracking { get; set; }
        public ulong trackingId
        {
            get { return _trackedBody != null ? _trackedBody.TrackingId : 0; }
        }
        public Body trackedBody { get { return _trackedBody; } }
        public Vector2 uiSpaceRightHandPos { get; private set; }
        public HandState rightHandState { get; private set; }
        public bool isLastBodySeemsComeClose { get; private set; }

        // body index
        public RenderTexture bodyIndexTexture { get { return _bodyIndexTexture; } }

        // regions (color, depth, body index)
        public Vector2 colorRegion { get; private set; }
        public Vector2 depthRegion { get; private set; }
        public Vector2 bodyIndexRegion { get; private set; }

        // screen aspect ratio
        public bool usesCustomScreenSize { get; set; }
        public Vector2 customScreenSize { get; set; }
        #endregion

        #region Private variables (Kinect)
        // sensor, reader
        private KinectSensor _sensor;
        private CoordinateMapper _coordinateMapper;
        private MultiSourceFrameReader _multiSourceFrameReader;

        // body index (and compute shaders)
        private byte[] _bodyIndexByteBuffer;
        private RenderTexture _bodyIndexTexture;
        private ComputeBuffer _bodyIndexBuffer;
        private ComputeShader _computeShader;
        private int _fillEmptyKernel, _chromakeyKernel;

        // body
        private Body[] _bodies;
        private Body _trackedBody;
        private int _trackedBodyIndex = -1;
        #endregion

        #region Events
        public delegate void OnBodyTracked(ulong trackingid);
        public delegate void OnBodyTilt(ulong trackingId, Vector3 tilt);
        public delegate void OnBodyTurn(ulong trackingId, Vector3 dir);
        public event OnBodyTracked onBodyTracked;
        public event OnBodyTilt onBodyTilt;
        public event OnBodyTurn onBodyTurn;
        #endregion

        #region Unity Variable

        #endregion


        #region Unity Methods
        public void Awake()
        {

            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        public void OnDestroy()
        {
            if (_bodyIndexTexture != null)
            {
                Destroy(_bodyIndexTexture);
                _bodyIndexTexture = null;
            }

            if (_multiSourceFrameReader != null)
            {
                _multiSourceFrameReader.Dispose();
                _multiSourceFrameReader = null;
            }

            if (_bodyIndexBuffer != null)
            {
                _bodyIndexBuffer.Release();
                _bodyIndexBuffer = null;
            }

            if (_sensor != null)
            {
                _sensor.Close();
                _sensor = null;
            }

            if (instance == this)
                instance = null;
        }
        #endregion

        public bool Init()
        {
            // initialize the sensor
            try
            {
                //키넥트 sdk 초기화
                _sensor = KinectSensor.GetDefault();
                if (_sensor == null)
                {
                    //디바이스가 없는경우임.
      //              Debug.LogWarning("No kinect device detected on this PC!");
                    return false;
                }
                Debug.Log(string.Format("Detected kinect device (id:{0}).", _sensor.UniqueKinectId));

                // mapper
                _coordinateMapper = _sensor.CoordinateMapper;

                // color frame
                FrameDescription colorFrameDesc = _sensor.ColorFrameSource.FrameDescription;
                colorRegion = new Vector2(colorFrameDesc.Width, colorFrameDesc.Height);
       //         Debug.Log(string.Format("Color region : ({0}, {1})", colorRegion.x, colorRegion.y));

                // depth frame
                FrameDescription depthFrameDesc = _sensor.DepthFrameSource.FrameDescription;
                depthRegion = new Vector2(depthFrameDesc.Width, depthFrameDesc.Height);
       //         Debug.Log(string.Format("Depth region : ({0}, {1})", depthRegion.x, depthRegion.y));

                // body index frame
                FrameDescription bodyIndexFrameDesc = _sensor.BodyIndexFrameSource.FrameDescription;
                bodyIndexRegion = new Vector2(bodyIndexFrameDesc.Width, bodyIndexFrameDesc.Height);
       //         Debug.Log(string.Format("Body Index region : ({0}, {1})", bodyIndexRegion.x, bodyIndexRegion.y));

                // another settings
                usesCustomScreenSize = false;
                customScreenSize = new Vector2(Screen.width, Screen.height);

                // make body index texture
                _bodyIndexTexture = new RenderTexture(Mathf.FloorToInt(bodyIndexRegion.x), Mathf.FloorToInt(bodyIndexRegion.y), 0, RenderTextureFormat.ARGB32);
                _bodyIndexTexture.name = "KinectHelper_bodyIndexTexture";
                _bodyIndexTexture.enableRandomWrite = true;
                _bodyIndexTexture.Create();

                // compute shader
                _computeShader = Resources.Load<ComputeShader>("Shaders/KinectHelper_BodyIndexTexture");
                _fillEmptyKernel = _computeShader.FindKernel("FillEmpty");
                _chromakeyKernel = _computeShader.FindKernel("Chromakey");
                _computeShader.SetTexture(_fillEmptyKernel, "Result", _bodyIndexTexture);
                _computeShader.Dispatch(_fillEmptyKernel, Mathf.FloorToInt(bodyIndexRegion.x) / 8, Mathf.FloorToInt(bodyIndexRegion.y) / 8, 1);
                _bodyIndexByteBuffer = new byte[Mathf.FloorToInt(bodyIndexRegion.x) * Mathf.FloorToInt(bodyIndexRegion.y)];
                _bodyIndexBuffer = new ComputeBuffer(_bodyIndexByteBuffer.Length / 4, 4);

                // open reader
                _multiSourceFrameReader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body | FrameSourceTypes.BodyIndex);
                _multiSourceFrameReader.MultiSourceFrameArrived += _OnArriveMultiSourceFrame;


                //키넥트 sdk 초기화가 끝난 후 사용시작.
                _sensor.Open();
            }
            catch
            {
                //초기화 과정중 에러
                Debug.LogWarning("Unable to initialize Kinect!");
                return false;
            }
            return true;
        }


        #region Frame
        private void _OnArriveMultiSourceFrame(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            _QueryBodies(e);
            _CaptureBodyIndex(e);
			_TrackBody();
			if (TravelManager.Instance._state == TravelManager.TravelState.SelectTheme ||
                TravelManager.Instance._state == TravelManager.TravelState.SelectCountry &&
                !TravelManager.Instance.TabletControll)
            {
                _TrackRightHand();
            }

            if (TravelManager.Instance._state == TravelManager.TravelState.Play && !TravelManager.Instance.TabletControll)
            {
                _TrackRightHandUp();
                _TrackBodyTilt();
                _TrackBodyTurn();
            }
        } 
        #endregion

        #region Tracking
        private bool _IsTrackableArea(ColorSpacePoint colorSpacePos, float cameraSpacePosZ)
        {
            float aspectRatio = (float)Screen.width / (float)Screen.height;

            float cond1 = Mathf.Abs(colorRegion.x * 0.5f - colorSpacePos.X);
            float cond3 = cameraSpacePosZ;

            bool check1 = cond1 < colorRegion.y * 0.5f * aspectRatio;
            bool check3 = cond3 < 5.0f;

            return check1 && check3;
        }

        private void _QueryBodies(MultiSourceFrameArrivedEventArgs e)
        {
            MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();
            using (BodyFrame bodyFrame = multiSourceFrame.BodyFrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (_bodies == null || _bodies.Length != bodyFrame.BodyCount)
                        _bodies = new Body[bodyFrame.BodyCount];

                    bodyFrame.GetAndRefreshBodyData(_bodies);
                }
            }
        }

        private void _CaptureBodyIndex(MultiSourceFrameArrivedEventArgs e)
        {
            MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();
            using (BodyIndexFrame bodyIndexFrame = multiSourceFrame.BodyIndexFrameReference.AcquireFrame())
            {
                using (KinectBuffer bodyIndexBuffer = bodyIndexFrame.LockImageBuffer())
                {
                    Marshal.Copy(bodyIndexBuffer.UnderlyingBuffer, _bodyIndexByteBuffer, 0, (int)_bodyIndexByteBuffer.Length);

                    _bodyIndexBuffer.SetData(_bodyIndexByteBuffer);
                    _computeShader.SetBuffer(_chromakeyKernel, "BodyIndex", _bodyIndexBuffer);
                    _computeShader.SetInt("Width", Mathf.FloorToInt(bodyIndexRegion.x));
                    _computeShader.SetInt("Body", _trackedBodyIndex);
                    _computeShader.SetTexture(_chromakeyKernel, "Result", _bodyIndexTexture);
                    _computeShader.Dispatch(_chromakeyKernel, Mathf.FloorToInt(bodyIndexRegion.x) / 8, Mathf.FloorToInt(bodyIndexRegion.y) / 8, 1);
                }
            }
        }

        private void _TrackBody()
        {
            if (enablesBodyTracking)
            {
                if (trackingId == 0)
                {
                    for (int i = 0; i < _bodies.Length; i++)
                    {
                        Body body = _bodies[i];
                        if (body.IsTracked)
                        {
                            var shoulderJoint = body.Joints[JointType.SpineShoulder];
                            var colorSpacePos = _coordinateMapper.MapCameraPointToColorSpace(shoulderJoint.Position);
                            if (_IsTrackableArea(colorSpacePos, shoulderJoint.Position.Z))
                            {
                                _trackedBody = body;
                                _trackedBodyIndex = i;
                                isLastBodySeemsComeClose = false;
                                //사용자 입장.
                                TravelManager.Instance.CatchPlayer();
                                Debug.Log("Find body with tracking id : " + trackingId);
                                if (onBodyTracked != null)
                                    onBodyTracked(trackingId);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    bool isTracked = false;
                    for (int i = 0; i < _bodies.Length; i++)
                    {
                        Body body = _bodies[i];
                        if (body.TrackingId == trackingId)
                        {
                            if (body.IsTracked)
                            {
                                var shoulderJoint = body.Joints[JointType.SpineShoulder];
                                var colorSpacePos = _coordinateMapper.MapCameraPointToColorSpace(shoulderJoint.Position);
                                isTracked = _IsTrackableArea(colorSpacePos, shoulderJoint.Position.Z);
                                _trackedBody = body;
                                _trackedBodyIndex = i;
                                break;
                            }
                        }
                    }

                    if (!isTracked)
                    {
                        var lastJoint = _trackedBody.Joints[JointType.SpineBase];
                        if (lastJoint.Position.Z < 0.9f && Mathf.Abs(lastJoint.Position.X) < 0.5f)
                        {
                            Debug.Log("Couldn't detect body because of coming too close.");
                            isLastBodySeemsComeClose = true;
                        }
                        else
                        {
                            Debug.Log("Couldn't detect body because of getting out of range.");
                            isLastBodySeemsComeClose = false;
                        }
                        
                        _trackedBody = null;
                        _trackedBodyIndex = -1;
                        TravelManager.Instance.LostPlayer();
                        Debug.Log("Lost body ");
                        if (onBodyTracked != null)
                            onBodyTracked(0);
                    }
                }
            }
        }

        private void _TrackBodyTilt()
        {
            if (_trackedBody != null)
            {
                var spineBase = _trackedBody.Joints[JointType.SpineBase];
                var shoulderBase = _trackedBody.Joints[JointType.SpineShoulder];
                
                Vector3 vec = new Vector3(
                    shoulderBase.Position.X - spineBase.Position.X,
                    shoulderBase.Position.Y - spineBase.Position.Y,
                    shoulderBase.Position.Z - spineBase.Position.Z
                    ).normalized;

                if (onBodyTilt != null)
                    onBodyTilt(_trackedBody.TrackingId, vec);
            }
        }

        private void _TrackBodyTurn()
        {
            if (_trackedBody != null)
            {
                var shoulderLeft = _trackedBody.Joints[JointType.ShoulderLeft];
                var shoulderRight = _trackedBody.Joints[JointType.ShoulderRight];

                if (shoulderLeft.TrackingState != TrackingState.Tracked)
                    shoulderLeft = _trackedBody.Joints[JointType.SpineShoulder];
                else if (shoulderRight.TrackingState != TrackingState.Tracked)
                    shoulderRight = _trackedBody.Joints[JointType.SpineShoulder];

                if (shoulderLeft.TrackingState == TrackingState.Tracked &&
                    shoulderRight.TrackingState == TrackingState.Tracked)
                {
                    Vector3 vec = new Vector3(
                        shoulderRight.Position.X - shoulderLeft.Position.X,
                        shoulderRight.Position.Y - shoulderLeft.Position.Y,
                        shoulderRight.Position.Z - shoulderLeft.Position.Z
                        ).normalized;

                    if (onBodyTurn != null)
                        onBodyTurn(_trackedBody.TrackingId, vec);
                }
            }
        }
        public bool AVG_Point;
        public int cnt_avg;
        public Vector3 avg;
        private void _TrackRightHand()
        {
            if(_trackedBody != null)
            {
                //var spineShoulder = _trackedBody.Joints[JointType.SpineShoulder];
                var head = _trackedBody.Joints[JointType.Head];
                var shoulderRight = _trackedBody.Joints[JointType.ShoulderRight];
                var shoulderLeft = _trackedBody.Joints[JointType.ShoulderLeft];
                var spine = _trackedBody.Joints[JointType.SpineBase];
                

                var handRight = _trackedBody.Joints[JointType.HandRight];

                float screenSpaceAspect = usesCustomScreenSize ? (customScreenSize.x / customScreenSize.y) : (Screen.width / (float)Screen.height);
                float depthSpaceAspect = depthRegion.x / depthRegion.y;
                //Debug.Log();
                Vector2 pos = Vector2.zero;


              /*  if (_trackedBody.Joints[JointType.ShoulderRight].TrackingState == TrackingState.Tracked &&
                    _trackedBody.Joints[JointType.ShoulderLeft].TrackingState == TrackingState.Tracked &&
                    _trackedBody.Joints[JointType.SpineMid].TrackingState == TrackingState.Tracked &&
                    _trackedBody.Joints[JointType.Head].TrackingState == TrackingState.Tracked  )
                {*/
                    Vector3 avgPoint = new Vector3(
                       shoulderLeft.Position.X + shoulderRight.Position.X + head.Position.X + spine.Position.X,
                       shoulderLeft.Position.Y + shoulderRight.Position.Y + head.Position.Y + spine.Position.Y,
                       shoulderLeft.Position.Z + shoulderRight.Position.Z + head.Position.Z + spine.Position.Z);
                    avgPoint = avgPoint / 4;

                    avg = avgPoint;
              //  }
                /*if (!AVG_Point)
                {
                    Vector3 avgPoint = new Vector3(
                       shoulderLeft.Position.X + shoulderRight.Position.X + head.Position.X + spine.Position.X,
                       shoulderLeft.Position.Y + shoulderRight.Position.Y + head.Position.Y + spine.Position.Y,
                       shoulderLeft.Position.Z + shoulderRight.Position.Z + head.Position.Z + spine.Position.Z);
                    avgPoint = avgPoint / 4;
                    
                    avg = avgPoint;
                    cnt_avg++;

                    if (cnt_avg >= 20)
                    {
                        AVG_Point = true;
                        Debug.Log("평균점 확정");
                    }
                }*/
                //머리, 명치, 양 어깨 좌표의 평균(합/4)로 노멀라이즈.

                Vector3 vec = new Vector3(
                    handRight.Position.X - avg.x,
                    handRight.Position.Y - avg.y,
                    handRight.Position.Z - avg.z
                    ).normalized;

                // Calculates normalized position via angle
                float angleX = (Mathf.PI - Mathf.Acos(vec.x)) - Mathf.PI * 0.5f;
                float angleY = -Mathf.Asin(vec.y);
                pos.x = angleX / Mathf.PI * 3.0f;
                pos.y = angleY / Mathf.PI * 4.0f;

                // [-1~1] to [0~1]
                pos.x = (pos.x + 1.0f) * 0.5f;
                pos.y = (pos.y + 1.0f) * 0.5f;
              //  Debug.Log("pos : " + pos);

                // Convert position from normalized space to ui space
                if (screenSpaceAspect < depthSpaceAspect)
                {
                    float screenHeight = usesCustomScreenSize ? customScreenSize.y : Screen.height;
                    pos.x = pos.x * screenHeight * depthSpaceAspect;
                    pos.x = screenHeight * 0.5f * screenSpaceAspect + pos.x - screenHeight * 0.5f * depthSpaceAspect;
                    pos.y = pos.y * screenHeight;

                    // Clamps to screen space
                    pos.x = Mathf.Clamp(pos.x, 0, screenHeight * screenSpaceAspect);
                    pos.y = Mathf.Clamp(pos.y, 0, screenHeight);
                }
                else
                {
                    float screenWidth = usesCustomScreenSize ? customScreenSize.x : Screen.width;
                    pos.y = pos.y * screenWidth / depthSpaceAspect;
                    pos.y = screenWidth * 0.5f / screenSpaceAspect + pos.y - screenWidth * 0.5f / depthSpaceAspect;
                    pos.x = pos.x * screenWidth;

                    // Clamps to screen space
                    pos.x = Mathf.Clamp(pos.x, 0, screenWidth);
                    pos.y = Mathf.Clamp(pos.y, 0, screenWidth / screenSpaceAspect);

                }
                uiSpaceRightHandPos = pos;
                rightHandState = _trackedBody.HandRightState;
            }
        }
        public bool CheckUperHand = false;
        private void _TrackRightHandUp()
        {
            if (_trackedBody != null)
            {
                var elbowRight = _trackedBody.Joints[JointType.ElbowRight];
                var head = _trackedBody.Joints[JointType.Head];
                var handRight = _trackedBody.Joints[JointType.HandRight];
                var shoulderRight = _trackedBody.Joints[JointType.ShoulderRight];

				float x = handRight.Position.X - elbowRight.Position.X;
				if (elbowRight.Position.Y > shoulderRight.Position.Y &&  handRight.Position.Y > head.Position.Y && (x > -0.1f && x < 0.1f))
                {
					TravelManager.Instance.CaptureEvent ();
                }                
				else 
				{
					TravelManager.Instance.CancleCapture ();
				}
            }
        }
        #endregion
    }
}
