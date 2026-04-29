using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ML.SportsMiniGame.KinectSkating;
namespace ML.SportsMiniGame.Common
{
    public class LookTarget : MonoBehaviour
    {
        public Transform mytr;
        public Transform target;
        public GameObject PlayerObj;
        // Use this for initialization
        void Start()
        {
            mytr = this.transform;
            if(target == null)
                target = GameObject.Find("[Camera]Intro_").transform.GetChild(0);
        }

        // Update is called once per frame
        void Update()
        {
            if (target != null)
            {
                if (KinectSkateManager.instance.playstate == PlayState.Play)
                {
                    float dist = Vector3.Distance(mytr.position, target.position);
                    if (dist < 50)
                        mytr.LookAt(target);
                    else
                        mytr.localRotation = Quaternion.identity;
                }
                else
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

