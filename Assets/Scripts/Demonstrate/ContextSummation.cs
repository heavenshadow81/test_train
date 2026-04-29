using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.InteropServices;

[Serializable]
class SaveTimesFormat
{
    public string lastTime, summation, mean;
    public List<double> times;
}

public class ContextSummation : MonoBehaviour
{
    
    static ContextSummation instance;
    TimeSpan totalmeanTime;
    List<double> listmeanTime = new List<double>();
    int numbers;
    [SerializeField]
    Text summation, mean, lastsense, today;
    string folderPath;
    string basefolderPath, applicationfolderPath;
    [SerializeField]
    string processName;
    
    public static ContextSummation Instance
    {
        get => instance;
    }
    //시간 저장
    public TimeSpan AddSpan
    {
        set
        {
            if (totalmeanTime.TotalSeconds > 1000)
            {
                totalmeanTime -= totalmeanTime;
                numbers = 0;
            }
            listmeanTime.Add(value.TotalSeconds);
            today.text = $"{DateTime.Now.Month}/{DateTime.Now.Day}";
            numbers++;
            totalmeanTime += value;
            summation.text = $"{numbers} 번";
            lastsense.text = $"{value.TotalSeconds:F3} 초";
            mean.text = $"{(totalmeanTime.TotalSeconds / (float)numbers):F3} 초";
        }
    }
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        folderPath = Application.persistentDataPath + "/saveTime.txt";
        basefolderPath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
        var folders = Directory.GetDirectories(basefolderPath);
        foreach(var folder in folders)
        {
            if (folder.Contains(processName))
            {
                applicationfolderPath = folder;
            }
        }
    }
    private void OnEnable()
    {
        numbers = 0;
        totalmeanTime = new TimeSpan();
        today.text = $"{DateTime.Now.Month}/{DateTime.Now.Day}";
    }
    private void OnDisable()
    {
        SaveTimesFormat saveFormat = new SaveTimesFormat();
        saveFormat.times = listmeanTime;
        saveFormat.mean = mean.text;
        saveFormat.lastTime = lastsense.text;
        saveFormat.summation = numbers.ToString();
        var lists = JsonConvert.SerializeObject(saveFormat);
        File.WriteAllText(folderPath, lists);
        numbers = 0;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string SClassName, string SWindowName);
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr findname);
    [DllImport("user32.dll")]
    private static extern bool ShowWindowAsync(IntPtr findname, int howShow);
    private const int showNORMAL = 1;
    private const int showMINIIZED = 2;
    private const int showMAXMIZED = 3;

    public void ChangeProcess()
    {
        //Process process = new Process();
        //process.StartInfo.FileName = applicationfolderPath + "/" + processName + ".exe";

        //process.Start();
        //Application.Quit();


        //Process currentProcess = Process.GetCurrentProcess();
        //ShowWindowAsync(currentProcess.MainWindowHandle, showMINIIZED);

        Process[] processList = Process.GetProcessesByName("KidsFutureGram");
        
        for (int i = 0; i < processList.Length; i++)
        {
            ShowWindowAsync(processList[i].MainWindowHandle, showMAXMIZED);
            SetForegroundWindow(processList[i].MainWindowHandle);
        }
    }
}
