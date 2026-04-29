using UnityEngine;

public class UITouchMotionGuidance : MonoBehaviour
{
    public UI2DSprite sprite;
    public UILabel label;

    public bool Active
    {
        get { return gameObject.activeInHierarchy; }
        set { gameObject.SetActive(value); }
    }
}