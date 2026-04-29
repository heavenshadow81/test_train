using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ML.SportsMiniGame.KinectSkating;
/*namespace ML.SportsMiniGame.KinectSkating
{*/
    public class TrailAnimation : MonoBehaviour
    {
        public TrailCreater leftTrail;
        public TrailCreater rightTrail;

        public void OnLeftCheck(bool ch)
        {
            leftTrail.OnGroundCheck(ch);
        }
        public void OnRightCheck(bool ch)
        {
            rightTrail.OnGroundCheck(ch);
        }

    }
//}

