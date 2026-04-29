using UnityEngine;

namespace ML.PlaywallKids.EAS
{
    using DragonPark;
    public class EASAToolPetFreeDrawingPanelBase : EASAnimatablePanel
    {
        #region Public variables
        public CanvasSplineDrawingSupport canvasSplineDrawingSupport;
        public Transform brushSizeContainer;
        public UISlider brushSizeSlider;
        public UISprite brushSizeSmallSprite, brushSizeBigSprite;
        public Transform fruitPos;
        public Shader templateShader;
        public AToolItemList itemList;
        public UISprite icon;
        public UIAtlas iconAtlas;
        public AudioClip poopSound;

        public System.Action<int> onSelectItem = null;
        #endregion

        #region Properties
        private static string[] _itemNames = {
        "apple",
        "mandarin",
        "melon",
        "watermelon",
        "strawberry"
    };
        public static string[] itemNames
        {
            get
            {
                return _itemNames;
            }
        }

        private Template3D _template = null;
        public Template3D template
        {
            get
            {
                return _template;
            }
            private set
            {
                _template = value;
            }
        }

        private int _selectedItemIndex = 0;
        public int selectedItemIndex
        {
            get
            {
                return _selectedItemIndex;
            }
        }

        public string selectedItemName
        {
            get
            {
                return _itemNames[_selectedItemIndex];
            }
        }
        #endregion

        #region Private variables
        private GameObject _dummyItemIcon = null;
        #endregion

        public override void BeginShow()
        {
            base.BeginShow();

            ClearModel();

            _InitItemList();

            SelectItem(0);
        }

        public override void Active()
        {
            base.Active();

            if (itemList != null)
            {
                itemList.Refresh();
            }
        }

        private void _InitItemList()
        {
            if (itemList != null && _dummyItemIcon == null)
            {
                UISprite dummy = NGUITools.AddSprite(this.gameObject, iconAtlas, "icon_apple");
                _dummyItemIcon = dummy.cachedGameObject;

                GameObject[] items = new GameObject[_itemNames.Length];

                for (int i = 0; i < _itemNames.Length; i++)
                {
                    string name = _itemNames[i];

                    UISprite newIcon = NGUITools.AddChild(this.gameObject, _dummyItemIcon).GetComponent<UISprite>();
                    newIcon.spriteName = string.Format("icon_{0}", name);

                    items[i] = newIcon.gameObject;
                    newIcon.cachedGameObject.SetActive(false);
                }

                itemList.items = items;
                itemList.rotateItem = false;

                itemList.onClick = (idx) =>
                {
                    SelectItem(idx);
                };

                Destroy(_dummyItemIcon);
            }
        }

        public void SelectItem(int idx)
        {
            _selectedItemIndex = idx;
            string name = _itemNames[idx];
            string lineName = string.Format("line_{0}", name);

            // watermelon uses same sprite as melon.
            if (name.Equals("watermelon"))
            {
                lineName = string.Format("line_melon");
            }

            // set sprite
            icon.spriteName = string.Format("icon_{0}", name);
            canvasSplineDrawingSupport.backgroundLineSprite.spriteName = lineName;

            // default background size
            canvasSplineDrawingSupport.backgroundLineSprite.width = 512;
            canvasSplineDrawingSupport.backgroundLineSprite.height = 384;

            // Watermelon needs to be bigger than melon.
            if (name.Equals("watermelon"))
            {
                canvasSplineDrawingSupport.backgroundLineSprite.width = 600;
                canvasSplineDrawingSupport.backgroundLineSprite.height = 450;
            }

            // clear previous draw
            canvasSplineDrawingSupport.Clear();

            // remove template and enable painting
            ClearModel();

            // tweens
            UITweener[] tweens = icon.GetComponents<UITweener>();
            foreach (UITweener tween in tweens)
            {
                tween.ResetToBeginning();
                tween.PlayForward();
            }

            if (onSelectItem != null)
            {
                onSelectItem(idx);
            }
        }

        public void ClearModel()
        {
            if (template != null)
            {
                Template3D templateRef = template;
                template = null;
                Destroy(templateRef.gameObject);
            }
            canvasSplineDrawingSupport.Clear();
            canvasSplineDrawingSupport.wantsPaint = true;
        }

        public void Triangulate()
        {
            if (canvasSplineDrawingSupport.spline == null) return;

            GameObject go = new GameObject("Mesh");
            go.layer = LayerMask.NameToLayer("Template3D");

            MeshFilter mf = go.AddComponent<MeshFilter>();

            // Mesh Renderer
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mr.material = new Material(templateShader);

            go.transform.parent = fruitPos;
            go.transform.localPosition = Vector3.zero;

            TriangulationContext ctx = new TriangulationContext(canvasSplineDrawingSupport.spline);
            ctx.Triangulate(true);
            MeshGenerationInfo info = MeshGeneration.Generate(ctx);
            MeshGeneration.Smooth(info, 0.5f, 1);
            mf.mesh = MeshGeneration.GetMesh(info);

            template = go.AddComponent<Template3D>();

            _AttachStalk();

            canvasSplineDrawingSupport.wantsPaint = false;
        }

        private void _AttachStalk()
        {
            string name = _itemNames[_selectedItemIndex];
            GameObject prefab = Resources.Load(string.Format("Fruits/stalk_{0}", name), typeof(GameObject)) as GameObject;
            if (prefab != null)
            {
                // Find location!
                Spline spline = canvasSplineDrawingSupport.spline;
                float left = float.MaxValue, right = float.MinValue, bottom = float.MaxValue, top = float.MinValue;
                foreach (Edge edge in spline.edges)
                {
                    left = Mathf.Min(edge.aPos.x, left);
                    right = Mathf.Max(edge.aPos.x, right);
                    bottom = Mathf.Min(edge.aPos.y, left);
                    top = Mathf.Max(edge.aPos.y, right);
                }
                float centerX = 0.5f * (right + left);
                float centerY = 0.5f * (top + bottom);
                Vector3 centerTopPos = new Vector3(float.MinValue, float.MinValue, 0.0f);
                foreach (Edge edge in spline.edges)
                {
                    if (Mathf.Abs(edge.aPos.x - centerX) < Mathf.Abs(centerTopPos.x - centerX) &&
                       edge.aPos.y >= centerY)
                    {
                        centerTopPos = edge.aPos;
                    }
                }

                // Instantiate
                GameObject stalk = (GameObject)Instantiate(prefab);
                Transform[] tfs = stalk.GetComponentsInChildren<Transform>();
                foreach (Transform t in tfs)
                {
                    t.gameObject.layer = LayerMask.NameToLayer("Template3D");
                }
                stalk.transform.parent = template.transform;
                stalk.transform.localPosition = centerTopPos;
                stalk.transform.localRotation = Quaternion.Euler(-90.0f, 0, 0);
                stalk.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}