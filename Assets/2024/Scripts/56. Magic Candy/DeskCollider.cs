using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskCollider : MonoBehaviour
{
    [SerializeField] GameObject starParticle; //파티클 프리팹
    private void OnCollisionEnter(Collision collision)
    {
        GameObject candy = collision.gameObject;

        //캔디 오브젝트와 충돌하면
        if(collision.transform.tag=="Point")
        {
            //파티클 생성 후 삭제
            GameObject particle = Instantiate(starParticle, collision.transform.position, Quaternion.identity);
            Destroy(particle, 2f);

            //떨어지는 캔디 오브젝트 삭제
            Destroy(collision.gameObject);
        }
    }
}
