using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FunctionButton : MonoBehaviour {
    private Text ButtonText;
    private string ThisFunction;
    private bool _activate;
    public bool _thisShift;
    private void Start()
    {
        ThisFunction = this.gameObject.name;
        ButtonText = this.transform.GetChild(0).GetComponent<Text>();
        _activate = false;
    }
    private void Update()
    {
        if (_thisShift && _activate)
        {
            if (!VirtualKeyboard.Instance.ShiftKey)
            {
                ButtonText.color = Color.white;
                _activate = false;
            }
        }
    }
    public void InputFunctionKey()
    {
        if (ThisFunction == "CapsLock")
        {
            if (!_activate)
            {
                ButtonText.color = Color.green;
                _activate = true;
            }
            else
            {
                ButtonText.color = Color.white;
                _activate = false;
            }
            VirtualKeyboard.Instance.CapsLockKey = !VirtualKeyboard.Instance.CapsLockKey;
        }

        if (ThisFunction == "Shift")
        {
            if (!_activate)
            {
                ButtonText.color = Color.green;
                _activate = true;
            }
            else
            {
                ButtonText.color = Color.white;
                _activate = false;
            }
            VirtualKeyboard.Instance.ShiftKey = !VirtualKeyboard.Instance.ShiftKey;
        }

        if (ThisFunction == "BackSpace")
        {
            VirtualKeyboard.Instance.BackSpaceKey();
        }

        if (ThisFunction == ".com")
        {
            VirtualKeyboard.Instance.Function_com_net(".com");
        }

        if (ThisFunction == ".net")
        {
            VirtualKeyboard.Instance.Function_com_net(".net");
        }

        if (ThisFunction == "Refresh")
        {
            VirtualKeyboard.Instance.FunctionRefresh();
        }
    }
}
