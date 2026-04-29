using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartRider
{
    public class InputManager : MonoBehaviour
    {
        public float Vertical { get; set; }
        public float Horizontal { get; set; }
        public bool HandBrake { get; set; }
        public bool Boosting { get; set; }

        private CarController carController;
        private float targetHorizontal = 0f; 
        private float changeSpeed = 10f;

        private void Start()
        {
            carController = FindObjectOfType<CarController>();
        }

        private void FixedUpdate()
        {
            //Vertical = Input.GetAxis("Vertical");
            //Horizontal = Input.GetAxis("Horizontal");
            //HandBrake = (Input.GetAxis("Jump") != 0) ? true : false;

            //if (Input.GetKey(KeyCode.LeftShift))
            //{
            //    Boosting = true;
            //}
            //else
            //{
            //    Boosting = false;
            //}

            // Horizontal을 targetHorizontal로 부드럽게 변경
            Horizontal = Mathf.MoveTowards(Horizontal, targetHorizontal, Time.fixedDeltaTime * changeSpeed);
            carController.horizontal = Horizontal;
        }

        public void LeftButtonDown()
        {
            targetHorizontal = -1;
        }

        public void LeftButtonUp()
        {
            targetHorizontal = 0;
        }

        public void RightButtonDown()
        {
            targetHorizontal = 1;
        }

        public void RightButtonUp()
        {
            targetHorizontal = 0;
        }
    }
}
