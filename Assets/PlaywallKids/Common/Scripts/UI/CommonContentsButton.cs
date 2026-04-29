using UnityEngine;
using System.Collections;

/// <summary>
/// 콘텐츠 버튼 정보
/// </summary>
[RequireComponent(typeof(UIButton))]
public class CommonContentsButton : MonoBehaviour
{
    public BigboardContentMode contens;
    public int contentIndex
    {
        get { return (int)contens; }
        set { contens = (BigboardContentMode)value; }
    }
    
    public UISprite sprite
    {
        get
        {  return GetComponent<UISprite>();  }
    }

    public UIButton button
    {
        get { return GetComponent<UIButton>(); }
    }

    UILabel _label;
    public string label{
        set
        {
            _label = GetComponentInChildren<UILabel>() as UILabel;
            if(_label == null)
            {
                if (this.transform.childCount > 0)
                {
                    Transform t = this.transform;
                    for(int i = 0 , len = t.childCount; i < len ; ++i)
                    {
                        
                        if(t.GetChild(i).GetComponent<UILabel>()!= null )
                        {
                            _label = t.GetChild(i).GetComponent<UILabel>();
                        }
                    }
                }
            }

            if (_label != null)
            { _label.text = value; }
        }

        get
        {
            if(_label == null)
            {
                if (this.transform.childCount > 0)
                {
                    Transform t = this.transform;
                    for (int i = 0, len = t.childCount; i < len; ++i)
                    {

                        if (t.GetChild(i).GetComponent<UILabel>() != null)
                        {
                            _label = t.GetChild(i).GetComponent<UILabel>();
                        }
                    }
                }
            }
            return _label .text;
        }
    }
}

