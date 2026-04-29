using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Text;
using ML.MapoContents.WorldTravel;
public class tsjtsnkjdnf : MonoBehaviour {

	// Use this for initialization
	void Start () {
        try
        {
            string nowPath = "C:/Users/mogencelab/AppData/LocalLow/DefaultCompany/MapoContents_Travel/ScreenShotEmail/163d47b9-65b5-4e39-a6a8-57d3863e2387";
            Directory.Delete(nowPath, true);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
