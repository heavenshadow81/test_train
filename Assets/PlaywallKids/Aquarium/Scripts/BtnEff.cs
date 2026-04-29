using UnityEngine;
using UnityEngine.UI;

namespace ML.PlaywallKids.Aquarium
{
    public class BtnEff : MonoBehaviour
    {

        public Sprite home;
        public Sprite draw;
        public Sprite play;
        public Sprite intro;

        private Rect hideBox = new Rect();
        private Rect homeBox = new Rect();
        private Rect drawBox = new Rect();
        private Rect playBox = new Rect();
        private Rect introBox = new Rect();

        public int buttonSize = 120;
        public int margin = 5;

        // Use this for initialization
        void Start()
        {
            hideBox.Set(0, 0, 0, 0);

            float tmp = (Screen.height - 10) - buttonSize;
            homeBox.Set(Screen.width - ((buttonSize + margin) * 4), tmp, buttonSize, buttonSize);
            drawBox.Set(Screen.width - ((buttonSize + margin) * 3), tmp, buttonSize, buttonSize);
            playBox.Set(Screen.width - ((buttonSize + margin) * 2), tmp, buttonSize, buttonSize);
            introBox.Set(Screen.width - (buttonSize + margin), tmp, buttonSize, buttonSize);

            GetComponent<Image>().sprite = home;
            HideBox();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ButtonDown(int idx)
        {
            switch (idx)
            {
                case 0:
                    GetComponent<Image>().sprite = home;
                    GetComponent<Image>().rectTransform.position = new Vector3(homeBox.x, homeBox.y);
                    break;
                case 1:
                    GetComponent<Image>().sprite = draw;
                    GetComponent<Image>().rectTransform.position = new Vector3(drawBox.x, drawBox.y);
                    break;
                case 2:
                    GetComponent<Image>().sprite = play;
                    GetComponent<Image>().rectTransform.position = new Vector3(playBox.x, playBox.y);
                    break;
                case 3:
                    GetComponent<Image>().sprite = intro;
                    GetComponent<Image>().rectTransform.position = new Vector3(introBox.x, introBox.y);
                    break;
            }

            GetComponent<AudioSource>().Play();
            GetComponent<Image>().color = Color.white;
            iTween.ColorTo(GetComponent<Image>().gameObject, iTween.Hash("color", Color.clear, "time", 1.5f, "oncomplete", "HideBox"));
        }

        void HideBox()
        {
            GetComponent<Image>().rectTransform.position = new Vector3(hideBox.x, hideBox.y);
        }
    }
}