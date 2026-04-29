using UnityEngine;

namespace ML.PlaywallKids.EAS
{
    using DragonPark;
    public class EASAToolRemote3DPanel : EASAnimatablePanel
    {
        #region Public variables
        public EASATool3DModelListPanel modelListPanel;
        public EASAToolRemote3DStepPanel stepPanel;
        public EASAToolRemote3DDrawingPanel drawingPanel;
        #endregion

        #region Private variables
        private EASAnimatablePanel _currentPanel;
        private bool _sendNextStepPacket = false;
        #endregion

        public override void BeginShow()
        {
            base.BeginShow();

            modelListPanel.Deactive();
            stepPanel.Deactive();
            drawingPanel.Deactive();

            _currentPanel = modelListPanel;

            stepPanel.mainPanel = this;
            drawingPanel.mainPanel = this;

            modelListPanel.socket = socket;
            stepPanel.socket = socket;
            drawingPanel.socket = socket;

            _sendNextStepPacket = false;

            NextStep();
        }

        public void Update()
        {
            if (connected && isShowing)
            {
                while (true)
                {
                    EASPacket packet = socket.Receive(EASPacket.kType3D);

                    if (packet != null)
                    {
                        ProcessPacket(packet);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public void ProcessPacket(EASPacket packet)
        {
            if (packet != null)
            {
                if (_currentPanel == modelListPanel)
                {
                    if (packet.GetString("data/model") != null)
                    {
                        modelListPanel.selectedModelName = packet.GetString("data/model");
                    }
                    if (packet.GetBool("data/command/next_step"))
                    {
                        //_sendNextStepPacket = true;

                        NextStep();
                    }
                }
                else if (_currentPanel == stepPanel)
                {
                    stepPanel.ProcessPacket(packet);
                }
                else if (_currentPanel == drawingPanel)
                {
                    drawingPanel.ProcessPacket(packet);
                }
            }
        }

        public void NextStep()
        {
            if (modelListPanel.isShowing)
            {
                modelListPanel.Hide();
                stepPanel.modelName = modelListPanel.selectedModelName;
                _currentPanel = stepPanel;
            }
            else if (stepPanel.isShowing)
            {
                stepPanel.Hide();
                drawingPanel.model = stepPanel.generatedGameObject;
                stepPanel.generatedGameObject = null;
                _currentPanel = drawingPanel;
            }
            else if (drawingPanel.isShowing)
            {
                // model
                GameObject model = drawingPanel.model;
                foreach (Template3D template in drawingPanel.modelControl.templates)
                {
                    TCCamera.sharedInstance.UnregisterTemplate(template);
                }
                drawingPanel.model = null;

                // enables animator
                Animator animator = model.GetComponent<Animator>();
                if (animator == null) animator = model.GetComponentInChildren<Animator>();
                if (animator != null) animator.enabled = true;

                // attach the animation control
                FreeDrawingAnimationControl animationControl = model.AddComponent<FreeDrawingAnimationControl>();

                // makes default accessories
                FreeDrawingRobotBone robot = model.GetComponent<FreeDrawingRobotBone>();
                if (robot != null)
                {
                    robot.PrepareDefaultAccessories();
                }
                FreeDrawingCarBone car = model.GetComponent<FreeDrawingCarBone>();
                if (car != null)
                {
                    car.PrepareDefaultAccessories();
                }
                FreeDrawingAirplaneBone airplane = model.GetComponent<FreeDrawingAirplaneBone>();
                if (airplane != null)
                {
                    airplane.PrepareDefaultAccessories();
                }

                // set layer as "MainScene"
                model.SetLayerRecursively("MainScene");

                // make transform parent as dragon park map
                model.transform.parent = EASServerMapManager.sharedInstance.dragonPark.transform;

                if (robot != null)
                {
                    model.transform.position = new Vector3(22.0f, 12.0f, Random.Range(-2.0f, 2.0f));

                    // land
                    animationControl.Land();

                    // enables effect 
                    FreeDrawingRobotEffect effect = model.GetComponent<FreeDrawingRobotEffect>();
                    if (effect != null)
                    {
                        effect.Attach();
                    }
                }
                else
                {
                    model.transform.position = new Vector3(22.0f, 0.0f, Random.Range(-2.0f, 2.0f));
                }

                // makes the model viewing front
                model.transform.localRotation = Quaternion.Euler(0, 90, 0);
                model.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                // register the instantiated object controller
                SimpleInstantiatedTemplateControl.Register(model, Random.Range(0, CustomInput.maxUserCount));

                // hide the panel
                Hide();

                // set current panel as null
                _currentPanel = null;
            }
            else
            {
                _currentPanel = modelListPanel;
            }

            if (_currentPanel != null)
            {
                _currentPanel.Show();
            }

            if (_sendNextStepPacket)
            {
                _sendNextStepPacket = false;
                if (connected)
                {
                    EASPacket packet = new EASPacket();
                    packet.type = EASPacket.kType3D;
                    packet.Set("data/command/next_step", true);
                    socket.Send(packet);
                }
            }
        }
    }
}