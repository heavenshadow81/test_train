using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Microsoft.Win32;

namespace ML.T_Sports.Common
{
    /// <summary>
    /// PC에 설치된 라이센스(인증키, MAC, 만료일)가 유효한지 검사하는 클래스.
    /// </summary>
    public class LicenseChecker : MonoBehaviour
    {
        public Text text;
        public const string CheckAuthURL = "http://grownshare.co.kr:9005/rest/checkAuthKey";

        public struct CheckAuthResult
        {
            public bool result;
            public string msg;
            public string validToDate;
        }

        private void Start()
        {
            StartCoroutine(LicenseCheck());
        }
        IEnumerator LicenseCheck()
        {
            text.text = "라이센스를 확인중 입니다...";
            yield return new WaitForSeconds(2f);
            StartCoroutine(CheckAuth());
        }
        IEnumerator LicenseCheckComplete()
        {
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene("00TouchSports_Main");
        }
        IEnumerator QuitProgram()
        {
            yield return new WaitForSeconds(2f);
            Application.Quit();
        }
        IEnumerator CheckAuth()
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

            if (string.IsNullOrEmpty(macAddress) || string.IsNullOrEmpty(serialKey) || string.IsNullOrEmpty(validToDate))
                message = "라이센스가 존재하지 않습니다.\n잠시후 자동으로 종료됩니다.";
            else if (!CheckMacAddress(macAddress))
                message = "라이센스가 올바르지 않습니다.\n잠시후 자동으로 종료됩니다.";
            else if(!System.DateTime.TryParse(validToDate, out validToDateObj))
                message = "라이센스가 올바르지 않습니다.\n잠시후 자동으로 종료됩니다.";
            else
            {
                // 라이센스 온라인 조회 (optional)
                string url = string.Format("{0}?authKey={1}&macAddress={2}", CheckAuthURL, AESEncrypt(serialKey), AESEncrypt(macAddress));
#if UNITY_EDITOR
                Debug.Log("AES(serialKey) : " + AESEncrypt(serialKey));
                Debug.Log("AES(macAddress) : " + AESEncrypt(macAddress));
#endif
                WWW www = new WWW(url);
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
#if UNITY_EDITOR
                    Debug.Log(www.text);
#endif
                    try
                    {
                        CheckAuthResult result = JsonUtility.FromJson<CheckAuthResult>(www.text);
                        if (result.result)
                        {
                            validToDate = result.validToDate;
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
                            message = string.Format("라이센스가 유효하지 않습니다.\n({0})\n잠시후 자동으로 종료됩니다.", result.msg);
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
                        message = "라이센스가 만료되었습니다.\n잠시후 자동으로 종료됩니다.";
                    }
                }
            }

            text.text = message;
            if (isPlayable)
                StartCoroutine(LicenseCheckComplete());
            else
                StartCoroutine(QuitProgram());
        }
        bool CheckMacAddress(string macAddress)
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
        string AESEncrypt(string input)
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
