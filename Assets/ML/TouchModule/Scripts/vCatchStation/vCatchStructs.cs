using System.Runtime.InteropServices;

namespace ML.TouchModule.vCatchStation
{
    /// <summary>
    /// 상태 코드
    /// </summary>
    public enum StatusCode : int
    {
        No_Sensor = 1,
        //	NG_USB              = 2,
        No_vCatchModule = 20,
        vCatch_IsNotStarted = 21,
        vCatch_IsNotWorking = 22,
        Stopped_ByCommand = 50,
        Stopped_ByTimeout = 51,
        Stopped_NotReady = 52,      // 센서와 준비작업에서 오류가 발생
        //	Stopped_NoSetting   = 53,
        Stopped_LackOfData = 54,    // 센서로 부터 일부 정보를 받지 못함
        No_Error = 0
    };

    /// <summary>
    /// 터치 모듈 ID
    /// </summary>
    public enum ModuleID : int
    {
        None = 0,
        Curling1 = 1,
        TwoDimen = 4,
        TwoDimenSpeed = 5,
        Trampoline = 6
    }

    public struct vCatchResult_SensorStatus
    {
        public ushort idModule;
        public ushort version;
        public uint msec;
        public int idSensor;
        public StatusCode code;
    }

    public struct vCatchResult_Stopped
    {
        public ushort idModule;
        public ushort version;
        public uint msec;
        public int idSensor;
        public StatusCode code;
    }

    public struct vCatchResult_Points
    {
        public ushort idModule;
        public ushort version;
        public uint msec;
        public ushort idPoint;
        public ushort status;
        public float posX;   // 0 ~ 1
        public float posY;   // 0 ~ 1
        public float width;  // 0 ~ 1
        public float height; // 0 ~ 1
    }

    public struct vCatchResult_1D
    {
        public ushort idModule;
        public ushort version;
        public uint msec;
        public int idSensor;
        public float pos;   // 0 ~ 1
        public float width; // 0 ~ 1
    }

    public struct vCatchResult_2D
    {
        public ushort idModule;
        public ushort version;
        public uint msec;
        public int idSensor;
        public float posX;   // 0 ~ 1
        public float posY;   // 0 ~ 1
        public float width;  // 0 ~ 1
        public float height; // 0 ~ 1
    }

    public struct vCatchResult_2DS
    {
        public ushort idModule;
        public ushort version;
        public uint msec;
        public int idSensor;
        public float posX;      // 0 ~ 1
        public float posY;      // 0 ~ 1
        public float width;     // 0 ~ 1
        public float height;    // 0 ~ 1
        public float speed;     // km
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct vCatchResult
    {
        [FieldOffset(0)]
        public uint module_status;
        [FieldOffset(0)]
        public vCatchResult_SensorStatus sensor_status;
        [FieldOffset(0)]
        public vCatchResult_Stopped stopped;
        [FieldOffset(0)]
        public vCatchResult_Points resultPoints;
        [FieldOffset(0)]
        public vCatchResult_1D result1D;
        [FieldOffset(0)]
        public vCatchResult_2D result2D;
        [FieldOffset(0)]
        public vCatchResult_2DS result2DS;
    }
}