//#define USE_TAG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.Interaction
{
    using Common;

    //콘텐츠 주 관리자
#if USE_TAG
    public class InteractionKickBallMainController : InteractionBaseClass
#else
public class InteractionKickBallMainController : MonoBehaviour
#endif
    {
        public Camera mainCam;
        public Camera uiCam;
        public GameObject contentsPrefab;
        public UIInteractionController uiController;
        
        public Collider colPlan_2x3;
        public Collider colPlan_2x6;

        public AudioSource sndGoal;
        public AudioClip sndKick;

        private IEnumerator cor;


        int _num;
#if USE_TAG
        public override int numberOfPlayers
#else
        public int numberOfPlayers
#endif
        {
            set
            {
                if (value < 0 || value > 2)
                    _num = 0;
                else
                    _num = value;
            }
            get
            {
                return _num;
            }
        }

        private const int width = 50;
        private InteractionKickBallContentsController[] cloneControllers;
        private GameObject[] cloneCameras;
        private int countStartingGame;

#if USE_TAG
        List<NFCUserInfo> userList;
#endif

        void OnEnable()
        {
#if USE_TAG
            if (userList == null)
                userList = new List<NFCUserInfo>();
#endif

            Init(numberOfPlayers);
        }

        void OnDisable()
        {

            mainCam.enabled = true;
            if (uiController != null)
                uiController.gameObject.SetActive(true);

            for (int i = 0; i < numberOfPlayers; ++i)
            {
                Destroy(cloneCameras[i].gameObject, 0.001f);
                Destroy(cloneControllers[i].gameObject, 0.01f);
            }

            cloneControllers = null;
        }

        void Init(int _numberOfPlayers)
        {
            if (cloneControllers != null || _numberOfPlayers <= 0) return;

            if (_numberOfPlayers == 0) _numberOfPlayers = 1;
            cloneControllers = new InteractionKickBallContentsController[_numberOfPlayers];

            cloneCameras = new GameObject[_numberOfPlayers];
            countStartingGame = 0;

            switch (_numberOfPlayers)
            {
                case 1:
                case 2:
                    for (int i = 0; i < _numberOfPlayers; ++i)
                    {
                        GameObject _camObject = NGUITools.AddChild(UIRoot.list[0].gameObject, uiCam.gameObject) as GameObject;
                        GameObject _go = NGUITools.AddChild(this.gameObject, contentsPrefab) as GameObject;

                        _go.transform.localPosition = new Vector3(i * width, 0, 20f);
                        _go.transform.eulerAngles = new Vector3(-10f, 0, 0);
                        _go.SetActive(true);

                        _camObject.transform.localPosition = new Vector3(i * width * 1000f, 0f, 0f);

                        UIInteractionController _uiController = _camObject.GetComponentInChildren<UIInteractionController>();

                        _uiController.currentTimer = _uiController.circleTypeTimer;
                        _uiController.uiCam = _camObject.GetComponent<Camera>();
                        _uiController.score.transform.localScale = new Vector3(1 / (float)_numberOfPlayers, 1 / (float)_numberOfPlayers, 1f);
                        _uiController.barTypeTimer.Active = false;
                        _uiController.score.gameObject.SetActive(true);
                        _uiController.guidanceObject.SetActive(true);
                        _uiController.numericOfCountdown.gameObject.SetActive(true);
                        _uiController.resultObject.gameObject.SetActive(true);
                        _uiController.currentTimer.gameObject.SetActive(true);

                        foreach (Transform _t in _uiController.GetComponentsInChildren<Transform>())
                        {
                            _t.gameObject.layer = Common.LayerConstants.OTHERLAYER;
                        }

                        InteractionContents _contents = _go.GetComponent<InteractionContents>();
                        _contents.uiController = _uiController;
                        _contents.indexOfNumber = i;
                        _contents.numberOfPlayers = _numberOfPlayers;
                        _uiController.uiCam.cullingMask = 0x01 << Common.LayerConstants.OTHERLAYER;
                        _uiController.uiCam.depth = 4;
                        cloneCameras[i] = _camObject.gameObject;
                        cloneControllers[i] = _go.GetComponent<InteractionKickBallContentsController>();
                        cloneControllers[i].cam.rect = _uiController.uiCam.rect = new Rect(i * (1f / (float)_numberOfPlayers), 0, (1f / (float)_numberOfPlayers), 1f);
                    }
                    break;
                case 3:
                case 4:
                default://무제한
                    break;
            }

            mainCam.enabled = false;
            uiController.gameObject.SetActive(false);
        }

#if USE_TAG
        void FixedUpdate()
        {
            if (userList.Count > 0)
            {
                NFCUserInfo _info = userList[0];
                int _index = 0;
                if (!CheckEqualUser(_info.userName, _index))
                {
                    for (int i = 0, len = cloneControllers.Length; i < len; ++i)
                    {
                        if (!cloneControllers[i].IsTaged && !cloneControllers[i].IsPlaying)
                        {
                            cloneControllers[i].SetUser(_info.userName, _info.seqNo);
                            userList.RemoveAt(0);
                            return;
                        }
                    }
                }
            }
        }

        bool CheckEqualUser(string _userName, int _index)
        {
            if (_index < cloneControllers.Length)
            {
                if (cloneControllers[_index].IsTaged || cloneControllers[_index].IsPlaying)
                {
                    if (cloneControllers[_index].EqualUser(_userName))
                    {
                        userList.RemoveAt(0); //중복 된 ID
                        return true;
                    }
                }
                else
                    return CheckEqualUser(_userName, ++_index);
            }

            return false;
        }

        public override void SetStringValue(string _jsonStr)
        {
            NFCUserInfo _info = NFCUserInfo.GetNFCUserInfo(_jsonStr);
            userList.Add(_info);
        }

#endif

        public void Kick(Camera _cam, float x, float y)
        {
            if (_cam != null || cloneControllers != null)
            {
                float _w = 1 / (float)numberOfPlayers;
                int _index = (int)(x / _w);
                if (cloneControllers.Length <= _index || !cloneControllers[_index].IsPlaying) return;

                x %= _w;
                x *= numberOfPlayers;
                Vector3 screenPoint = BarrelDistortionEffect.ConvertToDistorted(_cam, PositionType.ViewPort, PositionType.Screen, new Vector3(x, y));
                Ray ray = cloneControllers[_index].cam.ViewportPointToRay(new Vector3(x, y, 0f));
                RaycastHit hitInfo;

                if (Physics.Raycast(ray, out hitInfo, 50f, 0x01 << Common.LayerConstants.KICKBALL_COLLIDER))
                {
                    if (hitInfo.collider != null)// && hitInfo.collider.transform.parent == _tf)
                    {
                        if (hitInfo.collider.gameObject.layer == Common.LayerConstants.KICKBALL_COLLIDER)
                        {
                            InteractionKickBall ball = hitInfo.collider.GetComponentInParent<InteractionKickBall>();
                            if (ball != null)
                            {
                                cloneControllers[_index].CountKickIt();
                                Vector3 ballPoint = BarrelDistortionEffect.ConvertToOriginal(mainCam, PositionType.WorldPoint, PositionType.Screen, ball.transform.position);
                                Vector3 screenOriginalPoint = BarrelDistortionEffect.ConvertToOriginal(mainCam, PositionType.Screen, screenPoint);
                                bool inputLeft = true;
                                if (ballPoint.x < screenOriginalPoint.x)
                                    inputLeft = false;

                                ball.Shoot(inputLeft);
                            }

                            if (sndKick)
                                AudioSource.PlayClipAtPoint(sndKick, Vector3.zero);
                        }
                    }
                }
            }
        }

        public void Goal()
        {
            if (cor != null)
                StopCoroutine(cor);
            cor = GoalSound();
            StartCoroutine(cor);
        }

        IEnumerator GoalSound()
        {
            float startVolume = 1.0f;
            sndGoal.volume = startVolume;
            sndGoal.Play();

            yield return new WaitForSeconds(1.5f);

            while (sndGoal.volume > 0)
            {
                sndGoal.volume -= startVolume * Time.deltaTime / 3.0f;

                yield return null;
            }

            sndGoal.Stop();
        }

        public Collider GetCurrentCollider()
        {
            switch (ScreenUtil.screenType)
            {
                case ScreenType.Bigboard2x6:
                    return colPlan_2x6;

                default:
                    return colPlan_2x3;
            }
        }

    }
}