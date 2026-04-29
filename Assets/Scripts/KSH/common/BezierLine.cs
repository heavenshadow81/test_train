using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierLine
{
    public Transform target;

    public Transform p1;
    public Transform p2;

   
    public int lineCnt = 20;//¶óÀÎ ¼ö

    public List<Vector3> moveList = new List<Vector3>();

    public BezierLine(Transform target , Transform startpos1, Transform endPos1 , int lineCnt)
    {
       this.target = target;
       this.p1 = startpos1;
       this.p2 = endPos1;
       this.lineCnt = lineCnt;
    }




    public float Bezier(float P0, float P1, float P2, float P3, float t)
    {
        return Mathf.Pow((1 - t), 3) * P0 + Mathf.Pow((1 - t), 2) * 3 * t * P1 + Mathf.Pow(t, 2) * 3 * (1 - t) * P2 +
               Mathf.Pow(t, 3) * P3;
    }
    public Vector3 Bezier(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3, float t)
    {
        Vector3 M0 = Vector3.Lerp(P0, P1, t);
        Vector3 M1 = Vector3.Lerp(P1, P2, t);
        Vector3 M2 = Vector3.Lerp(P2, P3, t);

        Vector3 B0 = Vector3.Lerp(M0, M1, t);
        Vector3 B1 = Vector3.Lerp(M1, M2, t);

        return Vector3.Lerp(B0, B1, t);
    }


    public void DrawMove(Vector3 startPos , Vector3 startpos1 , Vector3 endPos1 , Vector3 endpos)
    {
        moveList.Clear();
        for (int i = 0; i < lineCnt; i++)
        {
            float t;
            if (i == 0)
            {
                t = 0;
            }
            else
            {
                t = (float)i / (lineCnt - 1);
            }

            Vector3 bezier = Bezier(startPos,
                                   startpos1,
                                    endPos1,
                                    endpos, t);
            moveList.Add(bezier);
        }
    }



  


}
