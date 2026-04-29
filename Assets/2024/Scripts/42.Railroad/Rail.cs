using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Railroad
{

    public class Rail : MonoBehaviour
    {
        [SerializeField] GameObject ownObject;
        [SerializeField] MeshRenderer[] meshRenderers;
        [SerializeField] Material railMat;
        [SerializeField] GameObject prop;

        private void Awake()
        {
            meshRenderers = ownObject.GetComponentsInChildren<MeshRenderer>();
        }

        public void ChangeMat()
        {
            for(int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].material = railMat;
            }
        }

        public void SetProps()
        {
            if(prop != null)
            {
                prop.SetActive(true);
            }
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                ChangeMat();
                SetProps();
            }
        }
    }
}

