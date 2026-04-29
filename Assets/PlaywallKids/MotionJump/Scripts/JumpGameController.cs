//#define USE_TAG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
using ETargetOfAge = ContentsSIModelingInfo.StateTargetOfAge;

namespace ML.PlaywallKids.MotionJump
{
    using Common;

    /// <summary>
    /// 모션 점프 메인 콘트롤러
    /// </summary>
    public class JumpGameController : MonoBehaviour
    {
        enum EState { NONE, READY, START, FIND_USER, END, PLAYING, FIREWORK, GAMEOVER, ARRIVE, FALLEN }

        #region Public variables
        /// <summary>
        /// 메인 홈, 모드 메뉴
        /// </summary>
        public CommonSelectMenu basicMenu;
        public AudioClip sndCheers;
        private static Texture2D[] mImgs;
        /// <summary>
        /// 공용 배경 이미지 참조
        /// </summary>
        public static Texture2D[] Images
        {
            get
            {
                if (mImgs == null)
                {
                    mImgs = new Texture2D[numImage];
                    for (int i = 0; i < mImgs.Length; ++i)
                    { mImgs[i] = Resources.Load<Texture2D>(string.Format("Interaction/Jump/Backgrounds/imgJumpBackground_{0}", i)); }
                    //Debug.Log("number of background images is " + mImgs.Length);
                }
                return mImgs;
            }
        }
        /// <summary>
        /// 아이템 회전 할 때 기준 값
        /// </summary>
        public static float fixedDeltaTime;
        /// <summary>
        /// 배경 패널
        /// </summary>
        public GameObject[] backgrounds;

        /// <summary>
        /// 배경 이미지 개수
        /// </summary>
        [Range(1, 30)]
        public int planeMaxIndex;
        /// <summary>
        /// camera following speed 
        /// </summary>
        [Range(1f, 50f)]
        public float followToPlayerSpeed;
        /// <summary>
        /// BackBoard default Height; 패널의 총 길이
        /// </summary>
        public float planeHeight;
        /// <summary>
        /// 낙하 속도 제한
        /// </summary>
        [Range(-100f, -1f)]
        public float limitVelocity;
        /// <summary>
        /// 키보드 점프 키
        /// </summary>
        public KeyCode jumpKey = KeyCode.LeftControl;
        /// <summary>
        /// 자동 재시작 설정
        /// </summary> 
        public bool restart;                       // true : game restart

        [HideInInspector]
        public MotionManager motionManager;
        /// <summary>
        /// 사용자 Index
        /// </summary>
        [HideInInspector]
        public int userIndex;
        #endregion

        #region Property
        EState currentState // intialize and operate per state
        {
            get
            { return mState; }
            set
            {
                if (value != mState)
                {
                    switch (value)
                    {
                        case EState.READY:
                            Init();
                            break;
                        case EState.FIND_USER:
                            break;
                        case EState.FIREWORK://결승 골인 시 불꽃축제 이펙트
                            fireworkController.Fireworks();
                            break;
                        case EState.NONE: break;
                        case EState.PLAYING: //체험 중
                            player.Animator.enabled = false;
                            heightInfoDisplay.gameObject.SetActive(true);
                            jumpHeightDisplay.gameObject.SetActive(true);
                            player.Jump();
                            break;
                        case EState.START:
                            break;

                        case EState.ARRIVE: //결승 도착
                            bReach = true;
                            player.BeArrived();
                            //Vector3 pos = camController.cam.WorldToViewportPoint( player.cachedTransform.localPosition  );
                            Vector3 pos = camController.cam.WorldToViewportPoint(player.CachedTransform.position);
                            if (pos.x < 0.2f) pos.x = 0.2f;
                            else if (pos.x > 0.8f) pos.x = 0.8f;
                            pos = camController.cam.ViewportToWorldPoint(pos);
                            camController.Zoom(pos + new Vector3(0, 0.5f, 0), ScreenUtil.aspectRatio >= 2 ? 4f : 6f);
                            heightInfoDisplay.gameObject.SetActive(false);
                            jumpHeightDisplay.gameObject.SetActive(false);
                            StartCoroutine(DisplayResultProcess());
                            currentState = EState.FIREWORK;
#if USE_TAG
                        IsTaged = false;
                        NFCUserInfo.SendData(seqOfContent, (player.CntCoin * 3 - (int)fPlayTime));
#endif
                            if (sndCheers != null)
                                UnityEngine.AudioSource.PlayClipAtPoint(sndCheers, Vector3.zero);
                            break;

                        case EState.GAMEOVER://게임오버 팝업창
                                             // Vector3 positionInViewPort = camController.cam.WorldToViewportPoint(player.cachedTransform.position);
                            gameoverController.GameOverDisplay(new Vector3(0.5f, 0, 0), false);
#if USE_TAG
                        IsTaged = false;
                        NFCUserInfo.SendData(seqOfContent, 0);
#endif
                            break;

                        case EState.FALLEN: //바닥에 떨어진 순간
                            camController.CameraShake();
                            currentState = EState.GAMEOVER;

                            break;
                    }
                    mState = value;
                }
            }
        }

        /// <summary>
        /// 콘텐츠 배경의 파티클 생성 및 참조 관리
        /// </summary>
        public JumpEnvironmentController environmentController
        {
            get
            {
                if (mEnvironmentController == null)
                { mEnvironmentController = GetComponentInChildren<JumpEnvironmentController>(); }
                return mEnvironmentController;
            }
        }

        /// <summary>
        /// 플레이어 아바타
        /// </summary>
        public JumpPlayerController player
        {
            get
            {
                if (mPlayer == null)
                { mPlayer = GetComponentInChildren<JumpPlayerController>(); }
                return mPlayer;
            }
        }

        /// <summary>
        /// 사용자 인식 확인 및 튜토리얼 제어 클래스
        /// </summary>
        public UIConnectToUserController connectController
        {
            get
            {
                if (mConnectController == null)
                { mConnectController = GetComponentInChildren<UIConnectToUserController>(); }
                return mConnectController;
            }
        }

        /// <summary>
        /// 콘텐츠 카메라 제어 클래스
        /// </summary>
        public JumpCameraController camController
        {
            get
            {
                if (mCamController == null)
                { mCamController = this.GetComponentInChildren<JumpCameraController>(); }
                return mCamController;
            }
        }

        /// <summary>
        /// 현재 사용안함
        /// </summary>
        public UIInputZoneController inputZoneController
        {
            get
            {
                if (mUIInputZoneDisplay == null)
                { mUIInputZoneDisplay = GetComponentInChildren<UIInputZoneController>(); }
                return mUIInputZoneDisplay;
            }
        }

        /// <summary>
        /// 카메라 페이드 인아웃
        /// </summary>
        public UIViewFadeControllder screenFade
        {
            get
            {
                if (mUIScreenFade == null)
                { mUIScreenFade = GetComponentInChildren<UIViewFadeControllder>(); }
                return mUIScreenFade;
            }
        }

        /// <summary>
        /// 사용자 높이를 디스플레이 하는 클래스
        /// </summary>
        public UIJumpHeightController jumpHeightDisplay
        {
            get
            {
                if (mUIJumpHeightDisplay == null)
                { mUIJumpHeightDisplay = GetComponentInChildren<UIJumpHeightController>(); }
                return mUIJumpHeightDisplay;
            }
        }

        /// <summary>
        /// 현재 높이를 숫자로 출력
        /// </summary>
        public NumericsDisplayer heightInfoDisplay
        {
            get
            {
                if (mHeightInfoDisplay == null)
                { mHeightInfoDisplay = GetComponentInChildren<NumericsDisplayer>(); }
                return mHeightInfoDisplay;
            }
        }

        /// <summary>
        /// 게임오버 팝업창
        /// </summary>
        public UIGameOverController gameoverController
        {
            get
            {
                if (mUIGameoverController == null)
                { mUIGameoverController = GetComponentInChildren<UIGameOverController>(); }
                return mUIGameoverController;
            }
        }

        /// <summary>
        /// 골인 하면 불꽃축제
        /// </summary>
        public JumpFireworkManager fireworkController
        {
            get
            {
                if (mFireworkController == null)
                { mFireworkController = GetComponentInChildren<JumpFireworkManager>(); }
                return mFireworkController;
            }
        }

        /// <summary>
        /// 코인, 아이템, 장애물 등 생성 하는 클래스
        /// </summary>
        public JumpItemManager itemManager
        {
            get
            {
                if (mItemManager == null)
                { mItemManager = GetComponentInChildren<JumpItemManager>(); }
                return mItemManager;
            }
        }

#if USE_TAG
 
#endif
        #endregion

        #region VARIABLES_FOR_PROPERTY
        private JumpEnvironmentController mEnvironmentController;
        private UIJumpHeightController mUIJumpHeightDisplay;
        private UIInputZoneController mUIInputZoneDisplay;
        private NumericsDisplayer mHeightInfoDisplay;
        private UIViewFadeControllder mUIScreenFade;
        private UIConnectToUserController mConnectController;
        private UIGameOverController mUIGameoverController;
        private JumpFireworkManager mFireworkController;
        private JumpCameraController mCamController;
        private JumpPlayerController mPlayer;
        private JumpItemManager mItemManager;
        private EState mState;
        #endregion

        #region PRIVATE_VARIABLES
        /// <summary>
        /// 목표 높이
        /// </summary>
        private float fGoalHeight;
        /// <summary>
        /// 지면으로 부터 배경 패널 게임오브젝트의 초기 높이, 길이의 의미가 아님 initial height of backpanel
        /// </summary>
        private float fDefaultHeight;
        /// <summary>
        /// 체험 시간
        /// </summary>
        private float fPlayTime;
        /// <summary>
        /// 현재 사용을 안함
        /// </summary>
        private float height;
        /// <summary>
        /// 해당 배경에 아이템 생성 비트플래스 체크, 해당 index bit == 0 :아이템 생성 bit == 1 무시
        /// </summary>
        private int bitCheckCreateItem;
        /// <summary>
        /// 아이템 생성 패턴 저장
        /// </summary>
        private int[][] patterns;
        /// <summary>
        /// 골인 도착
        /// </summary>
        private bool bReach;
        /// <summary>
        /// 일정 높이가 되면 이미 생성 된 아이템 비활성화 여부 변수
        /// </summary>
        private bool bDisableItem;
        /// <summary>
        /// 배경 이미지 개수
        /// </summary>
        private static int numImage;

#if USE_TAG
    private int seqOfContent;
    private string userName;
    public bool IsPlaying
    {
        get
        {   return currentState == EState.PLAYING; }
    }
    public bool IsTaged
    {
        get;
        private set;
    }

    private List<NFCUserInfo> listOfNFCInfo;
#endif

        #endregion

        void Awake()
        {

#if USE_TAG
        listOfNFCInfo = new List<NFCUserInfo>();
#endif

            if (planeMaxIndex == 0) planeMaxIndex = 28;
            //최종 높이 설정
            fGoalHeight = (planeMaxIndex) * planeHeight - (planeHeight * (ScreenUtil.aspectRatio >= 2 ? 0.25f : 0.3f));

            // 배경 패널 높이 설정
            fDefaultHeight = backgrounds[0].transform.localPosition.y;
            // 배경 이미지 총 수 설정
            numImage = planeMaxIndex;
            patterns = (BigboardServer.cachedSituationalInfo.targetOfAge == ETargetOfAge.School ? JumpItemPattern.youth_pattern : JumpItemPattern.child_patterns);
            bDisableItem = (BigboardServer.cachedSituationalInfo.targetOfAge == ETargetOfAge.School);
        }

        void OnEnable()
        { currentState = EState.READY; }

        void Init()
        {
            StartCoroutine(FindUserProcess());

            if (motionManager == null) GameObject.FindObjectOfType<MotionManager>();

            // 배경 패널 초기화
            for (int i = 0; i < backgrounds.Length; ++i)
            {
                backgrounds[i].GetComponent<Renderer>().material = backgrounds[i].GetComponent<Renderer>().material;
                float depth = backgrounds[i].transform.localPosition.z;
                //재시작시 높이 재설정
                backgrounds[i].transform.localPosition = new Vector3(0, planeHeight * i + fDefaultHeight, depth);
                //배경 이미지 설정
                backgrounds[i].GetComponent<Renderer>().material.mainTexture = Images[i];
            }

            //초기 높이 배경 파티클 생성
            environmentController.CreateParticleObjects(camController.cam, 0);
            //높이 숫자 초기화
            heightInfoDisplay.ChangeNumerics(NumericSplit.Split(0));
            //아바타 높이 Ui 초기화
            jumpHeightDisplay.UIDisplayJumpHeight((int)player.Height);
            bReach = false;
            screenFade.alpha = 1.0f;

            //사용자 인식 안내문 활성화
            connectController.gameObject.SetActive(true);

            //나머지 객체 비활성화
            gameoverController.gameObject.SetActive(false);
            fireworkController.gameObject.SetActive(false);
            heightInfoDisplay.gameObject.SetActive(false);
            jumpHeightDisplay.gameObject.SetActive(false);

            itemManager.Init();
            player.Init();
            camController.Initialize();
            connectController.Initialize();
            GetComponentInChildren<UIResultController>().Init();

            gameoverController.events = new EventDelegate(this.ReStart);
            bitCheckCreateItem = 0x01 << 0;
            List<GameObject> tempList = itemManager.CreateItem(patterns, (int)(fDefaultHeight / 2), 6, -3);
            itemManager.SetItemList(0, tempList);
            fPlayTime = 0;

#if USE_TAG
        IsTaged = false;
#endif
        }

        void Update()
        {
            if (currentState != EState.PLAYING) return;

            //제한 속도보다 빨리 떨어지면 제한 속도로 보정
            if (player.Rigid.linearVelocity.y < limitVelocity)
            {
                Vector3 velocity = player.Rigid.linearVelocity;
                velocity.y = limitVelocity;
                player.Rigid.linearVelocity = velocity;
            }

            // 바닥 충돌 체크
            if (player.OnTheFloor) currentState = EState.FALLEN;

            //목표 높이
            float goalHeight = planeHeight * planeMaxIndex - fDefaultHeight;

            // 목표 높이 도달 확인
            if (player.Height >= fGoalHeight)
            { currentState = EState.ARRIVE; }

            // 플레이 시간 누적
            fPlayTime += Time.deltaTime;

            //테스트용 입력
            if (Input.GetKeyDown(jumpKey))
            { player.Jump(); }

            heightInfoDisplay.ChangeNumerics(NumericSplit.Split((int)player.Height)); //Display Height Numerics
            jumpHeightDisplay.UIDisplayJumpHeight(player.Height / goalHeight); // Display Height UI

            Vector3 viewDomain = camController.cam.WorldToViewportPoint(player.CachedTransform.position); //check player in domain of view port 

            if (viewDomain.x <= 0 || viewDomain.x > 1f)
            {//if player is out of domain then move oppositely
                Vector3 reversePos = player.CachedTransform.localPosition;
                reversePos.x *= -0.9f;
                player.CachedTransform.localPosition = reversePos;
            }

            ChangeBackground();
        }

        void FixedUpdate()
        {
            if (currentState != EState.PLAYING) return;

            fixedDeltaTime = Time.fixedDeltaTime * 50f;
            if (Input.GetKey(KeyCode.LeftArrow))
            { player.MoveHorizontal(false); }
            else if (Input.GetKey(KeyCode.RightArrow))
            { player.MoveHorizontal(true); }
            CameraFollowToPlayer();
        }

        /// <summary>
        /// 배경 이미지 교체
        /// </summary>
        void ChangeBackground()
        {
            // 현재 사용자가 있는 배경 인덱스
            int iCurrentIndexOfHeight = (int)(player.Height / planeHeight);

            // 현재 높이
            float fCurrentHeight = iCurrentIndexOfHeight * planeHeight + fDefaultHeight;

            // change plane image and position
            if (player.Rigid.linearVelocity.y > 0) // 위로 점프 중
            {
                if (fCurrentHeight <= player.Height) // 현재배경이 위치한 높이 보다 사용자가 더 높으면
                {
                    if (iCurrentIndexOfHeight + 1 < planeMaxIndex)
                    { ++iCurrentIndexOfHeight; }//다음 배경이미지

                    if ((bitCheckCreateItem & 0x01 << iCurrentIndexOfHeight) == 0 && iCurrentIndexOfHeight < (planeMaxIndex - 1))  // bit check creating Items or not
                    {
                        bitCheckCreateItem |= 0x01 << iCurrentIndexOfHeight;
                        List<GameObject> tempList = itemManager.CreateItem(patterns, (int)(iCurrentIndexOfHeight * planeHeight + (fDefaultHeight / 2)), 6, -3f);// item Create
                        itemManager.SetItemList(iCurrentIndexOfHeight, tempList);
                        if (bDisableItem) itemManager.DisableItems(iCurrentIndexOfHeight - 2);
                        tempList = null;
                        environmentController.CreateParticleObjects(camController.cam, iCurrentIndexOfHeight);
                    }
                }
            }
            else // 추락 중
            {
                if (fCurrentHeight > player.Height)
                {
                    if (iCurrentIndexOfHeight - 1 >= 0)
                    { --iCurrentIndexOfHeight; } //이전 배경 이미지 인덱스
                }
            }

            if (iCurrentIndexOfHeight < planeMaxIndex)
            {
                //현재 카메라에서 안보이는 패널의 높이를 이전 또는 다음 높이로 조정
                int panelIndex = iCurrentIndexOfHeight % 2; //(0~1)
                backgrounds[panelIndex].GetComponent<Renderer>().material.mainTexture = Images[iCurrentIndexOfHeight];

                Vector3 panelHeight = backgrounds[panelIndex].transform.localPosition;
                panelHeight.y = (int)(iCurrentIndexOfHeight * planeHeight + fDefaultHeight);
                backgrounds[panelIndex].transform.localPosition = panelHeight;
            }
        }

        /// <summary>
        /// 카메라 이동 함수
        /// </summary>
        void CameraFollowToPlayer()
        {
            float demping = 1f;
            if (player.Rigid.linearVelocity.y < 0)
            {
                demping = player.Rigid.linearVelocity.y / -10;
                ++demping;
            }

            if (player.Height < camController.defaultPosition.y)
            { return; }

            float newHeight = Mathf.Lerp(camController.height, player.Height, Time.fixedDeltaTime * followToPlayerSpeed * demping);
            Vector3 camPos = camController.cachedTransform.localPosition;
            camPos.y = newHeight;
            camController.cachedTransform.localPosition = camPos;
        }

        public bool CheckReach()
        {
            return bReach;
        }

        /// <summary>
        /// 사용자 인식 프로세스
        /// </summary>
        /// <returns></returns>
        IEnumerator FindUserProcess()
        {
            screenFade.FadeEffect(false, 0.0f);
            if (!connectController.gameObject.activeInHierarchy)
                connectController.gameObject.SetActive(true);

#if USE_TAG
        connectController.SetStringWords(0, NFCConstants.DO_TAGGING);
        connectController.SetStringWords(1, "");
        do
        {
            if(listOfNFCInfo.Count > 0 && !IsTaged )
            {
                NFCUserInfo _info = listOfNFCInfo[0];
                listOfNFCInfo.RemoveAt(0);
                userName = _info.userName;
                seqOfContent = _info.seqNo;
                IsTaged = true;
            }
            yield return null;
        } while (!IsTaged);
        connectController.indexOfTagging = 1;
        connectController.SetStringWords(0, @"앞에 서주세요");
        connectController.SetStringWords(1, string.Format("{0}님 환영합니다.", userName));
#endif

            connectController.FindUser();
            do
            {
                yield return new WaitForEndOfFrame();
            } while (!connectController.findCheckComplete);

            connectController.Notice();
            do
            {
                yield return new WaitForEndOfFrame();
            } while (!connectController.maintainConnect);
            currentState = EState.PLAYING;
        }

        /// <summary>
        /// 재시작 
        /// </summary>
        /// <returns></returns>
        IEnumerator ReStartProcess()
        {
            yield return new WaitForSeconds(5f);
            screenFade.FadeEffect(true, 1.0f);
            yield return new WaitForSeconds(1f);
            currentState = EState.READY;
        }

        /// <summary>
        /// 결과창
        /// </summary>
        /// <returns></returns>
        IEnumerator DisplayResultProcess()
        {
            UIResultController result = GetComponentInChildren<UIResultController>();
            result.ShowUp();
            yield return new WaitForSeconds(result.duration);
            result.ChangeTime(fPlayTime);
            yield return new WaitForSeconds(1f);
            NumericsDisplayer.ChangePoint(result.goldPointDisplayer, player.CntCoin);

            float cnt = 0;
            int _score = (player.CntCoin * 3 - (int)fPlayTime);
            yield return new WaitForSeconds(1f);
            if (_score < 0) _score = 10;
            NumericsDisplayer.ChangePoint(result.scorePointDisplayer, _score);
            result.ChangeStarCount(_score);
            do
            {
                cnt += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            } while (cnt < 10f);
            ReStart();
        }

#if USE_TAG
    public void SetNFCValue(NFCUserInfo _value)
    {
        listOfNFCInfo.Add(_value);
    }

    public void SetNFCValue(string _value)
    {
        NFCUserInfo _info = NFCUserInfo.GetNFCUserInfo(_value);
        if(!string.IsNullOrEmpty( _info.userName) )
        {
            listOfNFCInfo.Add(_info);
        }
    }

    public bool EqualUser(string _userName)
    {
        return string.Compare(_userName, this.userName) == 0 ;
    }
#endif

        /// <summary>
        /// 외부에서 사용자 키 입력
        /// </summary>
        /// <param name="inUser"></param>
        /// <param name="isJump"></param>
        /// <param name="_commnad"></param>
        /// <param name="body"></param>
        public void SendCommand(bool inUser, ref bool isJump, string _commnad, Vector3[] body)
        {
            switch (currentState)
            {
                case EState.READY:

#if UNITY_EDITOR
                    if (inUser)
                        Debug.Log("( " + userIndex + ") user is standing in front of the kindect :  " + inUser);
#endif
                    connectController.findUser = inUser;
                    break;

                case EState.PLAYING:
                    switch (_commnad)
                    {
                        case GAME_MESSAGE.LEFT:
                            player.MoveHorizontal(false);
                            break;
                        case GAME_MESSAGE.RIGHT:
                            player.MoveHorizontal(true);
                            break;
                    }
#if UNITY_EDITOR
                    Debug.Log("Play ( " + userIndex + ") user is standing in front of the kindect :  " + inUser);
                    Debug.Log("Jump : " + isJump);
#endif
                    if (isJump)
                    {
                        player.Jump();
                        isJump = false;
                    }

                    //키넥트 모션을 아바타에 적용
                    if (body != null && body.Length > 0)
                    { player.skeleton.MoveJoints(body, true); }

                    break;
                    /*
                case EState.GAMEOVER:
                    break;
                default:
                    break;
                     */

            }
        }


        /// <summary>
        /// 사용 안함
        /// </summary>
        /// <param name="_body"></param>
        /// <param name="_message"></param>
        /// <param name="_counter"></param>
        public void SendCommand(Body _body, string _message, string _counter = null)
        {
            switch (currentState)
            {
                case EState.READY:

                    break;

                case EState.PLAYING:
                    if (_body != null && !string.IsNullOrEmpty(_message))
                    {
                        switch (_message)
                        {
                            case GAME_MESSAGE.LEFT:
                                player.MoveHorizontal(false);
                                break;
                            case GAME_MESSAGE.RIGHT:
                                player.MoveHorizontal(true);
                                break;
                            case GAME_MESSAGE.JUMP:
                                player.Jump();
                                break;
                        }
                    }

                    if (_body != null)
                    {
                        Vector3[] pos = new Vector3[20];
                        foreach (JointType jointType in _body.Joints.Keys)
                        {
                            if ((int)jointType < pos.Length)
                            {
                                var p = _body.Joints[jointType].Position;
                                pos[(int)jointType] = new Vector3(-p.X, p.Y, -p.Z);
                            }
                        }
                        player.skeleton.MoveJoints(pos, true);
                    }

                    break;
                case EState.GAMEOVER:

                    break;

                default:
                    break;

            }
        }

        /// <summary>
        /// 사용안함
        /// </summary>
        /// <param name="backround"></param>
        /// <param name="currentPlaneTextureIndex"></param>
        /// <param name="currentHeightOfPlane"></param>
        /// <param name="speed"></param>
        void ScrollBackground(Transform backround, int currentPlaneTextureIndex, float currentHeightOfPlane, float speed)
        {
            speed = Mathf.Max(-9.8f, speed - Time.deltaTime * 9.8f);

            float delta = speed * 0.25f * Time.deltaTime;

            if (currentPlaneTextureIndex == 0 && delta < 0 && currentHeightOfPlane < -delta)
                delta = -currentHeightOfPlane;
            else if (currentPlaneTextureIndex == 28 && delta > 0 && currentHeightOfPlane + delta > planeHeight)
                delta = planeHeight - currentHeightOfPlane - 0.01f;

            height += delta;
            currentHeightOfPlane += delta;

            backround.localPosition -= Vector3.up * delta;
        }

        public void ReStart()
        {
            if (restart)
            { StartCoroutine(ReStartProcess()); }
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.6f, 0.3f, 0f, 0.4f);
            Vector3 cubePos = new Vector3(-5f, planeHeight * (planeMaxIndex) * 0.5f, -0.5f);
            Gizmos.DrawCube(cubePos, new Vector3(5f, planeHeight * (planeMaxIndex), 0.5f));
        }
#endif
    }

    /*
     * 
           speed = Mathf.Max (-9.8f, speed - Time.deltaTime * 9.8f);

           if (Input.GetKeyDown (jumpKey)) {
               speed = Mathf.Min(14.7f, (speed < 0.0f ? 9.8f : speed + 4.9f));
           }

           float delta = speed * 0.25f * Time.deltaTime;

           if (currentPlaneTextureIndex == 0 && delta < 0 && currentHeightOfPlane < -delta)
               delta = -currentHeightOfPlane;
           else if (currentPlaneTextureIndex == 28 && delta > 0 && currentHeightOfPlane + delta > planeHeight)
               delta = planeHeight - currentHeightOfPlane - 0.01f;

           height += delta;
           currentHeightOfPlane += delta;

           background0.transform.localPosition -= Vector3.up * delta;
           background1.transform.localPosition -= Vector3.up * delta;

           if (currentHeightOfPlane >= planeHeight) {
               currentHeightOfPlane -= planeHeight;

               GameObject temp = background0;
               background0 = background1;
               background1 = temp;

               background1.transform.localPosition += Vector3.up * planeHeight * 2;

               currentPlaneTextureIndex++;
               Texture2D back = Resources.Load<Texture2D>(string.Format("Interaction/Jump/Backgrounds/background_{0}_{0}", 30 - (currentPlaneTextureIndex + 1)));
               var mat = background1.renderer.material;
               mat.mainTexture = back;
               background1.renderer.material = mat;
           }
           else if(currentHeightOfPlane < 0) {
               currentHeightOfPlane += planeHeight;

               GameObject temp = background0;
               background0 = background1;
               background1 = temp;

               background0.transform.localPosition -= Vector3.up * planeHeight * 2;

               currentPlaneTextureIndex--;
               Texture2D back = Resources.Load<Texture2D>(string.Format("Interaction/Jump/Backgrounds/background_{0}_{0}", 30 - currentPlaneTextureIndex));
               var mat = background0.renderer.material;
               mat.mainTexture = back;
               background0.renderer.material = mat;
           }*/





    //////////////////////////////////mine
    /* 
     * 
     *  if (player.rigid.velocity.y > 0) // change plane image and position
                {
                    if (iCurrentHeightIndex * planeHeight + fDefaultHeight < player.height)
                    {
                        int nextIndex = (iCurrentHeightIndex % 2 == 0) ? 1 : 0;
                        Vector3 avobePos = backgrounds[nextIndex].transform.localPosition;
                        avobePos.y = (iCurrentHeightIndex + 1) * planeHeight + fDefaultHeight;
                        backgrounds[nextIndex].transform.localPosition = avobePos;
                    }
                }
                else
                {
                    if (iCurrentHeightIndex * planeHeight + fDefaultHeight >= player.height)
                    {
                        int preIndex = (iCurrentHeightIndex % 2 == 0) ? 1 : 0;
                        Vector3 belowPos = backgrounds[preIndex].transform.localPosition;
                        belowPos.y = (iCurrentHeightIndex - 1) * planeHeight + fDefaultHeight;
                        backgrounds[preIndex].transform.localPosition = belowPos;
                    }
                }
     * 
     * void Update()
        {
            if (inputZoneController.AcceptInput() && Input.GetKeyDown(jumpKey))
            { player.Jump(); }

            iCurrentIndex = (int)(player.height / planeHeight);
            if (player.height <= 0) { iCurrentIndex -= 1; }
            float fJumpPercentage = player.height / (planeHeight * (maxPlaneIndex));
            jumpHeightDisplay.UIDisplayJumpHeight(fJumpPercentage);
            int iBackPanelNum = iCurrentIndex % numBackgrounds;
            switch (player.currentState)
            {
                default:
                    {
                        if (fGoalHeight <= player.height)//be reached
                        {
                            Vector3 goal = player.cachedTransform.localPosition;
                            goal.y = fGoalHeight;
                            player.cachedTransform.localPosition = goal;
                            player.BeReached();
                        }

                        Vector3 nextPosition = Vector3.zero;
                        if (iCurrentIndex != currentPlaneTextureIndex)
                        {

                            nextPosition.y = planeHeight * (iCurrentIndex + 1);
                            if (iBackPanelNum == 2  )
                            { backgrounds[0].transform.localPosition = nextPosition; }
                            else if (iBackPanelNum == 1)
                            { backgrounds[2].transform.localPosition = nextPosition; }
                            else
                            { backgrounds[1].transform.localPosition = nextPosition; }
                            currentPlaneTextureIndex = iCurrentIndex;
                        }
                    }
                    break;

                case PlayerController.Estate.FALLING:
                    if (iCurrentIndex != currentPlaneTextureIndex)
                    {
                        Vector3 prePostion = Vector3.zero;
                        currentPlaneTextureIndex = iCurrentIndex;
                        prePostion.y = planeHeight * (iCurrentIndex - 1);

                        if (iBackPanelNum == 2)
                        { backgrounds[1].transform.localPosition = prePostion; }
                        else if (iBackPanelNum == 1)
                        { backgrounds[0].transform.localPosition = prePostion; }
                        else
                        { backgrounds[2].transform.localPosition = prePostion; }
                    }
                    break;

                case PlayerController.Estate.BE_REACHED:
                    break;
            }

            Vector3 jumpPos = camController.cachedTransform.localPosition;
            int iCurrentDistance = (int)(player.height - camController.height);
            bool bJumping = (player.currentState != PlayerController.Estate.BE_REACHED);
            float iDistance = (bJumping ? betweenDistance + (iCurrentIndex / 2) : 0);

            if (iCurrentDistance > iDistance)
            {   jumpPos.y = Mathf.Lerp(camController.height, player.height - iDistance, Time.deltaTime);  }
            else if (iCurrentDistance < -1 * fallingDistance)
            {   jumpPos.y = Mathf.Lerp(camController.height, player.height + fallingDistance, Time.deltaTime); }
            camController.cachedTransform.localPosition = jumpPos;
            ///*camera look at player 
            Quaternion dir = Quaternion.LookRotation(player.cachedTransform.localPosition - camController.cachedTransform.localPosition);
            camController.cachedTransform.localRotation = dir;
        }
    */
}