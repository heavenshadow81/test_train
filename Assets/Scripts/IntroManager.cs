using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class IntroManager : MonoBehaviour
    {
        public GameObject IntroCameraObj;
        public GameObject PlayCameraObj;
        public CameraWork PlayCameraWork;

        public GUIManager guimanager;
        public Animation ReadyAnimation;
        public GameObject[] AI;
        public GameObject PlayerObj;
        public GameObject AIObj;
        public AudioSource UserCheckSound;
        public ReadyGoTrigger readygotrigger;
        private void Start()
        {
            PlayerObj.SetActive(false);
            AIObj.SetActive(false);
        }
        
        public void InitState()
        {
            PlayerObj.SetActive(false);
            AIObj.SetActive(false);
            IntroCameraObj.SetActive(true);
            PlayCameraObj.SetActive(false);
            PlayCameraWork.CameraMoveInit();

        }
        public void GameReady()
        {
            UserCheckSound.Play();
            PlayerObj.SetActive(true);
            AIObj.SetActive(true);
            for (int i = 0; i < AI.Length; i++)
                AI[i].SetActive(false);
            IntroCameraObj.SetActive(false);
            PlayCameraObj.SetActive(true);
            PlayCameraWork.CameraMovingStart();
            guimanager.SetGamePlay();
            KinectSkateManager.instance.playstate = PlayState.Ready;
            StartCoroutine(ReadyAnimationPlay());
        }
        IEnumerator ReadyAnimationPlay()
        {
            yield return new WaitForSeconds(2f);
            PlayerObj.SetActive(true);
            ReadyAnimation.Play("Ready");
        }
        public void GoAnimationStart()
        {
            if(readygotrigger.ImReady)
            {
                ReadyAnimation.Play("Go");
            }
               
        }
    }
}

