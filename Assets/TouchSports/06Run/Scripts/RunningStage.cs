using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.T_Sports.Running
{
    public class RunningStage : MonoBehaviour
    {
        public bool go;
        public float speed;
        public float TargetSpeed;
        public bool run;
        public bool stopable;
        public Animator Runner;
        public bool stopcheck;
        private void Awake()
        {
            stopable = false;
            stopcheck = false;
            speed = 0;
            TargetSpeed = 0;
        }
        void Update()
        {
            float animSpeed = speed / 5;
            if (run)
            {
                if (animSpeed < 0.1f)
                {
                    animSpeed = 0.1f;
                }

                if (speed < TargetSpeed)
                    speed += Time.deltaTime;
                
            }
            else
            {
                if (speed >= TargetSpeed)
                    speed -= Time.deltaTime * 3;
                if (speed <= 0)
                    speed = 0;
                if (stopable && animSpeed <= 0.1f)
                {
                    if (!stopcheck)
                    {
                        stopcheck = true;
                        Runner.SetTrigger("Stop");
                    }
                    animSpeed = 0.5f;
                }
            }
            
            Runner.speed = animSpeed;


            if (go)
            {
                this.transform.Rotate(Vector3.left * Time.deltaTime * speed);
            }
            else
            {
                this.transform.Rotate(Vector3.right * Time.deltaTime * speed);
            }
        }
        public void GoRun(float targetspeed)
        {
            Runner.SetTrigger("Run");
            run = true;
            stopcheck = false;
            TargetSpeed = targetspeed;
        }

        public void StopRun()
        {
            run = false;
            stopable = true;
            TargetSpeed = 0;
        }
    }
}
