using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
public class CheckAnotherCall : MonoBehaviour
{
    private void OnEnable()
    {
        var a = Process.GetProcessesByName("Puzzle")[0];
        print(a.StartInfo.Arguments);
        if (a.StartInfo.Arguments != "Bax5380058!")
        {
            Application.Quit();
        }
    }
}
