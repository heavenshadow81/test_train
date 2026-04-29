#define USING_PHYSIC3D

using UnityEngine;
using com.Loxwell.File;
using UserType;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// canvs class 에 과일 도장  찍는 클래스
    /// </summary>
    public class TwoDimensionFruitPrintPanel : TwoDimensionPanel
    {
        public AudioClip[] clips; //sndStampClap0~2
        [HideInInspector]
        public new CanvasForStamp cSketchCanvas;
        public UIParticleManager particleMamager;


        string[] szStamps;//= new string[] {FileName.AppleStamp, FileName.MelonStamp, FileName.OrangeStamp, FileName.StrawberryStamp, FileName.TomatoStamp, FileName.WaterMelonStamp  };
        Brush[] brushArr;
        float[] angleArr;
        ColorTable colorTable = new ColorTable();
        Transform cachedTransform;

        AudioSource _audio;
        AudioSource mAudio
        {
            get
            {
                if (_audio == null)
                {
                    _audio = GetComponent<AudioSource>();
                    if (_audio == null)
                    {
                        gameObject.AddComponent<AudioSource>();
                        _audio = gameObject.GetComponent<AudioSource>();
                    }
                }
                return _audio;
            }
        }

        /// <summary>
        /// Brush , Canvas class 초기화
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            cachedTransform = this.transform;
            szStamps = new string[] { FileName.AppleStamp, FileName.MelonStamp, FileName.OrangeStamp, FileName.StrawberryStamp, FileName.WaterMelonStamp };
            brushArr = new Brush[szStamps.Length];

            for (int i = 0; i < szStamps.Length; ++i)
            {
                brushArr[i] = CustomBrush.brush.RandomColorStamp(szStamps[i]);
                brushArr[i].shapeDynamicComponent.enable = true;
                brushArr[i].colorDynamicComponent.enable = false;
                brushArr[i].shapeDynamicComponent.angleControl = AngleControl.Angle;
            }

            angleArr = new float[szStamps.Length];

            base.cSketchCanvas.gameObject.SetActive(false);

            cSketchCanvas = NGUITools.AddChild<CanvasForStamp>(this.gameObject);
            cSketchCanvas.wantsPaint = false;
            cSketchCanvas.name = "FruitSketchbook";
            cSketchCanvas.uiTexture.SetAnchor(cPanel.cachedGameObject);
            cSketchCanvas.brush = GetBrush();
            cSketchCanvas.supportsMultiTouch = true;
            cSketchCanvas.textureSize = new Vector2(4096, 2048);
        }

        public override void OnDisable()
        {
            cStampBush = null;
            cSketchCanvas.ClearCanvas();
            cSketchCanvas.brushForTouchDict.Clear();
        }

        public Brush GetBrush(int _index, float _diameter, Vector3 _dir)
        {
            if (_index >= brushArr.Length) _index = brushArr.Length - 1;

            AudioSource.PlayClipAtPoint(clips[Random.Range(0, clips.Length)], Vector3.zero);

            cStampBush = brushArr[_index];
            float _angle = Quaternion.Dot(Quaternion.Euler(Vector3.up), Quaternion.Euler(_dir)) * 360f;
            Color c = colorTable.GetRandomColor();
            if ((c.r + c.g + c.b) == 3f)
            {
                c.g -= 0.8f;
                c.b -= 0.8f;
            }

            cStampBush.diameter = _diameter;
            cStampBush.color = c;
            cStampBush.angle = _angle;
            return cStampBush;
        }

        protected override Brush GetBrush()
        {
            AudioSource.PlayClipAtPoint(clips[Random.Range(0, clips.Length)], Vector3.zero);

            int currentIndex = (int)TwoDimensionFruitPrint.eFruitType;
            //cStampBush = brushArr[stampIndex];
            cStampBush = brushArr[(int)TwoDimensionFruitPrint.eFruitType];
            Vector3 dir = new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), 0);
            angleArr[currentIndex] = Quaternion.Dot(Quaternion.Euler(Vector3.up), Quaternion.Euler(dir)) * 360f;
            Color c = colorTable.GetRandomColor();
            if ((c.r + c.g + c.b) == 3f)
            {
                c.g -= 0.2f;
                c.b -= 0.2f;
            }

            cStampBush.color = c;
            cStampBush.angle = angleArr[currentIndex];

            /*
            cStampBush.colorDynamicComponent.hueJitter = Random.Range(0.5f, 1f);
            cStampBush.colorDynamicComponent.saturationJitter = Random.Range(0.7f, 1f);
            cStampBush.colorDynamicComponent.brightnessJitter = Random.Range(0.7f, 1f);
            cStampBush.shapeDynamicComponent.angleControl = AngleControl.Off;
            cStampBush.shapeDynamicComponent.angleJitter = Random.Range(0, 1f);
            */
            return cStampBush;
        }

        /// <summary>
        /// 사용자 입력과 상호 작용 하는 함수
        /// </summary>
        void Update()
        {
            if (CustomInput.touchCount > 0)
            {
                TouchInfo[] touches = CustomInput.touches;
                for (int i = 0; i < CustomInput.touchCount; ++i)
                {
                    if (touches[i].phase == TouchInfo.Phase.Begin)
                    {
                        //Vector3 curPos = UtilityScript.WindowToNGUI();
                        Ray ray = UtilityScript.NGUICamera.ScreenPointToRay(touches[i].position);

#if USING_PHYSICS2D
                    RaycastHit2D rayHit = Physics2D.Raycast(ray.origin, ray.direction, 100f, 0x01<<Constante.TWODIMENSION_PANEL) ;
#elif USING_PHYSIC3D

                        RaycastHit rayHit;
                        Physics.Raycast(ray, out rayHit, 100f, 0x01 << Constante.TWODIMENSION_PANEL);
#endif
                        // 과일 객체 선택
                        if (rayHit.collider != null)
                        {
#if USING_PHYSICS2D
                        FruitObject fruit = rayHit.collider.GetComponent<FruitObject>();
#elif USING_PHYSIC3D
                            FruitObject fruit = rayHit.collider.GetComponentInParent<FruitObject>();
#endif
                            if (fruit != null)
                            {
                                // 과일 비활성화
                                fruit.DisAppear();

                                // 과일 도장 할당
                                cSketchCanvas.brush = GetBrush(fruit.kindOfFruit, fruit.img.width, fruit.cachedTransform.localEulerAngles);
                                if (cSketchCanvas.brush == null)
                                { Debug.Log("Fruit Stamp cSketchCanvas.brush == null"); }
                                else
                                {
#if USING_PHYSICS2D
                                
#elif USING_PHYSIC3D
                                    Vector2 _pos = rayHit.collider.transform.parent.localPosition;
#endif
                                    // 실제 과일 도장을 Canvas에 그리는 함수 호출
                                    cSketchCanvas.Stamp(touches[i], _pos, cSketchCanvas.brush.maskName);
                                    if (particleMamager != null)
                                    {
                                        // 파티클 호출
                                        particleMamager.Emitt(cachedTransform, _pos, true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DerivedClearCanvas()
        {
            cSketchCanvas.ClearCanvas();
        }

        public override bool StateInPlay()
        {
            return true;
        }
        public override bool StateEventReady()
        {
            return true;
        }
        public override bool StateEventActivates()
        {
            return true;
        }
    }
}