using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSwapTest : MonoBehaviour {
    public int index = 0;
    public Mesh[] meshes;
    public float time = 0;
    public float ft = 0.2f;
    public MeshFilter mf;

    // Use this for initialization
    void Start()
    {
        mf.sharedMesh = meshes[index];    
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(time >= ft)
        {
            time -= ft;
            index = (index + 1) % meshes.Length;
            mf.sharedMesh = meshes[index];
        }
    }
}
