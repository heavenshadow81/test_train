using UnityEngine;
using com.Loxwell.File;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class TwoDimensionHandPrintPanel : TwoDimensionPanel
    {

        public AudioClip clip;

        float fAngle;
        Color palette;


        public override void Awake()
        {
            base.Awake();
            cSketchCanvas.textureSize = new Vector2(4096, 2048);

            // 터치 시 효과음이 출력되도록 컴포넌트 추가
            UIPlaySound playSound = cSketchCanvas.GetComponent<UIPlaySound>();
            if (playSound == null)
                playSound = cSketchCanvas.gameObject.AddComponent<UIPlaySound>();
            playSound.audioClip = clip;
            playSound.trigger = UIPlaySound.Trigger.OnPress;
        }

        protected override Brush GetBrush()
        {
            Debug.Log("derived GetBrush");

            if (cStampBush == null)
            {
                //mBrush = CustomBrush.brush.RandomColorStamp(FileName.HandStamp);
                cStampBush = CustomBrush.brush.RandomColorStamp(FileName.HandStamp);
                cStampBush.shapeDynamicComponent.enable = false;
                cStampBush.colorDynamicComponent.enable = false;
            }

            cStampBush.color = new Color(Random.Range(0.1f, 0.95f), Random.Range(0.1f, 0.95f), Random.Range(0.1f, 0.95f));

            palette = new Color(Random.Range(0.1f, 0.95f), Random.Range(0.1f, 0.95f), Random.Range(0.1f, 0.95f));
            Vector3 dir = new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), 0);
            fAngle = Quaternion.Dot(Quaternion.Euler(Vector3.up), Quaternion.Euler(dir)) * 360f;

            cStampBush.color = palette;
            cStampBush.angle = fAngle;


            return cStampBush;
        }

    }
    /*
            Texture2D black = Resources.Load(szResourcePathBack + "imgBlackWindow") as Texture2D;

            GameObject blackWindow = NGUITools.AddChild(cPanel.cachedGameObject);
            blackWindow.AddComponent<UITexture>();
            blackWindow.transform.localPosition = new Vector3(0, 0, 0);
            UITexture img = blackWindow.GetComponent<UITexture>();
            blackWindow.name = "black";
            img.mainTexture = black;
            img.MakePixelPerfect();

            Texture2D original = Resources.Load("Canvas/Palettes/HandImage") as Texture2D;
            Debug.Log(original);
            GameObject obj1 = NGUITools.AddChild(cPanel.cachedGameObject);
            obj1.AddComponent<UITexture>();
            obj1.transform.localPosition = new Vector3(0, 0, 0);
            UITexture img1 = obj1.GetComponent<UITexture>();

            img1.mainTexture = original;
            img1.MakePixelPerfect();
    */
}