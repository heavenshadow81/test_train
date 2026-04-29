using ML.T_Sports.ShotPut;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class coinManager : MonoBehaviour
{
    public GameObject scoreText;
    public GameObject effect;
    public int score;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "target")
        {
            scoreText.GetComponent<ShotPutScoreManager>().MyScore += score;
            Instantiate(effect,gameObject.transform);
            gameObject.GetComponent<BoxCollider>().enabled = false;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
