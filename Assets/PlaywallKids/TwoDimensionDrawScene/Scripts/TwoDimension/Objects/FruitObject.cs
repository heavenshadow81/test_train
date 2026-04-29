#define Rigidbody2D

using UnityEngine;
using System.Collections;
using UserType;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// 과일 객체
    /// </summary>
    public class FruitObject : MonoBehaviour
    {
        public UITexture img { get; set; }
        public GameObject obj { get; private set; }
        public BezierMove bezier { get; private set; }
        public AudioClip sndJump;
        [HideInInspector]
        public Transform cachedTransform;
        [Range(-100f, 100f)]
        public float jumpStrength;
        public int kindOfFruit;
        new Collider collider;
        float gravitScale;
#if Rigidbody2D
        public Rigidbody2D rigid { get; private set; }
#else
    public Rigidbody rigid { get; private set; }
#endif
        public TweenScale tween { get; private set; }
        Coroutine checkColliderPocess;
        /// <summary>
        /// 사용자가 과일 선택 시 크기가 작아진후 scale이 0 이면 해당 과일 객체 비활성화하는 대리자
        /// </summary>
        EventDelegate callbackFunc;

        /// <summary>
        /// TweenScale 애니메이션이 완료 후 Collider Enable =  true 할당 하는 대리자
        /// </summary>
        EventDelegate callbackActiveCollider;

        public EState CurrentState
        {
            set
            {
                if (value != _state)
                {
                    switch (value)
                    {
                        case EState.APPEAR: // 초기화
                            if (cachedTransform.localScale != Vector3.zero)
                                cachedTransform.localScale = Vector3.zero;

                            collider.enabled = false;
                            cachedTransform.localScale = Vector3.zero;
                            tween = TweenScale(Vector3.zero, Vector3.one);

                            if (tween.onFinished.Contains(callbackFunc))
                            { tween.RemoveOnFinished(callbackFunc); }

                            if (!tween.onFinished.Contains(callbackActiveCollider))
                                tween.onFinished.Add(callbackActiveCollider);
                            rigid.gravityScale = gravitScale;

                            checkColliderPocess = StartCoroutine(ColliderCheckProcess());
                            break;
                        case EState.IDLE:
                            if (rigid.gravityScale == 0f)
                                rigid.gravityScale = gravitScale;
                            break;
                        case EState.DISAPPEAR:
                            {
                                StopCoroutine(checkColliderPocess);
                                rigid.gravityScale = 0;
                                collider.enabled = false;
                                tween = TweenScale(Vector3.one, Vector3.one * 1.7f);
                                if (tween.onFinished.Contains(callbackActiveCollider)) tween.RemoveOnFinished(callbackActiveCollider);
                                if (!tween.onFinished.Contains(callbackFunc))
                                { tween.onFinished.Add(callbackFunc); }
                                tween.method = UITweener.Method.BounceOut;
                                tween.duration = 0.3f;
                            }
                            break;
                        case EState.MOVE: break;
                    }
                    _state = value;
                }
            }
        }

        EState _state;

        void Awake()
        {

            cachedTransform = this.transform;
            obj = this.gameObject;
            collider = obj.GetComponentInChildren<Collider>();

#if Rigidbody2D
            rigid = obj.GetComponent<Rigidbody2D>();
            gravitScale = rigid.gravityScale;
#else
        rigid = obj.GetComponent<Rigidbody>();
#endif
            img = obj.GetComponent<UITexture>();
            if (img == null)
                img = gameObject.AddComponent<UITexture>();
            img.depth = 10;

            callbackFunc = new EventDelegate(
                () => { obj.SetActive(false); }
                );
            callbackActiveCollider = new EventDelegate(
               () =>
               {
                   collider.enabled = true;
               }
                );
        }

        void OnEnable()
        {
            CurrentState = EState.APPEAR;
        }

        void OnDisable()
        {
            if (checkColliderPocess != null)
                StopCoroutine(checkColliderPocess);
        }

        void ActiveCollider(bool _value)
        {
            collider.enabled = _value;
        }

        /// <summary>
        /// 과일 크기가 완전히 커진 후 Collider 상태를 enable 시키는 코루틴
        /// </summary>
        /// <returns></returns>
        IEnumerator ColliderCheckProcess()
        {
            do
            {
                if (cachedTransform.localScale == Vector3.one || !collider.enabled) collider.enabled = true;
                yield return new WaitForFixedUpdate();
            } while (!collider.enabled);
        }

        TweenScale TweenScale(Vector3 _from, Vector3 _to)
        {
            TweenScale tween = UITweener.Begin<TweenScale>(this.gameObject, 0.1f);
            tween.from = _from;
            tween.to = _to;
            tween.tweenFactor = 0;
            tween.Play(true);
            return tween;
        }

        public void Jump()
        {
            Vector3 dir = Vector3.zero;
            dir.x = Random.Range(-5f, 5f);
            dir.y = jumpStrength;
            Jump(dir);
        }

        public void Jump(Transform _target)
        {
            Vector3 dir = _target.localPosition - cachedTransform.localPosition;
            dir.y = (dir.y >= 0) ? 1f : -1f;
            dir.y *=
            dir.x = Mathf.Clamp(dir.x, -3f, 3f);
            dir.z = 0f;
            Jump(dir);
        }

        public void Jump(Vector3 dir)
        {
            if (sndJump != null)
            {
                AudioSource.PlayClipAtPoint(sndJump, Vector3.zero);
            }
#if Rigidbody2D
            rigid.AddForce(dir, ForceMode2D.Force);
#else
        rigid.AddForce(dir, ForceMode.Force);
        rigid.useGravity = true;
#endif
        }

        public void DisAppear()
        {
            rigid.Sleep();
            --TwoDimensionFruitManufacturer.numOfEnableFruits;
            CurrentState = EState.DISAPPEAR;
        }

#if Rigidbody2D
        /*
        void OnCollisionEnter2D(Collision2D _other)
        {
            if (_other.collider !=null)
            {
                if (_other.collider.gameObject.layer == Constante.COLLIDER)
                { Jump(_other.transform); }
            }
        }

        void OnCollisionStay2D(Collision2D _other)
        {
            if (_other.collider != null)
            {
                if (_other.collider.gameObject.layer == Constante.COLLIDER)
                { Jump(_other.transform); }
            }
        }*/
#else
     
    void OnCollisionEnter(Collision _other)
    {
        if (_other.collider !=null)
        {
            Vector3 dir = _other.transform.localPosition - cachedTransform.localPosition;
            if(dir.y >= 0)
            {
                dir.y = -1f;
            }

            Jump(dir);
        }
        
    }
#endif
    }
}