using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SP_Planet : MonoBehaviour
{
    [SerializeField] GameObject effect;
    [SerializeField] SP_Manger manager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Enemy")
        {
            //스코어 올라감
            manager.score++;
            
            //행성 파괴하면 파티클 생성
            Destroy(collision.gameObject);
            GameObject particle = Instantiate(effect,gameObject.transform.position, Quaternion.identity);
            Destroy(particle,1f);
        }
    }
}
