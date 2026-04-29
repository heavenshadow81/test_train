using UnityEngine;
using System.Collections;
using UserType;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class NewCrab : MonoBehaviour
    {

        const string DAMAGE = "Damage";
        const string DIRECTION = "Direction";
        public const string TAG = "Player1";

        public Rigidbody2D rigid;
        public BoidAgent boid;
        public Animator ani;
        public Collider2D col;
        public UIParticleManager particleManager;
        [HideInInspector]
        public Transform cachedTransform;
        [HideInInspector]
        public GameObject obj;

        float activeTime;
        ParticleSystem ps;

        delegate void Func();
        Func[] functions;

        Coroutine appearCoroutine;
        Coroutine disappearCoroutine;

        EState _state;
        EState currentState
        {
            get { return _state; }
            set
            {
                if (value != _state || value == EState.NONE)
                {
                    _state = value;
                    activeTime = 0f;
                    switch (value)
                    {
                        case EState.NONE:
                            boid.MoveStop();
                            ani.SetBool("Dig", false);
                            ani.SetBool("Hide", false);
                            ani.SetBool("Appear", false);
                            cachedTransform.localScale = Vector3.zero;
                            currentState = EState.APPEAR;
                            break;
                        case EState.APPEAR:
                            appearCoroutine = StartCoroutine(AppearProcess());
                            break;
                        case EState.DISAPPEAR:
                            boid.MoveStop();
                            disappearCoroutine = StartCoroutine(DisapperProcess());
                            break;
                        case EState.MOVE:
                            boid.MoveStart();
                            break;
                        case EState.AFFRIGHT:
                            boid.MoveStop();
                            break;
                        case EState.IDLE:
                            ani.SetFloat(DAMAGE, 0);
                            ani.SetFloat(DIRECTION, 0);
                            boid.MoveStop();
                            break;
                    }
                }
            }
        }

        void Awake()
        {
            obj = this.gameObject;
            obj.tag = TAG;
            cachedTransform = this.transform;
            functions = new Func[5];
            functions[(int)EState.IDLE] = Idle;
            functions[(int)EState.MOVE] = Move;
            functions[(int)EState.AFFRIGHT] = Affright;
            functions[(int)EState.APPEAR] = Appear;
            functions[(int)EState.DISAPPEAR] = Disapper;
        }

        void OnEnable()
        {
            currentState = EState.NONE;
        }

        void OnDisable()
        {
            if (ps)
            {
                ps.Stop();
                ps.gameObject.SetActive(false);
            }

            if (appearCoroutine != null)
                StopCoroutine(appearCoroutine);
            if (disappearCoroutine != null)
                StopCoroutine(disappearCoroutine);
        }

        void FixedUpdate()
        {
            if (currentState != EState.EVENT)
                functions[(int)currentState]();
        }

        private void Move()
        {
            activeTime += Time.fixedDeltaTime;
            if (activeTime >= 5f)
            {
                currentState = EState.IDLE;
            }
            else
            {
                float _dir = boid.dir.x;
                if (_dir > 1) _dir = 1f;
                else if (_dir < -1f) _dir = -1f;
                ani.SetFloat(DIRECTION, _dir);
            }
        }

        private void Idle()
        {
            activeTime += Time.fixedDeltaTime;
            if (activeTime >= 3f)
            {
                currentState = EState.MOVE;
            }
        }

        private void Appear()
        {

        }

        private void Affright()
        {
            activeTime += Time.fixedDeltaTime;

            if (activeTime > 2.5f)
            {
                currentState = EState.IDLE;
            }
        }

        private void Disapper()
        {

        }

        public void Touch(float _damage)
        {
            if (currentState == EState.MOVE || currentState == EState.IDLE)
            {
                ani.SetFloat(DAMAGE, _damage);
                if (_damage < 0.5f)
                {
                    currentState = EState.AFFRIGHT;

                }
                else
                {
                    currentState = EState.DISAPPEAR;
                }
            }
        }

        IEnumerator AppearProcess()
        {
            col.enabled = false;
            Vector2 _pos = UtilityScript.RandomPostion(new Vector2(0.25f, 0.35f), new Vector2(0.75f, 0.65f));
            cachedTransform.localPosition = _pos;
            cachedTransform.localScale = Vector3.zero;
            ps = particleManager.OneShotEmitt(particleManager.CachedTransform, _pos, 7);
            yield return new WaitForSeconds(1f);
            ani.SetTrigger("Appear");
            int _size = (int)Random.Range(300f, 350f);
            cachedTransform.localScale = new Vector3(_size, _size, _size);
            yield return new WaitForSeconds(1f);
            ps.gameObject.SetActive(false);
            currentState = EState.IDLE;
            col.enabled = true;
        }

        IEnumerator DisapperProcess()
        {

            yield return new WaitForSeconds(1f);
            ps = particleManager.OneShotEmitt(particleManager.CachedTransform, cachedTransform.localPosition, 7);
            rigid.Sleep();
            col.enabled = false;
            ani.SetTrigger("Dig");
            yield return new WaitForSeconds(1f);
            ani.SetTrigger("Disappear");
            yield return new WaitForSeconds(1f);
            obj.SetActive(false);
        }
    }
}