using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.SportsMiniGame.KinectSkating
{
    public class CrowdY : MonoBehaviour
    {
        public GameObject[] crowds;
        public Transform[] seat;
        public int[] crowdCheck;
        public int yCount;
        public Material[] InstancedMaterial;
        private void Awake()
        {/*
            for (int i = 0; i < InstancedMaterial.Length; i++)
            {
                InstancedMaterial[i].enableInstancing = true;
            }
            
            CreateCrowdY();  */
        }
        public void CreateCrowdY()
        {
            crowdCheck = new int[yCount];
            seat = new Transform[yCount];
            for (int i = 0; i < seat.Length; i++)
                seat[i] = this.transform.GetChild(i);

            for (int i = 0; i < crowdCheck.Length; i++)
            {
                crowdCheck[i] = Random.Range(0, 3);
                if (crowdCheck[i] != 0)
                {
                    Transform tmp = Instantiate(crowds[Random.Range(0, crowds.Length)], seat[i].position, seat[i].rotation).transform;
                 /*   MaterialPropertyBlock matpropertyBlock = new MaterialPropertyBlock();
                    Color newColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                    matpropertyBlock.SetColor("_Color", newColor);
                    tmp.GetChild(0).GetComponent<SkinnedMeshRenderer>().SetPropertyBlock(matpropertyBlock);*/

                    tmp.parent = seat[i];
                    tmp.localPosition = Vector3.zero;
                    tmp.localRotation = Quaternion.identity;
                }
            }

            
        }
    }
}