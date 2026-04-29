using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ML.PlaywallKids.Aquarium
{
    using Common;

    /// <summary>
    /// Utility for capturing and printing a fish.
    /// </summary>
    public class AquariumPrinter : MonoBehaviour
    {
        #region Public variables
        public Camera outputCamera;
        public Texture previewTexture;
        public Vector2 previewSize = new Vector2(1024, 1536);
        public Vector2 outputSize = new Vector2(1024, 1536);
        public Texture2D outputTexture;
        public RawImage backImageUI, previewImageUI;
        public Texture2D backMapo;
        public Text dateText;
        #endregion

        #region Properties
        public bool isPrinting { get; private set; }
        public bool printResult { get; private set; }
        #endregion

        #region Private variables
        private int _userId;
        private string _identifier;

        // key : userId
        // value : recently printed object's identifier
        private static Dictionary<int, string> _printStatusDict = new Dictionary<int, string>();
        #endregion

        #region Unity methods
        public void OnDestroy()
        {
            if (outputTexture != null)
            {
                DestroyImmediate(outputTexture);
                outputTexture = null;
            }
        }

        public void Awake()
        {
            if (backImageUI != null)
            {
                if (CommonSettings.dist.Equals("mapo"))
                {
                    backImageUI.texture = backMapo;
                }
            }
        }
        #endregion

        #region Set
        public void Set(int newUserId, string identifier, Texture image)
        {
            _userId = newUserId;
            _identifier = identifier;
            previewTexture = image;
        }
        #endregion

        #region Printing
        public static bool IsPrinted(int userId, string identifier)
        {
            bool printStatus = false;
            if (_printStatusDict.ContainsKey(userId))
            {
                string printedObjectIdentifier = _printStatusDict[userId];
                printStatus = printedObjectIdentifier.Equals(identifier);
            }
            return printStatus;
        }

        public void Print()
        {
            if (previewTexture != null)
            {
                // 텍스처의 비율에 맞춰 UI aspect 조정 
                previewImageUI.texture = previewTexture;
                float previewUiAspect = previewSize.x / previewSize.y;
                float previewTextureAspect = previewTexture.width / (float)previewTexture.height;

                // 크기 조정
                Vector2 sizeDelta = previewImageUI.rectTransform.sizeDelta;
                if (previewUiAspect > previewTextureAspect)
                {
                    sizeDelta.x = previewSize.y * previewTextureAspect;
                    sizeDelta.y = previewSize.y;
                }
                else
                {
                    sizeDelta.x = previewSize.x;
                    sizeDelta.y = previewSize.x / previewTextureAspect;
                }
                previewImageUI.rectTransform.sizeDelta = sizeDelta;

                // 인쇄물에 붙이기
                int outputWidth = Mathf.FloorToInt(outputSize.x);
                int outputHeight = Mathf.FloorToInt(outputSize.y);
                RenderTexture outputRT = RenderTexture.GetTemporary(outputWidth, outputHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
                dateText.text = System.DateTime.Now.ToString("yyyy. M. d.");
                outputCamera.targetTexture = outputRT;
                outputCamera.Render();

                // RenderTexture -> Texture2D
                RenderTexture prevActive = RenderTexture.active;
                RenderTexture.active = outputRT;
                if (outputTexture != null && (outputTexture.width != outputWidth || outputTexture.height != outputHeight))
                {
                    DestroyImmediate(outputTexture);
                    outputTexture = null;
                }
                if (outputTexture == null)
                    outputTexture = new Texture2D(outputWidth, outputHeight, TextureFormat.ARGB32, false);
                outputTexture.ReadPixels(new Rect(0, 0, outputWidth, outputHeight), 0, 0);
                RenderTexture.active = prevActive;
                outputTexture.Apply();

                // 정리
                RenderTexture.ReleaseTemporary(outputRT);

                if (outputTexture != null)
                {
                    byte[] data = outputTexture.GetRawTextureData();
                    int width = outputTexture.width;
                    int height = outputTexture.height;
                    isPrinting = true;
                    printResult = false;

                    // 별도 thread로 프린터 출력 진행
                    Loom.Initialize();
                    Loom.RunAsync(() =>
                    {
                        PrintImage printImage = new PrintImage(data, width, height);
                        printResult = printImage.Print();
                        Loom.QueueOnMainThread(_OnPrintEnd);
                    });
                }
            }
        }

        private void _OnPrintEnd()
        {
            if (printResult)
            {
                _printStatusDict[_userId] = _identifier;
            }
            isPrinting = false;
        }
        #endregion
    }
}