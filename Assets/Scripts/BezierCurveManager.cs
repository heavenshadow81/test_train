using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class BezierCurveManager : MonoBehaviour
    {

        public Transform[] Lines;
        public Vector2[] LinePath;
        public Transform Cent, Mover;
        public Transform TargetPin;
        public float BezierSpeed;
        public int idx = 0;
        public Transform lineOP;
        public int pathIndex;
        public int CentIdx;
        public PathMove pathmove;
        public PathMovePlayAI pathmoveai;
        public Transform[] path, Cents;
        public GameObject LineObj;
        public int[] delet;
        private void Awake()
        {
            InitValues();
            //Lines = SelectLine2(Lines);
        }

        public void InitValues()
        {
            BezierSpeed = 0.1f;
            Lines = new Transform[80];
            delet = new int[80];
            pathIndex = 6;
            CentIdx = 5;
            if (pathmove != null)
            {
                path = pathmove.path;
                Cents = pathmove.Cents;
            }
            else if (pathmoveai != null)
            {
                path = pathmoveai.path;
                Cents = pathmoveai.Cents;
            }
            Mover.position = path[CentIdx].position;
            TargetPin = path[pathIndex];
            Cent.position = Cents[CentIdx].position;
            for (int i = 0; i < 16; i++)
                BezierSet();
            Lines = SelectLine(Lines);
        }       
        
        public Transform[] SelectLine(Transform[] selectLine)
        {
            int totalLength = selectLine.Length;
            Transform[] linetmp = selectLine;
            int prev = 0;
            for (int i = 0; i < delet.Length; i++)
                delet[i] = -1;


            for (int i = 1; i < selectLine.Length; i++)
            {
                prev = i - 1;
                float dist = Vector3.Distance(selectLine[i].position, selectLine[prev].position);
                if (dist < 0.5f)
                {
                    delet[i] = 1;
                }
            }
            int deletcnt = 0;
            for (int i = 0; i < delet.Length; i++)
            {
                if (delet[i] == 1)
                    deletcnt++;
            }
            deletcnt = totalLength - deletcnt;
            Transform[] NewLines = new Transform[deletcnt];
            //Debug.Log("지운우고 남은게 "+ deletcnt+"개");
            int lineidx = 0;
            for (int i = 0; i < totalLength; i++)
            {
                if (delet[i] == -1)
                {
                    NewLines[lineidx] = linetmp[i];
                    lineidx++;
                }
            }
            return NewLines;
        }
        public Transform[] SelectLine2(Transform[] selectLine)
        {
            int totalLength = selectLine.Length;
            Transform[] linetmp = selectLine;
            int prev = 0;
            for (int i = 0; i < delet.Length; i++)
                delet[i] = -1;


            for (int i = 1; i < selectLine.Length; i++)
            {
                prev = i - 1;
                float dist = Vector3.Distance(selectLine[i].position, selectLine[prev].position);
                //Debug.Log(prev+"/"+i+"길이는"+dist);
                if (dist < 0.5f)
                {
                    delet[i] = 1;
                }
            }
            int deletcnt = 0;
            for (int i = 0; i < delet.Length; i++)
            {
                if (delet[i] == 1)
                    deletcnt++;
            }
            deletcnt = totalLength - deletcnt;
            Transform[] NewLines = new Transform[deletcnt];
            //Debug.Log("지운우고 남은게 " + deletcnt + "개");
            int lineidx = 0;
            for (int i = 0; i < totalLength; i++)
            {
                if (delet[i] == -1)
                {
                    NewLines[lineidx] = linetmp[i];
                    lineidx++;
                }
            }
            return NewLines;
        }
        //직선베지어
        Vector2 BezierCurveSt(float t, Vector2 p0, Vector2 p1)
        {
            Vector2 tmp = ((1 - t) * p0) + ((t) * p1);
            return tmp;
        }
        //곡선베지어         
        Vector3 BezierCurve(float t, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            //p0 = 이동자, p1 = 센터, p2 = 목표
            Vector2 pa = BezierCurveSt(t, p0, p1);
            Vector2 pb = BezierCurveSt(t, p1, p2);
            Vector2 tmp = BezierCurveSt(t, pa, pb);
            Mover.position = new Vector3(tmp.x, 0, tmp.y);
            return new Vector3(tmp.x, 0, tmp.y);
        }
        Vector2 BezierVectorChange(Vector3 tmp)
        {
            return new Vector2(tmp.x, tmp.z);
        }
        Vector3 BezierVectorChangeReverse(Vector2 tmp)
        {
            return new Vector3(tmp.x, 0, tmp.y);
        }
        int lineIdx = 0;
       
        public void BezierSet()
        {            
            for (int i = 0; i < 5; i++)
            {
                if (Lines[lineIdx] == null)
                {
                    Lines[lineIdx] = (Instantiate(LineObj, Vector3.zero, Quaternion.identity)).transform;
                    Lines[lineIdx].parent = lineOP;
                    Lines[lineIdx].name = "Line_" + lineIdx.ToString();
                } 
                //Cent좌표 베지어 직선이동
                Vector2 tmp = BezierCurveSt(BezierSpeed,
                   new Vector2(Cent.position.x, Cent.position.z),
                    BezierVectorChange(TargetPin.position));
                Cent.position = new Vector3(tmp.x, 0, tmp.y);
                //Mover좌표 베지어 곡선이동
                BezierCurve(BezierSpeed,
                    BezierVectorChange(Mover.position),
                    BezierVectorChange(Cent.position),
                    BezierVectorChange(TargetPin.position));

                BezierSpeed = 0.1f * i;
                Lines[lineIdx].position = Mover.position;
                lineIdx++;
            }
            CentIdx++;
            pathIndex++;
            if (CentIdx >= Cents.Length)
                CentIdx = 0;
            if (pathIndex >= path.Length)
                pathIndex = 0;

            if (lineIdx >= Lines.Length)
                lineIdx = 0;
            BezierSpeed = 0.1f;
            //Mover.position = path[CentIdx].position;
            TargetPin = path[pathIndex];
            Cent.position = Cents[CentIdx].position;
            //StartCoroutine(beziertest(LineObj));
        }
    }

}
