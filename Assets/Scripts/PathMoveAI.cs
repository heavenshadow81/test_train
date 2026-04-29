using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.SportsMiniGame.KinectSkating
{
    public class PathMoveAI : MonoBehaviour
    {

        public Transform[] path;
        public int[] pathIndex;

        public Transform[] movingAI;
        public float movingSpeed;
        public float maxSpeed;
        public float[] minSpeed;
        public float rotateSpeed;        

        private void Awake()
        {
            InitValues();
        }
        // Update is called once per frame
        void Update()
        {
            AIIntroPathSkating();
        }
        public void InitValues()
        {
            maxSpeed = 10;
            minSpeed = new float[movingAI.Length];
            //pathIndex = new int[movingAI.Length];
            for (int i = 0; i < pathIndex.Length; i++)
            {
                pathIndex[i] = Random.Range(0, path.Length);
            }
            for (int i = 0; i < movingAI.Length; i++)
            {
                movingAI[i].position = path[pathIndex[i]].position;
                minSpeed[i] = Random.Range(4, 8);
                int nextidx = pathIndex[i] + 1;
                if (nextidx >= path.Length)
                    nextidx = 0;
                movingAI[i].rotation = Quaternion.LookRotation(path[nextidx].position - movingAI[i].position);
            }
        }
        public void AIIntroPathSkating()
        {
            if (KinectSkateManager.instance.playstate != PlayState.Play)
            {
                //최소 스피드는 유지
                for (int i = 0; i < minSpeed.Length; i++)
                {
                    if (movingSpeed < minSpeed[i])
                        movingSpeed += Time.deltaTime * 2;
                    else
                    {
                        movingSpeed -= Time.deltaTime;
                    }
                }


                rotateSpeed = movingSpeed / 10;
                if (rotateSpeed < 3)
                    rotateSpeed = 3;


                if (maxSpeed < movingSpeed)
                    movingSpeed = maxSpeed;

                float[] dist = new float[movingAI.Length];
                for (int i = 0; i < dist.Length; i++)
                {
                    dist[i] = Vector3.Distance(movingAI[i].position, path[pathIndex[i]].position);
                    if (dist[i] > 1)
                    {
                        movingAI[i].Translate(Vector3.forward * Time.deltaTime * movingSpeed);
                        movingAI[i].rotation = Quaternion.Lerp(movingAI[i].rotation, Quaternion.LookRotation(path[pathIndex[i]].position - movingAI[i].position), rotateSpeed * Time.deltaTime);

                    }
                    else
                    {
                        pathIndex[i]++;
                        if (pathIndex[i] >= path.Length)
                            pathIndex[i] = 0;
                    }
                }

            }
        }
    }
}
