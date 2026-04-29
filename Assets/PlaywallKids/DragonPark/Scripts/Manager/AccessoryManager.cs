using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Accessory manager.
/// It loads all accessories from Resources folder.
/// Dragon Accessories : <c>Assets/Resources/Accessory</c>
/// Avatar Accessories : <c>Assets/Resources/AvatarAccessory</c>
/// And easily get dummy, accessory name, mesh object and so on.
/// Simple. :-)
/// </summary>
public class AccessoryManager : MonoBehaviour {
	#region Properties
    private static Dictionary<AccessoryType, GameObject[]> _list = null;
    public static Dictionary<AccessoryType, GameObject[]> list
    {
		get {
			_Load();

			return _list;
		}
	}

	private static GameObject[] _avatarAccessories = null;
	public static GameObject[] avatarAccessories {
		get {
			_Load();

			return _avatarAccessories;
		}
	}
	#endregion

	#region Private variables
	private static Dictionary<string, GameObject> _accessoryDict = new Dictionary<string, GameObject>();
	#endregion


    public enum AccessoryType { Head, Ribbon, Backpack };
	#region Constants
	// const variable
	private const string kRootPath = "./Resources/";
    private const string kDirectoryNameAccessory = "Accessory/";
    private const string kDirectoryNameAvatarAccessory = "AvatarAccessory";

    private static string[] kDirectoryNameAccessoryType = new string[] { "Head", "Ribbon", "Backpack" };
	#endregion

    public static void Init()
    {
        _Load();
    }

    private static void _Load()
    {
        if (_list != null) return;

        _list =  new Dictionary<AccessoryType, GameObject[]>();
        
        _accessoryDict.Clear();

        // Dragon Accessories
        for (int i = 0; i < kDirectoryNameAccessoryType.Length; i++)
        {
            GameObject[] prefabs = _LoadAt(kDirectoryNameAccessory + kDirectoryNameAccessoryType[i]);
            if (_list.ContainsKey((AccessoryType)i) == false)
                _list.Add((AccessoryType)i, prefabs);
        }

        // Avatar Accessories
        _avatarAccessories = _LoadAt(kDirectoryNameAvatarAccessory);
    }

	private static GameObject[] _LoadAt(string directoryName) {
		Object[] objs = Resources.LoadAll(directoryName, typeof(GameObject));
		
		List<GameObject> list = new List<GameObject>();
		
		foreach(Object obj in objs) {
			GameObject go = obj as GameObject;
			if(go != null) {
				string name = go.name;
				_accessoryDict[_GetSafeAccessoryName(name)] = go;
				list.Add(go);
			}
		}
		
		return list.ToArray();
	}


	public static GameObject Get(string name) {
		_Load();

		GameObject go = null;

		_accessoryDict.TryGetValue(_GetSafeAccessoryName(name), out go);

		return go;
	}

	public static string GetAccessoryName(GameObject go) {
		if(go != null) {
			return _GetSafeAccessoryName(go.name.Replace("(Clone)", ""));
		}

		return "";
	}

	private static string _GetSafeAccessoryName(string name) {
		if(name.Length > 3) {
			int val = 0;
			int.TryParse(name.Substring(0, 2), out val);
			if(val > 0 && val < 100) {
				string newName = name.Remove(0, 3);
				return newName;
			}
		}
		return name;
	}

	public static Transform GetDummy(GameObject accessory) {
		return GetDummy(accessory, null);
	}

    public static Transform GetDummy(GameObject accessory, string characterName)
    {
		if(accessory != null) {
			GameObject mesh = GetMeshObject(accessory);
            if (characterName.Contains("Everland"))
                characterName = "Pico";

			for(int i = 0; i < accessory.transform.childCount; i++) {
				Transform t = accessory.transform.GetChild(i);
				if(t.gameObject != mesh) {
					if(string.IsNullOrEmpty(characterName)) {
						return t;
					}
					else if(t.name.Contains(characterName)) {
						return t;
					}
				}
			}
		}

		return null;
	}

	public static GameObject GetMeshObject(GameObject accessory) {
		GameObject go = null;
		if(accessory != null) {
			MeshRenderer[] mrs = accessory.GetComponentsInChildren<MeshRenderer>(true);
			if(mrs == null || mrs.Length == 0) {
				SkinnedMeshRenderer[] smrs = accessory.GetComponentsInChildren<SkinnedMeshRenderer>(true);
				if(smrs != null && smrs.Length > 0) {
					go = smrs[0].gameObject;
				}
			}
			else {
				go = mrs[0].gameObject;
			}
		}
		return go;
	}
}
