using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dolphin
{
    public class CircleMove : MonoBehaviour
    {
        float speed; //장애물 속도

        void Update()
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime); //장애물을 왼쪽으로 이동
            speed = 7; //속도 7
        }
        private void OnTriggerEnter(Collider other) //other와 부딪힘 감지
        {
            if (other.tag == "Ground") //other의 태그가 Ground라면
                Destroy(gameObject); //나 자신 삭제
        }
    }
}

