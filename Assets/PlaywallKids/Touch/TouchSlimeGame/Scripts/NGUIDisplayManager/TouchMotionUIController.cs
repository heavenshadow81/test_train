using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchMotionUIController : MonoBehaviour {

    public TouchMotionComboManager comboManager; // NHUI ComboManager GameObject
    public TouchMotionCountDownController countDownDisplay;     // NGUI Countdown GameObject
    public UIViewFadeControllder fadeController;
    public TouchMotionScoreController scoreDisplay;            // NGUI ScoreManager GameObject
    public UITimeDisplay timeDisplay;             // NGUI Time Display GameObject
    public UIResult result;
    public ComboDisplay pointManager;              // NGUI Popup NumManager GameObject
    public UITouchMotionGuidance guidance;
    public UITouchMotionGuidance popupWindow;
    public UIBrokenGlassManager brokenGlassManager;

    static TouchMotionUIController mUIController;
    public static TouchMotionUIController instance
    {
        get
        {
            if(mUIController == null)
            {   mUIController = GameObject.FindObjectOfType<TouchMotionUIController>(); }
            return mUIController;
        }
    }
}
