using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartRider
{
    public class ResetZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("KartBody"))
            {
                CarController car = other.GetComponentInParent<CarController>();

                car.ResetToLastCheckpoint();
            }
        }
    }
}
