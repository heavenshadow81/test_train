using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.MotionJump
{
    using Common;

    /// <summary>
    /// 콘텐츠 카메라 제어 클래스
    /// </summary>
    public class JumpCameraController : MonoBehaviour
    {

        public enum EDirection { NONE, LOOK_UP, LOOK_DOWN, CLOCKWISE, COUNTER_CLOCKWISE, FRONT }

        public Vector3 defaultPosition;

        public bool cameraShaking
        {
            get;
            private set;
        }
        public bool fixMoveHorizontal;

        private Transform mTransform;
        private Camera mCam;
        public Transform cachedTransform
        {
            get
            {
                if (mTransform == null)
                { mTransform = GetComponent<Transform>(); }
                return mTransform;
            }
        }

        public float height
        { get { return cachedTransform.localPosition.y; } }

        public Camera cam
        {
            get
            {
                if (mCam == null)
                { mCam = gameObject.GetComponentInChildren<Camera>(); }
                return mCam;
            }
        }

        Transform mCamTransform;
        public Transform camTransform
        {
            get
            {
                if (mCamTransform == null)
                { mCamTransform = gameObject.GetComponentInChildren<Camera>().transform; }
                return mCamTransform;
            }
        }

        public void Initialize()
        {
            StopCoroutine(LookProcess(Vector3.one));
            StopCoroutine(ZoomProcess(0));

            cam.fieldOfView = ScreenUtil.aspectRatio >= 2 ? 10 : 15;
            cachedTransform.localEulerAngles = Vector3.zero;

            float _top = cam.ViewportToWorldPoint(new Vector3(0, 0, defaultPosition.z)).y;
            float _bottom = cam.ViewportToWorldPoint(new Vector3(0, 1, defaultPosition.z)).y;
            defaultPosition.y = (_top - _bottom) * 0.3f;
            cachedTransform.localPosition = defaultPosition;
        }

        public void TurnVerticalRotation(Vector3 _target, float _angle)
        {
            Vector3 origin = cachedTransform.localPosition - _target;
            float y = Mathf.Cos(_angle) * origin.y - Mathf.Sin(_angle) * origin.z;
            float z = Mathf.Sin(_angle) * origin.y + Mathf.Cos(_angle) * origin.z;
            origin.y = y;
            origin.z = z;
            cachedTransform.localPosition = origin + _target;
        }

        public void TurnHorizontalRotation(Vector3 _target, float _angle)
        {
            Vector3 origin = cachedTransform.localPosition - _target;
            float x = Mathf.Cos(_angle) * origin.x + Mathf.Sin(_angle) * origin.z;
            float z = -1 * Mathf.Sin(_angle) * origin.x + Mathf.Cos(_angle) * origin.z;
            origin.x = x;
            origin.z = z;
            cachedTransform.localPosition = origin + _target;
        }

        public void CameraShake()
        {
            if (!cameraShaking)
            {
                cameraShaking = true;
                StartCoroutine(ShakeCameraProcess(1.5f, 0.1f));
            }
        }

        public void LookAt(Vector3 _target)
        {
            _target.x = (fixMoveHorizontal ? 0 : _target.x);
            //Quaternion q = Quaternion.LookRotation(_target - cachedTransform.localPosition);
            Quaternion q = Quaternion.LookRotation(_target - cachedTransform.position);
            cachedTransform.localRotation = Quaternion.Lerp(cachedTransform.localRotation, q, Time.deltaTime * 1.5f);
        }

        public bool AdjustFov(float targetFov, float dempingStrength = 1f, bool bZoomIn = true)
        {
            float fGap = targetFov - cam.fieldOfView;

            if ((bZoomIn && fGap >= -0.1f) || (!bZoomIn && fGap <= 0.1f))
            {
                cam.fieldOfView = targetFov;
                return true;
            }
            fGap *= Time.deltaTime * dempingStrength;
            cam.fieldOfView += fGap;

            return false;
        }

        public void Zoom(Vector3 target, float zoomValue)
        {
            StartCoroutine(LookProcess(target));
            StartCoroutine(ZoomProcess(zoomValue));
        }

        IEnumerator LookProcess(Vector3 _target)
        {
            Vector3 target = _target;
            float activeTime = 0;
            do
            {
                LookAt(target);
                activeTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            } while (activeTime < 3f);

        }

        IEnumerator ZoomProcess(float targetFov)
        {
            bool bZoomIn = (targetFov - cam.fieldOfView < 0);

            bool bComplete = false;
            do
            {
                bComplete = AdjustFov(targetFov, 2f, bZoomIn);
                yield return new WaitForEndOfFrame();
            } while (!bComplete);
        }

        IEnumerator ShakeCameraProcess(float duration, float sensitive)
        {
            float fDelatTime = 0;
            Vector3 cameraPosition = cam.transform.localPosition;

            while (duration > fDelatTime)
            {
                fDelatTime += Time.deltaTime;
                Vector3 temp = Vector3.zero;

                cam.transform.localPosition = cameraPosition;
                temp.x = Random.Range(-sensitive, sensitive);
                temp.y = Random.Range(-sensitive, sensitive);

                cam.transform.localPosition += temp;
                yield return new WaitForEndOfFrame();
            }
            cameraShaking = false;

            cam.transform.localPosition = cameraPosition;
        }
    }

    /* 
     * 
        public EDirection dirHorizontal
        {
            get { return mHorizontalDirection; }
            set
            {
                if (value != mHorizontalDirection)
                {
                    switch (value)
                    {
                        case EDirection.CLOCKWISE:
                            mPreHorizontalDirection = EDirection.CLOCKWISE;
                            fLimitHorizontalDegree = horizontalBorder;
                            break;

                        case EDirection.COUNTER_CLOCKWISE:
                            mPreHorizontalDirection = EDirection.COUNTER_CLOCKWISE;
                            fLimitHorizontalDegree = horizontalBorder;
                            break;

                        case EDirection.FRONT:
                            fLimitHorizontalDegree = 0;

                            break;
                    }
                    mHorizontalDirection = value;
                }
            }
        }

        public EDirection dirVertical
        {
            get { return mVecrticalDirection; }
            set
            {
                if (value != mVecrticalDirection)
                {
                    switch (value)
                    {
                        case EDirection.LOOK_UP:
                            bLookAtDown = false;
                            fLimitVertical = verticalBorder;
                            break;

                        case EDirection.LOOK_DOWN:
                            bLookAtDown = true;
                            break;

                        case EDirection.FRONT:
                            fLimitVertical = -1 * frontViewInitValue;
                            bLookAtDown = false;
                            break;
                    }
                    mVecrticalDirection = value;
                }
            }
        }
     * public bool TurnRotation(ref float vectorComponent, float limitAngle, float rotateSpeed, bool bClockwise)
        {
            bool bComplete = false;
            if (vectorComponent >= 180)
                vectorComponent -= 360f;

            float f = bClockwise ? 1 : -1;
            vectorComponent += f * Time.deltaTime * rotateSpeed;

            if (bClockwise)
            {
                bComplete = vectorComponent >= limitAngle;
                if (bComplete) vectorComponent = limitAngle;
            }
            else
            {
                bComplete = vectorComponent < -1 * limitAngle;
                if (bComplete) vectorComponent = 360 - limitAngle;
            }
            return bComplete;
        }
     * void FixedUpdate()
        {
            if (dirVertical != EDirection.NONE)
            {
                Vector3 dir = cachedTransform.localEulerAngles;
                bool complte = TurnRotation(ref dir.x, fLimitVertical, verticalRotateSpeed, bLookAtDown);
                cachedTransform.localEulerAngles = dir;
                if (complte) dirVertical = EDirection.NONE;
            }

            if (dirHorizontal != EDirection.NONE)
            {
                float degree = Time.deltaTime;
                if (dirHorizontal == EDirection.CLOCKWISE)
                { degree *= horizontalRotateSpeed; }
                else if (dirHorizontal == EDirection.COUNTER_CLOCKWISE)
                { degree *= -1 * horizontalRotateSpeed; }
                else if (dirHorizontal == EDirection.FRONT)
                {
                    if (mPreHorizontalDirection == EDirection.CLOCKWISE)
                    { degree *= -1 * horizontalRotateSpeed; }
                    else if (mPreHorizontalDirection == EDirection.COUNTER_CLOCKWISE)
                    { degree *= horizontalRotateSpeed; }
                }
                else
                { degree = 0; }

                if (TurnRotation(degree))
                    dirHorizontal = EDirection.NONE;
            }
        }
     * public bool TurnRotation(float _angle)
        {
            fRotateDegree += _angle;
            bool bComplete = false;
            if (fLimitHorizontalDegree != 0)
            {
                fRotateDegree = Mathf.Clamp(fRotateDegree, -1 * fLimitHorizontalDegree, fLimitHorizontalDegree);
                bComplete = (fRotateDegree >= fLimitHorizontalDegree || fRotateDegree <= -1 * fLimitHorizontalDegree ? true : false);
            }
            else
            {
                if (_angle > 0 && fRotateDegree >= 0 || _angle < 0 && fRotateDegree <= 0)
                {
                    fRotateDegree = 0f;
                    return true;
                }
            }

            if (!bComplete)
            { TurnHorizontalRotation(target.localPosition, _angle); }
            return bComplete;
        }*/
}