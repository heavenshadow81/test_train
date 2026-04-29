using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Slot_HNS : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI slotText;

    private void Awake()
    {
        slotText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetText(string newText)
    {
        if (slotText != null)
        {
            slotText.text = newText;
        }
    }

    public string GetText()
    {
        return slotText.text;
    }
}
