using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// class Canvas 객체 동적 생성 클래스
    /// </summary>
    public class TwoDimensionPanel : MonoBehaviour, IEvent
    {
        #region PUBLIC_PROPERTIES
        UIPanel _cPanel;
        public UIPanel cPanel
        {
            get
            {
                if (_cPanel == null)
                {
                    if (this.gameObject.GetComponent<UIPanel>() == null)
                    { this.gameObject.AddComponent<UIPanel>(); }

                    _cPanel = this.gameObject.GetComponent<UIPanel>();

                }
                return _cPanel;
            }
        }

        Canvas_ _sketchCanvas;
        /// <summary>
        /// Canvas 객체
        /// </summary>
        public Canvas_ cSketchCanvas
        {
            get
            {
                if (_sketchCanvas == null)
                {
                    // Find component in panel
                    _sketchCanvas = cPanel.cachedGameObject.GetComponent<Canvas_>();

                    if (_sketchCanvas == null)
                    {
                        // Find component in childrens
                        _sketchCanvas = cPanel.GetComponentInChildren<Canvas_>();

                        // If no canvas found, create new canvas.
                        if (_sketchCanvas == null) _sketchCanvas = CreateCanvas();
                    }
                }
                return _sketchCanvas;
            }
        }

        protected string szStampImage = "HandImage";
        ImagePool _cImages;
        ImagePool cImages
        {
            get
            {
                if (_cImages == null)
                {
                    _cImages = new ImagePool(szResourcePath, szStampImage);
                }
                return _cImages;
            }
        }

        #endregion  PUBLIC_PROPERTIES

        protected const string szResourcePath = "TwoDimensionContents/Contents/Image/";
        protected Brush cStampBush;

        float fPlayTime;
        int iNumCreatedcanvas;

        #region UNITY_BUILTIN_FUNCIONS
        public virtual void Awake()
        {
            iNumCreatedcanvas = 0;
            Init();
        }

        public virtual void OnDisable()
        {
            cStampBush = null;
            if (cSketchCanvas != null && cSketchCanvas.gameObject.activeInHierarchy)
            {
                cSketchCanvas.ClearCanvas();
                cSketchCanvas.brushForTouchDict.Clear();
            }
        }

        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ClearCanvas();
            }
#endif
            if (CustomInput.touchCount > 0)
            {
                TouchInfo[] touches = CustomInput.touches;
                for (int i = 0; i < CustomInput.touchCount; ++i)
                {
                    if (touches[i].phase == TouchInfo.Phase.Begin)
                    {
                        // 자식 클래스에서 재정의한 브러시 할당
                        cSketchCanvas.brush = GetBrush();
                        if (cSketchCanvas.brush == null)
                        {
                            Debug.Log("cSketchCanvas.brush == null");
                        }
                    }
                }
            }
        }
        #endregion UNITY_BUILTIN_FUNCIONS

        #region PUBLIC_FUNCTIONS
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool StateInPlay()
        {
            return true;
        }

        /// <summary>
        /// 이벤트 시작시 호출 함수
        /// </summary>
        /// <returns></returns>
        public virtual bool StateEventReady()
        {
            return true;
        }

        /// <summary>
        /// 이벤트 동작 중일 때 호출 함수
        /// </summary>
        /// <returns></returns>
        public virtual bool StateEventActivates()
        {
            return true;
        }

        /// <summary>
        /// canvas texture 데이터 초기화
        /// </summary>
        public void ClearCanvas()
        {
            cSketchCanvas.ClearCanvas();
        }
        #endregion UNITY_BUILTIN_FUNCIONS

        #region PROTECTED_FUNCTIONS
        //자식 클래스에서 해당 브러시 재정의
        protected virtual Brush GetBrush()
        { return CustomBrush.brush.Marker(); }

        /// <summary>
        /// Resource 폴더 내 이미지 로드
        /// </summary>
        /// <param name="_fileName"></param>
        /// <returns></returns>
        protected UITexture LoadUItexture(string _fileName)
        {
            if (_fileName == "") return null;

            GameObject objImg = new GameObject();
            objImg.name = _fileName;
            objImg.AddComponent<UITexture>();
            UITexture _img = objImg.GetComponent<UITexture>();
            _img.mainTexture = (Texture2D)Resources.Load(_fileName, typeof(Texture2D));
            return _img;
        }
        #endregion PROTECTED_FUNCTIONS

        #region PRIVATE_FUNCTIONS
        // canvas 해상도 설정 및 앵커 초기화
        private void Init()
        {
            cSketchCanvas.supportsMultiTouch = true;
            cSketchCanvas.textureSize = new Vector2(1024, 1024);
            cSketchCanvas.uiTexture.SetAnchor(cPanel.cachedGameObject);

            _cPanel = cPanel;
        }

        /// <summary>
        /// canvas 동적 생성 함수
        /// </summary>
        /// <returns></returns>
        private Canvas_ CreateCanvas()
        {
            Canvas_ canvasObj = NGUITools.AddChild<Canvas_>(cPanel.cachedGameObject);
            canvasObj.gameObject.name = "Canvas_" + iNumCreatedcanvas.ToString();
            return canvasObj;
        }
        #endregion PRIVATE_FUNCTIONS

        /// <summary>
        /// 텍스쳐 동적 생성 및 UIPanel의 자식 객체로 등록
        /// </summary>
        /// <param name="_panel"></param>
        /// <param name="_path"></param>
        /// <param name="_fileName"></param>
        /// <returns></returns>
        public static UITexture ImageRegistToPanel(ref UIPanel _panel, string _path, string _fileName)
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                Debug.Log(_fileName);
                //get Default File name
                return null;
            }

            GameObject objImg = NGUITools.AddChild(_panel.cachedGameObject);

            objImg.name = _fileName;
            objImg.AddComponent<UITexture>();
            UITexture _img = objImg.GetComponent<UITexture>();
            _img.mainTexture = (Texture2D)Resources.Load(_path + _fileName, typeof(Texture2D));
            _img.MakePixelPerfect();
            return _img;
        }

        /// <summary>
        /// UITexture를 UIPanel의 자식 객체로 등록
        /// </summary>
        /// <param name="_panel"></param>
        /// <param name="_img"></param>
        /// <returns></returns>
        public static UITexture ImageRegistToPanel(ref UIPanel _panel, UITexture _img)
        {
            GameObject objImg = NGUITools.AddChild(_panel.cachedGameObject, _img.cachedGameObject);

            return (UITexture)objImg.GetComponent<UITexture>();
        }
    }

    /*
     * //original code
        int iNumCreatedcanvas;
        protected const string szResourcePath ="TwoDimensionContents/Contents/Image/";
        protected const string szResourcePathBack = "TwoDimensionContents/Contents/Background/";

        protected Brush cStampBush;


    #region

        private UITexture _imgBackGround;
        protected UITexture imgBackGround{
            get {
                if(_imgBackGround == null)
                {
                    _imgBackGround = ImageRegistToPanel(ref _cPanel,szResourcePathBack, szBackgroundName); //backgroud image
                    if(_imgBackGround == null) return null;
                    _imgBackGround.SetAnchor(cPanel.cachedGameObject);
                    _imgBackGround.depth = 0;
                }
                return _imgBackGround;
            }
        }


        private UITexture _imgDecoration;
        protected UITexture imgDecoration
        {
            get{
                if(_imgDecoration== null)
                {
                    _imgDecoration = TwoDimensionPanel.ImageRegistToPanel(ref _cPanel,szResourcePath, szDecorationName); //Top, Bottom Decoration image
                    if(_imgDecoration == null) return null;
                    _imgDecoration.SetAnchor(cPanel.cachedGameObject);
                    _imgDecoration.depth = 4;
                }
                return _imgDecoration;
            }
        }


        protected string _szDecorationName;
        public string szDecorationName
        {
            get {
                if(string.IsNullOrEmpty(_szDecorationName) )
                { _szDecorationName = GetDecorationName();}

                return _szDecorationName;
            }

            protected set{
                if(string.IsNullOrEmpty( _szDecorationName ) )
                { _szDecorationName = value;}
            }
        }

        protected string _szBackgroundName;
        public string szBackgroundName
        {
            get {
                if(string.IsNullOrEmpty(_szBackgroundName) )
                { 
                    _szBackgroundName = GetBackGroundName();
                }

                return _szBackgroundName;
            }

            protected set{
                if(string.IsNullOrEmpty( _szBackgroundName ) )
                { _szBackgroundName = value;}
            }
        }

        UIPanel _cPanel;
        public UIPanel cPanel{
            get{
                if(_cPanel ==null)
                {
                    if(this.gameObject.GetComponent<UIPanel>() == null)
                    {this.gameObject.AddComponent<UIPanel>();	 }

                    _cPanel = this.gameObject.GetComponent<UIPanel>();

                }
                return _cPanel;
            }
        }

        TwoDimensionCanvas _sketchCanvas;
        protected TwoDimensionCanvas cSketchCanvas{
            get{
                if(_sketchCanvas == null )
                {
                    _sketchCanvas = cPanel.cachedGameObject.GetComponent<TwoDimensionCanvas>();
                    if(_sketchCanvas == null) 
                    {	_sketchCanvas = CreateCanvas();	}
                }
                return _sketchCanvas;
            }
        }




        protected string szStampImage = "HandImage";
        ImagePool _cImages;
        ImagePool cImages
        {
            get {
                if(_cImages == null)
                {
                    _cImages = new ImagePool(szResourcePath, szStampImage);
                }
                return _cImages;
            }
        }



    #endregion
        protected bool bDecoOnOff;
        protected bool bBackImageOnOff;

        public virtual void Awake()
        {
            iNumCreatedcanvas =0;
            Init();
        }

        private void Init()
        {
            cSketchCanvas.supportsMultiTouch = true;

            cSketchCanvas.supportsMultiTouch = true;
            cSketchCanvas.uiTexture.SetAnchor(cPanel.cachedGameObject);

            _cPanel = cPanel;
            if(bDecoOnOff)
            {_imgDecoration = imgDecoration;}

            if(bBackImageOnOff)
            {_imgBackGround = imgBackGround;}
        }

        TwoDimensionCanvas CreateCanvas()
        {
            TwoDimensionCanvas canvasObj = NGUITools.AddChild<TwoDimensionCanvas>(cPanel.cachedGameObject);
            canvasObj.gameObject.name = "Canvas_"+ iNumCreatedcanvas.ToString();
            return canvasObj;
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Return) )
            {
                cSketchCanvas.ClearCanvas();
            }

            if(CustomInput.touchCount > 0)
            {
                TouchInfo[] touches = CustomInput.touches;
                for(int i = 0 ; i< CustomInput.touchCount ; ++i)
                {

                    if(touches[i].phase == TouchInfo.Phase.Begin)
                    {	cSketchCanvas.brush = GetBrush();}
                }
            }
        }

        protected virtual Brush GetBrush()
        {return CustomBrush.brush.Marker() ;}


        protected virtual string GetDecorationName()
        { return null;}

        protected virtual string GetBackGroundName()
        {	return null; 	}


        public static UITexture ImageRegistToPanel(ref UIPanel _panel,string _path,  string _fileName)
        {
            if(string.IsNullOrEmpty( _fileName ))
            {
                Debug.Log(_fileName);
                //get Default File name
                return null;
            }

            GameObject objImg = NGUITools.AddChild(_panel.cachedGameObject);

            objImg.name = _fileName;
            objImg.AddComponent<UITexture>();
            UITexture _img = objImg.GetComponent<UITexture>();
            _img.mainTexture = (Texture2D) Resources.Load(_path + _fileName, typeof(Texture2D) );
            _img.MakePixelPerfect();
            return _img;
        }


        public static UITexture ImageRegistToPanel(ref UIPanel _panel ,UITexture _img )
        {
            GameObject objImg = NGUITools.AddChild(_panel.cachedGameObject,_img.cachedGameObject );

            return (UITexture)objImg.GetComponent<UITexture>();
        }



        protected UITexture LoadUItexture( string _fileName)
        {
            if(_fileName == "") return null;

            GameObject objImg = new GameObject();
            objImg.name = _fileName;
            objImg.AddComponent<UITexture>();
            UITexture _img = objImg.GetComponent<UITexture>();
            _img.mainTexture = (Texture2D) Resources.Load(_fileName, typeof(Texture2D) );
            return _img;
        }


        public virtual void OnDisable()
        {
            if(MovieBackground.instance!=null)
            {
                if(MovieBackground.instance.gameObject.activeInHierarchy)
                {
                    MovieBackground.instance.gameObject.SetActive(false);
                    MovieBackground.instance.movie = null;
                }
            } 
        }
     */
    ////////////////////////////////////////////////////////////////////////
    /// ////////////////////////////////////////////////////////////////////////
    /// ////////////////////////////////////////////////////////////////////////

    /*	
     * 	delegate Brush ptrFunc();
        ptrFunc _GetBrush;
        ptrFunc GetBrushFunc{
            get{
                if(_GetBrush == null)
                {
                    _GetBrush = GetBrush;
                }
                return _GetBrush;
            }
        }

     * private void Init()
        {

            cBackgroundImg = Resources.Load(resourcePath + handImage ) as Texture2D;
            GameObject back = new GameObject();
            back.AddComponent<UITexture>();
            back.GetComponent<UITexture>().mainTexture = cBackgroundImg;
            img = back.GetComponent<UITexture>();
            back.name ="2D_HAND";


            //img.transform.parent = cPanel.transform;

            // NGUITools.AddChild(cPanel.gameObject);
            img.transform.localPosition = Vector3.zero;

            img.pivot = UIWidget.Pivot.TopRight;
            //cPanel.AddWidget(img);
            img.transform.parent = cPanel.transform;
        }


        public virtual void Update ()
        {
            // draws only if wantsPaint flag is on.
            if(CustomInput.touchCount > 0)
            {

                // iterate all touches
                TouchInfo[] touches = CustomInput.touches;
                Rect rect = new Rect(0, 0, Screen.width,  Screen.height);

                for(int i = 0, touchCount = CustomInput.touchCount; i < touchCount; i++) {
                    TouchInfo t = touches[i];
                    Vector3 pos1 = new Vector3(t.axisX, t.axisY, 0);


                    img.transform.localPosition = Translate.screenToCanvasMatrix(img, UICamera.currentCamera) * new Vector4(t.axisX-(Screen.width/2), t.axisY-(Screen.height/2), 0, 1.0f);
                }
            }
        }
    */
}