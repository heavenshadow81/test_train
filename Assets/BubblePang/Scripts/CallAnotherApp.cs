using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;

public class CallAnotherApp : MonoBehaviour
{
    string directorypath = Directory.GetCurrentDirectory();
    string applicationpath = "";
    string argument = "";
    //다른 어플리케이션 부르기!!!
    public void AnotherApplication()
    {

        directorypath = Directory.GetParent(Application.dataPath).Parent.Parent + @"\";
        applicationpath = "WisePang.exe";
        argument = "";
        var a = new Process();
        a.StartInfo.Arguments = argument.Length > 1 ? argument : null;
        print(argument);
        print(a.StartInfo.Arguments);
        a.StartInfo.FileName = directorypath + applicationpath;
        a.Start();
        if (a != null)
        {
            Application.Quit();
        }
    }
}
