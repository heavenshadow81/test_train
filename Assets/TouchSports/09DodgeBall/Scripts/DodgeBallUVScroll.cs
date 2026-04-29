using UnityEngine;

namespace ML.T_Sports.DodgeBall
{
    public class DodgeBallUVScroll : MonoBehaviour
    {
        public Vector2 scroll = new Vector2(1, 1);
        private Material mat;

        public void Start()
        {
            if (GetComponent<Renderer>() != null)
                mat = GetComponent<Renderer>().material;
        }

        public void Update()
        {
            if (mat != null)
            {
                mat.mainTextureOffset += scroll * Time.deltaTime;
            }
        }
    }
}