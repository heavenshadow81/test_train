using System.IO;
using UnityEngine;

namespace ML.PlaywallKids.Common
{
    public static class CommonPath
    {
        /// <summary>
        /// Returns application data path. (Windows : AppData\Roaming\Mogencelab\SmartBigboard)
        /// </summary>
        public static string GetDataRootPath()
        {
            string appDataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            string path1 = Path.Combine(appDataPath, "BAX").Replace("\\", "/");
            if (!Directory.Exists(path1))
            {
                try
                {
                    DirectoryInfo info = Directory.CreateDirectory(path1);
                    Debug.Log("CommonPath.GetDataRootPath() : Created directory at " + info.FullName);
                }
                catch (IOException e)
                {
                    Debug.LogException(e);
                }
            }

            string path2 = Path.Combine(path1, "SmartBigboard").Replace("\\", "/");
            if (!Directory.Exists(path2))
            {
                try
                {
                    DirectoryInfo info = Directory.CreateDirectory(path2);
                    Debug.Log("CommonPath.GetDataRootPath() : Created directory at " + info.FullName);
                }
                catch (IOException e)
                {
                    Debug.LogException(e);
                }
            }

            return path2;
        }
    }
}
