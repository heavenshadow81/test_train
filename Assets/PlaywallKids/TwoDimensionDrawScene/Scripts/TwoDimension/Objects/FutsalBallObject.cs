#define USING_PHYSICS2D
using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// 축구공 객체
    /// </summary>
    public class FutsalBallObject : MonoBehaviour
    {
        enum EState
        {
            NONE = -1, STOP = 0, GRAB = 1, MOVING = 2, RELEASE = 3
        }

        static int _id = 1000;
        public const string BALL = "Player2";
        const string CRAB = "Player1";
        const string COLLIDER = "Player3";
        const string ETC = "Player4";
        const string FIREBALL_LINE = "Player5";
        const float OVER_POWER = 7f;

        public UIParticleManager particleManager;
        public AudioClip sndBounce;
        public AudioClip sndImpactSmall;
        public AudioClip sndImpactLarge;

#if USING_PHYSICS2D
        private Rigidbody2D rigid { get; set; }

#elif USING_PHYSICS
    public Rigidbody rigid { get; private set; }
#endif
        [HideInInspector]
        public bool beStrong;
        [HideInInspector]
        public int id;
        [HideInInspector]
        public Transform cachedTransform;
        [HideInInspector]
        public int touchID;

        public bool IsMoving
        {
            get
            {
                return CheckState(EState.MOVING);
            }
        }

        public bool hadReleased
        {
            get
            {
                return CheckState(EState.RELEASE);
            }
        }

        public bool isGrabbing
        {
            get
            {
                return CheckState(EState.GRAB);
            }

        }

        byte _index;
        public byte footIndex
        {
            get
            {
                ++_index;
                _index %= 2;
                return _index;
            }
        }

        /// <summary>
        /// 공의 회전 각도
        /// </summary>
        public float Angle
        {
            get
            {
                if (prePos == Vector2.zero) return 0;
                float _a = Vector2.Angle(Direction, Vector2.up);
                if (Direction.x < 0) _a = 360 - _a;
                return _a;
            }
        }

        /// <summary>
        /// 공이 현재 가고 있는 방향
        /// </summary>
        public Vector2 Direction
        {
            get
            {
                if (prePos == Vector2.zero) return Vector2.zero;
                Vector2 curPos = cachedTransform.localPosition;
                return curPos - prePos;
            }
        }

        /// <summary>
        /// 일정 거리 이상이면 발바닥 도장 생성
        /// </summary>
        public bool bStamp
        {
            get
            {
                if (distance > 200f)
                {
                    distance = 0f;
                    return true;
                }
                return false;
            }
        }

        #region PRIVATE_VARIABLES
        private Vector2 prePos;
        private Vector2 curDir;
        private Vector2 preVelocity;
        private GameObject cachedGameobject;
        private float distance;
        private int currentState;
        private bool collide;
        #endregion PRIVATE_VARIABLES

        #region UNITY_BUILTIN_FUNCTIONS
        void Awake()
        {
            rigid = this.GetComponent<Rigidbody2D>();
            if (rigid == null) rigid = gameObject.AddComponent<Rigidbody2D>();
            cachedTransform = this.transform;
            cachedGameobject = this.gameObject;
            id = GetID();
        }

        void OnEnable()
        {
            Initialize();
        }

        void Update()
        {
            if (CheckState(EState.MOVING) && !collide)
            { preVelocity = rigid.linearVelocity; }
        }

        /// <summary>
        /// 공의 2D 평면 이동을 3D 회전으로 변환
        /// </summary>
        void FixedUpdate()
        {
            if (CheckState(EState.MOVING))
            {
                Vector2 curPos = cachedTransform.localPosition;
                if (prePos != Vector2.zero)
                {
                    Vector2 _currentToLast = prePos - curPos;
                    float _segment = _currentToLast.magnitude;

                    distance += _segment;
                    if (_segment < 0.5f && distance > 1f)
                    {
                        distance = 0f;
                        beStrong = false;
                        ReleasState(EState.GRAB);
                        ReleasState(EState.RELEASE);
                        ReleasState(EState.MOVING);
                        SetState(EState.STOP);
                    }
                    else
                    {
                        //rotate ball image
                        Vector3 _ballDown = new Vector3(0f, 0f, 1f);
                        Vector3 _axis = Vector3.Cross(_ballDown, _currentToLast);

                        float _theta = _segment / (cachedTransform.localScale.x * 0.5f);
                        float _thetaDegrees = _theta * 180 / Mathf.PI;
                        Quaternion q = Quaternion.AngleAxis(_thetaDegrees, _axis);
                        cachedTransform.localRotation = q * cachedTransform.localRotation;
                    }
                }

                prePos = cachedTransform.localPosition;
                /*
                footPrintPositions[currentIndex] = cachedTransform.localPosition;
                ++currentIndex;
                currentIndex %= len;
                if (currentIndex + 1 == len) footPrintPositions[0] = footPrintPositions[len - 1];
                 * */
            }
        }

        void OnTriggerEnter2D(Collider2D _other)
        {
            Collision(_other.gameObject);
        }

        void OnCollisionEnter2D(Collision2D _other)
        {
            Collision(_other.gameObject);
        }
        #endregion UNITY_BUILTIN_FUNCTIONS

        /// <summary>
        /// 충돌시 대상 객체에 SendMessage() 함수 호출
        /// </summary>
        /// <param name="other"></param>
        void Collision(GameObject other)
        {
            int _particleIndex = -1;
            Vector3 _pos = cachedTransform.localPosition;
            switch (other.tag)
            {
                case BALL:
                    _particleIndex = 3;
                    _pos = _pos + PositionWithBetween(_pos, other.transform.localPosition);
                    if (sndBounce)
                        AudioSource.PlayClipAtPoint(sndBounce, Vector3.zero);
                    break;
                case CRAB:
                    if (rigid.linearVelocity.x != 0 && rigid.linearVelocity.y != 0)
                    {
                        AudioClip snd = null;

                        if (beStrong)
                        {
                            collide = true;
                            _particleIndex = 0;
                            snd = sndImpactLarge;
                            other.SendMessage("Touch", 0.5f, SendMessageOptions.DontRequireReceiver);
                        }
                        else
                        {
                            other.SendMessage("Touch", 0.2f, SendMessageOptions.DontRequireReceiver);
                            _particleIndex = 2;
                            snd = sndImpactSmall;
                        }
                        _pos = _pos + PositionWithBetween(_pos, other.transform.localPosition);
                        if (snd)
                            AudioSource.PlayClipAtPoint(snd, Vector3.zero);
                    }
                    break;

                case ETC:
                    return;

                default:
                case COLLIDER:
                    if (other.tag != COLLIDER)
                        other.SendMessage("Touch", SendMessageOptions.DontRequireReceiver);
                    _particleIndex = 1;
                    if (sndBounce)
                        AudioSource.PlayClipAtPoint(sndBounce, Vector3.zero);
                    break;
            }

            if (_particleIndex >= 0)
            {
                if (!particleManager || !UIParticleManager.instance)
                {
                    Debug.LogError("particleManager : " + particleManager + " ,  UIParticleManager.instance : " + UIParticleManager.instance);
                    return;
                }

                particleManager.Emitt(UIParticleManager.instance.CachedTransform,
                                                      _pos + new Vector3(0, 0, -100f), true, false, 0, _particleIndex);
            }
        }

        Vector3 PositionWithBetween(Vector3 a, Vector3 b)
        {
            return (b - a) * 0.5f;
        }


        /*
            void LateUpdate()
            {
                if (collide)
                {
                    rigid.AddForce(preVelocity, ForceMode2D.Force);
                    collide = false;
                }
            }
          */
        public void Initialize()
        {
            _index = 0;
            distance = 0f;
            touchID = 0;
            prePos = curDir = Vector2.zero;
            SetState(EState.MOVING);
        }

        /// <summary>
        /// 방향으로 공에 힘을 가함
        /// </summary>
        /// <param name="dir"></param>
        public void AddForce(Vector2 dir)
        {
            if (dir == Vector2.zero) return;
            rigid.AddForce(dir, ForceMode2D.Impulse);
            ReleaseGrab();
            SetState(EState.MOVING);
        }

        /// <summary>
        /// 공의 물리 움직임 정지
        /// </summary>
        public void Sleep()
        {
            rigid.Sleep();
        }

        public void Grab()
        {
            ReleasState(EState.STOP);
            SetState(EState.GRAB);
        }

        public void ReleaseGrab()
        {
            ReleasState(EState.GRAB);
            SetState(EState.RELEASE);
        }

        void ReleasState(EState _state)
        {
            currentState &= ~(0x01 << (int)_state);
        }

        void SetState(EState _state)
        {
            currentState |= 0x01 << (int)_state;
        }

        bool CheckState(EState _state)
        {
            return 0 != (currentState & 0x01 << (int)_state);
        }



        public static int GetID()
        {
            return _id++;
        }

    }
}