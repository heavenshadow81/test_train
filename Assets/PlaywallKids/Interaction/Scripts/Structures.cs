using UnityEngine;
using Windows.Kinect;
using System.Collections.Generic;

public static partial class Constants
{
    public const string DEFAULT_SERVER_IP = "127.0.0.1";
    public const int DEFAULT_SERVER_PORT = 7766;

    public const int DEFAULT_DEPTH_WIDTH = 512;
    public const int DEFAULT_DEPTH_HEIGHT = 424;

    public const float DEFAULT_DEPTH_RATIO_HEIGHT = 9f;
    public const float DEFAULT_DEPTH_RATIO_WIDTH = 10.86f;

    public const int DEFAULT_COLOR_WIDTH = 1920;
    public const int DEFAULT_COLOR_HEIGHT = 1080;

    public const float DEFAULT_RATIO_WIDTH = 16f;
    public const float DEFAULT_RATIO_HEIGHT = 9f;

    public const float DEFAULT_RATIO_WIDTH_2x2 = 16f;
    public const float DEFAULT_RATIO_WIDTH_2x3 = 24f;
    public const float DEFAULT_RATIO_WIDTH_2x6 = 48f;


    // 24 : 9 Image Color Size
    public const int COLOR_WIDTH_2X3 = (int)(((float)DEFAULT_DEPTH_WIDTH / (float)DEFAULT_RATIO_WIDTH_2x2) * DEFAULT_RATIO_WIDTH_2x3);
    public const int COLOR_HEIGHT_2X3 = DEFAULT_COLOR_HEIGHT;
    // 24 : 9 Image Depth Size
    public const int DEPTH_WIDTH_2X3 = (int)(((float)DEFAULT_DEPTH_WIDTH / (float)DEFAULT_DEPTH_RATIO_WIDTH) * DEFAULT_RATIO_WIDTH_2x3);
    public const int DEPTH_HEIGHT_2X3 = DEFAULT_DEPTH_HEIGHT;
    // 48 : 9 Image Color Size
    public const int COLOR_WIDTH_2X6 = (int)(((float)DEFAULT_COLOR_WIDTH / (float)DEFAULT_RATIO_WIDTH_2x2) * DEFAULT_RATIO_WIDTH_2x6);
    public const int COLOR_HEIGHT_2X6 = DEFAULT_COLOR_HEIGHT;
    // 48 : 9 Image Depth Size
    public const int DEPTH_WIDTH_2X6 = (int)(((float)DEFAULT_DEPTH_WIDTH / (float)DEFAULT_DEPTH_RATIO_WIDTH) * DEFAULT_RATIO_WIDTH_2x3);
    public const int DEPTH_HEIGHT_2X6 = DEFAULT_DEPTH_HEIGHT;


    public const double HandLength = 0.4;
    public const double MinArc = 80;
    public const double MaxArc = 110;

    public const int EffectFrameCount = 20;
    public const double PunchingDistance = 0.3;
    public const double KickDistance = 0.4;

    public const int MinUserDepth = 2;
    public const double ReadyThreshold = 0.1;
    public const double DepthThreshold = 0.3;

    public const int ParticleMinDstance = 1500;
}

public enum ContentsType
{
    None,
    JumpJump,
    Interaction
}

public enum MotionType
{
    HeartMotion,
    LampMotion,
    PunchMotion,
    KickMotion,
    PunchNKickMotion,
    ThrowMotion,
    None
}

public enum ImageType
{
    None,
    Original,               // 현재는 사용하면 에러발생할 수 있음.
    ParticleBody,
    ShadowBody,
    Chromakey
}

public enum InputType
{
    None,
    Color,
    Depth
}
public class UserData
{
    //public int index;
    //public int userNum;
    public bool isReadyPunchLeft;
    public bool isReadyPunchRight;
    public bool isReadyKickLeft;
    public bool isReadyKickRight;

    public float userID;
    public int shiftValue;
    public Dictionary<JointType, Windows.Kinect.Joint> joints = null;


    public Vector2 pHead = new Vector2();
    public Vector2 pHandLeft = new Vector2();
    public Vector2 pHandRight = new Vector2();
    public Vector2 pElbowLeft = new Vector2();
    public Vector2 pElbowRight = new Vector2();
    public Vector2 pShoulderLeft = new Vector2();
    public Vector2 pShoulderRight = new Vector2();
    public Vector2 pFootLeft = new Vector2();
    public Vector2 pFootRight = new Vector2();

    public List<Vector3> PunchDepthLeft = null;
    public List<Vector3> PunchDepthRight = null;
    public List<Vector3> KickDepthLeft = null;
    public List<Vector3> KickDepthRight = null;
    public List<HandState> HandStateLeft = null;
    public List<HandState> HandStateRight = null;

    public UserData()
    {
        isReadyPunchLeft = true;
        isReadyPunchRight = true;
        isReadyKickLeft = true;
        isReadyKickRight = true;

        PunchDepthLeft = new List<Vector3>();
        PunchDepthRight = new List<Vector3>();
        KickDepthLeft = new List<Vector3>();
        KickDepthRight = new List<Vector3>();
        HandStateLeft = new List<HandState>();
        HandStateRight = new List<HandState>();
    }

    public void ClearDepthData()
    {
        isReadyPunchLeft = true;
        isReadyPunchRight = true;
        isReadyKickLeft = true;
        isReadyKickRight = true;

        PunchDepthLeft.Clear();
        PunchDepthRight.Clear();
        KickDepthLeft.Clear();
        KickDepthRight.Clear();
        HandStateLeft.Clear();
        HandStateRight.Clear();
    }
}

public class ClusterColor
{
    public byte R, G, B;

    public ClusterColor(byte R, byte G, byte B)
    {
        this.R = R;
        this.G = G;
        this.B = B;
    }
}