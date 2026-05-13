using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    using Common;

    public class TwoDimensionNeonConstellation : MonoBehaviour
    {
        public Transform canvasParent;
        public UISprite stSketch;

        public GameObject goStart;
        public GameObject goMove;

        public TweenScale sampleScale;

        public int nDepth;

        private Camera _camera;
        public new Camera camera
        {
            get
            {
                if (_camera == null)
                {
                    _camera = UICamera.currentCamera;
                    if (_camera == null)
                        _camera = GetComponentInParent<Camera>();
                }
                return _camera;
            }
        }

        private const string PREFAB_ATALS_PATH = "TwoDimensionContents/Neon/NeonSketch_Atlas";
        private const string STRING_SKETCH = "Sketch";

        private const string STAR_OFF = "star_off";
        private const string STAR_ON = "star_on";

        private const int MAX_DEPTH = 4;    // 0-SketchBook, 1-Sketch, 2-Canvas,  3-Collider,Button
        private const int SKETCH_SIZE = 300;
        private const int ON_OFF_SIZE = 50;


        private TwoDimensionNeonConstellationManager manager;

        private CircleCollider2D[] listCollider;
        private Dictionary<CircleCollider2D, bool> dictionarySketch = new Dictionary<CircleCollider2D, bool>();

        private Canvas_ canvas;
        private Rigidbody2D rb2D;

        private int nIndex;
        private bool bComplete;

        private int? touchID = null;
        private Vector2 touchInterval;
        private Vector2 lastPosition;

        void Start()
        {

        }

        void Update()
        {
            for (int i = 0; i < CustomInput.touchCount; i++)
            {
                TouchInfo touch = CustomInput.GetTouch(i);
                switch (touch.phase)
                {
                    case TouchInfo.Phase.Begin:

                        if (RaycastCollisionCheck(touch, canvas.boxCollider) || RaycastCollisionCheck(touch, GetComponent<Collider>()))
                            manager.UpdateDepth(this);

                        if (canvas.wantsPaint == false)
                        {
                            if (MoveStart(touch, canvas.boxCollider) || MoveStart(touch, GetComponent<Collider>()))
                            {
                                Vector2 touchPosition = ScreenUtil.ViewportToNGUIScreen(camera.ScreenToViewportPoint(touch.position));
                                touchInterval += new Vector2(transform.localPosition.x, transform.localPosition.y) - touchPosition;
                            }
                        }
                        break;

                    case TouchInfo.Phase.End:
                    case TouchInfo.Phase.Cancel:
                        if (touchID == touch.id)
                            MoveEnd(touch);
                        break;

                    case TouchInfo.Phase.Move:
                        if (canvas.wantsPaint && touchID == null)
                        {
                            if (canvas.IsDrawing(touch.id))
                            {
                                CircleCollider2D circle = null;
                                Collider2D[] overlap = Physics2D.OverlapPointAll(camera.ScreenToWorldPoint(touch.position));
                                for (int j = 0; j < listCollider.Length; j++)
                                {
                                    for (int k = 0; k < overlap.Length; k++)
                                    {
                                        TwoDimensionNeonConstellation neon = overlap[k].GetComponent<TwoDimensionNeonConstellation>();
                                        if (neon != null)
                                        {
                                            if (neon != this)
                                                if (neon.nDepth > nDepth)
                                                {
                                                    circle = null;
                                                    break;
                                                }
                                        }
                                        if (listCollider[j] == overlap[k])
                                        {
                                            circle = listCollider[j];
                                            break;
                                        }

                                    }
                                }

                                if (circle != null)
                                {
                                    dictionarySketch[circle] = true;
                                    circle.GetComponent<UISprite>().spriteName = STAR_ON;
                                    circle.enabled = false;
                                }
                            }
                        }
                        break;
                }


                int count = 0;
                foreach (KeyValuePair<CircleCollider2D, bool> dic in dictionarySketch)
                    if (dic.Value == true)
                        count++;

                if (count == listCollider.Length)
                    if (bComplete == false)
                        StartCoroutine(DrawComplete());

            }

            TouchInfo? t = GetTouchInfo();
            if (t != null)
            {
                lastPosition = transform.localPosition;
            }
            else
            {
                if (rb2D != null)
                    if (rb2D.velocity.x < 0.05f && rb2D.velocity.y < 0.05f)
                        ShootRand();
            }

            //Debug.Log("Current State - count : " + count + " , length : " + listCollider.Length);
        }

        private void LateUpdate()
        {
            Move();
        }

        public void InitSketch(TwoDimensionNeonConstellationManager _manager, int index, int depth)
        {
            manager = _manager;
            nIndex = index;

            bComplete = false;
            touchID = null;

            gameObject.SetActive(true);
            transform.localPosition = Vector3.zero;
            transform.localPosition = ScreenUtil.ViewportToNGUIScreen(Random.value, Random.value);
            //transform.localScale = Vector3.one * 1f;

            NGUITools.SetActive(goStart, true);
            NGUITools.SetActive(goMove, false);


            // Setting Stketch

            stSketch = NGUITools.AddChild<UISprite>(gameObject);

            stSketch.atlas = (UIAtlas)Resources.Load(PREFAB_ATALS_PATH, typeof(UIAtlas));
            stSketch.gameObject.name = stSketch.spriteName = GetStringName();
            stSketch.depth = GetComponent<UISprite>().depth + 1;
            stSketch.width = stSketch.height = SKETCH_SIZE;
            //stSketch.pivot = UIWidget.Pivot.BottomLeft;

            stSketch.MakePixelPerfect();
            // Setting Colliders
            UISpriteData stData = stSketch.GetAtlasSprite();
            List<Vector2> listPos = GetPixelPosition();
            for (int i = 0; i < listPos.Count; i++)
            {
                UISprite stCol = NGUITools.AddChild<UISprite>(stSketch.gameObject);
                stCol.transform.localPosition = listPos[i];

                stCol.atlas = stSketch.atlas;
                stCol.spriteName = STAR_OFF;

                stCol.width = stCol.height = ON_OFF_SIZE;
                stCol.depth = stSketch.depth + 1;

                stCol.gameObject.AddComponent<CircleCollider2D>().radius = ON_OFF_SIZE * 0.333f;
            }

            listCollider = stSketch.GetComponentsInChildren<CircleCollider2D>();
            dictionarySketch.Clear();
            for (int i = 0; i < listCollider.Length; i++)
                dictionarySketch.Add(listCollider[i], false);

            // Setting Canvas
            canvas = NGUITools.AddChild<Canvas_>(gameObject);

            canvas.transform.localPosition = Vector3.back;

            canvas.wantsPaint = false;
            canvas.drawMode = Canvas_.DrawMode.COLLISION;
            canvas.textureSize = new Vector2(256, 256);

            Brush brush = new BrushSet().Get(BrushSet.kBrushNameMarker);
            brush.color = new ColorTable().GetRandomColor();
            brush.diameter = ON_OFF_SIZE * 0.25f;
            canvas.brush = brush;

            canvas.uiTexture.SetAnchor(gameObject);
            canvas.uiTexture.topAnchor.relative = 0.875f;
            canvas.uiTexture.depth = stSketch.depth + 1;


            // Setting RigidBody2D
            rb2D = gameObject.GetComponent<Rigidbody2D>();
            if (rb2D == null)
            {
                rb2D = gameObject.AddComponent<Rigidbody2D>();
            }

            rb2D.mass = 1.0f;
            rb2D.gravityScale = 0;
            rb2D.drag = 0.005f;
            rb2D.angularDrag = 0.5f;
            //rb2D.fixedAngle = true;
            rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb2D.interpolation = RigidbodyInterpolation2D.Extrapolate;
            rb2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            ShootRand();

            // Setting Depth
            SetDepth(depth);
        }


        public void StartPaint()
        {
            canvas.wantsPaint = true;

            Destroy(rb2D);

            NGUITools.SetActive(goStart, false);
            NGUITools.SetActive(goMove, true);
        }

        public void PressMove()
        {
            for (int i = 0; i < CustomInput.touchCount; i++)
            {
                TouchInfo touch = CustomInput.GetTouch(i);
                if (MoveStart(touch, goMove.GetComponent<Collider>()))
                {
                    touchInterval += -(new Vector2(goMove.transform.localPosition.x, goMove.transform.localPosition.y));
                    GetComponent<BoxCollider2D>().isTrigger = true;
                }
            }
        }

        public void ReleaseMove()
        {
            MoveEnd();
        }

        public void SetDepth(int depth)
        {
            nDepth = depth;

            int realDepth = nDepth * MAX_DEPTH;

            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -realDepth);

            // 0 depth
            GetComponent<UISprite>().depth = realDepth + 0;

            // 1 depth
            stSketch.depth = realDepth + 1;

            // 2 depth
            canvas.uiTexture.depth = realDepth + 2;

            // 3 depth
            goStart.GetComponent<UISprite>().depth = realDepth + 3;
            goMove.GetComponent<UISprite>().depth = realDepth + 3;

            UISprite[] colliderBtns = stSketch.GetComponentsInChildren<UISprite>();
            foreach (UISprite st in colliderBtns)
                if (st != stSketch)
                    st.depth = realDepth + 3;
        }

        public void ChangeParent(GameObject go, Transform parent)
        {
            go.transform.parent = parent;
            UIWidget widget = go.GetComponent<UIWidget>();
            if (widget != null)
            {
                widget.ParentHasChanged();
                for (int i = 0; i < go.transform.childCount; i++)
                    ChangeParent(go.transform.GetChild(i).gameObject, go.transform);
            }
            else
                Debug.LogWarning("Missing Widget - Wrong Input GameObject");
        }

        public void Destroy()
        {
            if (canvas != null)
                Destroy(canvas.gameObject);
            Destroy(gameObject);
        }

        private bool RaycastCollisionCheck(TouchInfo touch, Collider checkCollider)
        {

            Ray ray = camera.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
                if (hit.collider.Equals(checkCollider))
                    return true;

            return false;
        }

        private bool MoveStart(TouchInfo touch, Collider checkCollider)
        {
            if (RaycastCollisionCheck(touch, checkCollider))
            {
                touchID = touch.id;
                touchInterval = Vector2.zero;
                if (rb2D != null)
                    rb2D.velocity = Vector2.zero;
                manager.UpdateDepth(this);
                return true;
            }

            return false;
        }

        private void Move()
        {
            if (touchID != null)
            {
                TouchInfo? touch = GetTouchInfo();

                if (touch != null)
                {
                    Vector2 touchPos = ScreenUtil.ViewportToNGUIScreen(camera.ScreenToViewportPoint(touch.Value.position));
                    transform.localPosition = touchPos + touchInterval;
                    manager.UpdateDepth(this);
                }
            }
        }

        private void MoveEnd(TouchInfo? touch = null)
        {
            if (touch == null)
                touch = GetTouchInfo();
            if (touch != null)
            {
                Vector2 velocity = new Vector2(transform.localPosition.x, transform.localPosition.y) - lastPosition;
                if (rb2D != null)
                    rb2D.AddForce(velocity * 2f);
                GetComponent<BoxCollider2D>().isTrigger = false;

                touchID = null;
            }
        }

        private void ShootRand()
        {
            rb2D.AddForce(new Vector2(Random.value, Random.value) * (Random.value > 0.5f ? 1f : -1f));
        }

        private TouchInfo? GetTouchInfo()
        {
            for (int i = 0; i < CustomInput.touchCount; i++)
                if (CustomInput.GetTouch(i).id == touchID)
                    return CustomInput.GetTouch(i);
            return null;
        }


        private IEnumerator DrawComplete()
        {
            Debug.Log(GetStringName() + " COMPLETE");
            bComplete = true;
            canvas.wantsPaint = false;

            ChangeParent(canvas.gameObject, transform.parent);
            for (int i = 0; i < listCollider.Length; i++)
                ChangeParent(listCollider[i].gameObject, canvas.transform);


            UITexture accentTexture = NGUITools.AddChild<UITexture>(transform.parent.gameObject);
            accentTexture.name = "Canvas_Accent";

            ChangeParent(accentTexture.gameObject, transform.parent);

            accentTexture.transform.localScale = Vector3.one;
            accentTexture.transform.localPosition = canvas.transform.localPosition;

            accentTexture.mainTexture = canvas.uiTexture.mainTexture;
            accentTexture.width = canvas.uiTexture.width;
            accentTexture.height = canvas.uiTexture.height;
            accentTexture.depth = canvas.uiTexture.depth + 1;

            float accentTime = 0.75f;
            TweenAlpha.Begin(accentTexture.gameObject, accentTime, 0);
            TweenScale.Begin(accentTexture.gameObject, accentTime, Vector3.one * 1.5f);

            yield return new WaitForSeconds(accentTime * 1.5f);

            Destroy(accentTexture.gameObject);

            float time = 2f;
            Vector3 to = ScreenUtil.ViewportToNGUIScreen(new Vector2(Random.Range(0.1f, 0.9f), Random.Range(0.75f, 0.95f)));

            manager.AddDrawCanvas(canvas);

            TweenScale.Begin(canvas.gameObject, time, Vector3.one * 0.75f);
            TweenPosition twnPosition = TweenPosition.Begin(canvas.gameObject, time, to);
            twnPosition.onFinished.Add(new EventDelegate(() =>
            {
                ChangeParent(canvas.gameObject, canvasParent);
                for (int i = 0; i < listCollider.Length; i++)
                    ChangeParent(listCollider[i].gameObject, canvas.transform);

                Vector3 local = canvas.transform.localPosition;
                canvas.transform.localPosition = new Vector3(local.x, local.y, 0f);
                canvas.boxCollider.enabled = false;

                TweenScale canvasScale = canvas.GetComponent<TweenScale>();

                canvasScale.animationCurve = sampleScale.animationCurve;
                canvasScale.duration = sampleScale.duration;
                canvasScale.style = sampleScale.style;

                canvasScale.to = canvas.transform.localScale;
                canvasScale.delay = 1f + Random.value;

                canvasScale.enabled = true;
                canvasScale.ResetToBeginning();
                canvasScale.PlayForward();

            }));

            TweenAlpha twnAlpha = TweenAlpha.Begin(gameObject, 0.5f, 0f);
            twnAlpha.onFinished.Add(new EventDelegate(() =>
            {
                manager.DestoryConstellation(this);
                Destroy(gameObject);
            }));
        }

        private List<Vector2> GetPixelPosition()
        {
            Texture2D tex = stSketch.atlas.texture as Texture2D;
            Color checkColor = new Color(1f, 0f, 0f);

            int width = stSketch.GetAtlasSprite().width;
            int height = stSketch.GetAtlasSprite().height;

            int x = stSketch.GetAtlasSprite().x;
            int y = (tex.height - stSketch.GetAtlasSprite().y) - height;

            List<Vector2> list = new List<Vector2>();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color color = tex.GetPixel(x + i, y + j);
                    if (checkColor == color)
                        //list.Add(new Vector2(i, j));
                        list.Add(new Vector2(i - (width * 0.5f), j - (height * 0.5f)));
                }
            }

            return list;
        }

        private string GetStringName()
        {
            return string.Format("{0}{1:00}", STRING_SKETCH, nIndex);
        }
    }
}