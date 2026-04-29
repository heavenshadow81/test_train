using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class VirtualKeyboard : MonoBehaviour {
    public static VirtualKeyboard Instance;
    public bool ShiftKey;
    public bool CapsLockKey;
    /// <summary>
    /// this is InputBox
    /// </summary>
    public Text InputBox;
    public InputField InputBox_Field;
    // Use this for initialization
    void Start() {
        Instance = this;
        ShiftKey = false;
        CapsLockKey = false;
    }

    public void InputVirtualKey(char key)
    {
        if (InputBox != null)
        {
            string Box_text = InputBox.text;
            Box_text += key;
            InputBox.text = Box_text;
            if (ShiftKey)
                ShiftKey = false;
        }
        if (InputBox_Field != null)
        {
            string Box_text = InputBox_Field.text;
            Box_text += key;
            InputBox_Field.text = Box_text;
            if (ShiftKey)
                ShiftKey = false;
        }
    }
    public void FunctionRefresh()
    {
        if (InputBox_Field != null)
        {
            InputBox_Field.text = "";
        }
    }

    public void BackSpaceKey()
    {
        if (InputBox != null)
        {
            char[] box = InputBox.text.ToCharArray();
            int idx = box.Length - 1;
            string tmp = "";
            for (int i = 0; i < idx; i++)
            {
                tmp += box[i];
            }
            InputBox.text = tmp;
        }
        if (InputBox_Field != null)
        {
            char[] box = InputBox_Field.text.ToCharArray();
            int idx = box.Length - 1;
            string tmp = "";
            for (int i = 0; i < idx; i++)
            {
                tmp += box[i];
            }
            InputBox_Field.text = tmp;
        }

    }
    public void Function_com_net(string str)
    {
        if (InputBox != null)
        {
            string Box_text = InputBox.text;
            Box_text += str;
            InputBox.text = Box_text;
        }
        if (InputBox_Field != null)
        {
            string Box_text = InputBox_Field.text;
            Box_text += str;
            InputBox_Field.text = Box_text;
        }
    }
}
