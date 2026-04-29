using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Base class of animatable panel (contains Tweens, such as)
/// </summary>
public class AnimatablePanel : MonoBehaviour
{
    #region Properties
    private GameObject _cachedGameObject = null;
    /// <summary>
    /// Cached Game Object (for performance).
    /// </summary>
    public GameObject cachedGameObject
    {
        get
        {
            if (_cachedGameObject == null)
                _cachedGameObject = gameObject;
            return _cachedGameObject;
        }
    }

    private Transform _cachedTransform = null;
    /// <summary>
    /// Cached Transform (for performance).
    /// </summary>
    public Transform cachedTransform
    {
        get
        {
            if (_cachedTransform == null)
                _cachedTransform = transform;
            return _cachedTransform;
        }
    }

    private UIPanel _panel = null;
    public UIPanel panel
    {
        get
        {
            if (_panel == null)
            {
                _panel = GetComponent<UIPanel>();
            }
            return _panel;
        }
    }

    /// <summary>
    /// Is panel showing?
    /// </summary>
    private bool _isShowing = false;
    public bool isShowing
    {
        get
        {
            return _isShowing && cachedGameObject.activeSelf;
        }
    }
    #endregion

    #region Private variables
    private UITweener[] _innerTweens = null;
    private UIButton[] _innerButtons = null;
    private bool[] _innerButtonActiveStates = null;

    private EventDelegate _activeEventDel = null;
    private EventDelegate _deactiveEventDel = null;
    #endregion

    #region Events
    // The custom event handler you can add or delete.
    public event Action<AnimatablePanel> onBeginShow;
    public event Action<AnimatablePanel> onBeginHide;
    #endregion

    public void Show()
    {
        if (_isShowing && cachedGameObject.activeInHierarchy)
            return;
        _isShowing = true;
        _CheckInnerVariablesAreAllocated();

        if (!cachedGameObject.activeSelf)
            cachedGameObject.SetActive(true);
        DisableWidgets();

        // Events
        BeginShow();
        if (onBeginShow != null)
            onBeginShow(this);
        
        // Tweens
        UITweener longestTween = _GetLongestTween(true);
        if (longestTween != null)
        {
            longestTween.AddOnFinished(_activeEventDel);
            _PlayTween(true);
        }
        else
        {
            Active();
        }
    }

    public void Hide()
    {
        if (!isShowing) return;
        _isShowing = false;
        _CheckInnerVariablesAreAllocated();

        // Events
        BeginHide();
        if (onBeginHide != null)
            onBeginHide(this);

        // Tweens
        UITweener longestTween = _GetLongestTween(true);
        if (longestTween != null)
        {
            longestTween.AddOnFinished(_deactiveEventDel);
            DisableWidgets();
            _PlayTween(false);
        }
        else
        {
            Deactive();
        }
    }

    /// <summary>
    /// This method will be called when the panel begins showing.
    /// </summary>
    public virtual void BeginShow()
    {
    }

    /// <summary>
    /// This method will be called when the panel begins hiding.
    /// </summary>
    public virtual void BeginHide()
    {
    }

    /// <summary>
    /// This method will be called after showing tweens are completed.
    /// </summary>
    public virtual void Active()
    {
        EnableWidgets();
    }
    
    /// <summary>
    /// This method will be called after hiding tweens are completed.
    /// </summary>
    public virtual void Deactive()
    {
        if (_isShowing)
        {
            _isShowing = false;
            _CheckInnerVariablesAreAllocated();

            foreach (UITweener tween in _innerTweens)
                tween.ResetToBeginning();
        }
        cachedGameObject.SetActive(false);
    }

    /// <summary>
    /// This method will be called when the panel should enable the widgets.
    /// </summary>
    public virtual void EnableWidgets()
    {
        if (_innerButtons != null)
        {
            for (int i = 0; i < _innerButtons.Length; i++)
            {
                UIButton btn = _innerButtons[i];
                if (btn != null)
                {
                    btn.isEnabled = _innerButtonActiveStates[i];
                }
            }
        }
    }


    /// <summary>
    /// This method will be called when the panel should disable the widgets.
    /// </summary>
    public virtual void DisableWidgets()
    {
        if (_innerButtons != null)
        {
            for (int i = 0, cnt = _innerButtons.Length; i < cnt; i++)
            {
                UIButton btn = _innerButtons[i];
                if (btn != null)
                {
                    //_innerButtonActiveStates[i] = btn.isEnabled;
                    btn.isEnabled = false;
                }
            }
        }
    }

    public void RefreshInnerButtonActiveState()
    {
        _PerformRefreshInnerButtonActiveState(true);
    }

    private void _PerformRefreshInnerButtonActiveState(bool refreshInnerButtonsList)
    {
        List<UIButton> prevInnerButtons = new List<UIButton>();
        if (_innerButtons != null)
        {
            prevInnerButtons.AddRange(_innerButtons);
        }

        List<bool> prevInnerButtonActiveStates = new List<bool>();
        if (_innerButtonActiveStates != null)
        {
            prevInnerButtonActiveStates.AddRange(_innerButtonActiveStates);
        }

        if (_innerButtons == null || refreshInnerButtonsList)
            _innerButtons = GetComponentsInChildren<UIButton>(true);

        if (_innerButtonActiveStates == null || refreshInnerButtonsList)
        {
            _innerButtonActiveStates = new bool[_innerButtons.Length];

            for (int i = 0; i < _innerButtons.Length; i++)
            {
                UIButton btn = _innerButtons[i];
                if (prevInnerButtons.Count > 0 && prevInnerButtons.Contains(btn))
                {
                    int idx = prevInnerButtons.IndexOf(btn);
                    bool prevState = prevInnerButtonActiveStates[idx];
                    _innerButtonActiveStates[i] = prevState;
                }
                else
                {
                    _innerButtonActiveStates[i] = btn.isEnabled;
                }
            }
        }
    }

    private void _CheckInnerVariablesAreAllocated()
    {
        // inner buttons
        _PerformRefreshInnerButtonActiveState(false);

        // event delegates
        if (_activeEventDel == null)
        {
            _activeEventDel = new EventDelegate(this, "Active");
        }
        if (_deactiveEventDel == null)
        {
            _deactiveEventDel = new EventDelegate(this, "Deactive");
        }

        // tweens
        if (_innerTweens == null)
        {
            _innerTweens = GetComponents<UITweener>();
        }

        // all tweens have to be disabled.
        foreach (UITweener tween in _innerTweens)
        {
            if (tween != null) tween.enabled = false;
        }
    }

    private void _PlayTween(bool forward)
    {
        for (int i = 0, imax = _innerTweens.Length; i < imax; ++i)
        {
            UITweener tween = _innerTweens[i];
            /*
            if (forward)
            {
                tween.ResetToBeginning();
            }
            */
            tween.Play(forward);
        }
    }

    private UITweener _GetLongestTween(bool clearActiveOrDeactiveEventDelegate)
    {
        if (_innerTweens != null && _innerTweens.Length > 0)
        {
            float maxLength = 0.0f;
            UITweener longestTween = null;
            for (int i = 0; i < _innerTweens.Length; i++)
            {
                UITweener tween = _innerTweens[i];
                if (tween != null)
                {
                    if (clearActiveOrDeactiveEventDelegate)
                    {
                        tween.RemoveOnFinished(_activeEventDel);
                        tween.RemoveOnFinished(_deactiveEventDel);
                    }

                    if (maxLength < tween.delay + tween.duration)
                    {
                        longestTween = tween;
                        maxLength = tween.delay + tween.duration;
                    }
                }
            }

            return longestTween;
        }

        return null;
    }
}
