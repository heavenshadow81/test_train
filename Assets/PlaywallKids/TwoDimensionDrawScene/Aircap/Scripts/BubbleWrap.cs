using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// 뽁뽁이 터뜨리기 - 뽁뽁이 오브젝트
    /// </summary>
    public class BubbleWrap : MonoBehaviour
    {
        public Mesh unpopMesh;
        public Mesh[] popMeshes;

        public Texture2D[] unpopColors;
        public Texture2D[] popColors;
        public Texture2D unpopNormal;
        public Texture2D[] popNormals;

        private int _color = 0;
        public int color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = Mathf.Clamp(value, 0, unpopColors.Length);
                Reset();
            }
        }

        private bool _pop = false;
        public bool pop
        {
            get
            {
                return _pop;
            }
            set
            {
                _pop = value;
                Reset();
            }
        }

        public float opacity
        {
            get
            {
                if (material.HasProperty("_Transparent"))
                    return material.GetFloat("_Transparent");
                return 0.0f;
            }
            set
            {
                if (material.HasProperty("_Transparent"))
                    material.SetFloat("_Transparent", value);
            }
        }

        private Material _material;
        public Material material
        {
            get
            {
                if (_material == null)
                {
                    _material = GetComponent<Renderer>().material;
                    GetComponent<Renderer>().material = _material;
                }
                return _material;
            }
        }

        private MeshFilter _meshFilter;
        public MeshFilter meshFilter
        {
            get
            {
                if (_meshFilter == null)
                    _meshFilter = GetComponent<MeshFilter>();
                return _meshFilter;
            }
        }

        void Reset()
        {
            if (_pop)
            {
                int random = Random.Range(0, popMeshes.Length);
                meshFilter.mesh = popMeshes[random];
                material.mainTexture = popColors[_color];
                if (material.HasProperty("_BumpMap"))
                {
                    material.SetTexture("_BumpMap", popNormals[random]);
                }
                transform.localRotation = Quaternion.Euler(-90, 0, 0);
            }
            else
            {
                meshFilter.mesh = unpopMesh;
                material.mainTexture = unpopColors[_color];
                if (material.HasProperty("_BumpMap"))
                {
                    material.SetTexture("_BumpMap", unpopNormal);
                }
                transform.localRotation = Quaternion.identity;
            }
        }
    }
}