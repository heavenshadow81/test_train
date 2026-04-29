using UnityEngine;
using DG.Tweening;

namespace ML.T_Sports.VolleyBall
{
    /// <summary>
    /// 배구공 콘텐츠 카메라. 단순히 tween을 사용하기 위한 용도.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class VolleyBallMainCamera : MonoBehaviour
    {
        private Camera _cam;

        private void Awake()
        {
            _cam = GetComponent<Camera>();

            Vector3 toPos = transform.localPosition;
            Vector3 toEuler = transform.localEulerAngles;
            float toFov = _cam.fieldOfView;
            transform.localPosition += Vector3.up * 1.2f;
            transform.localEulerAngles = (new Vector3(toEuler.x * 2, toEuler.y, toEuler.z));
            _cam.fieldOfView += 10;
            transform.DOMove(toPos, 2.0f).SetEase(Ease.InOutSine);
            _cam.DOFieldOfView(toFov, 2.0f).SetEase(Ease.InOutSine);
            transform.DOLocalRotate(toEuler, 2.0f).SetEase(Ease.InOutSine);
        }


    }
}