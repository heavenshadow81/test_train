using UnityEngine;
using System.Collections;

public class InteractionPaintBallWall : MonoBehaviour {

    public enum WallType { LEFT, RIGHT, FRONT, BOTTOM }

    public WallType type;

    public Vector3 GetNavigate()
    {
        switch (type)
        {
            case WallType.LEFT: return Vector3.left;
            case WallType.RIGHT: return Vector3.right;
            case WallType.FRONT: return Vector3.forward;
            case WallType.BOTTOM: return Vector3.down;
        }

        return Vector3.zero;
    }
}
