using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;

public class Travel_EmailActivate : MonoBehaviour
{
    #region Email이 같은 PC에 있다면..
    string path = @"C:\EmailSender/TravelEmail.exe";
    
    #endregion
    
    private void OnEnable()
    {
        if (File.Exists(path))
        {
            Process.Start(path);
        }
    }
    private void OnDisable()
    {
        foreach(var process in Process.GetProcesses())
        {
            if (process.ProcessName.StartsWith("TravelEmail"))
            {
                process.Kill();
            }
        }
    }
}
