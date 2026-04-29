using UnityEngine;
using ProtoTurtle.BitmapDrawing;
using Windows.Kinect;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.UI;

namespace ML.PlaywallKids.Interaction
{
    using Common;
    //플레이월 키즈 키넥트 활용 콘텐츠 여기서 사람 동작 인식...! > 세계명소체험, 스케이팅은 KinectHelper.cs라는 스크립트로 키넥트 사용..
    public class CoordinateMapperView : MonoBehaviour
    {
        //최대 사람 숫자
        [HideInInspector]
        public int maxUserCount;

        // Member Variable
        public bool _isDebug = true;
        public byte alphaOpacity = 255;         // 알파값 0~255
        public byte particleOpacity = 255;
        public int _dividedRatio = 3;
        public int _oldDividedRatio = 0;
        //
        public MotionType _motionType = MotionType.None;
        public ScreenType _screenType = ScreenType.None;

        private ImageType _imageType = ImageType.None;
        private InputType _inputType = InputType.None;

        private Texture2D renderTexture;
        private byte[] _outputImageArray;
        private int _imageSingleWidth;
        private int _imageSingleHeight;
        private int _imageFullWidth;
        private int _imageFullHeight;
        private int[] _nSideMatrix = null;

        public float imageScale = 0.5f;

        public UITexture uiTexture;
        [SerializeField]
        RawImage image;
        public GameObject CoordinateMapperManager;

        public CoordinateMapperEffect CoordinateMapperEffect;

        private CoordinateMapper _coordinateMapper;
        private CoordinateMapperManager _coordinateMapperManager;
        private DepthSpacePoint[] _depthCoordinatesArray;
        private Body[] _bodyDataArrray;
        private byte[] _colorBufferArray;
        private byte[] _bodyIndexPointsArray;
        private ushort[] _depthBufferArray;

        private UserData[] _userDataArray = null;
        private byte[] _receiveBuffers;
        private int _receiveLength = 0;

        void Start()
        {
            InitializeObjects();
            Loom.Initialize();
        }

        #region Init Func
        public void InitializeObjects()
        {
            ReleaseBuffers();

            _motionType = MotionType.None;
            _screenType = ScreenUtil.screenType;

            if (CoordinateMapperManager == null) return;

            if (_userDataArray == null) _userDataArray = new UserData[6] { new UserData(), new UserData(), new UserData(), new UserData(), new UserData(), new UserData() };

            _coordinateMapperManager = CoordinateMapperManager.GetComponent<CoordinateMapperManager>();
            _coordinateMapper = _coordinateMapperManager.GetCoordinateMapper();
            _depthCoordinatesArray = _coordinateMapperManager.GetDepthCoordinates();
            _colorBufferArray = _coordinateMapperManager.GetColorImageBuffer();
            _bodyIndexPointsArray = _coordinateMapperManager.GetBodyIndexBuffer();
            _depthBufferArray = _coordinateMapperManager.GetDepthBuffer();
        }
        #endregion


        private Vector2 GetSize(float width, float height, Vector2? defaultRatio = null)
        {
            Vector2 ratio = new Vector2(ScreenUtil.aspectRatio, 1.0f) * 9.0f;
            if (defaultRatio == null)
                defaultRatio = new Vector2(Constants.DEFAULT_RATIO_WIDTH, Constants.DEFAULT_RATIO_HEIGHT);
            return new Vector2((int)((width / defaultRatio.Value.x) * ratio.x), (int)((height / defaultRatio.Value.y) * ratio.y));
        }

        public void SetImageType(ImageType imgType)
        {
            _imageType = imgType;
            _receiveBuffers = null;
            _receiveLength = 0;

            if (_imageType == ImageType.Chromakey)  // 크로마키
            {
                Vector2 color_size = GetSize(Constants.DEFAULT_COLOR_WIDTH, Constants.DEFAULT_COLOR_HEIGHT);

                _oldDividedRatio = _dividedRatio;

                _imageSingleWidth = _imageFullWidth = (int)(color_size.x / _dividedRatio);
                _imageSingleHeight = _imageFullHeight = (int)(color_size.y / _dividedRatio);

                _inputType = InputType.Color;
            }
            else if (_imageType == ImageType.Original) // 사용 되지 않음?
            {
                Vector2 default_size = GetSize(Constants.DEFAULT_COLOR_WIDTH, Constants.DEFAULT_COLOR_HEIGHT);

                _imageSingleWidth = _imageFullWidth = (int)default_size.x * 2;
                _imageSingleHeight = _imageFullHeight = (int)default_size.y * 2;
            }
            else if (_imageType != ImageType.None)  // 파티클, 쉐도우
            {
                Vector2 depth_size = GetSize(Constants.DEFAULT_DEPTH_WIDTH, Constants.DEFAULT_DEPTH_HEIGHT, new Vector2(Constants.DEFAULT_DEPTH_RATIO_WIDTH, Constants.DEFAULT_DEPTH_RATIO_HEIGHT));

                _imageSingleWidth = _imageFullWidth = (int)depth_size.x;
                _imageSingleHeight = _imageFullHeight = (int)depth_size.y;

                _inputType = InputType.Depth;
            }

            if (_screenType == ScreenType.Bigboard2x6)
            {
                _imageSingleWidth = (int)(_imageFullWidth / 2f);
                _imageSingleHeight = _imageFullHeight;

                int contentsNum = (int)ContentsType.Interaction;
                int motionNum = (int)_motionType;
                int imageNum = (int)_imageType;

                byte[] p1 = BitConverter.GetBytes(contentsNum);
                byte[] p2 = BitConverter.GetBytes(motionNum);
                byte[] p3 = BitConverter.GetBytes(imageNum);

                byte[] packet = new byte[12];

                Array.Copy(p1, 0, packet, 0, 4);
                Array.Copy(p2, 0, packet, 4, 4);
                Array.Copy(p3, 0, packet, 8, 4);
            }

            renderTexture = new Texture2D(_imageFullWidth, _imageFullHeight, TextureFormat.RGBA32, false);                            // false가 먼지 모르겠음.....
            renderTexture.wrapMode = TextureWrapMode.Repeat;

            if (uiTexture != null && renderTexture != null)
            {
                Rect uvRect = new Rect(0, 0, 1, -1);
                uiTexture.uvRect = uvRect;
                uiTexture.mainTexture = renderTexture;

                uiTexture.material.SetInt("width", _imageFullWidth);                     // Custom Shader에서 그리기 위한 패널 가로 길이 설정
                uiTexture.material.SetInt("height", _imageFullHeight);                   // Custom Shader에서 그리기 위한 패널 세로 길이 설정
            }

            if(image != null &&renderTexture != null)
            {
                Rect uvRect = new Rect(0, 0, 1, -1);
                image.uvRect = uvRect;
                image.texture = renderTexture;
                image.material.SetInt("width", _imageFullWidth); //
                image.material.SetInt("height", _imageFullHeight); //
            }

            _outputImageArray = new byte[_imageFullWidth * _imageFullHeight * 4];

            _nSideMatrix = new int[4];

            _nSideMatrix[0] = -(_imageFullWidth * 4) - 4;
            _nSideMatrix[1] = -(_imageFullWidth * 4);
            _nSideMatrix[2] = -4;
            _nSideMatrix[3] = 0;

        }
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                SetImageType((ImageType)((int)++_imageType % 5));
            }
            if (_screenType == ScreenType.None || _imageType == ImageType.None || _motionType == MotionType.None) return;
            if (_imageType == ImageType.Chromakey)
            {
                if (_oldDividedRatio != _dividedRatio) SetImageType(_imageType);
            }

            if (_coordinateMapperManager.IsSensorOpened())
            {
                ProcessFrame();         // 키넥트의 현재 프레임 정보를 가져옴
                CalcUserPosition();
                MakeOutputImage();
                CheckMotion();
            }

            renderTexture.LoadRawTextureData(_outputImageArray);
            renderTexture.Apply();
        }

        private void ProcessFrame()
        {
            var depthData = GCHandle.Alloc(_depthBufferArray, GCHandleType.Pinned);
            var depthCoordinatesData = GCHandle.Alloc(_depthCoordinatesArray, GCHandleType.Pinned);

            if (_coordinateMapper != null)
            {
                _coordinateMapper.MapColorFrameToDepthSpaceUsingIntPtr(
                    depthData.AddrOfPinnedObject(),
                    (uint)_depthBufferArray.Length * sizeof(ushort),
                    depthCoordinatesData.AddrOfPinnedObject(),
                    (uint)_depthCoordinatesArray.Length);
            }

            _bodyDataArrray = _coordinateMapperManager.GetBodyDataBuffer();

            depthCoordinatesData.Free();
            depthData.Free();
        }

        private void CalcUserPosition()
        {
            if (_bodyDataArrray == null) return;

            int userCount = 0;

            for (int i = 0; i < _bodyDataArrray.Length; i++)
            {
                try
                {
                    if (_bodyDataArrray[i] == null) continue;

                    if (_bodyDataArrray[i].IsTracked)
                    {
                        userCount++;

                        ColorSpacePoint pColor = _coordinateMapper.MapCameraPointToColorSpace(_bodyDataArrray[i].Joints[JointType.SpineBase].Position);

                        int xColor = (int)pColor.X;
                        int yColor = (int)pColor.Y;

                        int index = xColor + yColor * Constants.DEFAULT_COLOR_WIDTH;
                        if (index >= _depthCoordinatesArray.Length || index < 0) continue;

                        if ((!float.IsInfinity(_depthCoordinatesArray[index].X) && !float.IsNaN(_depthCoordinatesArray[index].X) && _depthCoordinatesArray[index].X != 0) ||
                            (!float.IsInfinity(_depthCoordinatesArray[index].Y) && !float.IsNaN(_depthCoordinatesArray[index].Y) && _depthCoordinatesArray[index].Y != 0))
                        {
                            float player = _bodyIndexPointsArray[(int)_depthCoordinatesArray[index].X + (int)(_depthCoordinatesArray[index].Y * Constants.DEFAULT_DEPTH_WIDTH)];

                            double xRatio = 0;
                            int xShift = 0;
                            if (_inputType == InputType.Color)
                            {
                                if (xColor < Constants.DEFAULT_COLOR_WIDTH / 2)
                                {
                                    xRatio = (float)((Constants.DEFAULT_COLOR_WIDTH / 2) - xColor) / (Constants.DEFAULT_COLOR_WIDTH / 2);
                                    xShift = (int)(_imageSingleWidth * _dividedRatio / 2 * xRatio);
                                    xShift = (Constants.DEFAULT_COLOR_WIDTH / 2 - xColor) - xShift + Math.Abs(Constants.DEFAULT_COLOR_WIDTH - _imageSingleWidth * _dividedRatio) / 2;
                                }
                                else
                                {
                                    xRatio = (float)(xColor - (Constants.DEFAULT_COLOR_WIDTH / 2)) / (Constants.DEFAULT_COLOR_WIDTH / 2);
                                    xShift = (int)(_imageSingleWidth * _dividedRatio / 2 * xRatio);
                                    xShift = xShift - (xColor - Constants.DEFAULT_COLOR_WIDTH / 2) + Math.Abs(Constants.DEFAULT_COLOR_WIDTH - _imageSingleWidth * _dividedRatio) / 2;
                                }
                            }
                            else if (_inputType == InputType.Depth)
                            {
                                if ((int)_depthCoordinatesArray[index].X < Constants.DEFAULT_DEPTH_WIDTH / 2)
                                {
                                    xRatio = (float)((Constants.DEFAULT_DEPTH_WIDTH / 2) - (int)_depthCoordinatesArray[index].X) / (Constants.DEFAULT_DEPTH_WIDTH / 2);
                                    xShift = (int)(_imageSingleWidth / 2 * xRatio);
                                    xShift = (Constants.DEFAULT_DEPTH_WIDTH / 2 - (int)_depthCoordinatesArray[index].X) - xShift + Math.Abs(Constants.DEFAULT_DEPTH_WIDTH - _imageSingleWidth) / 2;
                                }
                                else
                                {
                                    xRatio = (float)((int)_depthCoordinatesArray[index].X - (Constants.DEFAULT_DEPTH_WIDTH / 2)) / (Constants.DEFAULT_DEPTH_WIDTH / 2);
                                    xShift = (int)(_imageSingleWidth / 2 * xRatio);
                                    xShift = xShift - ((int)_depthCoordinatesArray[index].X - Constants.DEFAULT_DEPTH_WIDTH / 2) + Math.Abs(Constants.DEFAULT_DEPTH_WIDTH - _imageSingleWidth) / 2;
                                }
                            }

                            if (_userDataArray[i].userID != player) _userDataArray[i].ClearDepthData();
                            _userDataArray[i].userID = player;
                            _userDataArray[i].shiftValue = xShift;
                            _userDataArray[i].joints = _bodyDataArrray[i].Joints;

                            Dictionary<JointType, Windows.Kinect.Joint> joints = _userDataArray[i].joints;

                            if (_inputType == InputType.Color)
                            {
                                ColorSpacePoint pHandLeft = _coordinateMapper.MapCameraPointToColorSpace(joints[JointType.HandLeft].Position);
                                ColorSpacePoint pHandRight = _coordinateMapper.MapCameraPointToColorSpace(joints[JointType.HandRight].Position);
                                ColorSpacePoint pElbowLeft = _coordinateMapper.MapCameraPointToColorSpace(joints[JointType.ElbowLeft].Position);
                                ColorSpacePoint pElbowRight = _coordinateMapper.MapCameraPointToColorSpace(joints[JointType.ElbowRight].Position);
                                ColorSpacePoint pShoulderLeft = _coordinateMapper.MapCameraPointToColorSpace(joints[JointType.ShoulderLeft].Position);
                                ColorSpacePoint pShoulderRight = _coordinateMapper.MapCameraPointToColorSpace(joints[JointType.ShoulderRight].Position);
                                ColorSpacePoint pHead = _coordinateMapper.MapCameraPointToColorSpace(joints[JointType.Head].Position);
                                ColorSpacePoint pFootLeft = _coordinateMapper.MapCameraPointToColorSpace(joints[JointType.FootLeft].Position);
                                ColorSpacePoint pFootRight = _coordinateMapper.MapCameraPointToColorSpace(joints[JointType.FootRight].Position);

                                _userDataArray[i].pHandLeft.x = (pHandLeft.X + _userDataArray[i].shiftValue) / _dividedRatio;
                                _userDataArray[i].pHandLeft.y = _imageSingleHeight - (pHandLeft.Y / _dividedRatio);

                                _userDataArray[i].pHandRight.x = (pHandRight.X + _userDataArray[i].shiftValue) / _dividedRatio;
                                _userDataArray[i].pHandRight.y = _imageSingleHeight - (pHandRight.Y / _dividedRatio);

                                _userDataArray[i].pElbowLeft.x = (pElbowLeft.X + _userDataArray[i].shiftValue) / _dividedRatio;
                                _userDataArray[i].pElbowLeft.y = _imageSingleHeight - (pElbowLeft.Y / _dividedRatio);

                                _userDataArray[i].pElbowRight.x = (pElbowRight.X + _userDataArray[i].shiftValue) / _dividedRatio;
                                _userDataArray[i].pElbowRight.y = _imageSingleHeight - (pElbowRight.Y / _dividedRatio);

                                _userDataArray[i].pShoulderLeft.x = (pShoulderLeft.X + _userDataArray[i].shiftValue) / _dividedRatio;
                                _userDataArray[i].pShoulderLeft.y = _imageSingleHeight - (pShoulderLeft.Y / _dividedRatio);

                                _userDataArray[i].pShoulderRight.x = (pShoulderRight.X + _userDataArray[i].shiftValue) / _dividedRatio;
                                _userDataArray[i].pShoulderRight.y = _imageSingleHeight - (pShoulderRight.Y / _dividedRatio);

                                _userDataArray[i].pHead.x = (pHead.X + _userDataArray[i].shiftValue) / _dividedRatio;
                                _userDataArray[i].pHead.y = _imageSingleHeight - (pHead.Y / _dividedRatio);

                                _userDataArray[i].pFootLeft.x = (pFootLeft.X + _userDataArray[i].shiftValue) / _dividedRatio;
                                _userDataArray[i].pFootLeft.y = _imageSingleHeight - (pFootLeft.Y / _dividedRatio);

                                _userDataArray[i].pFootRight.x = (pFootRight.X + _userDataArray[i].shiftValue) / _dividedRatio;
                                _userDataArray[i].pFootRight.y = _imageSingleHeight - (pFootRight.Y / _dividedRatio);
                            }
                            else if (_inputType == InputType.Depth)
                            {
                                DepthSpacePoint pHandLeft = _coordinateMapper.MapCameraPointToDepthSpace(joints[JointType.HandLeft].Position);
                                DepthSpacePoint pHandRight = _coordinateMapper.MapCameraPointToDepthSpace(joints[JointType.HandRight].Position);
                                DepthSpacePoint pElbowLeft = _coordinateMapper.MapCameraPointToDepthSpace(joints[JointType.ElbowLeft].Position);
                                DepthSpacePoint pElbowRight = _coordinateMapper.MapCameraPointToDepthSpace(joints[JointType.ElbowRight].Position);
                                DepthSpacePoint pShoulderLeft = _coordinateMapper.MapCameraPointToDepthSpace(joints[JointType.ShoulderLeft].Position);
                                DepthSpacePoint pShoulderRight = _coordinateMapper.MapCameraPointToDepthSpace(joints[JointType.ShoulderRight].Position);
                                DepthSpacePoint pHead = _coordinateMapper.MapCameraPointToDepthSpace(joints[JointType.Head].Position);
                                DepthSpacePoint pFootLeft = _coordinateMapper.MapCameraPointToDepthSpace(joints[JointType.FootLeft].Position);
                                DepthSpacePoint pFootRight = _coordinateMapper.MapCameraPointToDepthSpace(joints[JointType.FootRight].Position);

                                _userDataArray[i].pHandLeft.x = pHandLeft.X + _userDataArray[i].shiftValue;
                                _userDataArray[i].pHandLeft.y = _imageSingleHeight - pHandLeft.Y;

                                _userDataArray[i].pHandRight.x = pHandRight.X + _userDataArray[i].shiftValue;
                                _userDataArray[i].pHandRight.y = _imageSingleHeight - pHandRight.Y;

                                _userDataArray[i].pElbowLeft.x = pElbowLeft.X + _userDataArray[i].shiftValue;
                                _userDataArray[i].pElbowLeft.y = _imageSingleHeight - pElbowLeft.Y;

                                _userDataArray[i].pElbowRight.x = pElbowRight.X + _userDataArray[i].shiftValue;
                                _userDataArray[i].pElbowRight.y = _imageSingleHeight - pElbowRight.Y;

                                _userDataArray[i].pShoulderLeft.x = pShoulderLeft.X + _userDataArray[i].shiftValue;
                                _userDataArray[i].pShoulderLeft.y = _imageSingleHeight - pShoulderLeft.Y;

                                _userDataArray[i].pShoulderRight.x = pShoulderRight.X + _userDataArray[i].shiftValue;
                                _userDataArray[i].pShoulderRight.y = _imageSingleHeight - pShoulderRight.Y;

                                _userDataArray[i].pHead.x = pHead.X + _userDataArray[i].shiftValue;
                                _userDataArray[i].pHead.y = _imageSingleHeight - pHead.Y;

                                _userDataArray[i].pFootLeft.x = pFootLeft.X + _userDataArray[i].shiftValue;
                                _userDataArray[i].pFootLeft.y = _imageSingleHeight - pFootLeft.Y;

                                _userDataArray[i].pFootRight.x = pFootRight.X + _userDataArray[i].shiftValue;
                                _userDataArray[i].pFootRight.y = _imageSingleHeight - pFootRight.Y;
                            }
                        }
                        else
                        {
                            _userDataArray[i].userID = -1;
                            _userDataArray[i].shiftValue = 0;
                            _userDataArray[i].joints = null;
                            _userDataArray[i].ClearDepthData();
                        }
                    }
                    else
                    {
                        _userDataArray[i].userID = -1;
                        _userDataArray[i].shiftValue = 0;
                        _userDataArray[i].joints = null;
                        _userDataArray[i].ClearDepthData();
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log("=================Foreach Body Exception===================");
                    Debug.Log(ex.Message);
                    Debug.Log("=================Foreach Body Exception===================");
                    break;
                }
            }
        }

        private void MakeOutputImage()
        {
            Array.Clear(_outputImageArray, 0, _outputImageArray.Length);

            if (_depthCoordinatesArray == null || _depthCoordinatesArray.Length == 0) return;

            if (_imageType == ImageType.Chromakey)
            {
                for (int y = 0; y < Constants.DEFAULT_COLOR_HEIGHT; y += _dividedRatio)
                {
                    for (int x = 0; x < Constants.DEFAULT_COLOR_WIDTH; x += _dividedRatio)
                    {
                        try
                        {
                            int index = x + y * Constants.DEFAULT_COLOR_WIDTH;
                            int colorIndex = (x * 4) + y * Constants.DEFAULT_COLOR_WIDTH * 4;

                            if ((!float.IsInfinity(_depthCoordinatesArray[index].X) && !float.IsNaN(_depthCoordinatesArray[index].X) && _depthCoordinatesArray[index].X != 0) ||
                                (!float.IsInfinity(_depthCoordinatesArray[index].Y) && !float.IsNaN(_depthCoordinatesArray[index].Y) && _depthCoordinatesArray[index].Y != 0))
                            {
                                float player = _bodyIndexPointsArray[(int)_depthCoordinatesArray[index].X + (int)(_depthCoordinatesArray[index].Y * Constants.DEFAULT_DEPTH_WIDTH)];

                                if (player != 255)
                                {
                                    for (int i = 0; i < _userDataArray.Length; i++)
                                    {
                                        if (_userDataArray[i].userID != player) continue;
                                        else
                                        {
                                            int xOutput = (x + _userDataArray[i].shiftValue) / _dividedRatio;
                                            int yOutput = y / _dividedRatio;
                                            int outputIndex = (xOutput * 4) + yOutput * (_imageFullWidth * 4);

                                            if (xOutput < 0 || xOutput >= _imageFullWidth) continue;
                                            if (outputIndex < 0 || outputIndex >= _outputImageArray.Length) continue;

                                            _outputImageArray[outputIndex + 0] = _colorBufferArray[colorIndex + 0];
                                            _outputImageArray[outputIndex + 1] = _colorBufferArray[colorIndex + 1];
                                            _outputImageArray[outputIndex + 2] = _colorBufferArray[colorIndex + 2];
                                            _outputImageArray[outputIndex + 3] = _colorBufferArray[colorIndex + 3];
                                        }

                                    }
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("Make Chromakey Image Error");
                            Debug.Log(ex.Message);
                            Debug.Log(ex.StackTrace);
                            //break;
                        }
                    }
                }
            }
            else if (_imageType == ImageType.ShadowBody)
            {
                byte R = 0, G = 0, B = 0;

                for (int y = 0; y < Constants.DEFAULT_DEPTH_HEIGHT; y++)
                {
                    for (int x = 0; x < Constants.DEFAULT_DEPTH_WIDTH; x++)
                    {
                        try
                        {
                            float player = _bodyIndexPointsArray[x + y * Constants.DEFAULT_DEPTH_WIDTH];

                            if (player != 255)
                            {
                                for (int i = 0; i < _userDataArray.Length; i++)
                                {
                                    if (_userDataArray[i].userID != player) continue;
                                    else
                                    {
                                        int xOutput = x + _userDataArray[i].shiftValue;
                                        int yOutput = y;
                                        int outputIndex = (xOutput * 4) + yOutput * (_imageFullWidth * 4);

                                        if (xOutput < 0 || xOutput >= _imageFullWidth) continue;
                                        if (outputIndex < 0 || outputIndex >= _outputImageArray.Length) continue;

                                        if (i == 0) { R = 255; G = 0; B = 0; }
                                        else if (i == 1) { R = 0; G = 255; B = 0; }
                                        else if (i == 2) { R = 0; G = 0; B = 255; }
                                        else if (i == 3) { R = 255; G = 255; B = 0; }
                                        else if (i == 4) { R = 232; G = 165; B = 229; }
                                        else if (i == 5) { R = 179; G = 255; B = 255; }

                                        _outputImageArray[outputIndex + 0] = B;
                                        _outputImageArray[outputIndex + 1] = G;
                                        _outputImageArray[outputIndex + 2] = R;
                                        _outputImageArray[outputIndex + 3] = alphaOpacity;
                                    }

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("Make Shadow Image Error");
                            Debug.Log(ex.Message);
                            Debug.Log(ex.StackTrace);
                        }
                    }
                }
            }
            else if (_imageType == ImageType.ParticleBody)
            {
                //byte R = 0, G = 0, B = 0;

                for (int y = 0; y < Constants.DEFAULT_DEPTH_HEIGHT; y += 3)
                {
                    for (int x = 0; x < Constants.DEFAULT_DEPTH_WIDTH; x += 3)
                    {
                        float player = _bodyIndexPointsArray[x + y * Constants.DEFAULT_DEPTH_WIDTH];
                        if (player != 255)
                        {
                            for (int i = 0; i < _userDataArray.Length; i++)
                            {
                                if (_userDataArray[i].userID != player) continue;
                                else
                                {
                                    int xOutput = x + _userDataArray[i].shiftValue;
                                    int yOutput = y;
                                    int outputIndex = (xOutput * 4) + yOutput * (_imageFullWidth * 4);

                                    if (xOutput < 0 || xOutput >= _imageFullWidth) continue;
                                    if (outputIndex < 0 || outputIndex >= _outputImageArray.Length) continue;

                                    int nDistance = (int)(_depthBufferArray[x + y * Constants.DEFAULT_DEPTH_WIDTH]);

                                    ClusterColor color = null;

                                    if (nDistance <= Constants.ParticleMinDstance) color = new ClusterColor(0x00, 0x00, 0xFF);
                                    else if (Constants.ParticleMinDstance < nDistance && nDistance <= Constants.ParticleMinDstance + 255) color = new ClusterColor((byte)(nDistance - Constants.ParticleMinDstance), 0x00, 0xFF);
                                    else if (Constants.ParticleMinDstance + 255 < nDistance && nDistance <= Constants.ParticleMinDstance + 510) color = new ClusterColor(0xFF, 0x00, (byte)(255 - (nDistance - Constants.ParticleMinDstance - 255)));
                                    else if (Constants.ParticleMinDstance + 510 < nDistance && nDistance <= Constants.ParticleMinDstance + 765) color = new ClusterColor(0xFF, (byte)(nDistance - Constants.ParticleMinDstance - 510), 0x00);
                                    else if (Constants.ParticleMinDstance + 765 < nDistance && nDistance <= Constants.ParticleMinDstance + 1020) color = new ClusterColor((byte)(255 - (nDistance - Constants.ParticleMinDstance - 765)), 0xFF, 0x00);
                                    else if (Constants.ParticleMinDstance + 1020 < nDistance && nDistance <= Constants.ParticleMinDstance + 1275) color = new ClusterColor(0x00, 0xFF, (byte)(nDistance - Constants.ParticleMinDstance - 1020));
                                    else if (Constants.ParticleMinDstance + 1275 < nDistance && nDistance <= Constants.ParticleMinDstance + 1530) color = new ClusterColor((byte)(nDistance - Constants.ParticleMinDstance - 1275), 0xFF, 0xFF);
                                    else if (nDistance > Constants.ParticleMinDstance + 1530) color = new ClusterColor(0xFF, 0xFF, 0xFF);

                                    //if (i == 0) { R = 255; G = 0; B = 0; }
                                    //else if (i == 1) { R = 0; G = 255; B = 0; }
                                    //else if (i == 2) { R = 0; G = 0; B = 255; }
                                    //else if (i == 3) { R = 255; G = 255; B = 0; }
                                    //else if (i == 4) { R = 232; G = 165; B = 229; }
                                    //else if (i == 5) { R = 179; G = 255; B = 255; }

                                    for (int j = 0; j < _nSideMatrix.Length; j++)
                                    {
                                        if (outputIndex + _nSideMatrix[j] < 0 || outputIndex + _nSideMatrix[j] >= _outputImageArray.Length - 1) continue;

                                        _outputImageArray[outputIndex + _nSideMatrix[j] + 0] = color.B;
                                        _outputImageArray[outputIndex + _nSideMatrix[j] + 1] = color.G;
                                        _outputImageArray[outputIndex + _nSideMatrix[j] + 2] = color.R;
                                        _outputImageArray[outputIndex + _nSideMatrix[j] + 3] = particleOpacity;
                                    }
                                }

                            }
                        }
                    }
                }
            }
            else if (_imageType == ImageType.Original)
            {
                _outputImageArray = _colorBufferArray;
            }
        }

        private void CheckMotion()
        {
            for (int i = 0; i < _userDataArray.Length; i++)
            {
                if (_userDataArray[i].userID == -1) continue;
                Dictionary<JointType, Windows.Kinect.Joint> joints = _userDataArray[i].joints;
                if (joints == null) continue;

                if (_motionType == MotionType.HeartMotion)
                {
                    double lengthLeftSE = Math.Sqrt(Math.Pow(_userDataArray[i].pShoulderLeft.x - _userDataArray[i].pElbowLeft.x, 2) + Math.Pow(_userDataArray[i].pShoulderLeft.y - _userDataArray[i].pElbowLeft.y, 2));
                    double lengthLeftEH = Math.Sqrt(Math.Pow(_userDataArray[i].pHandLeft.x - _userDataArray[i].pElbowLeft.x, 2) + Math.Pow(_userDataArray[i].pHandLeft.y - _userDataArray[i].pElbowLeft.y, 2));
                    double lengthLeftSH = Math.Sqrt(Math.Pow(_userDataArray[i].pShoulderLeft.x - _userDataArray[i].pHandLeft.x, 2) + Math.Pow(_userDataArray[i].pShoulderLeft.y - _userDataArray[i].pHandLeft.y, 2));

                    double lengthRightSE = Math.Sqrt(Math.Pow(_userDataArray[i].pShoulderRight.x - _userDataArray[i].pElbowRight.x, 2) + Math.Pow(_userDataArray[i].pShoulderRight.y - _userDataArray[i].pElbowRight.y, 2));
                    double lengthRightEH = Math.Sqrt(Math.Pow(_userDataArray[i].pHandRight.x - _userDataArray[i].pElbowRight.x, 2) + Math.Pow(_userDataArray[i].pHandRight.y - _userDataArray[i].pElbowRight.y, 2));
                    double lengthRightSH = Math.Sqrt(Math.Pow(_userDataArray[i].pShoulderRight.x - _userDataArray[i].pHandRight.x, 2) + Math.Pow(_userDataArray[i].pShoulderRight.y - _userDataArray[i].pHandRight.y, 2));

                    double arcLeft = Math.Acos((Math.Pow(lengthLeftSE, 2) + Math.Pow(lengthLeftEH, 2) - Math.Pow(lengthLeftSH, 2)) / (2 * lengthLeftSE * lengthLeftEH));
                    double arcRight = Math.Acos((Math.Pow(lengthRightSE, 2) + Math.Pow(lengthRightEH, 2) - Math.Pow(lengthRightSH, 2)) / (2 * lengthRightSE * lengthRightEH));

                    arcLeft = 180 * arcLeft / Math.PI;
                    arcRight = 180 * arcRight / Math.PI;

                    if (arcLeft > Constants.MinArc && arcLeft < Constants.MaxArc && arcRight > Constants.MinArc && arcRight < Constants.MaxArc)
                    {
                        if (_userDataArray[i].pShoulderLeft.y < _userDataArray[i].pElbowLeft.y
                            && _userDataArray[i].pElbowLeft.y < _userDataArray[i].pHandLeft.y
                            && _userDataArray[i].pShoulderRight.y < _userDataArray[i].pElbowRight.y
                            && _userDataArray[i].pElbowRight.y < _userDataArray[i].pHandRight.y)
                        {
                            double lengthBetweenHand = Math.Sqrt(Math.Pow(joints[JointType.HandLeft].Position.X - joints[JointType.HandRight].Position.X, 2) + Math.Pow(joints[JointType.HandLeft].Position.Y - joints[JointType.HandRight].Position.Y, 2));
                            if (lengthBetweenHand < Constants.HandLength)
                            {
                                if (_isDebug) renderTexture.DrawFilledCircle((int)_userDataArray[i].pHead.x, (int)_userDataArray[i].pHead.y, 30, Color.white);
                                DrawHeart(i, _userDataArray[i].pHead.x / _imageFullWidth, _userDataArray[i].pHead.y / _imageFullHeight);
                            }
                        }
                    }
                }
                else if (_motionType == MotionType.PunchNKickMotion)
                {
                    if (!_userDataArray[i].isReadyPunchLeft)
                        if (Math.Abs(_userDataArray[i].joints[JointType.SpineBase].Position.Z - _userDataArray[i].joints[JointType.HandLeft].Position.Z) < Constants.ReadyThreshold)
                            _userDataArray[i].isReadyPunchLeft = true;
                    if (_userDataArray[i].isReadyPunchLeft)
                    {
                        AddDepthData(joints, _userDataArray[i].PunchDepthLeft, JointType.HandLeft, i, _userDataArray[i].HandStateLeft);
                        AnalysisEffect(_userDataArray[i].PunchDepthLeft, JointType.HandLeft, i, _userDataArray[i].HandStateLeft);
                    }

                    if (!_userDataArray[i].isReadyPunchRight)
                        if (Math.Abs(_userDataArray[i].joints[JointType.SpineBase].Position.Z - _userDataArray[i].joints[JointType.HandRight].Position.Z) < Constants.ReadyThreshold)
                            _userDataArray[i].isReadyPunchRight = true;
                    if (_userDataArray[i].isReadyPunchRight)
                    {
                        AddDepthData(joints, _userDataArray[i].PunchDepthRight, JointType.HandRight, i, _userDataArray[i].HandStateRight);
                        AnalysisEffect(_userDataArray[i].PunchDepthRight, JointType.HandRight, i, _userDataArray[i].HandStateRight);
                    }

                    if (!_userDataArray[i].isReadyKickLeft)
                        if (Math.Abs(_userDataArray[i].joints[JointType.SpineBase].Position.Z - _userDataArray[i].joints[JointType.FootLeft].Position.Z) < Constants.ReadyThreshold)
                            _userDataArray[i].isReadyKickLeft = true;
                    if (_userDataArray[i].isReadyKickLeft)
                    {
                        AddDepthData(joints, _userDataArray[i].KickDepthLeft, JointType.FootLeft, i);
                        AnalysisEffect(_userDataArray[i].KickDepthLeft, JointType.FootLeft, i);
                    }

                    if (!_userDataArray[i].isReadyKickRight)
                        if (Math.Abs(_userDataArray[i].joints[JointType.SpineBase].Position.Z - _userDataArray[i].joints[JointType.FootRight].Position.Z) < Constants.ReadyThreshold)
                            _userDataArray[i].isReadyKickRight = true;
                    if (_userDataArray[i].isReadyKickRight)
                    {
                        AddDepthData(joints, _userDataArray[i].KickDepthRight, JointType.FootRight, i);
                        AnalysisEffect(_userDataArray[i].KickDepthRight, JointType.FootRight, i);
                    }
                }
                else if (_motionType == MotionType.PunchMotion)
                {
                    if (!_userDataArray[i].isReadyPunchLeft)
                        if (Math.Abs(_userDataArray[i].joints[JointType.SpineBase].Position.Z - _userDataArray[i].joints[JointType.HandLeft].Position.Z) < Constants.ReadyThreshold)
                            _userDataArray[i].isReadyPunchLeft = true;
                    if (_userDataArray[i].isReadyPunchLeft)
                    {
                        AddDepthData(joints, _userDataArray[i].PunchDepthLeft, JointType.HandLeft, i, _userDataArray[i].HandStateLeft);
                        AnalysisEffect(_userDataArray[i].PunchDepthLeft, JointType.HandLeft, i, _userDataArray[i].HandStateLeft);
                    }

                    if (!_userDataArray[i].isReadyPunchRight)
                        if (Math.Abs(_userDataArray[i].joints[JointType.SpineBase].Position.Z - _userDataArray[i].joints[JointType.HandRight].Position.Z) < Constants.ReadyThreshold)
                            _userDataArray[i].isReadyPunchRight = true;
                    if (_userDataArray[i].isReadyPunchRight)
                    {
                        AddDepthData(joints, _userDataArray[i].PunchDepthRight, JointType.HandRight, i, _userDataArray[i].HandStateRight);
                        AnalysisEffect(_userDataArray[i].PunchDepthRight, JointType.HandRight, i, _userDataArray[i].HandStateRight);
                    }
                }
                else if (_motionType == MotionType.KickMotion)
                {
                    if (!_userDataArray[i].isReadyKickLeft)
                        if (Math.Abs(_userDataArray[i].joints[JointType.SpineBase].Position.Z - _userDataArray[i].joints[JointType.FootLeft].Position.Z) < Constants.ReadyThreshold)
                            _userDataArray[i].isReadyKickLeft = true;
                    if (_userDataArray[i].isReadyKickLeft)
                    {
                        AddDepthData(joints, _userDataArray[i].KickDepthLeft, JointType.FootLeft, i);
                        AnalysisEffect(_userDataArray[i].KickDepthLeft, JointType.FootLeft, i);
                    }

                    if (!_userDataArray[i].isReadyKickRight)
                        if (Math.Abs(_userDataArray[i].joints[JointType.SpineBase].Position.Z - _userDataArray[i].joints[JointType.FootRight].Position.Z) < Constants.ReadyThreshold)
                            _userDataArray[i].isReadyKickRight = true;
                    if (_userDataArray[i].isReadyKickRight)
                    {
                        AddDepthData(joints, _userDataArray[i].KickDepthRight, JointType.FootRight, i);
                        AnalysisEffect(_userDataArray[i].KickDepthRight, JointType.FootRight, i);
                    }
                }
                else if (_motionType == MotionType.ThrowMotion)
                {
                    if (!_userDataArray[i].isReadyPunchLeft)
                        if (Math.Abs(_userDataArray[i].joints[JointType.SpineBase].Position.Z - _userDataArray[i].joints[JointType.HandLeft].Position.Z) < Constants.ReadyThreshold)
                            _userDataArray[i].isReadyPunchLeft = true;
                    if (_userDataArray[i].isReadyPunchLeft)
                    {
                        AddDepthData(joints, _userDataArray[i].PunchDepthLeft, JointType.HandLeft, i, _userDataArray[i].HandStateLeft);
                        AnalysisEffect(_userDataArray[i].PunchDepthLeft, JointType.HandLeft, i, _userDataArray[i].HandStateLeft);
                    }

                    if (!_userDataArray[i].isReadyPunchRight)
                        if (Math.Abs(_userDataArray[i].joints[JointType.SpineBase].Position.Z - _userDataArray[i].joints[JointType.HandRight].Position.Z) < Constants.ReadyThreshold)
                            _userDataArray[i].isReadyPunchRight = true;
                    if (_userDataArray[i].isReadyPunchRight)
                    {
                        AddDepthData(joints, _userDataArray[i].PunchDepthRight, JointType.HandRight, i, _userDataArray[i].HandStateRight);
                        AnalysisEffect(_userDataArray[i].PunchDepthRight, JointType.HandRight, i, _userDataArray[i].HandStateRight);
                    }
                }
                else if (_motionType == MotionType.LampMotion)
                {
                    PointF left = new PointF();
                    left.X = _userDataArray[i].pHandLeft.x / _imageFullWidth;
                    left.Y = _userDataArray[i].pHandLeft.y / _imageFullHeight;

                    PointF right = new PointF();
                    right.X = _userDataArray[i].pHandRight.x / _imageFullWidth;
                    right.Y = _userDataArray[i].pHandRight.y / _imageFullHeight;

                    TakeLamp(i, left, right, joints[JointType.SpineBase].Position.Z);
                }
            }
        }

        private void AddDepthData(Dictionary<JointType, Windows.Kinect.Joint> joints, List<Vector3> depthList, JointType type, int index, List<HandState> handStateList = null)
        {
            if (joints == null) return;

            Vector3 vPosition = new Vector3(joints[type].Position.X, joints[type].Position.Y, joints[type].Position.Z);
            try
            {
                if (depthList.Count < Constants.EffectFrameCount)
                {
                    depthList.Add(vPosition);
                    if (handStateList != null)
                    {
                        if (_bodyDataArrray[index].HandLeftState != HandState.NotTracked) handStateList.Add(_bodyDataArrray[index].HandLeftState);
                        else handStateList.Add(_bodyDataArrray[index].HandRightState);

                    }
                }
                else
                {
                    depthList.RemoveAt(0);
                    depthList.Add(vPosition);

                    if (handStateList != null)
                    {
                        handStateList.RemoveAt(0);
                        if (type.ToString().Contains("Left")) handStateList.Add(_bodyDataArrray[index].HandLeftState);
                        else handStateList.Add(_bodyDataArrray[index].HandRightState);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log("AddDepthData Error : " + ex.Message);
            }
        }

        private void AnalysisEffect(List<Vector3> depthList, JointType type, int index, List<HandState> handStateList = null)
        {
            if (_bodyDataArrray[index].Joints == null) return;
            if (_bodyDataArrray[index].Joints[JointType.Head].Position.Z < Constants.MinUserDepth)
            {
                depthList.Clear();
                if (handStateList != null) handStateList.Clear();
                return;
            }

            int MAX_OPEN = 0;
            int MAX_CLOSED = 0;
            double EFFECT_DISTANCE = 0;
            bool isHand = false;
            bool isClear = false;
            if (_motionType == MotionType.PunchMotion || _motionType == MotionType.PunchNKickMotion)
            {
                MAX_OPEN = 0;
                MAX_CLOSED = 3;
            }
            else if (_motionType == MotionType.ThrowMotion)
            {
                MAX_OPEN = 2;
                MAX_CLOSED = 0;
            }

            if (type.ToString().Contains("Hand"))
            {
                EFFECT_DISTANCE = Constants.PunchingDistance;
                isHand = true;
            }
            else if (type.ToString().Contains("Foot")) EFFECT_DISTANCE = Constants.KickDistance;

            if (depthList.Count == Constants.EffectFrameCount)
            {
                Vector3 interval = (depthList[0] - depthList[depthList.Count - 1]);

                if (interval.z > 0)
                {
                    if (interval.magnitude > EFFECT_DISTANCE)
                    {

                        if (isHand)
                        {
                            int openCount = 0;
                            int closedCount = 0;
                            for (int j = 0; j < handStateList.Count; j++)
                            {
                                if (handStateList[j] == HandState.Open)
                                {
                                    openCount++;
                                    if (openCount >= MAX_OPEN) break;
                                }

                                if (_motionType == MotionType.ThrowMotion) continue;

                                if (handStateList[j] == HandState.Closed)
                                {
                                    closedCount++;
                                    if (closedCount >= MAX_CLOSED) break;
                                }
                            }

                            if (openCount == MAX_OPEN && closedCount >= MAX_CLOSED)
                            {
                                if (_motionType == MotionType.PunchMotion || _motionType == MotionType.PunchNKickMotion)
                                {
                                    if (type.ToString().Contains("Left"))
                                    {
                                        if (_userDataArray[index].joints[JointType.Head].Position.Z - _userDataArray[index].joints[JointType.HandLeft].Position.Z > Constants.DepthThreshold)
                                        {
                                            EffectPunch(index, _userDataArray[index].pHandLeft.x / _imageFullWidth, _userDataArray[index].pHandLeft.y / _imageFullHeight);
                                            _userDataArray[index].isReadyPunchLeft = false;
                                            isClear = true;
                                        }
                                    }
                                    else
                                    {
                                        if (_userDataArray[index].joints[JointType.Head].Position.Z - _userDataArray[index].joints[JointType.HandRight].Position.Z > Constants.DepthThreshold)
                                        {
                                            EffectPunch(index, _userDataArray[index].pHandRight.x / _imageFullWidth, _userDataArray[index].pHandRight.y / _imageFullHeight);
                                            _userDataArray[index].isReadyPunchRight = false;
                                            isClear = true;
                                        }
                                    }
                                }
                                else if (_motionType == MotionType.ThrowMotion)
                                {
                                    Vector2 handPos = Vector2.zero;
                                    List<Vector3> dataList = null;

                                    if (type.ToString().Contains("Left"))
                                    {
                                        if (_userDataArray[index].joints[JointType.Head].Position.Z - _userDataArray[index].joints[JointType.HandLeft].Position.Z > Constants.DepthThreshold)
                                        {
                                            EffectThrow(index, _userDataArray[index].pHandLeft.x / _imageFullWidth, _userDataArray[index].pHandLeft.y / _imageFullHeight);
                                            handPos = _userDataArray[index].pHandLeft;
                                            dataList = _userDataArray[index].PunchDepthLeft;
                                            _userDataArray[index].isReadyPunchLeft = false;
                                        }
                                    }
                                    else
                                    {
                                        if (_userDataArray[index].joints[JointType.Head].Position.Z - _userDataArray[index].joints[JointType.HandRight].Position.Z > Constants.DepthThreshold)
                                        {
                                            EffectThrow(index, _userDataArray[index].pHandRight.x / _imageFullWidth, _userDataArray[index].pHandRight.y / _imageFullHeight);
                                            handPos = _userDataArray[index].pHandRight;
                                            dataList = _userDataArray[index].PunchDepthRight;
                                            _userDataArray[index].isReadyPunchRight = false;
                                        }
                                    }
                                }

                                if (isClear) depthList.Clear();
                                if (isClear) handStateList.Clear();
                            }
                        }
                        else
                        {
                            if (_isDebug) renderTexture.DrawFilledCircle((int)_userDataArray[index].pFootRight.x, (int)_userDataArray[index].pFootRight.y, 30, Color.white);

                            if (type.ToString().Contains("Left"))
                            {
                                if (_userDataArray[index].joints[JointType.SpineBase].Position.Z - _userDataArray[index].joints[JointType.FootLeft].Position.Z > Constants.DepthThreshold)
                                {
                                    EffectKick(index, _userDataArray[index].pFootLeft.x / _imageFullWidth, _userDataArray[index].pFootLeft.y / _imageFullHeight);
                                    _userDataArray[index].isReadyKickLeft = false;
                                    isClear = true;
                                }
                            }
                            else
                            {
                                if (_userDataArray[index].joints[JointType.SpineBase].Position.Z - _userDataArray[index].joints[JointType.FootRight].Position.Z > Constants.DepthThreshold)
                                {
                                    EffectKick(index, _userDataArray[index].pFootRight.x / _imageFullWidth, _userDataArray[index].pFootRight.y / _imageFullHeight);
                                    _userDataArray[index].isReadyKickRight = false;
                                    isClear = true;
                                }
                            }
                            if (isClear) depthList.Clear();
                        }
                    }

                }
            }
        }

        void OnDisable()
        {
            ReleaseBuffers();
        }

        private void ReleaseBuffers()
        {
            _depthCoordinatesArray = null;
            _bodyIndexPointsArray = null;
        }

        private void DrawHeart(int i, float x, float y)
        {
            Debug.Log(string.Format("My Heart! at ({0:0.00}, {1:0.00})", x, y));
            if (CoordinateMapperEffect != null)
                CoordinateMapperEffect.DrawHeart(i, x, y);
        }

        private void EffectPunch(int i, float x, float y)
        {
            Debug.Log(string.Format("My Punch! at ({0:0.00}, {1:0.00})", x, y));
            if (CoordinateMapperEffect != null)
                CoordinateMapperEffect.EffectPunch(i, x, y);
        }

        private void EffectKick(int i, float x, float y)
        {
            Debug.Log(string.Format("My Kick! at ({0:0.00}, {1:0.00})", x, y));
            if (CoordinateMapperEffect != null)
                CoordinateMapperEffect.EffectKick(i, x, y);
        }

        private void EffectThrow(int i, float x, float y)
        {
            Debug.Log(string.Format("My Throw! at ({0:0.00}, {1:0.00})", x, y));
            if (CoordinateMapperEffect != null)
                CoordinateMapperEffect.EffectThrow(i, x, y);
        }

        private void EffectThrow(int i, float x, float y, Vector3 navigate, float power)
        {
            Debug.Log(string.Format("My Throw! at ({0:0.00}, {1:0.00} p - {2:0.00} n - {3})", x, y, power, navigate));
            if (CoordinateMapperEffect != null)
                CoordinateMapperEffect.EffectThrow(i, x, y, navigate, power);
        }
        private void TakeLamp(int i, PointF leftHand, PointF rightHand, float z)
        {
            Debug.Log(string.Format("Lamp! at ({0:0.00}, {1:0.00}) ({2:0.00}, {3:0.00})", leftHand.X, leftHand.Y, rightHand.X, rightHand.Y));
            if (CoordinateMapperEffect != null)
                CoordinateMapperEffect.TakeLamp(i, new Vector2(leftHand.X, leftHand.Y), new Vector2(rightHand.X, rightHand.Y));
        }
    }
}