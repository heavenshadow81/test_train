using UnityEngine;
using System.Collections.Generic;
using ML.PlaywallKids.Common;

public class TouchMotionComboManager : MonoBehaviour {
    public enum EState
    {  NONE, READY, APPEAR, COMBO_COUNT, DISAPPEAR, HIDE, LENGTH  }

    #region public variable
    public UISprite[] imgArr;
    public string fileName;
    [Range(1f, 10f)]
    public float jumpHeight;
    #endregion

    #region property
    BezierMove bezierMiove
    {
        get{
            if (mBezierMove == null)
                mBezierMove = this.GetComponent<BezierMove>();
            return mBezierMove;
        }
    }

    BezierMove numBezierMove
    {
        get
        {
            if(mNumBezier == null)
            {   mNumBezier = imgArr[0].cachedGameObject.GetComponent<BezierMove>();  }
            return mNumBezier;
        }
    }

    public Transform cachedTransform
    {
        get
        {
            if(mTransform == null)
            {   mTransform = this.transform; }
            return mTransform;
        }
    }

    public bool Active
    {
        get
        {
            return this.gameObject.activeInHierarchy;
        }

        set
        {
            this.gameObject.SetActive(value);
        }
    }

    public EState currentState
    {
        set
        {
            if(value != mstate || value == EState.NONE)
            {
                preState = mstate;
                switch(value)
                {
                    case EState.HIDE: 
                        bezierMiove.origin = cachedTransform.localPosition;
                        bezierMiove.target = originPos;
                        break;
                    case EState.APPEAR:
                        SetDisable();
                        bezierMiove.origin = originPos;
                        bezierMiove.target = targetPos;
                        break;
                }
                mstate = value;
            }
        }

        get
        {   return mstate;  }
    }
    #endregion
    private Vector3 originPos;
    private Vector3 targetPos;
    Vector3 numPos;
    BezierMove mBezierMove;
    BezierMove mNumBezier;
    Transform mTransform;
    EState mstate;
    EState preState;

    void Awake()
    {
        for(int i  = 0 ; i< imgArr.Length ; ++i)
        {   imgArr[i].cachedGameObject.SetActive(false);  }

        originPos = new Vector3(-1*( (ScreenUtil.NGUIWidth / 2) + (imgArr[0].width * 10) ), 0.70f * (ScreenUtil.NGUIHeight / 2), 0);
        targetPos = new Vector3(-0.7f * (ScreenUtil.NGUIWidth / 2), 0.70f * (ScreenUtil.NGUIHeight / 2), 0);
        if (jumpHeight == 0) jumpHeight = 5f;
        numPos = imgArr[0].cachedTransform.localPosition;
        
        currentState = EState.HIDE;
    }

    public void DisplayCombo(int _iComboCnt)
    {
        if (_iComboCnt > 999) return;
        currentState = EState.APPEAR;

        List<int> list = NumericSplit.Split(_iComboCnt);
        for(int i = 0 ,length = list.Count; i< length ; ++i)
        {
            if (!imgArr[i].cachedGameObject.activeInHierarchy)
            { imgArr[i].cachedGameObject.SetActive(true); }

            imgArr[i].spriteName = fileName + list[i].ToString();
        }

        if(_iComboCnt > 1)
        {
            numBezierMove.origin = numPos;
            numBezierMove.target = numPos + new Vector3(0, jumpHeight, 0);
        }
    }

    public void DisappearDisplay()
    {   currentState = EState.HIDE;  }

    void OnEnable()
    {
        cachedTransform.localPosition = originPos;
    }

    void OnDisable()
    {
        SetDisable();
    }


    void SetDisable()
    {
        for (int i = 0; i < imgArr.Length; ++i)
        { imgArr[i].cachedGameObject.SetActive(false); }
    }
}
