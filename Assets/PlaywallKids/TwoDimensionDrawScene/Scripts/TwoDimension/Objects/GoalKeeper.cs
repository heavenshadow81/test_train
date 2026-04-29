using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class GoalKeeper : MonoBehaviour
    {
        public Animator ani;
        public Transform motor;
        public Quaternion top;
        public Quaternion center;
        public Quaternion bottom;
        public Quaternion front;
        [Range(50, 500)]
        public float runSpeed;

        bool bUp;
        float targetH
        {
            get { return bUp ? UtilityScript.height * 0.3f : UtilityScript.height * -0.3f; }
        }

        Quaternion targetQ
        {
            get
            { return bUp ? top : bottom; }
        }

        public Transform cachedTransform
        {
            get
            {
                if (_transform == null) _transform = this.transform;
                return _transform;
            }
        }

        public GameObject obj { get; private set; }

        Vector3 target;
        Transform _transform;
        EState _state;
        EState currentState
        {
            get
            { return _state; }

            set
            {
                if (value != _state)
                {
                    switch (value)
                    {
                        case EState.IDLE:
                            ani.SetBool("win", false);
                            ani.SetBool("lose", false);
                            StartCoroutine(IdleProcess());
                            break;

                        case EState.NONE:
                            break;

                        case EState.MOVE:
                            ani.SetFloat("speed", 1f);
                            break;
                        case EState.EVENT:

                            StartCoroutine(AniProcess("win"));
                            break;
                        case EState.AFFRIGHT:
                            StartCoroutine(AniProcess("lose"));
                            break;
                    }
                    _state = value;
                }
            }
        }

        void Awake()
        {
            obj = this.gameObject;
            bUp = (Random.Range(0, 2) % 2 == 0);
            if (ani == null)
            { ani = obj.GetComponentInChildren<Animator>(); }
        }

        void OnEnable()
        {
            motor.localRotation = center;
            target = cachedTransform.localPosition;
            target.y = targetH;
            currentState = EState.IDLE;
        }

        void Update()
        {
            switch (currentState)
            {
                case EState.MOVE: Move(); break;
            }
        }

        void Move()
        {
            TurnAround(targetQ, 10f);
            float distance = targetH - cachedTransform.localPosition.y;
            distance = distance > 0 ? distance : distance * -1f;
            if (distance > 1f)
            {
                Vector3 dir = target - cachedTransform.localPosition;
                cachedTransform.localPosition += dir.normalized * Time.deltaTime * runSpeed;
            }
            else
            {
                bUp = !bUp;
                target.y = targetH;
            }
        }

        bool TurnAround(Quaternion _target, float _speed)
        {
            motor.localRotation = Quaternion.Lerp(motor.localRotation, _target, Time.deltaTime * _speed);
            return motor.localRotation == _target;
        }

        public void PreventGoal()
        {
            currentState = EState.EVENT;
        }

        public void MissGoal()
        {
            currentState = EState.AFFRIGHT;
        }

        public void Stop()
        {
            ani.SetFloat("speed", 0f);
        }

        IEnumerator IdleProcess()
        {
            float _waitTime = Random.Range(0.2f, 0.5f);
            yield return new WaitForSeconds(_waitTime);
            currentState = EState.MOVE;
        }

        IEnumerator AniProcess(string _state)
        {
            Stop();
            yield return new WaitForEndOfFrame();
            bool bComplete = false;
            ani.SetBool(_state, true);
            do
            {
                bComplete = TurnAround(front, 15f);
                yield return new WaitForEndOfFrame();
            } while (!bComplete);

            yield return new WaitForSeconds(1f);
            ani.SetBool(_state, false);
            bComplete = false;
            do
            {
                bComplete = TurnAround(center, 10f);
                yield return new WaitForEndOfFrame();
            } while (!bComplete);
            currentState = EState.IDLE;
        }


        void OnCollisionEnter2D(Collision2D _other)
        {
            PreventGoal();
            _other.gameObject.SendMessage("Touch", SendMessageOptions.DontRequireReceiver);
        }
    }
}