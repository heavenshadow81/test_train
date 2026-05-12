using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    public class Fish : MonoBehaviour
    {
        public float RR = 2;
        public Animation ani;
        public AnimationClip randClip;
        public Rigidbody rigid;

        Vector3 dir, origin;
        float elapsed = 0;
        public float rotateTime = 2;
        public float changeTime = 5;
        bool lerp = false;

        void Start()
        {
            //RandomAni();
        }

        void Move()
        {
            float x = Random.Range(-RR, RR);
            float y = Random.Range(-RR, RR);
            float z = Random.Range(-RR, RR);
            dir = new Vector3(x, y, z);

            rigid.velocity = dir;
            origin = transform.forward;
            Invoke("Move", changeTime);
            lerp = true;
        }

        void Lerp()
        {
            if (!lerp)
                return;

            elapsed += Time.deltaTime;
            if (elapsed >= rotateTime)
            {
                elapsed = 0;
                lerp = false;
                return;
            }

            Vector3 forward = Vector3.Lerp(origin, dir, elapsed / rotateTime);
            transform.forward = forward;
        }

        void LateUpdate()
        {

        }

        void RandomAni()
        {

        }
    }
}
