using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// Authoring Tool <br>
    /// Step 5, Free Drawing
    /// </summary>
    public class AToolCharacterFreeDrawing : AToolCharacterStep
    {
        #region Public variables
        /// <summary>
        /// Canvas <-> Spline helper.
        /// </summary>
        public CanvasSplineDrawingSupport canvasSplineDrawingSupport;

        /// <summary>
        /// Palettes.
        /// </summary>
        public Transform palettes;

        /// <summary>
        /// Container that contains slider, sprites, etc.
        /// </summary>
        public Transform brushSizeContainer;

        /// <summary>
        /// Template3D brush size slider.
        /// </summary>
        public UISlider brushSizeSlider;

        /// <summary>
        /// Sprites of brush size related.
        /// </summary>
        public UISprite brushSizeSmallSprite, brushSizeBigSprite;

        /// <summary>
        /// Rotate button.
        /// </summary>
        public GameObject rotateButton;

        /// <summary>
        /// OK Button.
        /// </summary>
        public UIButton okButton;

        /// <summary>
        /// The shader that generated 3d model will attach.
        /// </summary>
        public Shader templateShader;

        /// <summary>
        /// Template list.
        /// </summary>
        public AToolItemList itemList;

        /// <summary>
        /// Fruit Icon(Left-Top)
        /// </summary>
        public UISprite icon;

        /// <summary>
        /// FreeDrawing atlas.
        /// </summary>
        public UIAtlas iconAtlas;

        /// <summary>
        /// The OK button sound.
        /// </summary>
        public AudioClip okButtonSound;

        /// <summary>
        /// Poop sound (fruit)
        /// </summary>
        public AudioClip poopSound;

        public UILabel containerName;
        #endregion

        #region Properties
        private SimpleModelControl _modelControl = null;
        public SimpleModelControl modelControl
        {
            get
            {
                if (_modelControl == null)
                {
                    _modelControl = GetComponent<SimpleModelControl>();
                    if (_modelControl == null)
                    {
                        _modelControl = gameObject.AddComponent<SimpleModelControl>();
                    }
                }

                return _modelControl;
            }
        }
        #endregion

        #region Private variables
        private static string[] _itemNames = {
            "apple",
            "mandarin",
            "melon",
            "watermelon",
            "strawberry"
        };

        private int _selectedItemIndex = 0;
        private GameObject _dummyItemIcon = null;
        #endregion

        public void OnEnable()
        {
            containerName.text = LocalizationManager.GetData(LocalizationKey.DRAWING3D_DRAGON_SELECTFRUIT);

            if (modelControl != null)
            {
                ClearModel();

                _InitPalettes();

                _ShowBrushColor();
            }
            _InitItemList();
            SelectItem(0);
        }

        public void ClearModel()
        {
            if (modelControl.model != null)
            {
                Template3D template = modelControl.template;
                modelControl.model = null;
                Destroy(template.gameObject);
            }
            modelControl.wantsPaint = true;
            canvasSplineDrawingSupport.Clear();
            canvasSplineDrawingSupport.wantsPaint = true;
        }

        public void OnDisable()
        {
            if (modelControl != null)
            {
                modelControl.wantsPaint = false;
            }
        }

        public void Update()
        {
            if (modelControl.model == null)
            {
                //rotateButton.isEnabled = false;
                if (canvasSplineDrawingSupport.canMakeSpline)
                {
                    OkButtonActive(true);
                }
                else
                {
                    OkButtonActive(false);
                }
            }
        }

        private void _InitPalettes()
        {

            for (int i = 0; i < palettes.childCount; i++)
            {
                Transform child = palettes.GetChild(i);
                UIEventTrigger trigger = child.GetComponent<UIEventTrigger>();
                if (trigger == null)
                {
                    trigger = child.gameObject.AddComponent<UIEventTrigger>();
                }
                if (trigger.onClick.Count == 0)
                {
                    EventDelegate.Set(trigger.onClick, delegate () { modelControl.SendMessage(child.name, child.gameObject); });
                    //trigger.onClick.Add(new EventDelegate(modelControl, child.name));
                    trigger.onClick.Add(new EventDelegate(_ShowBrushColor));
                }
            }

            if (modelControl != null)
            {
                brushSizeSlider.value = (modelControl.brushSize - 8.0f) / 24.0f;
            }

            HidePalettes(false);
        }

        private void _InitItemList()
        {
            NGUITools.SetActive(rotateButton, false);

            if (_dummyItemIcon == null)
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

            // set sprite
            icon.spriteName = string.Format("icon_{0}", name);
            canvasSplineDrawingSupport.backgroundLineSprite.spriteName = lineName;


            // default background size
            canvasSplineDrawingSupport.backgroundLineSprite.width = 400;
            canvasSplineDrawingSupport.backgroundLineSprite.height = 400;

            // clear previous draw
            canvasSplineDrawingSupport.Clear();

            // remove template and enable painting
            ClearModel();

            // hide palettes
            HidePalettes(true);

            // tweens
            UITweener[] tweens = icon.GetComponents<UITweener>();
            foreach (UITweener tween in tweens)
            {
                tween.ResetToBeginning();
                tween.PlayForward();
            }
        }

        private void _ShowBrushColor()
        {
            brushSizeSmallSprite.color = brushSizeBigSprite.color = modelControl.brushColor;
        }

        public void ChangeBrushSize()
        {
            if (modelControl != null)
            {
                modelControl.brushSize = Mathf.RoundToInt(8.0f + 24.0f * brushSizeSlider.value);
            }
        }

        public override void Rotate()
        {
            modelControl.RotateAndType(true, AutoRotate.RotateType.Right);
        }

        public override void RotateLeft()
        {
            modelControl.RotateAndType(true, AutoRotate.RotateType.Left);
        }

        public override void RotateStop()
        {
            modelControl.rotate = false;
        }

        public void Clean()
        {
            modelControl.template.CleanBrushTexture();
        }

        public override void GoToNextStep()
        {
            if (okButtonSound != null) AudioSource.PlayClipAtPoint(okButtonSound, Camera.main.transform.position);

            if (modelControl.model == null)
            {
                containerName.text = LocalizationManager.GetData(LocalizationKey.DRAWING3D_DRAGON_PAINT); ;
                Triangulate();
                NGUITools.SetActive(itemList.gameObject, false);
            }
            else
            {
                // release the reference.
                Template3D template = modelControl.template;
                modelControl.model = null;
                template.transform.parent = null;

                // set layer as MainScene
                template.gameObject.SetLayerRecursively("MainScene");

                // initialize fruit
                DragonFruit fruit = template.gameObject.AddComponent<DragonFruit>();
                fruit.userId = mainPanel.userId;
                fruit.dragon = SimpleInstantiatedTemplateControl.GetCurrentTemplate(mainPanel.userId).GetComponent<Dragon>();
                fruit.name = string.Format("Fruit({0}->{1})", _itemNames[_selectedItemIndex], fruit.dragon.name);
                fruit.poopSound = poopSound;
                fruit.showMenuAfterEat = true;

                base.GoToNextStep();
            }
        }

        public void Triangulate()
        {
            if (canvasSplineDrawingSupport.spline == null) return;

            RotateStop();

            GameObject go = new GameObject("Mesh");
            go.layer = LayerMask.NameToLayer("Template3D");

            MeshFilter mf = go.AddComponent<MeshFilter>();

            // Mesh Renderer
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mr.material = new Material(templateShader);

            go.transform.parent = canvasSplineDrawingSupport.transform;
            go.transform.localPosition = new Vector3(0, 0, -150);
            //go.transform.localScale = new Vector3(1, 1, 1);

            TriangulationContext ctx = new TriangulationContext(canvasSplineDrawingSupport.spline);
            ctx.Triangulate(true);
            MeshGenerationInfo info = MeshGeneration.Generate(ctx);
            MeshGeneration.Smooth(info, 0.5f, 1);
            mf.mesh = MeshGeneration.GetMesh(info);

            //mf.mesh = Triangulation.Triangulate3D(canvasSplineDrawingSupport.spline, 8, true);

            Template3D template = go.AddComponent<Template3D>();
            modelControl.model = go;

            _AttachStalk();

            ShowPalettes(true);

            canvasSplineDrawingSupport.wantsPaint = false;

            StartCoroutine(_ShowFreeDrawingObjectPopAnimation());
        }

        private IEnumerator _ShowFreeDrawingObjectPopAnimation()
        {
            NGUITools.SetActive(rotateButton, false);
            //rotateButton.isEnabled = false;
            OkButtonActive(false);

            Transform modelTf = modelControl.model.transform;
            Vector3 fromScale = Vector3.one * 0.05f;
            Vector3 toScale = modelTf.localScale;

            float time = 0.0f;
            while (time < 0.35f)
            {
                if (time < 0.28f)
                {
                    modelTf.localScale = Vector3.Lerp(fromScale, toScale * 1.2f, time * 3.57f);
                }
                else
                {
                    modelTf.localScale = Vector3.Lerp(toScale * 1.2f, toScale, (time - 0.28f) * 14.28f);
                }

                time += Time.deltaTime;

                yield return null;
            }

            modelTf.localScale = toScale;

            NGUITools.SetActive(rotateButton, true);
            //rotateButton.isEnabled = true;
            //OkButtonActive(true);
            okButton.isEnabled = true;

            TCCamera.sharedInstance.RequestRefreshTCRT();
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
                stalk.transform.parent = modelControl.model.transform;
                stalk.transform.localPosition = centerTopPos;
                stalk.transform.localRotation = Quaternion.Euler(-90.0f, 0, 0);
                stalk.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        private void OkButtonActive(bool active)
        {
            okButton.isEnabled = active;
            okButton.transform.localScale = Vector3.one;
            TweenScale twnScale = okButton.GetComponent<TweenScale>();
            twnScale.enabled = active;
            if (active)
                twnScale.PlayForward();
        }

        public void ShowPalettes(bool showsAniamtion)
        {
            palettes.gameObject.SetActive(true);
            brushSizeSlider.gameObject.SetActive(true);
            brushSizeSmallSprite.gameObject.SetActive(true);
            brushSizeBigSprite.gameObject.SetActive(true);

            if (showsAniamtion)
            {
                UITweener[] tweens = palettes.GetComponents<UITweener>();
                foreach (UITweener tween in tweens)
                {
                    tween.PlayForward();
                }
                tweens = brushSizeContainer.GetComponentsInChildren<UITweener>();
                foreach (UITweener tween in tweens)
                {
                    tween.PlayForward();
                }
            }
        }

        public void HidePalettes(bool showsAnimation)
        {
            if (showsAnimation)
            {
                UITweener[] tweens = palettes.GetComponents<UITweener>();
                foreach (UITweener tween in tweens)
                {
                    tween.PlayReverse();
                }
                tweens = brushSizeContainer.GetComponentsInChildren<UITweener>();
                foreach (UITweener tween in tweens)
                {
                    tween.PlayReverse();
                }
            }
            else
            {
                palettes.gameObject.SetActive(false);
                brushSizeSlider.gameObject.SetActive(false);
                brushSizeSmallSprite.gameObject.SetActive(false);
                brushSizeBigSprite.gameObject.SetActive(false);
            }
        }
    }
}