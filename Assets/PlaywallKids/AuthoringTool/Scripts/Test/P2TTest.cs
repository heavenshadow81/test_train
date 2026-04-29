using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Poly2Tri;

public class P2TTest : MonoBehaviour
{
    public PointInput pointInput;

    public TriangulationContext context;

    public Material mat;

    public SimpleDraw2D draw2D;

    private bool showsInput = true;

    private TriangulationContext.PruneTriangleSlowVersionProcess process;

    private IEnumerator loopTriangleEnumerator;

    private DelaunayTriangle selectedTriangle;

    // TEST
    [Range(1, 10)]
    public int iterationCount = 5;
    [Range(0.0f, 1.0f)]
    public float lambda = 0.5f;

    public void ToggleShowsInput()
    {
        showsInput = !showsInput;
    }

    void Start()
    {
        if (draw2D == null)
        {
            draw2D = gameObject.GetComponent<SimpleDraw2D>();
        }

        if (draw2D != null)
        {
            draw2D.onDraw.Add(() =>
            {
                draw2D.DrawSpline(pointInput.input, Color.green, Color.white);
                DrawSelectedTriangle(Color.white);
                DrawContext(Color.yellow);
                DrawChordalAxis(Color.blue);
            });
        }

        P2T.Warmup();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Clear();
        }
        else if (Input.GetMouseButton(0) && context != null)
        {
            List<DelaunayTriangle> triangles = context.finalTriangles;
            if (triangles.Count == 0)
            {
                triangles = context.originalTriangles;
            }

            Vector3 point = Input.mousePosition;
            point = Camera.main.ScreenToWorldPoint(point);
            selectedTriangle = null;

            foreach (DelaunayTriangle triangle in triangles)
            {
                if (Triangle.IsIn(Util.PointToVector3(triangle.Points[0]), Util.PointToVector3(triangle.Points[1]), Util.PointToVector3(triangle.Points[2]), point))
                {
                    selectedTriangle = triangle;
                    break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            iterationCount = Mathf.Min(10, iterationCount + 1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            iterationCount = Mathf.Max(0, iterationCount - 1);
        }
    }

    public void Clear()
    {
        context = null;
        pointInput.enabled = true;
        pointInput.input.Clear();
        selectedTriangle = null;
    }

    public void MeshGenerate()
    {
        if (context != null)
        {
            GameObject go = new GameObject("mesh");
            go.layer = LayerMask.NameToLayer("UI");
            MeshFilter mf = go.AddComponent<MeshFilter>();
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            Mesh mesh = MeshGeneration.GetMesh(context, 4, lambda, iterationCount);
            mesh = MeshGeneration.GetSmoothMesh(mesh, lambda, iterationCount);
            mesh.RecalculateNormals();
            mf.mesh = mesh;
            mr.material = mat;
            go.AddComponent<AutoRotate>().axis = Vector3.up;
        }
    }

    public void ClearMeshes()
    {
        pointInput.input.Clear();
        Clear();

        GameObject[] gos = FindObjectsOfType<GameObject>();
        foreach (GameObject go in gos)
        {
            if (go.name.Equals("mesh"))
            {
                Destroy(go);
            }
        }
    }

    public void DrawContext(Color color)
    {
        if (context == null) return;

        List<DelaunayTriangle> triangles = context.finalTriangles;
        if (triangles.Count == 0)
        {
            triangles = context.originalTriangles;
        }

        foreach (var triangle in triangles)
        {
            for (int i = 0; i < 3; i++)
            {
                List<Vector3> list = new List<Vector3>();
                bool constrained = triangle.GetConstrainedEdgeCCW(triangle.Points[i]);
                //bool hasConstrained = triangle.GetEdgeCCW(triangle.Points[i], out edge);

                if (constrained)
                {
                    color = Color.yellow;
                }
                //else if(constrained && !hasConstrained) {
                //	color = Color.yellow;
                //}
                else
                {
                    color = Color.red;
                }

                var p1 = triangle.Points[i];
                var p2 = triangle.Points[(i + 1) % 3];
                list.Add(new Vector3((float)p1.X, (float)p1.Y));
                list.Add(new Vector3((float)p2.X, (float)p2.Y));
                draw2D.DrawLine(list[0], list[1], color);
            }
        }

        if (process != null)
        {
            draw2D.DrawCircle(Util.PointToVector3(process.leftPoint), Color.black, 0.05f, 20.0f);
            draw2D.DrawCircle(Util.PointToVector3(process.rightPoint), Color.black, 0.05f, 20.0f);
            draw2D.DrawCircle(Util.PointToVector3(process.centerPoint), Color.white, (float)process.offset.Magnitude(), 20.0f);
        }
    }

    public void DrawChordalAxis(Color color)
    {
        if (context != null)
        {
            ChordalAxis chordalAxis = context.chordalAxis;

            foreach (DelaunayTriangle triangle in chordalAxis.triangles)
            {
                ChordalAxisInfo info = chordalAxis.chordalAxisInfoDict[triangle];

                switch (info.type)
                {
                    case ChordalAxisInfo.Type.Terminal:
                    case ChordalAxisInfo.Type.Sleeve:
                    case ChordalAxisInfo.Type.TwoPoint:
                        draw2D.DrawLine(Util.PointToVector3(info.points[0]), Util.PointToVector3(info.points[1]), color);
                        break;
                    case ChordalAxisInfo.Type.Junction:
                        draw2D.DrawLine(Util.PointToVector3(info.points[0]), Util.PointToVector3(info.points[1]), color);
                        draw2D.DrawLine(Util.PointToVector3(info.points[0]), Util.PointToVector3(info.points[2]), color);
                        draw2D.DrawLine(Util.PointToVector3(info.points[0]), Util.PointToVector3(info.points[3]), color);
                        break;
                }
            }
        }
    }

    public void DrawSelectedTriangle(Color color)
    {
        if (selectedTriangle != null)
        {
            draw2D.DrawTriangle(Util.PointToVector3(selectedTriangle.Points[0]),
                                Util.PointToVector3(selectedTriangle.Points[1]),
                                Util.PointToVector3(selectedTriangle.Points[2]),
                                color);

            // draw neighbors
            Color[] neighborColors = { Color.red, Color.green, Color.blue };
            for (int i = 0; i < 3; i++)
            {
                var neighbor = selectedTriangle.Neighbors[i];
                if (neighbor != null)
                {
                    draw2D.DrawTriangle(Util.PointToVector3(neighbor.Points[0]),
                                        Util.PointToVector3(neighbor.Points[1]),
                                        Util.PointToVector3(neighbor.Points[2]),
                                        color * 0.5f + neighborColors[i] * 0.5f);
                }
            }
        }
    }

    public void Triangulate()
    {
        if (pointInput.input.Count > 2)
        {
            //Util.LaplacianSmooth(pointInput.input, 0.5f, 2, true);

            List<PolygonPoint> list = new List<PolygonPoint>();
            foreach (Vector3 pos in pointInput.input)
            {
                list.Add(new PolygonPoint(pos.x, pos.y));
            }
            Polygon t = new Polygon(list);
            P2T.Triangulate(TriangulationAlgorithm.DTSweep, t);
            context = new TriangulationContext(t.Triangles);

            pointInput.enabled = false;
        }
    }

    public void GetChordalAxis()
    {
        if (context != null)
        {
            context.BeginTriangulate();
        }
    }

    public void PruneTriangles_SlowVersion()
    {
        if (context != null)
        {
            StartCoroutine(PruneTrianglesSlowVersion());
        }
    }

    public void PruneTriangles()
    {
        if (context != null)
        {
            context.PruneTriangles();
        }
    }

    public void LoopTriangle()
    {
        if (loopTriangleEnumerator != null)
        {
            StopCoroutine(loopTriangleEnumerator);
            loopTriangleEnumerator = null;
        }
        loopTriangleEnumerator = PerformLoopTriangle();
        StartCoroutine(loopTriangleEnumerator);
    }

    public IEnumerator PerformLoopTriangle()
    {
        if (context != null)
        {
            var list = context.GetTriangleStripCW();
            foreach (DelaunayTriangle t in list)
            {
                selectedTriangle = t;
                yield return new WaitForSeconds(0.1f);
            }
            selectedTriangle = null;
        }

        yield return null;
    }

    public IEnumerator PruneTrianglesSlowVersion()
    {
        List<DelaunayTriangle> list, terminalTriangles;
        context.PruneTriangles_SlowVersionBegin(out list, out terminalTriangles);

        foreach (DelaunayTriangle triangle in terminalTriangles)
        {
            process = new TriangulationContext.PruneTriangleSlowVersionProcess(triangle);
            bool end = false;

            do
            {
                if (process.currentPruningTriangle == null)
                {
                    end = true;
                }
                context.PerformPruneTriangleStartAt_SlowVersion(process, list, triangle);
                yield return new WaitForSeconds(0.5f);
            }
            while (!end);
        }

        process = null;

        context.PruneTriangle_SlowVersionEnd(list);
    }

    public void GetFinalTriangulation()
    {
        if (context != null)
        {
            context.FinalTriangulation();
        }
    }
}