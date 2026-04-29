using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetButtonText : MonoBehaviour {
    private GameObject MyButton;
    public char MyName;
    public bool isNumber;
    public bool isBar;
    private bool Shift_this;
    private Text MyText;
	// Use this for initialization
	void Start () {
        MyButton = this.gameObject;
        Shift_this = false;
        MyText = this.transform.GetChild(0).GetComponent<Text>();
        char[] arr = MyButton.name.ToCharArray();
        MyName = arr[0];
    }
    public void InputKey()
    {
        VirtualKeyboard.Instance.InputVirtualKey(MyName);
    }
    void Update()
    {
        if (isBar)
        {
            if (VirtualKeyboard.Instance.CapsLockKey || VirtualKeyboard.Instance.ShiftKey)
            {
                if (!Shift_this)
                {
                    MyName = '_';
                    MyText.text = "" + MyName;
                    Shift_this = true;
                }
            }
            if (!VirtualKeyboard.Instance.CapsLockKey && !VirtualKeyboard.Instance.ShiftKey)
            {
                if (Shift_this)
                {
                    MyName = '-';
                    MyText.text = "" + MyName;
                    Shift_this = false;
                }
            }
            return;
        }
        if (!isNumber)
        {
            if (VirtualKeyboard.Instance.CapsLockKey || VirtualKeyboard.Instance.ShiftKey)
            {
                if (!Shift_this)
                {
                    int conv = (int)MyName;
                    conv -= 32;
                    MyName = (char)conv;
                    MyText.text = "" +MyName;
                    Shift_this = true;
                }
            }

            if (!VirtualKeyboard.Instance.CapsLockKey && !VirtualKeyboard.Instance.ShiftKey)
            {
                if (Shift_this)
                {
                    int conv = (int)MyName;
                    conv += 32;
                    MyName = (char)conv;
                    MyText.text = "" + MyName;
                    Shift_this = false;
                }
            }
        }        
    }


}
