using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card_SP : MonoBehaviour
{
    public Image cardColor;
    public Image shape;

    private void Awake()
    {
        Transform cardColorTransform = transform.Find("CardColor");
        if (cardColorTransform != null)
        {
            cardColor = cardColorTransform.GetComponent<Image>();
        }

        Transform shapeTransform = transform.Find("Shape");
        if (shapeTransform != null)
        {
            shape = shapeTransform.GetComponent<Image>();
        }
    }

    public void SetCardData(Color cardColorValue, Color shapeColorValue, Sprite shapeSprite)
    {
        if (cardColor != null)
        {
            cardColor.color = cardColorValue;
        }

        if (shape != null)
        {
            shape.color = shapeColorValue;
            shape.sprite = shapeSprite;
        }
    }
}
