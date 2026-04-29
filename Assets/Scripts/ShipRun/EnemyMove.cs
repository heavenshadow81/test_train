using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ShipRun
{
    public class EnemyMove : MonoBehaviour
    {
        public GameObject end;
        float speed;
        PlayerMove player;

        private void Awake()
        {
            player = FindObjectOfType<PlayerMove>();
            speed = Random.Range(0.07f, 0.25f);
        }


        public void UpdateLogic()
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Ground")
            {
                //end.SetActive(true);

                player.stateClass.resultState = GameResult.Fail;
                player.zozo.Change(GameState.GameResult);
                //Time.timeScale = 0;
            }
        }
    }
}