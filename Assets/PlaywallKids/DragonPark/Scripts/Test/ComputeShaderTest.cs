using UnityEngine;

namespace ML.PlaywallKids.DragonPark.Test
{
    public class ComputeShaderTest : MonoBehaviour
    {
        public ComputeShader computeShader;
        public RenderTexture texture;
        public Renderer r;

        public RenderTexture textureBackup;

        // Use this for initialization
        void Start()
        {
            texture = new RenderTexture(2048, 2048, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            texture.enableRandomWrite = true;
            texture.Create();

            textureBackup = new RenderTexture(2048, 2048, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            textureBackup.enableRandomWrite = true;
            textureBackup.Create();

            //texture = new Texture2D(512, 512, TextureFormat.RGBA32, false);
            if (r != null)
            {
                Material m = r.material;
                m.shader = Shader.Find("Unlit/Transparent");
                m.mainTexture = texture;
                r.material = m;
            }
        }

        // Update is called once per frame
        void Update()
        {
            Compute();
        }

        void Compute()
        {
            if (SystemInfo.supportsComputeShaders)
            {
                int kernel = computeShader.FindKernel("TemplateDrawingMain");
                if (kernel > -1)
                {
                    RenderTexture.active = textureBackup;
                    Graphics.Blit(texture, textureBackup);
                    RenderTexture.active = null;

                    computeShader.SetTexture(kernel, "BrushTextureInput", textureBackup);
                    computeShader.SetTexture(kernel, "BrushTextureOutput", texture);

                    computeShader.Dispatch(kernel, texture.width / 8, texture.height / 8, 1);
                }
                else
                {
                    Debug.Log("Couldn't find kernel.");
                }
            }
        }

        /*
        void OnGUI()
        {
            if (GUI.Button(new Rect(20, 20, 100, 20), "Compute!"))
            {
                Compute();
            }
            if (GUI.Button(new Rect(140, 20, 100, 20), "Clear"))
            {
            }

            GUI.DrawTexture(new Rect(20, 60, 256, 256), texture);
        }
         * */
    }
}