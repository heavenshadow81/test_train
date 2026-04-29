using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.MapoContents.WorldTravel
{
    [ExecuteInEditMode]
    public class SumCamera : MonoBehaviour
    {
        public RenderTexture[] renderTextures;
        private Material _mat;

        public void Start()
        {
            Shader shader = Resources.Load<Shader>("Shaders/SumCamera");
            if (shader != null)
            {
                _mat = new Material(shader);
                _mat.name = "_sum_camera_render_textures";
            }
        }

        public void OnDestroy()
        {
            if (_mat != null)
            {
                DestroyImmediate(_mat);
                _mat = null;
            }
        }

        public void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (_mat != null)
            {
                Vector4 area = Vector4.zero;
                area.z = 1.0f / renderTextures.Length;
                area.w = 1.0f;
                foreach (var rt in renderTextures)
                {
                    _mat.SetVector("_Area", area);
                    Graphics.Blit(rt, dest, _mat);
                    area.x += area.z;
                }
            }
            else
            {
                Graphics.Blit(src, dest);
            }
        }
    }
}