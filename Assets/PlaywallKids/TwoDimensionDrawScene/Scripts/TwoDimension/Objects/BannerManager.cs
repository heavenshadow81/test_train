using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// 전광판 생성및 위치 설정
    /// </summary>
    public class BannerManager : MonoBehaviour
    {
        public GameObject[] prefabs;
        [Range(0f, 1f)]
        public float x;
        [Range(0f, 1f)]
        public float y;

        public float z;

        public Vector3 scale;
        public Vector3 eulerRotation;

        public bool horizontal;
        public bool vertical;

        void Awake()
        {
            Vector3 _originPos = Vector3.zero;
            _originPos.x = 0;
            _originPos.y = y * UtilityScript.height - UtilityScript.height * 0.5f;
            _originPos.z = z;
            if (horizontal)
            {
                float width = scale.x * 10f;
                float num = 8;
                int numPrefab = prefabs.Length;

                int cnt = 0;
                int index = 0;
                for (int i = 0; i < num; ++i)
                {
                    if (i == 0) index = 0;
                    else
                    {
                        if (i % 2 == 1)
                        {
                            ++cnt;
                            cnt %= 2;
                            index = cnt;
                        }
                    }
                    GameObject go = NGUITools.AddChild(gameObject, prefabs[index]) as GameObject;
                    Transform _cachedTransform = go.transform;
                    _cachedTransform.localPosition = _originPos + new Vector3(width * ((i + 1) / 2) * (i % 2 == 0 ? -1 : 1), 0, 0);
                    _cachedTransform.localRotation = Quaternion.Euler(eulerRotation);
                    _cachedTransform.localScale = scale;
                }
            }

        }

    }
}