using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class MixamoChracterJoint : MonoBehaviour
{
    public Transform jointTransform;
    public JointType type;
    public bool Using;
    public MixamoChracterJoint(Transform joinTr, JointType jointType)
    {
        jointTransform = joinTr;
        type = jointType;
        Using = true;
    }
    public MixamoChracterJoint(bool notUse)
    {
        Using = notUse;
    }
}

public class GetJointCharacter : MonoBehaviour {
    public MixamoChracterJoint[] joints;
    public Transform Mytr;
    public Transform[] KinectBones;

	// Use this for initialization
	void Start () {
        Mytr = this.transform;
        joints = new MixamoChracterJoint[25];
        /*
        joints[12] = new MixamoChracterJoint(Mytr.GetChild(0), JointType.HipLeft);
        joints[13] = new MixamoChracterJoint(Mytr.GetChild(0).GetChild(0), JointType.KneeLeft);
        joints[14] = new MixamoChracterJoint(Mytr.GetChild(0).GetChild(0).GetChild(0), JointType.AnkleLeft);
        joints[15] = new MixamoChracterJoint(Mytr.GetChild(0).GetChild(0).GetChild(0).GetChild(0), JointType.FootLeft);
        joints[16] = new MixamoChracterJoint(Mytr.GetChild(1), JointType.HipRight);
        joints[17] = new MixamoChracterJoint(Mytr.GetChild(1).GetChild(0), JointType.KneeRight);
        joints[18] = new MixamoChracterJoint(Mytr.GetChild(1).GetChild(0).GetChild(0), JointType.AnkleRight);
        joints[19] = new MixamoChracterJoint(Mytr.GetChild(1).GetChild(0).GetChild(0).GetChild(0), JointType.FootRight);
        joints[0] = new MixamoChracterJoint(Mytr.GetChild(2), JointType.SpineBase);
        joints[1] = new MixamoChracterJoint(Mytr.GetChild(2).GetChild(0), JointType.SpineMid);
        joints[20] = new MixamoChracterJoint(Mytr.GetChild(2).GetChild(0).GetChild(0), JointType.SpineShoulder);

        joints[2] = new MixamoChracterJoint(joints[20].jointTransform.GetChild(1), JointType.Neck);
        joints[3] = new MixamoChracterJoint(joints[20].jointTransform.GetChild(1).GetChild(0), JointType.Head);

        joints[4] = new MixamoChracterJoint(joints[20].jointTransform.GetChild(0).GetChild(0), JointType.ShoulderLeft);
        joints[5] = new MixamoChracterJoint(joints[20].jointTransform.GetChild(0).GetChild(0).GetChild(0), JointType.ElbowLeft);
        joints[6] = new MixamoChracterJoint(joints[20].jointTransform.GetChild(0).GetChild(0).GetChild(0).GetChild(0), JointType.WristLeft);
        joints[22] = new MixamoChracterJoint(joints[20].jointTransform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(4), JointType.ThumbLeft);

        joints[8] = new MixamoChracterJoint(joints[20].jointTransform.GetChild(2).GetChild(0), JointType.ShoulderRight);
        joints[9] = new MixamoChracterJoint(joints[20].jointTransform.GetChild(2).GetChild(0).GetChild(0), JointType.ElbowRight);
        joints[10] = new MixamoChracterJoint(joints[20].jointTransform.GetChild(2).GetChild(0).GetChild(0).GetChild(0), JointType.WristRight);
        joints[24] = new MixamoChracterJoint(joints[20].jointTransform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(4), JointType.ThumbRight);

        joints[7] = new MixamoChracterJoint(false);
        joints[11] = new MixamoChracterJoint(false);
        joints[21] = new MixamoChracterJoint(false);
        joints[23] = new MixamoChracterJoint(false);*/
        //KinectBones = new Transform[25];
    /*    for (int i = 0; i < KinectBones.Length; i++)
        {
            if (joints[i].Using)
            {
                KinectBones[i] = joints[i].jointTransform;
            }
        }*/
    }
    
}
