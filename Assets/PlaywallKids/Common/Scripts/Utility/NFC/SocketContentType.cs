//#define USE_TAG

using UnityEngine;
#if USE_TAG
using C = ML.PlaywallKids.Common.NFCConstants;
#endif

public class SocketContentType : MonoBehaviour
{
#if USE_TAG
    public enum EContentType { NONE, T2D_DRAWING, T3D_DRAWING, KICKBALL, THROW_PAINTBALL, THE_FRISBEE, TOUCH_SLIME, MOTION_JUMP }
    public EContentType contentType;
    
    static string[] contentTypes = new string[] { 
        "",
        C.VALUE_2D_DRAWING,
        C.VALUE_3D_DRAWING,
        C.VALUE_KICKBALL,
        C.VALUE_THROW_PAINTBALL,
        C.VALUE_THE_FRISBEE,
        C.VALUE_TOUCH_SLIME,
        C.VALUE_MOTION_JUMP
    };
    
    void OnEnable()
    {
        NFCClientSocket.instance.ContentType = contentTypes[(int)contentType];
    }
#endif
}
