using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LetterSlot_CEW : MonoBehaviour
{
    public TextMeshProUGUI letterText; // UI 텍스트 컴포넌트

    private string letter;

    public void Init()
    {
        letterText.text = "";
    }

    public string GetLetter()
    {
        return letterText.text;
    }

    public void SetLetter(string newLetter)
    {
        letter = newLetter;
        letterText.text = letter;
    }
}
