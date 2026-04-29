using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KartRider
{
    public class CarAudio : MonoBehaviour
    {
        // 이 스크립트는 차량의 현재 속성을 읽고 이에 따라 소리를 재생합니다.
        // 엔진 소리는 단일 클립을 반복 재생하거나 RPM 및 스로틀 상태에 따라
        // 네 가지 클립을 교차 페이드로 믹싱하여 재생할 수 있습니다.

        // 엔진 클립은 일정한 음높이를 유지하며 상승 또는 하강하지 않아야 합니다.

        // 네 가지 클립을 사용하는 경우:
        // lowAccelClip : 낮은 RPM에서 스로틀이 열렸을 때 (즉, 낮은 속도에서 가속 시작)
        // highAccelClip : 높은 RPM에서 스로틀이 열렸을 때 (즉, 거의 최대 속도로 가속)
        // lowDecelClip : 낮은 RPM에서 스로틀이 닫혔을 때 (즉, 낮은 속도에서 엔진 브레이크)
        // highDecelClip : 높은 RPM에서 스로틀이 닫혔을 때 (즉, 높은 속도에서 엔진 브레이크)

        // 교차 페이드를 올바르게 수행하려면 클립의 피치가 일치해야 하며,
        // 저음과 고음 사이에 옥타브 차이가 있어야 합니다.

        public enum EngineAudioOptions // 엔진 오디오 옵션
        {
            Simple,      // 간단한 스타일의 오디오
            FourChannel  // 네 가지 채널의 오디오
        }
        [SerializeField] private EngineAudioOptions engineSoundStyle = EngineAudioOptions.FourChannel; // 기본 오디오 옵션을 네 가지 채널로 설정

        [Header("오디오 속성 설정")]
        [SerializeField] private float pitchMultiplier = 1f;       // 오디오 클립의 피치를 변경하기 위한 값
        [SerializeField] private float lowPitchMin = 1f;           // 낮은 소리의 최소 피치
        [SerializeField] private float lowPitchMax = 6f;           // 낮은 소리의 최대 피치
        [SerializeField] private float highPitchMultiplier = 0.25f; // 높은 소리의 피치를 조정하는 값
        [SerializeField] private float maxRolloffDistance = 500f;  // 최대 롤오프 거리
        [SerializeField] private float dopplerLevel = 1f;          // 도플러 효과의 강도
        [SerializeField] private bool useDoppler = true;           // 도플러 효과 사용 여부

        [Header("오디오 클립")]
        [SerializeField] private AudioClip lowAccelClip;  // 낮은 가속 사운드의 AudioSource
        [SerializeField] private AudioClip lowDecelClip; // 높은 가속 사운드의 AudioSource
        [SerializeField] private AudioClip highAccelClip; // 높은 가속 사운드의 AudioSource
        [SerializeField] private AudioClip highDecelClip; // 높은 가속 사운드의 AudioSource

        [Header("오디오 소스")]
        [SerializeField] private AudioSource lowAccelSource;  // 낮은 가속 사운드의 AudioSource
        [SerializeField] private AudioSource lowDecelSource; // 높은 가속 사운드의 AudioSource
        [SerializeField] private AudioSource highAccelSource; // 높은 가속 사운드의 AudioSource
        [SerializeField] private AudioSource highDecelSource; // 높은 가속 사운드의 AudioSource

        [Header("참조 컴포넌트")]
        [SerializeField] private CarController carController; // 차량 컨트롤러 참조
        [SerializeField] private InputManager inputManager;   // 입력 매니저 참조

        private bool isStartedSound; // 사운드가 재생 중인지 여부 플래그
        //private AIcontroller aicontroler;  // AI 컨트롤러 참조

        private void StartSound()
        {
            // 엔진 소리를 초기화하고 시작합니다.
            highAccelSource = SetUpEngineAudioSource(highAccelClip);

            // 네 가지 채널 오디오를 사용하는 경우
            if (engineSoundStyle == EngineAudioOptions.FourChannel)
            {
                lowAccelSource = SetUpEngineAudioSource(lowAccelClip);
                lowDecelSource = SetUpEngineAudioSource(lowDecelClip);
                highDecelSource = SetUpEngineAudioSource(highDecelClip);
            }

            // 사운드가 시작되었음을 표시
            isStartedSound = true;
        }

        private void StopSound()
        {
            // 오브젝트에 연결된 모든 AudioSource를 제거합니다.
            foreach (var source in GetComponents<AudioSource>())
            {
                Destroy(source);
            }

            isStartedSound = false;
        }

        private void FixedUpdate()
        {
            // 메인 카메라와의 거리 계산
            float camDist = (Camera.main.transform.position - transform.position).sqrMagnitude;

            // 최대 롤오프 거리보다 멀어지면 사운드 정지
            if (isStartedSound && camDist > maxRolloffDistance * maxRolloffDistance)
            {
                StopSound();
            }

            // 최대 거리 이내로 가까워지면 사운드 시작
            if (!isStartedSound && camDist < maxRolloffDistance * maxRolloffDistance)
            {
                StartSound();
            }

            if (isStartedSound)
            {
                // 차량 RPM에 따라 피치 계산
                float pitch = ULerp(lowPitchMin, lowPitchMax, carController.engineRPM / carController.maxRPM);

                // 최대 피치를 초과하지 않도록 제한
                pitch = Mathf.Min(lowPitchMax, pitch);

                if (engineSoundStyle == EngineAudioOptions.Simple)
                {
                    // 단일 채널 엔진 사운드
                    highAccelSource.pitch = pitch * pitchMultiplier * highPitchMultiplier;
                    highAccelSource.dopplerLevel = useDoppler ? dopplerLevel : 0;
                    highAccelSource.volume = 1;
                }
                else
                {
                    // 네 가지 채널 엔진 사운드

                    // 피치 조정
                    lowAccelSource.pitch = pitch * pitchMultiplier;
                    lowDecelSource.pitch = pitch * pitchMultiplier;
                    highAccelSource.pitch = pitch * highPitchMultiplier * pitchMultiplier;
                    highDecelSource.pitch = pitch * highPitchMultiplier * pitchMultiplier;

                    // 가속 및 감속 페이드 값 계산
                    float accFade = Mathf.Abs((carController.vertical > 0 && !carController.test) ? carController.vertical : 0);
                    float decFade = 1 - accFade;

                    // 고속 및 저속 페이드 값 계산
                    float highFade = Mathf.InverseLerp(0.2f, 0.8f, carController.engineRPM / carController.maxRPM);
                    float lowFade = 1 - highFade;

                    // 값 조정
                    highFade = 1 - Mathf.Pow(1 - highFade, 2);
                    lowFade = 1 - Mathf.Pow(1 - lowFade, 2);
                    accFade = 1 - Mathf.Pow(1 - accFade, 2);
                    decFade = 1 - Mathf.Pow(1 - decFade, 2);

                    // 볼륨 조정
                    lowAccelSource.volume = lowFade * accFade;
                    lowDecelSource.volume = lowFade * decFade;
                    highAccelSource.volume = highFade * accFade;
                    highDecelSource.volume = highFade * decFade;

                    // 도플러 효과 조정
                    highAccelSource.dopplerLevel = useDoppler ? dopplerLevel : 0;
                    lowAccelSource.dopplerLevel = useDoppler ? dopplerLevel : 0;
                    highDecelSource.dopplerLevel = useDoppler ? dopplerLevel : 0;
                    lowDecelSource.dopplerLevel = useDoppler ? dopplerLevel : 0;
                }
            }
        }

        private AudioSource SetUpEngineAudioSource(AudioClip clip)
        {
            // 새 AudioSource 생성 및 초기화
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = 0;
            source.spatialBlend = 1;
            source.loop = true;
            source.time = Random.Range(0f, clip.length); // 랜덤 지점에서 시작
            source.Play();
            source.minDistance = 5;
            source.maxDistance = maxRolloffDistance;
            source.dopplerLevel = 0;
            return source;
        }

        private static float ULerp(float from, float to, float value)
        {
            // 범위를 초과할 수 있는 Lerp 함수
            return (1.0f - value) * from + value * to;
        }
    }
}
