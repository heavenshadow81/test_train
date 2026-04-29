using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using vCatchStation;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;
using VirtualTouch;
using static TouchInjection.TouchInjector;

public class MultiModal : vCatchDisplay, vCatchInput_iMmPlayer
{
    /*    [Space(10)]
        public TextMeshProUGUI handCount;   // 입력 손의 갯수 텍스트
        public TextMeshProUGUI inputCount;  // 누른 횟수 텍스트
        public TextMeshProUGUI touchPos;    // 터치 위치 텍스트
        public int inputCounting = 0;   // 누른 횟수
    */
    VMouse vMouse = new();

    new protected void Awake()
    {
        // 사용할 Face목록
        string[] aryFace = { "W1" };
        Faces = aryFace;

        base.Awake();

        vCatchInput_MmPlayer.AddEventListener(this);
        InitializeTouchInjection();
    }

    //int step = 0;

    new protected void Update()
    {
        base.Update();

        vClick[] Clicks = vCatchInput_Click.vClicks(targetDisplay);
        if (Clicks.Length > 0)
        {
            vMouse.Tap(Clicks[0].x, Clicks[0].y, 0, 0, 1920, 1080);
            //VTouch.Tap(Clicks[0].x, Clicks[0].y, 0, 0, 1920, 1080);
            Trace.WriteLine("Click len:" + Clicks.Length);
        }

        vDrag[] Drags = vCatchInput_Drag.vDrags(targetDisplay);
        //if (Drags.Length > 0)
        //{
        //    Trace.WriteLine("Drag len:" + Drags.Length);
        //    /*handCount.text = "Drag Counting : " + Clicks.Length.ToString();  // 입력 중인 손의 갯수
        //    inputCount.text = "Drag HandCount : " + (++inputCounting).ToString(); // 입력 횟수 출력*/
        //}

        //vMmB[] Bs = vCatchInput_MmB.vMmBs(targetDisplay);
        //if (Bs.Length > 0)
        //    Trace.WriteLine("Bs len:" + Bs.Length);

        //vMmJB[] JBs = vCatchInput_MmJB.vMmJBs(targetDisplay);
        //if (JBs.Length > 0)
        //    Trace.WriteLine("JBs len:" + JBs.Length);

        //vMmAGB[] AGBs = vCatchInput_MmAGB.vMmAGBs(targetDisplay);
        //if (AGBs.Length > 0)
        //    Trace.WriteLine("AGBs len:" + AGBs.Length);

        //vMmAGJB[] AGJBs = vCatchInput_MmAGJB.vMmAGJBs(targetDisplay);
        //if (AGJBs.Length > 0)
        //    Trace.WriteLine("AGJBs len:" + AGJBs.Length);
        ////Log.i("MM", "AGJB len : " + AGJBs.Length);//!!

        //vMmP[] Ps = vCatchInput_MmP.vMmPs(targetDisplay);
        //if (Ps.Length > 0)
        //    Trace.WriteLine("Ps len:" + Ps.Length);
    }

    public void vCatchInput_MmPlayer_OnMmPlayer() // 디바이스 연결상태 변화되면...
    {
        JArray ids = new JArray();

        vMmPlayer[] players = vCatchInput_MmPlayer.vMmPlayers(targetDisplay);
        string log = "";
        foreach (var p in players)
        {
            ids.Add(p.id);

            int check = 0;

            log += "  player:" + p.id;
            if (p.parts != null)
            {
                log += " " + p.parts;
                check++;
            }
            if (p.batlevel == null || p.batlevel >= 0.0f)
            {
                log += " " + p.batlevel;
                check++;
            }
            if (p.charging != null)
                log += " " + p.charging;

            if (check >= 2) // 모델명, 상태 정보를 다 받으면 data요청
            {
                JObject cmdC = new JObject();
                JArray idsC = new JArray();
                idsC.Add(p.id);
                cmdC.Add("pids", idsC);
                cmdC.Add("connect", "data");
                DetectionTypeTurnOn(SensorProtocolTypeName(sensorProtocolType),
                    "\"cmd\":" + cmdC.ToString(Formatting.None));
            }
        }
        Trace.WriteLine(log);
    }

    public void SelectPlayer()
    {
        vMmPlayer[] players = vCatchInput_MmPlayer.vMmPlayers(targetDisplay);
        if (players.Length <= 0)
        {
            Trace.WriteLine("Not enough player");
            return;
        }

        // 연결된 모든 디바이스에서 첫번째만 사용할 것임을 선언 - (추가 연결 방지됨)
        JArray ids = new JArray();
        ids.Add(players[0].id);
        string nameType = SensorProtocolTypeName(sensorProtocolType);
        DetectionTypeTurnOn(nameType, "\"player-ids\":" + ids.ToString(Formatting.None));
    }

    public void OpenAllPlayers()
    {
        // 추가 연결 허가
        string nameType = SensorProtocolTypeName(sensorProtocolType);
        DetectionTypeTurnOn(nameType, "\"player-ids\":[]");
    }

    public void RumbleAllPlayers()
    {
        JArray ids = new JArray();

        vMmPlayer[] players = vCatchInput_MmPlayer.vMmPlayers(targetDisplay);
        string log = "";
        foreach (var p in players)
        {
            ids.Add(p.id);

            log += "  player:" + p.id;
            if (p.parts != null)
                log += " " + p.parts;
            if (p.batlevel != null)
                log += " " + p.batlevel;
            if (p.charging != null)
                log += " " + p.charging;
        }
        Trace.WriteLine(log);

        // 연결된 모든 디바이스에게 명령보내는 예제
        JObject cmd = new JObject();
        cmd.Add("pids", ids);
        //cmd.Add("connect", "data");
        cmd.Add("rumble", "100 50 1");
        //cmd.Add("color", "6f0f");
        string nameType = SensorProtocolTypeName(sensorProtocolType);
        DetectionTypeTurnOn(nameType, "\"cmd\":" + cmd.ToString(Formatting.None));
    }
}
