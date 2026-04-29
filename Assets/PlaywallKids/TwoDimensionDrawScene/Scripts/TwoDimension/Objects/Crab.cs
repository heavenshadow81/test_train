using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public enum EState
    {
        NONE = -1, IDLE = 0, APPEAR, DISAPPEAR, MOVE, AFFRIGHT, EVENT, INOUT, WAIT, NUM,
    }

    public enum EDirection
    {
        NONE, LEFT, RIGHT,
    }

    public class Crab : MonoBehaviour
    {
        #region

        Rigidbody _crabRigidbody;
        public Rigidbody rigid
        {
            get
            {
                if (_crabRigidbody == null)
                {
                    _crabRigidbody = this.gameObject.GetComponent<Rigidbody>();
                    if (_crabRigidbody == null)
                    {
                        _crabRigidbody = this.gameObject.AddComponent<Rigidbody>();
                        _crabRigidbody.useGravity = false;
                    }
                }
                return _crabRigidbody;
            }
        }

        Transform _cachedTransform;
        public Transform cachedTransform
        {
            get
            {
                if (_cachedTransform == null)
                {
                    _cachedTransform = this.transform;
                }
                return _cachedTransform;
            }
        }

        public bool bTouch
        {
            get
            { return (CheckState(EState.IDLE) || CheckState(EState.MOVE)); }

            set
            {
                if (value)
                {
                    if ((CheckState(EState.IDLE) || CheckState(EState.MOVE)) && value)
                    {
                        wayPointList.Clear();
                        SetState(EState.AFFRIGHT);
                    }
                }
            }
        }

        float _radius;
        public float radius
        {
            get
            {
                if (_radius <= 0)
                {
                    SphereCollider col = this.GetComponent<SphereCollider>();
                    if (null == col)
                    {
                        this.gameObject.AddComponent<SphereCollider>();
                        col = this.GetComponent<SphereCollider>();
                        col.radius = 0.2f;
                        col.center = new Vector2(0, 0.16f);
                    }
                    col.radius = 0.2f;
                    _radius = col.radius;
                }
                return _radius;
            }
        }


        BetterList<Vector3> _wayPointLists;
        public BetterList<Vector3> wayPointList
        {
            get
            {
                if (_wayPointLists == null)
                {
                    _wayPointLists = new BetterList<Vector3>();
                }

                if (_wayPointLists.size == 0)
                {
                    Vector3 t = UtilityScript.GetRandomTarget(cachedTransform.localPosition);
                    _wayPointLists.Add(t);
                }

                return _wayPointLists;
            }

            set
            {
                if (value != null && ((BetterList<Vector3>)value).size > 0)
                {
                    if (_wayPointLists.size <= 0)
                    { _wayPointLists = value; }
                }
            }
        }

        GameObject _obj;
        public GameObject obj
        {
            get
            {
                if (_obj == null)
                {
                    _obj = this.gameObject;
                    //_obj.name = "crab";
                    //NGUITools.AddChild(_obj);
                }
                return _obj;
            }
        }

        private SandParticle _sandparticle;
        public SandParticle sandparticle
        {
            get
            {
                if (_sandparticle == null)
                {
                    GameObject g = Resources.Load("TwoDimensionContents/Contents/Effects/Sand_Particle") as GameObject;


                    g = NGUITools.AddChild(obj, g);
                    _sandparticle = g.GetComponent<SandParticle>();

                    _sandparticle.gameObject.SetActive(false);
                    _sandparticle.gameObject.name = "SandaParticle";
                    _sandparticle.transform.localRotation = Quaternion.Euler(new Vector3(-56f, 173f, 163f));

                }

                return _sandparticle;
            }

            set
            {
                if (value != null)
                {
                    if (value.GetComponent<SandParticle>() != null)
                    {
                        _sandparticle = value;
                    }
                }
            }
        }

        Animator _crabAni;
        public Animator ani
        {
            get
            {
                if (_crabAni == null)
                {
                    _crabAni = obj.GetComponent<Animator>();
                }
                return _crabAni;
            }
        }

        public Texture2D _imgPrefab;
        public Texture2D imgPrefab
        {
            get
            {
                if (_imgPrefab == null)
                {
                    _imgPrefab = Resources.Load("TwoDimensionContents/Contents/Image/imgShadow") as Texture2D;
                    //_imgPrefab
                }
                return _imgPrefab;
            }
        }

        EState _eState;
        public EState eState
        {
            get
            {
                return _eState;
            }
            set
            {
                if (value != _eState || value == EState.NONE)
                {
                    _eState = value;
                    switch (_eState)
                    {
                        case EState.APPEAR:

                            break;

                        case EState.AFFRIGHT:
                            iWayPoint = 0;
                            wayPointList.Clear();
                            break;

                        case EState.DISAPPEAR:

                            break;
                        case EState.IDLE:
                            ani.SetBool("Appear", false);

                            fIdletime = 0;
                            iWayPoint = 0;
                            fWaitTime = Random.Range(2f, 6f);
                            direction = EDirection.NONE;
                            wayPointList.Clear();
                            break;
                        case EState.MOVE:
                            direction = EDirection.NONE;
                            fIdletime = 0f;
                            fWaitTime = 0f;
                            break;

                        case EState.EVENT:
                            StartCoroutine(DisAppearProcess());

                            break;
                        case EState.NONE:
                            ani.SetBool("Appear", true);
                            ani.SetBool("Dig", false);
                            ani.SetBool("Left", false);
                            ani.SetBool("Right", false);
                            ani.SetBool("Hide", false);
                            break;

                    }
                }
            }
        }
        /*
        [HideInInspector]
        public Renderer render;

        public float alphaValue
        {
            get
            {
                return render.material.color.a;
            }
            set
            {
                Color c = render.material.color;
                c.a = value;
                render.material.color = c;
            }
        }
        */

        private float _fMoveSpeed;
        public float fMoveSpeed
        {
            get
            {
                if (_fMoveSpeed == 0)
                {
                    _fMoveSpeed = Random.Range(200f, 300f);
                }
                return _fMoveSpeed;
            }

            set
            {
                if (value >= 0)
                { _fMoveSpeed = value; }
            }
        }

        private float _fRunSpeed;
        public float fRunSpeed
        {
            get
            {
                if (_fRunSpeed == 0)
                {
                    _fRunSpeed = Random.Range(400f, 500f);
                }
                return _fRunSpeed;
            }

            set
            {
                if (value >= 0)
                { _fMoveSpeed = value; }
            }
        }

        EDirection _direction;
        public EDirection direction
        {
            get
            {
                return _direction;
            }
            set
            {
                if (_direction != value)
                {
                    _direction = value;
                    switch (value)
                    {
                        case EDirection.LEFT:
                            ani.SetBool("Left", true);
                            break;
                        case EDirection.RIGHT:
                            ani.SetBool("Right", true);

                            break;

                        case EDirection.NONE:
                            ani.SetBool("Right", false);
                            ani.SetBool("Left", false);
                            break;

                    }
                }
            }
        }
        #endregion

        public readonly static float iDigHeight = 300f;
        public int iID;

        Vector3 vSize = Vector3.zero;
        const float height = -50f;
        float fIdletime;
        float fWaitTime;
        int iWayPoint;

        Vector3 rightSide;
        Vector3 leftSide;

        delegate EState Func();
        Func[] acts;

        public void Destroy()
        {
            wayPointList.Clear();
            _wayPointLists = null;
        }

        void Awake()
        {
            leftSide = new Vector3(-22f, 87f, -4f);
            rightSide = new Vector3(-17f, -89f, -0.6f);

            acts = new Func[(int)EState.NUM];
            acts[(int)EState.MOVE] = Move;
            acts[(int)EState.IDLE] = Idle;
            acts[(int)EState.AFFRIGHT] = Run;
            acts[(int)EState.APPEAR] = Appear;
            acts[(int)EState.DISAPPEAR] = DisAppear;
        }

        void OnEnable()
        {
            fIdletime = 0;
            iWayPoint = 0;

            SetState(EState.APPEAR);
        }

        void OnDisable()
        {
            cachedTransform.localScale = vSize;
            SetState(EState.NONE);
        }

        public void SetState(EState _state)
        {
            if (eState != _state)
            {
                eState = _state;
            }
        }

        public void Touch()
        {
            bTouch = true;
        }

        public bool CheckState(EState _state)
        { return eState == _state; }

        void Update()
        {
            for (int i = 0; i < (int)EState.NUM; ++i)
            {
                if (CheckState((EState)i))
                {
                    if (acts != null && acts[i] != null)
                        SetState(acts[i]());
                }
            }
        }

        IEnumerator AppearProcess()
        {
            Vector3 vTempSize = Vector3.zero;
            vSize = cachedTransform.localScale;

            sandparticle.PlayEmit();
            this.cachedTransform.localScale = Vector3.zero;
            yield return new WaitForEndOfFrame();
            ani.SetBool("Appear", true);
            bool bComplete = false;
            //  Debug.Log("vSize : " + vSize);
            do
            {
                vTempSize = vSize - cachedTransform.localScale;
                vTempSize *= Time.deltaTime * 10f;

                float fSize = vTempSize.sqrMagnitude;
                //  Debug.Log("vTempSize : " +  vTempSize);
                if (fSize > 1f)
                { cachedTransform.localScale += vTempSize; }
                else
                {
                    bComplete = true;
                    cachedTransform.localScale = vSize;
                }
                yield return new WaitForEndOfFrame();

            } while (!bComplete);

            sandparticle.StopEmit();
            SetState(EState.IDLE);
        }

        IEnumerator DisAppearProcess()
        {
            sandparticle.PlayEmit();
            ani.SetBool("Dig", true);
            yield return new WaitForSeconds(0.5f);

#if UNITY_EDITOR
            UnityEditor.Animations.AnimatorController ac = ani.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            UnityEditor.Animations.AnimatorStateMachine sm = ac.layers[0].stateMachine;
#endif

            float fAniTime = 0.0f;

#if UNITY_EDITOR

            for (int i = 0; i < sm.states.Length; i++)
            {
                UnityEditor.Animations.ChildAnimatorState state = sm.states[i];
                string cur = state.state.name;
                if (string.Compare(cur, "Base Layer.Crab_Out") == 0)
                {
                    AnimationClip clip = state.state.motion as AnimationClip;
                    if (clip != null)
                    {
                        fAniTime = clip.length;

                    }
                }
            }
#else
        fAniTime = 0.7f;
#endif
            yield return new WaitForSeconds(fAniTime);
            ani.SetBool("Hide", false);

            Vector3 vTempSize = Vector3.zero;
            float fSize = 0;
            bool bComplete = false;
            do
            {
                vTempSize = Vector3.zero - cachedTransform.localScale;
                vTempSize *= Time.deltaTime * 10f;
                fSize = vTempSize.sqrMagnitude * Time.deltaTime * 10f;

                if (fSize > 1f)
                {
                    cachedTransform.localScale += vTempSize;
                }
                else
                {
                    bComplete = true;
                }

                yield return new WaitForEndOfFrame();
            } while (!bComplete);

            cachedTransform.localScale = Vector3.zero;

            yield return new WaitForSeconds(0.5f);
            sandparticle.StopEmit();
            ani.SetBool("Hide", false);
            obj.SetActive(false);
        }
        /*
        void OnCollisionEnter(Collision coll)
        {
            //collision with another crab
        }*/

        EState Move()
        {
            if (wayPointList.size > iWayPoint)
            {
                Vector3 vector = wayPointList[iWayPoint] - cachedTransform.localPosition;

                float betweenDist = vector.sqrMagnitude;
                vector = vector.normalized;
                direction = vector.x > 0 ? EDirection.RIGHT : EDirection.LEFT;
                fMoveSpeed = fMoveSpeed != 0 ? fMoveSpeed : 350f;
                Vector3 vecCurrent = vector * Time.deltaTime * fMoveSpeed;
                float distCurrent = vecCurrent.sqrMagnitude;

                if (betweenDist > distCurrent)
                {
                    cachedTransform.localPosition += vecCurrent;
                }
                else
                {
                    cachedTransform.localPosition = wayPointList[iWayPoint];
                    ++iWayPoint;
                }
            }
            else
            {
                direction = EDirection.NONE;
                return EState.IDLE;
            }

            return EState.MOVE;
        }

        EState Appear()
        {
            StartCoroutine(AppearProcess());
            return EState.INOUT;
        }

        EState DisAppear()
        {
            StartCoroutine(DisAppearProcess());
            return EState.INOUT;
        }

        EState Idle()
        {
            fIdletime += Time.deltaTime;
            if (fWaitTime <= fIdletime)
            {
                SetState(EState.MOVE);
            }
            return eState;
        }

        EState Run()
        {
            Vector3 vector = wayPointList[iWayPoint] - cachedTransform.localPosition;
            direction = vector.x > 0 ? EDirection.RIGHT : EDirection.LEFT;
            float dist = vector.sqrMagnitude;
            vector = vector.normalized;
            Vector3 vectorCurrent = vector * Time.deltaTime * fRunSpeed;
            float vecDist = vectorCurrent.sqrMagnitude;

            if (dist > vecDist)
            {
                cachedTransform.localPosition += vectorCurrent;
            }
            else
            {
                direction = EDirection.NONE;

                return EState.DISAPPEAR;
            }

            return EState.AFFRIGHT;
        }
    }
}