using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommonButtonManager : MonoBehaviour
{
    const string filePath = "Atlas/";
    const string prefixAtlasWord = "IconAtlas_";

    public UIGrid menuGrid;
    [HideInInspector]
    public UIAtlas iconAtlas;
    public GameObject tweenTarget;
    public GameObject buttonPrefab;
    public UIViewFadeControllder fadeController;
    public Font buttonFont;
    [HideInInspector]
    public BigboardContentMode currentContent;
    public bool enableFadeOut;
         
    private bool bClickListBtn;
    private bool bAppear;
    private bool bClick;
    private Vector3 menuPosition;
    /// <summary>
    /// NGUI.Atlas 파일 명 접미사
    /// </summary>
    private string atlasNameSuffix;
    private EventDelegate  callbackDoTweenScale;
    private EventDelegate callbackDoFade;
    private TweenScale tween;
    private Coroutine tweenCoroutine;

    void Awake()
    {
        menuPosition = tweenTarget.transform.localPosition;
        
        callbackDoTweenScale = new EventDelegate(DoTweenScale);
        callbackDoFade = new EventDelegate(FadeOut);
    }

    void OnEnable()
    {
        bClick = false; 
        bAppear = false;
        bClickListBtn = false;
        tweenTarget.transform.localScale = Vector3.zero;
        if (iconAtlas!= null)
        atlasNameSuffix = iconAtlas.name;
    }

    void OnDisable()
    {
        if (bAppear) DoTweenScale();
        if (tween != null && tween.onFinished.Count != 0)
        {   
            tween.onFinished.Clear();
            tween = null;
        }
    }

    void InitClick()
    { bClick = false; }

    void Start()
    {
        StartCoroutine(CheckButton());
    }

    void FadeOut()
    {
        if (!bClickListBtn && enableFadeOut)
        { fadeController.FadeEffect(true, 1f); }
    }

    public void ClickListBtn()
    {
        bClickListBtn = true;
    }

    IEnumerator CheckButton()
    {
        yield return new WaitForEndOfFrame();
        List<Transform> list = menuGrid.GetChildList();
        if (list.Count == 1)
        {
            UIButton btn = list[0].gameObject.GetComponent<UIButton>();
            EventDelegate.Remove(btn.onClick, callbackDoTweenScale);
            EventDelegate.Execute(btn.onClick);
            fadeController.FadeEffect(false, 0f);
        }
        else
        {   fadeController.FadeEffect(false, 0f, new EventDelegate( DoTweenScale) );  }
    }

    public void DoTweenPosition()
    {
        Vector3 target = Vector3.zero;
        target.x = (bAppear ? -1000 : 1000);
        bAppear = !bAppear;
        TweenPosition tween = TweenPosition.Begin(tweenTarget, 0.5f, target + tweenTarget.transform.localPosition);
        tween.method = UITweener.Method.Linear;
    }

    public void DoTweenScale()
    {
        if (bClick) return;
        bClick = true;
        bAppear = !bAppear;
        if (tweenCoroutine != null) StopCoroutine(tweenCoroutine);

        List<EventDelegate> callbackList = new List<EventDelegate>();
        if (!tweenTarget.activeInHierarchy) tweenTarget.SetActive(true);
        if (bAppear) { tweenCoroutine= StartCoroutine(DoTweenScaleProcess(true, callbackList)); }
        else
        {
            if (enableFadeOut) callbackList.Add(callbackDoFade);
            if (tweenTarget.activeInHierarchy)
            { tweenCoroutine = StartCoroutine(DoTweenScaleProcess(false, callbackList)); }
        }
    }

    IEnumerator DoTweenScaleProcess(bool _bAscending, List<EventDelegate> _callbackFuncs = null)
    {
        if (!tweenTarget.activeInHierarchy) tweenTarget.SetActive(true);
        Transform _tweenTarget = tweenTarget.transform;
        float _speed = 5f;

        _tweenTarget.localScale = _bAscending ? Vector3.zero : Vector3.one;
        bool _bComplete = false;
        do
        {
            Vector3 _scale = _tweenTarget.localScale;
            _scale += Vector3.one * Time.deltaTime * (_bAscending ? _speed : -_speed );
            if (_bComplete = _scale.x < 0f) _scale = Vector3.zero;
            else if ( _bComplete =  _scale.x > 1f) _scale = Vector3.one;

            _tweenTarget.localScale = _scale;
            yield return new WaitForEndOfFrame();
        } while (!_bComplete);

        if (_callbackFuncs != null)
        {
            EventDelegate.Execute(_callbackFuncs);
        }
        yield return new WaitForSeconds(0.15f);
        if (_callbackFuncs != null)
        {
            _callbackFuncs.Clear();
            _callbackFuncs = null;
        }
        InitClick();
    }

    /// <summary>
    /// 각 모드 별 콘텐츠 메뉴 생성함수
    /// </summary>
    /// <param name="_suffixAtlasWord">아틀라스 접미사</param>
    /// <param name="spriteName"></param>
    /// <param name="_contentsExplainingLabel"></param>
    /// <param name="contentType"></param>
    public void GenerateButton(string _suffixAtlasWord, string spriteName, string _contentsExplainingLabel, int contentType)
    {
        if (menuGrid == null)
            menuGrid = GetComponentInChildren<UIGrid>();

        // NGUI.Atlas가 Null or 등록 되어 있는 NGUI.Altas와 현재 모드의 Atlas 의 이름이 다르면 새로 생성
        if(iconAtlas == null || string.Compare(atlasNameSuffix, _suffixAtlasWord) != 0 )
        {
            atlasNameSuffix = _suffixAtlasWord;
            // 동적 로드 할 NGUI.Atlas 의 Full Path
            string fullPathOfAtlas = filePath + prefixAtlasWord + _suffixAtlasWord;

            GameObject file = Resources.Load(fullPathOfAtlas, typeof(GameObject)) as GameObject;
            iconAtlas = file.GetComponent<UIAtlas>();
        }
        // 버튼 동적 생성
        CommonContentsButton btnObj = NGUITools.AddChild(menuGrid.gameObject, buttonPrefab).GetComponent<CommonContentsButton>();
        btnObj.gameObject.name = spriteName;
        btnObj.label = _contentsExplainingLabel;    // Display explaning name of Menu
        btnObj.contentIndex = contentType;          // contents ID

        UIButton button = btnObj.button;
        if( button.onClick == null) {   button.onClick = new List<EventDelegate>();  }
        
        UISprite sprite = btnObj.sprite;
        sprite.atlas = iconAtlas;
        sprite.spriteName = spriteName;

        menuGrid.repositionNow = true;
        
        button.onClick.Add(new EventDelegate( 
                                             ()=>{
                                                 bClickListBtn = false;
                                                 // 모드 내 실행 시키고 싶은 콘텐츠를 NextContent() 함수내 재정의 해야함
                                                 GameObject[] objs = (GameObject[])GameObject.FindObjectsOfType<GameObject>();
                                                 foreach(GameObject obj in objs)
                                                 { 
                                                    obj.BroadcastMessage("NextContent",
                                                    btnObj.contentIndex,
                                                    SendMessageOptions.DontRequireReceiver
                                                    );
                                                     
                                                 }
                                                }
                                              ));
        
        // 현재 메뉴버튼들을 Tween animation 시킴
        button.onClick.Add(new EventDelegate( () => { DoTweenScale(); }));  //regist to function 

    }
}