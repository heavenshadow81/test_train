using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CakeCheck : MonoBehaviour
{
    [SerializeField] CS_GameManager manager;

    private void Start()
    {
        if (manager == null)
        {
            manager = FindObjectOfType<CS_GameManager>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Feed")
        {
            collision.transform.tag = "Finish";
            manager.CameraUp();
            
        }
    }
}

