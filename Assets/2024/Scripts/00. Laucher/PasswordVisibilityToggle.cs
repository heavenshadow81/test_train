using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PasswordVisibilityToggle : MonoBehaviour
{
    [SerializeField] TMP_InputField passwordInputField; 
    [SerializeField] Sprite showIcon; 
    [SerializeField] Sprite hideIcon;
    private Button toggleVisibilityButton; 
    private Image buttonImage; 

    private bool isPasswordVisible = false;

    private void Awake()
    {
        toggleVisibilityButton = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
    }

    private void Start()
    {
        toggleVisibilityButton.onClick.AddListener(TogglePasswordVisibility);
    }

    private void OnDestroy()
    {
        toggleVisibilityButton.onClick.RemoveListener(TogglePasswordVisibility);
    }

    private void TogglePasswordVisibility()
    {
        isPasswordVisible = !isPasswordVisible;
        UpdateInputField();
    }

    private void UpdateInputField()
    {
        if (isPasswordVisible)
        {
            passwordInputField.contentType = TMP_InputField.ContentType.Standard;
            buttonImage.sprite = hideIcon;
        }
        else
        {
            passwordInputField.contentType = TMP_InputField.ContentType.Password;
            buttonImage.sprite = showIcon;
        }

        passwordInputField.ForceLabelUpdate();
        passwordInputField.Select();
    }
}
