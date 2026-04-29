using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnderTheSea
{
    public class RemoveFish : MonoBehaviour
    {
       public void Remove()
        {
            Destroy(gameObject);
        }
    }
}