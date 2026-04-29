using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.T_Sports.Soccer
{
    public class GUI_Goal_Script : MonoBehaviour
    {
        //게임메니져에서 골/노골 GUI를 제어 하기 위한 스크립트, 각각의 상황에 따라 GUI active.
        public GameObject Goal_Texture;
        public GameObject No_Goal_Texture;

        void Start()
        {
            Goal_Texture.SetActive(false);
            No_Goal_Texture.SetActive(false);
        }

        public void Goal()
        {
            Goal_Texture.SetActive(true);
        }

        public void NoGoal()
        {
            No_Goal_Texture.SetActive(true);
        }

        public void reflash()
        {
            Goal_Texture.SetActive(false);
            No_Goal_Texture.SetActive(false);
        }

    }
}
