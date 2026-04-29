using UnityEngine;
using System.Collections.Generic;
using Poly2Tri;

public class MeshSmoothTest : MonoBehaviour
{
    public PointInput pointInput;
    public SimpleDraw2D draw2D;

    public GameObject go;
    public Material mat;

    public MeshGenerationInfo info;

    public float lambda = 0.5f;
    public int iterationCount = 5;

    public void Start()
    {
        if (draw2D == null)
        {
            draw2D = gameObject.GetComponent<SimpleDraw2D>();
        }

        draw2D.onDraw.Add(() =>
        {
            draw2D.DrawSpline(pointInput.input, Color.green, Color.white);
        });
    }

    public void LaplicianSmooth()
    {
        List<Vector3> input = pointInput.input;
        List<Vector3> output = new List<Vector3>(pointInput.input);

        if (input.Count < 2) return;

        for (int i = 1; i < input.Count - 1; i++)
        {
            Vector3 pPrev = input[i - 1];
            Vector3 p = input[i];
            Vector3 pNext = input[i + 1];

            p = p + ((pPrev - p) * 0.5f + (pNext - p) * 0.5f) * 0.5f;
            output[i] = p;
        }

        pointInput.input.Clear();
        pointInput.input.AddRange(output);
    }

    public void MeshSmooth()
    {
        if (go != null)
        {
            if (info != null)
            {
                MeshGeneration.Smooth(info, lambda, iterationCount);
                Debug.Log("info vertices count : " + info.GetVertices().Length);
                Debug.Log("info normals count : " + info.GetNormals().Length);
                Debug.Log("info indices count : " + info.GetIndices().Length);
            }
            Mesh mesh = MeshGeneration.GetMesh(info);

            MeshFilter mf = go.GetComponent<MeshFilter>();
            SkinnedMeshRenderer smr = go.GetComponent<SkinnedMeshRenderer>();
            if (mf != null)
            {
                mf.mesh = mesh;
            }
            else if (smr != null)
            {
                smr.sharedMesh = mesh;
            }
        }
    }

    public void Triangulate()
    {
        System.DateTime s = System.DateTime.Now;
        System.DateTime f;

        List<PolygonPoint> list = new List<PolygonPoint>();
        foreach (Vector3 pos in pointInput.input)
        {
            list.Add(new PolygonPoint(pos.x, pos.y));
        }
        Polygon t = new Polygon(list);
        P2T.Triangulate(TriangulationAlgorithm.DTSweep, t);

        f = System.DateTime.Now;
        Debug.Log("Triangulate - elapsed time : " + (f.Subtract(s).Milliseconds) + "ms");

        TriangulationContext context = new TriangulationContext(t.Triangles);
        context.Triangulate(true);

        f = System.DateTime.Now;
        Debug.Log("Teddy - elapsed time : " + (f.Subtract(s).Milliseconds) + "ms");

        info = MeshGeneration.Generate(context, 3, lambda, iterationCount);
        Debug.Log("info vertices count : " + info.GetVertices().Length);
        Debug.Log("info normals count : " + info.GetNormals().Length);
        Debug.Log("info indices count : " + info.GetIndices().Length);

        Mesh mesh = MeshGeneration.GetMesh(info);

        f = System.DateTime.Now;
        Debug.Log("Mesh - elapsed time : " + (f.Subtract(s).Milliseconds) + "ms");

        if (go == null)
        {
            go = new GameObject("mesh");
            go.layer = LayerMask.NameToLayer("UI");
            go.transform.localPosition = Vector3.zero;

            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>().material = mat;
        }

        go.GetComponent<MeshFilter>().mesh = mesh;
    }

    public void Clear()
    {
        if (go != null)
        {
            Destroy(go);
            go = null;
        }
        info = null;
        pointInput.input.Clear();
    }
}