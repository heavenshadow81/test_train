using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class CanvasForStamp : Canvas_
    {
        public Brush CheckBrush(int _id, string _maskName)
        {
            Brush b = null;
            if (!brushForTouchDict.ContainsKey(_id))
            {
                b = (Brush)brush.Clone();
                brushForTouchDict[_id] = b;
            }
            else
            {
                b = brushForTouchDict[_id];
                if (!string.IsNullOrEmpty(_maskName) && string.Compare(b.maskName, _maskName) != 0)
                { brushForTouchDict[_id] = brush; }
            }

            return b;
        }

        public void Stamp(Brush b, Vector2 _stampPos)
        {
            b.StartPaint(this, _stampPos);
            b.EndPaint(this);
            Flush();
        }

        public void Stamp(TouchInfo _info, Vector2 _stampPos, string _maskName)
        {
            int _touchID = _info.id;
            CheckBrush(_info.id, _maskName);
            Stamp(_info, _stampPos);
        }

        private void Stamp(TouchInfo _userInput, Vector2 _stampPos)
        {
            wantsPaint = true;
            // size of the texture
            Vector2 size = uiTexture.localSize;

            // canvas rect
            Rect rect = new Rect(0, 0, size.x, size.y);

            // iterate all touches
            Vector2 canvasPos = screenToCanvasMatrix * new Vector4(_userInput.position.x, _userInput.position.y, 0, 1.0f);
            if (rect.Contains(canvasPos))
            {
                Vector2 stampPos = _stampPos;
                stampPos.x = UtilityScript.width * 0.5f + stampPos.x;
                stampPos.y = UtilityScript.height * 0.5f + stampPos.y;
                // draw
                base._DrawForTouch(_userInput, stampPos);
                Flush();
            }
            // upload to texture
            wantsPaint = false;
        }

        public void Stamp()
        {
            wantsPaint = true;
            base.Update();
            wantsPaint = false;

            /*
            // size of the texture
            Vector2 size = uiTexture.localSize;

            // canvas rect
            Rect rect = new Rect(0, 0, size.x, size.y);

            // iterate all touches
            TouchInfo[] touches = CustomInput.touches;
            for (int i = 0, touchCount = CustomInput.touchCount; i < touchCount; i++)
            {
                TouchInfo t = touches[i];
                Vector2 canvasPos = screenToCanvasMatrix * new Vector4(t.axisX, t.axisY, 0, 1.0f);

                // check whether touch position is on the canvas region.
                if (rect.Contains(canvasPos))
                {
                    // draw
                    _DrawForTouch(t, ScreenToCanvas(t.position));

                    // if single touch mode, break loop.
                    if (!supportsMultiTouch)
                    {   break;  }
                }
            }

            // upload to texture
            if (touches.Length > 0)
                Flush();
             * */
        }

        static public T CreateCanvas<T>(GameObject _partent, Vector2 _canvasTextureSize) where T : Canvas_
        {
            T _canvas = NGUITools.AddChild<T>(_partent);
            _canvas.wantsPaint = false;
            _canvas.uiTexture.SetAnchor(_partent);
            _canvas.supportsMultiTouch = true;
            _canvas.textureSize = _canvasTextureSize;
            return _canvas;
        }
    }
}