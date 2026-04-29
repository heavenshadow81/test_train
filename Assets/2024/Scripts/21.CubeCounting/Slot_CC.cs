using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Slot_CC : MonoBehaviour
{
    TextMeshProUGUI hintText;
    TextMeshProUGUI timerText;

    private void Awake()
    {
        TextMeshProUGUI[] textComponents = GetComponentsInChildren<TextMeshProUGUI>();

        foreach (var textComponent in textComponents)
        {
            if (textComponent.name == "Hint_txt")
            {
                hintText = textComponent;
            }
            else if (textComponent.name == "Timer_txt")
            {
                timerText = textComponent;
            }
        }
    }

    public void SetHintText(string newText)
    {
        if (hintText != null)
        {
            hintText.text = newText;
        }
    }

    public string GetHintText()
    {
        return hintText.text;
    }

    public void SetTimerText(string newText)
    {
        if (timerText != null)
        {
            timerText.text = newText;
        }
    }

    public TextMeshProUGUI GetTimerTextComponent()
    {
        return timerText;
    }
}
