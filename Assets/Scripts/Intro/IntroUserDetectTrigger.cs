using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class IntroUserDetectTrigger : MonoBehaviour
    {
        public GUIManager guimanager;
        public IntroManager intro;
        public void DetectUser()
        {
            KinectSkateManager.instance.playstate = PlayState.UserSearch;
            guimanager.UserDetectStart();
        }
        public void SetGameReady()
        {
            intro.GameReady();
            //guimanager.SetExplanation();
        }
    }
}

