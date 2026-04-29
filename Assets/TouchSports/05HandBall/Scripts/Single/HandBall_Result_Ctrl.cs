using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ML.T_Sports.HandBall
{
    public class HandBall_Result_Ctrl : MonoBehaviour
    {
        //**결과 창 애니메이션 관장 

        public int Goal_Count;
        public int NoGoal_Count;

        public int tmp_goal;
        public int tmp_nogoal;
        public int tmp_score;

        public Text Goal_text;
        public Text NoGoal_text;

        public Text result_text;
        public Animation result_Ani;
        private void Start()
        {
            tmp_goal = 0;
            tmp_nogoal = 0;
            tmp_score = 0;

        }

        private void Update()
        {

            Goal_text.text = tmp_goal.ToString() ;
            NoGoal_text.text = tmp_nogoal.ToString();
            result_text.text = tmp_score.ToString()+"점";


        }

        public void SetValues(int goal, int nogoal)
        {
            Goal_Count = goal;
            NoGoal_Count = nogoal;
            Invoke("Ani_Start", 1.0f);
        }
        public void Ani_Start()
        {
            result_Ani.Play("Result_Apear");

        }

        public void Disapear()
        {
            StopAllCoroutines();
            tmp_goal = 0;
            tmp_nogoal = 0;
            tmp_score = 0;
            result_Ani.Play("Result_Disapear");

        }

        public void CountStart()
        {
            StartCoroutine("StartCounter");
        }

        public void ScoreCount()
        {
            StartCoroutine("ScoreCounter");
        }

        IEnumerator ScoreCounter()
        {
            while (true)
            {
                if (tmp_score < Goal_Count * 10)
                {
                    tmp_score++;
                }
                else
                {
                    tmp_score = Goal_Count * 10;
                }


                yield return new WaitForSeconds(0.01f);
            }
        }


        IEnumerator StartCounter()
        {

            while (true)
            {
                if (tmp_goal < Goal_Count&& tmp_nogoal < NoGoal_Count)
                {
                    tmp_goal++;
                    tmp_nogoal++;
                }
                else if(tmp_goal >= Goal_Count && tmp_nogoal < NoGoal_Count)
                {
                    tmp_goal = Goal_Count;
                    tmp_nogoal++;
                    
                }
                else if (tmp_goal < Goal_Count && tmp_nogoal >= NoGoal_Count)
                {
                    tmp_goal++;
                    tmp_nogoal=NoGoal_Count;

                }
                else
                {
                    tmp_goal = Goal_Count;
                    tmp_nogoal = NoGoal_Count;
                }
                yield return new WaitForSeconds(0.01f);
            }
        }

    }
}