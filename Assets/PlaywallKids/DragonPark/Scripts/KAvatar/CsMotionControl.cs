using UnityEngine;
using System.Collections;

public class CsMotionControl : MonoBehaviour {
    public Transform bone;

    public int jointCount = 16;

    public Transform[] joints;

    public bool enablesFootPositionCorrection = true;

    private int[] JointAngleValue = new int[] {
            1,2,2,2,1,1,2,2,2,1,
            2,1,2,2,1,2,0,0,0,0,
            0,0,0,0,0,0
        };
	
	//                        Index :     0, 1, 2, 3, 4, 5, 6, 7, 8, 9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25  
	private int [] JointAngleAbsValue = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};	
    private int [] RelJointValue =      { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};		
    private int [] JointDirValue =      { 1, 1, 1,-1,-1, 1,-1,-1, 1,-1,-1, 1,-1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};

    // Joint 3, 8 axis (for ignoring gimbol-lock)
    private Transform Joint3AxisRefTf, Joint8AxisRefTf;

    // Baseline (bottom-center) position
    private Vector3 _baselineBottomCenterFootPosition = Vector3.zero;

    public void Awake()
    {
        _FindJoints();
        if (joints.Length > 9 && joints[4] != null && joints[9] != null)
        {
            _baselineBottomCenterFootPosition = (joints[4].position + joints[9].position) * 0.5f;
        }
    }

    private void _FindJoints()
    {
        if (jointCount < 16) jointCount = 16;
        else if (jointCount > 26) jointCount = 26;

        if(joints == null || joints.Length < jointCount)
        {
            Transform[] newJoints = new Transform[jointCount];
            if (joints != null)
            {
                for (int i = 0; i < joints.Length; i++)
                {
                    newJoints[i] = joints[i];
                }
            }
            joints = newJoints;
        }

        if (bone == null) bone = transform.Find("Bip01");
        if (bone == null) bone = transform.Find("Bip001");

        if (bone != null)
        {
            Transform[] tfs = bone.GetComponentsInChildren<Transform>();
            foreach (Transform tf in tfs)
            {
                string boneName = tf.name;

                if (boneName.Contains("L Thigh"))
                {
                    if (joints[0] == null) joints[0] = tf;
                    if (joints[1] == null) joints[1] = tf;
                }
                else if (boneName.Contains("L Calf"))
                {
                    if (joints[2] == null) joints[2] = tf;
                }
                else if (boneName.Contains("L Foot"))
                {
                    if (joints[3] == null) joints[3] = tf;
                    if (joints[4] == null) joints[4] = tf;

                    if (joints[3] != null)
                    {
                        Joint3AxisRefTf = new GameObject("joint3_ref").transform;
                        Joint3AxisRefTf.parent = joints[3].parent;
                        Joint3AxisRefTf.position = joints[3].position + joints[3].forward;
                    }
                }
                else if (boneName.Contains("R Thigh"))
                {
                    if (joints[5] == null) joints[5] = tf;
                    if (joints[6] == null) joints[6] = tf;
                }
                else if (boneName.Contains("R Calf"))
                {
                    if (joints[7] == null) joints[7] = tf;
                }
                else if (boneName.Contains("R Foot"))
                {
                    if (joints[8] == null) joints[8] = tf;
                    if (joints[9] == null) joints[9] = tf;

                    if (joints[8] != null)
                    {
                        Joint8AxisRefTf = new GameObject("joint8_ref").transform;
                        Joint8AxisRefTf.parent = joints[8].parent;
                        Joint8AxisRefTf.position = joints[8].position + joints[8].forward;
                    }
                }
                else if (boneName.Contains("L UpperArm"))
                {
                    if (joints[10] == null) joints[10] = tf;
                    if (joints[11] == null) joints[11] = tf;
                }
                else if (boneName.Contains("L Forearm"))
                {
                    if (joints[12] == null) joints[12] = tf;
                }
                else if (boneName.Contains("R UpperArm"))
                {
                    if (joints[13] == null) joints[13] = tf;
                    if (joints[14] == null) joints[14] = tf;
                }
                else if (boneName.Contains("R Forearm"))
                {
                    if (joints[15] == null) joints[15] = tf;
                }
            }
        }
    }
	
	// JS I/F Part.******************************************************************************//
	public void CommandPose(string poseText)
	{
		int nNum =0;
		
		// 각 축의 Joint Value를 분해한다.
		string [] jointAngle = poseText.Split(',');
		
		//JointAngleAbsValue값을 참조로, RelJointValue값을 구함.
        for (nNum = 0; nNum < jointCount; nNum++)
        {
            float parsedJointAngle = 0.0f;
            float.TryParse(jointAngle[nNum], out parsedJointAngle);

            // Calculates new angle
            RelJointValue[nNum] = Mathf.FloorToInt(parsedJointAngle) - JointAngleAbsValue[nNum];
            float angle = JointDirValue[nNum]*RelJointValue[nNum];

            // Get axis of joint
            Vector3 axis = GetJointAxis(nNum);

            // Get angle axis quaternion
            Quaternion angleAxis = Quaternion.AngleAxis(angle, axis);

            // Rotate the current joint
            Quaternion rotation = joints[nNum].rotation;
            rotation = angleAxis * rotation;
            joints[nNum].rotation = rotation;
            
            // Saves current angle value
            JointAngleAbsValue[nNum] = Mathf.FloorToInt(parsedJointAngle);
        }
	}

    public Vector3 GetJointAxis(int joint)
    {
        Vector3 axis = Vector3.zero;

        if (joint >= 0 && joint < jointCount)
        {
            if (joint == 0 || joint == 5)
            {
                axis = transform.forward;
            }
            else if (joint == 10 || joint == 13)
            {
                axis = transform.right;
            }
            else if (joint == 3 && Joint3AxisRefTf != null)
            {
                axis = Joint3AxisRefTf.position - joints[3].position;
            }
            else if (joint == 8 && Joint8AxisRefTf != null)
            {
                axis = Joint8AxisRefTf.position - joints[8].position;
            }
            else
            {
                int angleValue = JointAngleValue[joint];

                if (angleValue == 0) axis = joints[joint].right;
                else if (angleValue == 1) axis = joints[joint].up;
                else axis = joints[joint].forward;
            }
        }

        return axis;
    }

    void LateUpdate()
    {
        if (enablesFootPositionCorrection)
        {
            // position correction
            _PerformBottomCenterFootPositionCorrection();

            // rotation correction
            /*
             * float r1 = joint4.rotation.eulerAngles.z;
            float r2 = joint9.rotation.eulerAngles.z;

            float r = (r1 + r2) * 0.5f;
            Debug.Log("rotation : " + r);

            Quaternion rotation = bone.localRotation;
            rotation.SetEulerAngles(-r, 90.0f, 0.0f);
            bone.localRotation = rotation;
            */
            // position correction again!
            _PerformBottomCenterFootPositionCorrection();
        }
    }

    private void _PerformBottomCenterFootPositionCorrection()
    {
        Vector3 bottomCenterPos = Vector3.zero;
        if (joints.Length > 9 && joints[4] != null && joints[9] != null)
        {
            bottomCenterPos = (joints[4].position + joints[9].position) * 0.5f;
        }
        Vector3 pos = bone.position;
        pos.x -= (bottomCenterPos - _baselineBottomCenterFootPosition).x;
        pos.y -= (bottomCenterPos - _baselineBottomCenterFootPosition).y;
        bone.position = pos;
    }
}
