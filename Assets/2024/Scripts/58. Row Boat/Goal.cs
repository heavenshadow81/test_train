using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RowBoat
{
    public class Goal : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                SoundMGR.Instance.SoundPlay("Goal");
                GameManager.Instance.GameClear();
            }
        }
    }
}
