using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DOTween 네임스페이스 추가

namespace Railroad
{
    public class BlinkingEffect : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;  // 깜빡임 효과를 줄 메쉬 렌더러
        [SerializeField] private Color blinkColor = Color.white;  // 깜빡임 색상 (기본값: 화이트)
        [SerializeField] private float blinkDuration = 0.5f;  // 깜빡이는 시간 간격 (초)

        private Material material;       // 메테리얼 변수
        private Color originalColor;     // 원래의 메테리얼 색상

        private void Start()
        {
            if (meshRenderer == null)
                meshRenderer = GetComponentInChildren<MeshRenderer>();

            // 메테리얼과 원래의 색상 저장
            material = meshRenderer.material;
            originalColor = material.color;

            // DOTween 초기화 (프로젝트 전체에서 한 번만 호출하면 됨)
            DOTween.Init();

            // 깜빡임 효과 시작
            StartBlinkingEffect();
        }

        private void StartBlinkingEffect()
        {
            // DOTween을 사용하여 색상 변화를 반복적으로 실행
            material.DOColor(blinkColor, blinkDuration)  // 깜빡임 색상으로 변경
                    .SetLoops(-1, LoopType.Yoyo)         // 무한 반복 및 Yoyo(원래 색상과 깜빡임 색상을 번갈아가며 반복)
                    .SetEase(Ease.InOutSine);            // 부드러운 전환
        }

        // 스크립트 종료 시 메테리얼 원상 복구 (참조 유지 방지)
        private void OnDisable()
        {
            // DOTween에서 모든 애니메이션을 중지하고 메테리얼 색상을 원래 색상으로 되돌림
            DOTween.Kill(material);
            if (material != null)
            {
                material.color = originalColor;
            }
        }
    }
}
