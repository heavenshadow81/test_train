using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// Dragon Park Free-Drawing Panel :
    /// Step 2: Parts
    /// </summary>
    public class ATool3DStep : AnimatablePanel
    {
        #region Public variables
        public CanvasSplineDrawingSupport canvasDrawingSupport;

        public Transform previewPos;

        public GameObject carPrefab, robotPrefab, airplanePrefab;

        public UILabel stepLabel;

        public UILabel stepNameLabel;

        public UISprite[] carStepSprites;
        public UISprite[] robotStepSprites;
        public UISprite[] airplaneStepSprites;

        public UIButton nextButton, okButton;
        #endregion

        #region Properties
        public string modelName
        {
            get;
            set;
        }

        public int stepCount
        {
            get
            {
                switch (modelName)
                {
                    case "robot":
                        return 4;
                    case "car":
                        return 2;
                    case "airplane":
                        return 4;
                    default:
                        return 0;
                }
            }
        }

        public LocalizationKey[] stepNames
        {
            get
            {
                if (modelName.Equals("car"))
                {
                    return _carStepNames;
                }
                else if (modelName.Equals("robot"))
                {
                    return _robotStepNames;
                }
                else if (modelName.Equals("airplane"))
                {
                    return _airplaneStepNames;
                }

                return null;
            }
        }

        private GameObject _generatedGameObject;
        public GameObject generatedGameObject
        {
            get
            {
                return _generatedGameObject;
            }
            set
            {
                _generatedGameObject = value;
            }
        }

        private System.WeakReference _mainPanel;
        public ATool3DPanel mainPanel
        {
            get
            {
                if (_mainPanel != null)
                {
                    return _mainPanel.Target as ATool3DPanel;
                }

                return null;
            }
            set
            {
                if (_mainPanel == null)
                {
                    _mainPanel = new System.WeakReference(value);
                }
                else
                {
                    _mainPanel.Target = value;
                }
            }
        }
        #endregion

        #region Private variables
        protected int _currentStep = 0;
        private LocalizationKey[] _carStepNames =
        {
        LocalizationKey.DRAWING3D_FREEDRAWING_STEP_BODY,
        LocalizationKey.DRAWING3D_FREEDRAWING_STEP_WHEEL,
    };
        private LocalizationKey[] _robotStepNames =
         {
        LocalizationKey.DRAWING3D_FREEDRAWING_STEP_HEAD,
        LocalizationKey.DRAWING3D_FREEDRAWING_STEP_BODY,
        LocalizationKey.DRAWING3D_FREEDRAWING_STEP_ARM,
        LocalizationKey.DRAWING3D_FREEDRAWING_STEP_LEG,
    };
        private LocalizationKey[] _airplaneStepNames =
         {
        LocalizationKey.DRAWING3D_FREEDRAWING_STEP_BODY,
        LocalizationKey.DRAWING3D_FREEDRAWING_STEP_WING,
        LocalizationKey.DRAWING3D_FREEDRAWING_STEP_TAIL,
        LocalizationKey.DRAWING3D_FREEDRAWING_STEP_PROPELLER,
    };
        #endregion

        public override void BeginShow()
        {
            base.BeginShow();

            canvasDrawingSupport.canvas.brush.color = Color.blue;

            _currentStep = 0;

            Refresh();

            GameObject prefab = null;
            switch (modelName)
            {
                case "car":
                    prefab = carPrefab;
                    break;
                case "robot":
                    prefab = robotPrefab;
                    break;
                case "airplane":
                    prefab = airplanePrefab;
                    break;
                default:
                    break;
            }

            if (_generatedGameObject != null)
            {
                DestroyImmediate(_generatedGameObject);
                _generatedGameObject = null;
            }

            if (prefab != null)
            {
                _generatedGameObject = (GameObject)Instantiate(prefab);
                _generatedGameObject.gameObject.SetActive(true);
                _generatedGameObject.transform.parent = previewPos;
                _generatedGameObject.transform.localPosition = Vector3.zero;
                _generatedGameObject.transform.localScale = Vector3.one;
                _generatedGameObject.name = prefab.name + string.Format("({0})", System.DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"));

                // disables effect
                if (prefab == robotPrefab)
                {
                    FreeDrawingRobotEffect robotEffect = _generatedGameObject.GetComponent<FreeDrawingRobotEffect>();
                    if (robotEffect != null)
                    {
                        robotEffect.Detach();
                    }
                }
            }

            if (nextButton != null)
            {
                nextButton.gameObject.SetActive(true);
            }
            if (okButton != null)
            {
                okButton.gameObject.SetActive(false);
            }
        }

        public void Refresh()
        {
            stepLabel.text = string.Format(LocalizationManager.GetData(LocalizationKey.DRAWING3D_FREEDRAWING_STEP), _currentStep + 1, stepCount);
            if (stepNames != null && _currentStep < stepNames.Length)
            {
                stepNameLabel.text = LocalizationManager.GetData(stepNames[_currentStep]);
            }
            else
            {
                stepNameLabel.text = "";
            }

            for (int i = 0; i < carStepSprites.Length; i++)
            {
                bool flag = modelName.Equals("car") && _currentStep == i;
                carStepSprites[i].cachedGameObject.SetActive(flag);
                if (flag)
                {
                    canvasDrawingSupport.backgroundLineSprite = carStepSprites[i];
                }
            }
            for (int i = 0; i < robotStepSprites.Length; i++)
            {
                bool flag = modelName.Equals("robot") && _currentStep == i;
                robotStepSprites[i].cachedGameObject.SetActive(flag);
                if (flag)
                {
                    canvasDrawingSupport.backgroundLineSprite = robotStepSprites[i];
                }
            }
            for (int i = 0; i < airplaneStepSprites.Length; i++)
            {
                bool flag = modelName.Equals("airplane") && _currentStep == i;
                airplaneStepSprites[i].cachedGameObject.SetActive(flag);
                if (flag)
                {
                    canvasDrawingSupport.backgroundLineSprite = airplaneStepSprites[i];
                }
            }
        }
        //부분 메시 만들기 함수
        public void MakePartsMesh(Spline spline)
        {
            var startTime = System.DateTime.Now;
            print($"메시 만들기 시작{startTime}");
            BoneObject bone = _generatedGameObject.GetComponent<BoneObject>();

            if (bone == null)
            {
                switch (modelName)
                {
                    case "car":
                        bone = _generatedGameObject.AddComponent<FreeDrawingCarBone>();
                        break;
                    case "robot":
                        bone = _generatedGameObject.AddComponent<FreeDrawingRobotBone>();
                        break;
                    case "airplane":
                        bone = _generatedGameObject.AddComponent<FreeDrawingAirplaneBone>();
                        break;
                }
            }

            FreeDrawingMeshMaker.MakeFreeDrawingParts(bone, _currentStep, spline);

            _SetupLayer();
            //함수 종료 시점
            var deltaTime = System.DateTime.Now - startTime;
            
            ContextSummation.Instance.AddSpan = deltaTime;
            print($"메시 만들기 종료 :{System.DateTime.Now} 걸린 시간 : {deltaTime}");
        }

        private void _SetupLayer()
        {
            _generatedGameObject.SetLayerRecursively("Template3D");
        }
        //버튼 호출 함수
        public void NextStep()
        {
            if (canvasDrawingSupport.canMakeSpline)
            {
                canvasDrawingSupport.enabled = false;

                _PerformNextStep();
            }
        }
        
        private void _PerformNextStep()
        {
            // makes real mesh
            MakePartsMesh(canvasDrawingSupport.spline);

            // appends the step
            _currentStep++;

            // refresh labels
            Refresh();

            // clear canvas and spline
            canvasDrawingSupport.Clear();

            // make canvas drawable
            canvasDrawingSupport.enabled = true;
            canvasDrawingSupport.Clear();

            // if all step have been ended, hide the panel and send to main panel.
            if (_currentStep == stepCount)
            {
                mainPanel.NextStep();
            }
            // if current step is final step, shows ok button instead of next button.
            else if (_currentStep + 1 == stepCount)
            {
                nextButton.gameObject.SetActive(false);
                okButton.gameObject.SetActive(true);
            }
        }
    }
}