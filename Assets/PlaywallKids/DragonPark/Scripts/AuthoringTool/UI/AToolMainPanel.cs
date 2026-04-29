using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    public class AToolMainPanel : MonoBehaviour
    {
        // Public variables
        public AToolCharacterSelect characterSelect;
        public AToolCharacterSizing characterSizing;
        public AToolCharacterEquip characterEquip;
        public AToolCharacterDrawing characterDrawing;

        public AToolCharacterFreeDrawing characterFreeDrawing;

        public AToolMenuSelector menuSelector;

        public delegate void EndStep(int id);
        private EndStep endStep;

        // Enums
        public enum Step
        {
            None,
            SelectCharacter,
            Sizing,
            Equip,
            Drawing,
            FreeDrawing
        }

        // Properties
        private int _userId = 0;
        public int userId
        {
            get
            {
                return _userId;
            }
            set
            {
                _userId = (byte)Mathf.Clamp(value, 0, 10);
            }
        }

        private int _userSeq = 0;
        public int userSeq
        {
            get
            {
                return _userSeq;
            }
            set
            {
                _userSeq = value;
            }
        }

        private Template3D _currentTemplate;
        public Template3D currentTemplate
        {
            get
            {
                return _currentTemplate;
            }
        }

        private SimpleModelControl _modelControl;
        public SimpleModelControl modelControl
        {
            get
            {
                if (_modelControl == null)
                {
                    _modelControl = gameObject.GetComponent<SimpleModelControl>();
                    if (_modelControl == null)
                    {
                        _modelControl = gameObject.AddComponent<SimpleModelControl>();
                    }
                }
                return _modelControl;
            }
        }

        private Step _currentStep = Step.None;
        public Step currentStep
        {
            get
            {
                return _currentStep;
            }
            set
            {
                _PerformSetStep(value);
            }
        }

        // Private variables
        private UIPanel _panel;
        private AToolCharacterStep _currentStepView;

        public void Awake()
        {
            characterSelect.Deactive();
            characterSizing.Deactive();
            characterEquip.Deactive();
            characterDrawing.Deactive();
            characterFreeDrawing.Deactive();

            characterSelect.mainPanel = characterSizing.mainPanel = characterEquip.mainPanel =
                characterDrawing.mainPanel = characterFreeDrawing.mainPanel = menuSelector.mainPanel = this;

            endStep = null;
        }

        public void Start()
        {
            currentStep = Step.SelectCharacter;
        }

        public void SetDelegate(EndStep _endStep)
        {
            endStep = _endStep;
        }

        private void _PerformSetStep(Step newStep)
        {
            Step prevStep = _currentStep;

            if (prevStep == newStep) return;

            if (_currentStepView != null)
            {
                _currentStepView.Hide();
            }

            if (prevStep == Step.SelectCharacter && newStep != Step.FreeDrawing)
            {
                _InitForCharacter(characterSelect.selectedCharacter);
            }

            if (newStep == Step.SelectCharacter)
            {
                characterSelect.Reset();
                _currentStepView = characterSelect;
            }
            else if (newStep == Step.Sizing)
            {
                characterSizing.Reset();
                _currentStepView = characterSizing;
            }
            else if (newStep == Step.Equip)
            {
                characterEquip.Reset();
                _currentStepView = characterEquip;
            }
            else if (newStep == Step.Drawing)
            {
                characterDrawing.Reset();
                _currentStepView = characterDrawing;
            }
            else if (newStep == Step.FreeDrawing)
            {
                characterFreeDrawing.Reset();
                _currentStepView = characterFreeDrawing;
            }

            if (_currentStepView != null)
            {
                _currentStepView.Show();
            }

            _currentStep = newStep;
        }

        private void _InitForCharacter(GameObject go)
        {
            if (_currentTemplate != null)
            {
                Destroy(_currentTemplate.gameObject);
            }

            GameObject newGo = (GameObject)Instantiate(go);
            Transform[] tfs = go.GetComponentsInChildren<Transform>();
            foreach (Transform t in tfs)
            {
                t.gameObject.layer = LayerMask.NameToLayer("Template3D");
            }

            newGo.transform.position = go.transform.position;
            newGo.transform.rotation = go.transform.rotation;
            Vector3 firstCPScale = characterSelect.characterPos.transform.localScale;
            Vector3 newScale = Vector3.Scale(go.transform.lossyScale, characterSizing.characterPos.transform.localScale);
            newScale.x /= firstCPScale.x;
            newScale.y /= firstCPScale.y;
            newScale.z /= firstCPScale.z;
            newGo.transform.localScale = newScale;

            Template3D newTemplate = newGo.GetComponent<Template3D>();
            if (newTemplate == null)
            {
                newTemplate = newGo.AddComponent<Template3D>();
            }

            newTemplate.character = characterSelect.selectedCharacterName;
            newTemplate.userId = userId;
            newTemplate.userSeq = userSeq;

            Dragon dragon = newGo.GetComponent<Dragon>();
            if (dragon == null)
            {
                dragon = newGo.AddComponent<Dragon>();
            }
            dragon.characterName = characterSelect.selectedCharacterName;

            _currentTemplate = newTemplate;
            modelControl.model = newTemplate.gameObject;
        }

        public void Next()
        {
            // 먹이주기 화면은 별도 창(AToolPetFeedPanel)로 분리. 호환성을 위해 주석 처리
            //if (_currentStep == Step.FreeDrawing)
            if (_currentStep == Step.Drawing)
            {
                gameObject.SetActive(false);
                if (endStep != null)
                    endStep(userId);
            }
            else
            {
                int val = (int)_currentStep;
                currentStep = (Step)(val + 1);
            }
        }
    }
}