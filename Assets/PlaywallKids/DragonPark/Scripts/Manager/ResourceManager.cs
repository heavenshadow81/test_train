using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using ML.PlaywallKids.DragonPark;

/// <summary>
/// This class manages the user-generated objects like <seealso cref="Template3D"/> or <seealso cref="ML.PlaywallKids.Aquarium.exAtoolFath"/>.
/// All resources are saved in %APPDATA%/Mogencelab/SmartBigboard/Resources folder.
/// </summary>
public partial class ResourceManager : MonoBehaviour
{
    #region Structs
    public struct IdentifierInfo
    {
        public string identifier;
        public int userId;
        public int userSeq;
    }
    #endregion

    #region Properties
    public static string dataRootPath
    {
        get { return ML.PlaywallKids.Common.CommonPath.GetDataRootPath(); }
    }
    #endregion

    #region Constants
    private const string kDirectoryNameTemplate = "Templates";
    #endregion

    /// <summary>
    /// Returns the path of resource directory. (%APPDATA%/Mogencelab/SmartBigboard/Resources)
    /// </summary>
    public static string GetResourcePath()
    {
        string path = Path.Combine(dataRootPath, "Resources").Replace("\\", "/");
        if (!Directory.Exists(path))
        {
            try
            {
                DirectoryInfo info = Directory.CreateDirectory(path);
                Debug.Log("ResourceManager.GetResourcePath() : Created directory at " + info.FullName);
            }
            catch { }
        }
        return path;
    }

    public static string GetTemplatePath()
    {
        string path = GetResourcePath();
        path = Path.Combine(path, kDirectoryNameTemplate).Replace("\\", "/");

        try
        {
            if (!Directory.Exists(path))
            {
                DirectoryInfo info = Directory.CreateDirectory(path);
                Debug.Log("ResourceManager.GetTemplatePath() : Created directory at " + info.FullName);
            }
        }
        catch { }

        return path;
    }

    public static string GetRecentTemplate3DIdentifier(int userSeq, int userId)
    {
        string identifier = "";
        string path = GetTemplatePath();
        
        try
        {
            string[] subdir = Directory.GetDirectories(path);

            DateTime creationTime = DateTime.Parse("1990-12-31");

            foreach (string subpath in subdir)
            {
                DateTime newTime = Directory.GetCreationTime(subpath);
                if (newTime > creationTime)
                {
                    string infoText = Path.Combine(subpath, "info.txt");
                    string jsonText = File.ReadAllText(infoText);
                    Dictionary<string, object> dict = MiniJSON.Json.Deserialize(jsonText) as Dictionary<string, object>;

                    int templateUserSeq = 0;
                    int templateUserId = 0;
                    if (dict.ContainsKey("user_seq") && dict["user_seq"] != null)
                    {
                        templateUserSeq = int.Parse(dict["user_seq"].ToString());
                    }
                    if (dict.ContainsKey("user_id") && dict["user_id"] != null)
                    {
                        templateUserId = int.Parse(dict["user_id"].ToString());
                    }

                    if (templateUserSeq != 0)
                    {
                        if (templateUserSeq == userSeq)
                        {
                            identifier = Path.GetFileName(subpath);
                            creationTime = newTime;
                        }
                    }
                    else if (templateUserId == userId)
                    {
                        identifier = Path.GetFileName(subpath);
                        creationTime = newTime;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("ResourceManager.GetRecentTemplate3DIdentifier() : exception occured.");
            Debug.Log(e);
        }

        return identifier;
    }

    /// <summary>
    /// Returns the list of the template 3d identifiers that recently was created.
    /// </summary>
    /// <param name="userSeq">User Sequence(0 : All)</param>
    /// <param name="userId">User ID(-1 : All users)</param>
    /// <param name="maxNumberOfIdentifier">Maximum number of identifiers. (total <= 50)</param>
    public static IdentifierInfo[] GetRecentTemplate3DIdentifiers(int userSeq = 0, int userId = -1, int maxNumberOfIdentifier = 20)
    {
        int MAX_NUM = 50;
        if (maxNumberOfIdentifier <= 0 || maxNumberOfIdentifier > MAX_NUM) maxNumberOfIdentifier = MAX_NUM;

        string path = GetTemplatePath();
        List<IdentifierInfo> list = new List<IdentifierInfo>();

        try
        {
            string[] subdirs = Directory.GetDirectories(path, "*-*-*-*-*", SearchOption.TopDirectoryOnly);
            Array.Sort(subdirs, (a, b) =>
            {
                // sort the directory list by creation date in descending order.
                DateTime d1 = Directory.GetCreationTime(a);
                DateTime d2 = Directory.GetCreationTime(b);
                return d2.CompareTo(d1);
            });

            for (int i = 0; i < subdirs.Length; i++)
            {
                try
                {
                    // Parse data from info.txt
                    string infoText = Path.Combine(subdirs[i], "info.txt").Replace("\\", "/");
                    string jsonText = File.ReadAllText(infoText);
                    Dictionary<string, object> dict = MiniJSON.Json.Deserialize(jsonText) as Dictionary<string, object>;

                    int templateUserSeq = 0;
                    int templateUserId = 0;
                    string characterName = "";
                    if (dict.ContainsKey("user_seq") && dict["user_seq"] != null)
                    {
                        templateUserSeq = int.Parse(dict["user_seq"].ToString());
                    }
                    if (dict.ContainsKey("user_id") && dict["user_id"] != null)
                    {
                        templateUserId = int.Parse(dict["user_id"].ToString());
                    }
                    if (dict.ContainsKey("character") && dict["character"] != null)
                        characterName = dict["character"].ToString();

                    // userSeq, userId, characterName check
                    if ((userSeq == 0 || userSeq == templateUserSeq) &&
                        (userId < 0 || userId == templateUserId) &&
                        Array.Exists(Dragon.characterNames, c => c.Equals(characterName)))
                    {
                        IdentifierInfo info = new IdentifierInfo();
                        info.userId = templateUserId;
                        info.userSeq = templateUserSeq;
                        info.identifier = Path.GetFileNameWithoutExtension(subdirs[i]);
                        list.Add(info);
                        if (list.Count >= maxNumberOfIdentifier)
                            break;
                    }
                }
                catch (IOException e)
                {
                    Debug.LogError("ResourceManager.GetRecentTemplate3DIdentifiers() : raised IO error.");
                    Debug.LogException(e);
                }
            }
        }
        catch (IOException e)
        {
            Debug.LogError("ResourceManager.GetRecentTemplate3DIdentifiers() : raised IO error.");
            Debug.LogException(e);
        }

        return list.ToArray();
    }

    public static void SaveTemplate3D(Template3D template, bool savesTextureInJson = true)
    {
        string path = GetTemplatePath();
        path = Path.Combine(path, template.identifier).Replace("\\", "/");
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }

        // create directory for template
        Directory.CreateDirectory(path);

        // save infos
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("character", template.character);
        dict.Add("user_id", template.userId);
        dict.Add("user_seq", template.userSeq);

        // additional infos
        Dictionary<string, object> boneScaleDict = new Dictionary<string, object>();
        Dragon dragon = template.GetComponent<Dragon>();
        if (dragon != null)
        {
            // bone scales
            Dictionary<string, float> boneScales = dragon.GetBoneScales();
            foreach (string key in boneScales.Keys)
            {
                boneScaleDict[key] = boneScales[key];
            }
            dict.Add("bone_scales", boneScaleDict);

            // accessories
            List<string> accessories = dragon.GetAccessoryNames();
            List<object> accessoriesObjList = new List<object>();
            foreach (string value in accessories)
            {
                accessoriesObjList.Add(value);
            }
            dict.Add("accessories", accessoriesObjList);
        }

        // packs the brush texture as PNG
        byte[] pngData = null;
        if (template.brushTexture is RenderTexture)
        {
            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = (RenderTexture)template.brushTexture;

            Texture2D texture = new Texture2D(template.brushTexture.width, template.brushTexture.height, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0, 0, template.brushTexture.width, template.brushTexture.height), 0, 0);
            texture.Apply();

            pngData = texture.EncodeToPNG();
            Destroy(texture);

            RenderTexture.active = prev;
        }
        else if (template.brushTexture is Texture2D)
        {
            pngData = ((Texture2D)template.brushTexture).EncodeToPNG();
        }
        else
        {
            Debug.LogError("ResourceManager.SaveTemplate3D() : Invalid texture type.");
            return;
        }

        if (!savesTextureInJson)
        {
            // save as png file
            File.WriteAllBytes(Path.Combine(path, "brush_tex.png"), pngData);
        }
        else
        {
            // identifier
            dict.Add("identifier", template.identifier);

            // base64 brush texture
            string base64PngStr = Convert.ToBase64String(pngData);
            dict.Add("brush_tex_base64", base64PngStr);
        }

        // save to text file
        string jsonStr = MiniJSON.Json.Serialize(dict);
        File.WriteAllText(Path.Combine(path, "info.txt"), jsonStr);
    }

    public static Template3D LoadTemplate3D(string identifier)
    {
        string path = GetTemplatePath();
        path = Path.Combine(path, identifier).Replace("\\", "/");

        if (Directory.Exists(path))
        {
            Template3D template = null;

            try
            {
                string jsonStr = File.ReadAllText(Path.Combine(path, "info.txt"));
                Dictionary<string, object> dict = MiniJSON.Json.Deserialize(jsonStr) as Dictionary<string, object>;

                string character = dict["character"].ToString();
                int userId = 0;
                int userSeq = 0;
                if (dict.ContainsKey("user_id") && dict["user_id"] != null)
                {
                    userId = int.Parse(dict["user_id"].ToString());
                }
                if (dict.ContainsKey("user_seq") && dict["user_seq"] != null)
                {
                    userSeq = int.Parse(dict["user_seq"].ToString());
                }

                GameObject prefab = Resources.Load("Dragons/" + character, typeof(GameObject)) as GameObject;

                GameObject go = Instantiate(prefab) as GameObject;
                go.name = string.Format("Template({0}, {1})", character, identifier);
                go.transform.localPosition = new Vector3(0.0f, -0.78f, 0.0f);
                template = go.AddComponent<Template3D>();
                template.character = character;
                template.userId = userId;
                template.userSeq = userSeq;
                template.identifier = identifier;

                // brush texture
                byte[] imageBytes = null;
                if (dict.ContainsKey("brush_tex_base64") && dict["brush_tex_base64"] != null)
                {
                    string base64PngStr = dict["brush_tex_base64"].ToString();
                    imageBytes = Convert.FromBase64String(base64PngStr);
                    base64PngStr = null;
                }
                else
                {
                    imageBytes = File.ReadAllBytes(Path.Combine(path, "brush_tex.png"));
                }
                if (imageBytes != null)
                {
                    template.LoadBrushTexture(imageBytes);
                }

                Dragon dragon = template.GetComponent<Dragon>();
                if (dragon == null)
                {
                    dragon = go.AddComponent<Dragon>();
                    dragon.characterName = character;
                }

                if (dragon != null)
                {
                    Dictionary<string, object> boneScales = dict["bone_scales"] as Dictionary<string, object>;
                    if (boneScales != null)
                    {
                        foreach (string key in boneScales.Keys)
                        {
                            float val = 1.0f;
                            object obj = boneScales[key];
                            if (obj != null)
                            {
                                float.TryParse(obj.ToString(), out val);
                            }
                            dragon.SetBoneScale(key, val);
                        }
                    }

                    if (dict.ContainsKey("accessories"))
                    {
                        List<object> accessories = dict["accessories"] as List<object>;
                        foreach (object obj in accessories)
                        {
                            string name = obj.ToString();

                            GameObject accessoryPrefab = AccessoryManager.Get(name);
                            if (accessoryPrefab != null)
                            {
                                GameObject accessory = (GameObject)Instantiate(accessoryPrefab);
                                accessory.name = name;
                                dragon.SetAccessory(accessory, true);
                            }
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Debug.Log("ResourceManager.LoadTemplate3D() : raised IO error.");
                Debug.Log(e);
            }
            catch (Exception e)
            {
                Debug.Log("ResourceManager.LoadTemplate3D() : error occured.");
                Debug.Log(e);
            }

            return template;
        }

        return null;
    }

    /// <summary>
    /// Removes old Template3D folders.
    /// </summary>
    public static void RemoveOldTemplate3Ds()
    {
        string path = GetTemplatePath();
        DateTime now = DateTime.Now;
        int numDeleted = 0;
        
        try
        {
            string[] subdirs = Directory.GetDirectories(path, "*-*-*-*-*", SearchOption.TopDirectoryOnly);
            Array.Sort(subdirs, (a, b) =>
            {
                // sort the directory list by creation date in descending order.
                DateTime d1 = Directory.GetCreationTime(a);
                DateTime d2 = Directory.GetCreationTime(b);
                return d2.CompareTo(d1);
            });

            int maxNum = 500;
            if (subdirs.Length > maxNum)
            {
                for (int i = maxNum; i < subdirs.Length; i++)
                {
                    string dir = subdirs[i];
                    Directory.Delete(dir, true);
                    numDeleted++;
                }
            }
        }
        catch (IOException e)
        {
            Debug.Log("ResourceManager.RemoveOldTemplate3Ds() : error occured.");
            Debug.Log(e);
        }

        if (numDeleted > 0)
            Debug.Log(string.Format("ResourceManager.RemoveOldTemplate3Ds() : Deleted {0} old template 3Ds.", numDeleted));
    }
}