using ML.PlaywallKids.Aquarium;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartRider
{
    public class CarAnimation : MonoBehaviour
    {
        Animator animator;
        [SerializeField] CameraController cam;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Opening()
        {
            animator.SetTrigger("open");
        }

        public void AnimEnable()
        {
            animator.enabled = !animator.enabled;
        }

        public void CameraTrue()
        {
            cam.PlayChange();
        }

        public void Ending()
        {
            EndingAnim ending = FindObjectOfType<EndingAnim>();
            ending.CamNoneFollow();
        }
    }
}
