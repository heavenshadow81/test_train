//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Windows.Kinect;

//namespace ML.MLBKids
//{
//    public class KinectHelper : MonoBehaviour
//    {
//        #region Classes
//        public class UserData
//        {
//            public bool isReadyToPitchLeft, isReadyToPitchRight;
//            public List<Vector3> depthListLeft = new List<Vector3>();
//            public List<Vector3> depthListRight = new List<Vector3>();
//            public List<HandState> handStateLeft = new List<HandState>();
//            public List<HandState> handStateRight = new List<HandState>();
//            public void Clear()
//            {
//                isReadyToPitchLeft = isReadyToPitchRight = false;
//                depthListLeft.Clear();
//                depthListRight.Clear();
//                handStateLeft.Clear();
//                handStateRight.Clear();
//            }
//        }
//        #endregion

//        #region Properties
//        // singleton
//        public static KinectHelper instance { get; private set; }

//        // sensor state
//        public bool isOpen { get { return _sensor != null; } }

//        // body tracking
//        public bool enablesBodyTracking { get; set; }
//        public ulong trackingId
//        {
//            get { return _trackedBody != null ? _trackedBody.TrackingId : 0; }
//        }
//        public Body trackedBody { get { return _trackedBody; } }
//        public Vector2 uiSpaceRightHandPos { get; private set; }
//        public HandState rightHandState { get; private set; }
//        public bool isLastBodySeemsComeClose { get; private set; }

//        // regions (color, depth)
//        public Vector2 colorRegion { get; private set; }
//        public Vector2 depthRegion { get; private set; }
//        #endregion

//        #region Private variables (Kinect)
//        private KinectSensor _sensor;
//        private CoordinateMapper _coordinateMapper;
//        private BodyFrameReader _bodyFrameReader;
//        private MultiSourceFrameReader _multiSourceFrameReader;

//        private Body[] _bodies;
//        private Body _trackedBody;

//        private UserData[] _userDatas;
//        #endregion

//        #region Events
//        public delegate void OnBodyTracked(ulong trackingid);
//        public delegate void OnPitch(bool isRightHand, ulong trackingId, Vector2 normalizedPos, Vector3 direction, float power);
//        public event OnBodyTracked onBodyTracked;
//        public event OnPitch onPitch;
//        #endregion

//        #region Unity Methods
//        public void Awake()
//        {
//            if (instance == null)
//            {
//                instance = this;
//            }
//            else
//            {
//                Destroy(this);
//            }
//        }

//        public void OnDestroy()
//        {
//            if (_multiSourceFrameReader != null)
//            {
//                _multiSourceFrameReader.Dispose();
//                _multiSourceFrameReader = null;
//            }

//            if (_sensor != null)
//            {
//                _sensor.Close();
//                _sensor = null;
//            }

//            if (instance == this)
//                instance = null;
//        }
//        #endregion

//        public bool Init()
//        {
//            // initialize the sensor
//            try
//            {
//                _sensor = KinectSensor.GetDefault();
//                if (_sensor == null)
//                {
//                    Debug.LogWarning("No kinect device detected on this PC!");
//                    return false;
//                }
//                Debug.Log(string.Format("Detected kinect device (id:{0}).", _sensor.UniqueKinectId));

//                // mapper
//                _coordinateMapper = _sensor.CoordinateMapper;

//                // color frame
//                FrameDescription colorFrameDesc = _sensor.ColorFrameSource.FrameDescription;
//                colorRegion = new Vector2(colorFrameDesc.Width, colorFrameDesc.Height);
//                Debug.Log(string.Format("Color region : ({0}, {1})", colorRegion.x, colorRegion.y));

//                // depth frame
//                FrameDescription depthFrameDesc = _sensor.DepthFrameSource.FrameDescription;
//                depthRegion = new Vector2(depthFrameDesc.Width, depthFrameDesc.Height);
//                Debug.Log(string.Format("Depth region : ({0}, {1})", depthRegion.x, depthRegion.y));

//                // open reader
//                _multiSourceFrameReader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body);
//                _multiSourceFrameReader.MultiSourceFrameArrived += _OnArriveMultiSourceFrame;

//                _sensor.Open();
//            }
//            catch
//            {
//                Debug.LogWarning("Unable to initialize Kinect!");
//                return false;
//            }
//            return true;
//        }

//        #region Frame
//        private void _OnArriveMultiSourceFrame(object sender, MultiSourceFrameArrivedEventArgs e)
//        {
//            _QueryBodies(e);
//            _TrackBody();
//            _TrackRightHand();
//            _TrackGestures();
//        }
//        #endregion

//        #region Tracking
//        private bool _IsTrackableArea(ColorSpacePoint colorSpacePos, float cameraSpacePosZ)
//        {
//            float aspectRatio = (float)Screen.width / (float)Screen.height;

//            float cond1 = Mathf.Abs(colorRegion.x * 0.5f - colorSpacePos.X);
//            float cond3 = cameraSpacePosZ;

//            bool check1 = cond1 < colorRegion.y * 0.5f * aspectRatio;
//            bool check3 = cond3 < 3.0f;

//            return check1 && check3;
//        }

//        private void _QueryBodies(MultiSourceFrameArrivedEventArgs e)
//        {
//            MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();
//            using (BodyFrame bodyFrame = multiSourceFrame.BodyFrameReference.AcquireFrame())
//            {
//                if (bodyFrame != null)
//                {
//                    if (_bodies == null || _bodies.Length != bodyFrame.BodyCount)
//                        _bodies = new Body[bodyFrame.BodyCount];

//                    bodyFrame.GetAndRefreshBodyData(_bodies);
//                }
//            }
//        }

//        private void _TrackBody()
//        {
//            if (enablesBodyTracking)
//            {
//                if (trackingId == 0)
//                {
//                    for (int i = 0; i < _bodies.Length; i++)
//                    {
//                        Body body = _bodies[i];
//                        if (body.IsTracked)
//                        {
//                            var shoulderJoint = body.Joints[JointType.SpineShoulder];
//                            var colorSpacePos = _coordinateMapper.MapCameraPointToColorSpace(shoulderJoint.Position);
//                            if (_IsTrackableArea(colorSpacePos, shoulderJoint.Position.Z))
//                            {
//                                _trackedBody = body;
//                                isLastBodySeemsComeClose = false;
//                                Debug.Log("Find body with tracking id : " + trackingId);
//                                if (onBodyTracked != null)
//                                    onBodyTracked(trackingId);
//                                break;
//                            }
//                        }
//                    }
//                }
//                else
//                {
//                    bool isTracked = false;
//                    for (int i = 0; i < _bodies.Length; i++)
//                    {
//                        Body body = _bodies[i];
//                        if (body.TrackingId == trackingId)
//                        {
//                            if (body.IsTracked)
//                            {
//                                var shoulderJoint = body.Joints[JointType.SpineShoulder];
//                                var colorSpacePos = _coordinateMapper.MapCameraPointToColorSpace(shoulderJoint.Position);
//                                isTracked = _IsTrackableArea(colorSpacePos, shoulderJoint.Position.Z);
//                                _trackedBody = body;
//                                break;
//                            }
//                        }
//                    }

//                    if (!isTracked)
//                    {
//                        var lastJoint = _trackedBody.Joints[JointType.SpineBase];
//                        if (lastJoint.Position.Z < 0.9f && Mathf.Abs(lastJoint.Position.X) < 0.5f)
//                        {
//                            Debug.Log("Couldn't detect body because of coming too close.");
//                            isLastBodySeemsComeClose = true;
//                        }
//                        else
//                        {
//                            Debug.Log("Couldn't detect body because of getting out of range.");
//                            isLastBodySeemsComeClose = false;
//                        }

//                        Debug.Log("x value is " + lastJoint.Position.X);
//                        Debug.Log("z value is " + lastJoint.Position.Z);

//                        _trackedBody = null;
//                        Debug.Log("Lost body ");
//                        if (onBodyTracked != null)
//                            onBodyTracked(0);
//                    }
//                }
//            }
//        }

//        private void _TrackRightHand()
//        {
//            if(_trackedBody != null)
//            {
//                var spineShoulder = _trackedBody.Joints[JointType.SpineShoulder];
//                var shoulderRight = _trackedBody.Joints[JointType.ShoulderRight];
//                var handRight = _trackedBody.Joints[JointType.HandRight];

//                float screenSpaceAspect = (float)Screen.width / (float)Screen.height;
//                float colorSpaceAspect = colorRegion.x / colorRegion.y;
//                float depthSpaceAspect = depthRegion.x / depthRegion.y;
//                Vector2 pos = Vector2.zero;

//                Vector3 vec = new Vector3(
//                    handRight.Position.X - shoulderRight.Position.X,
//                    handRight.Position.Y - spineShoulder.Position.Y,
//                    handRight.Position.Z - spineShoulder.Position.Z
//                    ).normalized;

//                // Calculates normalized position via angle
//                float angleX = (Mathf.PI - Mathf.Acos(vec.x)) - Mathf.PI * 0.5f;
//                float angleY = -Mathf.Asin(vec.y);
//                pos.x = angleX / Mathf.PI * 3.0f;
//                pos.y = angleY / Mathf.PI * 4.0f;

//                // [-1~1] to [0~1]
//                pos.x = (pos.x + 1.0f) * 0.5f;
//                pos.y = (pos.y + 1.0f) * 0.5f;

//                // Convert position from normalized space to ui space [1080x1920]
//                pos.x = pos.x * 1920.0f * depthSpaceAspect;
//                pos.x = 960.0f * screenSpaceAspect + pos.x - 960.0f * depthSpaceAspect;
//                pos.y = pos.y * 1920.0f;

//                // Clamps to screen space
//                pos.x = Mathf.Clamp(pos.x, 0, 1920.0f * screenSpaceAspect);
//                pos.y = Mathf.Clamp(pos.y, 0, 1920.0f);

//                uiSpaceRightHandPos = pos;
//                rightHandState = _trackedBody.HandRightState;
//            }
//        }

//        private void _TrackGestures()
//        {
//            if (_bodies == null || trackingId == 0) return;

//            if (_userDatas == null || _userDatas.Length != _bodies.Length)
//            {
//                _userDatas = new UserData[_bodies.Length];
//                for (int i = 0; i < _userDatas.Length; i++)
//                    _userDatas[i] = new UserData();
//            }

//            for (int i = 0; i < _bodies.Length; i++)
//            {
//                Body body = _bodies[i];
//                UserData userData = _userDatas[i];

//                if (!body.IsTracked)
//                {
//                    userData.Clear();
//                    continue;
//                }
//                if (body.Joints[JointType.SpineBase].TrackingState == TrackingState.NotTracked &&
//                    (body.Joints[JointType.HandLeft].TrackingState == TrackingState.NotTracked ||
//                    body.Joints[JointType.HandRight].TrackingState == TrackingState.NotTracked))
//                {
//                    userData.Clear();
//                    continue;
//                }

//                float readyThreshold = GlobalConstants.instance.gestureReadyThreshold;

//                if (!userData.isReadyToPitchLeft)
//                {
//                    if (Mathf.Abs(body.Joints[JointType.SpineBase].Position.Z - body.Joints[JointType.HandLeft].Position.Z) < readyThreshold)
//                        userData.isReadyToPitchLeft = true;
//                }
//                if (userData.isReadyToPitchLeft)
//                {
//                    _AddDepthData(body, userData, JointType.HandLeft);
//                    _AnalyzeEffect(body, userData, JointType.HandLeft);
//                }

//                if (!userData.isReadyToPitchRight)
//                {
//                    if (Mathf.Abs(body.Joints[JointType.SpineBase].Position.Z - body.Joints[JointType.HandRight].Position.Z) < readyThreshold)
//                        userData.isReadyToPitchRight = true;
//                }
//                if (userData.isReadyToPitchRight)
//                {
//                    _AddDepthData(body, userData, JointType.HandRight);
//                    _AnalyzeEffect(body, userData, JointType.HandRight);
//                }
//            }
//        }

//        private void _AddDepthData(Body body, UserData userData, JointType jointType)
//        {
//            var cameraSpaceJointPos = body.Joints[jointType].Position;
//            Vector3 vPos = new Vector3(cameraSpaceJointPos.X, cameraSpaceJointPos.Y, cameraSpaceJointPos.Z);

//            int effectFrameCount = GlobalConstants.instance.gestureEffectFrameCount;
//            var depthList = jointType == JointType.HandLeft ? userData.depthListLeft : userData.depthListRight;
//            var handStateList = jointType == JointType.HandLeft ? userData.handStateLeft : userData.handStateRight;
//            var handState = jointType == JointType.HandLeft ? body.HandLeftState : body.HandRightState;

//            if (depthList.Count >= effectFrameCount)
//            {
//                depthList.RemoveAt(0);
//                handStateList.RemoveAt(0);
//            }
//            depthList.Add(vPos);
//            handStateList.Add(handState);
//        }

//        private void _AnalyzeEffect(Body body, UserData userData, JointType jointType)
//        {
//            float effectDistance = GlobalConstants.instance.gestureEffectDistance;
//            float depthThreshold = GlobalConstants.instance.gestureDepthThreshold;
//            int effectFrameCount = GlobalConstants.instance.gestureEffectFrameCount;

//            var depthList = jointType == JointType.HandLeft ? userData.depthListLeft : userData.depthListRight;
//            var handStateList = jointType == JointType.HandLeft ? userData.handStateLeft : userData.handStateRight;

//            int maxHandOpened = 2;
//            if (depthList.Count == effectFrameCount)
//            {
//                Vector3 interval = (depthList[0] - depthList[depthList.Count - 1]);
//                if (interval.z > 0 && interval.magnitude > effectDistance)
//                {
//                    int openCount = 0;
//                    foreach (var handState in handStateList)
//                    {
//                        if (handState == HandState.Open)
//                        {
//                            openCount++;
//                            if (openCount >= maxHandOpened) break;
//                        }
//                    }

//                    if (openCount == maxHandOpened)
//                    {
//                        Vector2 handPos = Vector2.zero;
//                        float depthValue = body.Joints[JointType.Head].Position.Z - body.Joints[jointType].Position.Z;
//                        if (depthValue > depthThreshold)
//                        {
//                            bool isRightHand = jointType == JointType.HandRight;

//                            float screenSpaceAspect = (float)Screen.width / (float)Screen.height;
//                            DepthSpacePoint depthSpaceHandPos = _coordinateMapper.MapCameraPointToDepthSpace(body.Joints[jointType].Position);
//                            Vector2 depthSpacePos = new Vector2(depthSpaceHandPos.X, depthSpaceHandPos.Y);
//                            Vector2 normalizedPos = new Vector2(
//                                (depthSpacePos.x - depthRegion.x * 0.5f) / (depthRegion.x * 0.5f * screenSpaceAspect),
//                                (depthRegion.y * 0.5f - depthSpacePos.y) / (depthRegion.y * 0.5f));
                            
//                            Vector3 direction = Vector3.forward + Vector3.up * (0.1f + (2.0f - normalizedPos.y) * 0.15f);
//                            direction += Vector3.right * (isRightHand ? -1.0f : 1.0f) * Random.Range(-0.075f, 0.175f);

//                            float speed = Mathf.Abs(depthList[0].z - depthList[5].z) * 6.0f;
//                            /*
//                            float powerFactor = Mathf.Abs(interval.z) / effectDistance;
//                            powerFactor += 0.2f * normalizedPos.y;
//                            if (powerFactor < 0.7f) power = 0.7f;
//                            */
//                            float powerFactor = speed;
//                            if (powerFactor < 0.7f)
//                                powerFactor = 0.7f;

//                            if (onPitch != null)
//                                onPitch(jointType == JointType.HandRight, body.TrackingId, normalizedPos, direction, powerFactor);

//                            Debug.Log(string.Format("Pitch motion detected. : {0} hand, pos : {1}, dir : {2}, power : {3:0.00}", 
//                                isRightHand ? "right" : "left", normalizedPos.ToString("0.00"), direction.ToString("0.00"), powerFactor));

//                            if (isRightHand)
//                                userData.isReadyToPitchRight = false;
//                            else
//                                userData.isReadyToPitchLeft = false;
//                        }
//                    }
//                }
//            }
//        }
//        #endregion
//    }
//}
