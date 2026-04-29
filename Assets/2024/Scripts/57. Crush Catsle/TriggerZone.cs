using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrushCatsle
{
    public class TriggerZone : MonoBehaviour
    {
        private HashSet<GameObject> targetsInZone = new HashSet<GameObject>();

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("target"))
            {
                targetsInZone.Add(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("target"))
            {
                targetsInZone.Remove(other.gameObject);

                if (targetsInZone.Count == 0)
                {
                    GameManager.Instance.SpawnCandy();
                }
            }
        }
    }
}
