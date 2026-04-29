using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Net.NetworkInformation;
using UnityEngine;
using Microsoft.Win32;

namespace ML.PlaywallKids.Launcher
{
    /// <summary>
    /// PC에 설치된 라이센스(인증키, MAC, 만료일)가 유효한지 검사하는 클래스.
    /// </summary>
    public class LicenseChecker : MonoBehaviour
    {
        private static LicenseChecker _instance = null;
        public static LicenseChecker Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("LicenseChecker");
                    go.hideFlags = HideFlags.HideInHierarchy;
                    _instance = go.AddComponent<LicenseChecker>();
                }
                return _instance;
            }
        }

        public const string CheckAuthURL = "http://grownshare.co.kr:9005/rest/checkAuthKey";

        public struct CheckAuthResult
        {
            public bool result;
            public string msg;
            public string validToDate;
        }

        public static void CheckLicense(System.Action<bool, string> handler)
        {
#if UNITY_ANDROID
    Instance.StartCoroutine(CheckAuthAndroid(handler));
#else
            Instance.StartCoroutine(CheckAuth(handler));
#endif
        }

        private static IEnumerator CheckAuth(System.Action<bool, string> handler)
        {
            string registryRoot = string.Format("Software\\{0}\\{1}", Application.companyName, Application.productName);
            string currentUserRoot = string.Format("HKEY_CURRENT_USER\\{0}", registryRoot);
            string localMachineRoot = registryRoot;
            string serialKey = "";
            string macAddress = "";
            string validToDate = (string)Registry.GetValue(currentUserRoot, "ValidToDate", "");
            RegistryKey hklmRKey = null;

            try
            {
                // 라이센스 정보를 레지스트리 (HKLM) 에서 불러온다.
                // 만료일 조회는 HKCU가 우선순위.
                hklmRKey = Registry.LocalMachine.OpenSubKey(localMachineRoot);
                if (hklmRKey != null)
                {
                    serialKey = (string)hklmRKey.GetValue("SerialKey", "");
                    macAddress = (string)hklmRKey.GetValue("MACAddress", "");
                    if (string.IsNullOrEmpty(validToDate))
                        validToDate = (string)hklmRKey.GetValue("ValidToDate", "");
                }
            }
            catch (System.Exception e)
            {
                // SecurityException...
                Debug.LogException(e);
                hklmRKey = null;
            }

            System.DateTime validToDateObj = default(System.DateTime);

#if UNITY_EDITOR
            Debug.Log("Serial Key : " + serialKey);
            Debug.Log("MAC Address : " + macAddress);
            Debug.Log("Valid to date : " + validToDate);
#endif

            bool checkAuth = false;
            bool isPlayable = false;
            string message = "";

            //if (string.IsNullOrEmpty(macAddress) || string.IsNullOrEmpty(serialKey) || string.IsNullOrEmpty(validToDate))
            //    message = "라이센스가 존재하지 않습니다.";
            //else if (!CheckMacAddress(macAddress))
            //    message = "라이센스가 올바르지 않습니다.";
            //else if (!System.DateTime.TryParse(validToDate, out validToDateObj))
            //    message = "라이센스가 올바르지 않습니다.";
            //else
            {
                // 라이센스 온라인 조회 (optional)

                //foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
                //{
                //    if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                //    {
                //        string mac = nic.GetPhysicalAddress().ToString();
                //        string n = $"{mac.Substring(0, 2)}-{mac.Substring(2, 2)}-{mac.Substring(4, 2)}-{mac.Substring(6, 2)}-{mac.Substring(8, 2)}-{mac.Substring(10, 2)}";
                //        print(n);
                //        macAddress = n;
                //    }
                //}
                foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if ((nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet || nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) &&
                        nic.OperationalStatus == OperationalStatus.Up)
                    {
                        IPInterfaceProperties ipProperties = nic.GetIPProperties();
                        foreach (var ip in ipProperties.UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                string mac = nic.GetPhysicalAddress().ToString();
                                //string formattedMac = $"{mac.Substring(0, 2)}-{mac.Substring(2, 2)}-{mac.Substring(4, 2)}-{mac.Substring(6, 2)}-{mac.Substring(8, 2)}-{mac.Substring(10, 2)}";

                                // 베타버전일때 테스트용 배포를 위해 사용
                                string formattedMac = $"{mac.Substring(0, 2)}-{mac.Substring(2, 2)}-{mac.Substring(4, 2)}-{mac.Substring(6, 2)}-{mac.Substring(8, 2)}-{mac.Substring(10, 2)}-TouchBeta";
                                Debug.Log("MAC: " + formattedMac);
                                macAddress = formattedMac;
                                break; // 첫 번째 유효한 IP를 찾으면 반복 중단
                            }
                        }
                    }
                }
                string path = Application.dataPath + @"\lic.da";
                print(path);
                if (!Directory.Exists(Application.dataPath))
                {
                    Directory.CreateDirectory(Application.dataPath);
                }
                if (File.Exists(path))
                {
                    string data = File.ReadAllText(path).Split('\n')[0];
                    print(data);
                    serialKey = data;
                    print(serialKey);
                }

                //// 강제로 값 설정
                //macAddress = "AN-DR-OI-DT-ES-T0";
                //serialKey = "GDAJPDPMS7920R6W";

                //임의 추가된 부분
                string u = $"{CheckAuthURL}?authKey={AESEncrypt(serialKey)}&macAddress={AESEncrypt(macAddress)}";

                //System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(u);
                //req.Method = System.Net.WebRequestMethods.Http.Get;
                //req.Credentials = new System.Net.NetworkCredential("admin", "password");
                //req.Accept = "application/json";
                ////req.Headers["Authorization"] = "Basic" + authInfo;
                //var response = req.GetResponse();
                //string ni;
                //using(var sr = new StreamReader(response.GetResponseStream()))
                //{
                //    ni = sr.ReadToEnd();
                //}
                //print(ni);


#if UNITY_EDITOR
                Debug.Log("AES(serialKey) : " + AESEncrypt(serialKey));
                Debug.Log("AES(macAddress) : " + AESEncrypt(macAddress));
#endif
                WWW www = new WWW(u);

                yield return www;
                Debug.Log(www.text);

#if UNITY_EDITOR
                isPlayable = true;
                message = "에디터에서 실행하였습니다";
#else
                if (string.IsNullOrEmpty(www.error))
                {
                    try
                    {
                        CheckAuthResult result = JsonUtility.FromJson<CheckAuthResult>(www.text);
                        Debug.Log(result.result);
                        if (result.result)
                        {
                            validToDate = result.validToDate;
                            message = result.msg;
                            System.DateTime newValidToDateObj = default(System.DateTime);
                            if (System.DateTime.TryParse(validToDate, out newValidToDateObj))
                                validToDateObj = newValidToDateObj;

                            if (hklmRKey != null)
                            {
                                // 유효날짜를 서버에서 조회한 날짜로 갱신한다.
                                // HKCU와 HKLM 모두 저장.
                                Registry.SetValue(currentUserRoot, "ValidToDate", result.validToDate);
                                try { hklmRKey.SetValue("ValidToDate", result.validToDate); }
                                catch { }
                            }
                        }
                        else
                        {
                            checkAuth = true;
                        }
                    }
                    catch (System.Exception) { }
                }

                // 기간 만료 확인
                if (!checkAuth)
                {
                    System.DateTime currentDateObj = System.DateTime.Now;
                    System.DateTime validToDateObjComp = validToDateObj;

                    // 9999-12-31일 경우 1일 더하면 ArgumentOutOfRangeException 발생 가능하므로 예외처리
                    try { validToDateObjComp = validToDateObj.AddDays(1); }
                    catch { validToDateObjComp = validToDateObj; }

                    if (currentDateObj.CompareTo(validToDateObjComp) <= 0)
                    {
                        isPlayable = true;
                        message = "라이센스가 확인 되었습니다.";
                    }
                    else
                    {
                        message = "라이센스가 만료되었습니다.";
                    }
                }
#endif
            }
            handler?.Invoke(isPlayable, message);
        }

        private static IEnumerator CheckAuthAndroid(System.Action<bool, string> handler)
        {
            // 강제로 값 설정
            string macAddress = "AN-DR-OI-DT-ES-T0";
            string serialKey = "GDAJPDPMS7920R6W";

            // 인증 요청 URL 생성
            string u = $"{CheckAuthURL}?authKey={AESEncrypt(serialKey)}&macAddress={AESEncrypt(macAddress)}";

            // 서버 요청
            WWW www = new WWW(u);
            yield return www;

            bool isPlayable = false;
            string message = "";

            if (string.IsNullOrEmpty(www.error))
            {
                try
                {
                    // 서버 응답 처리
                    CheckAuthResult result = JsonUtility.FromJson<CheckAuthResult>(www.text);
                    Debug.Log($"서버 응답: result={result.result}, msg={result.msg}, validToDate={result.validToDate}");

                    if (result.result)
                    {
                        isPlayable = true;
                        message = result.msg;
                    }
                    else
                    {
                        isPlayable = false;
                        message = "인증 실패: 서버에서 유효하지 않은 키로 확인되었습니다.";
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"JSON 파싱 오류: {e.Message}");
                    isPlayable = false;
                    message = "서버 응답 처리 중 오류가 발생했습니다.";
                }
            }
            else
            {
                Debug.LogError($"네트워크 오류: {www.error}");
                isPlayable = false;
                message = "네트워크 연결에 실패했습니다.";
            }

            // 결과 콜백 호출
            handler?.Invoke(isPlayable, message);
        }


        private static bool CheckMacAddress(string macAddress)
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                byte[] macBytes = nic.GetPhysicalAddress().GetAddressBytes();
                if (macBytes != null && macBytes.Length >= 6)
                {
                    string nicMacAddr = string.Format("{0:X2}-{1:X2}-{2:X2}-{3:X2}-{4:X2}-{5:X2}",
                        macBytes[0], macBytes[1], macBytes[2], macBytes[3], macBytes[4], macBytes[5]);
                    if (macAddress.Equals(nicMacAddr))
                        return true;
                }
            }
            return false;
        }

        private static string AESEncrypt(string input)
        {
            // AES
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 128;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Encoding.UTF8.GetBytes("mogencelab!@#$%^");

            // Encrypt
            var encrypt = aes.CreateEncryptor();
            byte[] buffer = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Encoding.UTF8.GetBytes(input);
                    cs.Write(xXml, 0, xXml.Length);
                }
                buffer = ms.ToArray();
            }

            // byte -> string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                sb.Append(buffer[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
