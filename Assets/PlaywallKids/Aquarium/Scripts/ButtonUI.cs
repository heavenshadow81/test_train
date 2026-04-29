using UnityEngine;
using UnityEngine.SceneManagement;

namespace ML.PlaywallKids.Aquarium
{
    public class ButtonUI : MonoBehaviour
    {
        public GUISkin skin;

        static private int state;

        private bool menuMode = false;

        private Texture2D tmpHome;
        public Texture2D homeOn;
        public Texture2D homeOff;

        private Texture2D tmpDraw;
        public Texture2D drawOn;
        public Texture2D drawOff;

        private Texture2D tmpPlay;
        public Texture2D playOn;
        public Texture2D playOff;

        private Texture2D tmpIntro;
        public Texture2D IntroOn;
        public Texture2D IntroOff;

        public float buttonSize = 50;
        public float margin = 5;

        static private Rect MenuBox = new Rect();
        static private Rect homeBox = new Rect();
        static private Rect drawBox = new Rect();
        static private Rect playBox = new Rect();
        static private Rect introBox = new Rect();

        public BtnEff eff;

        // Use this for initialization
        void Start()
        {
            SetState(0);

            float tmp = (Screen.height - 10) - buttonSize;

            MenuBox.Set(Screen.width - ((buttonSize + margin) * 4), tmp, (buttonSize + margin) * 4, buttonSize + margin);

            homeBox.Set(Screen.width - ((buttonSize + margin) * 4), tmp, buttonSize, buttonSize);
            drawBox.Set(Screen.width - ((buttonSize + margin) * 3), tmp, buttonSize, buttonSize);
            playBox.Set(Screen.width - ((buttonSize + margin) * 2), tmp, buttonSize, buttonSize);
            introBox.Set(Screen.width - (buttonSize + margin), tmp, buttonSize, buttonSize);
        }

        // Update is called once per frame
        void Update()
        {
            /*
            if (menuMode == false)
            {
                //Debug.Log(Time.time);
                //guiTexture.color = Color.Lerp(Color.white, Color.clear, Time.time - startTime);
            }
             * */
        }

        public void TouchButton(Vector2 pos)
        {
            //Debug.Log("press button" + pos);

            if (menuMode == false)
            {
                if (MenuBox.Contains(pos))
                {
                    menuMode = true;
                }
            }
            else
            {
                if (homeBox.Contains(pos))
                {
                    if (state != 0)
                    {
                        SetState(0);
                        eff.ButtonDown(0);
                    }
                }
                else if (drawBox.Contains(pos))
                {
                    if (state != 1)
                    {
                        SetState(1);
                        eff.ButtonDown(1);
                    }
                }
                else if (playBox.Contains(pos))
                {
                    if (state != 2)
                    {
                        SetState(2);
                        eff.ButtonDown(2);
                    }
                }
                else if (introBox.Contains(pos))
                {
                    if (state != 3)
                    {
                        SetState(3);
                        eff.ButtonDown(3);
                        FadeOut();
                    }
                }
            }
        }

        static public int GetState()
        {
            return state;
        }

        void SetState(int index)
        {
            state = index;
            //Debug.Log("Btn index=" + index);

            switch (index)
            {
                case 0:
                    tmpHome = homeOn;
                    tmpDraw = drawOff;
                    tmpPlay = playOff;
                    tmpIntro = IntroOff;
                    break;

                case 1:
                    tmpHome = homeOff;
                    tmpDraw = drawOn;
                    tmpPlay = playOff;
                    tmpIntro = IntroOff;
                    break;

                case 2:
                    tmpHome = homeOff;
                    tmpDraw = drawOff;
                    tmpPlay = playOn;
                    tmpIntro = IntroOff;
                    break;

                case 3:
                    tmpHome = homeOff;
                    tmpDraw = drawOff;
                    tmpPlay = playOff;
                    tmpIntro = IntroOn;
                    break;
            }

            menuMode = false;
        }

        void FadeOut()
        {
            iTween.CameraFadeAdd();
            iTween.CameraFadeTo(iTween.Hash("amount", 1.0f, "time", 3.5, "delay", 1, "onComplete", "LoadNextLevel", "onCompleteTarget", gameObject));
            iTween.AudioTo(gameObject, iTween.Hash("volume", 0, "time", 6));
        }

        void LoadNextLevel()
        {
            SceneManager.LoadScene(0);
        }

        void OnGUI()
        {
            if (menuMode == false)
                return;

            GUI.skin = skin;

            GUILayout.BeginArea(new Rect(Screen.width - ((buttonSize + GUI.skin.button.margin.right) * 4), 10, (buttonSize + GUI.skin.button.margin.right) * 4, buttonSize + GUI.skin.button.margin.bottom));

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(tmpHome, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
            {
                //SetState(0);
            }

            if (GUILayout.Button(tmpDraw, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
            {
                //SetState(1);
            }

            if (GUILayout.Button(tmpPlay, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
            {
                //SetState(2);
            }

            if (GUILayout.Button(tmpIntro, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
            {
                //SetState(3);
            }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
    }
}