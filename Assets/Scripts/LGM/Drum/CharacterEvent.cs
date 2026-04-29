using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LGM
{
    namespace Drum
    {
        public class CharacterEvent : MonoBehaviour
        {
            private Animator ani;
            private AudioSource audioSource;
            public GameObject onAir;

            private void Awake()
            {
                ani = GetComponent<Animator>();
                audioSource = GetComponent<AudioSource>();
            }

            public void AnimationPlay()
            {
                ani.SetTrigger("AniTrigger");   // AniTrigger활성화
            }

            // 소리 재생(애니메이션 시작 시 실행)
            public void PlaySuound()
            {
                if (audioSource.clip != null)
                {
                    audioSource.Play();
                }
            }
            
            // OnAir오브젝트 활성화/비활성화 (애니메이션 시작과 끝에 실행)
            public void ActiveOnAir(int active)
            {
                onAir.SetActive(active == 0 ? false : true);
            }
        }
    }
}

