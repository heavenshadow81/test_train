using UnityEngine;
using System.Collections.Generic;

public class SimpleDraw2D : MonoBehaviour
{
    public Shader shader;
    public List<System.Action> onDraw = new List<System.Action>();
    private Material lineMaterial;
    
    public void OnPostRender()
    {
        CreateLineMaterial();
        lineMaterial.SetPass(0);

        for (int i = 0, cnt = onDraw.Count; i < cnt; i++)
        {
            System.Action action = onDraw[i];
            if (action != null)
            {
                action();
            }
        }
    }

    public void OnDestroy()
    {
        if (lineMaterial != null)
        {
            Destroy(lineMaterial);
            lineMaterial = null;
        }
    }

    private void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    public void DrawCircle(Vector3 p, Color pointColor, float radius, float acc)
    {
        GL.Begin(GL.LINES);
        float t = Mathf.PI / acc;
        for (float f = 0.0f; f < 2.0f * Mathf.PI; f += t)
        {
            GL.Color(pointColor);
            GL.Vertex3(p.x + radius * Mathf.Cos(f), p.y + radius * Mathf.Sin(f), p.z - 0.1f);
            GL.Color(pointColor);
            GL.Vertex3(p.x + radius * Mathf.Cos(f + t), p.y + radius * Mathf.Sin(f + t), p.z - 0.1f);
        }
        GL.End();
    }

    public void DrawSpline(Spline spline, Color lineColor, Color pointColor)
    {
        if (spline.edges.Count > 0)
        {
            GL.Begin(GL.LINES);
            for (int i = 0, count = spline.edges.Count; i < count; i++)
            {
                Edge edge = spline.edges[i];
                GL.Color(lineColor);
                GL.Vertex(edge.aPos);
                GL.Color(lineColor);
                GL.Vertex(edge.bPos);

                if (i == 0)
                {
                    DrawCircle(edge.aPos, pointColor, 0.02f, 10);
                }
                DrawCircle(edge.bPos, pointColor, 0.02f, 10);
            }
            GL.End();
        }
    }

    public void DrawSpline(List<Vector3> points, Color lineColor, Color pointColor)
    {
        if (points.Count > 1)
        {
            GL.Begin(GL.LINES);
            for (int i = 0, count = points.Count; i < count; i++)
            {
                if (i == count - 1) break;
                GL.Color(lineColor);
                GL.Vertex(points[i]);
                GL.Color(lineColor);
                GL.Vertex(points[(i + 1) % count]);
            }
            GL.End();

            for (int i = 0, count = points.Count; i < count; i++)
            {
                DrawCircle(points[i], pointColor, 0.02f, 10);
            }
        }
    }

    public void DrawTriangle(Vector3 a, Vector3 b, Vector3 c, Color color)
    {
        GL.Begin(GL.TRIANGLES);
        GL.Color(color);
        GL.Vertex(a);
        GL.Vertex(b);
        GL.Vertex(c);
        GL.End();
    }

    public void DrawLine(Vector3 from, Vector3 to, Color color)
    {
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(from);
        GL.Color(color);
        GL.Vertex(to);
        GL.End();
    }
}