using UnityEngine;
using Windows.Kinect;
using System.Collections.Generic;

namespace ML.PlaywallKids.MotionJump
{
    public enum GameType
    {
        None,
        JumpJump
    }

    public enum BigBoardSize
    {
        None,
        TwoByThree,
        TwoBySix
    }

    public static class GAME_MESSAGE
    {
        public const string LEFT = "Left";
        public const string RIGHT = "Right";
        public const string CENTER = "Center";
        public const string JUMP = "Jump";
    }

    public class JumpJumpData
    {
        public bool inUser = false;
        public bool isJump = false;
        public string command = null;
        public Vector3[] body = new Vector3[20];

        // 점프 계산에 쓰임
        public List<DepthSpacePoint> LeftHandArr;
        public List<DepthSpacePoint> RightHandArr;
        private int FlappingDistance;

        public JumpJumpData()
        {
            FlappingDistance = -1;
            LeftHandArr = new List<DepthSpacePoint>();
            RightHandArr = new List<DepthSpacePoint>();
        }

        public void SetFlappingDistance(int distance)
        {
            FlappingDistance = (int)(distance * 0.75f);
        }

        public int GetFlappingDistance()
        {
            return FlappingDistance;
        }
    }

    public static partial class Constants
    {
        public const int DEFAULT_DEPTH_WIDTH = 512;
        public const int DEFAULT_DEPTH_HEIGHT = 424;

        public const double FILTER_FAR_DISTANCE = 3.1;
        public const double FILTER_DISTANCE = 2.7;
        public const int MAX_USER_COUNT = 2;
        public const int MAX_TRACKING_FRAME = 30;
        public const int MIN_TRACKING_FRAME = 5;

        public const int DIRECTION_ANGLE = 10;
        public const int VERTICAL_ANGLE = 90;

        public const int ENTER_TIME = 5;
        public const int MIN_IN_TIME = 2;
        public const int MAX_OUT_TIME = 3;
    }
}