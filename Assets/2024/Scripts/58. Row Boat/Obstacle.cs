using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RowBoat
{
    public class Obstacle : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                SoundMGR.Instance.SoundPlay("Obstacle");
            }
        }
    }
}
