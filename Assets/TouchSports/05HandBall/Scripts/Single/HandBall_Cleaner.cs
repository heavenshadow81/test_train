using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.T_Sports.HandBall
{
    public class HandBall_Cleaner : MonoBehaviour
    {
        //볼을 디스트로이를 위한 스크립트, 필드위에 있는 공들을 전부 가져온 다음 전부 삭제시킨다.

        public Ball_Ctrl_HandBall[] balls;


        public void Allclear()
        {
            balls = FindObjectsOfType<Ball_Ctrl_HandBall>();

            for(int i =0; i<balls.Length; i++)
            {
                Destroy(balls[i].gameObject);
            }
            
        }
        
    }
}
