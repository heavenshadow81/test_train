using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartRider
{
    public class DontDestroyObject : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void DestroyObject()
        {
            Destroy(gameObject);
        }
    }
}
