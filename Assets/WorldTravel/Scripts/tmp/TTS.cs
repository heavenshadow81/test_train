using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using System.IO;
using System;


public class TTS : MonoBehaviour {

	// Use this for initialization
	void Start () {

        string text = "좋은 하루 되세요."; // 음성합성할 문자값
        string url = "https://openapi.naver.com/v1/voice/tts.bin";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Headers.Add("X-Naver-Client-Id", "BQyk5aCZpaa4BPUAIRHD");
        request.Headers.Add("X-Naver-Client-Secret", "9Q8U2bL8Od");
        request.Method = "POST";
        byte[] byteDataParams = Encoding.UTF8.GetBytes("speaker=mijin&speed=0&text=" + text);
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = byteDataParams.Length;
        Stream st = request.GetRequestStream();
        st.Write(byteDataParams, 0, byteDataParams.Length);
        st.Close();
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        string status = response.StatusCode.ToString();
        Debug.Log("status=" + status);
        using (Stream output = File.OpenWrite("E:/work/tts.mp3"))
        using (Stream input = response.GetResponseStream())
        {
            //input.CopyTo(output);
        }
       // Debug.Log("c:/tts.mp3 was created");


    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
