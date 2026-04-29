using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class ColliderVisualizer : MonoBehaviour
{
    private MeshCollider meshCollider;

    void OnDrawGizmos()
    {
        meshCollider = GetComponent<MeshCollider>();

        if (meshCollider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireMesh(meshCollider.sharedMesh, transform.position, transform.rotation, transform.lossyScale);
        }
    }
}