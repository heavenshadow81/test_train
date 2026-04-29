using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touches : MonoBehaviour 
{
    [SerializeField]
    ParticleSystem particleSystem;
	void FixedUpdate()
    {
        if (Input.touchCount > 0)
        {
            Touch[] touches;

            touches = Input.touches;
            foreach(var touch in touches)
            {
                particleSystem.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1));
                particleSystem.Play();
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            
            particleSystem.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
            print("지금 찍은 위치 x: "+Input.mousePosition.x +" y: "+ Input.mousePosition.y);

            particleSystem.Play();
        }
    }
}
