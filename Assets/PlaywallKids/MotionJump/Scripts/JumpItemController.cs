using UnityEngine;

namespace ML.PlaywallKids.MotionJump
{
    /// <summary>
    /// 아바타와 아이템이 충돌시 파티클 이벤트 제어 클래스
    /// </summary>
    public class JumpItemController : MonoBehaviour
    {

        Transform cachedTransform;
        public ParticleSystem particle;
        public AudioClip sndItem;
        public bool autoRotate;

        void Awake()
        {
            cachedTransform = this.transform;
            if (particle != null) particle.gameObject.SetActive(false);
        }

        void FixedUpdate()
        {
            if (autoRotate)
                cachedTransform.localEulerAngles += new Vector3(0, JumpGameController.fixedDeltaTime, 0);
        }

        void OnTriggerEnter(Collider _other)
        {
            if (particle == null) return;
            GameObject particleObj = Instantiate(particle.gameObject) as GameObject;
            particleObj.transform.position = cachedTransform.position;
            particleObj.SetActive(true);
            Destroy(particleObj, 3.0f);

            if (sndItem != null)
                Common.AudioPlay2D.PlayClip(sndItem);
        }
    }
}