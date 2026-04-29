using UnityEngine;

namespace ML.PlaywallKids.DragonPark.Test
{
    public class AvatarSkeletonTest : MonoBehaviour
    {
        public Transform[] joints;
        public AvatarSkeleton avatar;

        public bool mirrored = false;

        private Vector3[] _cachedPositions;


        void Start()
        {
            if (joints == null || joints.Length == 0)
            {
                AvatarSkeleton skeleton = GetComponent<AvatarSkeleton>();
                if (skeleton != null)
                {
                    joints = skeleton.joints;
                }
            }
            _cachedPositions = new Vector3[joints.Length];
        }

        // Update is called once per frame
        void Update()
        {
            bool moveJoints = false;

            for (int i = 0, cnt = joints.Length; i < cnt; i++)
            {
                if (joints[i] != null && (_cachedPositions[i] - joints[i].localPosition).sqrMagnitude > 0.0001f)
                {
                    _cachedPositions[i] = joints[i].localPosition;
                    moveJoints = true;
                }
            }

            if (moveJoints) avatar.MoveJoints(_cachedPositions, mirrored);
        }

        public void OnDrawGizmos()
        {
            var jointConnections = AvatarSkeleton.jointConnections;
            foreach (AvatarSkeleton.Joint to in jointConnections.Keys)
            {
                AvatarSkeleton.Joint from = jointConnections[to];
                if (joints[(int)from] != null && joints[(int)to] != null)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(joints[(int)from].position, joints[(int)to].position);
                }
            }
        }
    }
}