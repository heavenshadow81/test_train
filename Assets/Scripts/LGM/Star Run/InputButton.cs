using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LGM
{
    namespace StarRun
    {
        public class InputButton : MonoBehaviour, IPointerDownHandler
        {
            private AudioSource audioSource;

            private void Awake()
            {
                audioSource = GetComponent<AudioSource>();
            }

            // 오브젝트 클릭(다운) 시
            public void OnPointerDown(PointerEventData eventData)
            {
                audioSource.PlayOneShot(audioSource.clip);  // 사운드 재생
                StarController.Instance.revers *= -1;   // 방향 반전
            }
        }
    }
}

