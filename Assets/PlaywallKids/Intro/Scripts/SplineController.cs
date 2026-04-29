using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.Intro
{
    public enum eOrientationMode { NODE = 0, TANGENT }

    public class SplineController : MonoBehaviour
    {
        public GameObject SplineParent;
        public float Duration = 10.0f;
        public eOrientationMode OrientationMode = eOrientationMode.NODE;
        public eWrapMode WrapMode = eWrapMode.ONCE;
        public bool AutoStart = true;
        public bool AutoClose = true;
        public bool HideOnExecute = true;

        private SplineInterpolator mSplineInterp = null;
        private List<Transform> mTransforms = null;

        //@script AddComponentMenu("Splines/Spline Controller")

        void OnDrawGizmos()
        {
            List<Transform> trans = GetTransforms();

            if (trans.Count < 2)
                return;

            var interp = mSplineInterp;
            if (interp == null)
            {
                mSplineInterp = gameObject.AddComponent<SplineInterpolator>();
                interp = mSplineInterp;
            }
            SetupSplineInterpolator(interp, trans);

            interp.StartInterpolation(null, false, WrapMode);

            Vector3 prevPos = trans[0].position;
            for (int c = 1; c <= 100; c++)
            {
                float currTime = c * Duration / 100.0f;
                var currPos = interp.GetHermiteAtTime(currTime);
                float mag = (currPos - prevPos).magnitude * 2.0f;
                Gizmos.color = new Color(mag, 0.0f, 0.0f, 1.0f);
                Gizmos.DrawLine(prevPos, currPos);
                prevPos = currPos;
            }
        }

        void Start()
        {
            mSplineInterp = gameObject.AddComponent<SplineInterpolator>();

            mTransforms = GetTransforms();

            if (HideOnExecute)
                DisableTransforms();

            if (AutoStart)
                FollowSpline();
        }


        public void SetupSplineInterpolator(SplineInterpolator interp, List<Transform> trans)
        {
            interp.Reset();

            float step = 0;

            if (AutoClose)
                step = Duration / trans.Count;
            else
                step = Duration / (trans.Count - 1);

            int c = 0;
            for (; c < trans.Count; c++)
            {
                if (OrientationMode == eOrientationMode.NODE)
                {
                    interp.AddPoint(trans[c].position, trans[c].rotation, step * c, new Vector2(0.0f, 1.0f));
                }
                else if (OrientationMode == eOrientationMode.TANGENT)
                {
                    Quaternion rot;

                    if (c != trans.Count - 1)
                        rot = Quaternion.LookRotation(trans[c + 1].position - trans[c].position, trans[c].up);
                    else if (AutoClose)
                        rot = Quaternion.LookRotation(trans[0].position - trans[c].position, trans[c].up);
                    else
                        rot = trans[c].rotation;

                    interp.AddPoint(trans[c].position, rot, step * c, new Vector2(0.0f, 1.0f));
                }
            }

            if (AutoClose)
                interp.SetAutoCloseMode(step * c);
        }


        // We need this to sort GameObjects by name
        class NameComparer : IComparer<Transform>
        {
            public int Compare(Transform trA, Transform trB)
            {
                return trA.gameObject.name.CompareTo(trB.gameObject.name);
            }
        }


        //
        // Returns children transforms already sorted by name
        //
        public List<Transform> GetTransforms()
        {
            List<Transform> ret = new List<Transform>();

            if (SplineParent != null)
            {
                // We need to use an ArrayList because there´s not Sort method in Array...
                var tempTransformsArray = new List<Transform>();
                var tempTransforms = SplineParent.GetComponentsInChildren<Transform>();

                // We need to get rid of the parent, which is also returned by GetComponentsInChildren...
                foreach (var tr in tempTransforms)
                {
                    if (tr != SplineParent.transform)
                        tempTransformsArray.Add(tr);
                }

                tempTransformsArray.Sort(new NameComparer());
                ret = new List<Transform>(tempTransformsArray);
            }

            return ret;
        }

        //
        // Disables the spline objects, we generally don't need them because they are just auxiliary
        //
        public void DisableTransforms()
        {
            if (SplineParent != null)
            {
                SplineParent.SetActive(false);
            }
        }

        //
        // Starts the interpolation
        //
        public void FollowSpline()
        {
            if (mTransforms.Count > 0)
            {
                SetupSplineInterpolator(mSplineInterp, mTransforms);
                mSplineInterp.StartInterpolation(null, true, WrapMode);
            }
        }
    }
}