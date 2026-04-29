using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;
using System.IO;
using System.Threading;
using Windows.Kinect;

namespace ML.PlaywallKids.MotionJump
{
    public class MotionManager : MonoBehaviour
    {

        private const int JumpJumpPacketLength = 507;
        private const int WSAEINTR = 10004;
        private const int WSAEWOULDBLOCK = 10035;
        private const int WSAECONNABORTED = 10053;

        // 필수 변수들
        public GameType _gameType = GameType.None;              // 게임에 맞는 값으로 실행시 할당
        private BigBoardSize _boardSize = BigBoardSize.None; // 빅보드 스크린 크기
        private KinectSensor _sensor;
        private CoordinateMapper _coordinateMapper; // Windows.Kinect.CoordinateMapper
        private BodyFrameReader _bodyFrameReader;   // Windows.Kinect.BodyFrameReader
        private Body[] _bodyDataArr;

        //	private User[] _users;
        private JumpJumpData[] _jumpData = null;    //팔 움직임 추적 클래스

        #region SOCKET

        private Socket _socket = null;
        private string _ip = null;
        private int _port;
        private byte[] _buffer = null;
        private bool _isConnecting = false;
        private bool _isDisconnecting = false;
        private bool _isConnected = false;
        private bool _host = false;
        private Thread _thread = null;
        private bool _isSendPakcet = false;
        public Socket socket
        {
            get { return _socket; }
        }
        public bool IsConnecting
        {
            get { return _isConnecting; }
        }
        public bool IsDisconnecting
        {
            get { return _isDisconnecting; }
        }
        public bool IsConnected
        {
            get { return _isConnected; }
        }

        #endregion

        public JumpJumpData GetCommand(int num)
        {
            if (num > 3)
                return null;
            else
                return _jumpData[num];
        }

        public void SetDefaultData(int num)
        {
            _jumpData = new JumpJumpData[4];

            if (num == 3)
            {
                _boardSize = BigBoardSize.TwoByThree;

                _jumpData[0] = new JumpJumpData();     //팔 움직임 추적 클래스
                _jumpData[1] = new JumpJumpData();
                _jumpData[2] = null;
                _jumpData[3] = null;
            }
            else if (num == 6)
            {
                _boardSize = BigBoardSize.TwoBySix;

                _jumpData[0] = new JumpJumpData();
                _jumpData[1] = new JumpJumpData();
                _jumpData[2] = new JumpJumpData();
                _jumpData[3] = new JumpJumpData();

                try // 3~4인용 하기 위한 소켓
                {
                    FileInfo fileInfo = new FileInfo("ServerInfo.txt");
                    if (fileInfo.Exists == true)
                    {
                        StreamReader sReader = new StreamReader("ServerInfo.txt");
                        _ip = sReader.ReadLine();
                        _port = int.Parse(sReader.ReadLine());

                        sReader.Close();
                    }
                    else
                    {
                        FileStream fs = File.Create("ServerInfo.txt");
                        StreamWriter sWriter = new StreamWriter(fs);
                        sWriter.WriteLine("127.0.0.1");
                        sWriter.WriteLine("7766");

                        _ip = "127.0.0.1";
                        _port = 7766;

                        sWriter.Close();
                        fs.Close();
                    }

                    _buffer = new byte[JumpJumpPacketLength];
                }
                catch (IOException ex)
                {
                    _ip = null;
                    _port = 0;
                    Debug.Log(ex.Message);
                }
                if (_ip != null)
                {
                    Connect(_ip, _port, (flag) =>
                            {
                                if (flag)
                                {
                                    Debug.Log("Connect success");
                                }
                                else
                                {
                                    _jumpData[2] = null;
                                    _jumpData[3] = null;
                                }
                            });
                }
            }
        }

        // Start Kinect Function(Motion Games)
        public void InitializeDefaultSensor()
        {
            _sensor = KinectSensor.GetDefault(); //키넥트 센서 정보

            if (_sensor != null)
            {
                _coordinateMapper = _sensor.CoordinateMapper;
                _sensor.Open(); // 키넥트 전원 킴

                if (_sensor.IsOpen) _bodyFrameReader = _sensor.BodyFrameSource.OpenReader();
            }
        }

        // Stop Kinect Function(Motion Games)
        public void ReleaseDefaultSesor()
        {
            if (_bodyFrameReader != null)
            {
                _bodyFrameReader.Dispose();
                _bodyFrameReader = null;
            }

            if (_sensor != null)
            {
                _sensor.Close();
                _sensor = null;
            }
        }

        private void HandleSocketException(SocketException e, out bool closeSocket)
        {
            closeSocket = false;

            if (e != null)
            {
                switch (e.ErrorCode)
                {
                    case WSAEINTR:
                    case WSAECONNABORTED:
                        Debug.Log("Socket exception - error code : " + e.ErrorCode);
                        Debug.LogException(e);
                        closeSocket = true;
                        break;
                    case WSAEWOULDBLOCK:
                        break;
                }
            }
        }
        // Use this for initialization
        void Start()
        {
            Loom.Initialize();

            _isSendPakcet = false;
            ReleaseDefaultSesor();
            InitializeDefaultSensor();
        }
        
        
        void OnEnable()
        {
            InitializeDefaultSensor();
        }

        void OnDisable()
        {
            ReleaseDefaultSesor();
            Disconnect();
        }

#if UNITY_EDITOR
        void OnGUI()
        {

            if (_jumpData == null)
                return;

            if (_jumpData[2] == null)
                return;

            //GUIStyle guiStyle = new GUIStyle(GUI.skin.textField);
            //guiStyle.alignment = TextAnchor.MiddleCenter;

            //GUI.BeginGroup(new Rect(0,0, Screen.width, Screen.height));
            //GUI.TextField(new Rect(10,10,200,20),_jumpData[2].inUser.ToString(), guiStyle);
            //GUI.TextField(new Rect(Screen.width - 210,10,200,20), _jumpData[2].command, guiStyle);
            //GUI.EndGroup();
        }
#endif

        // Update is called once per frame
        void Update()
        {
            // 키보드의 K를 통해 키넥트를 On/Off 시킬 수 있음
            if (Input.GetKeyDown(KeyCode.K))
            {
                if (_sensor != null && _sensor.IsOpen)
                {
                    ReleaseDefaultSesor();
                }
                else
                {
                    InitializeDefaultSensor();
                }
            }
            else if (Input.GetKeyDown(KeyCode.J))
            {
                // J키로 점프점프 상태로 게임타입 설정
                _gameType = GameType.JumpJump;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                SetDefaultData(6);
            }

            // 게임 타입이 설정되어 있지 않으면 실행할 필요 없음.
            if (_gameType == GameType.None)
                return;

            // 키넥트가 구동되기 전에 Update문에 들어오는 것을 방지하기 위해 bodyFrameReader Null Check
            if (_bodyFrameReader == null)
                return;

            // 키넥트의 현재 프레임의 body Data를 할당하는 부분
            using (var bodyFrame = _bodyFrameReader.AcquireLatestFrame())
            {
                if (bodyFrame != null)
                {
                    if (_bodyDataArr == null)
                        _bodyDataArr = new Body[_sensor.BodyFrameSource.BodyCount]; // body Data를 저장할 배열객체가 할당되어 있지 않을 경우 할당

                    bodyFrame.GetAndRefreshBodyData(_bodyDataArr);  // 현재 프레임의 body Data 복제(할당)
                }
            }

            if (_gameType == GameType.JumpJump)
                ProcessJumpJump();
        }

        public void Connect(string IP, int port, System.Action<bool> connectionHandler)
        {
            if (_socket == null)
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.SendTimeout = 1000;
                _socket.NoDelay = true;
                _socket.ReceiveBufferSize = 65536;
                _socket.SendBufferSize = 65536;
            }

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["socket"] = _socket;
            dict["handler"] = connectionHandler;

            // begin connect
            System.IAsyncResult ar = _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(IP), port), new System.AsyncCallback(OnConnect), dict);
        }

        private void OnConnect(System.IAsyncResult ar)
        {
            Dictionary<string, object> dict = ar.AsyncState as Dictionary<string, object>;
            if (dict != null)
            {
                _isConnecting = false;

                Socket socket = null;
                System.Action<bool> connectionHandler = null;

                if (dict.ContainsKey("socket") && dict["socket"] != null)
                {
                    socket = dict["socket"] as Socket;
                }

                if (dict.ContainsKey("handler") && dict["handler"] != null)
                {
                    connectionHandler = dict["handler"] as System.Action<bool>;
                }

                if (socket != null)
                {
                    _isConnected = socket.Connected;

                    // Check if the thread is allocated
                    if (_thread != null)
                    {
                        _thread.Abort();
                        _thread = null;
                    }

                    _thread = new Thread(new ThreadStart(SocketReadWriteLoop));

                    // Run thread loop
                    try
                    {
                        _thread.Start();
                    }
                    catch (System.Exception e)
                    {
                        Loom.QueueOnMainThread(() =>
                                               {
                                                   Debug.LogException(e);
                                               });
                    }
                    // Handle connection state, print the log
                    if (_isConnected)
                    {
                        // success log
                        Loom.QueueOnMainThread(() =>
                                               {
                                                   Debug.Log(string.Format("EASSocket - Connection Success!"));
                                               });
                    }
                    else
                    {
                        // failed log
                        Loom.QueueOnMainThread(() =>
                                               {
                                                   Debug.Log("EASSocket - Connection failed.");
                                               });

                        // close and release the socket
                        _socket.Close();
                        _socket = null;
                    }

                    // Send connection result to handler
                    if (connectionHandler != null)
                    {
                        Loom.QueueOnMainThread(() =>
                                               {
                                                   connectionHandler(_isConnected);
                                               });
                    }

                    socket.EndConnect(ar);
                }
            }
        }

        private void SocketReadWriteLoop()
        {
            while (_isConnected)
            {
                try
                {
                    //Loom.QueueOnMainThread(() =>
                    //{
                    if (_buffer == null) continue;
                    System.Array.Clear(_buffer, 0, _buffer.Length);
                    int length = _socket.Receive(_buffer, 0, JumpJumpPacketLength, SocketFlags.None);

                    if (length == 506)
                    {
                        ParsePacket(_buffer);
                    }
                    //});
                }
                catch (SocketException ex)
                {
                    bool closeSocket = false;
                    HandleSocketException(ex, out closeSocket);
                    if (closeSocket)
                    {

                    }
                }
            }
        }

        private void ParsePacket(byte[] packets)
        {
            if (packets[0] == 0x00)
            {
                _jumpData[2].inUser = false;
                _jumpData[2].isJump = false;
                _jumpData[2].command = GAME_MESSAGE.CENTER;
            }
            else if (packets[0] == 0x01)
            {
                _jumpData[2].inUser = true;
                Vector3[] body = new Vector3[20];
                Vector3 shoulder = new Vector3();
                for (int i = 0; i < 20; i++)
                {
                    int index = 1 + 12 * i;
                    body[i].x = System.BitConverter.ToSingle(packets, index + 0);
                    body[i].y = System.BitConverter.ToSingle(packets, index + 4);
                    body[i].z = System.BitConverter.ToSingle(packets, index + 8);
                }
                shoulder.x = System.BitConverter.ToSingle(packets, 241 + 0);
                shoulder.y = System.BitConverter.ToSingle(packets, 241 + 4);
                shoulder.z = System.BitConverter.ToSingle(packets, 241 + 8);
                AnalysisJumpJumpMotionSub(2, body, shoulder);
            }

            if (packets[253] == 0x00)
            {
                _jumpData[3].inUser = false;
                _jumpData[3].isJump = false;
                _jumpData[3].command = GAME_MESSAGE.CENTER;
            }
            else if (packets[253] == 0x01)
            {
                _jumpData[3].inUser = true;

                Vector3[] body = new Vector3[20];
                Vector3 shoulder = new Vector3();
                for (int i = 0; i < 20; i++)
                {
                    int index = 254 + 12 * i;
                    body[i].x = System.BitConverter.ToSingle(packets, index + 0);
                    body[i].y = System.BitConverter.ToSingle(packets, index + 4);
                    body[i].z = System.BitConverter.ToSingle(packets, index + 8);
                }
                shoulder.x = System.BitConverter.ToSingle(packets, 494 + 0);
                shoulder.y = System.BitConverter.ToSingle(packets, 494 + 4);
                shoulder.z = System.BitConverter.ToSingle(packets, 494 + 8);
                AnalysisJumpJumpMotionSub(3, body, shoulder);
            }
        }
        public void Disconnect()
        {
            if (_isConnected && !_isConnecting && !_isDisconnecting)
            {
                _isDisconnecting = true;

                if (_socket != null)
                {
                    if (!_host)
                    {
                        try
                        {
                            _socket.Shutdown(SocketShutdown.Both);
                        }
                        catch (SocketException e)
                        {
                            Debug.Log("EASSocket - exception occured doing shutdown.");
                            Debug.LogException(e);
                        }
                    }
                    _socket.Close();
                    _socket = null;
                }

                if (_thread != null)
                {
                    _thread.Abort();
                    _thread = null;
                }

                _isDisconnecting = false;
                _isConnected = false;
                _host = false;

                Loom.QueueOnMainThread(() =>
                                       {
                                           Debug.Log("EASSocket - Disconnected.");
                                       });
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////// JumpJump Game Functions Start ////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ProcessJumpJump()
        {

            if (!_isSendPakcet && _socket != null)
            {
                if (_socket.Connected == true)
                {
                    try
                    {
                        byte[] packet = new byte[12];

                        int contentsNum = (int)ContentsType.JumpJump;
                        int motionNum = 0;
                        int imageNum = 0;

                        byte[] p1 = BitConverter.GetBytes(contentsNum);
                        byte[] p2 = BitConverter.GetBytes(motionNum);
                        byte[] p3 = BitConverter.GetBytes(imageNum);

                        Array.Copy(p1, 0, packet, 0, 4);
                        Array.Copy(p2, 0, packet, 4, 4);
                        Array.Copy(p3, 0, packet, 8, 4);

                        _socket.Send(packet);

                        _isSendPakcet = true;
                    }
                    catch (Exception ex)
                    {
                        _isSendPakcet = false;
                    }
                }
            }
            if (_bodyDataArr == null)
                return;

            int index1P = -1;
            int index2P = -1;

            float left1P = Constants.DEFAULT_DEPTH_WIDTH / 8.0f;
            float right1P = Constants.DEFAULT_DEPTH_WIDTH * 3 / 8.0f;
            float left2P = Constants.DEFAULT_DEPTH_WIDTH * 5 / 8.0f;
            float right2P = Constants.DEFAULT_DEPTH_WIDTH * 7 / 8.0f;

            float minDepth1P = 999;
            float minDepth2P = 999;

            for (int i = 0; i < _bodyDataArr.Length; i++)
            {
                CameraSpacePoint pCameraMap = _bodyDataArr[i].Joints[JointType.SpineBase].Position;

                DepthSpacePoint pDepthMap = _coordinateMapper.MapCameraPointToDepthSpace(pCameraMap);

                if (pCameraMap.Z > Constants.FILTER_DISTANCE) continue;

                if (pDepthMap.X > left1P && pDepthMap.X < right1P)
                {
                    if (pCameraMap.Z < minDepth1P)
                    {
                        minDepth1P = pCameraMap.Z;
                        index1P = i;
                    }
                }
                else if (pDepthMap.X > left2P && pDepthMap.X < right2P)
                {
                    if (pCameraMap.Z < minDepth2P)
                    {
                        minDepth2P = pCameraMap.Z;
                        index2P = i;
                    }
                }
            }

            Body body1 = null;
            Body body2 = null;
            if (index1P != -1)
            {
                body1 = _bodyDataArr[index1P];

                DepthSpacePoint pShoulder = _coordinateMapper.MapCameraPointToDepthSpace(body1.Joints[JointType.SpineShoulder].Position);
                DepthSpacePoint pMid = _coordinateMapper.MapCameraPointToDepthSpace(body1.Joints[JointType.SpineMid].Position);

                int distance = (int)(pMid.Y - pShoulder.Y);
                print($"1플레이어 팔 움직일 거리{distance}");
                _jumpData[0].SetFlappingDistance(distance);
            }

            if (index2P != -1)
            {
                body2 = _bodyDataArr[index2P];

                DepthSpacePoint pShoulder = _coordinateMapper.MapCameraPointToDepthSpace(body2.Joints[JointType.SpineShoulder].Position);
                DepthSpacePoint pMid = _coordinateMapper.MapCameraPointToDepthSpace(body2.Joints[JointType.SpineMid].Position);

                int distance = (int)(pMid.Y - pShoulder.Y);
                print($"2플레이어 팔 움직일 거리{distance}");
                _jumpData[1].SetFlappingDistance(distance);
            }

            AnalysisJumpJumpMotionMain(0, body1);
            AnalysisJumpJumpMotionMain(1, body2);
        }

        private void AnalysisJumpJumpMotionMain(int playerNum, Body player)
        {
            if (player == null)
            {
                _jumpData[playerNum].inUser = false;
                _jumpData[playerNum].isJump = false;
                _jumpData[playerNum].command = GAME_MESSAGE.CENTER;
                return;
            }
            else
            {
                _jumpData[playerNum].inUser = true;

                Vector3[] pos = new Vector3[20];
                foreach (JointType jointType in player.Joints.Keys)
                {
                    if ((int)jointType < pos.Length)
                    {
                        var p = player.Joints[jointType].Position;
                        pos[(int)jointType] = new Vector3(-p.X, p.Y, -p.Z);
                    }
                }
                _jumpData[playerNum].body = pos;

                DepthSpacePoint pHandLeft = _coordinateMapper.MapCameraPointToDepthSpace(player.Joints[JointType.HandLeft].Position);
                DepthSpacePoint pElbowLeft = _coordinateMapper.MapCameraPointToDepthSpace(player.Joints[JointType.ElbowLeft].Position);
                DepthSpacePoint pShoulderLeft = _coordinateMapper.MapCameraPointToDepthSpace(player.Joints[JointType.ShoulderLeft].Position);
                DepthSpacePoint pShoulderRight = _coordinateMapper.MapCameraPointToDepthSpace(player.Joints[JointType.ShoulderRight].Position);
                DepthSpacePoint pElbowRight = _coordinateMapper.MapCameraPointToDepthSpace(player.Joints[JointType.ElbowRight].Position);
                DepthSpacePoint pHandRight = _coordinateMapper.MapCameraPointToDepthSpace(player.Joints[JointType.HandRight].Position);

                if (pHandLeft.X < pElbowLeft.X && pElbowLeft.X < pShoulderLeft.X && pShoulderRight.X < pElbowRight.X && pElbowRight.X < pHandRight.X)
                {
                    if (_jumpData[playerNum].LeftHandArr.Count >= Constants.MAX_TRACKING_FRAME)
                    {
                        _jumpData[playerNum].LeftHandArr.RemoveAt(0);
                        _jumpData[playerNum].RightHandArr.RemoveAt(0);
                    }
                    _jumpData[playerNum].LeftHandArr.Add(pHandLeft);
                    _jumpData[playerNum].RightHandArr.Add(pHandRight);
                }
                else
                {
                    _jumpData[playerNum].LeftHandArr.Clear();
                    _jumpData[playerNum].RightHandArr.Clear();
                }
                print($"점프 데이터{_jumpData[playerNum].LeftHandArr.Count}{_jumpData[playerNum].RightHandArr.Count}");
                if (_jumpData[playerNum].LeftHandArr.Count >= Constants.MIN_TRACKING_FRAME)
                {
                    print("점프 최소 기준 충족");
                    int leftIndex = 0;
                    int rightIndex = 0;

                    int leftDownCount = 0;
                    int rightDownCount = 0;

                    for (int i = _jumpData[playerNum].LeftHandArr.Count - 1; i >= 3; i -= 3)
                    {
                        if ((_jumpData[playerNum].LeftHandArr[i].Y > _jumpData[playerNum].LeftHandArr[i - 3].Y))
                        {
                            if (leftDownCount == 0) leftIndex = i;
                            leftDownCount++;
                        }
                        else leftDownCount = 0;

                        if ((_jumpData[playerNum].RightHandArr[i].Y > _jumpData[playerNum].RightHandArr[i - 3].Y))
                        {
                            if (rightDownCount == 0) rightIndex = i;
                            rightDownCount++;
                        }
                        else rightDownCount = 0;

                        int leftCnt, rightCnt;
                        leftCnt = rightCnt = Constants.MIN_TRACKING_FRAME / 2;
                        //if (ScreenManager.currentScreenType == ScreenType.Bigboard2x2)
                        leftCnt = rightCnt = 1;
                        
                        //if (leftDownCount > leftCnt && rightDownCount > rightCnt)
                        //if (leftDownCount > Constants.MIN_TRACKING_FRAME / 2 && rightDownCount > Constants.MIN_TRACKING_FRAME / 2)
                        {
                            //  Debug.LogError("id : " + playerNum + ", handLeftDistance : " + _jumpData[playerNum].GetFlappingDistance() + ", handRightDistance : " + _jumpData[playerNum].GetFlappingDistance());
                            
                            if (_jumpData[playerNum].LeftHandArr[leftIndex].Y - _jumpData[playerNum].LeftHandArr[i - 2].Y > _jumpData[playerNum].GetFlappingDistance() *0.1f
                               && _jumpData[playerNum].RightHandArr[rightIndex].Y - _jumpData[playerNum].RightHandArr[i - 2].Y > _jumpData[playerNum].GetFlappingDistance() *0.1f)
                            {

                                //        Debug.LogError("id : " + playerNum + ", JUMP!!");

                                _jumpData[playerNum].LeftHandArr.Clear();
                                _jumpData[playerNum].RightHandArr.Clear();

                                _jumpData[playerNum].isJump = true;
                                break;
                            }
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.S))
                    _jumpData[playerNum].isJump = true;

                DepthSpacePoint pSpineShoulder = _coordinateMapper.MapCameraPointToDepthSpace(player.Joints[JointType.SpineShoulder].Position);
                DepthSpacePoint pSpineBase = _coordinateMapper.MapCameraPointToDepthSpace(player.Joints[JointType.SpineBase].Position);

                float trWidth = Mathf.Abs(pSpineShoulder.X - pSpineBase.X);
                float trHeight = Mathf.Abs(pSpineShoulder.Y - pSpineBase.Y);

                float angle = Mathf.Atan(trHeight / trWidth);
                angle = 180 * angle / Mathf.PI;

                if (Constants.VERTICAL_ANGLE - Constants.DIRECTION_ANGLE > angle)
                {
                    if (pSpineShoulder.X < pSpineBase.X)
                        _jumpData[playerNum].command = GAME_MESSAGE.LEFT;
                    else
                        _jumpData[playerNum].command = GAME_MESSAGE.RIGHT;
                }
                else
                {
                    _jumpData[playerNum].command = GAME_MESSAGE.CENTER;
                }
            }
        }

        private void AnalysisJumpJumpMotionSub(int playerNum, Vector3[] player, Vector3 shoulder)
        {
            CameraSpacePoint[] csPoints = new CameraSpacePoint[20];

            for (int i = 0; i < player.Length; i++)
            {
                CameraSpacePoint cPoint = new CameraSpacePoint();

                cPoint.X = player[i].x;
                cPoint.Y = player[i].y;
                cPoint.Z = player[i].z;

                csPoints[i] = cPoint;
            }
            DepthSpacePoint pHandLeft = _coordinateMapper.MapCameraPointToDepthSpace(csPoints[(int)JointType.HandLeft]);
            DepthSpacePoint pElbowLeft = _coordinateMapper.MapCameraPointToDepthSpace(csPoints[(int)JointType.ElbowLeft]);
            DepthSpacePoint pShoulderLeft = _coordinateMapper.MapCameraPointToDepthSpace(csPoints[(int)JointType.ShoulderLeft]);
            DepthSpacePoint pShoulderRight = _coordinateMapper.MapCameraPointToDepthSpace(csPoints[(int)JointType.ShoulderRight]);
            DepthSpacePoint pElbowRight = _coordinateMapper.MapCameraPointToDepthSpace(csPoints[(int)JointType.ElbowRight]);
            DepthSpacePoint pHandRight = _coordinateMapper.MapCameraPointToDepthSpace(csPoints[(int)JointType.HandRight]);

            if (pHandLeft.X < pElbowLeft.X && pElbowLeft.X < pShoulderLeft.X && pShoulderRight.X < pElbowRight.X && pElbowRight.X < pHandRight.X)
            {
                if (_jumpData[playerNum].LeftHandArr.Count >= Constants.MAX_TRACKING_FRAME)
                {
                    _jumpData[playerNum].LeftHandArr.RemoveAt(0);
                    _jumpData[playerNum].RightHandArr.RemoveAt(0);
                }
                _jumpData[playerNum].LeftHandArr.Add(pHandLeft);
                _jumpData[playerNum].RightHandArr.Add(pHandRight);
            }
            else
            {
                _jumpData[playerNum].LeftHandArr.Clear();
                _jumpData[playerNum].RightHandArr.Clear();
            }

            if (_jumpData[playerNum].LeftHandArr.Count >= Constants.MIN_TRACKING_FRAME)
            {
                int leftIndex = 0;
                int rightIndex = 0;

                int leftDownCount = 0;
                int rightDownCount = 0;

                for (int i = _jumpData[playerNum].LeftHandArr.Count - 1; i >= 2; i -= 2)
                {
                    if ((_jumpData[playerNum].LeftHandArr[i].Y > _jumpData[playerNum].LeftHandArr[i - 2].Y))
                    {
                        if (leftDownCount == 0) leftIndex = i;
                        leftDownCount++;
                    }
                    else leftDownCount = 0;

                    if ((_jumpData[playerNum].RightHandArr[i].Y > _jumpData[playerNum].RightHandArr[i - 2].Y))
                    {
                        if (rightDownCount == 0) rightIndex = i;
                        rightDownCount++;
                    }
                    else rightDownCount = 0;

                    if (leftDownCount > Constants.MIN_TRACKING_FRAME / 2 && rightDownCount > Constants.MIN_TRACKING_FRAME / 2)
                    {
                        if (_jumpData[playerNum].LeftHandArr[leftIndex].Y - _jumpData[playerNum].LeftHandArr[i - 2].Y > _jumpData[playerNum].GetFlappingDistance()
                           && _jumpData[playerNum].RightHandArr[rightIndex].Y - _jumpData[playerNum].RightHandArr[i - 2].Y > _jumpData[playerNum].GetFlappingDistance())
                        {
                            _jumpData[playerNum].LeftHandArr.Clear();
                            _jumpData[playerNum].RightHandArr.Clear();

                            _jumpData[playerNum].isJump = true;
                            break;
                        }
                    }
                }
            }
            CameraSpacePoint shoulderPoint = new CameraSpacePoint();
            shoulderPoint.X = shoulder.x;
            shoulderPoint.Y = shoulder.y;
            shoulderPoint.Z = shoulder.z;

            DepthSpacePoint pSpineShoulder = _coordinateMapper.MapCameraPointToDepthSpace(shoulderPoint);
            DepthSpacePoint pSpineBase = _coordinateMapper.MapCameraPointToDepthSpace(csPoints[(int)JointType.SpineBase]);

            float trWidth = Mathf.Abs(pSpineShoulder.X - pSpineBase.X);
            float trHeight = Mathf.Abs(pSpineShoulder.Y - pSpineBase.Y);

            float angle = Mathf.Atan(trHeight / trWidth);
            angle = 180 * angle / Mathf.PI;

            if (Constants.VERTICAL_ANGLE - Constants.DIRECTION_ANGLE > angle)
            {
                if (pSpineShoulder.X < pSpineBase.X)
                    _jumpData[playerNum].command = GAME_MESSAGE.LEFT;
                else
                    _jumpData[playerNum].command = GAME_MESSAGE.RIGHT;
            }
            else
            {
                _jumpData[playerNum].command = GAME_MESSAGE.CENTER;
            }

            for (int i = 0; i < 20; i++)
            {
                player[i].x *= -1;
                player[i].z *= -1;
            }

            _jumpData[playerNum].body = player;
        }
    }
}