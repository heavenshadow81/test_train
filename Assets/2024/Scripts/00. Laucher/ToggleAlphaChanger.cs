using UnityEngine;
using UnityEngine.UI;

public class ToggleAlphaChanger : MonoBehaviour
{
    private Toggle autoLoginToggle;
    private Text autoLoginText; 
    private Color textColor;

    private void Awake()
    {
        autoLoginToggle = GetComponent<Toggle>();
        autoLoginText = GetComponentInChildren<Text>();
    }

    private void Start()
    {
        textColor = autoLoginText.color;

        UpdateTextAlpha(autoLoginToggle.isOn);

        autoLoginToggle.onValueChanged.AddListener(UpdateTextAlpha);
    }

    private void OnDestroy()
    {
        autoLoginToggle.onValueChanged.RemoveListener(UpdateTextAlpha);
    }

    private void UpdateTextAlpha(bool isOn)
    {
        float alpha = isOn ? 1f : 0.5f;

        Color newColor = textColor;
        newColor.a = alpha;
        autoLoginText.color = newColor;
    }
}
