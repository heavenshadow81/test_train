using UnityEngine;
using System.Net.NetworkInformation;
using System.IO;
using System.Text;
using Microsoft.Win32;

public class MACCertified : MonoBehaviour {

    // Use this for initialization  {'M', 'O', 'G', 'E', 'N', 'C', 'E', 'L', 'A', 'B', 'R', 'D'}
    private static byte[] Key = new byte[] { 77, 79, 71, 69, 78, 67, 69, 76, 65, 66, 82, 68 };
    private string Mac_Add = "";

    public static bool IsPlayable()
    {
        try
        {
            if (File.Exists("License.mgl")){
                FileStream fs = new FileStream("License.mgl", FileMode.Open, FileAccess.Read);

                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Close();
                //for(int i=0; i<bytes.Length;i+=2)
                //    Debug.Log(bytes[i].ToString());   

                string subkey = "SOFTWARE\\MGLab";

                RegistryKey key = Registry.CurrentUser.OpenSubKey(subkey, true);

                if (key == null)
                {
                    /////exception or Destroy
                    ;
                }

                // get
                string data = (string)key.GetValue("MAC_CODE");
                bool Mac_Check = false;
                for (int m = 0; m < NetworkInterface.GetAllNetworkInterfaces().GetLength(0); m++)
                {
                    string Mac_Code = NetworkInterface.GetAllNetworkInterfaces()[m].GetPhysicalAddress().ToString();
                    if (data == Mac_Code)
                        Mac_Check = true;
                    //Debug.Log(Mac_Code.ToString());
                }
                if (Mac_Check)
                    Debug.Log(data.ToString());
                else
                    return false;
                key.Close();

                bool Check = true;
                byte[] chk_bytes = new byte[data.Length];
                //Debug.Log(data + "//////" + Mac_Add);
                for (int i = 0; i < data.Length; i++)
                {
                    chk_bytes = Encoding.UTF8.GetBytes(data);
                    byte tmp = (byte)(chk_bytes[i] ^ Key[i]);
                    //Debug.Log(tmp.ToString());
                    if (bytes[i * 2] != tmp)
                        Check = false;
                }              

                if (Check)
                {
                    Debug.Log("Loading Program");
                    return true;
                }
                else
                    return false;
            }
        }
        catch
        {
            Debug.Log("File Not Loading");
        }

        return false;
    }
}
