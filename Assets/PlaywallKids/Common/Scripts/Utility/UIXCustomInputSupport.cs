using UnityEngine;

/// <summary>
/// CustomInput integration class for NGUI.
/// </summary>
public class UIXCustomInputSupport : MonoBehaviour
{
    void Start()
    {
        if (CustomInput.supportsTouch)
        {
            DebugUtil.Log("UIXCustomInputSupport.Start() : Attaching Touch supports...");
            UICamera.GetInputTouchCount = GetCustomInputTouchCount;
            UICamera.GetInputTouch = GetCustomInputTouch;
            //UICamera.onCustomInput = ProcessCustomInputTouches;

            for (int i = 0; i < UICamera.list.size; i++)
            {
                UICamera camera = UICamera.list[i];
                camera.useMouse = CustomInput.receivesMouseInput;
                camera.useTouch = true;
            }
        }
        else
        {
            DebugUtil.Log("UIXCustomInputSupport.Start() : This platform doesn't support touch input.");
            UICamera.GetInputTouchCount = null;
            UICamera.GetInputTouch = null;
            //UICamera.onCustomInput = null;

            for (int i = 0; i < UICamera.list.size; i++)
            {
                UICamera camera = UICamera.list[i];
                camera.useMouse = true;
                camera.useTouch = true;
            }
        }
    }

    protected int GetCustomInputTouchCount()
    {
        int touchCount = 0;

        for (int i = 0; i < CustomInput.touchCount; i++)
        {
            TouchInfo touch = CustomInput.GetTouch(i);
            if (touch.type == TouchInfo.Type.Touch)
                touchCount += 1;
        }

        return touchCount;
    }

    protected UICamera.Touch GetCustomInputTouch(int idx)
    {
        UICamera.Touch result = new UICamera.Touch();
        
        for(int i = 0, idx2 = 0; i < CustomInput.touchCount; i++)
        {
            TouchInfo touch = CustomInput.GetTouch(i);
            if (touch.type != TouchInfo.Type.Touch)
                continue;

            if (idx == idx2)
            {
                result.fingerId = Mathf.Abs(touch.id);
                switch(touch.phase)
                {
                    case TouchInfo.Phase.Begin:
                        result.phase = TouchPhase.Began;
                        break;
                    case TouchInfo.Phase.Move:
                        result.phase = TouchPhase.Moved;
                        break;
                    case TouchInfo.Phase.Stay:
                        result.phase = TouchPhase.Stationary;
                        break;
                    case TouchInfo.Phase.End:
                        result.phase = TouchPhase.Ended;
                        break;
                    case TouchInfo.Phase.Cancel:
                        result.phase = TouchPhase.Canceled;
                        break;
                }
                result.position = touch.position;
                result.tapCount = touch.tapCount;
                break;
            }
            else
            {
                idx2++;
            }
        }

        return result;
    }

    public void ProcessCustomInputTouches()
    {
        UICamera camera = UICamera.current;

        for (int i = 0; i < CustomInput.touchCount; ++i)
        {
            TouchInfo touch = CustomInput.GetTouch(i);
            if (touch.type == TouchInfo.Type.Mouse) continue;

            int currentTouchID = camera.allowMultiTouch ? touch.id : 0;
            UICamera.MouseOrTouch currentTouch = UICamera.GetTouch(currentTouchID, true);
            UICamera.currentTouchID = currentTouchID;
            UICamera.currentTouch = currentTouch;

            bool pressed = (touch.phase == TouchInfo.Phase.Begin) || currentTouch.touchBegan;
            bool unpressed = (touch.phase == TouchInfo.Phase.Cancel) || (touch.phase == TouchInfo.Phase.End);
            currentTouch.delta = pressed ? Vector2.zero : touch.position - currentTouch.pos;
            currentTouch.pos = touch.position;
            UICamera.currentKey = KeyCode.None;

            // Raycast into the screen
            UICamera.Raycast(currentTouch);

            // We don't want to update the last camera while there is a touch happening
            if (pressed) currentTouch.pressedCam = UICamera.currentCamera;
            else if (currentTouch.pressed != null) UICamera.currentCamera = currentTouch.pressedCam;

            // Double-tap support
            if (touch.tapCount > 1) currentTouch.clickTime = RealTime.time;

            // Process the events from this touch
            camera.ProcessTouch(pressed, unpressed);

            // If the touch has ended, remove it from the list
            if (unpressed) UICamera.RemoveTouch(currentTouchID);

            currentTouch.touchBegan = false;
            currentTouch.last = null;
            currentTouch = null;

            // Don't consider other touches
            if (!camera.allowMultiTouch) break;
        }
    }
}