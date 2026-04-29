using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.DragonPark
{
    using Common;

    /// <summary>
    /// Authoring Tool <br>
    /// Step 4, Drawing
    /// </summary>
    public class AToolCharacterDrawing : AToolCharacterStep
    {
        public Transform characterPos;
        public Transform palettes;
        public UISlider brushSizeSlider;
        public UISprite brushSizeSmallSprite, brushSizeBigSprite;

        public SimpleModelControl modelControl
        {
            get
            {
                SimpleModelControl value = null;
                if (mainPanel != null)
                {
                    value = mainPanel.modelControl;
                }
                return value;
            }
        }

        public override void Reset()
        {
            if (modelControl != null)
            {
                Template3D template = modelControl.template;
                if (template != null)
                {
                    template.CleanMainTexture();

                    if (template.cachedTransform.parent != characterPos)
                    {
                        template.cachedTransform.parent = characterPos;

                        TweenPosition tp = TweenPosition.Begin(template.gameObject, 0.25f, Vector3.zero);
                        TweenRotation.Begin(template.gameObject, 0.25f, Quaternion.identity);

                        if (tp.onFinished.Count == 0)
                        {
                            tp.onFinished.Add(new EventDelegate(() =>
                            {
                                TCCamera.sharedInstance.RequestRefreshTCRT();
                            }));
                        }
                    }
                }
            }
        }

        public void OnEnable()
        {
            if (modelControl != null)
            {
                modelControl.wantsPaint = true;

                _InitPalettes();
                _ShowBrushColor();
            }
        }

        public void OnDisable()
        {
            if (modelControl != null)
            {
                modelControl.wantsPaint = false;
            }
        }

        public override void Rotate()
        {
            SimpleModelControl modelControl = mainPanel.modelControl;
            modelControl.RotateAndType(true, AutoRotate.RotateType.Right);
        }

        public override void RotateLeft()
        {
            SimpleModelControl modelControl = mainPanel.modelControl;
            modelControl.RotateAndType(true, AutoRotate.RotateType.Left);
        }

        public override void RotateStop()
        {
            modelControl.rotate = false;
        }

        public override void GoToNextStep()
        {
            // 탬플릿 저장하기
            ResourceManager.SaveTemplate3D(mainPanel.currentTemplate);

            // 드래곤 오브젝트 생성, 사용자 앞에 위치
            Template3D template = SimpleInstantiatedTemplateControl.LoadTemplate(mainPanel.currentTemplate.identifier);
            template.gameObject.SetLayerRecursively(LayerMask.NameToLayer("MainScene"));
            template.transform.position = DragonComeToFront.GetDummyPosition("front", mainPanel.userId);
            template.transform.LookAt(DragonComeToFront.GetDummyPosition("frontlook", mainPanel.userId));

            // 캡처
            _Capture(template);

            // 애니메이션 길찾기 끄기
            DragonAnimationControl dragonAnimation = template.GetComponent<DragonAnimationControl>();
            if (dragonAnimation != null)
                dragonAnimation.movesAlongPath = false;

            // 스폰 효과 나타내기
            DragonSpawnEffect spawnEffect = template.GetComponent<DragonSpawnEffect>();
            if (spawnEffect != null)
            {
                int userId = mainPanel.userId;
                if (MenuControl.sharedInstance != null)
                {
                    MenuControl.sharedInstance.Activate(userId);
                    MenuControl.sharedInstance.Deactivate(userId);
                }
                spawnEffect.onComplete += () =>
                {
                // 사용자 메뉴 표시
                if (MenuControl.sharedInstance != null)
                        MenuControl.sharedInstance.ShowPetMenu(userId);
                };
                spawnEffect.Play();
            }

            base.GoToNextStep();
        }

        private void _Capture(Template3D template)
        {
            var printer = MenuControl.sharedInstance.GetDragonPrinter(mainPanel.userId);
            printer.Set(mainPanel.userId, template.gameObject);
            printer.StartCoroutine(_UploadFTP(printer, template.identifier));
        }

        private IEnumerator _UploadFTP(DragonPrinter printer, string identifier)
        {
            while (printer.isCapturing)
                yield return null;
            FTPUploader.Upload(SettingsManager.ftpAddress, SettingsManager.ftpUsername, SettingsManager.ftpPassword, identifier, printer.previewTexture);
        }

        public void ChangeBrushSize()
        {
            if (modelControl != null)
            {
                modelControl.brushSize = Mathf.RoundToInt(8.0f + 24.0f * brushSizeSlider.value);
            }
        }

        public void Clean()
        {
            if (modelControl != null && modelControl.template != null)
                modelControl.template.CleanBrushTexture();
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
                    trigger.onClick.Add(new EventDelegate(_ShowBrushColor));
                }
            }

            if (modelControl != null)
                brushSizeSlider.value = (modelControl.brushSize - 8.0f) / 24.0f;
        }

        private void _ShowBrushColor()
        {
            brushSizeSmallSprite.color = brushSizeBigSprite.color = modelControl.brushColor;
        }
    }
}