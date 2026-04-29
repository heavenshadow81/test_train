using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ML.PlaywallKids.Aquarium;
using ML.PlaywallKids.Common;

namespace ML.PlaywallKids.Aquarium
{
    public class AToolFishInfo
    {
        public string identifier = "";
        public string templateName = "";
        public int userId = 0;
        public List<Texture> textures = new List<Texture>();
        public Texture printTexture = null;
    }
}
public partial class ResourceManager
{
    #region Constants
    private const string kDirectoryNameAquarium = "Aquarium";
    private const int kMaxNumAquariumFish = 500;
    #endregion

    public static string GetAquariumPath()
    {
        string path = GetResourcePath();
        path = Path.Combine(path, kDirectoryNameAquarium).Replace("\\", "/");

        try
        {
            if (!Directory.Exists(path))
            {
                DirectoryInfo info = Directory.CreateDirectory(path);
                Debug.Log("ResourceManager.GetAquariumPath() : Created directory at " + info.FullName);
            }
        }
        catch { }
        print($"아쿠아리움 패스: {path}");
        return path;
    }
    
    public static string GetRecentAToolFishIdentifier(int userId)
    {
        string path = GetAquariumPath();

        //path = Path.Combine(path, userId.ToString()).Replace("\\", "/");

        string identifier = null;
        try
        {
            string[] subdirs = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
            DateTime creationTime = new DateTime(1991, 1, 1);
            for (int i = 0; i < subdirs.Length; i++)
            {
                DateTime dt = Directory.GetCreationTime(subdirs[i]);
                if (creationTime < dt)
                {
                    creationTime = dt;
                    identifier = Path.GetFileNameWithoutExtension(subdirs[i]);
                }
            }
        }
        catch (IOException e)
        {
            Debug.LogError("ResourceManager.GetRecentAToolFishIdentifier() : raised IO error.");
            Debug.LogException(e);
        }
        return identifier;
    }

    /// <summary>
    /// Returns the list of the fish template identifiers that recently was created.
    /// </summary>
    /// <param name="userId">User ID(-1 : All users)</param>
    /// <param name="maxNumberOfIdentifier">Maximum number of identifiers.</param>
    public static IdentifierInfo[] GetRecentAToolFishIdentifiers(int userId = -1, int maxNumberOfIdentifier = 20)
    {
        int MAX_NUM = kMaxNumAquariumFish;
        if (maxNumberOfIdentifier <= 0 || maxNumberOfIdentifier > MAX_NUM) maxNumberOfIdentifier = MAX_NUM;

        string path = GetAquariumPath();
        List<IdentifierInfo> list = new List<IdentifierInfo>();

        for (int i = 0; i < CommonSettings.maxUserCount; i++)
        {
            if (userId < 0 || userId == i)
            {
                string userPath = Path.Combine(path, i.ToString()).Replace("\\", "/");
                if (Directory.Exists(userPath))
                {
                    try
                    {
                        string[] subdirs = Directory.GetDirectories(userPath, "*-*-*-*-*", SearchOption.TopDirectoryOnly);
                        if (subdirs.Length > MAX_NUM / 2)
                        {
                            Array.Sort(subdirs, (a, b) =>
                            {
                                DateTime d1 = Directory.GetCreationTime(a.Replace("\\", "/"));
                                DateTime d2 = Directory.GetCreationTime(b.Replace("\\", "/"));
                                return d2.CompareTo(d1);
                            });
                        }

                        for (int j = 0; j < Math.Min(subdirs.Length, MAX_NUM / 2); j++)
                        {
                            IdentifierInfo info = new IdentifierInfo();
                            info.userId = j;
                            info.userSeq = 0;
                            info.identifier = subdirs[j].Replace("\\", "/");
                            list.Add(info);
                        }
                    }
                    catch (IOException e)
                    {
                        Debug.LogError("ResourceManager.GetRecentAToolFishIdentifiers() : raised IO error.");
                        Debug.LogException(e);
                    }
                }
            }
        }

        list.Sort((a, b) =>
        {
            DateTime d1 = Directory.GetCreationTime(a.identifier);
            DateTime d2 = Directory.GetCreationTime(b.identifier);
            return d2.CompareTo(d1);
        });

        if (list.Count > maxNumberOfIdentifier)
            list.RemoveRange(maxNumberOfIdentifier, list.Count - maxNumberOfIdentifier);

        for (int i = 0; i < list.Count; i++)
        {
            IdentifierInfo info = list[i];
            info.identifier = Path.GetFileNameWithoutExtension(info.identifier);
            list[i] = info;
        }

        return list.ToArray();
    }

    public static void SaveAToolFish(AToolFishInfo info)
    {
        if (info == null)
        {
            Debug.LogError("ResourceManager.SaveAToolFish() : AToolFish info is null.");
            return;
        }

        if (string.IsNullOrEmpty(info.identifier))
        {
            Debug.LogError("ResourceManager.SaveAToolFish() : You must specify the unique identifier.");
            return;
        }

        // info.txt
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict["identifier"] = info.identifier;
        dict["template_name"] = info.templateName;
        dict["user_id"] = info.userId;
        string jsonStr = MiniJSON.Json.Serialize(dict);

        // textures
        List<byte[]> textureBytesList = new List<byte[]>();
        for (int i = 0; i < info.textures.Count; i++)
        {
            Texture texture = info.textures[i];
            if (texture is Texture2D)
            {
                Texture2D tex2D = texture as Texture2D;
                textureBytesList.Add(tex2D.EncodeToPNG());
            }
            else if (texture is RenderTexture)
            {
                // RenderTexture -> Texture2D
                Texture2D outTex = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, true, true);
                RenderTexture prevActive = RenderTexture.active;
                RenderTexture.active = (RenderTexture)texture;
                outTex.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
                RenderTexture.active = prevActive;
                outTex.Apply();
                textureBytesList.Add(outTex.EncodeToPNG());
                Destroy(outTex);
            }
        }

        // print texture
        byte[] printTextureBytes = null;
        if (info.printTexture != null)
        {
            if (info.printTexture is Texture2D)
            {
                Texture2D tex2D = info.printTexture as Texture2D;
                printTextureBytes = tex2D.EncodeToPNG();
            }
            else if (info.printTexture is RenderTexture)
            {
                // RenderTexture -> Texture2D
                Texture2D outTex = new Texture2D(info.printTexture.width, info.printTexture.height, TextureFormat.ARGB32, true, true);
                RenderTexture prevActive = RenderTexture.active;
                RenderTexture.active = (RenderTexture)info.printTexture;
                outTex.ReadPixels(new Rect(0, 0, info.printTexture.width, info.printTexture.height), 0, 0);
                RenderTexture.active = prevActive;
                outTex.Apply();
                printTextureBytes = outTex.EncodeToPNG();
                Destroy(outTex);
            }
        }

        // %RESOURCE_PATH%/Aquarium/USER_ID/UUID
        string path = GetAquariumPath();
        
        
        path = Path.Combine(path, info.userId.ToString());
        path = Path.Combine(path, info.identifier);
        path = path.Replace("\\", "/");
        print($"기본 파일 저장 경로{path}");
        try
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists) dir.Create();

            File.WriteAllText(Path.Combine(path, "info.txt").Replace("\\", "/"), jsonStr);
            for (int i = 0; i < textureBytesList.Count; i++)
            {
                File.WriteAllBytes(Path.Combine(path, string.Format("{0}.png", i + 1)).Replace("\\", "/"), textureBytesList[i]);
            }
                
            if (printTextureBytes != null)
            {
                string printImagePath = Path.Combine(path, "print.png").Replace("\\", "/");
                File.WriteAllBytes(printImagePath, printTextureBytes);

                // Copy the image file to "print" directory (to be compatible with legacy codes)
                string printImageCompatibilityPath = GetAquariumPath();
                printImageCompatibilityPath = Path.Combine(printImageCompatibilityPath, "print").Replace("\\", "/");
                if (!Directory.Exists(printImageCompatibilityPath))
                    Directory.CreateDirectory(printImageCompatibilityPath);
                printImageCompatibilityPath = Path.Combine(printImageCompatibilityPath, string.Format("{0}.png", info.userId)).Replace("\\", "/");
                if (File.Exists(printImageCompatibilityPath))
                    File.Delete(printImageCompatibilityPath);
                File.Copy(printImagePath, printImageCompatibilityPath);
            }
        }
        catch (IOException e)
        {
            Debug.LogError("ResourceManager.SaveAToolFish() : Failed to save the information due to IO error.");
            Debug.LogException(e);
        }
    }

    public static AToolFishInfo LoadAToolFish(string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            Debug.LogError("ResourceManager.LoadAToolFish() : identifier is null or empty.");
            return null;
        }
        string identifierShort = identifier.Substring(0, Mathf.Min(8, identifier.Length));
        AToolFishInfo info = null;

        string path = GetAquariumPath();
        try
        {
            string[] subdirs = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
            foreach (string subdir in subdirs)
            {
                path = Path.Combine(subdir, identifier).Replace("\\", "/");
                if (Directory.Exists(path))
                {
                    info = new AToolFishInfo();

                    // info.txt
                    string jsonStr = File.ReadAllText(Path.Combine(path, "info.txt").Replace("\\", "/"));
                    Dictionary<string, object> dict = MiniJSON.Json.Deserialize(jsonStr) as Dictionary<string, object>;
                    if (dict.ContainsKey("user_id") && dict["user_id"] != null)
                        int.TryParse(dict["user_id"].ToString(), out info.userId);
                    if (dict.ContainsKey("identifier") && dict["identifier"] != null)
                        info.identifier = dict["identifier"].ToString();
                    if (dict.ContainsKey("template_name") && dict["template_name"] != null)
                        info.templateName = dict["template_name"].ToString();

                    // textures
                    for (int texId = 1; texId <= 10; texId++)
                    {
                        string texPath = Path.Combine(path, string.Format("{0}.png", texId)).Replace("\\", "/");
                        if (!File.Exists(texPath)) break;
                        byte[] texBytes = File.ReadAllBytes(texPath);
                        Texture2D tex = new Texture2D(4, 4);
                        tex.LoadImage(texBytes);
                        tex.Apply();
                        tex.name = string.Format("{0}_{1}", identifierShort, texId + 1);
                        info.textures.Add(tex);
                    }

                    // print texture
                    string printTexPath = Path.Combine(path, "print.png").Replace("\\", "/");
                    if (File.Exists(printTexPath))
                    {
                        byte[] texBytes = File.ReadAllBytes(printTexPath);
                        Texture2D tex = new Texture2D(4, 4);
                        tex.LoadImage(texBytes);
                        tex.Apply();
                        tex.name = string.Format("{0}_print", identifierShort);
                        info.printTexture = tex;
                    }

                    break;
                }
            }
        }
        catch (IOException e)
        {
            Debug.LogError("ResourceManager.LoadAToolFish() : raised IO error.");
            Debug.LogException(e);
            info = null;
        }

        return info;
    }

    /// <summary>
    /// Releases system memories in AToolFishInfo.
    /// </summary>
    public static void ReleaseAToolFishInfo(AToolFishInfo info)
    {
        if (info == null)
        {
            Debug.LogWarning("ResourceManager.ReleaseAToolFishInfo() : info is null.");
            return;
        }
        
        for (int i = 0; i < info.textures.Count; i++)
        {
            Texture t = info.textures[i];
            if (t != null)
            {
                if (t is RenderTexture)
                    ((RenderTexture)t).Release();
                Destroy(t);
            }
        }

        if (info.printTexture != null)
        {
            if (info.printTexture is RenderTexture)
                ((RenderTexture)info.printTexture).Release();
            Destroy(info.printTexture);
        }
    }

    /// <summary>
    /// Removes old aquarium template folders.
    /// </summary>
    public static void RemoveOldAquariumTemplates()
    {
        string path = GetAquariumPath();
        DateTime now = DateTime.Now;
        int numDeleted = 0;

        try
        {
            List<string> subdirs = new List<string>();
            for (int userId = 0; userId < CommonSettings.maxUserCount; userId++)
            {
                string userPath = Path.Combine(path, userId.ToString());
                string[] userSubDirs = Directory.GetDirectories(userPath, "*-*-*-*-*", SearchOption.TopDirectoryOnly);
                subdirs.AddRange(userSubDirs);
            }

            subdirs.Sort((a, b) =>
            {
                // sort the directory list by creation date in descending order.
                DateTime d1 = Directory.GetCreationTime(a);
                DateTime d2 = Directory.GetCreationTime(b);
                return d2.CompareTo(d1);
            });

            int maxNum = kMaxNumAquariumFish;
            if (subdirs.Count > maxNum)
            {
                for (int i = maxNum; i < subdirs.Count; i++)
                {
                    string dir = subdirs[i];
                    Directory.Delete(dir, true);
                    numDeleted++;
                }
            }
        }
        catch (IOException e)
        {
            Debug.Log("ResourceManager.RemoveOldAquariumTemplates() : error occured.");
            Debug.Log(e);
        }

        if (numDeleted > 0)
            Debug.Log(string.Format("ResourceManager.RemoveOldAquariumTemplates() : Deleted {0} old aquarium templates.", numDeleted));
    }
}
