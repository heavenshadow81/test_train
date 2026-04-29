using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "SceneList", menuName = "ScriptableObjects/SceneList")]
public class SceneListScriptableObject : ScriptableObject
{
    public List<string> scenes = new List<string>();

    private void OnValidate()
    {
        RefreshSceneList();
    }

    public void RefreshSceneList()
    {
        scenes.Clear();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                scenes.Add(System.IO.Path.GetFileNameWithoutExtension(scene.path));
            }
        }
    }
}
