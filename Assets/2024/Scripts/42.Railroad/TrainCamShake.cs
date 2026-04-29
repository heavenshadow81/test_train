using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Splines;

namespace Railroad
{
    public class TrainCamShake : MonoBehaviour
    {
        [Header("기차 덜컹 애니메이션")]
        private float shakeIntensity = 0.1f;   // 덜컹거림의 강도
        private float shakeDuration = 0.1f;     // 덜컹거림 지속 시간
        private float minShakeInterval = 0.5f;  // 덜컹거림 최소 간격
        private float maxShakeInterval = 2f;    // 덜컹거림 최대 간격

        [SerializeField] CableCar cablecar;
        [SerializeField] GameObject backGround;
        [SerializeField] GameObject scoreImage;
        private void Start()
        {
            // 덜컹거리는 애니메이션을 무한 반복 실행
            StartCoroutine(ShakeEffect());
        }

        private void OnEnable()
        {
            backGround.SetActive(false);
            scoreImage.SetActive(true);
        }

        private void OnDisable()
        {
            backGround.SetActive(true);
            scoreImage.SetActive(false);
        }

        private IEnumerator ShakeEffect()
        {
            while (!cablecar.isSplineFinished)
            {
                // 간헐적인 덜컹거림을 위해 랜덤한 시간 간격 설정
                float randomInterval = Random.Range(minShakeInterval, maxShakeInterval);

                // 기다린 후 덜컹거리는 효과 실행
                yield return new WaitForSeconds(randomInterval);

                // 현재 위치를 기준으로 덜컹거리는 효과 적용
                transform.DOShakePosition(shakeDuration, shakeIntensity, vibrato: 10, randomness: 90, snapping: false, fadeOut: true)
                         .SetEase(Ease.InOutSine);
            }
        }
    }
}
