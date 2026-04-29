using System.Collections;
using UnityEngine;

namespace KartRider
{
    public class BoostZone : MonoBehaviour
    {
        [SerializeField] private float boostForce = 700f; // 부스트 힘
        [SerializeField] private float boostDuration = 3f; // 부스트 지속 시간

        private float remainingBoostTime = 0f; // 남은 부스트 시간

        private CarEffect carEffect; // 부스트 효과
        private CarController car; // 카트 컨트롤러

        private void Start()
        {
            car = FindObjectOfType<CarController>();
            carEffect = FindObjectOfType<CarEffect>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("KartBody"))
            {
                if (car != null)
                {
                    remainingBoostTime = boostDuration;

                    SoundMGR.Instance.SoundRePlay("Booster");

                    car.ActivateBoost(boostForce, remainingBoostTime);
                }
            }
        }

        private void FixedUpdate()
        {
            // 부스트가 활성 상태일 때만 처리
            if (car.boostFlag)
            {
                if (remainingBoostTime > 0f)
                {
                    remainingBoostTime -= Time.fixedDeltaTime; // 남은 부스트 시간 감소
                }
            }    
        }
    }
}
