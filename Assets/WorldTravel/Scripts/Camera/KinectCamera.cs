using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.MapoContents.WorldTravel
{
    using ML.MapoContents.Kinect;

    [RequireComponent(typeof(PanoramicCamera))]
    public class KinectCamera : MonoBehaviour
    {
        private PanoramicCamera _panoramicCamera;

        private ulong _trackingId;
        private float _lerpRotY;
        //private float _lerpPitch;
        public bool BodyCheck;
        public void Start()
        {
            _panoramicCamera = GetComponent<PanoramicCamera>();

               KinectHelper helper = KinectHelper.instance;
            if (helper != null )
            {
                helper.onBodyTracked += _OnBodyTracked;
                helper.onBodyTurn += _OnBodyTurn;
                helper.onBodyTilt += _OnBodyTilt;
            }
		}
        public void CloseUpDown(bool _updown)
        {
            if (_updown)
            {
                _panoramicCamera.fieldOfView -= 0.5f;
                if (_panoramicCamera.fieldOfView <= 10)
                    _panoramicCamera.fieldOfView = 10;
            }
            else
            {
                _panoramicCamera.fieldOfView += 0.5f;
                if (_panoramicCamera.fieldOfView >= 72)
                    _panoramicCamera.fieldOfView = 72;
            }
        }
        public void Update()
        {
            _lerpRotY = Mathf.Lerp(_lerpRotY, 0.0f, Time.deltaTime * 6.5f);
            //_lerpPitch = Mathf.Lerp(_lerpPitch, 0.0f, Time.deltaTime * 6.5f);
            KinectHelper helper = KinectHelper.instance;
			BodyCheck = true;
			if (helper.trackingId != 0 && BodyCheck && !TravelManager.Instance.TabletControll)
            {
                var joint = helper.trackedBody.Joints[Windows.Kinect.JointType.SpineBase];
				float z = joint.Position.Z;
				_panoramicCamera.fieldOfView = Mathf.Lerp(_panoramicCamera.fieldOfView, Mathf.Clamp(72.0f * (z * 0.25f), 24.0f, 72.0f), Time.deltaTime * 6.5f);//0.333f -> 0.25f
            }
        }

        public void OnDestroy()
        {
            KinectHelper helper = KinectHelper.instance;
            if (helper != null)
            {
                helper.onBodyTracked -= _OnBodyTracked;
                helper.onBodyTurn -= _OnBodyTurn;
                helper.onBodyTilt -= _OnBodyTilt;
            }
        }

        private void _OnBodyTracked(ulong trackingId)
        {
            _trackingId = trackingId;
        }

        private void _OnBodyTurn(ulong trackingId, Vector3 dir)
        {
            if (_trackingId == trackingId && !TravelManager.Instance.TabletControll)
            {
                float val = Vector3.Dot(Vector3.back, dir);
                val = Mathf.Clamp01(Mathf.Abs(val) * 1.3f - 0.3f) * (val >= 0 ? 1.0f : -1.0f) * 10.0f;
                _lerpRotY = val;
                CameraRotation.RotateY(transform, _lerpRotY);
            }
        }
        public void CommandBOdyTurn(float r)
        {
            CameraRotation.RotateY(transform, r);
        }

        private void _OnBodyTilt(ulong trackingId, Vector3 dir)
        {
            /*
            if (_trackingId == trackingId)
            {
                float val = -dir.z;
                _lerpPitch = val;
                CameraRotation.Pitch(transform, _lerpPitch);
            }
            */
        }
    }
}