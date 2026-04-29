using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public enum FruitType
    {
        APPLE, MELON, ORANGE, STRAWBERRY, WATERMELON, NUM
    }

    /// <summary>
    /// 과일 도장 찍기 메인 클래스 및 과일 index를 제어 하는 클래스
    /// </summary>
    public class TwoDimensionFruitPrint : TwoDimensionBase
    {
        static float _elapsedTime;
        /// <summary>
        /// 과일 이미지 index
        /// </summary>
        static int fruitIndex;
        static FruitType _eFruitType;

        /// <summary>
        /// 현재 선택 된 과일 종류
        /// </summary>
        public static FruitType eFruitType
        {
            get
            { return _eFruitType; }
            set
            { _eFruitType = value; }
        }

        public TwoDimensionPanel cTwoDFruitPrintPanel
        {
            get
            {
                if (!_cTwoDFruitPrintPanel)
                {
                    _cTwoDFruitPrintPanel = FindObjectOfType<TwoDimensionPanel>();
                    if (!_cTwoDFruitPrintPanel) gameObject.AddComponent<TwoDimensionPanel>();
                }
                return _cTwoDFruitPrintPanel;
            }
        }

        public TwoDimensionInteractionPanel cTwoInteractionPanel
        {
            get
            {
                if (!_cTwoInteractionPanel)
                {
                    _cTwoInteractionPanel = FindObjectOfType<TwoDimensionInteractionPanel>();
                    if (!_cTwoInteractionPanel) gameObject.AddComponent<TwoDimensionInteractionPanel>();
                }
                return _cTwoInteractionPanel;
            }
        }


        public TwoDimensionPanel _cTwoDFruitPrintPanel;
        public TwoDimensionInteractionPanel _cTwoInteractionPanel;

        IEvent[] iEvents;

        void Awake()
        {
            iEvents = new IEvent[2];

            iEvents[0] = (IEvent)cTwoDFruitPrintPanel;
            iEvents[1] = (IEvent)cTwoInteractionPanel;
        }

        void OnEnable()
        {
            _elapsedTime = 0;
            fruitIndex = 0;
            eFruitType = FruitType.APPLE;
        }

        public override bool PlayStart()
        {
            return true;
        }

        /// <summary>
        /// 과일 index 제어 
        /// </summary>
        /// <returns></returns>
        public override bool Play()
        {

            _elapsedTime += Time.deltaTime;
            if (_elapsedTime > 10f)
            {
                _elapsedTime = 0;
                ++fruitIndex;
                fruitIndex %= (int)FruitType.NUM;
                eFruitType = (FruitType)fruitIndex;

                for (int i = 0; i < iEvents.Length; ++i)
                {
                    if (iEvents[i] == null) continue;
                    iEvents[i].StateEventActivates();
                }
            }

            return false;
        }

        public override bool PlayEnd()
        {
            return true;
        }

        // 테스트 용 함수, TwiDimensionAdmin 클래스를 통해 생성 된것이 아닌 테스트 용으로 사용 할 경우 아래 의 함수를 호출 해야함
        // 빌드 시에는 주석 처리 해야함
#if UNITY_EDITOR
        void Update()
        {
            Play();
        }
#endif
    }
}