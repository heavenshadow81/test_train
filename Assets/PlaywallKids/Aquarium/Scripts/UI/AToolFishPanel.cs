using UnityEngine;
using DG.Tweening;

namespace ML.PlaywallKids.Aquarium
{
    using AuthoringTool;
    /// <summary>
    /// 아쿠아리움 물고기 자유롭게 그리기 창
    /// </summary>
    public class AToolFishPanel : MonoBehaviour
    {
        #region Public variables
        public int userId;
        public UIPanel target;
        public UITexture fillTexture;
        public UITexture drawingTexture;
        public UIPanel palettes;
        public UIPanel tools;
        public UIPanel fishes;
        public UIButton nextButton, okButton;
        public UILabel messageLabel;
        #endregion

        #region Private variables
        private UserData _userData;
        private int _instanceId;
        private float _palettePosY;
        private const float kAnimationTime = 0.25f;
        #endregion
        
        public void Awake()
        {
            _instanceId = target.GetInstanceID();

            // Initialize UIs
            target.cachedTransform.localScale = Vector3.one * 0.8f;
            target.alpha = 0;
            NGUITools.SetActive(fillTexture.cachedGameObject, false);
            NGUITools.SetActive(drawingTexture.cachedGameObject, false);
            _palettePosY = palettes.cachedTransform.localPosition.y; // for animation
            palettes.alpha = 0;
            tools.alpha = 0;

            // Animation
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(0.05f);
            seq.Insert(0.05f, DOTween.To(x => target.cachedTransform.localScale = Vector3.one * x, 0.7f, 0.85f, 0.25f));
            seq.Insert(0.05f, DOTween.To(x => target.alpha = x, 0.0f, 1.0f, 0.25f));
            seq.Play();
        }

        public void Start()
        {
            // Initialize canvas
            Canvas_ canvas = drawingTexture.GetComponent<Canvas_>();
            canvas.brush.diameter = 32;

            // Initialize default user states
            _userData = UserData.Instance();
            _userData.onStateChanged += _OnStateChanged;
            _userData.SetInstanceID(_instanceId, userId);
            _userData.SetBrushTool(_instanceId, UserData.BrushTool.MARKER);
            _userData.SetColor(_instanceId, canvas.brush.color);
            _userData.SetBrushSize(_instanceId, (int)canvas.brush.diameter);
            _userData.SetState(_instanceId, UserData.State.WAIT);
        }

        #region Stage changes
        public void SetWaitMode()
        {
            _HidePalettes();
            _HideTools();
            messageLabel.alpha = 1.0f;
            messageLabel.text = LocalizationManager.GetData(LocalizationKey.AQUARIUM_FISH_SELECT);
            nextButton.gameObject.SetActive(false);
            okButton.gameObject.SetActive(false);
        }

        public void SetCustomizationMode()
        {
            _HidePalettes();
            _HideTools();
            messageLabel.alpha = 1.0f;
            messageLabel.text = LocalizationManager.GetData(LocalizationKey.AQUARIUM_FISH_CUSTOMIZE);
            nextButton.gameObject.SetActive(true);
            okButton.gameObject.SetActive(false);
        }

        public void SetDrawMode()
        {
            _ShowPalettes();
            _ShowTools();
            messageLabel.alpha = 0.0f;
            nextButton.gameObject.SetActive(false);
            okButton.gameObject.SetActive(true);
        }

        public void End()
        {
            _userData.onStateChanged -= _OnStateChanged;
            Sequence seq = DOTween.Sequence();
            seq.Insert(0, target.cachedTransform.DOScale(0.7f, 0.3f));
            seq.Insert(0, DOTween.To(x => target.alpha = x, 1.0f, 0.0f, 0.25f));
            seq.OnComplete(() => { Destroy(gameObject); });

            if (_userData.GetState(_instanceId) == UserData.State.END)
            {
                if (AquariumMenuControl.sharedInstance != null)
                    AquariumMenuControl.sharedInstance.ShowFishMenu(userId);
            }
            else
            {
                if (AquariumMenuControl.sharedInstance != null)
                    AquariumMenuControl.sharedInstance.Activate(userId);
            }
        }

        private void _OnStateChanged(int instanceId, int userId, UserData.State state)
        {
            if (instanceId == _instanceId)
            {
                switch (state)
                {
                    case UserData.State.WAIT:
                        SetWaitMode();
                        break;
                    case UserData.State.CUSTOMIZING_BACK:
                        _userData.SetState(_instanceId, UserData.State.CUSTOMIZING);
                        break;
                    case UserData.State.CUSTOMIZING:
                        SetCustomizationMode();
                        break;
                    case UserData.State.DRAW_START:
                        _userData.SetState(_instanceId, UserData.State.DRAW);
                        break;
                    case UserData.State.DRAW:
                        SetDrawMode();
                        break;
                    case UserData.State.END:
                        End();
                        break;
                }
            }
        }
        #endregion

        #region UI Animations
        private void _HidePalettes()
        {
            DOTween.Kill(palettes);
            if (palettes.alpha > 0)
            {
                DOTween.To(x => palettes.alpha = x, palettes.alpha, 0.0f, palettes.alpha * kAnimationTime).SetTarget(palettes);
                DOTween.To(y =>
                {
                    Vector3 pos = palettes.cachedTransform.localPosition;
                    pos.y = y;
                    palettes.cachedTransform.localPosition = pos;
                }, palettes.cachedTransform.localPosition.y, _palettePosY - 80.0f, palettes.alpha * kAnimationTime).SetTarget(palettes);
            }
        }

        private void _ShowPalettes()
        {
            DOTween.Kill(palettes);
            DOTween.To(x => palettes.alpha = x, palettes.alpha, 1.0f, (1.0f - palettes.alpha) * kAnimationTime).SetTarget(palettes);
            DOTween.To(x => tools.alpha = x, tools.alpha, 1.0f, (1.0f - tools.alpha) * kAnimationTime).SetTarget(tools);
            DOTween.To(y =>
            {
                Vector3 pos = palettes.cachedTransform.localPosition;
                pos.y = y;
                palettes.cachedTransform.localPosition = pos;
            }, Mathf.Max(_palettePosY - 80.0f, palettes.cachedTransform.localPosition.y - 80.0f), _palettePosY, kAnimationTime).SetTarget(palettes);
        }

        private void _HideTools()
        {
            DOTween.Kill(tools);
            if (tools.alpha > 0)
            {
                DOTween.To(x => tools.alpha = x, tools.alpha, 0.0f, tools.alpha * kAnimationTime).SetTarget(tools);
            }
        }

        private void _ShowTools()
        {
            DOTween.Kill(tools);
            DOTween.To(x => tools.alpha = x, tools.alpha, 1.0f, (1.0f - tools.alpha) * kAnimationTime).SetTarget(tools);
        }
        #endregion
    }
}