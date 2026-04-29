using UnityEngine;

namespace ML.PlaywallKids.EAS
{
    using DragonPark;
    public class EASATool3DStepPanelBase : EASAnimatablePanel
    {
        #region Public variables
        public CanvasSplineDrawingSupport canvasDrawingSupport;

        public Transform previewPos;

        public GameObject carPrefab, robotPrefab;

        public UILabel stepLabel;

        public UILabel stepNameLabel;

        public UISprite[] carStepSprites;
        public UISprite[] robotStepSprites;
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
                    default:
                        return 0;
                }
            }
        }

        public string[] stepNames
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
        #endregion

        #region Private variables
        protected int _currentStep = 0;
        private string[] _carStepNames = { "Body", "Wheel" };
        private string[] _robotStepNames = { "Head", "Body", "Arm", "Leg" };
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
        }

        public void Refresh()
        {
            stepLabel.text = string.Format("Step {0}/{1}", _currentStep + 1, stepCount);
            if (stepNames != null && _currentStep < stepNames.Length)
            {
                stepNameLabel.text = stepNames[_currentStep];
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
        }

        public void MakePartsMesh(Spline spline)
        {
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
                }
            }

            FreeDrawingMeshMaker.MakeFreeDrawingParts(bone, _currentStep, spline);

            _SetupLayer();
        }

        private void _SetupLayer()
        {
            _generatedGameObject.SetLayerRecursively("Template3D");
        }
    }
}