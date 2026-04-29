using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// class Canvas 초기 설정 클래스
    /// </summary>
    public class TwoDimensionSandDrawPanel : TwoDimensionPanel
    {


        bool _bEnableCanvas;
        public bool bEnableCanvas
        {
            get
            { return _bEnableCanvas; }

            set
            {
                if (value != _bEnableCanvas)
                {
                    _bEnableCanvas = value;
                    cSketchCanvas.wantsPaint = value;
                }
            }
        }

        /// <summary>
        /// Canvas class 초기화
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            bEnableCanvas = true;
            // 
            cSketchCanvas.brush = GetBrush();

            // 해상도 설정
            cSketchCanvas.textureSize = new Vector2(2048, 1024);
        }

        /// <summary>
        /// canvas 데이터 초기화
        /// </summary>
        void OnEnable()
        {
            cSketchCanvas.ClearCanvas();
        }

        /// <summary>
        /// 콘텐츠 체험 중 호출
        /// </summary>
        /// <returns></returns>
        public override bool StateInPlay()
        {
            if (!bEnableCanvas)
            {
                bEnableCanvas = true;
            }
            return false;
        }

        /// <summary>
        /// 이벤트 시작 시 호출
        /// </summary>
        /// <returns></returns>
        public override bool StateEventReady()
        {
            bEnableCanvas = false;
            return true;
        }

        /// <summary>
        /// 이벤트 중일 때 호출
        /// </summary>
        /// <returns></returns>
        public override bool StateEventActivates()
        {
            return true;
        }

        protected override Brush GetBrush()
        {
            if (cStampBush == null)
            {
                do
                {
                    cStampBush = CustomBrush.brush.SandBrush();
                } while (cStampBush == null);

            }
            return cStampBush;
        }
    }
}