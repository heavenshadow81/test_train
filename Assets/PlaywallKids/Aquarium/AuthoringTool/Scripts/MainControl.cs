using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    //[AddComponentMenu("NGUI/Interaction/Panel Alpha")]
    public class MainControl : MonoBehaviour
    {
        /*	
            public UISprite sprite;
            public GameObject mainPanel;

            // Use this for initialization
            void Start () {
                sprite.transform.parent.localPosition = mainPanel.transform.localPosition;
                sprite.transform.parent.localScale = mainPanel.transform.localScale;

                mWidgets = GetComponentsInChildren<UIWidget>(true);

                //sprite.transform.localScale = mainPanel.transform.localScale;
        //		DrawingPanel.transform.col
            }

            // Update is called once per frame
            void Update () {

            }
            */
        public GameObject mainPanel;

        public float alpha = 0f;
        Collider[] mColliders;
        UIWidget[] mWidgets;
        float[] mAlpha;
        float mLastAlpha = 1f;
        int mLevel = 2;

        void Start()
        {
            //		Ray ray = UICamera.currentCamera.ScreenPointToRay (UICamera.lastTouchPosition);
            //		float dist = 0f;
            //		Vector3 mDragStartPosition = ray.GetPoint (dist);s

            //		mainPanel.transform.position = new Vector3(mDragStartPosition.x, mDragStartPosition.y, 0f);

            mColliders = GetComponentsInChildren<Collider>(true);
            mWidgets = GetComponentsInChildren<UIWidget>(true);

            if (mWidgets.Length == 0)
            {
                Debug.LogError("Expected to find widgets to work with", this);
                enabled = false;
                return;
            }

            // Remember the initial alpha
            mAlpha = new float[mWidgets.Length];
            for (int i = 0, imax = mWidgets.Length; i < imax; ++i)
                mAlpha[i] = mWidgets[i].alpha;

            // Set the initial fade level
            mLastAlpha = Mathf.Clamp01(alpha);
            mLevel = (mLastAlpha > 0.99f) ? 2 : (mLastAlpha < 0.01f ? 0 : 1);

            UpdateAlpha();
        }

        void Update()
        {
            alpha = Mathf.Clamp01(alpha);

            if (mLastAlpha != alpha)
            {
                mLastAlpha = alpha;
                UpdateAlpha();
            }
            if (alpha < 1f)
                alpha += 0.01f;
        }

        void UpdateAlpha()
        {
            // Update the widget alpha
            for (int i = 0, imax = mWidgets.Length; i < imax; ++i)
            {
                UIWidget w = mWidgets[i];
                if (w != null)
                    w.alpha = mAlpha[i] * alpha;
            }

            if (mLevel == 0)
            {
                // Fade in started -- enable all game objects
                Transform trans = transform;
                for (int i = 0, imax = trans.childCount; i < imax; ++i)
                    NGUITools.SetActive(trans.GetChild(i).gameObject, true);
                for (int i = 0, imax = mColliders.Length; i < imax; ++i)
                    mColliders[i].enabled = false;
                mLevel = 1;
            }
            else if (mLevel == 2 && alpha < 0.99f)
            {
                // Fade out started -- disable tweens and colliders
                TweenColor[] tweens = GetComponentsInChildren<TweenColor>();
                for (int i = 0, imax = tweens.Length; i < imax; ++i)
                    tweens[i].enabled = false;
                for (int i = 0, imax = mColliders.Length; i < imax; ++i)
                    mColliders[i].enabled = false;
                mLevel = 1;
            }

            if (mLevel == 1)
            {
                if (alpha < 0.01f)
                {
                    // Fade out finished -- disable all game objects
                    Transform trans = transform;
                    for (int i = 0, imax = trans.childCount; i < imax; ++i)
                        NGUITools.SetActive(trans.GetChild(i).gameObject, false);
                    mLevel = 0;
                }
                else if (alpha > 0.99f)
                {
                    // Fade in finished -- enable all colliders
                    for (int i = 0, imax = mColliders.Length; i < imax; ++i)
                        mColliders[i].enabled = true;
                    mLevel = 2;
                }
            }
        }
    }
}