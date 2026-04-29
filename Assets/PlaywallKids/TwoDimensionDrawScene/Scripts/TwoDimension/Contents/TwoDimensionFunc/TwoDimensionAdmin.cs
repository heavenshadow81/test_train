using UnityEngine;
using System.Collections;
using System.IO;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public enum ETwoDimensionContents //   in order of modes of TwoDimensionMode.txt
    { NOTE, SAND_DRAW, NEON_DRAW, FRUIT_PRINT, SAND_PRINT, }

    public class TwoDimensionAdmin : MonoBehaviour
    {
        #region
        public enum EState
        { NONE, PLAYING, FADE_IN, FADE_OUT, CLOSING, READY, NEXT_CONTENTS }

        /// <summary>
        /// 테스트 용 플레이 타임
        /// </summary>
        public bool test;
        /// <summary>
        /// 씬 내 배경 로고 오브젝트 참조
        /// </summary>
        public GameObject backPanel;

        /// <summary>
        /// 2D drawing 프리팹 경로
        /// Drawing2D_*** 접두사 사용
        /// </summary>
        protected virtual string prefabPath
        {
            get
            { return "TwoDimensionContents/Contents/Prefabs/"; }
        }

        public virtual string backgroundPath
        {
            get
            { return "TwoDimensionContents/Contents/Background/"; }
        }

        private UIPanel _cPanel;
        /// <summary>
        /// NGUI asset을 동적 생성할 때 할당 할 부모 객체
        /// </summary>
        protected UIPanel cPanel
        {
            get
            {
                if (_cPanel == null)
                { _cPanel = this.gameObject.GetComponent<UIPanel>(); }
                return _cPanel;
            }
        }

        private EState mState;
        /// <summary>
        /// 현재 2D 콘텐츠 모드 상태 (생성 -> 다른 콘텐츠로 전이)
        /// </summary>
        public EState currentState
        {
            set
            {
                if (mState != value)
                {
                    switch (value)
                    {
                        case EState.FADE_IN:
                            cCurrentContents.gameObject.SetActive(true);
                            StartCoroutine(FadeIn());
                            break;
                        case EState.CLOSING:
                            //mState = EState.FADE_OUT;
                            break;
                        case EState.FADE_OUT: break;
                        case EState.PLAYING: break;
                        case EState.READY:
                            if (bgm != null)
                                bgm.Stop();
                            Debug.Log(" current state == Ready -> iCurrentIndex :  " + (BigboardContentMode)inOrderOfAscendingIndex[iCurrentIndex]);
                            // 현재 실행 할 콘텐츠를 메모리 풀에서 검색, 없으면 생성함
                            cCurrentContents = cContentsObject.getObject(inOrderOfAscendingIndex[iCurrentIndex]);
                            // 현재 콘텐츠가 등록 되어 있는 콘텐츠가 아니면
                            if (cCurrentContents == null)
                            {
                                //다음 콘텐츠
                                ++iCurrentIndex;
                                int cnt_check = 0;
                                for (int idx = iCurrentIndex; cnt_check < (iMaxContents - 1); ++cnt_check)
                                {
                                    cCurrentContents = cContentsObject.getObject(inOrderOfAscendingIndex[idx]);
                                    if (cCurrentContents != null) break;
                                    else
                                    {
                                        ++idx;
                                        idx %= iMaxContents;
                                    }
                                }
                                if (cCurrentContents == null) { Debug.LogError("error Exception, contents none"); }
                            }

                            _fCurrentTime = 0;
                            break;

                        case EState.NEXT_CONTENTS: //다음 콘텐츠로 전이
                            bSelectMenu = true;
                            if (!bNext) bNext = true;
                            break;
                    }
                    mState = value;
                }
            }

            get
            { return mState; }

        }

        private UIPanel _cCurtainPanel;
        public UIPanel cCurtainPanel
        {
            get
            {
                if (_cCurtainPanel == null)
                {
                    _cCurtainPanel = NGUITools.AddChild<UIPanel>(cPanel.cachedGameObject);
                    _cCurtainPanel.depth = cPanel.depth + 6;
                }
                return _cCurtainPanel;
            }
        }

        protected TwoDimensionTable _cModeFile;
        protected virtual TwoDimensionTable cModeFile  //using 2D contents table list
        {
            get
            {
                if (_cModeFile == null)
                { _cModeFile = new TwoDimensionTable(); }
                return _cModeFile;
            }
        }


        private int _iMaxContents;
        /// <summary>
        /// 2D Drawing  Contents 수
        /// </summary>
        protected int iMaxContents
        {
            get
            {
                if (_iMaxContents == 0)
                {
                    _iMaxContents = cModeFile.TableCount();
                    if (_iMaxContents == 0)
                    { cModeFile.LoadTable(); }
                }
                return _iMaxContents;
            }
        }

        private int _iCurrentIndex;
        /// <summary>
        /// 현재 재생중인 inOrderOfAscendingIndex 배열의 index
        /// </summary>
        protected int iCurrentIndex
        {
            get { return _iCurrentIndex; }
            set { _iCurrentIndex = value % cModeFile.TableCount(); }
        }

        /// <summary>
        /// 2D 콘텐츠 컬렉션
        /// </summary>
        public CObjectListToDic<int, TwoDimensionBase> contents //manager of 2Dcontent prefabs
        {
            get
            {
                if (cContentsObject == null) LoadContentsObject();
                return cContentsObject;
            }
        }
        /// <summary>
        /// 콘텐츠 당 실행 될 시간
        /// </summary>
        protected float _fPlayTime;
        /// <summary>
        /// 현재 콘텐츠 실행 하고 있는  시간
        /// </summary>
        private float _fCurrentTime;

        /// <summary>
        /// 2D sequence index , TwoDimensionBase 몌모리 풀
        /// </summary>
        private CObjectListToDic<int, TwoDimensionBase> cContentsObject;

        /// <summary>
        /// 현재 동작 중인 2D 콘텐츠
        /// </summary>
        private TwoDimensionBase cCurrentContents;

        public Texture2D _imgBlack;
        /// <summary>
        /// 검은 이미지 텍스쳐
        /// </summary>
        public Texture2D imgBlack
        {
            get
            {
                if (_imgBlack == null)
                { _imgBlack = Resources.Load(Path.Combine(backgroundPath, "imgBlackWindow").Replace("\\", "/")) as Texture2D; }
                return _imgBlack;
            }
        }

        UITexture _imgCurtain;
        /// <summary>
        /// 암막 텍스쳐
        /// </summary>
        UITexture imgCurtain
        {
            get
            {
                if (_imgCurtain == null)
                {
                    _imgCurtain = NGUITools.AddChild<UITexture>(cCurtainPanel.cachedGameObject) as UITexture;
                    _imgCurtain.mainTexture = imgBlack;
                    _imgCurtain.SetAnchor(cCurtainPanel.cachedTransform);
                    _imgCurtain.cachedGameObject.SetActive(true);
                    Vector3 _size = _imgCurtain.cachedTransform.localScale;
                    _size.y += _size.y * 0.05f;
                    _size.x += _size.x * 0.05f;
                    _imgCurtain.cachedTransform.localScale = _size;

                }
                return _imgCurtain;
            }
        }

        AudioSource bgm
        { get { return GetComponent<AudioSource>(); } }

        /// <summary>
        /// 일전 시간 후 다음 콘텐츠 자동 재생
        /// </summary>
        [HideInInspector]
        public bool autoPlay; //select config

        /// <summary>
        /// 순차로 재생 할 콘텐츠 index 배열
        /// 콘텐츠 매니저에서 2D 콘텐츠 재생 순서를 순차로 저장 및 재생
        /// </summary>
        int[] inOrderOfAscendingIndex;

        protected bool bActiveCurtain;

        /// <summary>
        /// 현재 콘텐츠 재생 종료 확인 변수
        /// </summary>
        protected bool bComplete;
        /// <summary>
        /// 이전 콘텐츠 종료 후 다음 콘텐츠로 전환을 확인 하는 변수
        /// </summary>
        bool bNext;
        /// <summary>
        /// 특정 콘텐츠 선택 확인 변수
        /// </summary>
        bool bSelectMenu;

        [SerializeField]
        UnityEngine.UI.Button revert;
        #endregion
        protected virtual void Awake()
        {
        }


        protected virtual void OnEnable()
        {
            bActiveCurtain = false;
            bComplete = false;
            Reset();
            currentState = EState.NONE;
            StartCoroutine(FadeIn());
            autoPlay = SettingsManager.enablesAutoPlay2D;

            if (backPanel != null) backPanel.SetActive(true);


#if UNITY_EDITOR

            StartCoroutine(TestLogInProcess());
#else
        InitContentList();
#endif
        }

        protected virtual void OnDisable()
        {
            // Stop all coroutines
            StopCoroutine("LetsPlay");
            StopCoroutine("FadeIn");
            StopCoroutine("FadeOut");
        }

        public void Reset()
        {
            // Load objects
            LoadContentsObject();

            // If content already exists, disable it
            if (cCurrentContents != null)
                cCurrentContents.gameObject.SetActive(false);
            cCurrentContents = null;

            // Reset content index
            _iCurrentIndex = 0;

            // Enables curtain and curtain panel.
            imgCurtain.alpha = 1.0f;
            cCurtainPanel.gameObject.SetActive(true);
        }

        /// <summary>
        /// 다음 콘텐츠 활성화
        /// </summary>
        /// <param name="_contentSeqNo">현재 재생 할 콘텐츠 BigboardContentMode.Drawing2D_***</param>
        public void NextContent(int _contentSeqNo)
        {
            revert?.gameObject.SetActive(true);
            Debug.Log(" Content Click : " + (BigboardContentMode)_contentSeqNo);
            
            if (backPanel != null && backPanel.activeInHierarchy) backPanel.SetActive(false);

            if (autoPlay)
            {
                float minutes = test ? 0.14f : SettingsManager.autoPlayMinute2D;
                _fPlayTime = minutes * 60f; //_fPlayTime = a minute * 60 seconds
            }

            if (currentState == EState.NONE || inOrderOfAscendingIndex[iCurrentIndex] != _contentSeqNo) //재생 할 콘텐츠 seq 번호 와 현재 재생 중인 콘텐츠가 다르면 다음 콘텐츠 재생
            {
                if (currentState == EState.PLAYING) //현재 콘텐츠 재생 중
                {
                    iCurrentIndex = ConvertSeqNoToIndex(_contentSeqNo);
                    currentState = EState.NEXT_CONTENTS;
                }
                else
                {
                    if (currentState == EState.NONE) // 2D 메뉴 에서 처음 콘텐츠를 선택할 경우
                    {
                        iCurrentIndex = ConvertSeqNoToIndex(_contentSeqNo);
                        StartCoroutine(LetsPlay(_fPlayTime));
                    }
                }
            }
            else //현재 콘텐츠 계속 재생
            { Debug.Log("Current state : " + currentState); }
        }

        /// <summary>
        /// sequence no이 저장 되어 있는 inOrderOfAscendingIndex의 배열 index를 반환
        /// </summary>
        /// <param name="_contentSeqNo"></param>
        /// <returns></returns>
        int ConvertSeqNoToIndex(int _contentSeqNo)
        {
            int index = 0;

            for (; index < inOrderOfAscendingIndex.Length; ++index)
            {
                if (inOrderOfAscendingIndex[index] == _contentSeqNo)
                { return index; }
            }

            return 0;
        }

        /// <summary>
        /// 콘텐츠 객체 메모리풀 초기화
        /// </summary>
        private void LoadContentsObject()
        {
            if (cContentsObject == null)
            {
                cContentsObject = new CObjectListToDic<int, TwoDimensionBase>(
                    (int iID) =>
                    {
                    // id에 해당하는 TwoDimensionMode 객체 반화
                    TwoDimensionMode _cMode = cModeFile.GetTwoDimensionMode(iID);
                        Debug.Log("TwoDimensionAdmin.LoadContentsObject ID : " + " (int) : " + iID + " , " + (BigboardContentMode)iID);
                        if (_cMode == null) Debug.LogError("Resource Load Fail in TwoDAdmin.LoadContentsObject()");
                        GameObject _cContents = null;

                        if (!string.IsNullOrEmpty(_cMode.szMode))
                        {
                            _cContents = Resources.Load(Path.Combine(prefabPath, _cMode.szMode).Replace("\\", "/")) as GameObject;
                            _cContents = NGUITools.AddChild(cPanel.cachedGameObject, _cContents);

                            if (_cContents == null)
                            {
                                Debug.Log("contents path : " + prefabPath);
                                Debug.Log("contents filename : " + _cMode.szMode);
                                Debug.LogError("contents null");
                            }
                        //_cContents = Instantiate(_cContents) as GameObject;
                        _cContents.SetActive(false);

                            return (TwoDimensionBase)_cContents.GetComponent<TwoDimensionBase>();
                        }

                        return null;
                    },
                    (TwoDimensionBase _cMode) =>
                    {
                        return !_cMode.gameObject.activeInHierarchy;
                    }
                );
            }
        }

        /// <summary>
        /// 콘텐츠 재생 trigger 함수
        /// </summary>
        /// <param name="_time"></param>
        /// <returns></returns>
        protected IEnumerator LetsPlay(float _time)
        {
            currentState = EState.READY;
            currentState = EState.FADE_IN;

            /** Start Content */
            OnContentStart(cCurrentContents);

            do
            {
                //각 콘텐츠 별 시작 이벤트 호출 함수
                bComplete = cCurrentContents.PlayStart();
                yield return new WaitForEndOfFrame();
            } while (!bComplete || bActiveCurtain);

            if (bNext) bNext = false;

            /** Playing */
            currentState = EState.PLAYING;
            OnContentPlay(cCurrentContents);

            do
            {
                //콘텐츠 실제 체험 단계
                bComplete = cCurrentContents.Play();

                if (autoPlay)
                {
                    _fCurrentTime += Time.deltaTime;
                    bNext = (_time <= _fCurrentTime);
                }

                yield return new WaitForEndOfFrame();
            } while (!bNext);
            /** End Content */
            OnContentEnd(cCurrentContents);

            do
            {
                // 콘텐츠 종료 이벤트 단계
                bComplete = cCurrentContents.PlayEnd();
                yield return new WaitForEndOfFrame();
            } while (!bComplete);

            currentState = EState.FADE_OUT;
            bActiveCurtain = true;
            StartCoroutine(FadeOut());

            while (bActiveCurtain)
            { yield return new WaitForFixedUpdate(); }

            cCurrentContents.gameObject.SetActive(false);

            if (autoPlay)
            {
                if (!bSelectMenu)
                { iCurrentIndex++; }
                else
                { bSelectMenu = false; }
            }
            yield return new WaitForSeconds(1f);
            StartCoroutine(LetsPlay(_time));
        }

        IEnumerator FadeIn()
        {
            bActiveCurtain = true;
            cCurtainPanel.cachedGameObject.SetActive(true);
            if (imgCurtain.alpha != 1f) imgCurtain.alpha = 1f;
            do
            {
                imgCurtain.alpha -= Time.deltaTime * 3f;
                bActiveCurtain = imgCurtain.alpha > 0 ? true : false;
                yield return new WaitForEndOfFrame();
            }
            while (bActiveCurtain);
            cCurtainPanel.cachedGameObject.SetActive(false);
        }

        IEnumerator FadeOut()
        {
            bActiveCurtain = true;
            cCurtainPanel.cachedGameObject.SetActive(true);
            yield return new WaitForEndOfFrame();

            do
            {
                imgCurtain.alpha += Time.deltaTime * 3f;
                bActiveCurtain = imgCurtain.alpha < 1f ? true : false;
                yield return new WaitForEndOfFrame();
            }
            while (bActiveCurtain);

            imgCurtain.alpha = 1f;
        }

        public void ReturnToMainMenu()
        {
            StartCoroutine(ReturnToMenuProcess());
        }

        IEnumerator ReturnToMenuProcess()
        {
            Coroutine fadeOut = StartCoroutine(FadeOut());
            do
            {
                yield return new WaitForEndOfFrame();
            } while (imgCurtain.alpha != 1f);
            if (cCurrentContents != null)
                cCurrentContents.gameObject.SetActive(false);
            //cContentsObject.Destroy
            //Application.LoadLevel("BigBoardMainMenu");
            
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("BigBoardMainMenu");
        }

        public void Next()
        {
            bNext = true;
        }

        /// <summary>
        /// This method will be called when current content is being started.
        /// </summary>
        /// <param name="currentContent">TwoDimensionBase content.</param>
        public virtual void OnContentStart(TwoDimensionBase currentContent)
        {

        }

        /// <summary>
        /// This method will be called when current content starts playing.
        /// </summary>
        /// <param name="currentContent">TwoDimensionBase content.</param>
        public virtual void OnContentPlay(TwoDimensionBase currentContent)
        {

        }

        /// <summary>
        /// This method will be called when current content is being ended.
        /// </summary>
        /// <param name="currentContent">TwoDimensionBase content.</param>
        public virtual void OnContentEnd(TwoDimensionBase currentContent)
        {

        }

        /// <summary>
        /// 2D 드로잉 모드에 체험 가능 한 콘텐츠 등록
        /// </summary>
        void InitContentList()
        {
            cModeFile.LoadTable();
            inOrderOfAscendingIndex = new int[cModeFile.TableCount()];

            int cnt = 0;
            foreach (int value in cModeFile.keys)
            {
                //inOrderOfAscendingIndex[cnt] =  BigboardContentMode.Drawing2D_***
                inOrderOfAscendingIndex[cnt] = value;
                Debug.Log("TwoDimension Adimin Initialize Index: " + inOrderOfAscendingIndex[cnt] + " : " + (BigboardContentMode)inOrderOfAscendingIndex[cnt]);
                ++cnt;
            }
        }

#if UNITY_EDITOR
        IEnumerator TestLogInProcess()
        {

            bool bLogin = false;
            /*
            if (BigboardServerDataManager.GetListAllContentsStoreItemInfo().Count == 0)
            {
                test = true;
                BigboardServerDataManager.TestLogin(
                    (bLoginSuccess) =>
                    { bLogin = bLoginSuccess; }
                    );

            }else*/
            { bLogin = true; }

            do
            {
                Debug.Log("Admin try login");
                yield return new WaitForEndOfFrame();
            } while (!bLogin);

            InitContentList();
        }
#endif

    }
}