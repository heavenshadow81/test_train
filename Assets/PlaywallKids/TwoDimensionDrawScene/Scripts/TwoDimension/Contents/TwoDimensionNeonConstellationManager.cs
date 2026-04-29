using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    using Common;
    public class TwoDimensionNeonConstellationManager : MonoBehaviour
    {
        private int _depth = 0;
        public int depth
        {
            get
            {
                return _depth++;
            }
        }

        private const int SKETCH_COUNT = 5;

        private int MIN_COUNT = 2;
        private int MAX_COUNT
        {
            get
            {
                switch (ScreenUtil.screenType)
                {
                    case ScreenType.Bigboard2x2: return 2;
                    case ScreenType.Bigboard2x3: return 4;
                    case ScreenType.Bigboard2x4: return 5;
                    case ScreenType.Bigboard2x6: return 6;
                }
                return 2;
            }
        }

        private const float MAKE_TIME = 5f;

        public GameObject goConstellation;
        public GameObject goConstellationCanvas;

        public BoxCollider2D top;
        public BoxCollider2D bottom;
        public BoxCollider2D left;
        public BoxCollider2D right;


        private List<TwoDimensionNeonConstellation> listConstellation = new List<TwoDimensionNeonConstellation>();
        private List<Canvas_> listDrawCanvas = new List<Canvas_>();


        private float fMakeInterval;

        void Start()
        {
            Vector2 size = new Vector2(ScreenUtil.aspectRatio, 1.0f) * ScreenUtil.defaultNGUIScreenHeight;
            float scale = (1f / UIRoot.list[0].transform.localScale.x) * 1f;

            top.offset = new Vector2(0, (size.y + scale) * 0.5f);
            top.size = new Vector2(size.x, scale);

            bottom.offset = new Vector2(0, (size.y + scale) * -0.5f);
            bottom.size = new Vector2(size.x, scale);

            right.offset = new Vector2((size.x + scale) * 0.5f, 0);
            right.size = new Vector2(scale, size.y);

            left.offset = new Vector2((size.x + scale) * -0.5f, 0);
            left.size = new Vector2(scale, size.y);
        }

        void OnEnable()
        {
            Init();
        }

        void Init()
        {
            _depth = 0;
            fMakeInterval = 0f;

            for (int i = 0; i < listDrawCanvas.Count; i++)
                Destroy(listDrawCanvas[i].gameObject);
            listDrawCanvas.Clear();

            for (int i = 0; i < listConstellation.Count; i++)
                Destroy(listConstellation[i].gameObject);
            listConstellation.Clear();

            for (int i = 0; i < MIN_COUNT; i++)
                MakeConstellation();
        }

        void Update()
        {
            fMakeInterval += Time.deltaTime;
            if (fMakeInterval > MAKE_TIME + Random.value * 3f)
            {
                if (listConstellation.Count < MAX_COUNT)
                    MakeConstellation();
                fMakeInterval = 0;
            }
        }

        public void UpdateDepth(TwoDimensionNeonConstellation constellation)
        {
            for (int i = 0; i < listConstellation.Count; i++)
                if (listConstellation[i] != constellation)
                    if (listConstellation[i].nDepth > constellation.nDepth)
                        listConstellation[i].SetDepth(listConstellation[i].nDepth - 1);

            constellation.SetDepth(listConstellation.Count - 1);
        }

        public void DestoryConstellation(TwoDimensionNeonConstellation constellation)
        {
            listConstellation.Remove(constellation);
        }

        public void AddDrawCanvas(Canvas_ canvas)
        {
            listDrawCanvas.Add(canvas);
        }

        public void Clear()
        {
            Canvas_[] child = goConstellationCanvas.transform.GetComponentsInChildren<Canvas_>();

            for (int i = 0; i < child.Length; i++)
                Destroy(child[i].gameObject);
        }

        private void MakeConstellation()
        {
            TwoDimensionNeonConstellation constellation = ((GameObject)Instantiate(goConstellation)).GetComponent<TwoDimensionNeonConstellation>();
            constellation.ChangeParent(constellation.gameObject, transform);
            constellation.InitSketch(this, Random.Range(0, SKETCH_COUNT), listConstellation.Count - 1);

            listConstellation.Add(constellation);

            UpdateDepth(constellation);

            TweenScale twnScale = constellation.gameObject.GetComponent<TweenScale>();
            twnScale.onFinished.Add(new EventDelegate(() =>
            {
                Destroy(twnScale);
            }));
        }
    }
}