using UnityEngine;
using UnityEngine.UI;
namespace ML.PlaywallKids.Aquarium
{
    public class IntroBtn : MonoBehaviour
    {
        public SeaIntro intro;

        private Color ori;
        private Color des;

        private bool overEnd = false;

        private Rect btnBox = new Rect();

        // Use this for initialization
        void Start()
        {
            ori = GetComponent<Image>().color;
            des = new Color(0.2f, 0.2f, 0.2f, 0.2f);

            //GetComponent<GUITexture>().pixelInset = new Rect((Screen.width - GetComponent<GUITexture>().pixelInset.width) / 2, (Screen.height * 2) / 3, GetComponent<GUITexture>().texture.width * 2, GetComponent<GUITexture>().texture.height * 2);
            //btnBox = GetComponent<GUITexture>().pixelInset;
        }

        // Update is called once per frame
        void Update()
        {
            global::TouchInfo[] touches = global::CustomInput.touches;
            for (int TouchIndex = 0; TouchIndex < touches.Length; TouchIndex++)
            {
                //check touch-end
                if (touches[TouchIndex].phase != TouchInfo.Phase.End)
                    continue;

                TouchButton(touches[TouchIndex].position);
            }

            /*
            //mouse test
            if (Input.GetMouseButtonDown(0))
            {
                //Transmit buttonUI
                TouchButton(Input.mousePosition);
            }
            */
        }

        public void TouchButton(Vector2 pos)
        {
            //Debug.Log("press button" + pos);
            if (btnBox.Contains(pos))
            {
                //Debug.Log("click!!!" + pos);
                StartGame();
            }
        }

        void OnMouseDown()
        {
            //Debug.Log("down");
            StartGame();
        }

        void ReadyToEnd()
        {
            iTween.ColorTo(GetComponent<Image>().gameObject, Color.clear, 1.5f);
        }

        void StartGame()
        {
            GetComponent<AudioSource>().Play();
            overEnd = true;

            iTween.ColorTo(GetComponent<Image>().gameObject, Color.white, 0.2f);
            Invoke("ReadyToEnd", 0.2f);

            intro.FadeOut();
        }

        void OnMouseOver()
        {
            if (overEnd == false)
            {
                iTween.ColorTo(GetComponent<Image>().gameObject, ori + des, 0.3f);
            }


        }

        void OnMouseExit()
        {
            if (overEnd == false)
            {
                iTween.ColorTo(GetComponent<Image>().gameObject, ori, 0.3f);
            }
        }
    }
}