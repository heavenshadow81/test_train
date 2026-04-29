#define UNUSING_FRUIT_PROTRAIT

using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// 괴일 이미지 배열 관리 및 현재 과일 이미지 교체하는 클래스
    /// </summary>
    public class TwoDimensionInteractionFruitPanel : TwoDimensionInteractionPanel
    {
        #region
        public Texture2D[] _imgFruits;
        /// <summary>
        /// 과일 생상 개수
        /// </summary>
        [Range(8, 12)]
        public int numFruit = 14;
        #endregion

        #region PUBLIC_PROPERTIES
        /// <summary>
        /// 과일 텍스쳐 배열 참조 및 로드
        /// </summary>
        public Texture2D[] ImgFruits
        {
            get
            {
                if (_imgFruits == null || _imgFruits.Length != szFileNames.Length)
                {
                    _imgFruits = null;
                    _imgFruits = new Texture2D[szFileNames.Length];
                    for (int i = 0; i < szFileNames.Length; ++i)
                    { _imgFruits[i] = Resources.Load(szFilePath + szFileNames[i]) as Texture2D; }
                }
                return _imgFruits;
            }
        }

        /// <summary>
        /// 상단 바에 과일 객체 - 현재는 비활성화 됨
        /// </summary>
        public UIFruitTexture ImgFruitPotrait
        {
            get
            {
                _imgFruit = NGUITools.AddChild<UIFruitTexture>(cPanel.gameObject);
                // _imgFruit.cachedTransform.localPosition =  new Vector3(0, (Util.fHeight/2 * 0.75f) - (_imgFruit.height/2), 0f);

                return _imgFruit;
            }
        }

        /// <summary>
        /// 화면 상단에 과일 객체들 - 현재는 비활성화 됨
        /// </summary>
        public UIFruitTexture[] ImgFruitArr
        {
            get
            {
                if (_imgFruitArr == null)
                {
                    _imgFruitArr = new UIFruitTexture[numFruit];
                    float fWidth = UtilityScript.width / _imgFruitArr.Length;
                    // 과일들 위치 설정
                    for (int i = 0; i < _imgFruitArr.Length; ++i)
                    {
                        _imgFruitArr[i] = NGUITools.AddChild<UIFruitTexture>(cPanel.gameObject);
                        float n = (fWidth * (i / 2));
                        float _x = (n + fWidth / 2) * ((i % 2 == 0) ? -1 : 1);

                        _imgFruitArr[i].cachedTransform.localPosition = new Vector3(_x, (UtilityScript.height / 2 * 0.75f) - (_imgFruitArr[i].height / 2), 0f);
                        _imgFruitArr[i].name = "fruit_" + i.ToString();
                    }
                }
                return _imgFruitArr;
            }
        }
        #endregion PUBLIC_PROPERTIES

        #region PROPERTY_VARAIABLSE
        private UIFruitTexture _imgFruit;
        private UIFruitTexture[] _imgFruitArr;
        #endregion PROPERTY_VARAIABLSE

        string[] szFileNames = new string[] { "imgApple", "imgMelon", "imgOrange", "imgStrawberry", "imgWatermelon" };

        const string szFilePath = "TwoDimensionContents/Contents/Objects/";

        void Awake()
        {
            numFruit = 14;
            _imgFruitArr = null;
        }

        void OnEnable()
        {
            UIFruitTexture.curImage = ImgFruits[(int)TwoDimensionFruitPrint.eFruitType];

            for (int i = 0; i < ImgFruitArr.Length; ++i)
            {
                if (ImgFruitArr[i])
                { ImgFruitArr[i].currentState = EPotraitState.INITIALIZE; }
                else
                {//null 이면 새로 생성
                    ImgFruitArr[i] = ImgFruitPotrait;
                    float fWidth = UtilityScript.width / ImgFruitArr.Length;
                    float fHeight = ImgFruitArr.Length / 2;
                    ImgFruitArr[i].cachedTransform.localPosition = GetPosition(fWidth, fHeight, i);
                    ImgFruitArr[i].currentState = EPotraitState.INITIALIZE;
                }
            }

            for (int i = 0; i < ImgFruitArr.Length; ++i)
            { ImgFruitArr[i].currentState = EPotraitState.APPEAR; }

            //ImgFruitPotrait.currentState = EPotraitState.INITIALIZE;
            //ImgFruitPotrait.currentState = EPotraitState.APPEAR;
        }
        void Disable()
        {
            for (int i = 0; i < ImgFruitArr.Length; ++i)
            { ImgFruitArr[i].currentState = EPotraitState.DISAPPEAR; }

            for (int i = 0; i < ImgFruitArr.Length; ++i)
            { ImgFruitArr[i].currentState = EPotraitState.NONE; }

            //ImgFruitPotrait.currentState = EPotraitState.DISAPPEAR;
        }

        Vector3 GetPosition(float _fWith, float _fHeight, int _idx)
        {
            float n = (_fWith * (_idx / 2));
            float _x = (n + _fWith / 2) * ((_idx % 2 == 0) ? -1 : 1);

            return new Vector3(_x, (UtilityScript.height / 2 * 0.75f) - (_fHeight / 2), 0f);
        }

        public override bool StateInPlay() { return true; }
        public override bool StateEventReady() { return true; }
        public override bool StateEventActivates()
        {
            int idx = (int)TwoDimensionFruitPrint.eFruitType;

            // 과일 이미지 교체하는 위치
            UIFruitTexture.nextImage = ImgFruits[idx];

            for (int i = 0; i < ImgFruitArr.Length; ++i)
            { ImgFruitArr[i].cachedTransform.localRotation = ImgFruitArr[0].cachedTransform.localRotation; }

            for (int i = 0; i < ImgFruitArr.Length; ++i)
            { ImgFruitArr[i].currentState = EPotraitState.CHANGE; }

            //ImgFruitPotrait.currentState = EPotraitState.CHANGE;

            return true;
        }
    }

    public enum EPotraitState
    { NONE, INITIALIZE, IDLE, CHANGE, APPEAR, DISAPPEAR }


    /// <summary>
    /// 사용 안함
    /// </summary>
    public class UIFruitTexture : UITexture
    {
        public bool bUpdate;
        bool bLeft;
        bool bIdle;
        Vector3 vRotation;
        public static Texture2D curImage;
        public static Texture2D nextImage;

        EPotraitState _state;
        public EPotraitState currentState
        {
            set
            {
                if (_state != value && !bUpdate)
                {
                    _state = value;
                    switch (value)
                    {
                        case EPotraitState.APPEAR:
                            bUpdate = true;
                            cachedGameObject.SetActive(true);
                            StartCoroutine(AppearProcess());
                            break;
                        case EPotraitState.DISAPPEAR:
                            bUpdate = true;
                            StartCoroutine(DisAppearProcess());
                            cachedGameObject.SetActive(false);
                            break;
                        case EPotraitState.IDLE:

                            if (!bIdle)
                            {
                                bIdle = true;
                                StartCoroutine(IdleProcess());
                            }
                            break;
                        case EPotraitState.INITIALIZE:
                            cachedGameObject.SetActive(false);
                            break;
                        case EPotraitState.CHANGE:
                            bUpdate = true;
                            StartCoroutine(ChangeProcess());
                            break;
                    }
                }
            }
        }

        void Initialize()
        {
            _state = EPotraitState.NONE;
            cachedTransform.localScale = Vector3.zero;
            vRotation = Vector3.zero;
            bUpdate = false;
            bIdle = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Initialize();
        }

#if UNUSING_FRUIT_PROTRAIT
        IEnumerator AppearProcess()
        {
            mainTexture = curImage;
            yield return new WaitForSeconds(2f);
            currentState = EPotraitState.IDLE;
        }

        IEnumerator DisAppearProcess()
        {
            yield return new WaitForSeconds(2f);
        }

        /// <summary>
        /// 과일 이미지 교체
        /// </summary>
        /// <returns></returns>
        IEnumerator ChangeProcess()
        {
            yield return new WaitForSeconds(2f);
            curImage = nextImage;
            mainTexture = curImage;
            yield return new WaitForFixedUpdate();
            currentState = EPotraitState.IDLE;
        }

        IEnumerator IdleProcess()
        {
            do
            {
                yield return new WaitForFixedUpdate();
            } while (cachedGameObject.activeInHierarchy);
        }
#else

        IEnumerator AppearProcess()
        {
            mainTexture = curImage;
            yield return new WaitForSeconds(1.5f);
            float w = 2.5f;//mainTexture.width;// *0.2f;//float h = mainTexture.height * 0.2f; 
        
            Vector3 vSize = Vector3.zero;
            vSize.z = 1f;
            float fSize =0;
            do
            {
                cachedTransform.localScale = IncreaseSize(ref fSize, w);
            
                yield return new WaitForFixedUpdate();
            } while (fSize != w);
            vSize.x = fSize - fSize * 0.15f;
            vSize.y = fSize - fSize * 0.15f;
            cachedTransform.localScale = vSize;
            bUpdate = false;
            currentState = EPotraitState.IDLE;
        }

        Vector3 IncreaseSize(ref float _side, float _to)
        {
            Vector3 _size = new Vector3(0, 0, 1f);
            _side += Time.deltaTime * 10f;
            _side = _side < _to ? _side : _to;

            _size.x = _side;
            _size.y = _side;
            return _size;
        }

        IEnumerator DisAppearProcess()
        {
            float fSize = cachedTransform.localScale.x;
            do
            {
                cachedTransform.localScale = DecreaseSize(ref fSize);

                yield return new WaitForFixedUpdate();
            } while (cachedTransform.localScale.x > 0);
        }

        Vector3 DecreaseSize(ref float _side)
        {
            Vector3 _size = new Vector3(0, 0, 1f);

            _side -= (_side/2 + Time.deltaTime * 5f); 
            _side = _side > 0 ? _side : 0;

            _size.x = _side;
            _size.y = _side;
            return _size;

        }

        IEnumerator ChangeProcess()
        {
            float fSize = cachedTransform.localScale.x;
            do
            {
                cachedTransform.localScale = DecreaseSize(ref fSize);
                yield return new WaitForFixedUpdate();
            } while (cachedTransform.localScale.x > 0);
            curImage = nextImage;
            mainTexture = curImage;
            yield return new WaitForFixedUpdate();

            float w = 2.5f;//mainTexture.width;// *0.2f;//float h = mainTexture.height * 0.2f; 

            Vector3 vSize = Vector3.zero;
            vSize.z = 1f;
            fSize = 0;
            do
            {
                cachedTransform.localScale = IncreaseSize(ref fSize, w);
                yield return new WaitForFixedUpdate();
            } while (fSize != w);
            vSize.x = fSize - fSize * 0.15f;
            vSize.y = fSize - fSize * 0.15f;
            cachedTransform.localScale = vSize;
            bUpdate = false;
            currentState = EPotraitState.IDLE;

        }

        IEnumerator IdleProcess()
        {
            do
            {
                if (bLeft)
                {  
                    vRotation.z -= Time.deltaTime * 30f;
                    if (vRotation.z < -20f)
                    {
                        vRotation.z = -20f;
                        bLeft = false;
                    }
                }
                else
                {  
                    vRotation.z += Time.deltaTime * 30f;
                    if (vRotation.z > 20f)
                    {
                        vRotation.z = 20f;
                        bLeft = true;
                    }
                }
                cachedTransform.localRotation = Quaternion.Euler(vRotation);

                yield return new WaitForFixedUpdate();
            } while (cachedGameObject.activeInHierarchy);
        }

#endif
    }
}