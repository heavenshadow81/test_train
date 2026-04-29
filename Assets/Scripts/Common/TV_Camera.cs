using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ML.SportsMiniGame.KinectSkating;
namespace ML.SportsMiniGame.Common
{
    public class TV_Camera : MonoBehaviour
    {

        public Transform mytr;
        public Transform target;
        public Transform PlayerPos;
        public GameObject PlayerObj;
        public bool OnParents;
        // Use this for initialization
        void Start()
        {
            mytr = this.transform;
            mytr.parent = null;
            OnParents = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (KinectSkateManager.instance.playstate == PlayState.Play && !OnParents)
            {
                OnParents = true;
                mytr.parent = PlayerPos;
            }
            if (target != null)
            {
                mytr.LookAt(target); 
            }

        }

        public void SetLookTarget(Vector3 pos, Transform looktarget)
        {
            mytr.parent = null;
            mytr.position = pos;
            target = looktarget;
            PlayerObj.SetActive(false);
        }
    }

}
