using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FindPotion
{
    public class PotionCube : MonoBehaviour
    {
        [SerializeField] private GameObject[] waterDropEffects;
        [SerializeField] private GameObject successEffect;
        [SerializeField] private GameObject[] FinalEffects;
        [SerializeField] private GameObject failEffect;
        [SerializeField] private MagicLife life;
        private int effectIdx;
        private int successCondition = 5;
        [SerializeField] private int failCount = 0;

        public void SetWaterDropEffect()
        {
            effectIdx = Random.Range(0, waterDropEffects.Length);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name.Contains("WaterDrop"))
            {
                if (!GameManager.Instance.isCheckAnswer)
                {
                    SoundMGR.Instance.SoundPlay("WaterDrop");
                    // 정답인거 체크해서 이펙트등 표현해주기
                    if (other.GetComponent<WaterDrop>().isAnswer)
                    {
                        ActivateWaterDropEffect(effectIdx, 2.0f).Forget(); // 2초 후 이펙트 비활성화                     
                    }
                }
                else
                {
                    // 정답인거 체크해서 이펙트등 표현해주기
                    if (other.GetComponent<WaterDrop>().isAnswer)
                    {
                        SoundMGR.Instance.SoundPlay("Answer");
                        GameManager.Instance.successStack++;

                        // successStack이 5 이전이면 ActivateSuccessEffect 호출
                        if (GameManager.Instance.successStack < successCondition)
                        {
                            transform.DOShakePosition(0.5f, 0.3f, vibrato: 10, randomness: 90, fadeOut: true);
                            ActivateSuccessEffect(1.0f, 2.0f).Forget();
                        }
                        // successStack이 5이면 ActivateFinishEffect 호출
                        else
                        {
                            ActivateFinishEffect(1.0f, 2.0f).Forget();
                        }
                    }
                    else
                    {
                        SoundMGR.Instance.SoundPlay("Wrong");
                        transform.DOShakePosition(0.5f, 0.3f, vibrato: 10, randomness: 90, fadeOut: true);
                        ActivateFailEffect(1.5f).Forget();
                    }

                    GameManager.Instance.isCheckAnswer = false;
                }

            }
        }

        // UniTask를 이용하여 효과를 활성화하고 일정 시간 후 비활성화하는 함수
        private async UniTask ActivateWaterDropEffect(int index, float delayTime)
        {
            waterDropEffects[index].SetActive(true);  // 이펙트 활성화
            await UniTask.Delay((int)(delayTime * 1000)); // 지연 시간 (밀리초 단위)
            waterDropEffects[index].SetActive(false); // 지연 시간 후 이펙트 비활성화
        }

        private async UniTask ActivateFailEffect(float delayTime, float delayTime2 = 0.5f)
        {
            failEffect.SetActive(true);  // 이펙트 활성화
            await UniTask.Delay((int)(delayTime * 1000)); // 지연 시간 (밀리초 단위)
            failEffect.SetActive(false); // 지연 시간 후 이펙트 비활성화
            await UniTask.Delay((int)(delayTime2 * 1000)); // 지연 시간 (밀리초 단위)
            life.LifeDelete(); // 목숨 1개 제거
            failCount++;
            if (failCount < 3)
            {
                PotionManager.Instance.Init(); // 포션 다시 세팅
            }
        }

        private async UniTask ActivateSuccessEffect(float delayTime, float delayTime2, float delayTime3 = 0.5f)
        {
            successEffect.SetActive(true);  // 첫번째 이펙트 활성화
            await UniTask.Delay((int)(delayTime * 1000)); // 지연 시간 (밀리초 단위)

            successEffect.SetActive(false); // 지연 시간 후 첫번째 이펙트 비활성화         
            PlayFinalEffect(true); // 지연 시간 후 두번째 이펙트 활성화
            await UniTask.Delay((int)(delayTime2 * 1000)); // 지연 시간 (밀리초 단위)

            SoundMGR.Instance.SoundStop("Final");
            PlayFinalEffect(false); ; // 지연 시간 후 두번째 이펙트 비활성화        
            await UniTask.Delay((int)(delayTime3 * 1000)); // 지연 시간 (밀리초 단위)

            PotionManager.Instance.Init(); // 포션 다시 세팅
        }

        private async UniTask ActivateFinishEffect(float delayTime, float delayTime2, float delayTime3 = 0.2f)
        {
            SoundMGR.Instance.SoundPlay("Clear");
            // 첫 번째 이펙트 활성화
            successEffect.SetActive(true);
            await UniTask.Delay((int)(delayTime * 1000)); // 첫 번째 이펙트 활성화 시간 대기

            successEffect.SetActive(false); // 첫 번째 이펙트 비활성화

            // 점프 없이 직접 공중으로 이동하고 부르르 떠는 애니메이션 추가
            await PlayFloatingAndShakingAnimation(1.0f, 0.3f); // floatDuration: 1초 동안 공중으로 이동, shakeStrength: 0.3의 부르르 떨림

            // PlayFinalEffect 실행
            PlayFinalEffect(true);
            await UniTask.Delay((int)(delayTime2 * 1000)); // 두 번째 이펙트 활성화 후 시간 대기

            PlayFinalEffect(false); // 두 번째 이펙트 비활성화
            await UniTask.Delay((int)(delayTime3 * 1000));

            GameManager.Instance.GameClear();
        }

        // 마법 큐브 애니메이션 함수
        private async UniTask PlayFloatingAndShakingAnimation(float floatDuration, float shakeStrength)
        {
            // 큐브가 공중으로 이동하는 애니메이션 (floatDuration 동안 위로 이동)
            await transform.DOMoveY(transform.position.y + 1.5f, floatDuration)
                           .SetEase(Ease.OutQuad)
                           .AsyncWaitForCompletion(); // 위로 이동할 때까지 대기

            // 공중에 떠있는 상태에서 부르르 떠는 애니메이션 실행
            _ = transform.DOShakePosition(0.5f, shakeStrength, vibrato: 10, randomness: 90, fadeOut: true);
        }

        private void PlayFinalEffect(bool isActive)
        {
            if (isActive)
            {
                SoundMGR.Instance.SoundPlay("Final");
            }

            for (int i = 0; i < GameManager.Instance.successStack; i++)
            {
                FinalEffects[i].SetActive(isActive);
            }
        }
    }
}


