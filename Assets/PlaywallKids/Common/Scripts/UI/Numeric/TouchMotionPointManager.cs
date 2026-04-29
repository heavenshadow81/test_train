using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchMotionPointManager : MonoBehaviour
{
    #region Private value
    private CObjectList<NumObject> mList;
    private UIPanel mPanel;
    private GameObject mObj;
    private const string szPath = "TouchMotion/";
    private const string szImageFile = "objNum";
    private bool bStart;
    #endregion

    #region Public value
    public UIAtlas numAtlasPrefab;
    public GameObject numPrefab;
    [Range(0.1f, 2f)]
    public float width;
    [Range(0.1f, 2f)]
    public float height;
    public string spriteName;
    #endregion

    #region Property
    public UIPanel cPanel
    {
        get
        {
            if (mPanel == null)
                mPanel = this.gameObject.GetComponent<UIPanel>();
            return mPanel;
        }
    }

    public GameObject obj
    {
        get
        {
            if (mObj == null)
                mObj = this.gameObject;
            return mObj;
        }
    }

    public CObjectList<NumObject> spriteList
    {
        get
        {
            if (mList == null)
            {
                mList = new CObjectList<NumObject>(5,
               () =>
               {
                   if (numPrefab == null)
                       numPrefab = Resources.Load(szPath + szImageFile) as GameObject;
                   GameObject temp = NGUITools.AddChild(this.gameObject, numPrefab);
                   temp.SetActive(false);

                   NumObject num = temp.GetComponent<NumObject>();
                   num.spriteName = spriteName;

                   for (int i = 0, len = num.imagesOfNumeric.Length; i < len; ++i)
                   {
                       num.imagesOfNumeric[i].atlas = numAtlasPrefab;
                       num.imagesOfNumeric[i].spriteName = spriteName + 0.ToString();
                       num.imagesOfNumeric[i].width = (int)(num.imagesOfNumeric[i].width * width);
                       num.imagesOfNumeric[i].height = (int)(num.imagesOfNumeric[i].width * height);
                   }

                   return num;
               },
               (NumObject temp) =>
               { return !temp.gameObject.activeInHierarchy; }
               );
            }
            return mList;
        }
    }

    List<NumObject> _list;
    private List<NumObject> activatedObjectsList
    {
        get
        {
            if (_list == null)
            {
                _list = new List<NumObject>();
            }

            return _list;
        }
    }

    public bool Active
    {
        get { return gameObject.activeSelf; }
        set { gameObject.SetActive(value); }
    }
    #endregion

    #region Unity Func

    void Awake()
    {
        mList = spriteList;
    }

    void OnEnable()
    {
        bStart = false;
        Beginning();
    }

    void OnDisable()
    {
        ComeToAStop();
    }
    #endregion

    #region UserTypeFunc
    public void DisplayScore(Vector3 _pos, int _num)
    {
        NumObject numObj = spriteList.GetObject();
        numObj.gameObject.SetActive(true);
        numObj.transform.localPosition = _pos;
        numObj.num = _num;
        activatedObjectsList.Add(numObj);
    }

    public void Beginning()
    {
        if (bStart) return;
        bStart = true;
        StartCoroutine(CheckActivatedObjectsProcess());
    }

    public void ComeToAStop()
    {
        foreach (NumObject _obj in activatedObjectsList)
        {
            _obj.gameObject.SetActive(false);
        }

        activatedObjectsList.Clear();
    }

    #endregion

    IEnumerator CheckActivatedObjectsProcess()
    {
        GameObject _obj = this.gameObject;
        while (_obj.activeInHierarchy)
        {
            for (int _index = 0, _count = activatedObjectsList.Count; _index < _count;)
            {
                NumObject _num = activatedObjectsList[_index];
                float _sum = 0f;
                for (int i = 0, len = _num.imagesOfNumeric.Length; i < len; ++i)
                {
                    float _alpha = _num.imagesOfNumeric[i].alpha;
                    _num.imagesOfNumeric[i].alpha = _alpha = Mathf.Lerp(_alpha, 0, Time.deltaTime);
                    _sum += _alpha;
                }

                _num.transform.localPosition += new Vector3(0, Time.fixedDeltaTime * 50f, 0);

                if (_sum < 0.03f)
                {
                    _num.gameObject.SetActive(false);
                    --_count;
                    activatedObjectsList[_index] = activatedObjectsList[_count];
                    activatedObjectsList[_count] = _num;
                    activatedObjectsList.RemoveAt(_count);
                }
                else
                {
                    ++_index;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }
}