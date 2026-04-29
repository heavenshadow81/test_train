using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    public class UserFishModeling : MonoBehaviour
    {
        public Texture2D heart;
        public Texture2D bubble;
        public GameObject tex;
        public Material[] materials;
        public ParticleSystem particle;

        void Start()
        {
            gameObject.SetActive(false);  // SetActive를 사용
        }

        public void Init(Texture2D[] textrues)
        {
            gameObject.SetActive(true);  // SetActive를 사용
            tex.SetActive(false);  // SetActive를 사용
            particle.Stop();
            for (int i = 0; i < 3; ++i)
                materials[i].mainTexture = textrues[i];
        }

        public void Bubble()
        {
            particle.gameObject.SetActive(true);  // SetActive를 사용
            particle.Stop();
            particle.Play();
            Invoke("HideTexture", 1);
        }

        public void Heart()
        {
            tex.GetComponent<Renderer>().material.mainTexture = heart;
            tex.SetActive(true);  // SetActive를 사용
            Invoke("HideTexture", 1);
        }

        void HideTexture()
        {
            tex.SetActive(false);  // SetActive를 사용
            particle.Stop();
        }
    }
}
