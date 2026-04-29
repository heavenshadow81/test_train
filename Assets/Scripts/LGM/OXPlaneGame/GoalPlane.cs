using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGM
{
    namespace OXPlaneGame
    {
        public class GoalPlane : PlaneEvent
        {
            private Animator ani;
            public AudioClip openSound;

            protected override void Awake()
            {
                base.Awake();
                ani = GetComponent<Animator>();
            }
            protected override void PlaneDownEvent()
            {
                if (!manager.clickRock && manager.getKey) 
                {
                    base.PlaneDownEvent();
                    audioSource.Play();
                    ani.SetTrigger("Open Treasure");
                }
            }
            public void GameClear()
            {
                GameManager.Instance.GameClear();
            }
            public void ChangeAudioSound(AudioClip clip)
            {
                audioSource.loop = false;   // 루프 잠금
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }
}

