using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartRider
{
    public class KartBodyCollision : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Track"))
            {
                SoundMGR.Instance.SoundPlay("Collision");
            }
        }
    }
}
