using ML.PlaywallKids.Aquarium;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartRider
{
    public class EndingAnim : MonoBehaviour
    {
        Transform car;
        GameObject endingUI;

        private void Start()
        {
            Empty script = FindObjectOfType<Empty>();
            endingUI = script.gameObject;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("KartBody"))
            {
                car = other.transform.parent.transform;
                Animator anim = car.GetComponent<Animator>();

                anim.SetTrigger("close");
                anim.enabled = true;

                gameObject.transform.GetComponent<BoxCollider>().enabled = false;           
            }
        }

        public void CamNoneFollow()
        {
            Camera cam = Camera.main;

            car.GetComponent<CarAnimation>().CameraTrue();
            cam.transform.position = new Vector3(1951, 263, -9360);
            cam.transform.rotation = Quaternion.Euler(357, 282, 0);

            endingUI.transform.GetChild(0).gameObject.SetActive(true);
            car.GetComponent<CarEffect>().enabled= false;
            car.GetComponent<CarController>().enabled = false;

            cam.fieldOfView = 60;
        }
    }
}
