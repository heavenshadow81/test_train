using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KartRider
{
    public class CameraController : MonoBehaviour
    {
        private GameObject player;
        private CarController carController;
        private GameObject cameraConstraint, cameralookAt;
        private float speed = 0;
        private float defaultFOV = 0;
        private float desiredFOV = 0;
        [SerializeField, Range(0, 50)] float smoothTime = 9.5f;

        [SerializeField] TextMeshProUGUI timerText;
        [SerializeField] TextMeshProUGUI endingText;
        public float timer = 0;

        public bool isPalying = false;

        private void Awake()
        {
            //isPalying =true;
            player = GameObject.FindGameObjectWithTag("Player");
            carController = player.GetComponent<CarController>();
            cameralookAt = player.transform.Find("Camera Look At").gameObject;
            cameraConstraint = player.transform.Find("Camera Constraint").gameObject;
        }

        private void Update()
        {
            if (isPalying)
            {
                //타이머 계산
                timer += Time.deltaTime;

                int minutes = Mathf.FloorToInt(timer / 60f);
                int seconds = Mathf.FloorToInt(timer % 60f);
                int milliseconds = Mathf.FloorToInt((timer * 100) % 100);

                timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
                endingText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
            }
        }

        private void FixedUpdate()
        {
            if (isPalying)
            {
                Follow();
                BoostFOV();
            }
        }

        private void Follow()
        {
            //speed = Mathf.Lerp(speed, carController.KMH / 10, Time.deltaTime);
            speed = carController.KmH / smoothTime;

            transform.position = Vector3.Lerp(transform.position, cameraConstraint.transform.position, Time.deltaTime * speed);
            transform.LookAt(cameralookAt.transform.position);
        }

        public void BoostFOV()
        {
            if (carController.boostFlag)
            {
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, desiredFOV, Time.deltaTime * 5);
            }
            else
            {
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, defaultFOV, Time.deltaTime * 5);
            }
        }

        public void PlayChange()
        {
            isPalying = !isPalying;

#if UNITY_STANDALONE
            defaultFOV = Camera.main.fieldOfView + 30f;
            desiredFOV = defaultFOV + 30f;
#else
            defaultFOV = Camera.main.fieldOfView;
            desiredFOV = defaultFOV + 30f;
#endif

            Camera.main.fieldOfView = defaultFOV;
        }
    }
}

