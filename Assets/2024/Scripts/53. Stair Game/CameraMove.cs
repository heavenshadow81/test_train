using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

namespace StairGame
{
    public class CameraMove : MonoBehaviour
    {
        private Vector3 initialOffset;

        // 초기 오프셋을 설정
        public void InitializeOffset(Vector3 playerPosition)
        {
            initialOffset = transform.position - playerPosition;
        }

        // 카메라 이동 함수
        public void MoveCam(Vector3 targetPos, Action onComplete = null)
        {
            transform.DOMove(initialOffset + targetPos, 1.5f).OnComplete(() => onComplete?.Invoke());
        }
    }
}
