using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.SportsMiniGame.KinectSkating
{
    public class IntroCamLookCenter : MonoBehaviour
    {
        public Transform LookingTarget;
        public Transform IntroCam;
        public float speed;

        void Update()
        {
            if (KinectSkateManager.instance.playstate == PlayState.Explanation || KinectSkateManager.instance.playstate == PlayState.UserSearch)
                IntroCam.LookAt(LookingTarget);

        }
    }
}

