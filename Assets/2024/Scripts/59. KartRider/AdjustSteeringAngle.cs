using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

namespace KartRider
{
    public class AdjustSteeringAngle : MonoBehaviour
    {
        [SerializeField] float targetRadiusScale = 35f;

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("KartBody"))
            {
                CarController car = other.GetComponentInParent<CarController>();

                car.radiusScale = targetRadiusScale;
            }
        }
    }
}
