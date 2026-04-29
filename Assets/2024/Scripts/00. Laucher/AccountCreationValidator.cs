using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AccountCreationValidator : MonoBehaviour
{
    [SerializeField] TMP_InputField[] inputFields;
    private Button createAccountButton;
    private TextMeshProUGUI buttonText;

    private void Awake()
    {
        createAccountButton = GetComponent<Button>();
        buttonText = createAccountButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        createAccountButton.interactable = false;

        foreach (TMP_InputField inputField in inputFields)
        {
            inputField.onValueChanged.AddListener(delegate { ValidateInputFields(); });
        }
    }

    private void OnDestroy()
    {
        foreach (TMP_InputField inputField in inputFields)
        {
            inputField.onValueChanged.RemoveAllListeners();
        }
    }

    private void ValidateInputFields()
    {
        foreach (TMP_InputField inputField in inputFields)
        {
            if (string.IsNullOrWhiteSpace(inputField.text))
            {
                createAccountButton.interactable = false;
                SetTextAlpha(0.5f);
                return;
            }
        }

        createAccountButton.interactable = true;
        SetTextAlpha(1f);
    }

    private void SetTextAlpha(float alpha)
    {
        Color color = buttonText.color; 
        color.a = alpha;                
        buttonText.color = color;      
    }
}
