using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace KartRider
{
    /// <summary>
    /// 차량의 다양한 움직임 상태에 따라 오디오를 재생하는 클래스입니다.
    /// </summary>
    public class CarEngineAudio : MonoBehaviour
    {
        [Tooltip("카트가 시작할 때 재생할 오디오 클립")]
        public AudioSource StartSound;
        [Tooltip("카트가 아무것도 하지 않을 때 재생할 오디오 클립")]
        public AudioSource IdleSound;
        [Tooltip("카트가 움직일 때 재생할 오디오 클립")]
        public AudioSource RunningSound;
        [Tooltip("카트가 드리프트 중일 때 재생할 오디오 클립")]
        public AudioSource Drift;
        [Tooltip("최고 속도에서 러닝 사운드의 최대 볼륨")]
        [Range(0.1f, 1.0f)] public float RunningSoundMaxVolume = 1.0f;
        [Tooltip("최고 속도에서 러닝 사운드의 최대 피치")]
        [Range(0.1f, 2.0f)] public float RunningSoundMaxPitch = 1.0f;
        [Tooltip("카트가 후진할 때 재생할 오디오 클립")]
        public AudioSource ReverseSound;
        [Tooltip("최고 후진 속도에서 리버스 사운드의 최대 볼륨")]
        [Range(0.1f, 1.0f)] public float ReverseSoundMaxVolume = 0.5f;
        [Tooltip("최고 후진 속도에서 리버스 사운드의 최대 피치")]
        [Range(0.1f, 2.0f)] public float ReverseSoundMaxPitch = 0.6f;

        private CarController carController;

        void Awake()
        {
            carController = GetComponentInParent<CarController>();
        }

        void Update()
        {
            float carSpeed = 0.0f;
            if (carController != null)
            {
                // KmH를 그대로 사용
                carSpeed = carController.KmH;

                // 드리프트 여부와 볼륨 계산
                Drift.volume = carController.isDrifting && carController.IsGrounded()
                               ? carSpeed / 500f  // 최대 속도를 500으로 가정
                               : 0.0f;
            }

            IdleSound.volume = Mathf.Lerp(0.6f, 0.0f, carSpeed * 4);

            if (carSpeed < 0.0f)
            {
                // 후진 중
                RunningSound.volume = 0.0f;
                ReverseSound.volume = Mathf.Lerp(0.1f, ReverseSoundMaxVolume, -carSpeed * 1.2f);
                ReverseSound.pitch = Mathf.Lerp(0.1f, ReverseSoundMaxPitch, -carSpeed + (Mathf.Sin(Time.time) * .1f));
            }
            else
            {
                // 전진 중
                ReverseSound.volume = 0.0f;
                RunningSound.volume = Mathf.Lerp(0.1f, RunningSoundMaxVolume, carSpeed * 1.2f);
                RunningSound.pitch = Mathf.Lerp(0.3f, RunningSoundMaxPitch, carSpeed + (Mathf.Sin(Time.time) * .1f));
            }
        }
    }
}
