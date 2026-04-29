using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System.Threading;

namespace ML.PlaywallKids.DragonPark.Test
{
    using Common;

    public class PrintTest : MonoBehaviour
    {
        public Camera printCamera;
        public Texture2D outputTexture;
        public Vector2 paperSize = new Vector2(4, 6);
        public GameObject template;
        public Text nameText;

        public GameObject[] templates;
        
        public void Update()
        {
            GameObject newTemplate = template;
            if (Input.GetKeyDown(KeyCode.Alpha1))
                newTemplate = templates[0];
            if (Input.GetKeyDown(KeyCode.Alpha2))
                newTemplate = templates[1];
            if (Input.GetKeyDown(KeyCode.Alpha3))
                newTemplate = templates[2];
            if (Input.GetKeyDown(KeyCode.Alpha4))
                newTemplate = templates[3];
            if (template != newTemplate)
            {
                template.SetActive(false);
                newTemplate.SetActive(true);
                template = newTemplate;
            }
            nameText.text = template.name;
        }

        public void LateUpdate()
        {
            Transform tf = template.transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Neck/Bip001 Head");
            Vector3 pos = printCamera.transform.position;
            pos.x = tf.position.x;
            pos.y = tf.position.y + 0.066f;
            printCamera.transform.position = pos;
        }

        public void OnGUI()
        {
            if (GUILayout.Button("Capture"))
            {
                StartCoroutine(Capture());
            }
            if (GUILayout.Button("Print"))
            {
                Print();
            }
        }

        IEnumerator Capture()
        {
            if (outputTexture != null)
            {
                DestroyImmediate(outputTexture);
                outputTexture = null;
            }

            Animator anim = template.GetComponent<Animator>();
            if(anim != null)
            {
                if (anim.HasState(0, Animator.StringToHash("Pico_Hi_Nor")))
                    anim.Play("Pico_Hi_Nor", 0, 0.66f);
                else if (anim.HasState(0, Animator.StringToHash("Arrow_Hi_Host")))
                    anim.Play("Arrow_Hi_Host", 0, 0.66f);
                else if (anim.HasState(0, Animator.StringToHash("Fun_Act_01")))
                    anim.Play("Fun_Act_01", 0, 0.66f);
                else if (anim.HasState(0, Animator.StringToHash("Cougar_Hi_Host")))
                    anim.Play("Cougar_Hi_Host", 0, 0.66f);
                yield return null;
                anim.speed = 0;
            }

            float aspectRatio = paperSize.x / paperSize.y;
            int paperWidth = 1024;
            int paperHeight = Mathf.FloorToInt(paperWidth / aspectRatio);

            RenderTexture rt = RenderTexture.GetTemporary(paperWidth, paperHeight, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            rt.antiAliasing = 4;
            printCamera.targetTexture = rt;
            printCamera.Render();

            RenderTexture prevActive = RenderTexture.active;
            RenderTexture.active = rt;
            outputTexture = new Texture2D(paperWidth, paperHeight, TextureFormat.ARGB32, false);
            outputTexture.ReadPixels(new Rect(0, 0, paperWidth, paperHeight), 0, 0);
            RenderTexture.active = prevActive;
            outputTexture.Apply();

            printCamera.targetTexture = null;

            byte[] pngData = outputTexture.EncodeToPNG();
            File.WriteAllBytes(Path.Combine(ResourceManager.GetResourcePath(), "Templates/print.png").Replace("\\", "/"), pngData);
        }

        public void Print()
        {
            new Thread(new ThreadStart(() =>
            {
                PrintImage printImage = new PrintImage(Path.Combine(ResourceManager.GetResourcePath(), "Templates/print.png").Replace("/", "\\"));
                printImage.Print();
            })).Start();
        }
    }

}