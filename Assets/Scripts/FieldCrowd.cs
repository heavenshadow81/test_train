using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class FieldCrowd : MonoBehaviour
    {
        public Animator MyAnim;
        public SkinnedMeshRenderer MyMesh;
        public Material[] mats;
        public int animationCnt;
        private void Awake()
        {
            MyAnim = this.GetComponent<Animator>();
            MyMesh = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
            MyMesh.material = mats[Random.Range(0,mats.Length)];
            RandomSitAnimation();
        }
        public void RandomSitAnimation()
        {
            int rand = Random.Range(0, animationCnt);
            switch (rand)
            {
                case 0:
                    MyAnim.SetTrigger("SitAnim1");
                    break;
                case 1:
                    MyAnim.SetTrigger("SitAnim2");
                    break;
                case 2:
                    MyAnim.SetTrigger("SitAnim3");
                    break;

            }
        }
    }
}
