using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dolphin
{
    public class DolphinMove : MonoBehaviour
    {
       [SerializeField] bool Jump; //점프 값

        public Animator jumpAnim; //점프벨 애니메이션
        Animator dolphin; //돌고래 애니메이션

        private void OnEnable()
        {
            dolphin = GetComponent<Animator>(); //돌고래 애니메이터 컴포넌트 
        }
        void Update()
        {
            if (dolphin.GetCurrentAnimatorStateInfo(0).IsName("dolphinJump")) //돌핀점프 애니메이션이 재생중이라면
            {
                Jump = false; //점프 값 false
            }
            else if (dolphin.GetCurrentAnimatorStateInfo(0).IsName("Dolphin")) //돌핀 애니메이션이 재생중이라면
            {
                Jump = true; //점프 값 true
            }

            if (dolphin.GetCurrentAnimatorStateInfo(0).IsName("TwoJump")) //2단점프 애니메이션이 재생중이라면
            {
                Jump = false; //점프 값 false
            }
        }
        
        public void JumpBtn() //1단점프 버튼 함수
        {
            if (Jump && DGameManager.gameStart) //점프 값이 true라면
            {
                dolphin.SetTrigger("DolphinJump"); //돌고래 돌핀점프 애니메이션 재생
                jumpAnim.SetTrigger("1Jump"); //점프벨 1단점프 애니메이션 재생
                GameObject.Find("SoundManager").GetComponent<DolphinSound>().JumpSound1(); //점프 사운드 재생
                Jump = false; //1단점프 값 false
            }
        }
        public void TwoJumpBtn() //2단점프 버튼 함수
        {
            if (Jump && DGameManager.gameStart) //점프 값이 true라면
            {
                dolphin.SetTrigger("TwoJump"); //돌고래 2단점프 애니메이션 재생
                jumpAnim.SetTrigger("2Jump"); //점프벨 2단점프 애니메이션 재생
                GameObject.Find("SoundManager").GetComponent<DolphinSound>().JumpSound2(); //점프 사운드 재생
                Jump = false; //2단점프 값 false
            }
        }
    }
}

   
