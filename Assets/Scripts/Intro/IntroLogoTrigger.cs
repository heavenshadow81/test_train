using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.SportsMiniGame.KinectSkating
{
    public class IntroLogoTrigger : MonoBehaviour
    {
        public Animation IntroCameraAnim;
        public GameObject IntroExplanation;

        
        public void SetExplanation()
        {
            IntroCameraAnim.Play("UserSearchStart");
            //IntroExplanation.SetActive(true);
            KinectSkateManager.instance.playstate = PlayState.UserSearch;
        }
    }
}
