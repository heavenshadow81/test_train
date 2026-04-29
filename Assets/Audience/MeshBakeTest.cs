using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBakeTest : MonoBehaviour
{
    public float maxFrame = 56;
    public float time = 0.0f;
    public SkinnedMeshRenderer smr;
    public Animator animator;
    public int frame = 0;

    public void Start()
    {
        smr = GetComponentInChildren<SkinnedMeshRenderer>();
        animator = GetComponentInChildren<Animator>();
        animator.speed = 0;

        StartCoroutine(_Update());
    }

    public IEnumerator _Update()
    {
        float len = animator.GetCurrentAnimatorClipInfo(0).Length;
        for (frame = 0; frame <= maxFrame; frame++)
        {
            animator.Play("idle", -1, frame / maxFrame);
            animator.speed = 0;
            yield return 0;
            Mesh mesh = new Mesh();
            smr.BakeMesh(mesh);
#if UNITY_EDITOR
            UnityEditor.MeshUtility.Optimize(mesh);
            UnityEditor.AssetDatabase.CreateAsset(mesh, "Assets/Audience/Baked/idle" + frame + ".asset");
#endif
            yield return 0;
        }
    }
}
