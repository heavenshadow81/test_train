using UnityEngine;

namespace ML.T_Sports.VolleyBall
{
    /// <summary>
    /// 별 의미는 없음. 이스터에그.
    /// </summary>
    public class VolleyBallArrowRandomMove : MonoBehaviour
    {
        public Transform[] pos;
        public int posIndex;
        public Transform ball;
        public Animator animator;

        private Vector3 _direction = Vector3.zero;
        private float _speed;

        public void OnEnable()
        {
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
            RandomMove();
        }

        public void Update()
        {
            // 대상 지점으로 이동, 공은 계속 굴리기
            transform.localPosition += _direction * Time.deltaTime * _speed;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.LookRotation(_direction, Vector3.up), Time.deltaTime * 13.0f);
            ball.Rotate(Vector3.right, 180.0f * Time.deltaTime, Space.Self);

            // 목표 지점에 가까워지면 다음 지점으로 이동
            if (Vector3.Distance(pos[posIndex].position, transform.position) <= 0.5f)
            {
                RandomMove();
            }
        }

        public void RandomMove()
        {
            // 현재 지점과 중복되지 않은 랜덤 지점을 이동하도록 지정한다.
            int prevIndex = posIndex;
            while (prevIndex == posIndex)
                posIndex = Random.Range(0, pos.Length);

            _direction = pos[posIndex].position - pos[prevIndex].position;
            _direction = _direction.normalized;
            _speed = Random.Range(1.0f, 3.0f);
            animator.SetFloat("speed", _speed / 3.0f);
        }
    }
}
