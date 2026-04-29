
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvatarSkeleton : MonoBehaviour {
    public enum Joint {
        HipCenter = 0,
        Spine,
        ShoulderCenter,
        Head,
        ShoulderLeft,
        ElbowLeft,
        WristLeft,
        HandLeft,
        ShoulderRight,
        ElbowRight,
        WristRight,
        HandRight,
        HipLeft,
        KneeLeft,
        AnkleLeft,
        FootLeft,
        HipRight,
        KneeRight,
        AnkleRight,
        FootRight
    }

    public Transform rootBone;

    public bool movesPosition = false;

    #region Properties
    public static int jointCount
    {
        get
        {
            return System.Enum.GetValues(typeof(Joint)).Length;
        }
    }

    private static Dictionary<string, Joint> _jointValuesOfBoneNames = null;
    public static Dictionary<string, Joint> jointValuesOfBoneNames
    {
        get
        {
            if (_jointValuesOfBoneNames == null)
            {
                _jointValuesOfBoneNames = new Dictionary<string, Joint>();

                _jointValuesOfBoneNames["Spine1"] = Joint.HipCenter;
                _jointValuesOfBoneNames["Spine2"] = Joint.Spine;
                _jointValuesOfBoneNames["Pelvis"] = Joint.HipCenter;
                _jointValuesOfBoneNames["Spine"] = Joint.Spine;
                _jointValuesOfBoneNames["Neck"] = Joint.ShoulderCenter;
                _jointValuesOfBoneNames["Head"] = Joint.Head;
                _jointValuesOfBoneNames["L Clavicle"] = Joint.ShoulderLeft;
                _jointValuesOfBoneNames["L UpperArm"] = Joint.ElbowLeft;
                _jointValuesOfBoneNames["L Forearm"] = Joint.WristLeft;
                _jointValuesOfBoneNames["L Hand"] = Joint.HandLeft;
                _jointValuesOfBoneNames["R Clavicle"] = Joint.ShoulderRight;
                _jointValuesOfBoneNames["R UpperArm"] = Joint.ElbowRight;
                _jointValuesOfBoneNames["R Forearm"] = Joint.WristRight;
                _jointValuesOfBoneNames["R Hand"] = Joint.HandRight;
                _jointValuesOfBoneNames["L Thigh"] = Joint.HipLeft;
                _jointValuesOfBoneNames["L Calf"] = Joint.KneeLeft;
                _jointValuesOfBoneNames["L Foot"] = Joint.AnkleLeft;
                _jointValuesOfBoneNames["L Toe0"] = Joint.FootLeft;
                _jointValuesOfBoneNames["R Thigh"] = Joint.HipRight;
                _jointValuesOfBoneNames["R Calf"] = Joint.KneeRight;
                _jointValuesOfBoneNames["R Foot"] = Joint.AnkleRight;
                _jointValuesOfBoneNames["R Toe0"] = Joint.FootRight;
            }

            return _jointValuesOfBoneNames;
        }
    }

    private static Dictionary<Joint, Joint> _jointConnections = null;
    public static Dictionary<Joint, Joint> jointConnections
    {
        get
        {
            if (_jointConnections == null)
            {
                _jointConnections = new Dictionary<Joint, Joint>();

                _jointConnections[Joint.Spine] = Joint.HipCenter;
                _jointConnections[Joint.ShoulderCenter] = Joint.Spine;
                _jointConnections[Joint.Head] = Joint.ShoulderCenter;
                _jointConnections[Joint.ShoulderLeft] = Joint.ShoulderCenter;
                _jointConnections[Joint.ElbowLeft] = Joint.ShoulderLeft;
                _jointConnections[Joint.WristLeft] = Joint.ElbowLeft;
                _jointConnections[Joint.HandLeft] = Joint.WristLeft;
                _jointConnections[Joint.ShoulderRight] = Joint.ShoulderCenter;
                _jointConnections[Joint.ElbowRight] = Joint.ShoulderRight;
                _jointConnections[Joint.WristRight] = Joint.ElbowRight;
                _jointConnections[Joint.HandRight] = Joint.WristRight;
                _jointConnections[Joint.HipLeft] = Joint.HipCenter;
                _jointConnections[Joint.KneeLeft] = Joint.HipLeft;
                _jointConnections[Joint.AnkleLeft] = Joint.KneeLeft;
                _jointConnections[Joint.FootLeft] = Joint.AnkleLeft;
                _jointConnections[Joint.HipRight] = Joint.HipCenter;
                _jointConnections[Joint.KneeRight] = Joint.HipRight;
                _jointConnections[Joint.AnkleRight] = Joint.KneeRight;
                _jointConnections[Joint.FootRight] = Joint.AnkleRight;
            }
            return _jointConnections;
        }
    }

    private Transform[] _joints = null;
    public Transform[] joints
    {
        get
        {
            if (_joints == null || _joints.Length == 0)
            {
                _FindJoints();
            }
            return _joints;
        }
    }

    private Vector3 _initialPosition = Vector3.zero;

    #endregion

    public void Start()
    {
        // Find root bone automatically
        if (rootBone == null) rootBone = transform.Find("Bip01");
        if (rootBone == null) rootBone = transform.Find("Bip001");

        _initialPosition = transform.localPosition;

        _FindJoints();
    }

    private void _FindJoints()
    {
        if (_joints == null || _joints.Length != jointCount)
        {
            _joints = new Transform[jointCount];
        }

        Transform[] children = rootBone.GetComponentsInChildren<Transform>();

        foreach (Transform t in children)
        {
            string name = t.name.Replace(string.Format("{0} ", rootBone.name), "");

            if (jointValuesOfBoneNames.ContainsKey(name))
            {
                Joint joint = jointValuesOfBoneNames[name];
                _joints[(int)joint] = t;
            }
        }
    }

    public void OnDrawGizmos()
    {
        foreach (Transform t in joints)
        {
            if (t != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(t.position + transform.right, 0.02f);
            }
        }

        foreach (AvatarSkeleton.Joint to in jointConnections.Keys)
        {
            AvatarSkeleton.Joint from = jointConnections[to];
            if (joints[(int)from] != null && joints[(int)to] != null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(joints[(int)from].position + transform.right, joints[(int)to].position + transform.right);
            }
        }
    }

    public void MoveJoints(Vector3[] positions)
    {
        MoveJoints(positions, true);
    }

    public void MoveJoints(Vector3[] positions, bool mirrored)
    {
        if (positions == null || positions.Length < jointCount) return;

        // Move Joints
        foreach (Joint to in jointConnections.Keys)
        {
            if (to == Joint.Head) continue;

            Joint from = jointConnections[to];

            MoveJoint2(from, to, positions, mirrored);
        }

        if (movesPosition)
        {
            Vector3 localPosition = transform.localPosition;
            localPosition.x = 23.0f - (3.0f - 3.0f * (positions[(int)Joint.HipCenter].z - 1000.0f) / 2000.0f);
            localPosition.z = _initialPosition.z + 2.0f - 4.0f * (positions[(int)Joint.HipCenter].x / 1920.0f);

            transform.localPosition = localPosition;
        }
    }

    public void MoveJoint2(Joint from, Joint to, Vector3[] positions)
    {
        MoveJoint2(from, to, positions, true);
    }

    public void MoveJoint2(Joint from, Joint to, Vector3[] positions, bool mirrored)
    {
        if (joints[(int)from] == null || joints[(int)to] == null) return;

        // Variables
        float distance = 0;
        Vector3 direction = Vector3.zero;
        Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;

        // Get current bone's direction and distance between joints.
        direction = joints[(int)to].position - joints[(int)from].position;
        distance = direction.magnitude;

        // Set new position from input
        Vector3 fromPosition = positions[(int)from];
        Vector3 toPosition = positions[(int)to];
        direction = (toPosition - fromPosition).normalized;

        // mirrored?
        if (mirrored)
        {
            from = GetMirroredJoint(from);
            to = GetMirroredJoint(to);
        }

        //Debug.Log(string.Format("joint ({0} -> {1}) direction : ({2:0.0000}, {3:0.0000}, {4:0.0000})", from, to, direction.x, direction.y, direction.z));

        // constrain the hip
        if (to == Joint.HipLeft || to == Joint.HipRight)
        {
            direction.x = 0;
        }

        /*
         * Biped axis
         * 				^
         *  z(rotation) |
         *	 			|	      x
         * Joint(to) --------------> Joint(from)
         * */
        Vector3 right = transform.TransformDirection(-direction);   // connection
        Vector3 forward = joints[(int)to].forward;                  // rotation axis
        Vector3 top = Vector3.Cross(forward, right).normalized;     // get top using cross product
        forward = Vector3.Cross(right, top).normalized;             // cross product again and make sure that three axis is linear independant

        // set!
        joints[(int)to].LookAt(joints[(int)to].position + forward, top);

        /*
        Debug.Log(string.Format(
            "joint ({0} -> {1}) right : ({2:0.0000}, {3:0.0000}, {4:0.0000}), " + 
            "forward : ({5:0.0000}, {6:0.0000}, {7:0.0000})," + 
            "top : ({8:0.0000}, {9:0.0000}, {10:0.0000})", 
            from, to,
            right.x, right.y, right.z, 
            forward.x, forward.y, forward.z, 
            top.x, top.y, top.z));
         */
    }

    /// <summary>
    /// Gets the mirrored joint that exchanges left and right.
    /// </summary>
    /// <returns>The mirrored joint.</returns>
    /// <param name="joint">Joint.</param>
    public Joint GetMirroredJoint(Joint joint)
    {
        switch (joint)
        {
            case Joint.ShoulderLeft:
                return Joint.ShoulderRight;
            case Joint.ElbowLeft:
                return Joint.ElbowRight;
            case Joint.WristLeft:
                return Joint.WristRight;
            case Joint.HandLeft:
                return Joint.HandRight;
            case Joint.ShoulderRight:
                return Joint.ShoulderLeft;
            case Joint.ElbowRight:
                return Joint.ElbowLeft;
            case Joint.WristRight:
                return Joint.WristLeft;
            case Joint.HandRight:
                return Joint.HandLeft;
            case Joint.HipLeft:
                return Joint.HipRight;
            case Joint.KneeLeft:
                return Joint.KneeRight;
            case Joint.AnkleLeft:
                return Joint.AnkleRight;
            case Joint.FootLeft:
                return Joint.FootRight;
            case Joint.HipRight:
                return Joint.HipLeft;
            case Joint.KneeRight:
                return Joint.KneeLeft;
            case Joint.AnkleRight:
                return Joint.AnkleLeft;
            case Joint.FootRight:
                return Joint.FootLeft;
            default:
                return joint;
        }
    }
}
