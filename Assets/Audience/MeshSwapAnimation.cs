using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class MeshSwapAnimation : MonoBehaviour
    {
        public int AnimationNumber;
        public int MaterialNumber;

        public int meshIndex = 0;
        public Mesh[] applause;
        public float frameT_app;
        public Mesh[] celebration;
        public float frameT_cele;

        public Material[] materials;

        public float time = 0;
        public MeshFilter myMeshFilter;
        public MeshRenderer myMeshRenderer;
        // Use this for initialization
        
        void Awake()
        {
            if (Random.Range(0, 2) == 0)
            {
                this.gameObject.SetActive(false);
            }
            myMeshFilter = this.GetComponent<MeshFilter>();
            myMeshRenderer = this.GetComponent<MeshRenderer>();

            AnimationNumber = Random.Range(0, 2);
            MaterialNumber = Random.Range(0, materials.Length);
            myMeshRenderer.material = materials[MaterialNumber];

            frameT_app = 0.06f;
            frameT_cele = 0.05f;
            if (AnimationNumber == 0)
            {
                //meshIndex = Random.Range(0, applause.Length);
                myMeshFilter.sharedMesh = applause[meshIndex];
            }
            else
            {
               //meshIndex = Random.Range(0, celebration.Length);
                myMeshFilter.sharedMesh = celebration[meshIndex];
            }

        }

        // Update is called once per frame
        void Update()
        {
            time += Time.deltaTime;
            if (AnimationNumber == 0)
            {
                if (time >= frameT_app)
                {
                    time -= frameT_app;
                    meshIndex = (meshIndex + 1) % applause.Length;
                    myMeshFilter.sharedMesh = applause[meshIndex];
                }
            }
            else
            {
                if (time >= frameT_cele)
                {
                    time -= frameT_cele;
                    meshIndex = (meshIndex + 1) % celebration.Length;
                    myMeshFilter.sharedMesh = celebration[meshIndex];
                }
            }
        }

        private void OnBecameVisible()
        {
            enabled = true;
        }

        private void nBecameInvisible()
        {
            enabled = false;
        }
    }
}
