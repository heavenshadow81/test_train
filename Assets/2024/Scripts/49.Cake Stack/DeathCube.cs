using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class DeathCube : MonoBehaviour
    {
    [SerializeField] MagicLife life; //라이프 스크립트;

    private void OnCollisionEnter(Collision collision)
    {
        //케이크가 바닥에 떨어지면 삭제하고 라이프 감소
        if(collision.transform.tag == "Feed" || collision.transform.tag=="Finish")
        {
            Destroy(collision.gameObject);
            life.LifeDelete();
        }
    }
}
