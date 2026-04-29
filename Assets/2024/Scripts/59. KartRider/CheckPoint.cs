using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartRider
{
    public class CheckPoint : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("KartBody"))
            {
                CarController car = other.GetComponentInParent<CarController>();

                car.resetPoint = transform;
            }
        }
    }
}
