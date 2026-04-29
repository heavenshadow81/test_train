//#define USE_TAG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Body = Windows.Kinect.Body;
using JointType = Windows.Kinect.JointType;
using CameraSpacePoint = Windows.Kinect.CameraSpacePoint;

namespace ML.PlaywallKids.Interaction
{
    using Common;
    using EContentState = InteractionContentsEnum.EState;

#if USE_TAG
    public class InteractionFrisbeeMainController : InteractionBaseClass
#else
    public class InteractionFrisbeeMainController :MonoBehaviour
#endif
    {
#if UNUSE_KINECT
    public enum ETestState
    {
        NONE = 0, FAR = 1, NEAR, IN_AREA
    }

    public ETestState testDistanceWithKinect;
#endif
        public GameObject frisbeePrefab;
        public GameObject brokenTheFrisbee;

        public Camera cam;

        public float throwTime;
        public AudioClip sndEffect;

        public Sprite[] guidanceSprites;

        [HideInInspector]
        int _num;
#if USE_TAG
        public override int numberOfPlayers
#else
    public int numberOfPlayers
#endif
        {
            get
            {
                return _num;
            }
            set
            {
                //   if (!contentsController) contentsController = GetComponent<InteractionContents>();
                if (value <= 0)
                    _num = 1;
                else
                    _num = value;

                //  contentsController.numberOfPlayers = base.numberOfPlayers;
                //  contentsController.index = 0;
            }
        }

        //contents
        private int numOfProduct;
        private int countOfBrokenFrisbee;

        private Coroutine[] coroutines;

        private InteractionContents contentsController;
        private HashSet<GameObject> hashSetOfFrisbee;
        private List<GameObject> listOfBrokenFrisbee;

        public bool CanPlay
        {
            get
            {
                return contentsController.IsPlaying;
            }
        }

        void Awake()
        {
            if (contentsController == null) contentsController = this.GetComponent<InteractionContents>();
            contentsController.SetCallback(EContentState.NONE, Initialize);
            contentsController.SetCallback(EContentState.PLAY_STATE0, CallbackGuidance);
            contentsController.SetCallback(EContentState.PLAYING, InitializeToPlay);
            contentsController.SetCallback(EContentState.CLOSE_EVENT0, InitializeToClose);
            contentsController.SetCallback(EContentState.CLOSE_EVENT0, SetScore);
        }

        void OnEnable()
        {
            if (!contentsController) contentsController = GetComponent<InteractionContents>();
        }

        void OnDisable()
        {
            foreach (GameObject go in hashSetOfFrisbee)
                Destroy(go);
            hashSetOfFrisbee.Clear();
            if (listOfBrokenFrisbee != null)
                listOfBrokenFrisbee.TrimExcess();
            listOfBrokenFrisbee = null;
        }

#if USE_TAG
        public override void SetStringValue(string _jsonStr)
        {
            contentsController.SetUser(_jsonStr);
        }
#endif

        public void PunchDish(Camera _cam, float x, float y)
        {
            if (float.IsInfinity(x) || float.IsInfinity(y))
                return;

            //Ray ray = _cam.ViewportPointToRay(BarrelDistortionEffect.ConvertToDistorted(_cam, PositionType.ViewPort, new Vector3(x, y, 0)));
            Ray ray = _cam.ViewportPointToRay(new Vector3(x, y, 0));
            RaycastHit2D _rayHit = Physics2D.Raycast(ray.origin, ray.direction);
            if (_rayHit.collider != null && _rayHit.collider.gameObject != null)
            {
                GameObject go = _rayHit.collider.gameObject;
                if (hashSetOfFrisbee.Contains(go))//  Hit
                {
                    UIInteractionController uiController = contentsController.uiController;
                    if (brokenTheFrisbee != null)
                    {
                        Vector2 _viewPort = _cam.WorldToViewportPoint(_rayHit.transform.position);
                        Vector2 _nguiPosition = uiController.uiCam.ViewportToScreenPoint(_viewPort);
                        _nguiPosition *= ScreenUtil.NGUIScaleRatio;
                        _nguiPosition.x -= ScreenUtil.NGUIWidth * 0.5f;
                        _nguiPosition.y -= ScreenUtil.NGUIHeight * 0.5f;

                        uiController.uiPointOfScoreManager.DisplayScore(_nguiPosition, 10);
                        uiController.score.Score += 10;

                        ++countOfBrokenFrisbee;
                        if (sndEffect)
                            AudioSource.PlayClipAtPoint(sndEffect, Vector2.zero);
                        var effect = (GameObject)Instantiate(brokenTheFrisbee);
                        effect.SetActive(true);
                        effect.transform.parent = go.transform.parent;
                        effect.transform.localPosition = go.transform.localPosition;
                        effect.transform.localRotation = go.transform.localRotation;
                        effect.transform.localScale = brokenTheFrisbee.transform.localScale;
                        Destroy(effect, 6.0f);
                    }

                    hashSetOfFrisbee.Remove(go);
                    Destroy(go);
                }
            }
        }

        IEnumerator MakeNewAFrisbeeProcess(Camera _cam)
        {
            do
            {
                yield return new WaitForSeconds(Random.Range(0.3f, 1.5f));

                if (frisbeePrefab != null)
                {
                    GameObject go = (GameObject)Instantiate(frisbeePrefab);
                    go.SetActive(true);
                    go.transform.parent = _cam.transform;

                    Vector3 screenPos = new Vector3((Random.Range(0, 4) % 2 == 0 ? -Screen.width * 0.2f : Screen.width * 1.2f), Random.Range(-0.3f, 0.2f) * Screen.height, 10f + Random.value * 7.5f);
                    Vector3 pos = _cam.ScreenToWorldPoint(screenPos);
                    go.transform.position = pos;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = frisbeePrefab.transform.localScale;

                    AutoRotate ar = go.GetComponent<AutoRotate>();
                    if (ar == null) ar = go.AddComponent<AutoRotate>();
                    ar.isLocal = true;
                    ar.axis = Vector3.up + Vector3.forward * Random.Range(0f, 0.1f);
                    ar.anglePerSecond = 180.0f;

                    float pow = 2f;// Random.Range(1.75f, 2.5f);

                    Rigidbody2D rigidbody2D = go.GetComponent<Rigidbody2D>();
                    if (rigidbody2D == null) rigidbody2D = go.AddComponent<Rigidbody2D>();
                    rigidbody2D.AddForce(new Vector2(-go.transform.position.x * pow / throwTime, -Physics2D.gravity.y * pow / throwTime), ForceMode2D.Impulse);
                    rigidbody2D.gravityScale = Mathf.Pow(pow / throwTime, 2);

                    TweenScale tweenScale = go.GetComponent<TweenScale>();
                    if (tweenScale == null) tweenScale = go.AddComponent<TweenScale>();
                    tweenScale.from = Vector3.one * Random.Range(0.9f, 1.2f);
                    tweenScale.to = Vector3.one * Random.Range(0.4f, 1f);
                    tweenScale.duration = 3f;

                    if (Random.value > 0.5f)
                    {
                        Vector3 temp = tweenScale.to;
                        tweenScale.to = tweenScale.from;
                        tweenScale.from = temp;
                    }

                    go.transform.localScale = tweenScale.from;
                    tweenScale.PlayForward();
                    hashSetOfFrisbee.Add(go);
                    ++numOfProduct;
                }


            } while (contentsController.IsPlaying);
        }

        IEnumerator CheckOutOfAreaProcsee(Camera _cam)
        {
            do
            {
                foreach (GameObject go in hashSetOfFrisbee)
                {
                    if (_cam.WorldToScreenPoint(go.transform.position).y < Screen.height * -1.0f)
                    {
                        listOfBrokenFrisbee.Add(go);
                    }
                }

                foreach (GameObject go in listOfBrokenFrisbee)
                {
                    hashSetOfFrisbee.Remove(go);
                    Destroy(go);
                }
                yield return new WaitForSeconds(2f);
            } while (contentsController.IsPlaying);
        }

        private void CallbackGuidance()
        {
            contentsController.guidanceSprites = guidanceSprites;
            contentsController.words = new string[]{
            "본 체험은\r\n시간내에 접시를",  //@"접시가 날으면",
            "깨서 점수를\r\n얻는 게임입니다.",  //@"주먹과 발로",
            "접시를 손과 발을\r\n이용해서 깨보세요",//@"접시를 향해 뻗어!",
        };
        }

        private void BrokenADish(Camera _cam, float x, float y)
        {
            if (float.IsInfinity(x) || float.IsInfinity(y))
                return;

            Ray ray = _cam.ViewportPointToRay(BarrelDistortionEffect.ConvertToDistorted(_cam, PositionType.ViewPort, new Vector3(x, y, 0)));

            RaycastHit2D _rayHit = Physics2D.Raycast(ray.origin, ray.direction);
            if (_rayHit.collider != null && _rayHit.collider.gameObject != null)
            {
                GameObject go = _rayHit.collider.gameObject;
                if (hashSetOfFrisbee.Contains(go))
                {
                    if (brokenTheFrisbee != null) //  Hit
                    {
                        var effect = (GameObject)Instantiate(brokenTheFrisbee);
                        effect.SetActive(true);
                        effect.transform.parent = go.transform.parent;
                        effect.transform.localPosition = go.transform.localPosition;
                        effect.transform.localRotation = go.transform.localRotation;
                        effect.transform.localScale = brokenTheFrisbee.transform.localScale;
                        Destroy(effect, 6.0f);
                    }

                    hashSetOfFrisbee.Remove(go);
                    Destroy(go);
                }
            }
        }

        private void Initialize()
        {
            countOfBrokenFrisbee = numOfProduct = 0;
            listOfBrokenFrisbee = new List<GameObject>();
            hashSetOfFrisbee = new HashSet<GameObject>();
            coroutines = new Coroutine[2];
        }

        private void InitializeToPlay()
        {
            coroutines[0] = StartCoroutine(MakeNewAFrisbeeProcess(cam));
            coroutines[1] = StartCoroutine(CheckOutOfAreaProcsee(cam));
        }

        private void SetScore()
        {
            UIInteractionController uiController = contentsController.uiController;
            uiController.resultObject.texts = new string[] {
                        string.Format("총 {0}개", numOfProduct),
                        countOfBrokenFrisbee.ToString(),
                        uiController.score.Score.ToString()
                        };
        }

        private void InitializeToClose()
        {
            if (coroutines != null)
                for (int i = 0; i < coroutines.Length; ++i)
                    StopCoroutine(coroutines[i]);
        }
    }
}