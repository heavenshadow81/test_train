using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.Aquarium
{
    public class CatmullRomSpline : object
    {
        private Matrix4x4 coefficient = new Matrix4x4();
        private ArrayList points = new ArrayList();
        private ArrayList tangents = new ArrayList();

        public CatmullRomSpline()
        {
            // Set up matrix
            // Hermite polynomial
            coefficient[0, 0] = 2;
            coefficient[0, 1] = -2;
            coefficient[0, 2] = 1;
            coefficient[0, 3] = 1;
            coefficient[1, 0] = -3;
            coefficient[1, 1] = 3;
            coefficient[1, 2] = -2;
            coefficient[1, 3] = -1;
            coefficient[2, 0] = 0;
            coefficient[2, 1] = 0;
            coefficient[2, 2] = 1;
            coefficient[2, 3] = 0;
            coefficient[3, 0] = 1;
            coefficient[3, 1] = 0;
            coefficient[3, 2] = 0;
            coefficient[3, 3] = 0;
        }

        public void Push(Vector3 p)
        {
            points.Add(p);
            tangents.Add(new Vector3());
        }

        public void Pop()
        {
            if (Count() > 0)
            {
                points.RemoveAt(0);
                tangents.RemoveAt(0);
            }
        }

        public Vector3 At(int index)
        {
            return (Vector3)points[index];
        }

        public Vector3 TangentAt(int index)
        {
            return (Vector3)tangents[index];
        }

        public int Count()
        {
            return points.Count;
        }

        public void Clear()
        {
            points.Clear();
            tangents.Clear();
        }

        //	public Vector3 FastInterpolate(int fromIndex, float t) {
        //		float t2 = t * t;
        //		float t3 = t2 * t;
        //		
        //		Vector3 p0 = At (fromIndex - 1);
        //		Vector3 p1 = At (fromIndex);
        //		Vector3 p2 = At (fromIndex + 1);
        //		Vector3 p3 = At (fromIndex + 1);
        //		
        //		float tension = 0.5f;
        //		
        //		Vector3 T1 = tension * (p2 - p0);
        //		Vector3 T2 = tension * (p3 - p1);
        //		
        //		float blend1 = 2 * t3 - 3 * t2 + 1;
        //		float blend2 = -1 * t3 + 3 * t2;
        //		float blend3 = t3 - 2 * t2 + t;
        //		float blend4 = t3 - t2;
        //		
        //		return blend1 * p1 + blend2 * p2 + blend3 * T1 + blend4 * T2;
        //	}

        public Vector3 Interpolate(int fromIndex, float t)
        {
            if ((fromIndex + 1) == points.Count)
            {
                // Duff request, cannot blend to nothing
                // Just return source
                return At(fromIndex);
            }

            // Fast special cases
            if (t == 0.0f)
            {
                return At(fromIndex);
            }
            else if (t == 1.0f)
            {
                return At(fromIndex + 1);
            }

            // float interpolation
            // Form a vector of powers of t
            float t2, t3;
            t2 = t * t;
            t3 = t2 * t;
            Vector4 powers = new Vector4(t3, t2, t, 1);

            // Algorithm is ret = powers * coefficient * Matrix4(point1, point2, tangent1, tangent2)
            Vector3 point1 = At(fromIndex);
            Vector3 point2 = At(fromIndex + 1);
            Vector3 tan1 = TangentAt(fromIndex);
            Vector3 tan2 = TangentAt(fromIndex + 1);
            Matrix4x4 pt = new Matrix4x4();

            pt[0, 0] = point1.x;
            pt[0, 1] = point1.y;
            pt[0, 2] = point1.z;
            pt[0, 3] = 1.0f;
            pt[1, 0] = point2.x;
            pt[1, 1] = point2.y;
            pt[1, 2] = point2.z;
            pt[1, 3] = 1.0f;
            pt[2, 0] = tan1.x;
            pt[2, 1] = tan1.y;
            pt[2, 2] = tan1.z;
            pt[2, 3] = 1.0f;
            pt[3, 0] = tan2.x;
            pt[3, 1] = tan2.y;
            pt[3, 2] = tan2.z;
            pt[3, 3] = 1.0f;

            //Vector4 ret = powers * coefficient * pt;
            Vector4 ret = multiply(multiply(powers, coefficient), pt);

            return new Vector3(ret.x, ret.y, ret.z);
        }

        public ArrayList FastInterpolate(int fromIndex, float step)
        {
            ArrayList list = new ArrayList();

            if ((fromIndex + 1) == points.Count)
            {
                // Duff request, cannot blend to nothing
                // Just return source
                list.Add(At(fromIndex));
                return list;
            }

            // Algorithm is ret = powers * coefficient * Matrix4(point1, point2, tangent1, tangent2)
            Vector3 point1 = At(fromIndex);
            Vector3 point2 = At(fromIndex + 1);
            Vector3 tan1 = TangentAt(fromIndex);
            Vector3 tan2 = TangentAt(fromIndex + 1);
            Matrix4x4 pt = new Matrix4x4();

            pt[0, 0] = point1.x;
            pt[0, 1] = point1.y;
            pt[0, 2] = point1.z;
            pt[0, 3] = 1.0f;
            pt[1, 0] = point2.x;
            pt[1, 1] = point2.y;
            pt[1, 2] = point2.z;
            pt[1, 3] = 1.0f;
            pt[2, 0] = tan1.x;
            pt[2, 1] = tan1.y;
            pt[2, 2] = tan1.z;
            pt[2, 3] = 1.0f;
            pt[3, 0] = tan2.x;
            pt[3, 1] = tan2.y;
            pt[3, 2] = tan2.z;
            pt[3, 3] = 1.0f;

            Matrix4x4 m = coefficient * pt;

            for (float t = 0; t < 1.0f; t += step)
            {
                // Fast special cases
                if (t == 0.0f)
                {
                    list.Add(At(fromIndex));
                    continue;
                }
                else if (t == 1.0f)
                {
                    list.Add(At(fromIndex + 1));
                    continue;
                }

                // float interpolation
                // Form a vector of powers of t
                float t2, t3;
                t2 = t * t;
                t3 = t2 * t;
                Vector4 powers = new Vector4(t3, t2, t, 1);

                Vector4 ret = multiply(powers, m);
                list.Add(new Vector3(ret.x, ret.y, ret.z));
            }

            return list;
        }

        public void RecalcTangents()
        {
            // Catmull-Rom approach
            // 
            // tangent[i] = 0.5 * (point[i+1] - point[i-1])
            //
            // Assume endpoint tangents are parallel with line with neighbour

            int i, nupoints;
            bool isClosed;

            nupoints = points.Count;
            if (nupoints < 2)
            {
                // Can't do anything yet
                return;
            }

            // Closed or open?
            if (At(0) == At(nupoints - 1))
            {
                isClosed = true;
            }
            else
            {
                isClosed = false;
            }

            for (i = 0; i < nupoints; ++i)
            {
                if (i == 0)
                {
                    // Special case start
                    if (isClosed)
                    {
                        // Use nupoints-2 since nupoints-1 is the last point and == [0]
                        tangents[i] = 0.5f * (At(1) - At(nupoints - 2));
                    }
                    else
                    {
                        tangents[i] = 0.5f * (At(1) - At(0));
                    }
                }
                else if (i == nupoints - 1)
                {
                    // Special case end
                    if (isClosed)
                    {
                        // Use same tangent as already calculated for [0]
                        tangents[i] = TangentAt(0);
                    }
                    else
                    {
                        tangents[i] = 0.5f * (At(i) - At(i - 1));
                    }
                }
                else
                {
                    tangents[i] = 0.5f * (At(i + 1) - At(i - 1));
                }
            }
        }

        private Vector4 multiply(Vector4 v, Matrix4x4 m)
        {
            return new Vector4(
                v.x * m[0, 0] + v.y * m[1, 0] + v.z * m[2, 0] + v.w * m[3, 0],
                v.x * m[0, 1] + v.y * m[1, 1] + v.z * m[2, 1] + v.w * m[3, 1],
                v.x * m[0, 2] + v.y * m[1, 2] + v.z * m[2, 2] + v.w * m[3, 2],
                v.x * m[0, 3] + v.y * m[1, 3] + v.z * m[2, 3] + v.w * m[3, 3]
            );
        }
    }
}