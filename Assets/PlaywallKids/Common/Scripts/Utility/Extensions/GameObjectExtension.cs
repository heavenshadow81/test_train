using UnityEngine;
using System.Collections;

/// <summary>
/// GameObject class extension.
/// </summary>
public static class GameObjectExtension
{
    public static void SetLayerRecursively(this GameObject go, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        SetLayerRecursively(go, layer);
    }

    public static void SetLayerRecursively(this GameObject go, string newLayerName, string targetLayerName)
    {
        int newLayer = LayerMask.NameToLayer(newLayerName);
        int targetLayer = LayerMask.NameToLayer(targetLayerName);
        SetLayerRecursively(go, newLayer, targetLayer);
    }

    public static void SetLayerRecursively(this GameObject go, int newLayer)
    {
        SetLayerRecursively(go, newLayer, -1);
    }

    public static void SetLayerRecursively(this GameObject go, int newLayer, int targetLayer)
    {
        // if given layer is invalid, set layer as default id(0).
        if (newLayer < 0) newLayer = 0;

        // get all transforms of game object
        Transform[] tfs = go.GetComponentsInChildren<Transform>();

        // loop each transform
        foreach (Transform t in tfs)
        {
            // if target layer is not specified (-1), or target layer is equal to current game object's layer
            if (targetLayer < 0 || targetLayer == t.gameObject.layer)
            {
                // set layer as given layer id.
                t.gameObject.layer = newLayer;
            }
        }

        // set root layer as given layer id.
        go.layer = newLayer;
    }

    public static T GetOrAddComponent<T>(this GameObject go) where T : MonoBehaviour
    {
        T comp = go.GetComponent<T>();
        if (comp == null)
            comp = go.AddComponent<T>();
        return comp;
    }
}