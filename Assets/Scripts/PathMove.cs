using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class PathMove : MonoBehaviour
    {
        public Transform[] path;
        public int pathIndex;
        public int Index;

        public Transform movingObject;
        public float movingSpeed;
        public float ChracterSpeed;

        public float maxSpeed;
        public float minSpeed;
        public float rotateSpeed;

        public ParticleSystem SpeedParticle;
        public float particleEmit;
        public Animator PlayerAnimator;

        public BezierCurveManager bezier;
        public Transform[] Cents;
        public int CentIdx;

        public Transform lineOP;
        public Transform[] Lines;

        public MotionGuide RunningGuide;
        public MotionGuide LineGuide;
        public float GuideCoolTime;

        private void Awake()
        {
            InitValues();
        }
        void FixedUpdate()
        {
            /* if (Input.GetKeyDown(KeyCode.C))
                 bezier.BezierSet(LineObj);*/
            if (Input.GetKeyDown(KeyCode.A))
                PlusSpeed();
            PathSkating();
         /*  if (KinectSkateManager.instance.playstate == PlayState.Play || KinectSkateManager.instance.playstate == PlayState.Ending)
            {
                if (KinectSkateManager.instance.playstate == PlayState.Ending)
                    minSpeed = 8;
               // PlayerAnimator.speed = movingSpeed / 5;
                PlayerAnimator.SetFloat("Speed", movingSpeed);
                particleEmit = (movingSpeed / maxSpeed) * 50;
                if (particleEmit > 25)
                    SpeedParticle.emissionRate = particleEmit;
                //최소 스피드는 유지
                if (movingSpeed < minSpeed)
                    movingSpeed += Time.deltaTime * 2;
                else
                {
                    movingSpeed -= Time.deltaTime / 2;
                }

                //속도가 8 미만에서 6초이상 지속되면 러닝가이드 활성화
                if (movingSpeed < 8f)
                {
                    if (GuideCoolTime <= 0)
                    {
                        if (!LineGuide.isActive)
                        {
                            GuideCoolTime = 10;
                            RunningGuide.GuideStart(2f);
                        } 
                    }
                    else
                        GuideCoolTime -= Time.deltaTime;
                }
                else
                    GuideCoolTime = 6;

                rotateSpeed = movingSpeed / 5;
                if (rotateSpeed < 1.5f)
                    rotateSpeed = 1.5f;


                if (maxSpeed < movingSpeed)
                    movingSpeed = maxSpeed;

                ChracterSpeed = movingSpeed;//* 0.7f;
                float dist = Vector3.Distance(movingObject.position, Lines[Index].position);
                
                movingObject.Translate(Vector3.forward * Time.deltaTime * ChracterSpeed);
                movingObject.rotation = Quaternion.Lerp(movingObject.rotation, Quaternion.LookRotation(Lines[Index].position - movingObject.position), rotateSpeed * Time.deltaTime);

                if (dist < 1)
                {
                    int prev = Index;
                    Index++;
                    if (Index >= Lines.Length)
                    {
                        Index = 0;
                    }

                    float dist_line = Vector3.Distance(Lines[Index].position, Lines[prev].position);
                    //Debug.Log("Index" + Index + "/" + dist_line);
                    if (dist_line < 0.5f)
                    {
                        //   Debug.Log("제거" + Index);
                        Index++;
                    }


                    if (Index >= Lines.Length)
                    {
                        Index = 0;
                    }
                }
            }*/
        }

        public void InitValues()
        {
            GuideCoolTime = 6;
            Lines = bezier.Lines;
            Index = 0;
            minSpeed = 7f;
            maxSpeed = 10;
            movingObject.position = path[4].position;
            movingObject.rotation = Quaternion.LookRotation(path[5].position - movingObject.position);
        }
        public void PathSkating()
        {
            if (KinectSkateManager.instance.playstate == PlayState.Play || KinectSkateManager.instance.playstate == PlayState.Ending)
            {
                if (KinectSkateManager.instance.playstate == PlayState.Ending)
                    minSpeed = 8;
                // PlayerAnimator.speed = movingSpeed / 5;
                PlayerAnimator.SetFloat("Speed", movingSpeed);
                particleEmit = (movingSpeed / maxSpeed) * 50;
                if (particleEmit > 25)
                    SpeedParticle.emissionRate = particleEmit;
                //최소 스피드는 유지
                if (movingSpeed < minSpeed)
                    movingSpeed += Time.deltaTime * 2;
                else
                {
                    movingSpeed -= Time.deltaTime / 2;
                }

                //속도가 8 미만에서 6초이상 지속되면 러닝가이드 활성화
                if (movingSpeed < 8f)
                {
                    if (GuideCoolTime <= 0)
                    {
                        if (!LineGuide.isActive)
                        {
                            GuideCoolTime = 10;
                            RunningGuide.GuideStart(2f);
                        }
                    }
                    else
                        GuideCoolTime -= Time.deltaTime;
                }
                else
                    GuideCoolTime = 6;

                rotateSpeed = movingSpeed / 5;
                if (rotateSpeed < 1.5f)
                    rotateSpeed = 1.5f;


                if (maxSpeed < movingSpeed)
                    movingSpeed = maxSpeed;

                ChracterSpeed = movingSpeed;//* 0.7f;
                if (Index > Lines.Length)
                {
                    Index = 0;
                }
                float dist = Vector3.Distance(movingObject.position, Lines[Index].position);

                movingObject.Translate(Vector3.forward * Time.deltaTime * ChracterSpeed);
                movingObject.rotation = Quaternion.Lerp(movingObject.rotation, Quaternion.LookRotation(Lines[Index].position - movingObject.position), rotateSpeed * Time.deltaTime);

                if (dist < 1)
                {
                    int prev = Index;
                    Index++;
                    if (Index >= Lines.Length)
                    {
                        Index = 0;
                    }

                    float dist_line = Vector3.Distance(Lines[Index].position, Lines[prev].position);
                    //Debug.Log("Index" + Index + "/" + dist_line);
                    if (dist_line < 0.5f)
                    {
                        //   Debug.Log("제거" + Index);
                        Index++;
                    }


                    if (Index >= Lines.Length)
                    {
                        Index = 0;
                    }
                }
            }
        }
        public void PlusSpeed()
        {
            movingSpeed += 1.5f;
        }

    }
}
