using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class ReadyGoTrigger : MonoBehaviour
    {
        public AudioSource ReadySound;
        public AudioSource GoSound;
        public AudioSource GunFire;
        public Animator PlayerAnim;
        public Animator AIAnim;
        public BGMAudioFade IntroBGM, PlayBGM;
        public bool ImReady;
        public IntroManager intro;
        

        void Awake()
        {
            ImReady = false;
        }
        public void SetImReady()
        {
            ImReady = true;
            StartCoroutine(ReadyGo());
        }
        IEnumerator ReadyGo()
        {
            yield return new WaitForSeconds(2);
            intro.GoAnimationStart();
        }
        public void BGMChange()
        {
            IntroBGM.VoluemeFadeDownStart();
        }
        public void ReadyVoicePlay()
        {
            PlayerAnim.SetTrigger("Ready");
            AIAnim.SetTrigger("Ready");
            KinectSkateManager.instance.playstate = PlayState.Ready;

            ReadySound.Play();
        }
        public void GoVoicePlay()
        {
            PlayerAnim.SetTrigger("Play");
            AIAnim.SetTrigger("Play");
            GoSound.Play();
            GunFire.Play();
            KinectSkateManager.instance.playstate = PlayState.Play;
            PlayBGM.VoluemeFadeUpStart();
        }
    }
}


