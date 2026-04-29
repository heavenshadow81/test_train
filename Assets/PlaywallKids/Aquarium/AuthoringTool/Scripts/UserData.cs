using UnityEngine;
using System.Collections;
using System;

namespace AuthoringTool
{
    public class UserData
    {
        public const int nMax = 10;
        private static UserData _instance;
        private Hashtable templatesDB;

        public enum BrushTool
        {
            NONE = 0,
            ERASER,
            MARKER,
            CRAYON,
            BRUSH,
            SPRAY,
            PALETTE,
            PASTEL
        }

        public enum State
        {
            NONE = 0,
            START,
            WAIT,
            CUSTOMIZING,
            DRAW_START,
            DRAW,
            CUSTOMIZING_BACK,
            SAVE,
            END
        }

        public enum PaletteColor
        {
            NONE = 0,
            WHITE,
            BLACK,
            BLUE,
            BLUEVIOLET,
            DARKGREEN,
            GREEN,
            ORANGE,
            ORANGERED,
            RED,
            SKYBLUE,
            VIOLET,
            VIOLETRED,
            YELLOW,
            YELLOWGREEN,
            PINK
        }

        public enum TemplateType
        {
            NONE = 0,
            TEMPLATE,
            USERTEMPLATE
        }

        protected struct Data
        {
            public PaletteColor paletteColor;
            public State state;
            public BrushTool brushTool;
            public TemplateType templateType;
            public string strTemplateName;
            public int nUserId;
            public int nInstanceId;
            public Vector2[] ToolPosition;
            public Color col;
            public string[] strTemplates;
            public int nBrushSize;
        }

        protected Data[] data = new Data[nMax];
        protected static int nCount = 0;

        public delegate void OnStateChanged(int nInstanceId, int nUserId, State state);
        public event OnStateChanged onStateChanged;

        public bool SetInstanceID(int nItnstanceId, int nUserId)
        {
            int nIndex = GetIndexByUserId(nUserId);
            if (nIndex == -1)
            {
                nIndex = nCount++;
            }

            if (nIndex >= 0 && nIndex < nMax)
            {
                data[nIndex].nInstanceId = nItnstanceId;
                data[nIndex].nUserId = nUserId;
                data[nIndex].strTemplateName = "NONE";
                data[nIndex].brushTool = BrushTool.MARKER;
                data[nIndex].paletteColor = PaletteColor.RED;
                data[nIndex].col = Color.red;
                data[nIndex].state = State.START;
                data[nIndex].templateType = TemplateType.NONE;
                data[nIndex].nBrushSize = 4;

                data[nIndex].ToolPosition = new Vector2[7];
                data[nIndex].ToolPosition[Convert.ToInt32(BrushTool.ERASER) - 1].x = 0;
                data[nIndex].ToolPosition[Convert.ToInt32(BrushTool.ERASER) - 1].y = 0;
                data[nIndex].ToolPosition[Convert.ToInt32(BrushTool.MARKER) - 1].x = 0;
                data[nIndex].ToolPosition[Convert.ToInt32(BrushTool.MARKER) - 1].y = 0;
                data[nIndex].ToolPosition[Convert.ToInt32(BrushTool.CRAYON) - 1].x = 0;
                data[nIndex].ToolPosition[Convert.ToInt32(BrushTool.CRAYON) - 1].y = 0;
                data[nIndex].ToolPosition[Convert.ToInt32(BrushTool.BRUSH) - 1].x = 0;
                data[nIndex].ToolPosition[Convert.ToInt32(BrushTool.BRUSH) - 1].y = 0;
                data[nIndex].ToolPosition[Convert.ToInt32(BrushTool.SPRAY) - 1].x = 0;
                data[nIndex].ToolPosition[Convert.ToInt32(BrushTool.SPRAY) - 1].y = 0;
                data[nIndex].ToolPosition[Convert.ToInt32(BrushTool.PALETTE) - 1].x = 0;
                data[nIndex].ToolPosition[Convert.ToInt32(BrushTool.PALETTE) - 1].y = 0;
                data[nIndex].ToolPosition[Convert.ToInt32(BrushTool.PASTEL) - 1].x = 0;
                data[nIndex].ToolPosition[Convert.ToInt32(BrushTool.PASTEL) - 1].y = 0;

                return true;
            }
            return false;
        }

        public TemplateType GetTemplateType(int nItnstanceId)
        {
            int nIndex = GetIndex(nItnstanceId);

            if (nIndex >= 0 && nIndex < nMax)
                return data[nIndex].templateType;
            else
                return TemplateType.NONE;
        }

        public BrushTool GetBrushTool(int nItnstanceId)
        {
            int nIndex = GetIndex(nItnstanceId);

            if (nIndex >= 0 && nIndex < nMax)
                return data[nIndex].brushTool;
            else
                return BrushTool.NONE;
        }

        public bool SetBrushTool(int nItnstanceId, BrushTool brushTool)
        {
            int nIndex = GetIndex(nItnstanceId);

            if (nIndex >= 0 && nIndex < nMax)
            {
                data[nIndex].brushTool = brushTool;
                return true;
            }
            else
                return false;
        }

        public PaletteColor GetPaletteColor(int nItnstanceId)
        {
            int nIndex = GetIndex(nItnstanceId);

            if (nIndex >= 0 && nIndex < nMax)
                return data[nIndex].paletteColor;
            else
                return PaletteColor.NONE;
        }

        public bool SetPaletteColor(int nItnstanceId, PaletteColor paletteColor)
        {
            int nIndex = GetIndex(nItnstanceId);

            if (nIndex >= 0 && nIndex < nMax)
            {
                data[nIndex].paletteColor = paletteColor;
                return true;
            }
            else
                return false;
        }

        public State GetState(int nItnstanceId)
        {
            int nIndex = GetIndex(nItnstanceId);

            if (nIndex >= 0 && nIndex < nMax)
                return data[nIndex].state;
            else
                return State.NONE;
        }

        public bool SetState(int nInstanceId, State state)
        {
            int nIndex = GetIndex(nInstanceId);

            if (nIndex >= 0 && nIndex < nMax)
            {
                data[nIndex].state = state;
                if (onStateChanged != null)
                    onStateChanged(nInstanceId, nIndex, state);
                return true;
            }
            else
                return false;
        }

        public string GetTemplateName(int nItnstanceId)
        {
            int nIndex = GetIndex(nItnstanceId);

            if (nIndex >= 0 && nIndex < nMax)
                return data[nIndex].strTemplateName;
            else
                return "NONE";
        }

        public bool SetTemplateName(int nItnstanceId, TemplateType templateType, string str)
        {
            int nIndex = GetIndex(nItnstanceId);

            if (nIndex >= 0 && nIndex < nMax)
            {
                data[nIndex].strTemplateName = str;
                data[nIndex].templateType = templateType;
                return true;
            }
            else
                return false;
        }

        public int GetUserId(int nItnstanceId)
        {
            int nIndex = GetIndex(nItnstanceId);

            if (nIndex >= 0 && nIndex < nMax)
                return data[nIndex].nUserId;
            else
                return -1;
        }

        public bool SetUserId(int nItnstanceId, int nUserid)
        {
            int nIndex = GetIndex(nItnstanceId);

            if (nIndex >= 0 && nIndex < nMax)
            {
                data[nIndex].nUserId = nUserid;
                return true;
            }
            else
                return false;
        }

        public void DeleteData(int nItnstanceId)
        {
            for (int i = 0; i < nCount; i++)
            {
                if (data[i].nInstanceId == nItnstanceId)
                {
                    for (int y = i + 1; y < nCount; y++)
                    {
                        data[y - 1] = data[y];
                    }
                    nCount--;
                    for (int z = nCount; z < nMax; z++)
                    {
                        data[z].nInstanceId = 0;
                        data[z].nUserId = 0;
                        data[z].strTemplateName = "NONE";
                        data[z].brushTool = BrushTool.CRAYON;
                        data[z].paletteColor = PaletteColor.WHITE;
                        data[z].col = Color.white;
                        data[z].state = State.WAIT;
                        data[z].templateType = TemplateType.NONE;
                        data[z].nBrushSize = 1;
                    }
                    break;
                }
            }
        }

        public Vector2 GetToolPosition(int nItnstanceId, int tooltype)
        {
            int nIndex = GetIndex(nItnstanceId);

            if (nIndex >= 0 && nIndex < nMax)
                return data[nIndex].ToolPosition[tooltype - 1];
            else
                return new Vector2(0f, 0f);
        }

        public bool SetToolPosition(int nItnstanceId, int tooltype, Vector2 position)
        {
            int nIndex = GetIndex(nItnstanceId);

            if (nIndex >= 0 && nIndex < nMax)
            {
                data[nIndex].ToolPosition[tooltype - 1] = position;
                return true;
            }
            else
                return false;
        }

        public bool SetColor(int nItnstanceId, Color c)
        {
            int nIndex = GetIndex(nItnstanceId);

            if (nIndex >= 0 && nIndex < nMax)
            {
                data[nIndex].col = c;
                return true;
            }
            else
                return false;
        }

        public Color GetColor(int nItnstanceId)
        {
            int nIndex = GetIndex(nItnstanceId);

            if (nIndex >= 0 && nIndex < nMax)
            {
                return data[nIndex].col;
            }
            else
                return new Color(1f, 1f, 1f, 1f);
        }

        public string[] SetTemplates(int nItnstanceId, int nCount, string[] str)
        {
            int nIndex = GetIndex(nItnstanceId);

            if (nIndex >= 0 && nIndex < nMax)
            {
                data[nIndex].strTemplates = str;
                return data[nIndex].strTemplates;
            }
            else
                return null;
        }

        public string[] GetTemplates(int nItnstanceId)
        {
            int nIndex = GetIndex(nItnstanceId);

            if (nIndex >= 0 && nIndex < nMax)
            {
                return data[nIndex].strTemplates;
            }
            else
                return null;
        }

        public static UserData Instance()
        {
            if (_instance == null)
            {
                _instance = new UserData();
            }
            return _instance;
        }

        public int GetIndexByUserId(int nUserId)
        {
            int nIndex = -1;
            for (int i = 0; i < nCount; i++)
            {
                if (data[i].nUserId == nUserId)
                {
                    nIndex = i;
                    break;
                }
                else
                    nIndex = -1;
            }
            return nIndex;
        }

        public int GetIndex(int nInstanceId)
        {
            int nIndex = -1;
            for (int i = 0; i < nCount; i++)
            {
                if (data[i].nInstanceId == nInstanceId)
                {
                    nIndex = i;
                    break;
                }
                else
                    nIndex = -1;
            }
            return nIndex;
        }

        public bool PutCheck()
        {
            if (nCount >= 0 && nCount < nMax)
                return true;
            else
                return false;
        }

        public string GetTemplate(int nItnstanceId, int nIndex)
        {
            return data[GetIndex(nItnstanceId)].strTemplates[nIndex];
        }

        public void SetTemplate(int nItnstanceId, string TemplateName, int nIndex)
        {
            data[GetIndex(nItnstanceId)].strTemplates[nIndex] = TemplateName;
        }

        public void SetBrushSize(int nItnstanceId, int nSize)
        {
            int nIndex = GetIndex(nItnstanceId);

            if (nIndex >= 0 && nIndex < nMax)
            {
                data[nIndex].nBrushSize = nSize;
            }
        }

        public int GetBrushSize(int nItnstanceId)
        {
            int nIndex = GetIndex(nItnstanceId);

            if (nIndex >= 0 && nIndex < nMax)
            {
                return data[nIndex].nBrushSize;
            }
            else
                return 0;
        }

        public void SetDB(Hashtable h)
        {
            templatesDB = h;
        }

        public int getDB()
        {
            return templatesDB.Count;
        }
    }
}