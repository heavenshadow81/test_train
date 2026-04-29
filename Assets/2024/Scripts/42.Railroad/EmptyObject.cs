using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Railroad
{
    public class EmptyObject : MonoBehaviour
    {
        public GameObject emptyRail;
        public GameObject leftRail;
        public GameObject straightRail;
        public GameObject rightRail;
        public GameObject questionMark;

        public void DeActiveEmptyRail()
        {
            emptyRail.SetActive(false);
        }
    }
}

