using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    public class UI : MonoBehaviour
    {
        //public BloomAndLensFlares bloom;
        //public GlowEffect glow;
        public GameObject lightobj;
        public HTLiquidSpriteSheet ss;
        public MeshRenderer mr;
        public Material ssMaterial;
        bool bloomToggle = false;
        bool glowToggle = false;
        bool lightToggle = true;
        bool groundToggle = true;

        void OnGUI()
        {
            /*
            bool curBloom = GUI.Toggle(new Rect(10, 10, 200, 20), bloomToggle, "BloomAndLengsFlares");
            bool curglow = GUI.Toggle(new Rect(10, 40, 200, 20), glowToggle, "Glow Effect");
            lightToggle = GUI.Toggle(new Rect(10, 70, 200, 20), lightToggle, "Light");
            //groundToggle = GUI.Toggle(new Rect(10, 100, 200, 20), groundToggle, "Ground Effect");

            GUI.Label(new Rect(10, 180, 200, 20), format);

            if(lightToggle)
                lightobj.SetActiveRecursively(true);
            else
                lightobj.SetActiveRecursively(false);

            if(curBloom != bloomToggle)
            {
                bloomToggle = curBloom;
                SetBloom(bloomToggle);
            }
            */
            /*
            if(groundToggle)
                ss.enabled = true;
            else
                ss.enabled = false;
            */
            /*
            if(curglow != glowToggle)
            {
                glowToggle = curglow;
                if(glowToggle)
                    glow.enabled = true;
                else
                    glow.enabled = false;
            }
            */
        }

        void SetBloom(bool show)
        {
            /*
            MonoBehaviour mono = bloom as MonoBehaviour;
            if (show)
                mono.enabled = true;
            else
                mono.enabled = false;
            */
        }

        public float updateInterval = 0.01F;
        private float accum = 0; // FPS accumulated over the interval
        private int frames = 0; // Frames drawn over the interval
        private float timeleft; // Left time for current interval
        string format;

        void Start()
        {
            Application.targetFrameRate = 300;
            timeleft = updateInterval;
        }

        void Update()
        {
            timeleft -= Time.deltaTime;
            accum += Time.timeScale / Time.deltaTime;
            ++frames;

            // Interval ended - update GUI text and start new interval
            if (timeleft <= 0.0)
            {
                // display two fractional digits (f2 format)
                float fps = accum / frames;
                format = System.String.Format("{0:F2} FPS", fps);
                timeleft = updateInterval;
                accum = 0.0F;
                frames = 0;
            }
        }
    }
}