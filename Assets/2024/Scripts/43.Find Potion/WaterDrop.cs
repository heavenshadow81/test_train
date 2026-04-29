using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FindPotion
{
    public class WaterDrop : MonoBehaviour
    {
        private Vector3 originalPosition;    // 초기 위치 저장
        private Quaternion originalRotation;  // 초기 회전 값 저장
        public bool isAnswer = false;
        [SerializeField] private Potion potion;

        private void Awake()
        {
            originalPosition = transform.localPosition;
            originalRotation = transform.localRotation;
        }

        private void OnEnable()
        {
            // 현재 로컬의 다운 방향을 Vector3.down 방향으로 맞추도록 회전값을 계산
            Quaternion targetRotation = Quaternion.FromToRotation(-transform.up, Vector3.down) * transform.rotation;

            // 회전값을 적용하여 로컬 다운 방향을 월드의 다운 방향과 맞춤
            transform.rotation = targetRotation;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.name.Contains("Potion Cube"))
            {               
                ResetPosition();
            }
        }

        public void ResetPosition()
        {
            transform.localPosition = originalPosition;
            transform.localRotation = originalRotation;
            gameObject.SetActive(false);

            potion.ResetPosition(() =>
            {
                PotionManager.Instance.OnPotionResetComplete(); // 포션 리셋 완료 시 호출      
            });
        }
    }
}

