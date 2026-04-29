using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.SportsMiniGame.KinectSkating
{
    public class IntroCameraTrigger : MonoBehaviour
    {
        public Animation IntroCameraAnim;
        public Animation IntroGUIAnimation;
        public GUIManager guimanager;
        //로그 투명화
        public void LogoOut()
        {
            IntroGUIAnimation.Play("LogoOut");
            StartCoroutine(WaitForSecondInCorutain(1f));
        }
        IEnumerator WaitForSecondInCorutain(float sec)
        {
            yield return new WaitForSeconds(sec);
            guimanager.SetUserDetect();
            //guimanager.SetExplanation();
        }
        public void Search()
        {
            IntroCameraAnim.Play("UserSearch");
        }
    }
}
