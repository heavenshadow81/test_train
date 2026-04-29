using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;
//상위 폴더의 어플리케이션 실행하기: 이름과 무관하게 실행 가능하게 설정
public class CallChildApplication : MonoBehaviour
{
    string directorypath = "";
    string applicationpath = "";
    string argument = "";
    //다른 어플리케이션 부르기!!!

    public void AnotherApplication(int contents)
    {
        if(!Directory.Exists(Application.dataPath + "/Apps"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Apps");
        }
        //Data폴더 내 Apps 폴더 내부 폴더 검색
        var directories = Directory.GetDirectories(Application.dataPath + "/Apps");
        //들어온 변수가 내부 폴더 갯수보다 작을 경우에만..
        if (directories.Length > contents)
        {
            //하위 폴더 경로(몇 번째) 지정
            directorypath = directories[contents];
            //.exe실행 파일만 검색
            var files = Directory.GetFiles(directorypath, "*.exe", SearchOption.TopDirectoryOnly);
            foreach (var application in files)
            {
                //UnityCrashHander가 아니라면
                if (!application.Contains("CrashHandler"))
                {
                    applicationpath = application;
                }
            }
            print(directorypath);
            var a = new Process();
            a.StartInfo.Arguments = argument.Length > 1 ? argument : null;
            print(argument);
            print(a.StartInfo.Arguments);
            a.StartInfo.FileName = applicationpath;
            a.Start();
            if (a != null)
            {
                Application.Quit();
            }
        }
        
    }
}
