using UnityEngine;
using DG.Tweening;

namespace ML.T_Sports.DodgeBall
{
    [RequireComponent(typeof(DodgeBallBoid))]
    public class DodgeBallPlayer : MonoBehaviour
    {
        public GameObject hitEffect;

        private DodgeBallBoid _boid;
        private Animator _anim;
        private Collider _collider;

        public bool alive { get { return _alive; } }
        private bool _alive = true;
        private Vector3 _prevPos;
        private Vector3 _scale;

        public void Awake()
        {
            _scale = transform.localScale;
        }

        public void OnEnable()
        {
            _boid = GetComponent<DodgeBallBoid>();
            _anim = GetComponentInChildren<Animator>();
            _collider = GetComponentInChildren<Collider>();
            Revive();
        }

        public void Update()
        {
            if (_alive && Time.deltaTime > 0)
            {
                Vector3 velocity = (transform.position - _prevPos) / Time.deltaTime;
                Vector3 viewVector = (transform.position - Camera.main.transform.position);
                float dot = Vector3.Dot(velocity.normalized, transform.forward);
                float speed = velocity.magnitude * dot;
                _anim.SetFloat("speed", speed / 4.0f, 0.7f, Time.deltaTime);
                _prevPos = transform.position;
            }
        }

        public void Die(Vector3 hit)
        {
            if (_alive)
            {
                _alive = false;
                _anim.SetTrigger("die");
                _boid.enabled = false;
                if (_collider != null)
                    _collider.enabled = false;
                if (hitEffect != null)
                    Destroy(Instantiate(hitEffect, hit, Quaternion.identity), 2.0f);
                transform.DOScale(_scale * 0.01f, 0.01f).SetDelay(2.5f).SetEase(Ease.Linear).SetTarget(transform).OnComplete(() => { gameObject.GetComponentInChildren<Renderer>().enabled = false; });
            }
        }

        public void Revive()
        {
            if (!_alive)
            {
                _alive = true;
                if (gameObject.activeSelf)
                    _anim.SetTrigger("revive");
                _boid.enabled = true;
                if (_collider != null)
                    _collider.enabled = true;
                gameObject.SetActive(true);
                gameObject.GetComponentInChildren<Renderer>().enabled = true;

                // Transform 재설정
                DOTween.Kill(transform);
                var pos = transform.localPosition;
                pos.y = 0;
                transform.localPosition = pos;
                transform.localScale = _scale;
            }
        }
    }
}