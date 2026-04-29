using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class PathMovePlayAI : MonoBehaviour
    {
        public Transform[] path;
        public int pathIndex;
        public int Index;

        public Transform movingObject;
        public float movingSpeed;
        public float ChracterSpeed;
        public float targetSpeed;

        public float rotateSpeed;        
        public Animator PlayerAnimator;        
        public Transform[] Cents;

        public int CentIdx;
        public Transform lineOP;
        public Transform[] Lines;

        public PathMove playerPath;
       
        private void Start()
        {
            InitValues();
        }
        
        void FixedUpdate()
        {
            AIPathSkating();
        }

        public void InitValues()
        {
            if (KinectSkateManager.instance.stage == Stage.Stage1)
                targetSpeed = 9f;
            else if (KinectSkateManager.instance.stage == Stage.Stage2)
                targetSpeed = 10f;
            else if (KinectSkateManager.instance.stage == Stage.Stage3)
                targetSpeed = 11.5f;
            path = playerPath.path;
            Lines = playerPath.Lines; Index = 0;
            movingObject.position = path[4].position;
            movingObject.rotation = Quaternion.LookRotation(path[5].position - movingObject.position);
        }

        public void AIPathSkating()
        {
            if (KinectSkateManager.instance.playstate == PlayState.Play || KinectSkateManager.instance.playstate == PlayState.Ending)
            {
                if (KinectSkateManager.instance.playstate == PlayState.Ending)
                    targetSpeed = 8;
                // PlayerAnimator.speed = movingSpeed / 5;
                PlayerAnimator.SetFloat("Speed", movingSpeed);

                //목표 스피드까지 가속
                if (movingSpeed <= targetSpeed)
                    movingSpeed += Time.deltaTime * 2.5f;
                else
                {
                    movingSpeed -= Time.deltaTime;
                }

                rotateSpeed = movingSpeed / 10;
                if (rotateSpeed < 3)
                    rotateSpeed = 3;

                ChracterSpeed = movingSpeed;// * 0.7f;
                
                float dist = Vector3.Distance(movingObject.position, Lines[Index].position);
                if (dist > 1)
                {
                    movingObject.Translate(Vector3.forward * Time.deltaTime * ChracterSpeed);
                    movingObject.rotation = Quaternion.Lerp(movingObject.rotation, Quaternion.LookRotation(Lines[Index].position - movingObject.position), rotateSpeed * Time.deltaTime);

                }
                else
                {
                    int prev = Index;
                    Index++;
                    if (Index >= Lines.Length)
                    {
                        Index = 0;
                    }

                    float dist_line = Vector3.Distance(Lines[Index].position, Lines[Index].position);
                    if (dist_line < 0.5f)
                        Index++;

                    if (Index >= Lines.Length)
                    {
                        Index = 0;
                    }
                }
            }
        }

        public void PlusSpeed()
        {
            movingSpeed += 0.5f;
        }

    }
}

