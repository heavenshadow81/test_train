//#define UNUSE_KINECT
//#define USE_TAG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NFCKeys = ML.PlaywallKids.Common.NFCConstants;

#if !UNUSE_KINECT
using Body = Windows.Kinect.Body;
using JointType = Windows.Kinect.JointType;
using CameraSpacePoint = Windows.Kinect.CameraSpacePoint;
#endif

namespace ML.PlaywallKids.Interaction
{
    using EContentState = InteractionContentsEnum.EState;

    public class InteractionContents : MonoBehaviour
    {

#if UNUSE_KINECT
    public enum ETestState
    {
        NONE = 0, FAR = 1, NEAR, IN_AREA
    }

    public ETestState testDistanceWithKinect;
#endif

        public UIInteractionController uiController;
        public Sprite guidanceTagImage, guidanceReadyImage;

        public int numberOfPlayers;
        public float durationOfGame; // 초 단위

        [HideInInspector]
        public int sequneceNumber;
        [HideInInspector]
        public int indexOfNumber; // 2인 사용할 경우 1p, 2p구분 짓기 위함
        [HideInInspector]
        public Sprite[] guidanceSprites;
        [HideInInspector]
        public string[] words;
        [HideInInspector]
        public string UserName
        {
            get;
            private set;
        }

        public delegate void Func();
        private Func callbackNoneInit;
        private Func callbackReadyInit;
        private Func callbackReadyEventInit0;
        private Func callbackReadyEventInit1;
        private Func callbackReadyEventInit2;
        private Func callbackPlayStateInit0;
        private Func callbackPlayingInit;
        private Func callbackCloseEventInit0;
        private Func callbackCloseInit1;
        private Func callbackCloseInit2;

        private CoordinateMapperManager kinectMapperManager;

#if USE_TAG
        private List<NFCUserInfo> listOfUsers;
        private bool bSendData;
#endif

        private int kinectIndex;

        private float elapsedEventTime;
        private float elapsedTime;

        private const string STAND_AT_THE_FRONT_OF_DISPLAY = "화면 앞에 서주세요";
        private const string SING_TOO_NEAR = "뒤로 물러나세요";
        private const string SIGN_TOO_FAR = "앞으로 오세요";
        private const string SIGN_IN_AREA = "정위치 입니다.";

        private const double FAR_DISTANCE = 3.0f;// Constants.FILTER_FAR_DISTANCE;
        private const double NEAR_DISTANCE = 2.0f;// Constants.FILTER_DISTANCE;


        bool _bTag;
        public bool isTaged
        {
            get
            {
                return _bTag;
            }
            set
            {
                if (value)
                {
                    currentState = EContentState.READY;
                }
                _bTag = value;
            }
        }

        public bool IsPlaying
        {
            get
            {
                return _state == EContentState.PLAYING;
            }
        }

        EContentState _state;
        public EContentState currentState
        {
            get
            {
                return _state;
            }

            set
            {
                if (value != _state)
                {
                    elapsedEventTime = 0f;
                    switch (value)
                    {
                        case EContentState.NONE:

                            sequneceNumber = -1;
#if USE_TAG
                            bSendData = false;
#endif
                            //  ResetUserInfo();
                            isTaged = false;

                            uiController.guidanceImage.sprite2D = guidanceTagImage;
                            uiController.guidanceImage.cachedTransform.localScale = Vector3.one;
                            uiController.score.Score = 0;

                            uiController.currentTimer.Active = false;
                            uiController.score.Active = false;
                            if (kinectMapperManager == null) kinectMapperManager = FindObjectOfType<CoordinateMapperManager>();
                            if (callbackNoneInit != null) callbackNoneInit();
                            elapsedTime = 0f;
                            if (durationOfGame == 0f) durationOfGame = 60f;
                            uiController.currentTimer.InitTime((int)(durationOfGame));
#if USE_TAG
                            uiController.wordsOfGuidance.text = NFCKeys.DO_TAGGING;
#else
                            uiController.wordsOfGuidance.text = STAND_AT_THE_FRONT_OF_DISPLAY;
#endif
                            uiController.tagOfUser.cachedGameObject.SetActive(false);
                            uiController.numericOfCountdown.Active = false;
                            uiController.resultObject.active = false;
                            uiController.guidanceObject.SetActive(true);
                            break;

                        case EContentState.READY: // tag 확인
                            if (callbackReadyInit != null) callbackReadyInit();
                            uiController.guidanceImage.sprite2D = guidanceReadyImage;
                            uiController.tagOfUser.cachedGameObject.SetActive(true);
                            uiController.wordsOfGuidance.text = STAND_AT_THE_FRONT_OF_DISPLAY;
                            break;

                        case EContentState.READY_EVENT0:
                            if (callbackReadyEventInit0 != null) callbackReadyEventInit0();
                            uiController.numericOfCountdown.Active = false;
                            break;

                        case EContentState.READY_EVENT1:
                            if (callbackReadyEventInit1 != null) callbackReadyEventInit1();
                            uiController.numericOfCountdown.Active = true;
                            kinectIndex = -1;
                            break;

                        case EContentState.PLAY_STATE0:
                            if (callbackPlayStateInit0 != null) callbackPlayStateInit0();
                            uiController.tagOfUser.gameObject.SetActive(false);
                            StartCoroutine(GuidanceProcess());
                            break;

                        case EContentState.PLAYING:
                            if (callbackPlayingInit != null) callbackPlayingInit();
                            uiController.score.Active = true;
                            uiController.currentTimer.Active = true;
                            uiController.numericOfCountdown.Active = false;
                            uiController.guidanceObject.SetActive(false);
                            uiController.uiPointOfScoreManager.gameObject.SetActive(true);
                            uiController.uiPointOfScoreManager.Beginning();
                            break;
                        case EContentState.CLOSE_EVENT0:
                            if (callbackCloseEventInit0 != null) callbackCloseEventInit0();
                            uiController.uiPointOfScoreManager.ComeToAStop();
                            uiController.resultObject.active = true;
                            uiController.resultObject.Display();
                            uiController.score.Active = false;
                            uiController.currentTimer.Active = false;
#if USE_TAG
                            if (!bSendData)
                            {
                                SendScore();
                                bSendData = true;
                            }
#endif
                            break;
                    }
                    _state = value;
                }
            }
        }

        void OnEnable()
        {
#if USE_TAG
            if (listOfUsers == null) listOfUsers = new List<NFCUserInfo>();
#endif
            currentState = EContentState.DEFAULT;
        }

        void OnDestroy()
        {
            callbackNoneInit = null;
            callbackReadyInit = null;
            callbackReadyEventInit0 = null;
            callbackReadyEventInit1 = null;
            callbackReadyEventInit2 = null;
            callbackPlayingInit = null;
            callbackCloseEventInit0 = null;
            callbackCloseInit1 = null;
            callbackCloseInit2 = null;
        }

        void Update()
        {
            switch (currentState)
            {
                case EContentState.DEFAULT:
                    currentState = EContentState.NONE;
                    break;
                case EContentState.NONE:
#if !USE_TAG
                currentState = EContentState.READY;
#endif
                    break;

                case EContentState.READY: //wearable tag 기다리기
#if UNITY_EDITOR
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        uiController.tagOfUser.cachedGameObject.SetActive(true);
                        currentState = EContentState.READY_EVENT0;
                    }
#endif

#if USE_TAG
                    if (isTaged)
                    {
                        currentState = FindTheUserWithKinect();
                    }
                    else
                    {
                        if (listOfUsers.Count > 0)
                        {
                            NFCUserInfo _info = listOfUsers[0];
                            listOfUsers.RemoveAt(0);
                            SetUser(_info.userName, _info.seqNo);
                        }
                    }
#else
                currentState = EContentState.READY_EVENT0;
#endif
                    break;

                case EContentState.READY_EVENT0:
                    // 사용자 탐색, 인식이 될경우 환영 인사 할 것
                    //if() 사용자 있을 경우 적정거리 확인 할 것

#if !UNUSE_KINECT
                    if (kinectIndex < 0)
                    {
                        currentState = EContentState.READY_EVENT1;
                        return;
                    }

                    Body[] _b = kinectMapperManager.GetBodyDataBuffer();
                    if (_b == null)
                    {
                        return;
                    }
                    Body _body = _b[kinectIndex];
                    if (kinectMapperManager.GetBodyDataBuffer()[kinectIndex] != null &&
                        kinectMapperManager.GetBodyDataBuffer()[kinectIndex].IsTracked &&
                        CheckInArea(_body, indexOfNumber))
                    {
#endif
#if UNUSE_KINECT
                if ( testDistanceWithKinect == ETestState.FAR)
#else
                        if (_body.Joints[Windows.Kinect.JointType.SpineBase].Position.Z > FAR_DISTANCE)
#endif
                        {
                            if (uiController.numericOfCountdown.Active) uiController.numericOfCountdown.Active = false;
                            uiController.wordsOfGuidance.text = SIGN_TOO_FAR;
                            elapsedEventTime = 0f;
                        }
#if UNUSE_KINECT
                else if (testDistanceWithKinect == ETestState.NEAR)
#else
                        else if (_body.Joints[Windows.Kinect.JointType.SpineBase].Position.Z < NEAR_DISTANCE)
#endif
                        {
                            if (uiController.numericOfCountdown.Active) uiController.numericOfCountdown.Active = false;
                            uiController.wordsOfGuidance.text = SING_TOO_NEAR;
                            elapsedEventTime = 0f;
                        }
#if UNUSE_KINECT
                else if(testDistanceWithKinect == ETestState.IN_AREA)
#else
                        else
#endif
                        {
                            uiController.wordsOfGuidance.text = SIGN_IN_AREA;
                            if (5f - elapsedEventTime > -1)
                            {
                                if (!uiController.numericOfCountdown.Active) uiController.numericOfCountdown.Active = true;
                                NumericsDisplayer.ChangePoint(uiController.numericOfCountdown, 5 - (int)elapsedEventTime);
                            }
                            else
                            {
                                uiController.numericOfCountdown.Active = false;
                                currentState = EContentState.PLAY_STATE0;
                            }
                            elapsedEventTime += Time.deltaTime;

                        }
#if UNUSE_KINECT
                else if(testDistanceWithKinect == ETestState.NONE)
                {
                    uiController.wordsOfGuidance.text = STAND_AT_THE_FRONT_OF_DISPLAY;
                    currentState = EContentState.READY_EVENT1;
                }

    
#else
                    }
                    else // 사용자가 없을 경우 찾기
                    {
                        kinectIndex = -1;
                        uiController.wordsOfGuidance.text = STAND_AT_THE_FRONT_OF_DISPLAY;
                        currentState = EContentState.READY_EVENT1;
                    }
#endif
                    break;

                case EContentState.READY_EVENT1:// 사용자 찾기
#if UNUSE_KINECT
                if (testDistanceWithKinect == ETestState.IN_AREA)
                { currentState = EContentState.READY_EVENT0; }
#else
                    if (FindTheUserWithKinect() == EContentState.READY_EVENT0)
                    {
                        currentState = EContentState.READY_EVENT0;
                    }
#endif
                    else
                    {
                        int _theTimeLimit = 20;
                        if ((elapsedEventTime += Time.deltaTime) > _theTimeLimit)
                            currentState = EContentState.DEFAULT;
                        else
                            NumericsDisplayer.ChangePoint(uiController.numericOfCountdown, _theTimeLimit - (int)elapsedEventTime);
                    }
                    break;

                case EContentState.PLAYING: // 실제 플레이
                    elapsedTime += Time.deltaTime;
                    uiController.currentTimer.ChangeTime((int)(durationOfGame - elapsedTime));

                    if (elapsedTime > durationOfGame)
                        currentState = EContentState.CLOSE_EVENT0;
                    break;
                case EContentState.CLOSE_EVENT0: // 결과창 출력
                    if ((elapsedEventTime += Time.deltaTime) > 11f)
                    {
                        currentState = EContentState.NONE;
                    }
                    else
                    {
                        if (elapsedEventTime >= 5f)
                        {
                            if (!uiController.numericOfCountdown.Active) uiController.numericOfCountdown.Active = true;
                            uiController.numericOfCountdown.ChangeNumerics(NumericSplit.Split(10 - (int)elapsedEventTime));
                        }
                    }
                    break;
            }
        }

#if USE_TAG
        public void SetUser(string _jsonStr)
        {
            if (string.IsNullOrEmpty(_jsonStr)) return;

            NFCUserInfo _info = NFCUserInfo.GetNFCUserInfo(_jsonStr);

            string _userName = _info.userName;
            int _seq = _info.seqNo;
            if (string.IsNullOrEmpty(_userName)) return;

            if (!isTaged && !IsPlaying)
            {
                SetUser(_userName, _seq);
            }
            else
            {
                listOfUsers.Add(new NFCUserInfo() { userName = _userName, seqNo = _seq });
            }
        }

        public void SetUser(string _userName, int _seqNo)
        {
            UserName = _userName;
            uiController.tagOfUser.text = string.Format("{0}친구 반가워!", _userName);
            sequneceNumber = _seqNo;
            isTaged = true;
        }

        private void SendScore()
        {
            SendScore(sequneceNumber.ToString(), uiController.score.Score.ToString());
        }

        private void SendScore(string _seq, string _score)
        {
            bool bSuccess = NFCClientSocket.instance.SendData(new Dictionary<string, object>() {
                            {NFCKeys.KEY_CONTENTS_SEQ, _seq},
                            {NFCKeys.KEY_RESULT_SCORE, _score}
                            });
        }
#endif

#if !UNUSE_KINECT

        private EContentState FindTheUserWithKinect()
        {
            if (numberOfPlayers == 0 || numberOfPlayers > 2) return EContentState.READY_EVENT0;

            float _nearDistance = 0f;
            Body[] _bodies = kinectMapperManager.GetBodyDataBuffer();
            if (_bodies == null) return EContentState.READY_EVENT1;
            kinectIndex = -1;
            for (int i = 0; i < _bodies.Length; ++i)
            {
                if (_bodies[i].IsTracked)
                {
                    Windows.Kinect.CameraSpacePoint _p = _bodies[i].Joints[Windows.Kinect.JointType.SpineBase].Position;
                    Windows.Kinect.DepthSpacePoint pDepthMap = kinectMapperManager.GetCoordinateMapper().MapCameraPointToDepthSpace(_p);

                    float _width = Constants.DEFAULT_DEPTH_WIDTH * (1f / numberOfPlayers);

                    if (CheckInArea(_bodies[i], indexOfNumber))
                    {
                        if ((_nearDistance != 0 && _nearDistance > _p.Z) || _nearDistance == 0)
                        {
                            _nearDistance = _p.Z;
                            kinectIndex = i;
                        }
                    }
                }
            }

            if (kinectIndex < 0)
                return EContentState.READY_EVENT1;
            else
                return EContentState.READY_EVENT0;
        }

        bool CheckInArea(Body _body, int _index)
        {
            Windows.Kinect.CameraSpacePoint _p = _body.Joints[Windows.Kinect.JointType.SpineBase].Position;
            Windows.Kinect.DepthSpacePoint pDepthMap = kinectMapperManager.GetCoordinateMapper().MapCameraPointToDepthSpace(_p);
            float _width = Constants.DEFAULT_DEPTH_WIDTH * (1f / numberOfPlayers);
            return (_width * _index < pDepthMap.X && pDepthMap.X < _width * (_index + 1));
        }
#endif

        public void SetCallback(EContentState _state, Func _delegae)
        {
            switch (_state)
            {
                case EContentState.NONE:
                    callbackNoneInit += _delegae;
                    break;
                case EContentState.READY:
                    callbackReadyInit += _delegae;
                    break;
                case EContentState.READY_EVENT0:
                    callbackReadyEventInit0 += _delegae;
                    break;
                case EContentState.READY_EVENT1:
                    callbackReadyEventInit1 += _delegae;
                    break;
                case EContentState.READY_EVENT2:
                    callbackReadyEventInit2 += _delegae;
                    break;
                case EContentState.PLAY_STATE0:
                    callbackPlayStateInit0 += _delegae;
                    break;
                case EContentState.PLAYING:
                    callbackPlayingInit += _delegae;
                    break;
                case EContentState.CLOSE_EVENT0:
                    callbackCloseEventInit0 += _delegae;
                    break;
                case EContentState.CLOSE_EVENT1:
                    callbackCloseInit1 += _delegae;
                    break;
                case EContentState.CLOSE_EVENT2:
                    callbackCloseInit2 += _delegae;
                    break;
            }
        }

        IEnumerator GuidanceProcess()
        {
            UI2DSprite _image = uiController.guidanceImage;
            int _flagImage = 0;
            int _flagWait = 0;
            for (int i = 0, len = words.Length; i < len; ++i)
            {
                float _t = 0f;
                do
                {
                    _t += Time.deltaTime * 1.5f;
                    float _scale = Mathf.Sin(_t * Mathf.PI);
                    _image.cachedTransform.localScale = new Vector3(_scale, 1f, 1f);
                    if ((_flagImage & 0x01 << i) == 0 && _scale < 0.15f)
                    {
                        _flagImage |= 0x01 << i;
                        _image.sprite2D = i < guidanceSprites.Length ? guidanceSprites[i] : null;
                        uiController.wordsOfGuidance.text = words[i];
                    }
                    else if ((0x01 << i & _flagWait) == 0 && 0.995f < _scale)
                    {
                        _flagWait |= 0x01 << i;
                        yield return new WaitForSeconds(0.02f);
                    }
                    yield return null;
                } while (_t < 1.0f);
            }
            currentState = EContentState.PLAYING;
        }
    }
}