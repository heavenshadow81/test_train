using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageUIManager : MonoBehaviour
{
    private Button closeBtn;
    [SerializeField] TextMeshProUGUI errorMessage;

    private void Awake()
    {
        closeBtn = GetComponentInChildren<Button>();
    }

    private void Start()
    {
        closeBtn.onClick.AddListener(CloseMessage);
    }

    private void CloseMessage()
    {
        gameObject.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        gameObject.SetActive(true);
        errorMessage.text = message;
    }

}

