using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.T_Sports.HandBall
{
    public class Handball_Cleaner_Multi : MonoBehaviour
    {
        //**<summary>필드 위의 모든 공들을 호출됨으로 전부 삭제

        public Handball_Ctrl_Multi[] balls;


        public void Allclear()
        {
            balls = FindObjectsOfType<Handball_Ctrl_Multi>();

            for(int i =0; i<balls.Length; i++)
            {
                Destroy(balls[i].gameObject);
            }
            
        }
        
    }
}
