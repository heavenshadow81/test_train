using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    using Common;

    /// <summary>
    /// 텍스쳐 UV 애니메이션 동작 클래스
    /// </summary>
    public class ScrollingUVs : MonoBehaviour
    {
        public int materailIndex = 0;
        public string firstTexName = "_FirstTex";
        public string imageOffsetName = "_Offset";
        public Renderer render;
        public Texture2D bannerDefault, bannerMapo;

        float _offset = 0f;
        float _waitTime = 0f;
        float _time = 0f;

        void OnEnable()
        {
            _waitTime = 0f;
            _time = 0f;
            _offset = 0f;
            if (CommonSettings.dist.Equals("mapo") && bannerMapo != null)
                render.materials[0].SetTexture(firstTexName, bannerMapo);
            else if (bannerDefault != null)
                render.materials[0].SetTexture(firstTexName, bannerDefault);
            render.materials[0].SetFloat(imageOffsetName, _offset);
        }

        void LateUpdate()
        {
            _waitTime += Time.deltaTime;
            if (_waitTime > 4f)
            {
                _time += Time.deltaTime;
                if (_time <= 1f)
                {
                    _offset += Time.deltaTime * 0.5f;
                    render.materials[0].SetFloat(imageOffsetName, _offset);
                }
                else
                {
                    _offset = _offset % 1;
                    if (Mathf.Abs(_offset - 1.0f) < 0.125f)
                        _offset = 0.0f;
                    else if (Mathf.Abs(_offset - 0.5f) < 0.125f)
                        _offset = 0.5f;
                    render.materials[0].SetFloat(imageOffsetName, _offset);

                    _waitTime = 0f;
                    _time = 0f;
                }
            }
            // renderer.materials[1].SetTextureOffset(textureName, uvOffset + new Vector2(0, 1f));
        }
    }
}