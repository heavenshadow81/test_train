using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class NGUIWidgetAlpha : MonoBehaviour {
    #region Public variables
    public float alpha = 1.0f;
    #endregion

    #region Private variables
    private UIPanel _panel;
    private UIWidget _widget;

    private float _prevAlpha = float.MinValue;
    #endregion

    public void Start()
    {
        _panel = GetComponent<UIPanel>();
        _widget = GetComponent<UIWidget>();
    }

    public void Update()
    {
        if (Mathf.Abs(_prevAlpha - alpha) >= 0.00001f)
        {
            if (_panel != null)
            {
                _panel.alpha = alpha;
            }
            else if (_widget != null)
            {
                _widget.alpha = alpha;
            }
            _prevAlpha = alpha;
        }
    }
}
