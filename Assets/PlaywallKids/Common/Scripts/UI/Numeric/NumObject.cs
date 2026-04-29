using UnityEngine;
using Numeric = TouchMotionScoreController;

public class NumObject : MonoBehaviour
{
    public UISprite[] imagesOfNumeric;
    public string spriteName;

    public int num
    {
        set
        {
            if (value > 0)
            {
                for (int i = 0; i < imagesOfNumeric.Length; ++i)
                    imagesOfNumeric[i].alpha = 1f;
                Numeric.ChangScore(imagesOfNumeric, NumericSplit.Split(value), spriteName);
            }
        }
    }
}