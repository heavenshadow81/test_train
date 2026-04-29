using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class TwoDimensionColliderManager : MonoBehaviour
    {
        public Transform top;
        public Transform bottom;
        public Transform left;
        public Transform right;
        public Transform front;
        public Transform back;

        [Range(0f, 1f)]
        public float bottomRatio;
        [Range(0f, 1f)]
        public float topRatio;
        [Range(0f, 1f)]
        public float leftRatio;
        [Range(0f, 1f)]
        public float rightRatio;
        public int depth = 1000;
        // Use this for initialization
        void Start()
        {

            if (bottom != null) bottom.localPosition = new Vector3(0, (UtilityScript.height + depth) * -bottomRatio, 0);
            if (top != null) top.localPosition = new Vector3(0, (UtilityScript.height + depth) * topRatio, 0);
            if (left != null) left.localPosition = new Vector3((UtilityScript.width + depth) * -leftRatio, 0, 0);
            if (right != null) right.localPosition = new Vector3((UtilityScript.width + depth) * rightRatio, 0, 0);
            if (front != null) front.localPosition = new Vector3(0, 0, -depth);
            if (back != null) back.localPosition = new Vector3(0, 0, depth);

            if (front != null) front.localScale = new Vector3(UtilityScript.width, UtilityScript.height, depth);
            if (left != null) left.localScale = new Vector3(depth, UtilityScript.height, depth);
            if (bottom != null) bottom.localScale = new Vector3(UtilityScript.width, depth, depth);

            if (top != null) top.localScale = bottom.localScale;
            if (right != null) right.localScale = left.localScale;
            if (back != null) back.localScale = front.localScale;
            //this.transform.localScale = new Vector3(UtilityScript.fWidth, UtilityScript.fHeight, 200f);
            /*
            MeshFilter filter = GetComponent<MeshFilter>();
            if (filter != null)
            {
                Mesh mesh = filter.mesh;

                Vector3[] normals = mesh.normals;
                for (int i = 0; i < normals.Length; ++i)
                { normals[i] = -normals[i]; }

                for (int m = 0; m < mesh.subMeshCount; m++)
                {
                    int[] triangles = mesh.GetTriangles(m);
                    for (int i = 0; i < triangles.Length; i += 3)
                    {
                        int temp = triangles[i + 0];
                        triangles[i + 0] = triangles[i + 1];
                        triangles[i + 1] = temp;
                    }
                    mesh.SetTriangles(triangles, m);
                }

                this.GetComponent<MeshCollider>().sharedMesh = filter.mesh;
                this.GetComponent<MeshRenderer>().enabled = false;
            }
             */
        }
    }
}