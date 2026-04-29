using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using vCatchStation;

public class CodeReachCertification : vCatchBehaviour
{
    [SerializeField] string _contents_code = "Unique name for the content";
    [SerializeField] string _contents_key = "xxxxxx";

    void Awake()
    {
        // register cert detection-types to use
        var certType = new Dictionary<string, JToken>();
        certType.Add("type", "certification");
        certType.Add("required", "CodeReach Contents Certification");
        AddDetectionType(certType);

        Face = null;
    }

    string _certContentsCode = null;
    string _certCertification = null;
    string _genkey = "";

    void Start()
    {
        _certContentsCode = null;
        _certCertification = null;

        // protocol-type 요청 - certification
        _genkey = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString();
        string options = "\"contents\":\""+_contents_code+"\",\"genkey\":\"" + _genkey + "\"";
        DetectionTypeTurnOn("certification", options);
    }

    protected override bool OnData(string protocol, JArray jdata)
    {
        if (protocol != "certification")
            return false;

        if (jdata.Count == 0)
        {
            //Middleware - click 중지됨
            _certContentsCode = "";
            _certCertification = null;
            return true;
        }

        foreach (var jitem in jdata)
        {
            try
            {
                // certification data
                _certContentsCode = (string)jitem["contents"];
                _certCertification = (string)jitem["certification"];
            }
            catch
            {
                Log.w(TAG, "broken certification data detected");
            }
        }

        return true;
    }

    new void Update()
    {
        base.Update();

        SensorProtocolState state = GetProtocolState("certification");
        switch (state)
        {
            case SensorProtocolState.NotConnected: // no vCatchStation
                enabled = false;
                vCatchUnityUtil.MessageAndQuit("Middleware를 찾지 못합니다.");
                break;
            case SensorProtocolState.NotSupported:
                enabled = false;
                vCatchUnityUtil.MessageAndQuit("CodeReach Contents Certification를 설치하세요.");
                break; // no protocol
            case SensorProtocolState.Initialized:
                string cert = _certCertification;
                if (_certContentsCode != null && _certContentsCode != _contents_code)
                    cert = "";
                if (cert == null) // 응답기다리는 중
                    break;
                if (cert == "")
                {
                    // 인증서 없음
                    enabled = false;
                    vCatchUnityUtil.MessageAndQuit("CodeReach 인증서가 없습니다.");
                }
                else // 인증서 비교
                {
                    short sGen = 0;
                    int idxGen = 0;
                    short cg;
                    for (; idxGen < _genkey.Length; idxGen++) {
                        cg = (short)_genkey[idxGen];
                        sGen += cg;
                    }
                    string certMade = "";

                    string strCode = _contents_code + _contents_key;
                    if (strCode.Length < 21)
                    {
                        strCode += "012345678901234567890";
                        strCode = strCode.Substring(0, 21);
                    }

                    idxGen = 0;
                    short cv;
                    for (int idxCode = 0; idxCode < strCode.Length; idxCode++)
                    {
                        cv = (short)strCode[idxCode];

                        sGen += (short)_genkey[idxGen++];
                        if (idxGen >= _genkey.Length) idxGen = 0;

                        double s = Math.Sin(1.5 / sGen);
                        double d = Math.Floor(s * 1000000);
                        int o = (int)(d - (Math.Floor(d / 10) * 10)) + ((sGen + cv) & 0x0f);
                        certMade += (char)('A' + o);

                        sGen += (short)_genkey[idxGen++];
                        if (idxGen >= _genkey.Length) idxGen = 0;

                        s = Math.Sin(1.5 / sGen);
                        d = Math.Floor(s * 1000000);
                        o = (int)(d - (Math.Floor(d / 10) * 10)) + ((sGen + (cv >> 4)) & 0x0f);
                        certMade += (char)('A' + o);
                    }
                    
                    if (certMade == cert)
                    {
                        // 인증성공
                        Log.i(TAG, "CodeReach Certification Verified.");
                        enabled = false;
                    }
                    else
                    {
                        // 인증 실패
                        enabled = false;
                        vCatchUnityUtil.MessageAndQuit("CodeReach 인증서가 무효합니다.");
                    }
                }
                break;
        }
    }

    const string TAG = "AppReqCert";
}
