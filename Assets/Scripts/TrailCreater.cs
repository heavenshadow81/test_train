using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class TrailCreater : MonoBehaviour
    {
        public Transform mytr;
        public GameObject Trails;
        public Transform trail;
        public GameObject Prevtrail;
        public bool OnGround;
        void Start()
        {
            OnGround = false;
            mytr = this.transform;
        }
        private void FixedUpdate()
        {
            CheckingTrailPositon();
        }
        public void CheckingTrailPositon()
        {
            float y = mytr.transform.position.y;
            if (y < 0.05f)
            {
                OnGroundCheck(true);
            }
            else if (y > 0.5f)
            {
                OnGroundCheck(false);
            }
            if (trail != null && OnGround)
            {
                Vector3 tmp = mytr.position;
                tmp.y = 0.02f;
                trail.position = tmp;
            }
        }
        public void OnGroundCheck(bool check)
        {
            if (!OnGround && check)
            {
                if (trail != null)
                {
                    Prevtrail = trail.gameObject;
                    Destroy(Prevtrail, 1f);
                }                    
                trail = Instantiate(Trails, mytr.position, Quaternion.identity).transform;
                Vector3 tmp = trail.position;
                tmp.y = 0.05f;
                trail.position = tmp;
            }

            OnGround = check;
        }

    }
}
