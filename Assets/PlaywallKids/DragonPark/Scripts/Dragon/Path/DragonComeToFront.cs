using UnityEngine;
using ML.PlaywallKids.Common;

namespace ML.PlaywallKids.DragonPark
{
    public class DragonComeToFront : MonoBehaviour
    {
        public enum ComeReason
        {
            None, Eat, Interaction, Ride
        }

        #region Public variables
        private static int _userCount = 0;
        public static int userCount
        {
            get
            {
                return _userCount;
            }
            set
            {
                _userCount = Mathf.Clamp(value, 1, CommonSettings.maxUserCount);
            }
        }

        public int userId = 0;
        public bool receivesTouch = false;
        #endregion

        #region Properties
        private DragonAnimationControl _dragonAnimation;
        public DragonAnimationControl dragonAnimation
        {
            get
            {
                if (gameObject != null)
                {
                    if (_dragonAnimation == null)
                    {
                        _dragonAnimation = GetComponent<DragonAnimationControl>();
                    }
                }
                return _dragonAnimation;
            }
        }

        private bool _isComing = false;
        public bool isComing
        {
            get
            {
                return _isComing;
            }
        }

        private bool _came = false;
        public bool came
        {
            get
            {
                return _came;
            }
        }
        #endregion

        private const float BACK_TIMER = 5f;
        private float fBackTimer;

        private ComeReason comeReason = ComeReason.None;

        public delegate void DelegateComeFinished(DragonAnimationControl animationConterol);
        public DelegateComeFinished delegateComeFinished;

        void Awake()
        {
            if (userCount == 0)
            {
                userCount = CommonSettings.maxUserCount;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!_isComing)
            {
                if (receivesTouch)
                {
                    int touchCount = CustomInput.touchCount;
                    for (int i = 0; i < touchCount; i++)
                    {
                        TouchInfo touch = CustomInput.GetTouch(i);
                        if (touch.phase != TouchInfo.Phase.Begin) continue;

                        // Ray
                        Vector3 point = touch.position;
                        if (BackgroundManager.sharedInstance != null && BackgroundManager.sharedInstance.mainCamera != null)
                        {
                            BarrelDistortionEffect distortionEffect = BackgroundManager.sharedInstance.mainCamera.GetComponent<BarrelDistortionEffect>();

                            if (distortionEffect != null)
                            {
                                point = distortionEffect.GetDistoredScreenPosFromOriginal(point);
                            }
                        }
                        Ray ray = Camera.main.ScreenPointToRay(point);
                        RaycastHit hitInfo;
                        if (Physics.Raycast(ray, out hitInfo))
                        {
                            if (hitInfo.collider.gameObject == gameObject)
                            {
                                if (!_came)
                                {
                                    //userId = touch.userId;
                                    Come();
                                }
                                else
                                {
                                    Back();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (Mathf.Abs((transform.position - GetFrontPosition()).sqrMagnitude) < 0.5f)
                {
                    //dragonAnimation.Stop();

                    _came = true;
                    _isComing = false;

                    if (delegateComeFinished != null)
                        delegateComeFinished(_dragonAnimation);

                }
                else
                {
                    // Move!
                    dragonAnimation.SetDestination(GetFrontPosition());
                }
            }

            if (came)
            {
                //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(GetFrontLookPosition(), transform.up), Time.deltaTime * 4.0f);
                Quaternion rot = transform.rotation;
                transform.LookAt(GetFrontLookPosition());
                Quaternion frontLook = transform.rotation;
                transform.rotation = Quaternion.Slerp(rot, frontLook, Time.deltaTime * 5.0f);

                //			transform.LookAt(GetFrontLookPosition());

                switch (comeReason)
                {
                    case ComeReason.Interaction:
                        if (fBackTimer < BACK_TIMER)
                        {
                            fBackTimer += Time.deltaTime;
                            if (fBackTimer >= BACK_TIMER)
                                Back();
                        }
                        break;
                }
            }
        }

        public void Come()
        {
            Come(ComeReason.None, null);
        }

        public void Come(ComeReason reason)
        {
            Come(reason, null);
        }

        public void Come(ComeReason reason, DelegateComeFinished complete)
        {
            comeReason = reason;
            delegateComeFinished = complete;

            // set true flag
            _isComing = true;

            // Pause moving along path
            dragonAnimation.movesAlongPath = false;

            // Wake if sleeping
            dragonAnimation.Wake(true);

            // Move!
            Vector3 newDestination = GetFrontPosition();
            Vector3 offset = (newDestination - transform.position).normalized * 1.5f;
            dragonAnimation.SetDestination(GetFrontPosition());
        }

        public void Back()
        {
            dragonAnimation.movesAlongPath = true;
            _came = false;
            _isComing = false;
        }

        public void BackTimerInit()
        {
            fBackTimer = 0f;
        }

        public Vector3 GetFrontPosition()
        {
            Vector3 pos = GetDummyPosition("front", userId);
            if (pos == Vector3.zero)
            {
                // ~3.5f ~ 6.5f
                float offset = 10.0f / (userCount - 1);
                pos = new Vector3(20.0f, 0.0f, -2.5f + userId + 0.5f * offset);
            }
            return pos;
        }

        public Vector3 GetFrontLookPosition()
        {
            Vector3 pos = GetDummyPosition("frontlook", userId);
            if (pos == Vector3.zero)
            {
                pos = GetFrontPosition() + new Vector3(1.0f, 0.0f, 0.0f);
            }
            return pos;
        }

        public static Vector3 GetDummyPosition(string dummyName, int userId)
        {
            Vector3 pos = Vector3.zero;

            GameObject first = null, last = null, prev = null;
            float avgDistBetweenDummies = 0.0f;
            int dummyCount = 0;

            for (int i = 1, cnt = 10; i <= cnt; i++)
            {
                GameObject go = GameObject.Find(string.Format("{0}_{1}", dummyName, i));
                if (first == null)
                {
                    first = go;
                }
                else if (go != null)
                {
                    last = go;

                    if (prev != null)
                    {
                        avgDistBetweenDummies += (prev.transform.position - go.transform.position).magnitude;
                    }
                }

                if (i == cnt || go == null)
                {
                    dummyCount = (go == null ? i - 1 : i);
                    avgDistBetweenDummies = dummyCount <= 1 ? 0 : (avgDistBetweenDummies / (float)(dummyCount - 1));
                    break;
                }

                prev = go;
            }

            if (first != null && last != null)
            {
                Vector3 pos1 = first.transform.position;
                Vector3 pos2 = last.transform.position;
                Vector3 center = (pos1 + pos2) * 0.5f;

                Vector3 offset = (pos2 - pos1).normalized * avgDistBetweenDummies;
                pos = center + offset * (-0.5f * (userCount - 1) + userId);

                /* 
                 * If the dummy name contains "front" or equals to "fruit", "petmotion",
                 * We'll use the screen pos rather than actual dummy position.
                 * Because we need to match the position of the UI and game objects.
                 */
                if (dummyName.Contains("front") || dummyName.Equals("fruit") || dummyName.Equals("petmotion"))
                {
                    Vector3 screenPos1 = Camera.main.WorldToScreenPoint(pos1);
                    Vector3 screenPos2 = Camera.main.WorldToScreenPoint(pos2);

                    // If the main camera has the distortion effect, adjust the screen position
                    BarrelDistortionEffect distortionEffect = Camera.main.GetComponent<BarrelDistortionEffect>();
                    if (distortionEffect != null)
                    {
                        screenPos1 = distortionEffect.GetOriginalScreenPosFromDistorted(screenPos1);
                        screenPos2 = distortionEffect.GetOriginalScreenPosFromDistorted(screenPos2);
                    }

                    Vector3 screenPosCurrent = (screenPos1 + screenPos2) * 0.5f;
                    screenPosCurrent.x = (Screen.width / CommonSettings.maxUserCount) * (userId + 0.5f);

                    if (distortionEffect != null)
                    {
                        screenPosCurrent = distortionEffect.GetDistoredScreenPosFromOriginal(screenPosCurrent);
                    }

                    Ray ray = Camera.main.ScreenPointToRay(screenPosCurrent);
                    RaycastHit hitInfo;

                    // 1 : Default
                    if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1))
                    {
                        if (hitInfo.collider != null && hitInfo.collider.name.ToLower().Contains("ground"))
                            pos = hitInfo.point;
                    }
                }
            }
            else if (first != null)
                pos = first.transform.position;

            return pos;
        }
    }
}