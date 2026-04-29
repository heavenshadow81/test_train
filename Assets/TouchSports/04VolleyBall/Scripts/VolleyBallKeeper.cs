using UnityEngine;
using System.Collections.Generic;

namespace ML.T_Sports.VolleyBall
{
    using Common;

    /// <summary>
    /// 배구 공막는 오브젝트
    /// </summary>
    public class VolleyBallKeeper : MonoBehaviour
    {
        public Animator animator;
        public float speed = 2;
        public float minX = -7.5f, maxX = 7.5f;
        public float jumpHeight = 2.0f;
        public float radius = 0.4f;
        public AudioClip jumpSound;

        public static List<VolleyBallKeeper> list = new List<VolleyBallKeeper>();

        // command
        public enum Command : int
        {
            Stay,
            MoveLeft,
            MoveRight
        }
        private Command _prevCommand = Command.Stay;   // 0 : idle, 1 : left, 2 : right

        // jump
        private bool _isJumping = false;
        private float _jumpTime = 0;
        
        public void OnEnable()
        {
            list.Add(this);
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
            DoCommand();
        }

        public void OnDisable()
        {
            list.Remove(this);
            CancelInvoke("DoCommand");
        }

        public void DoCommand()
        {
            Command val = _prevCommand;
            while (_prevCommand == val)
                val = (Command)Random.Range(0, 3);
            _DoCommand(val);

            if (_prevCommand == Command.Stay)
                Invoke("DoCommand", 1.0f);
            else
                Invoke("DoCommand", Random.Range(3.0f, 4.0f));
        }

        private void _DoCommand(Command val)
        {
            if (_prevCommand != val)
            {
                _prevCommand = val;
                RandomSpeed();
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                Jump();

            if (this == list[0])
                _UpdateCommands();

            bool isPlaying = ContentsManagerBase.Current != null && ContentsManagerBase.Current.IsPlaying;

            if (_isJumping)
            {
                // 1초동안 점프
                if (_jumpTime >= 1.0f)
                    _isJumping = false;

                // 간단한 sin 공식으로...
                Vector3 pos = transform.localPosition;
                pos.y = Mathf.Sin(Mathf.Clamp01(_jumpTime) * Mathf.Deg2Rad * 180) * jumpHeight;
                transform.localPosition = pos;
                _jumpTime += Time.deltaTime;
            }
            else if (isPlaying)
            {
                switch (_prevCommand)
                {
                    case Command.MoveRight:
                        transform.localPosition += Vector3.right * speed * Time.deltaTime;
                        break;
                    case Command.MoveLeft:
                        transform.localPosition += Vector3.left * speed * Time.deltaTime;
                        break;
                }
            }

            float animSpeed = speed / 2;
            if (_prevCommand == Command.MoveLeft)
                animSpeed *= -1;
            else if (_prevCommand == Command.Stay)
                animSpeed = 0;

            animator.SetFloat("speed", isPlaying ? animSpeed : 0.0f, 0.1f, Time.deltaTime);
        }
        
        private void _UpdateCommands()
        {
            for(int i = 0; i < list.Count; i++)
            {
                var a = list[i];
                int neighborLeft = 0, neighborRight = 0;
                for (int j = 0; j < list.Count; j++)
                {
                    var b = list[j];
                    Vector3 offset = (b.transform.position - a.transform.position);

                    if (a != b && offset.sqrMagnitude <= 4 * radius * radius)
                    {
                        if (Vector3.Dot(offset, Vector3.right) >= 0)
                            neighborRight++;
                        else
                            neighborLeft++;
                    }
                }
                if (a.transform.localPosition.x >= maxX)
                    neighborRight += 1;
                if (a.transform.localPosition.x <= minX)
                    neighborLeft += 1;

                if (neighborRight + neighborLeft >= 2)
                    a._DoCommand(Command.Stay);
                else if (neighborLeft > 0)
                    a._DoCommand(Command.MoveRight);
                else if (neighborRight > 0)
                    a._DoCommand(Command.MoveLeft);
            }
        }

        public void RandomSpeed()
        {
            speed = Random.Range(1.5f, 2.25f);
        }

        public void Jump()
        {
            if (!_isJumping)
            {
                _jumpTime = 0;
                _isJumping = true;
                animator.SetTrigger("jump");

                float volume = ContentsManagerBase.Current.GetSharedPropertyValue(ContentsPropertyType.SFX);
                AudioSource.PlayClipAtPoint(jumpSound, Camera.main.transform.position, volume);
            }
        }
    }
}