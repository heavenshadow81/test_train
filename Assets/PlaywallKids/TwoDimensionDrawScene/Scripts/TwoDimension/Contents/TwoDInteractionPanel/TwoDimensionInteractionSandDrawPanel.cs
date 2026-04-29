
//#define TEST_INPUT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UserType;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class UserTouchInfo
    {
        public float fTime;
        public Vector3 touchPos;

        public UserTouchInfo()
        {
            fTime = 0;
            touchPos = Vector3.zero;
        }
    }

    /// <summary>
    /// 콘텐츠 내 사용자 입력에 상호 작용하는 객체들의 참조 및 이벤트 발생 시 제어하는 클래스
    /// </summary>
    public class TwoDimensionInteractionSandDrawPanel : TwoDimensionInteractionPanel
    {
#if TEST_INPUT
        public TestInpuResponseManager displayer;
#endif
        /// <summary>
        /// Layer index
        /// </summary>
        const int LAYER_NGUI = 11;
        /// <summary>
        /// Layer index
        /// </summary>
        const int LAYER_2D_PANEL = 12;
        /// <summary>
        /// Layer index
        /// </summary>
        const int LAYER_INTERACTION_OBJECTS = 14;
        /// <summary>
        /// 꽃게 크기
        /// </summary>
        public float crabSize = 350;
        /// <summary>
        /// 꽃게 이동 속도
        /// </summary>
        [Range(0, 1000f)]
        public float fMoveSpeed;
        /// <summary>
        /// 꽃게 도망 갈 때 이동 속도
        /// </summary>
        [Range(0, 1000f)]
        public float fRunSpeed;
        /// <summary>
        /// GameObject.Tube
        /// </summary>
        public GameObject btnClear;
        //public AnimationCurve btnAniCurve;
        /// <summary>
        /// 야자수 나무 생성 관리 클래스
        /// </summary>
        public NGUIManufactureManger falmTrees;

        public const string szFilePath = "TwoDimensionContents/Contents/Objects/";
        public const string szParticlePath = "TwoDimensionContents/Contents/Effects/";
        public const string szCrabObj = "objCrab";
        public const string szStarfishObj = "objStarfish";
        public const string szClam = "imgClam";

        private bool canSpawnCrab;
        private int cntCrab;

        #region MANAGEMENT_CLASS
        /// <summary>
        /// 모래 파티클 관리 클래스
        /// </summary>
        SandParticleManager SandParticlesManager
        {
            get
            {
                if (_sandManager == null)
                    _sandManager = new SandParticleManager();
                return _sandManager;
            }
        }
        #endregion MANAGEMENT_CLASS

        #region COLLECTIONS
        /// <summary>
        /// 조개, 불가사리 등 객체 참조 컬렉션
        /// </summary>
        BetterList<UITexture> OceanicObjList
        {
            get
            {
                if (_oceanicObjList == null)
                    _oceanicObjList = new BetterList<UITexture>();
                return _oceanicObjList;
            }
        }

        /// <summary>
        /// 사용자 입력 컬렉션
        /// </summary>
        public Dictionary<int, UserTouchInfo> UserTouches
        {
            get
            {
                if (_userTouches == null)
                    _userTouches = new Dictionary<int, UserTouchInfo>();
                return _userTouches;
            }
        }

        /// <summary>
        /// 꽃게 객체 메모리 풀
        /// </summary>
        private CObjectList<GameObject> objList
        {
            get
            {
                if (_objList == null)
                    InitCrabsPool();
                return _objList;
            }
        }
        #endregion COLLECTIONS

        #region PRIVATE_PROPERTIES
        /// <summary>
        /// 조개 텍스쳐 프리팹
        /// </summary>
        private Texture2D imgClam
        {
            get
            {
                if (_imgClam == null)
                {
                    _imgClam = Resources.Load(szFilePath + szClam) as Texture2D;
                }
                return _imgClam;
            }

        }

        private GameObject CrabPrefab
        {
            get
            {
                if (_crabPrefab == null)
                {
                    _crabPrefab = Resources.Load(szFilePath + szCrabObj) as GameObject;
                    //_crabPrefab.name = "crab";
                }
                return _crabPrefab;
            }
        }

        /// <summary>
        /// 불가사리 프리팹
        /// </summary>
        private GameObject starfishPrefab
        {
            get
            {
                if (_starfishPrefab == null)
                {
                    _starfishPrefab = Resources.Load(szFilePath + szStarfishObj) as GameObject;
                    //_starfishPrefab.name = "starfishPrefab";
                }
                return _starfishPrefab;
            }
        }

        /// <summary>
        /// 꽃게 객체 참조
        /// </summary>
        public BetterList<Crab> ListOfCrabs
        {
            get
            {
                if (_crabs == null)
                    _crabs = new BetterList<Crab>();
                return _crabs;
            }
        }

        private AudioSource mAudio
        {
            get
            {
                if (_audio == null)
                    _audio = GetComponent<AudioSource>();
                return _audio;
            }
        }

        private Transform CachedTransform
        {
            get
            {
                if (_transform == null)
                    _transform = this.transform;
                return _transform;
            }
        }
        #endregion PRIVATE_PROPERTIES

        #region PROPERTY_VARIABLES
        private BetterList<UITexture> _oceanicObjList;
        private CObjectList<GameObject> _objList;
        private Dictionary<int, UserTouchInfo> _userTouches;
        private SandParticleManager _sandManager;
        private BetterList<Crab> _crabs;
        #endregion PROPERTY_VARIABLES

        #region PRIVATE_VARIABLES
        private AudioSource _audio;
        private GameObject _starfishPrefab;
        private GameObject _crabPrefab;
        private Transform _transform;
        private Texture2D _imgClam;
        #endregion PRIVATE_VARIABLES

        #region UNITY_BUILTIN_FUNCTIONS
        void Awake()
        {
            InitCrabsPool();
            SpawningOceanObjects();
            btnClear.transform.localPosition = new Vector3(
                UtilityScript.width * (UtilityScript.ratio >= 2 ? -0.38f : -0.3f), UtilityScript.height * 0.445f, -130f);
            if (falmTrees)
                falmTrees.GenerateObjects();
        }

        protected virtual void OnEnable()
        {
            canSpawnCrab = true;

            //TranslateBtn(true);
            StartCoroutine(GenerateStarfishProcess());
            StartCoroutine(SpawnACrabProcess());
        }

        protected virtual void OnDisable()
        {
            //TranslateBtn(false);
            StopCoroutine(SpawnACrabProcess());
            SandParticlesManager.Destroy();
            if (ListOfCrabs != null)
            {
                for (int i = 0, len = ListOfCrabs.size; i < len; ++i)
                {
                    if (ListOfCrabs[i] != null)
                        ListOfCrabs[i].obj.SetActive(false);
                }
            }
        }

        protected virtual void Update()
        {
            if (CustomInput.touchCount > 0)
            {
#if TEST_INPUT
            if (displayer!= null)
                displayer.InitInfo();
#endif
                TouchInfo[] infos = CustomInput.touches;
                for (int i = 0; i < CustomInput.touchCount; ++i)
                {
                    TouchInfo touch = infos[i];
                    //screen pos to NGUI screen pos
                    Vector3 curPos = UtilityScript.WindowToNGUI(touch.position);

                    int _id = touch.userId;

#if TEST_INPUT
                if(displayer!= null )
                {
                    int _fingerID = touch.id;
                    if (CustomInput.startTimeDic.ContainsKey(_fingerID))//터치 ID별로 확인
                    {
                        // TimeSpan 구조체 (.NET Framework 4.6 and 4.5 API)
                        // TimeSpan 객체는 경과된 시간을 나타낸다( A TimeSpan object represents a time interval (duration of time or elapsed time) )
                        // 현재 시스템 시간과 등록 된 ID의 최초시간을 비교해서 경과 시간을 확인 한다.
                        System.TimeSpan ts = System.DateTime.Now - CustomInput.startTimeDic[_fingerID];
                        // TimeSpan.TotalMilliseconds - 밀리초의 정수 부분과 소수 부분으로 표시된 현재 TimeSpan 구조체의 값
                        displayer.SetInfo(_fingerID, (int)ts.TotalMilliseconds, touch.phase);//터치 ID별로 터치 상태와 경과 시간을 등록
                    }
                }
#endif
                    switch (touch.phase)
                    {
                        case TouchInfo.Phase.Begin:
                            {
                                Ray ray = UtilityScript.NGUICamera.ScreenPointToRay(touch.position);
                                RaycastHit hit;

                                if (Physics.Raycast(ray, out hit, 100f, 0x01 << LAYER_INTERACTION_OBJECTS | 0x01 << Constante.TWODIMENSION_PANEL | 0x01 << Constante.NGUI))
                                {
                                    Crab crab = hit.collider.gameObject.GetComponent<Crab>();
                                    if (crab != null)
                                    {
                                        AudioSource.PlayClipAtPoint(mAudio.clip, crab.cachedTransform.position);
                                        OnTabCrab(crab);
                                    }
                                    else // 불가사리, 튜브 등
                                    { hit.collider.SendMessage("Touch", SendMessageOptions.DontRequireReceiver); }
                                }

                                UserTouchInfo finger = new UserTouchInfo();
                                finger.touchPos = curPos;
                                finger.fTime = Time.deltaTime;
                                if (UserTouches.ContainsKey(_id))
                                { UserTouches[_id] = finger; }
                                else
                                { UserTouches.Add(_id, finger); }
                            }
                            break;
                        case TouchInfo.Phase.Move:
                        case TouchInfo.Phase.Stay:
                            {
                                UserTouchInfo userTouch;

                                if (!UserTouches.ContainsKey(_id))
                                {
                                    userTouch = new UserTouchInfo();
                                    userTouch.touchPos = curPos;
                                    UserTouches.Add(_id, userTouch);
                                    return;
                                }
                                else
                                { userTouch = UserTouches[_id]; }

                                float dis = (curPos - userTouch.touchPos).sqrMagnitude;

                                // 사용자 입력 이벤트
                                // 일정거리 이내 2초 이상 사용자가 터치를 할 경우 그 위치에 꽃게가 등장함
                                if (dis < 3f) // 사용자 터치가 일정거리 이내
                                {
                                    userTouch.fTime += Time.deltaTime; //터치 시간 측정

                                    if (0.5f < userTouch.fTime && userTouch.fTime < 2f)
                                    { //모래 이펙트 재생 
                                        SandParticlesManager.PlayEmit(_id, CachedTransform, curPos);
                                    }
                                    else if (userTouch.fTime > 2f)
                                    { // 일정 시간 이상이면 꽃게 생성 및 모래파티클 재생 정지
                                        userTouch.fTime = 0;
                                        OnDigFinished(curPos);
                                        SandParticlesManager.StopEmit(_id);
                                    }
                                }
                                else
                                {//일정 거리 이상이면 모든 터치 이벤트 종료
                                    userTouch.fTime = 0;
                                    userTouch.touchPos = curPos;
                                    SandParticlesManager.StopEmit(_id);
                                }

                                UserTouches[_id] = userTouch;

                            }
                            break;

                        case TouchInfo.Phase.End:
                        case TouchInfo.Phase.Cancel:
                            {
                                if (UserTouches.ContainsKey(_id))
                                {
                                    UserTouchInfo user = UserTouches[_id];
                                    user.fTime = 0;
                                    user.touchPos = Vector3.zero;
                                    UserTouches[_id] = user;
                                }

                                SandParticlesManager.StopEmit(_id);
                            }
                            break;
                    }
                }
            }

            // 동적으로 삭제 된 꽃게 확인 및 꽃게 참조 리스트 관리
            for (int i = 0, num_crab = ListOfCrabs.size; i < num_crab; ++i)
            {
                // 해당 인덱스 참조가 null이면 해당 index에 참조 재할당 및 리스트 크기 조절
                if (ListOfCrabs[i] == null)
                {
                    if (i >= ListOfCrabs.size)
                    {
                        num_crab = ListOfCrabs.size;
                        break;
                    }
                    else
                    {
                        int idx = ListOfCrabs.size - 1;
                        while (ListOfCrabs[idx] == null)
                        {
                            --idx;
                            if (idx <= i) return;
                        }
                        ListOfCrabs[i] = ListOfCrabs[idx];
                        ListOfCrabs.size = idx;
                    }
                }

                // 해당 꽃게가 비활성이면 새로운 꽃게 생성
                if (!ListOfCrabs[i].obj.activeInHierarchy)
                {
                    ListOfCrabs[i] = ListOfCrabs[num_crab - 1];
                    ListOfCrabs.Pop();
                }
            }
        }
        #endregion UNITY_BUILTIN_FUNCTIONS

        #region PUBLIC_FUNCTIONS
        /// <summary>
        /// 파도 이벤트 시작 준비
        /// </summary>
        /// <returns></returns>
        public override bool StateEventReady()
        {
            canSpawnCrab = false;
            int cntInactive = 0;
            for (int cnt = 0, length = ListOfCrabs.size; cnt < length; ++cnt)
            {

                if (ListOfCrabs[cnt] == null || !ListOfCrabs[cnt].obj.activeInHierarchy)
                {
                    cntInactive++;
                    continue;
                }
                ListOfCrabs[cnt].eState = EState.EVENT;
            }

            if (cntInactive == ListOfCrabs.size)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 파도 이벤트 중
        /// </summary>
        /// <returns></returns>
        public override bool StateEventActivates()
        {

            canSpawnCrab = false;

            return true;
        }

        /// <summary>
        /// 콘텐츠 체험 중
        /// </summary>
        /// <returns></returns>
        public override bool StateInPlay()
        {
            if (!canSpawnCrab)
                canSpawnCrab = true;

            return false;
        }

        /// <summary>
        /// 꽃게 터치 이벤트
        /// </summary>
        /// <param name="crab"></param>
        public virtual void OnTabCrab(Crab crab)
        {
            crab.bTouch = true;
        }

        public virtual void OnDigFinished(Vector3 curPos)
        {
            SpawnCrab(curPos, GetRandomSize(), fMoveSpeed, fRunSpeed);
        }

        /// <summary>
        /// 꽃게 생성
        /// </summary>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        /// <param name="moveSpeed"></param>
        /// <param name="runSpeed"></param>
        /// <returns></returns>
        public virtual Crab SpawnCrab(Vector3 position, Vector3 scale, float moveSpeed, float runSpeed)
        {
            if (cntCrab > 100) return null;

            GameObject temp = objList.GetObject();
            if (temp == null || !temp.GetComponent<Crab>())
                return null;

            Crab crab = temp.GetComponent<Crab>();
            crab.name = "_crab";
            crab.iID = ++cntCrab % 100;

            crab.cachedTransform.localScale = scale;
            crab.cachedTransform.localPosition = position;
            crab.fMoveSpeed = moveSpeed;
            crab.fRunSpeed = runSpeed;

            crab.obj.SetActive(true);

            return crab;
        }
        #endregion PUBLIC_FUNCTIONS

        #region PRIVATE_FUNCTIONS
        private Vector3 GetRandomPosition()
        {
            float fHalfWidth = UtilityScript.width / 2;
            float fHalfHeight = UtilityScript.height / 2;

            Vector3 pos = new Vector3(Random.Range((fHalfWidth * -1) + 30, fHalfWidth - 30), Random.Range((-1 * fHalfHeight) + 50, fHalfHeight - 50f), 0);

            return pos;
        }

        private Vector3 GetRandomSize()
        {
            crabSize = Random.Range(300f, 400f);
            return Vector3.one * crabSize;
        }

        /// <summary>
        /// 조개 생성
        /// </summary>
        private void SpawningOceanObjects()
        {
            float _fTempSize = 0;

            for (int i = 0; i < 8; ++i)
            {
                UITexture _clam = null;
                _clam = NGUITools.AddChild<UITexture>(cPanel.cachedGameObject);
                _clam.mainTexture = imgClam;
                _fTempSize = UtilityScript.height * 0.05f;
                _clam.SetDimensions(Mathf.RoundToInt(_fTempSize), Mathf.RoundToInt(_fTempSize));
                float x = Random.Range(UtilityScript.width * -0.4f, UtilityScript.width * 0.4f);
                float y = Random.Range(UtilityScript.height * -0.4f, UtilityScript.height * 0.1f);
                _clam.transform.localPosition = new Vector3(x, y, 0f);
                _clam.transform.localEulerAngles = new Vector3(0, 0, Random.Range(-180f, 90f));
                OceanicObjList.Add(_clam);
            }
        }

        /// <summary>
        /// 꽃게 객체 메모리풀 초기화
        /// </summary>
        private void InitCrabsPool()
        {
            _objList = new CObjectList<GameObject>(
                7,
                () =>
                {
                    GameObject go = NGUITools.AddChild(cPanel.cachedGameObject, CrabPrefab);
                    if (CrabPrefab == null)
                    {
                        go = Resources.Load(szFilePath + szCrabObj) as GameObject;
                        go = NGUITools.AddChild(cPanel.cachedGameObject, go);
                    }
                    go.SetActive(false);
                    go.transform.localPosition = new Vector3(0, 0, 500f);

                    return go;
                },
            (GameObject go) =>
            {
                return !go.activeInHierarchy;
            }
            );
        }

        /// <summary>
        /// 불가사리 생성 코루틴
        /// </summary>
        /// <returns></returns>
        private IEnumerator GenerateStarfishProcess()
        {
            int len = Random.Range(6, 8);
            float _fTempSize = 0;
            yield return new WaitForSeconds(1f);
            do
            {
                GameObject _tempObj = NGUITools.AddChild(cPanel.cachedGameObject, starfishPrefab);
                _tempObj.SetActive(true);
                float x = Random.Range(UtilityScript.width * -0.48f, UtilityScript.width * 0.48f);
                float y = Random.Range(UtilityScript.height * -0.45f, UtilityScript.height * 0.1f);
                _tempObj.transform.localPosition = new Vector3(x, y, 0f);
                _fTempSize = UtilityScript.height * Random.Range(0.15f, 0.2f);
                _tempObj.transform.localScale = new Vector3(_fTempSize, _fTempSize, 1f);
                _tempObj.transform.localRotation = Quaternion.AngleAxis(Random.Range(0f, 180f), Vector3.zero);
                OceanicObjList.Add(_tempObj.GetComponent<UITexture>());

                float waitTime = Random.Range(0.8f, 1.2f);
                yield return new WaitForSeconds(waitTime);//순차적으로 생성
                len--;
            } while (len > 0);
        }

        /// <summary>
        /// 꽃게 객체 생성 코루틴
        /// </summary>
        /// <returns></returns>
        private IEnumerator SpawnACrabProcess()
        {
            float elapsedTime = 0; // 경과 시간
            float waitTime = 5f; // 다음 꽃게 생성 시간
            int num_max = 8; // 꽃게 최대 생성 수
            GameObject obj = this.gameObject;
            do
            {
                while (!canSpawnCrab)
                { yield return new WaitForFixedUpdate(); }

                elapsedTime += Time.deltaTime;
                if (elapsedTime >= waitTime || ListOfCrabs.size < 5)
                {
                    elapsedTime = 0;
                    waitTime = Random.Range(1f, 2f);

                    if (num_max > ListOfCrabs.size)
                    {
                        //if (objList.getObject() == null) Debug.Log("objList.getObject() null");
                        //if (objList.getObject().GetComponent<Crab>() == null) Debug.Log("objList.getObject().GetComponent<Crab>() null");

                        Crab crab = SpawnCrab(GetRandomPosition() + new Vector3(0, 0, -70f), GetRandomSize(), fMoveSpeed, fRunSpeed);
                        if (crab != null)
                            ListOfCrabs.Add(crab);
                    }
                }

                yield return new WaitForSeconds(waitTime);
            } while (obj && obj.activeInHierarchy);
        }
        #endregion PRIVATE_FUNCTIONS
    }
}