using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StairGame
{
    public class AnimCheckBox : MonoBehaviour
    {      
        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                GameManager.Instance.StageCount++;
                Debug.Log(GameManager.Instance.StageCount);

                Player player = other.GetComponent<Player>();

                player.StopJumpAnim();

                player.Rigid.position = transform.position;

                player.CurrentPlayerPos = player.Rigid.position;

                player.MoveCameraToTarget(() =>
                {
                    player.GoToMagicCircle();
                });              
            }
        }
    }
}
