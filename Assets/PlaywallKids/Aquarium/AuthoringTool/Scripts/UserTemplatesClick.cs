using UnityEngine;
using System.IO;

namespace ML.PlaywallKids.Aquarium
{
    using AuthoringTool;
    public class UserTemplatesClick : MonoBehaviour
    {

        protected UISprite drawingSprite;
        //	protected UISprite backgroundSprite;
        //	protected UITexture drawingTexture;
        protected UISprite target;

        protected UISprite templateShadow;
        protected UITexture templateTexture;
        //	protected UITexture fillTexture;
        protected UITexture drawingTexture;

        protected UserData userData;
        protected int nInstanceId = 0;

        void Start()
        {
            nInstanceId = gameObject.transform.parent.parent.parent.gameObject.GetComponent<UIPanel>().GetInstanceID();

            userData = UserData.Instance();

            foreach (UISprite sprite in gameObject.transform.parent.parent.parent.gameObject.GetComponent<UIPanel>().GetComponentsInChildren<UISprite>())
            {
                if (sprite.name.Equals("UserTemplate Shadow"))
                {
                    templateShadow = sprite;
                }
            }

            foreach (UITexture texture in gameObject.transform.parent.parent.parent.gameObject.GetComponent<UIPanel>().GetComponentsInChildren<UITexture>())
            {
                if (texture.name.Equals("Template Texture"))
                {
                    templateTexture = texture;
                }
                if (texture.name.Equals("Drawing Texture"))
                {
                    drawingTexture = texture;
                }
                /*
                if(texture.name.Equals("Fill Texture")){
                    fillTexture = texture;				
                }
                */
            }
        }

        void OnClick()
        {
            string strTexName = this.name.Replace(" Button", "");

            userData.SetTemplateName(nInstanceId, UserData.TemplateType.USERTEMPLATE, strTexName);
            userData.SetState(nInstanceId, UserData.State.DRAW);

            Debug.Log("PATH : " + userData.GetUserId(nInstanceId));

            string sUserPath = string.Concat("data\\UserTemplate\\", userData.GetUserId(nInstanceId), "\\");
            string sPath = "data\\Templates\\Templates\\";

            templateShadow.SendMessage("selectUserTemplate", strTexName);

            Debug.Log("PATH : " + string.Concat(sUserPath, strTexName, ".png"));

            Texture2D img = new Texture2D(4, 4);
            img.LoadImage(File.ReadAllBytes(string.Concat(sUserPath, strTexName, ".png")));

            drawingTexture.mainTexture = img;

            Texture2D tex = new Texture2D(4, 4);

            tex.LoadImage(File.ReadAllBytes(string.Concat(sPath, strTexName, ".png")));

            templateTexture.mainTexture = tex;


            //fillTexture.SendMessage("SelectTemplate");


            //		drawingSprite.color = new Color32 (255, 255, 255, 255);

            //		drawingSprite.spriteName = this.name.Replace (" Button", "");

            //		Debug.Log("spriteName : " + drawingSprite.spriteName+"/");

            //		drawingSprite.SendMessage("setTexture", drawingSprite.spriteName);
        }
    }
}