using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ML.PlaywallKids.Common;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// Utility for capturing and printing a dragon and free drawing object.
    /// </summary>
    public class DragonPrinter : MonoBehaviour
    {
        #region Public variables
        public Camera previewCamera;
        public Camera outputCamera;
        public RenderTexture previewTexture;
        public Vector2 previewSize = new Vector2(1024, 1536);
        public Vector2 outputSize = new Vector2(1024, 1536);
        public Texture2D outputTexture;
        public RawImage backImageUI, previewImageUI;
        public Texture2D backMapo;
        public Text dateText;

        public GameObject target;
        #endregion

        #region Properties
        public bool isCapturing { get; private set; }
        public bool isPrinting { get; private set; }
        public bool printResult { get; private set; }
        #endregion

        #region Private variables
        private int _userId;
        private BoneObject _boneObject;

        // key : userId
        // value : recently printed object's name
        private static Dictionary<int, string> _printStatusDict = new Dictionary<int, string>();
        #endregion

        #region Unity methods
        public void OnDestroy()
        {
            if (previewTexture != null)
            {
                previewTexture.Release();
                previewTexture = null;
            }
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
        public void Set(int userId)
        {
            GameObject current = SimpleInstantiatedTemplateControl.GetCurrentTemplate(userId);
            Set(userId, current);
        }

        public void Set(int userId, GameObject another)
        {
            // duplication check
            if (target != null && another != null && target.name.Equals(another.name))
                return;

            if (another != null)
            {
                GameObject go = Instantiate(another);
                go.name = another.name;
                _Set(go);
            }
            else
            {
                _Set(null);
            }

            _userId = userId;
            if (_printStatusDict.ContainsKey(_userId))
                _printStatusDict.Remove(_userId);
            outputTexture = null;
            StartCoroutine(_Capture());
        }

        private void _Set(GameObject newTarget)
        {
            if (target != null)
            {
                // 동일한 탬플릿은 기존에 캡처한 이미지를 재사용.
                if (newTarget != null && target.name.Equals(newTarget.name))
                    return;
                Destroy(target);
                target = null;
                _boneObject = null;
            }

            if (newTarget != null)
            {
                target = newTarget;
                target.transform.parent = transform;
                target.transform.localPosition = Vector3.zero;
                target.transform.localRotation = Quaternion.Euler(0, 180, 0);
                target.transform.localScale = Vector3.one;
                int printingLayer = LayerMask.NameToLayer("Printing");
                if (printingLayer != -1)
                    target.gameObject.SetLayerRecursively(printingLayer);

                // 탬플릿 외곽선 추가
                Template3D template = target.GetComponent<Template3D>();
                if (template != null)
                    template.ShowOutline();

                // 드래곤 효과 지우기
                DragonEffect dragonEffect = target.GetComponent<DragonEffect>();
                if (dragonEffect != null)
                    Destroy(dragonEffect);

                // 애니메이션 컨트롤러 비활성화
                DragonAnimationControl animationControl = target.GetComponent<DragonAnimationControl>();
                if (animationControl != null)
                {
                    animationControl.movesAlongPath = false;
                    animationControl.usesNavMesh = false;
                    animationControl.enabled = false;
                }

                // bone 오브젝트 가져오기 (카메라 위치 잡을때 사용)
                BoneObject boneObject = target.GetComponent<BoneObject>();
                _boneObject = boneObject;
            }
        }
        #endregion

        #region Capturing
        public void UpdateCamera()
        {
            if (_boneObject != null)
            {
                Transform tf1 = _boneObject.GetBone(BoneObject.kFootstepBone);
                Transform tf2 = _boneObject.GetBone(BoneObject.kHeadNubBone);
                float headBoneScale = _boneObject.GetBoneScale(BoneObject.kHeadBone);

                if (tf1 != null && tf2 != null)
                {
                    // 발 중심과 정수리 지점의 가운데 위치로 잡음, 모자 액세서리를 고려하여 추가 오프셋을 더함.
                    Vector3 pos = Vector3.zero;
                    pos.x = (tf1.position.x + tf2.position.x) * 0.5f;
                    pos.y = (tf1.position.y + tf2.position.y) * 0.5f;
                    pos.z = 0;
                    Vector3 add = (tf2.position - tf1.position).normalized;
                    if (_boneObject.GetAccessory(BoneObject.kHeadBone))
                        add *= headBoneScale * 0.2f;
                    else
                        add *= headBoneScale * 0.05f;
                    pos += add;
                    previewCamera.transform.position = pos;
                    previewCamera.orthographicSize = ((tf2.position.y + add.y) - tf1.position.y) * 0.7f;
                }
                else if (_boneObject is FreeDrawingObjectBone)
                {
                    // 하드코딩으로 알맞은 카메라 포즈 지정
                    switch (((FreeDrawingObjectBone)_boneObject).objectType)
                    {
                        case FreeDrawingObjectType.Car:
                            target.transform.localEulerAngles = new Vector3(10, -160, 0);
                            target.transform.localPosition = Vector3.down;
                            previewCamera.orthographicSize = 2.25f;
                            break;
                        case FreeDrawingObjectType.Robot:
                            target.transform.localEulerAngles = new Vector3(10, -160, 0);
                            target.transform.localPosition = Vector3.down;
                            previewCamera.orthographicSize = 1.75f;
                            break;
                        case FreeDrawingObjectType.Airplane:
                            target.transform.localEulerAngles = new Vector3(10, -160, 0);
                            target.transform.localPosition = Vector3.down * 0.5f;
                            previewCamera.orthographicSize = 1.666f;
                            break;
                    }
                    previewCamera.transform.localPosition = Vector3.zero;
                }
                else
                {
                    // bone 정보를 알 수 없는 경우 원점 위치로 잡음.
                    previewCamera.transform.localPosition = Vector3.zero;
                    previewCamera.orthographicSize = 1.25f;
                }
            }
        }

        IEnumerator _Capture()
        {
            isCapturing = true;

            Animator anim = target.GetComponent<Animator>();
            if (anim != null)
            {
                // 렌더링하지 않은 상태에서도 포즈를 제대로 취해야 하므로
                anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;

                // 캐릭터에 맞는 포즈 취하기 (하드코드)
                if (anim.HasState(0, Animator.StringToHash("Pico_Hi_Nor")))         // Pico
                    anim.Play("Pico_Hi_Nor", 0, 0.66f);
                else if (anim.HasState(0, Animator.StringToHash("Arrow_Hi_Host")))  // Arrow
                    anim.Play("Arrow_Hi_Host", 0, 0.66f);
                else if (anim.HasState(0, Animator.StringToHash("Fun_Act_01")))     // Hansen
                    anim.Play("Fun_Act_01", 0, 0.66f);
                else if (anim.HasState(0, Animator.StringToHash("Cougar_Hi_Host"))) // Cougar
                    anim.Play("Cougar_Hi_Host", 0, 0.66f);
                else if (anim.HasState(0, Animator.StringToHash("Robot_HeadSpin"))) // Robot
                    anim.Play("Robot_HeadSpin", 0, 0.9f);
                anim.speed = 0;
            }

            // 프리드로잉 오브젝트의 경우 눈이 감는 모습이 찍히지 않도록 관련 애니메이션을 모두 꺼야 한다.
            if (_boneObject != null)
            {
                GameObject eyeL = _boneObject.GetAccessory(FreeDrawingObjectBone.kEyeLBone);
                GameObject eyeR = _boneObject.GetAccessory(FreeDrawingObjectBone.kEyeRBone);
                if (eyeL != null)
                {
                    Animator eyeAnim = eyeL.GetComponent<Animator>();
                    if (eyeAnim != null)
                    {
                        eyeAnim.Play(0, -1, 0);
                        eyeAnim.speed = 0;
                    }
                }
                if (eyeL != null)
                {
                    Animator eyeAnim = eyeL.GetComponent<Animator>();
                    if (eyeAnim != null)
                    {
                        eyeAnim.Play(0, -1, 0);
                        eyeAnim.speed = 0;
                    }
                }
            }

            // 애니메이터가 업데이트하도록 다음 프레임까지 대기
            if (anim != null)
                yield return null;

            // bone 위치를 기반으로 카메라 위치 및 영역 조정
            UpdateCamera();

            // 프린트할 이미지 크기
            float aspectRatio = previewSize.x / previewSize.y;
            int previewWidth = Mathf.FloorToInt(previewSize.x);
            int previewHeight = Mathf.FloorToInt(previewSize.y);

            // 미리보기 텍스처 생성
            if (previewTexture != null && (previewTexture.width != previewWidth || previewTexture.height != previewHeight))
            {
                previewTexture.Release();
                previewTexture = null;
            }
            if (previewTexture == null)
                previewTexture = RenderTexture.GetTemporary(previewWidth, previewHeight, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);

            // 렌더링
            previewCamera.targetTexture = previewTexture;
            previewCamera.Render();

            // 정리
            previewCamera.targetTexture = null;
            isCapturing = false;
        }
        #endregion

        #region Printing
        public static bool IsPrinted(int userId, string objName)
        {
            bool printStatus = false;
            if (_printStatusDict.ContainsKey(userId))
            {
                string printedObjectName = _printStatusDict[userId];
                printStatus = printedObjectName.Equals(objName);
            }
            return printStatus;
        }

        public void Print()
        {
            if (outputTexture == null)
            {
                // 인쇄물에 붙이기
                int outputWidth = Mathf.FloorToInt(outputSize.x);
                int outputHeight = Mathf.FloorToInt(outputSize.y);
                RenderTexture outputRT = RenderTexture.GetTemporary(outputWidth, outputHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
                previewImageUI.texture = previewTexture;
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
                outputTexture = new Texture2D(outputWidth, outputHeight, TextureFormat.ARGB32, false);
                outputTexture.ReadPixels(new Rect(0, 0, outputWidth, outputHeight), 0, 0);
                RenderTexture.active = prevActive;
                outputTexture.Apply();
                #region 의심되는 부분
                var savefiles = outputTexture.EncodeToJPG();
                System.IO.File.WriteAllBytes($"{Application.dataPath}/{Random.Range(0, 60000)}.jpg", savefiles);

                #endregion
                // 정리
                RenderTexture.ReleaseTemporary(outputRT);
            }

            byte[] data = outputTexture.GetRawTextureData();
            int width = outputTexture.width;
            int height = outputTexture.height;
            isPrinting = true;
            printResult = false;

            // 별도 thread로 프린터 출력 진행
            Loom.Initialize();
            Loom.RunAsync(() =>
            {
                try
                {
                    PrintImage printImage = new PrintImage(data, width, height);
                    printResult = printImage.Print();
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Failed to print image!");
                    Debug.LogException(e);
                    printResult = false;
                }
                Loom.QueueOnMainThread(_OnPrintEnd);
            });
        }

        private void _OnPrintEnd()
        {
            if (printResult)
            {
                _printStatusDict[_userId] = target ? target.name : "";
            }
            isPrinting = false;
        }
        #endregion
    }
}

public class DragonPrinter : ML.PlaywallKids.DragonPark.DragonPrinter { }