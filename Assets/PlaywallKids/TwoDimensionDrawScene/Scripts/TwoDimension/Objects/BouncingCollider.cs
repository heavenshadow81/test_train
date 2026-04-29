using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// 충돌 된 객체를 일정 방향으로 반동 시키는 클래스
    /// </summary>
    public class BouncingCollider : MonoBehaviour
    {
        public string tag;
        public float xAxis;
        public float yAxis;
        void OnCollisionEnter2D(Collision2D _other)
        {
            // if(string.Compare(tag, _other.gameObject.tag) == 0)
            {
                Vector2 velocity = _other.rigidbody.linearVelocity;
                // Debug.Log(_other.gameObject.tag + " : "  + string.Format("{0}, {1}", velocity.x, velocity.y));
                velocity.y *= -1f;
                if (velocity.y == 0f)
                {
                    velocity.y = yAxis;
                }

                if (velocity.x == 0f)
                {
                    velocity.x = xAxis;
                }
                //_other.rigidbody.Sleep();
                _other.rigidbody.AddForce(velocity, ForceMode2D.Force);
                //_other.rigidbody.velocity = velocity;
            }

        }

        void OnCollisionEnter(Collision _other)
        {

            Vector3 velocity = _other.rigidbody.linearVelocity;
            velocity.y *= -1f;
            // _other.rigidbody.Sleep();
            _other.rigidbody.AddForce(velocity, ForceMode.Force);
        }
    }
}