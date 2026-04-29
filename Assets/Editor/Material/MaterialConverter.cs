using UnityEngine;
using UnityEditor;

public class MaterialConverter : Editor
{
    [MenuItem("Tools/Convert Autodesk Interactive Materials to Standard")]
    public static void ConvertMaterialsToStandard()
    {
        string[] guids = AssetDatabase.FindAssets("t:Material");
        int convertedCount = 0; // 변환된 재질 개수를 저장할 변수

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

            // Autodesk Interactive 셰이더를 Standard 셰이더로 변환
            if (material.shader.name == "Autodesk Interactive")
            {
                material.shader = Shader.Find("Standard");
                convertedCount++; // 변환된 경우 개수를 증가
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"All Autodesk Interactive materials converted to Standard. Total converted: {convertedCount}");
    }
}
