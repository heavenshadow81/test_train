using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class CrowdCratorNoma : MonoBehaviour
    {
        public GameObject Crowd_Y;
        public Transform Crowd_Start;
        public int CreateCount;
        public CrowdY mycrowdy;

        public bool[] SeatlessCheck;
        // Use this for initialization
        void Start()
        {
            //IntancingCrowd();
        }
        public void IntancingCrowd()
        {
            for (int i = 0; i < CreateCount; i++)
            {
                Transform MyCrowd = Instantiate(Crowd_Y, Crowd_Start.position, Crowd_Start.rotation).transform;
                MyCrowd.parent = Crowd_Start;

                Vector3 right = MyCrowd.TransformDirection(Vector3.right);
                //Debug.Log(this.transform.name+"/"+right);
                Vector3 tmp = MyCrowd.position;
                tmp -= right * (1.5f * i);
                //tmp.z -= (1.5f * i);
                MyCrowd.position = tmp;
                mycrowdy = MyCrowd.GetComponent<CrowdY>();
            }
        }
    }
}
 
