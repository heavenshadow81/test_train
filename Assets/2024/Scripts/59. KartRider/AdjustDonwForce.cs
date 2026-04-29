using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartRider
{
    public class AdjustDonwForce : MonoBehaviour
    {
        [SerializeField] private float desiredDownForceValue = 200f; // 변경할 다운포스 힘

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("KartBody"))
            {
                CarController car = other.GetComponentInParent<CarController>();
                if (car != null)
                {
                    car.downForceValue = desiredDownForceValue;
                }
            }
        }
    }
}
