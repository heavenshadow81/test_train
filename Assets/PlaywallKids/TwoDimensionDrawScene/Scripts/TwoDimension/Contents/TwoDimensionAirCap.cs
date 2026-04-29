using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class TwoDimensionAirCap : TwoDimensionBase
    {
        public BubbleWrapBoard boardPrefab;
        public UITexture screenTexture;
        public RenderTexture aircapRenderTexture;
        public Camera mainCam;
        private BubbleWrapBoard _board;

        float _hideTime = 0.0f;

        void Awake()
        {
            if (aircapRenderTexture == null)
            {
                aircapRenderTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
                mainCam.targetTexture = aircapRenderTexture;
                mainCam.clearFlags = CameraClearFlags.SolidColor;
                mainCam.backgroundColor = Color.clear;
            }

            if (aircapRenderTexture != null)
                screenTexture.mainTexture = aircapRenderTexture;
        }

        public override bool PlayStart()
        {
            if (_board == null)
            {
                _board = FindObjectOfType<BubbleWrapBoard>();
                if (_board == null)
                {
                    _board = (BubbleWrapBoard)Instantiate(boardPrefab, new Vector3(-100.0f, -200.0f, 0.0f), Quaternion.identity);
                    _board.transform.localScale = Vector3.one;
                }
            }
            _hideTime = 0.0f;

            return true;
        }

        public override bool Play()
        {
            if (!_board.gameObject.activeInHierarchy)
            {
                _board.gameObject.SetActive(true);
            }

            return true;
        }

        public override bool PlayEnd()
        {
            if (_hideTime < 2.0f)
            {
                foreach (var bubble in _board.bubbles)
                {
                    bubble.opacity -= Time.deltaTime * 0.5f;
                }

                _hideTime += Time.deltaTime;

                return false;
            }

            _board.gameObject.SetActive(false);

            return true;
        }
    }
}