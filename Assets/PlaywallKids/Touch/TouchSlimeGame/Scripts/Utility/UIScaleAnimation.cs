using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIScaleAnimation : MonoBehaviour
{
    #region Various
    public float duration;
    public float startDelay;
    public Vector3 scaleTo;
    public AnimationCurve animationCurve;
    #endregion

    #region Property
    GameObject mObj;
    public GameObject obj
    {
        get
        {   
            if (mObj == null) 
                mObj = this.gameObject;
            return mObj;
        }
     
    }

    #endregion

    #region Unity Func
    void Awake()
    {
        if (duration == 0) duration = 0.2f;
        if (startDelay == 0) startDelay = 0.1f;
        if (scaleTo == Vector3.zero) scaleTo = Vector3.one;

       SetDisable();
    }

    void OnEnable()
    {
        Open();
    }

    void OnDisable()
    {
        Close();
    }
    #endregion


    #region USER TYPE FUNC
   public void Open()
   {
        Init(); 
   }

    public void Close()
   { SetDisable(); }

    void Init()
    {
       // transform.localScale = Vector3.zero;
        TweenScale tween = TweenScale.Begin(obj, duration, scaleTo);
        tween.duration   = duration;
        tween.delay      = startDelay;
        tween.animationCurve = animationCurve;
    }

    void SetDisable()
    {
        transform.localScale = Vector3.zero;
        obj.SetActive(false);
    }

    #endregion

}
