using UnityEngine;

namespace ML.PlaywallKids.EAS
{
    public class EASPlaywallLogo : MonoBehaviour
    {
        public UISprite sprite;

        private float _spriteAspectRatio = 1.0f;

        // Use this for initialization
        void Start()
        {
            if (sprite == null)
            {
                sprite = GetComponent<UISprite>();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}