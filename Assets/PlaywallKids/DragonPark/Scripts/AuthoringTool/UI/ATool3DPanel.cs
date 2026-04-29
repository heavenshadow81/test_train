using System;
using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.DragonPark
{
    using Common;
    /// <summary>
    /// 드래곤파크 자유롭게 그리기(프리드로잉) 창
    /// </summary>
    public class ATool3DPanel : AnimatablePanel
    {
        #region Public variables
        public ATool3DModelList modelListPanel;
        public ATool3DStep stepPanel;
        public ATool3DDrawing drawingPanel;
        public int userId = 0;
        #endregion

        public override void BeginShow()
        {
            base.BeginShow();

            modelListPanel.Deactive();
            stepPanel.Deactive();
            drawingPanel.Deactive();

            modelListPanel.mainPanel = this;
            stepPanel.mainPanel = this;
            drawingPanel.mainPanel = this;
        }

        public override void Active()
        {
            base.Active();

            NextStep();
        }

        public void NextStep()
        {
            if (modelListPanel.isShowing)
            {
                modelListPanel.Hide();
                stepPanel.modelName = modelListPanel.selectedModelName;
                stepPanel.Show();
            }
            else if (stepPanel.isShowing)
            {
                stepPanel.Hide();
                drawingPanel.model = stepPanel.generatedGameObject;
                drawingPanel.Show();
            }
            else if (drawingPanel.isShowing)
            {
                _Generate();
                Hide();
            }
            else
            {
                modelListPanel.Show();
                modelListPanel.EnableWidgets();
            }
        }


        private void _Generate()
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
            FreeDrawingAnimationControl animationControl = model.GetComponent<FreeDrawingAnimationControl>();
            if (animationControl == null)
                animationControl = model.AddComponent<FreeDrawingAnimationControl>();
            animationControl.comeToFront.userId = userId;

            // makes default accessories
            FreeDrawingObjectBone bone = model.GetComponent<FreeDrawingObjectBone>();
            if (bone != null)
                bone.PrepareDefaultAccessories();

            // set layer as "MainScene"
            model.SetLayerRecursively("MainScene");

            // change the parent.
            model.transform.parent = null;

            // move to the front
            model.transform.position = DragonComeToFront.GetDummyPosition("front", userId);

            // makes the model viewing front
            model.transform.LookAt(DragonComeToFront.GetDummyPosition("frontlook", userId));
            model.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            // register the instantiated object controller
            SimpleInstantiatedTemplateControl.Register(model, userId);

            // per-type settings
            if (bone != null)
            {
                // If the object is robot, shows the landing animation
                if (bone.objectType == FreeDrawingObjectType.Robot)
                {
                    // move up
                    bone.transform.position += Vector3.up * 12.0f;

                    // land
                    if (animationControl != null)
                        animationControl.Land();

                    // enables effect 
                    FreeDrawingRobotEffect effect = bone.GetComponent<FreeDrawingRobotEffect>();
                    if (effect != null)
                    {
                        effect.Attach();
                    }
                }
                else if (bone.objectType == FreeDrawingObjectType.Airplane)
                {
                    // Slightly fly up the airplane 
                    model.transform.position += Vector3.up;
                }
            }

            // Capture image.
            _Capture(model);

            // Shows spawn effect
            DragonSpawnEffect spawnEffect = model.GetComponent<DragonSpawnEffect>();
            if (spawnEffect != null)
            {
                spawnEffect.onComplete += _OnComplete;
                spawnEffect.Play();
            }
            else
            {
                _OnComplete();
            }
        }

        private void _Capture(GameObject model)
        {
            string uuid = Guid.NewGuid().ToString();
            var printer = MenuControl.sharedInstance.GetDragonPrinter(userId);
            printer.Set(userId, model);
            printer.StartCoroutine(_UploadFTP(printer, uuid));
        }

        private IEnumerator _UploadFTP(DragonPrinter printer, string identifier)
        {
            while (printer.isCapturing)
                yield return null;
            FTPUploader.Upload(SettingsManager.ftpAddress, SettingsManager.ftpUsername, SettingsManager.ftpPassword, identifier, printer.previewTexture);
        }

        private void _OnComplete()
        {
            if (MenuControl.sharedInstance != null)
            {
                MenuControl.sharedInstance.ShowFreeDrawingMenu(userId);
            }
        }

        public void PrevStep()
        {
            if (drawingPanel.isShowing)
            {
                drawingPanel.Hide();
                stepPanel.Show();
            }
            else if (stepPanel.isShowing)
            {
                stepPanel.Hide();
                modelListPanel.Show();
            }
        }
    }
}