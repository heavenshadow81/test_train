using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;
public class CallParentApplication : MonoBehaviour
{
    string directorypath = Directory.GetCurrentDirectory();
    string applicationpath = "";
    string argument = "";
    //다른 어플리케이션 부르기!!!
    public void AnotherApplication(int contents)
    {
        print(Directory.GetCurrentDirectory());
        print(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName);
        var application = Directory.GetFiles(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "*.exe", SearchOption.TopDirectoryOnly);
        foreach(var app in application)
        {
            print(app);
            if (!app.Contains("CrashHandler"))
            {
                applicationpath = app;
            }
        }
        print(applicationpath);
        var a = new Process();
        a.StartInfo.Arguments = argument.Length>1 ? argument : null;
        print(argument);
        print(a.StartInfo.Arguments);
        a.StartInfo.FileName = applicationpath;
        a.Start();
        if(a != null)
        {
            Application.Quit();
        }
    }
}
