using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace FindPotion
{
    public class Potion : MonoBehaviour
    {
        private float duration = 1f;      // 애니메이션 지속 시간
        private Vector3 originalPosition;    // 초기 위치 저장
        private Quaternion originalRotation;  // 초기 회전 값 저장
        [SerializeField] private WaterDrop waterDrop;

        private void Awake()
        {
            // 초기 위치와 회전 값을 저장
            originalPosition = transform.position;
            originalRotation = transform.rotation;
        }

        public void MovePotion(Transform target,TweenCallback onCompleteCallback = null)
        {
            // 포션을 이동시키고, 회전도 함께 수행
            transform.DOMove(target.position, duration).OnComplete(() =>
            {
                transform.DORotateQuaternion(target.rotation, duration).OnComplete(() =>
                {
                    //waterDrop.gameObject.SetActive(true);  // WaterDrop 활성화
                    onCompleteCallback?.Invoke();   //애니메이션이 모두 끝난 후 콜백 호출
                });
            });
        }

        public void ResetPosition(TweenCallback onCompleteCallback = null)
        {
            // Sequence를 사용하여 위치와 회전을 동시에 애니메이션 적용
            Sequence resetSequence = DOTween.Sequence();

            // 위치와 회전을 동시에 수행
            resetSequence.Join(transform.DOMove(originalPosition, duration))
                         .Join(transform.DORotateQuaternion(originalRotation, duration))
                         .OnComplete(() =>
                         {
                             onCompleteCallback?.Invoke();  //애니메이션이 모두 끝난 후 콜백 호출
                         });
        }

        public void SetAnswer()
        {
            waterDrop.isAnswer = true;
        }

        public void ResetAnswer()
        {
            waterDrop.isAnswer = false;
        }

        public void ActivateWaterDrop()
        {
            // 물방울이 존재하면 활성화
            if (waterDrop != null)
            {
                waterDrop.gameObject.SetActive(true);
            }
        }
    }
}