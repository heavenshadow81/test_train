using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Penguin
{
    public class MoveCheck : MonoBehaviour
    {
        public bool check;

        private void OnEnable()
        {
            check = false;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.tag != "Player" && other.tag != "Finish")
                check = true;

        }
    }
}
