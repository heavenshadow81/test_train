#define USING_COLLIDER_2D

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UserType;
using com.Loxwell.File;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    using KeyCollection = System.Collections.Generic.Dictionary<int, FutsalBallObject>.KeyCollection;

    /// <summary>
    /// 사용자 입력과 콘텐츠 내 객체와 상호 작용을 제어하는 클래스
    /// </summary>
    public class TwoDimensionSandPrintPanel : TwoDimensionPanel
    {
        enum EUIType { ARROW = 0, FOOT = 1, GAUGE = 2, BALL_TAIL = 3, LENGTH = 4 }
        const int MAX_VALUE = 300;
#if USING_CANVAS
    [HideInInspector]
    public new CanvasForStamp cSketchCanvas;
    ColorTable colorTable = new ColorTable();
#endif
        /// <summary>
        /// 콘텐츠 내 각종 파티클 관리 클래스
        /// </summary>
        public UIParticleManager particleManager;
        /// <summary>
        /// 축구공 궤적을 따라 이동하는 불꽃 파티클 제어 클래스
        /// </summary>
        public UIParticleManager fireballLineManager;
        /// <summary>
        /// 축구공 불꽃 파티클 제어
        /// </summary>
        public UIParticleManager fireballManager;
        /// <summary>
        /// 콘텐츠 내 UI 객체 관리 클래스
        /// </summary>
        public UIManager uiManager;
        /// <summary>
        /// 발자국 객체 생성 및 참조 관리
        /// </summary>
        public ObjectPool footManager;
        /// <summary>
        /// 콘텐츠 내 야자수 나무 생성
        /// </summary>
        public NGUIManufactureManger palmTreeManager;

        public TrailRenderer lineRenderPrefab;
        public TrailRenderer sandRenderPrefab;
        public AudioClip sndStamp;
        /// <summary>
        /// 공의 순간 가속력
        /// </summary>
        [Range(50, 350)]
        public int dragPow;

        /// <summary>
        /// 사용자 터치
        /// </summary>
        Dictionary<int, UserInputInfo> touches;
        /// <summary>
        /// 사용자 터치 ID 별 공 객체 Dictionary
        /// </summary>
        Dictionary<int, FutsalBallObject> ballPerBallID;
        /// <summary>
        /// 공ID 별 공의 밝은 빛의 형태로 궤적을 그리는 라인렌더러
        /// </summary>
        ObjectDictionaryPool<int, TrailRenderer> lineRenderDic;
        /// <summary>
        /// 공ID 별 고의 궤적 아래에 그려지는 모래 자국 라인 렌더러
        /// </summary>
        ObjectDictionaryPool<int, TrailRenderer> sandRenderDic;

        Brush[] footStamps = new Brush[2];
        string[] fileNames = new string[] { FileName.FootStamp, FileName.RightFootStamp };

        public override void Awake()
        {
            //base.Awake();
            //base.cSketchCanvas.gameObject.SetActive(false);
#if USING_GAUGE
        uiManager.GenerateDictionary((int)EUIType.GAUGE, uiManager.prefabs[(int)EUIType.GAUGE]);
#endif

#if USING_ARROW
        uiManager.GenerateDictionary((int)EUIType.ARROW, uiManager.prefabs[(int)EUIType.ARROW]);
#endif

#if USING_CANVAS
        for (int i = 0; i < footStamps.Length; ++i)
        {
            footStamps[i] = CustomBrush.brush.RandomColorStamp(fileNames[i]);
            footStamps[i].shapeDynamicComponent.angleControl = AngleControl.Angle;
            footStamps[i].shapeDynamicComponent.enable = true;
            footStamps[i].colorDynamicComponent.enable = false;
            footStamps[i].diameter = 100f;
        }

        base.cSketchCanvas.gameObject.SetActive(false);
        cSketchCanvas = CanvasForStamp.CreateCanvas<CanvasForStamp>(cPanel.gameObject, new Vector2(4096, 2048));
        //  cSketchCanvas.brush = GetBrush(0);
#endif
        }

        Brush GetBrush(int _index)
        {
            if (_index >= footStamps.Length) _index = 0;
            return footStamps[_index];
        }

        void OnDisable() //do not delete
        {
            foreach (KeyValuePair<int, FutsalBallObject> kv in ballPerBallID)
            {
                GameObject.Destroy(kv.Value.gameObject);
            }

            sandRenderDic.Destroy();
            lineRenderDic.Destroy();
            sandRenderDic = null;
            lineRenderDic = null;

            ballPerBallID.Clear();
            touches.Clear();

            touches = null;
            ballPerBallID = null;
        }

        /// <summary>
        /// 공의 이동에 따라 파티클, TrailRenderer 이동 처림
        /// </summary>
        void BallUpdate()
        {
            int _len = ballPerBallID.Count;
            List<int> deleteList = new List<int>();
            if (_len > 0)
            {
                foreach (KeyValuePair<int, FutsalBallObject> _element in ballPerBallID)
                {
                    FutsalBallObject _ball = _element.Value;

#if USING_CANVAS
                if(_ball.velocity == Vector2.zero)
                {
                   // _ball.tail = null;

                    if (cSketchCanvas.brushForTouchDict.ContainsKey(_ballid))
                        cSketchCanvas.brushForTouchDict.Remove(_ballid);

                    continue;
                }
#endif

                    if (_ball.IsMoving)// 공이 이동 중
                    {
                        if (_ball.beStrong) //공의 힘이 강함
                        {
                            Vector3 particlePos = _ball.cachedTransform.localPosition;
                            particlePos.z -= 105f;
                            fireballLineManager.Emitt(UIParticleManager.instance.CachedTransform, particlePos, true, true, _ball.id, 0);// 불꽃 라인 파티클
                        }
                        else
                        {
                            //공의 힘이 약함
                            if (!lineRenderDic.activeDic.ContainsKey(_ball.id))
                            {
                                TrailRenderer r = lineRenderDic.GetObejct(_ball.id);
                                r.transform.parent = transform;
                            }
                            //공의 궤적 TrailRenderer 이동 좌표 설정
                            lineRenderDic.activeDic[_ball.id].transform.localPosition = _ball.cachedTransform.localPosition;
                        }

                        // 모래 자국 TrailRenderer 이동 좌표 설정
                        TrailRenderer _sandMark = null;
                        if (sandRenderDic.activeDic.ContainsKey(_ball.id))
                        {
                            _sandMark = sandRenderDic.activeDic[_ball.id];
                        }
                        else
                        {
                            _sandMark = sandRenderDic.GetObejct(_ball.id);
                            _sandMark.transform.parent = this.transform;
                        }
                        _sandMark.transform.localPosition = _ball.cachedTransform.localPosition;
                        // fireballLineManager.Disable(_ball.id);
                    }
                    else
                    {

                        if (!_ball.isGrabbing && !_ball.hadReleased)
                        {
                            /*
                            if (lineRenderDic.activeDic.ContainsKey(_ball.id))
                            {
                                lineRenderDic.activeDic[_ball.id].gameObject.SetActive(false);
                                lineRenderDic.activeDic.Remove(_ball.id);
                            }*/
                            fireballLineManager.Disable(_ball.id);
                            //if current state is release then remove ball from dictionary

                            deleteList.Add(_ball.id);
                        }
                    }
#if USING_FOOTSTAMP
                if(_ball.bStamp)
                {//궤적
					Color c = colorTable.GetRandomColor();
					if ((c.r + c.g + c.b) == 3f)
					{
						c.g -= 0.8f;
						c.b -= 0.8f;
					}

					int _footIndex = _ball.footIndex;
                    Vector2 _stampPos = _ball.cachedTransform.localPosition;
#if USING_CANVAS
                    _stampPos.x = UtilityScript.width * 0.5f + _stampPos.x;
                    _stampPos.y = UtilityScript.height * 0.5f + _stampPos.y;
#else
					
#endif

#if USING_CANVAS
                    cSketchCanvas.brush = GetBrush(_footIndex);
                    Brush b = cSketchCanvas.CheckBrush(_ballid, fileNames[_footIndex]);
                    
                    b.color = c;
                    b.angle = _ball.angle;
                    cSketchCanvas.Stamp(b, _stampPos);
#endif

                }
#endif
                }
            }

            if (deleteList.Count > 0)
            {
                for (int i = 0, len = deleteList.Count; i < len; ++i)
                {
                    ballPerBallID.Remove(deleteList[i]);
                }
            }
            lineRenderDic.CheckActiveObjProcess();
        }

        void FixedUpdate()
        {

        }

        /// <summary>
        /// 사용자 입력 처리
        /// </summary>
        void Update()
        {
            if (CustomInput.touchCount > 0)
            {
                for (int i = 0, len = CustomInput.touchCount; i < len; ++i)
                {
                    TouchInfo _currentUserInput = CustomInput.touches[i];
                    Ray ray = UtilityScript.NGUICamera.ScreenPointToRay(_currentUserInput.position);
#if USING_COLLIDER_2D
                    RaycastHit2D rayHit = Physics2D.Raycast(ray.origin, ray.direction, 200f);

#endif
                    switch (_currentUserInput.phase)
                    {
                        case TouchInfo.Phase.Begin:
                            InputBegin(rayHit, _currentUserInput);
                            break;

                        case TouchInfo.Phase.Move:
                            InputMove(rayHit, _currentUserInput);
                            break;

                        case TouchInfo.Phase.End:
                            InputEnd(rayHit, _currentUserInput);
                            break;
                    }
                }
            }
            BallUpdate();
        }

        public void Pause()
        {
            Time.timeScale = 0;
        }

        /// <summary>
        /// Collection 및 참조 변수 초기화
        /// </summary>
        public void OnEnable()
        {
            touches = new Dictionary<int, UserInputInfo>();
            ballPerBallID = new Dictionary<int, FutsalBallObject>();
            sandRenderDic = new ObjectDictionaryPool<int, TrailRenderer>(sandRenderPrefab);
            lineRenderDic = new ObjectDictionaryPool<int, TrailRenderer>(lineRenderPrefab);
            lineRenderDic.parent = this.transform;
            sandRenderDic.parent = this.transform;

            List<GameObject> palmTrees = palmTreeManager.GenerateObjects();
            for (int i = 0, len = palmTrees.Count; i < len; ++i)
            {
                BoidController.instance.SetBoid(palmTrees[i].GetComponent<BoidAgent>());
            }
        }

        protected override Brush GetBrush()
        {
            if (cStampBush == null)
            {
                cStampBush = CustomBrush.brush.RandomColorStamp(FileName.FootStamp);
            }

            cStampBush.color = new Color(Random.Range(0.1f, 0.95f), Random.Range(0.1f, 0.95f), Random.Range(0.1f, 0.95f));
            cStampBush.colorDynamicComponent.hueJitter = Random.Range(0, 1f);
            cStampBush.colorDynamicComponent.saturationJitter = Random.Range(0, 1f);
            cStampBush.colorDynamicComponent.brightnessJitter = Random.Range(0, 1f);
            cStampBush.shapeDynamicComponent.angleControl = AngleControl.Off;
            cStampBush.shapeDynamicComponent.angleJitter = Random.Range(0, 1f);

            return cStampBush;
        }

        // 2D 평면 이동을 3D 회전값으로 변환 하는 함수
        float Dot(Vector2 dir)
        {
            float dot = Vector2.Dot(Vector2.up, dir);
            float angle = 90 - 90 * dot;
            if (dir.x < 0)
            { angle = 360 - angle; }

            return angle;
        }

        public new void ClearCanvas()
        {
            cSketchCanvas.ClearCanvas();
        }

        /// <summary>
        /// 사용자 터치 시작
        /// </summary>
        /// <param name="_rayHit"></param>
        /// <param name="_info"></param>
        void InputBegin(RaycastHit2D _rayHit, TouchInfo _info)
        {
            if (_rayHit.collider == null)
            {
                //screen pos -> NGUI Screen Pos 좌표 변환
                Vector2 _pos = UtilityScript.WindowToNGUI(_info.position);

                GetFootImg(_pos, Random.Range(0, 10) % 2, Random.Range(0f, 359f));
                if (sndStamp != null)
                    AudioSource.PlayClipAtPoint(sndStamp, Vector2.zero);
                return;
            }

            //터치 ID
            int _touchId = _info.id;
            if (_rayHit.collider.tag == FutsalBallObject.BALL)
            {
                FutsalBallObject _ball = _rayHit.collider.GetComponent<FutsalBallObject>();
                if (!ballPerBallID.ContainsKey(_ball.id))
                    ballPerBallID.Add(_ball.id, _ball);

                _ball.Grab();
                _ball.Sleep();

                if (!touches.ContainsKey(_touchId))
                { // new Touch ID
                    float diameter = _ball.cachedTransform.localScale.x;
                    foreach (KeyValuePair<int, UserInputInfo> kv in touches)
                    {
                        Vector2 dir = _info.position - kv.Value.oriPos;
                        if (dir.sqrMagnitude <= diameter * diameter)
                            return; //avoid overlapping input between touches
                    }
                    touches.Add(_touchId, new UserInputInfo(ETarget.NONE));
                }
#if USING_GAUGE
            uiManager.DisplayUIObject(_touchId, (int)EUIType.GAUGE, UtilityScript.WindowToNGUI(_currentUserInput.position), Time.deltaTime);
#endif
                // 사용자 입력 정보 초기화
                UserInputInfo _touchInfo = touches[_touchId];
                _touchInfo.oriPos = _touchInfo.endPos = _info.position;
                _touchInfo.targetType = ETarget.BALL;
                _touchInfo.target = _rayHit.transform;
                _touchInfo.component = (Component)_ball;
                _ball.touchID = _touchId;

                ballPerBallID[_ball.id] = _ball;

                particleManager.Emitt(particleManager.CachedTransform, _ball.cachedTransform.localPosition, true, true, _touchId, 4);
                touches[_touchId] = _touchInfo;
            }
        }

        /// <summary>
        /// 사용자 입력 이동 또는 제자리
        /// </summary>
        /// <param name="_rayHit"></param>
        /// <param name="_info"></param>
        void InputMove(RaycastHit2D _rayHit, TouchInfo _info)
        {
            if (!touches.ContainsKey(_info.id)) return;

            int _touchId = _info.id;
            UserInputInfo _userInfo = touches[_touchId];
            if (_userInfo.targetType == ETarget.BALL)
            {
                Transform _ball = _userInfo.target;
                _ball.GetComponent<Rigidbody2D>().Sleep();

                Vector2 _currentPos = _info.position;
                Vector2 dir = _currentPos - _userInfo.oriPos;
                _userInfo.endPos = _currentPos;

#if USING_ARROW
            float _scalar = _ball.localScale.x * 0.5f;

            Vector2 normalVector = dir.normalized;
            UIArrow arrow = (UIArrow)uiManager.DisplayUIObject(_touchId, (int)EUIType.ARROW, _ball.localPosition + (Vector3)(normalVector * _scalar) + new Vector3(0, 0, -150f), Dot(normalVector));
            dir += normalVector * _scalar;

            int length = (int)dir.magnitude;

            FutsalBallObject _b = (FutsalBallObject)_userInfo.component;
            if (_b!=null)
            {

                if (length > _scalar * 2)
                {
                    _b.beStrong = !(length < LENGTH);
                    if (_b.beStrong) //High
                    {
                        arrow.img.height = LENGTH;
                        arrow.img.width = LENGTH;
                        fireballManager.Emitt(particleManager.cachedTransform, _ball.localPosition + new Vector3(0, 0, -150f), true, true, _touchId, 0);
                        
                    }
                    else //Normal 
                    {
                        arrow.img.height = length;
                        int _w = length * 3 ;
                        arrow.img.width = LENGTH > _w ? _w : LENGTH;
                        fireballManager.Disable(_touchId);
                    }
                }
                else
                {
                    arrow.img.width = 0;
                    arrow.img.height = 0;
                }
#else
                FutsalBallObject _b = (FutsalBallObject)_userInfo.component;
                if (_b != null)
                {
                    float _pow = _userInfo.value;

                    //공에 가할 힘 축적
                    _pow += Time.deltaTime * dragPow;
                    if (_pow > MAX_VALUE)
                    {
                        _pow = MAX_VALUE;
                        _b.beStrong = true;
                        fireballManager.Emitt(particleManager.CachedTransform, _ball.localPosition + new Vector3(0, 0, -150f), true, true, _touchId, 0);
                    }
                    _userInfo.value = _pow;
#endif
                    particleManager.Emitt(particleManager.CachedTransform, _ball.localPosition + new Vector3(0, 0, -150f), true, true, _touchId, 4);
                }

#if USING_GAUGE
        uiManager.DisplayUIObject(_touchId, (int)EUIType.GAUGE, _ballPos, Time.deltaTime);
#endif
                touches[_touchId] = _userInfo;
            }
        }

        /// <summary>
        /// 사용자 터치 종료 시
        /// </summary>
        /// <param name="_rayHit"></param>
        /// <param name="_info"></param>
        void InputEnd(RaycastHit2D _rayHit, TouchInfo _info)
        {
            if (!touches.ContainsKey(_info.id)) return;

            int _touchId = _info.id;
            UserInputInfo _inputInfo = touches[_touchId];

            if (_inputInfo.targetType == ETarget.BALL)
            {
                float _pow = 0f;
#if USING_ARROW
           
            UIArrow arrow =(UIArrow) uiManager.GetObject(_touchId, (int)EUIType.ARROW) ;
            
            if(arrow!= null)
            {
                _pow = arrow.img.height;

#else
                {
                    // 공에 가할 힘
                    _pow = touches[_touchId].value;
#endif

                    FutsalBallObject _ball = (FutsalBallObject)_inputInfo.component;
#if USING_GAUGE
                float _clicktime = uiManager.GetObject(_touchId, (int)EUIType.ARROW).value;
                ballDic[_touchId].rigid.AddForce(_dir.normalized * _clicktime * _len, ForceMode2D.Force);
#else
                    if (_ball != null) // 사용자가 가르키는 방향으로 공을 이동 시킴
                    {
                        _ball.ReleaseGrab();
                        _ball.AddForce((_inputInfo.endPos - _inputInfo.oriPos).normalized * _pow * 0.03f);

                        if (!ballPerBallID.ContainsKey(_ball.id))
                        {
                            ballPerBallID.Add(_ball.id, _ball);
                        }
                    }
#endif
                }

#if USING_ARROW
            uiManager.DisableUIObject(_touchId);
#endif
                particleManager.Disable(_touchId);
                fireballManager.Disable(_touchId);
            }
            touches.Remove(_touchId);
        }

        /// <summary>
        /// 발바닥 도장 객체 생성 및 초기화
        /// </summary>
        /// <param name="_pos"></param>
        /// <param name="_footIndex"></param>
        /// <param name="_angle"></param>
        /// <returns></returns>
        UIFoots GetFootImg(Vector3 _pos, int _footIndex, float _angle)
        {
            GameObject foot = footManager.GetObejct();
            UIFoots img = foot.GetComponent<UIFoots>();
            if (img != null)
            {
                img.Value = _footIndex;
                img.CachedTransform.localPosition = _pos;
                img.CachedTransform.localEulerAngles = new Vector3(0, 0, -_angle);
                img.imgFoot.color = ColorTable.GetColor();
            }
            return img;
        }
    }
    /*
            if (cStampBush == null)
            {  
                cStampBush = CustomBrush.brush.RandomColorStamp(FileName.FootStamp);
                cStampBush.shapeDynamicComponent.angleControl = AngleControl.Angle;
                cStampBush.shapeDynamicComponent.enable = true;
                cStampBush.colorDynamicComponent.enable = false;
                cStampBush.diameter = _diameter;
            }
          * 
          *    return cStampBush;
          */

    /*
                        case TouchInfo.Phase.Move:
                            if (!inputInfoPerUser.ContainsKey(_userId)) return;

                            Debug.LogError("Clicking _userId : " + _userId);
                            UserInputInfo _info = inputInfoPerUser[_userId];
                            if (_info.targetKind == ETarget.BALL)
                                inputInfoPerUser[_userId].inputCoordis[1] = _currentUserInput.position;
                            if ((_info.target != null) && (_info.targetKind == ETarget.OBSTRUCTION))
                            {
                                Vector2 _targetPos = _info.target.localPosition;
                                float _h = UtilityScript.WindowToNGUI(_currentUserInput.position).y;
                                _targetPos.y = Mathf.Lerp(_targetPos.y, _h, Time.deltaTime * 4f);
                                if ((_targetPos.y <= UtilityScript.height * 0.46f) && (UtilityScript.height * -0.46f <= _h))
                                {   _info.target.localPosition = _targetPos;  }
                            }
                            break;
                            */
}