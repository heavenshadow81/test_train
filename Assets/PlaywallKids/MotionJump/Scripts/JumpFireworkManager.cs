using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.MotionJump
{
    /// <summary>
    /// 불꽃 파티클 생성 및 관리
    /// </summary>
    public class JumpFireworkManager : MonoBehaviour
    {
        public float activeTime;
        public Camera cam;
        public ParticleSystem[] particles;
        public AudioClip[] sndExplosion;

        void OnEnable()
        {
            if (cam == null)
            { cam = this.GetComponentInParent<JumpGameController>().GetComponentInChildren<JumpCameraController>().cam; }
            for (int i = 0; i < particles.Length; ++i)
            { particles[i].gameObject.SetActive(false); }
        }

        public void Fireworks()
        {
            if (!this.gameObject.activeInHierarchy)
                this.gameObject.SetActive(true);

            StartCoroutine(FireworksProcess());
        }

        /// <summary>
        /// 카메라 영역 내 불꽃생성
        /// </summary>
        /// <returns></returns>
        IEnumerator FireworksProcess()
        {
            float currentTime = 0;
            float waitTime = 0;
            int bitExplosionCheck = 0;

            do
            {
                waitTime = Random.Range(0.1f, 0.7f);
                currentTime += Time.deltaTime;
                int randomLength = Random.Range(1, 3);

                for (int i = 0; i < randomLength; ++i)
                {
                    int index = 0;
                    do
                    {
                        index = Random.Range(0, particles.Length);
                    } while ((bitExplosionCheck & 0x01 << index) != 0);
                    bitExplosionCheck |= 0x01 << index;

                    ParticleSystem fire = particles[index];
                    float x = Random.Range(0.3f, 0.8f);
                    float y = Random.Range(0.3f, 0.8f);
                    fire.transform.position = cam.ViewportToWorldPoint(new Vector3(x, y, 41));
                    fire.gameObject.SetActive(true);
                    fire.transform.localScale = Vector3.one * 0.05f;
                    AudioSource.PlayClipAtPoint(sndExplosion[Random.Range(0, sndExplosion.Length)], Vector3.zero);
                    float fWaitTime = Random.Range(0.2f, 0.7f);
                    yield return new WaitForSeconds(fWaitTime);
                }

                bitExplosionCheck = 0;
                yield return new WaitForSeconds(waitTime);
            } while (activeTime > currentTime);
        }
    }
}