using UnityEngine;
using System;

namespace ML.PlaywallKids.Aquarium
{
    public enum TOOLS_TYPE
    {
        ERASER = 0,
        MARKER,
        CRAYON,
        BRUSH,
        SPRAY,
        PALETTE
    }

    public class UserInfo : MonoBehaviour
    {
        private int UserID;
        private int nR;
        private Vector2[] ToolPosition = new Vector2[6];

        void Init()
        {
            UserID = 1;

            ToolPosition[Convert.ToInt32(TOOLS_TYPE.ERASER)].x = 0;
            ToolPosition[Convert.ToInt32(TOOLS_TYPE.ERASER)].y = 0;
            ToolPosition[Convert.ToInt32(TOOLS_TYPE.MARKER)].x = 0;
            ToolPosition[Convert.ToInt32(TOOLS_TYPE.MARKER)].y = 0;
            ToolPosition[Convert.ToInt32(TOOLS_TYPE.CRAYON)].x = 0;
            ToolPosition[Convert.ToInt32(TOOLS_TYPE.CRAYON)].y = 0;
            ToolPosition[Convert.ToInt32(TOOLS_TYPE.BRUSH)].x = 0;
            ToolPosition[Convert.ToInt32(TOOLS_TYPE.BRUSH)].y = 0;
            ToolPosition[Convert.ToInt32(TOOLS_TYPE.SPRAY)].x = 0;
            ToolPosition[Convert.ToInt32(TOOLS_TYPE.SPRAY)].y = 0;
            ToolPosition[Convert.ToInt32(TOOLS_TYPE.PALETTE)].x = 0;
            ToolPosition[Convert.ToInt32(TOOLS_TYPE.PALETTE)].y = 0;
        }

        // Use this for initialization
        void Start()
        {
            Init();
        }

        public int GetUserID()
        {
            return UserID;
        }

        public void SetUserID(int userid)
        {
            UserID = userid;
            //Debug.Log("UserID " + UserID);
        }

        public Vector2 GetToolPosition(int tooltype)
        {
            return ToolPosition[tooltype];
        }

        public void SetToolPosition(int tooltype, Vector2 position)
        {
            ToolPosition[tooltype] = position;
            //Debug.Log("tooltype " + tooltype +" X " + position.x + " Y " + position.y );
        }
    }
}